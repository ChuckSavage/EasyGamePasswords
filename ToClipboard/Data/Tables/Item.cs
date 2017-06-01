using ToClipboard.Model;
using System;
using System.Diagnostics;

namespace ToClipboard.Data.Tables
{
    [DebuggerDisplay("SQLite Item: {ItemId} : {Title} - {Text}")]
    public class Item : IItem
    {
        public Item()
        {
            DateCreated = DateTime.Now;
            DateLastUsed = DateTime.Now;
        }

        public long ItemId { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }

        public string LaunchApp { get; set; }
        public bool DoLaunchApp { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastUsed { get; set; }
        public long CountUsed { get; set; }

        public long? JumpListId { get; set; } // long? Item is either child of JumpList or Category
        public JumpList JumpList { get; set; }

        public long? CategoryId { get; set; } // long? Item is either child of JumpList or Category
        public Category Category { get; set; }
    }
}
