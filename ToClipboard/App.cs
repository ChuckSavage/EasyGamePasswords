using System;
using System.IO;
using System.Reflection;

namespace ToClipboard
{
    public partial class App
    {
        public const string TITLE = "JumpList to Clipboard";
        public const string COMPANY = "Other";

        public static bool Try_AppAndIcon_IsHttp(string httplocation, Action<FileInfo> action)
        {
            Uri uri;
            if (!string.IsNullOrWhiteSpace(httplocation)
                && Uri.TryCreate(httplocation, UriKind.Absolute, out uri)
                && !uri.IsFile
                )
            {
                var iconLocation = IconLocation(uri);
                action(iconLocation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// If icon is an image, run an action with the icon location.
        /// </summary>
        /// <param name="iconfile"></param>
        /// <param name="action"></param>
        public static bool Try_AppAndIcon_IsImage(string iconfile, Action<FileInfo> action)
        {
            if (!string.IsNullOrWhiteSpace(iconfile)
                && File.Exists(iconfile)
                && iconfile.IsImage())
            {
                // Get the temporary file location for the icon
                var temp = IconLocation(iconfile);
                action(temp);
                return true;
            }
            return false;
        }

        public static bool Try_AppAndIcon_IsSteam(string iconfile, Action<FileInfo> action)
        {
            if (!string.IsNullOrWhiteSpace(iconfile)
                && iconfile.ToLower().StartsWith("steam"))
            {
                action(null);
                return true;
            }
            return false;
        }

        static string AppName
        {
            get
            {
                if (string.IsNullOrEmpty(_ExecutingAssemblyName))
                {
                    string fullName = Assembly.GetExecutingAssembly().FullName;
                    _ExecutingAssemblyName = fullName.Split(',')[0].Trim();
                }
                return _ExecutingAssemblyName;
            }
        }
        static string _ExecutingAssemblyName;

        /// <summary>
        /// Usually located at C:\ProgramData\[company]\[AppName]
        /// </summary>
        public static DirectoryInfo DataDirectory
        {
            get
            {
                if (null == _DataDirectory)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    path = PathAdd_Company_and_AppName(path);
                    _DataDirectory = new DirectoryInfo(path);
                }
                return _DataDirectory;
            }
        }
        static DirectoryInfo _DataDirectory;

        /// <summary>
        /// Get temp file for the icon.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileInfo IconLocation(string file)
        {
            return TempDirectory.File(Path.GetFileNameWithoutExtension(file) + ".ico");
        }

        /// <summary>
        /// Get temp file for the icon.
        /// </summary>
        /// <param name="IN"></param>
        /// <returns></returns>
        public static FileInfo IconLocation(Uri uri)
        {
            if (uri.IsFile)
                return IconLocation(uri.AbsolutePath);
            // Remove all periods in host name "www.domain.com" becomes "wwwdomaincom"
            string temp = uri.Host.Replace(".", "").ToLower();
            return TempDirectory.File(temp + ".ico");
        }

        public static string PathAdd_Company_and_AppName(string path)
        {
            if (!string.IsNullOrWhiteSpace(COMPANY))
                path = Path.Combine(path, COMPANY);
            if (!string.IsNullOrWhiteSpace(AppName))
                path = Path.Combine(path, AppName);
            return path;
        }

        public static DirectoryInfo TempDirectory
        {
            get
            {
                if (null == _TempDirectory)
                {
                    string path = Path.GetTempPath();
                    path = PathAdd_Company_and_AppName(path);
                    _TempDirectory = new DirectoryInfo(path);
                    if (!_TempDirectory.Exists)
                        Directory.CreateDirectory(_TempDirectory.FullName);
                }
                return _TempDirectory;
            }
        }
        static DirectoryInfo _TempDirectory;

        /// <summary>
        /// Usually located at C:\Users\[user]\AppData\Roaming\[company]\[AppName]
        /// </summary>
        public static DirectoryInfo UserDataDirectory
        {
            get
            {
                if (null == _UserData)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    path = PathAdd_Company_and_AppName(path);
                    _UserData = new DirectoryInfo(path);
                }
                return _UserData;
            }
        }
        static DirectoryInfo _UserData;
    }
}
