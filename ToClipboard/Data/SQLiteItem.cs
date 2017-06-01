using JumpList_To_Clipboard.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpList_To_Clipboard.Data
{
    [DebuggerDisplay("SQLiteItem: {Title} - {Text}")]
    public class SQLiteItem : IItem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUsed { get; set; }
        public long CountUsed { get; set; }
    }
}
