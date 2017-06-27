﻿/*
 * Open source file from https://gist.github.com/darkfall/1656050
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ToClipboard.Misc
{
    /// <summary>
    /// Provides helper methods for creating icons
    /// </summary>
    public static class IconHelper
    {
        const string GOOGLE = @"http://www.google.com/s2/favicons?domain=";

        /// <summary>
        /// Download Icon from internet
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="outFile"></param>
        /// <returns></returns>
        public static bool HttpToIcon(Uri uri, string outFile)
        {
            if (null == uri
                || uri.IsFile)
                return false;

            if (string.IsNullOrWhiteSpace(outFile))
                return false;

            var client = new System.Net.WebClient();
            client.DownloadFile(
                GOOGLE + uri.Host,
                outFile);

            return true;
        }

        /// <summary>
        /// Download Icon from internet
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="outFile"></param>
        /// <returns></returns>
        public static bool HttpToIcon(string uri, string outFile)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return false;
            return HttpToIcon(new Uri(uri), outFile);
        }


        /// <summary>
        /// Converts an image to an icon (ico)
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="output">The output stream</param>
        /// <param name="size">Needs to be a factor of 2 (16x16 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ImageToIcon(Stream input, Stream output, int size = 16, bool preserveAspectRatio = false)
        {
            var inputBitmap = (Bitmap)Bitmap.FromStream(input);
            if (inputBitmap == null)
                return false;

            float width = size, height = size;
            if (preserveAspectRatio)
            {
                if (inputBitmap.Width > inputBitmap.Height)
                    height = ((float)inputBitmap.Height / inputBitmap.Width) * size;
                else
                    width = ((float)inputBitmap.Width / inputBitmap.Height) * size;
            }

            var newBitmap = new Bitmap(inputBitmap, new Size((int)width, (int)height));
            if (newBitmap == null)
                return false;

            // save the resized png into a memory stream for future use
            using (MemoryStream memoryStream = new MemoryStream())
            {
                newBitmap.Save(memoryStream, ImageFormat.Png);

                var iconWriter = new BinaryWriter(output);
                if (output == null || iconWriter == null)
                    return false;

                // 0-1 reserved, 0
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);

                // 2-3 image type, 1 = icon, 2 = cursor
                iconWriter.Write((short)1);

                // 4-5 number of images
                iconWriter.Write((short)1);

                // image entry 1
                // 0 image width
                iconWriter.Write((byte)width);
                // 1 image height
                iconWriter.Write((byte)height);

                // 2 number of colors
                iconWriter.Write((byte)0);

                // 3 reserved
                iconWriter.Write((byte)0);

                // 4-5 color planes
                iconWriter.Write((short)0);

                // 6-7 bits per pixel
                iconWriter.Write((short)32);

                // 8-11 size of image data
                iconWriter.Write((int)memoryStream.Length);

                // 12-15 offset of image data
                iconWriter.Write((int)(6 + 16));

                // write image data
                // png data must contain the whole png data file
                iconWriter.Write(memoryStream.ToArray());

                iconWriter.Flush();
            }

            return true;
        }

        /// <summary>
        /// Converts an image to an icon (ico)
        /// </summary>
        /// <param name="inputPath">The input path</param>
        /// <param name="outputPath">The output path</param>
        /// <param name="size">Needs to be a factor of 2 (16x16 px by default)</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool ImageToIcon(string inputPath, string outputPath, int size = 16, bool preserveAspectRatio = false)
        {
            using (FileStream inputStream = new FileStream(inputPath, FileMode.Open))
            using (FileStream outputStream = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                return ImageToIcon(inputStream, outputStream, size, preserveAspectRatio);
            }
        }
    }
}
