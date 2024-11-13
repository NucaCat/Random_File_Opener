using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class MyFFMpegConverter
    {
        private Process _ffMpegProcess;
        private bool _ffMpegProcessWaitForAsyncReadersCompleted;
        // ReSharper disable once ReplaceAutoPropertyWithComputedProperty
        private static string FFMpegExeName { get; } = "ffmpeg.exe";
        private static string FFMpegToolPath { get; } = AppDomain.CurrentDomain.BaseDirectory;
        private static string WorkingDirectory { get; } = Path.GetDirectoryName(FFMpegToolPath);
        private static string FFMpegExePath { get; } = Path.Combine(FFMpegToolPath, FFMpegExeName);
        
        private static readonly ConcurrentQueue<StreamBuffer> _buffers = new ConcurrentQueue<StreamBuffer>(StreamBuffer.Get(500));
        private static readonly ConcurrentQueue<ProcessStartInfoCache> _processCaches = new ConcurrentQueue<ProcessStartInfoCache>(ProcessStartInfoCache.Get(500));

        public Stream GetVideoThumbnail(string inputFile, double frameTime)
        {
            var thumbnailStream = new MemoryStream();

            var input = new Media
            {
                Filename = inputFile
            };
            var output = new Media
            {
                Filename = Path.GetTempFileName(),
                DataStream = thumbnailStream,
                Format = "mjpeg"
            };
            ConvertMedia(input, output, frameTime);

            return thumbnailStream;
        }

        private void ConvertMedia(Media input, Media output, double frameTime)
        {
            if (!File.Exists(FFMpegExePath))
                return;

            _processCaches.TryDequeue(out var startInfo);
            if (startInfo is null)
                startInfo = new ProcessStartInfoCache();

            _buffers.TryDequeue(out var buffer);
            if (buffer is null)
                buffer = new StreamBuffer();

            try
            {
                var arguments = ComposeFFMpegCommandLineArgs(input.Filename, output.Filename, output.Format, frameTime);

                startInfo.ProcessStartInfo.Arguments = arguments;
                if (_ffMpegProcess != null)
                    throw new InvalidOperationException("FFMpeg process is already started");

                _ffMpegProcessWaitForAsyncReadersCompleted = false;
                _ffMpegProcess = Process.Start(startInfo.ProcessStartInfo);

                // ReSharper disable once PossibleNullReferenceException
                _ffMpegProcess.BeginOutputReadLine();
                _ffMpegProcess.BeginErrorReadLine();
                WaitFFMpegProcessForExit();
                if (_ffMpegProcess.ExitCode != 0)
                    throw new Exception(_ffMpegProcess.ExitCode.ToString());
                _ffMpegProcess.Close();
                _ffMpegProcess = null;

                using (var inputStream = new FileStream(output.Filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    CopyStream(inputStream, output.DataStream, buffer);
            }
            catch (Exception)
            {
                EnsureFFMpegProcessStopped();
            }
            finally
            {
                File.Delete(output.Filename);
                if (startInfo != null)
                    _processCaches.Enqueue(startInfo);
                
                if (buffer != null)
                    _buffers.Enqueue(buffer);
            }
        }

        private void CopyStream(Stream inputStream, Stream outputStream, StreamBuffer buffer)
        {
            int count;
            while ((count = inputStream.Read(buffer.Buffer, 0, buffer.Buffer.Length)) > 0)
                outputStream.Write(buffer.Buffer, 0, count);
        }

        private string ComposeFFMpegCommandLineArgs(
            string inputFile,
            string outputFile,
            string outputFormat,
            double frameTime)
        {
            var inputArgs = $" -ss {frameTime.ToString(CultureInfo.InvariantCulture)}";

            var outputArgs = $" -f {outputFormat} -vframes 1";

            return $"-y {inputArgs} -i \"{inputFile}\" {outputArgs} \"{outputFile}\"";
        }

        private void WaitFFMpegProcessForExit()
        {
            if (_ffMpegProcess == null)
                throw new Exception("FFMpeg process was aborted");
            if (_ffMpegProcess.HasExited)
                return;
            if (!_ffMpegProcess.WaitForExit(int.MaxValue))
            {
                EnsureFFMpegProcessStopped();
                throw new Exception("FFMpeg process was aborted");
            }
            var ffMpegProcess = _ffMpegProcess;
            if (ffMpegProcess == null)
                return;
            lock (ffMpegProcess)
            {
                if (ffMpegProcess != _ffMpegProcess || _ffMpegProcessWaitForAsyncReadersCompleted)
                    return;
                ffMpegProcess.WaitForExit();
                _ffMpegProcessWaitForAsyncReadersCompleted = true;
            }
        }

        private void EnsureFFMpegProcessStopped()
        {
            if (_ffMpegProcess == null)
                return;
            if (!_ffMpegProcess.HasExited)
            {
                try
                {
                    _ffMpegProcess.Kill();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            _ffMpegProcess = null;
        }

        private class Media
        {
            public string Filename { get; set; }

            public string Format { get; set; }

            public Stream DataStream { get; set; }
        }

        private class StreamBuffer
        {
            public byte[] Buffer { get; } = new byte[262144];

            public static List<StreamBuffer> Get(int count)
                => Enumerable.Range(0, count).Select(_ => new StreamBuffer()).ToList();
        }

        private class ProcessStartInfoCache
        {
            public ProcessStartInfo ProcessStartInfo { get; } = new ProcessStartInfo
            {
                FileName = FFMpegExePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            public static List<ProcessStartInfoCache> Get(int count)
                => Enumerable.Range(0, count).Select(_ => new ProcessStartInfoCache()).ToList();
        }
    }
}