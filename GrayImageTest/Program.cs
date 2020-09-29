using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GrayImageTest
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            {
                var systemBitmap = new Bitmap("image.png");
                ConvertToGray(systemBitmap);
                systemBitmap.Save("system-gray.png");
            }

            {
                var skiaBitmap = SKBitmap.Decode("image.png");
                ConvertToGray(skiaBitmap);
                using var stream = File.Create("skia-pointer-gray.png");
                skiaBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            {
                var skiaBitmap = SKBitmap.Decode("image.png");
                ConvertToGrayWithColor(skiaBitmap);
                using var stream = File.Create("skia-color-gray.png");
                skiaBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
        }

        private static void ConvertToGray(Bitmap bitmap)
        {
            var rect = new Rectangle(Point.Empty, bitmap.Size);
            var locked = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var pixels = new Span<byte>((byte*)locked.Scan0, locked.Stride * locked.Height);
            for (var i = 0; i < pixels.Length; i += 4)
            {
                var sum = pixels[i + 0] + pixels[i + 1] + pixels[i + 2];
                var avg = (byte)(sum / 3);

                pixels[i + 0] = avg;
                pixels[i + 1] = avg;
                pixels[i + 2] = avg;
            }

            bitmap.UnlockBits(locked);
        }

        private static void ConvertToGray(SKBitmap bitmap)
        {
            var pixels = new Span<byte>((byte*)bitmap.GetPixels(out var length), (int)length);
            for (var i = 0; i < pixels.Length; i += 4)
            {
                var sum = pixels[i + 0] + pixels[i + 1] + pixels[i + 2];
                var avg = (byte)(sum / 3);

                pixels[i + 0] = avg;
                pixels[i + 1] = avg;
                pixels[i + 2] = avg;
            }
        }

        private static void ConvertToGrayWithColor(SKBitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var sum = pixel.Red + pixel.Green + pixel.Blue;
                    var avg = (byte)(sum / 3);
                    bitmap.SetPixel(x, y, new SKColor(avg, avg, avg, pixel.Alpha));
                }
            }
        }
    }
}
