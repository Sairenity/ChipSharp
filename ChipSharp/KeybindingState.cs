using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipSharp
{
    public class KeybindingState
    {
        public List<(byte Index, Keys Keybind)> Keybindings { get; set; } = new List<(byte Index, Keys Keybind)>
        {
            (0x01, Keys.D1), (0x02, Keys.D2), (0x03,Keys.D3), (0x0C,Keys.D4),
            (0x04, Keys.Q), (0x05, Keys.W), (0x06, Keys.E), (0x0D, Keys.R),
            (0x07, Keys.A), (0x08, Keys.S), (0x09, Keys.D), (0x0E, Keys.F),
            (0x0A, Keys.Z), (0x00, Keys.X), (0x0B, Keys.C), (0x0F, Keys.V),
        };
    }
}
