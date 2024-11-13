using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using FFmpeg.AutoGen;

namespace Random_File_Opener_Win_Forms
{
    internal static class ImageService
    {
        public static Bitmap[] GetFitImages(GeneratedFileListItem file, PictureBox largePictureBox, PictureBox[] smallPictureBoxes)
        {
            var sourceImages = GetSourceImage(file);

            if (sourceImages.IsEmpty())
                return Array.Empty<Bitmap>();

            if (sourceImages.Length == 1)
            {
                var resizedImage = ResizeImageToFitPictureBox(sourceImages[0], largePictureBox.Size);
                sourceImages[0].Dispose();
                return new[] { resizedImage };
            }

            var resized = sourceImages
                .Zip(smallPictureBoxes, (u, v) => (Image: u, PictureBox: v))
                .Select(u => ResizeImageToFitPictureBox(u.Image, u.PictureBox.Size))
                .ToArray();

            sourceImages.ForAll(u => u.Dispose());

            return resized;
        }

        private static Bitmap[] GetSourceImage(GeneratedFileListItem file)
        {
            if (file.Extension.IsNullOrWhiteSpace())
                return Array.Empty<Bitmap>();
            
            if (Consts.ImageExtensions.Contains(file.Extension))
                return new [] { new Bitmap(file.PathToFile) };

            if (Consts.VideoExtensions.Contains(file.Extension) && Consts.VideoThumbnailPositions.IsNotEmpty())
                return GetVideoThumbnails(file, Consts.VideoThumbnailPositions);

            return Array.Empty<Bitmap>();
        }

        private static Bitmap[] GetVideoThumbnails(GeneratedFileListItem file, params TimeSpan[] positions)
        {
            var exists = File.Exists(file.PathToFile);
            if (!exists)
                return Array.Empty<Bitmap>();

            var mediaFile = MediaFile.Open(file.PathToFile);
            if (mediaFile.Video is null)
                return Array.Empty<Bitmap>();
            
            var thumbnails = new List<Bitmap>(positions.Length);
        
            foreach (var position in positions)
            {
                var success = mediaFile.Video.TryGetFrame(position, out var frame);
                if (!success)
                    break;
                
                thumbnails.Add(frame.ToBitmap());
            }
            
            return thumbnails.ToArray();
        }

        private static unsafe Bitmap ToBitmap(this ImageData bitmap)
        {
            fixed(byte* p = bitmap.Data)
            {
                return new Bitmap(bitmap.ImageSize.Width, bitmap.ImageSize.Height, bitmap.Stride, PixelFormat.Format24bppRgb, new IntPtr(p));
            }
        }
        
        private static Bitmap ResizeImageToFitPictureBox(Bitmap sourceImage, Size sizeToFit)
        {
            var resizedSize = SizeToFitPictureBox(sourceImage, sizeToFit);

            var resized = ResizeImage(sourceImage, resizedSize);
            return resized;
        }

        private static Size SizeToFitPictureBox(Bitmap sourceImage, Size sizeToFit)
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
                return new Size(width: width, height: height);
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

            return new Size(width: width, height: height);
        }

        private static Bitmap ResizeImage(Image image, Size size)
        {
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(width: size.Width, height: size.Height);

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