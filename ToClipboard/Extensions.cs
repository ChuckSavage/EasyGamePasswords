using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToClipboard
{
    public static class Extensions
    {
        /// <summary>
        /// Append name to current file name, returning a new file.
        /// </summary>
        /// <param name="value" />
        /// <param name="name" />
        /// <exception cref="PathTooLongException" />
        public static FileInfo AppendName(this FileInfo value, string name)
        {
            name = value.NameWithoutExtension() + name + value.Extension;
            return value.Rename(name);
        }

        /// <summary>
        /// Combine directory's full name with passed parameters to create a new path.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Combine(this DirectoryInfo dir, params string[] name)
        {
            if (name.Any(n => n.Contains("..")))
            {
                List<string> path = dir.FullName.Split(Path.DirectorySeparatorChar).ToList();
                foreach (string n in name)
                {
                    List<string> subpath = n.Split(Path.DirectorySeparatorChar).ToList();
                    foreach (string s in subpath)
                    {
                        if (s == "..")
                            path.RemoveAt(path.Count - 1);
                        else
                            path.Add(s);
                    }
                }
                return string.Join(Path.DirectorySeparatorChar.ToString(), path);
            }
            {
                List<string> path = new List<string> { dir.FullName };
                path.AddRange(name);
                return Path.Combine(path.ToArray());
            }
        }

        /// <summary>
        /// Does the string contain any of the arguments supplied, count the number of occurances.
        /// </summary>
        /// <param name="IN"></param>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string IN, out int count, params string[] args)
        {
            count = 0;
            foreach (string arg in args)
                if (IN.Contains(arg))
                    count++;
            return count > 0;
        }

        /// <summary>
        /// Counts the number of arguments that are matched in the string.
        /// </summary>
        /// <param name="IN"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ContainsCount(this string IN, params string[] args)
        {
            int count;
            ContainsAny(IN, out count, args);
            return count;
        }

        public static void DeleteIfExists(this FileInfo file)
        {
            if (null != file)
                DeleteFileIfExists(file.FullName);
        }

        public static void DeleteFileIfExists(this string file)
        {
            if (!string.IsNullOrWhiteSpace(file)
                && System.IO.File.Exists(file))
                System.IO.File.Delete(file);
        }

        /// <summary>
        /// Directory within the directory (or sub-directory), doesn't check for its existence.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DirectoryInfo Directory(this DirectoryInfo dir, params string[] name)
        {
            return new DirectoryInfo(Combine(dir, name));
        }

        public static bool Exists(this FileInfo file, bool refresh)
        {
            if (refresh)
                file.Refresh();
            return file.Exists;
        }

        /// <summary>
        /// File within the directory (or sub-directory), doesn't check for its existence.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileInfo File(this DirectoryInfo dir, params string[] name)
        {
            return new FileInfo(Combine(dir, name));
        }

        /// <summary>
        /// Get File's Name without its extension
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NameWithoutExtension(this FileInfo value)
        {
            return Path.GetFileNameWithoutExtension(value.Name);
        }

        /// <summary>
        /// Open in Explorer
        /// </summary>
        /// <param name="dir"></param>
        public static void OpenLocation(this DirectoryInfo dir)
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
        public static Process OpenLocation(this FileInfo file)
        {
            file.Refresh();
            if (!file.Exists)
                throw new FileNotFoundException(file.FullName, file.FullName);

            return WindowsCommand(file.FullName, "Open", null);
        }

        /// <summary>
        /// Rename the file (returning the new file), the original stays the same.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileInfo Rename(this FileInfo file, params string[] name)
        {
            return file.Directory.File(name);
        }

        /// <summary>
        /// Create a new file based on current, with a different name and has the same extension.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <exception cref="PathTooLongException" />
        public static FileInfo SetName(this FileInfo file, string name)
        {
            name = Path.ChangeExtension(name, file.Extension);
            return file.Rename(name);
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
