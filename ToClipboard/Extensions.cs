using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToClipboard
{
    public static class Extensions
    {

        /// <summary>
        /// Open with default 'open' program
        /// </summary>
        /// <param name="value"></param>
        public static Process Open(this FileInfo value)
        {
            return WindowsCommand(value, "Open", null);
        }

        public static Process WindowsCommand(this FileInfo value, string verb, string arguments)
        {
            value.Refresh();
            if (!value.Exists)
                throw new FileNotFoundException("File doesn't exist", value.FullName);
            Process p = new Process();
            p.StartInfo.FileName = value.FullName;
            if (!string.IsNullOrEmpty(arguments))
                p.StartInfo.Arguments = arguments;
            p.StartInfo.Verb = verb;
            p.Start();
            return p;
        }
    }
}
