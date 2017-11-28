using System;
using System.Diagnostics;
using System.IO;
using ToClipboard.Model;

namespace ToClipboard
{
    /// <summary>
    /// This is set in ToClipboard properties to be the start of the application. If
    /// there are arguments (IE someone clicked a JumpList item) set that item to the
    /// clipboard and exit. Fast and easy. (and / or update database to reflect item clicked for sorting)
    /// </summary>
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool hidden = false;
            //Debugger.Launch();
            // Args has the format of "ITEMID" if lanched from JumpList item
            if (args != null && args.Length == 1)
            {
                long itemId;
                if (long.TryParse(args[0] ?? "0", out itemId) && itemId > 0)
                {
                    hidden = true;
                    IItem item = null;
                    using (var db = new Data.DataSQLite())
                    {
                        item = db.Item_Clicked(itemId);
                        db.SaveChanges();
                    }
                    if (null != item)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Text))
                            System.Windows.Clipboard.SetText(item.Text);

                        // If set to launch the app after copying text to clipboard
                        if (item.DoLaunchApp && !string.IsNullOrWhiteSpace(item.LaunchApp))
                        {
                            if (!App.Try_AppAndIcon_IsSteam(item, icon => Process.Start(item.LaunchApp)))
                                try
                                {
                                    // Throws NotSupportedException for URI's
                                    FileInfo file = new FileInfo(item.LaunchApp);
                                    if (file.Exists)
                                        file.OpenLocation();
                                }
                                catch (NotSupportedException)
                                {
                                    string http = item.LaunchApp.ToLower();
                                    if (!http.Contains("http"))
                                        http = "http://" + http;
                                    Process.Start(http);
                                }
                        }
                    }
                }
            }
            var app = new App()
            {
                HideOnRun = hidden
            };
            app.Run();
        }
    }
}
