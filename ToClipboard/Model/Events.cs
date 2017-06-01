using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToClipboard.Model
{
    public class Events
    {
        public delegate void EntityChangen(string name, object oldvalue, object newvalue);
    }
}
