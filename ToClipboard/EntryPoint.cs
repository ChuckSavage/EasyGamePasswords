using System;
using System.IO;
using System.Linq;
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
            // Args has the format of "ITEMID ITEMTEXT"
            if (args != null && args.Length > 1)
            {
                long itemId;
                if (long.TryParse(args.Take(1).SingleOrDefault() ?? "0", out itemId) && itemId > 0)
                {
                    IItem item = null;
                    using (var db = new Data.DataSQLite())
                    {
                        item = db.Item_Clicked(itemId);
                        db.SaveChanges();
                    }
                    System.Windows.Clipboard.SetText(string.Join(" ", args.Skip(1)));

                    // If set to launch the app after copying text to clipboard
                    if (null != item && item.DoLaunchApp && !string.IsNullOrWhiteSpace(item.LaunchApp))
                    {
                        FileInfo file = new FileInfo(item.LaunchApp);
                        if (file.Exists)
                            file.OpenLocation();
                    }
                }
            }
            else
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
