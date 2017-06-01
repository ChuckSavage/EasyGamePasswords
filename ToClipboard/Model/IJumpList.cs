using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToClipboard.Model
{
    public interface IJumpList
    {
        long JumpListId { get; set; }
        string Name { get; set; }
    }
}
