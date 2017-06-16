using System;
using System.IO;
using System.Reflection;

namespace ToClipboard
{
    public partial class App
    {
        public const string COMPANY = "Other";

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

        public static string PathAdd_Company_and_AppName(string path)
        {
            if (!string.IsNullOrWhiteSpace(COMPANY))
                path = Path.Combine(path, COMPANY);
            if (!string.IsNullOrWhiteSpace(AppName))
                path = Path.Combine(path, AppName);
            return path;
        }

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
