using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSharp
{
    public class IOState
    {
        public byte[] Rom { get; set; }
        public bool[] KeyState { get; set; } = new bool[16];
        public bool Reset { get; set; }
        public bool ForceRedraw { get; set; }
    }
}
