using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipSharp
{
    public partial class KeybindingForm : Form
    {
        private readonly KeybindingState _state;
        private bool _isRebinding;
        private int _rebindIndex;
        private readonly List<Button> _buttonList = new List<Button>();

        public KeybindingForm(KeybindingState state)
        {
            _state = state;
            InitializeComponent();
            for (var idx = 0; idx <= 0x0f; idx++)
            {
                var keybindButton = new Button
                {
                    Text = state.Keybindings[idx].Keybind.ToString(),
                    Size = new Size(30, 30),
                    Location = new Point(12 + 36 * (idx % 4), 12 + 36 * (idx / 4)),
                    Tag = Tuple.Create(idx, state.Keybindings[idx].Keybind)
                };
                keybindButton.Click += (o, e) =>
                {
                    if (_isRebinding) return;

                    var btn = (Button)o!;
                    var (id, key) = (Tuple<int, Keys>)btn.Tag;
                    _isRebinding = true;
                    _rebindIndex = id;
                    btn.Text = "...";
                };
                Controls.Add(keybindButton);
                _buttonList.Add(keybindButton);
            }
        }

        private void KeybindingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isRebinding)
            {
                _buttonList[_rebindIndex].Text = e.KeyCode.ToString();
                _state.Keybindings[_rebindIndex] = (_state.Keybindings[_rebindIndex].Index, e.KeyCode);
                _isRebinding = false;
                _rebindIndex = 0x00;
            }
        }
    }
}
