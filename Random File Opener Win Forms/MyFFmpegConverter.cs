using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using NReco.VideoConverter;

namespace Random_File_Opener_Win_Forms
{
    internal sealed class MyFFmpegConverter
    {
        private Process FFMpegProcess;
        private bool FFMpegProcessWaitForAsyncReadersCompleted;
        private static string FFMpegExeName { get; } = "ffmpeg.exe";
        private static string FFMpegToolPath { get; } = AppDomain.CurrentDomain.BaseDirectory;
        private static string WorkingDirectory { get; } = Path.GetDirectoryName(FFMpegToolPath);
        private static string FFMpegExePath { get; } = Path.Combine(FFMpegToolPath, FFMpegExeName);


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
                
                FFMpegProcess.BeginOutputReadLine();
                FFMpegProcess.BeginErrorReadLine();
                WaitFFMpegProcessForExit();
                if (FFMpegProcess.ExitCode != 0)
                    throw new Exception(FFMpegProcess.ExitCode.ToString());
                FFMpegProcess.Close();
                FFMpegProcess = null;

                using (var inputStream = new FileStream(output.Filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    CopyStream(inputStream, output.DataStream, 262144);
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

        private void CopyStream(Stream inputStream, Stream outputStream, int bufSize)
        {
            var buffer = new byte[bufSize];
            int count;
            while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                outputStream.Write(buffer, 0, count);
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
                throw new FFMpegException(-1, "FFMpeg process was aborted");
            if (FFMpegProcess.HasExited)
                return;
            if (!FFMpegProcess.WaitForExit(int.MaxValue))
            {
                EnsureFFMpegProcessStopped();
                throw new FFMpegException(-2, "FFMpeg process was aborted");
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
    }
}