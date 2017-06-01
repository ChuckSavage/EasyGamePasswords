using System.Collections.Generic;
using System.Diagnostics;

namespace ToClipboard.Data.Tables
{
    [DebuggerDisplay("SQLite Category: {CategoryId} : {Name}")]
    public class Category
    {
        public long CategoryId { get; set; }
        public string Name { get; set; }

        public List<Item> Items { get; set; }

    }
}
