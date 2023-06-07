using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Random_File_Opener_Win_Forms
{
    internal static class ImageService
    {
        public static Bitmap[] GetFitImages(GeneratedFileListItem file, PictureBox largePictureBox, PictureBox[] smallPictureBoxes)
        {
            var sourceImage = GetSourceImage(file);

            if (sourceImage.Length == 0)
                return Array.Empty<Bitmap>();

            if (sourceImage.Length == 1)
                return new[] { ResizeImageToFitPictureBox(sourceImage[0], largePictureBox.Size) };

            var resized = sourceImage
                .Select((u, index) => ResizeImageToFitPictureBox(u, smallPictureBoxes[index].Size))
                .ToArray();

            return resized;
        }

        private static Bitmap[] GetSourceImage(GeneratedFileListItem file)
        {
            var extension = Utilities.ExtractExtension(file.FileName);
            
            if (Consts.ImageExtensions.Contains(extension))
                return new [] { new Bitmap(file.Path) };

            if (Consts.VideoExtensions.Contains(extension))
                return GetVideoThumbnails(file, Consts.VideoThumbnailPositions);

            return Array.Empty<Bitmap>();
        }

        private static Bitmap[] GetVideoThumbnails(GeneratedFileListItem file, params TimeSpan[] positions)
        {
            var thumbnails = positions.Prepend(TimeSpan.FromSeconds(1))
                .AsParallel()
                .AsOrdered()
                .Select(u => GetThumbnailAtPosition(file, u))
                .Where(u => u != null)
                .ToArray();

            if (thumbnails.Length == positions.Length + 1)
                return thumbnails.Skip(1).ToArray();

            return thumbnails;
        }

        private static Bitmap GetThumbnailAtPosition(GeneratedFileListItem file, TimeSpan position)
        {
            var ffmpeg = new MyFFMpegConverter();
            
            var thumbnailStream = ffmpeg.GetVideoThumbnail(file.Path, position.TotalSeconds);
            
            if (thumbnailStream.Length == 0)
                return null;
            
            return new Bitmap(thumbnailStream);
        }

        private static Bitmap ResizeImageToFitPictureBox(Bitmap sourceImage, Size sizeToFit)
        {
            var (height, width) = DimensionsToFitPictureBox(sourceImage, sizeToFit);

            var resized = ResizeImage(sourceImage, width, height);
            return resized;
        }

        private static (int height, int width) DimensionsToFitPictureBox(Bitmap sourceImage, Size sizeToFit)
        {
            var height = sourceImage.Height;
            var width = sourceImage.Width;

            if (height < sizeToFit.Height && width < sizeToFit.Width)
            {
                var hRatio = sizeToFit.Height / (double)height;
                var wRatio = sizeToFit.Width / (double)width;

                var minRatio = Math.Min(hRatio, wRatio);
                height = (int)(height * minRatio);
                width = (int)(width * minRatio);
                return (height, width);
            }

            if (height > sizeToFit.Height)
            {
                var ratio = (double)height / sizeToFit.Height;
                height = (int)Math.Ceiling(height / ratio);
                width = (int)Math.Ceiling(width / ratio);
            }

            if (width > sizeToFit.Width)
            {
                var ratio = (double)width / sizeToFit.Width;
                height = (int)Math.Ceiling(height / ratio);
                width = (int)Math.Ceiling(width / ratio);
            }

            return (height, width);
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}