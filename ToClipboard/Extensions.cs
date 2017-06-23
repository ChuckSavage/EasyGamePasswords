using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToClipboard
{
    public static class Extensions
    {
        /// <summary>
        /// Open in Explorer
        /// </summary>
        /// <param name="dir"></param>
        public static void Open(this DirectoryInfo dir)
        {
            dir.Refresh();
            if (!dir.Exists)
                throw new DirectoryNotFoundException(dir.FullName);

            WindowsCommand(dir.FullName, "Open", null);
        }

        /// <summary>
        /// Open with default 'open' program
        /// </summary>
        /// <param name="value"></param>
        public static Process Open(this FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
                throw new FileNotFoundException(file.FullName, file.FullName);

            return WindowsCommand(file.FullName, "Open", null);
        }

        public static Process WindowsCommand(this string path, string verb, string arguments)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            if (!string.IsNullOrEmpty(arguments))
                p.StartInfo.Arguments = arguments;
            p.StartInfo.Verb = verb;
            p.Start();
            return p;
        }

        public static bool IsImage(this string fileName)
        {
            string targetExtension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(targetExtension))
                return false;
            else
                targetExtension = "*" + targetExtension.ToLowerInvariant();

            if (null == _imageExtensions)
            {
                _imageExtensions = new List<string>();
                // retrieve a list of extensions that the system recognises as image files
                foreach (System.Drawing.Imaging.ImageCodecInfo imageCodec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
                    _imageExtensions.AddRange(imageCodec.FilenameExtension.ToLowerInvariant().Split(";".ToCharArray()));
            }

            return _imageExtensions.Any(ext => ext.Equals(targetExtension));
        }
        static List<string> _imageExtensions;
    }
}
