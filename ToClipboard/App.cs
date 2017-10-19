using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ToClipboard.Model;
using System.Linq;

namespace ToClipboard
{
    public partial class App
    {
        public const string TITLE = "JumpList to Clipboard";
        public const string COMPANY = "Other";

        static App()
        {
            ProgramFiles_x86 = new DirectoryInfo(_ProgramFilesx86());
        }

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

        public static bool Try_AppAndIcon_IsSteam(IItem item, Action<FileInfo> action)
        {
            string iconfile = item.LaunchApp;
            if (!string.IsNullOrWhiteSpace(iconfile)
                && iconfile.ToLower().StartsWith("steam"))
            {
                FileInfo icon = SteamApp(item);
                action(icon);
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

        static Dictionary<string, FileInfo> _steamApps;
        public static FileInfo SteamApp(IItem item)
        {
            if (SteamAppsDirectory.Exists)
            {
                if (null == _steamApps)
                    _steamApps = new Dictionary<string, FileInfo>();
                if (_steamApps.ContainsKey(item.Title))
                    return _steamApps[item.Title];

                var dirs = SteamAppsDirectory.GetDirectories();
                if (dirs.Length > 0)
                {
                    List<string> title = item.Title.ToLower().Split(' ').ToList();
                    title.AddRange(item.Text.ToLower().Split(' ')); // user can specify appname here if they aren't using it for the clipboard
                    string[] titles = title.ToArray();

                    // Find directory that most matches the title given by the user
                    var dirsPairs = dirs
                        .Select(d => new { Dir = d, Count = d.Name.ToLower().ContainsCount(titles) });
                    //int max = dirs.Max(d => d.Count);
                    var dirPair = dirsPairs.OrderByDescending(d => d.Count).First();

                    var files = dirPair.Dir.GetFiles("*.exe", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        // Find the application that most matches the title given by the user
                        // This doesn't necessarily find the correct steam app.
                        // It is only a stab in the dark to find it
                        // It's up to the user to supply the best name, and if the application name is funky
                        // to supply its name exactly if they want the icon
                        var filePairs = files
                            .Select(f => new { File = f, Count = f.NameWithoutExtension().ToLower().ContainsCount(titles) });
                        var filePair = filePairs.OrderByDescending(f => f.Count).First();
                        FileInfo file = filePair.File;
                        _steamApps.Add(item.Title, file);
                        return file;
                    }
                }
            }
            return null;
        }


        public static DirectoryInfo SteamAppsDirectory
        {
            get
            {
                if (null == _SteamAppsDirectory)
                    _SteamAppsDirectory = ProgramFiles_x86.Directory(@"Steam\steamapps\common");
                return _SteamAppsDirectory;
            }
        }
        static DirectoryInfo _SteamAppsDirectory;

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

        #region ProgramFiles X86
        public static DirectoryInfo ProgramFiles_x86;

        // the following is from: https://stackoverflow.com/a/194223/353147
        static string _ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }
        #endregion
    }
}
