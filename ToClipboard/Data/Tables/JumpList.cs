using System.Collections.Generic;
using System.Diagnostics;
using ToClipboard.Model;

namespace ToClipboard.Data.Tables
{
    [DebuggerDisplay("SQLite JumpList: {JumpListId} : {Name}")]
    public class JumpList : IJumpList
    {
        public long JumpListId { get; set; }
        public string Name { get; set; }

        public List<Category> Groups { get; set; }
        public List<Item> Items { get; set; }
    }
}
