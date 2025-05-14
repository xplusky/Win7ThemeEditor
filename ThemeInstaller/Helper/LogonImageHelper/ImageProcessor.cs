#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#endregion

namespace ThemeInstaller.LogonImage
{
    public class ImageProcessor
    {
        // Methods
        public static JpgImage CompressToJpeg(Image image, Size targetDimension, long targetSizeBytes)
        {
            var image2 = new JpgImage();
            var image3 = new JpgImage();
            image2.Quality = 100;
            image3.Quality = 0;
            using (var image5 = ResizeImage(image, targetDimension))
            {
                do
                {
                    var image4 = new JpgImage
                    {
                        Quality = image3.Quality + ((image2.Quality - image3.Quality) / 2)
                    };
                    image4.WriteImageToStream(image5);
                    if (image4.Size > targetSizeBytes)
                    {
                        image2.Dispose();
                        image2 = image4;
                    }
                    else
                    {
                        image3.Dispose();
                        image3 = image4;
                    }
                }
                while (Math.Abs((image2.Quality - image3.Quality)) > 1);
            }
            if ((targetSizeBytes >= image2.Size) && (image2.Size > image3.Size))
            {
                image3.Dispose();
                return image2;
            }
            image2.Dispose();
            return image3;
        }

        public static Image CropImage(Image img, Rectangle cropArea)
        {
            var bitmap = new Bitmap(img);
            return bitmap.Clone(cropArea, img.PixelFormat);
        }

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            Image image = new Bitmap(size.Width, size.Height);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
            }
            return image;
        }
    }


}
