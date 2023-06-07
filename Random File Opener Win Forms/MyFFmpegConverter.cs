using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class MyFFmpegConverter
    {
        private static readonly StreamBuffer[] _buffers = StreamBuffer.Get(8);

        private Process FFMpegProcess;
        private bool FFMpegProcessWaitForAsyncReadersCompleted;
        public static string FFMpegExeName { get; } = "ffmpeg.exe";
        public static string FFMpegToolPath { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string WorkingDirectory { get; } = Path.GetDirectoryName(FFMpegToolPath);
        public static string FFMpegExePath { get; } = Path.Combine(FFMpegToolPath, FFMpegExeName);


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

                var startInfo = new ProcessStartInfo(FFMpegExePath, arguments)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = WorkingDirectory,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                if (FFMpegProcess != null)
                    throw new InvalidOperationException("FFMpeg process is already started");

                FFMpegProcessWaitForAsyncReadersCompleted = false;
                FFMpegProcess = Process.Start(startInfo);
                
                FFMpegProcess.OutputDataReceived += (o, args) => { };
                FFMpegProcess.BeginOutputReadLine();
                FFMpegProcess.BeginErrorReadLine();
                WaitFFMpegProcessForExit();
                if (FFMpegProcess.ExitCode != 0)
                    throw new Exception(FFMpegProcess.ExitCode.ToString());
                FFMpegProcess.Close();
                FFMpegProcess = null;

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
                buffer = new StreamBuffer();

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
            if (FFMpegProcess == null)
                throw new Exception("FFMpeg process was aborted");
            if (FFMpegProcess.HasExited)
                return;
            if (!FFMpegProcess.WaitForExit(int.MaxValue))
            {
                EnsureFFMpegProcessStopped();
                throw new Exception("FFMpeg process was aborted");
            }
            var ffMpegProcess = FFMpegProcess;
            if (ffMpegProcess == null)
                return;
            lock (ffMpegProcess)
            {
                if (ffMpegProcess != FFMpegProcess || FFMpegProcessWaitForAsyncReadersCompleted)
                    return;
                ffMpegProcess.WaitForExit();
                FFMpegProcessWaitForAsyncReadersCompleted = true;
            }
        }

        private void EnsureFFMpegProcessStopped()
        {
            if (FFMpegProcess == null)
                return;
            if (!FFMpegProcess.HasExited)
            {
                try
                {
                    FFMpegProcess.Kill();
                }
                catch (Exception)
                {
                }
            }
            FFMpegProcess = null;
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
    }
}