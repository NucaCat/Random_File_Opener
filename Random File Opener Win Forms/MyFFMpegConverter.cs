using System;
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
        
        private static readonly StreamBuffer[] _buffers = StreamBuffer.Get(8);
        private static readonly ProcessStartInfoCache[] _processCaches = ProcessStartInfoCache.Get(8);

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

            try
            {
                var arguments = ComposeFFMpegCommandLineArgs(input.Filename, output.Filename, output.Format, frameTime);

                ProcessStartInfoCache startInfo;
                lock (_processCaches)
                {
                    startInfo = _processCaches.FirstOrDefault(u => !u.IsLocked);
                    if (startInfo != null)
                        startInfo.IsLocked = true;
                }

                if (startInfo == null)
                    startInfo = new ProcessStartInfoCache { IsLocked = true };

                startInfo.ProcessStartInfo.Arguments = arguments;
                if (_ffMpegProcess != null)
                    throw new InvalidOperationException("FFMpeg process is already started");

                _ffMpegProcessWaitForAsyncReadersCompleted = false;
                _ffMpegProcess = Process.Start(startInfo.ProcessStartInfo);

                // ReSharper disable once PossibleNullReferenceException
                _ffMpegProcess.BeginOutputReadLine();
                _ffMpegProcess.BeginErrorReadLine();
                WaitFFMpegProcessForExit();
                startInfo.IsLocked = false;
                if (_ffMpegProcess.ExitCode != 0)
                    throw new Exception(_ffMpegProcess.ExitCode.ToString());
                _ffMpegProcess.Close();
                _ffMpegProcess = null;

                using (var inputStream = new FileStream(output.Filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    CopyStream(inputStream, output.DataStream);
            }
            catch (Exception)
            {
                EnsureFFMpegProcessStopped();
                throw;
            }
            finally
            {
                File.Delete(output.Filename);
            }
        }

        private void CopyStream(Stream inputStream, Stream outputStream)
        {
            StreamBuffer buffer;
            lock (_buffers)
            {
                buffer = _buffers.FirstOrDefault(u => !u.IsLocked);
                if (buffer != null)
                    buffer.IsLocked = true;
            }

            if (buffer == null)
                buffer = new StreamBuffer{ IsLocked = true };

            int count;
            while ((count = inputStream.Read(buffer.Buffer, 0, buffer.Buffer.Length)) > 0)
                outputStream.Write(buffer.Buffer, 0, count);

            buffer.IsLocked = false;
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
            public bool IsLocked { get; set; }

            public static StreamBuffer[] Get(int count)
                => Enumerable.Range(0, count).Select(_ => new StreamBuffer()).ToArray();
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

            public bool IsLocked { get; set; }

            public static ProcessStartInfoCache[] Get(int count)
                => Enumerable.Range(0, count).Select(_ => new ProcessStartInfoCache()).ToArray();
        }
    }
}