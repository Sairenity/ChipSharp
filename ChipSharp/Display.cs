using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipSharp
{
    public partial class Display : Form
    {
        private readonly IOState _ioState;
        public KeybindingState KeybindingState { get; set; }
        private readonly Bitmap _displayBuffer = new Bitmap(640, 320, PixelFormat.Format32bppArgb);
        private string _originalText;
        private static Color _backColor = Color.FromArgb(255, 0, 0, 23);

        public int IPS
        {
            set
            {
                this.Text = $"{_originalText} - {value} ips";
            }
        }

        public Display(IOState ioState)
        {
            _ioState = ioState;
            InitializeComponent();
            _originalText = Text;
            KeybindingState = new KeybindingState();
        }

        private void Display_KeyDown(object sender, KeyEventArgs e)
        {
            var idx = KeybindingState.Keybindings.FirstOrDefault(q => q.Keybind == e.KeyCode).Index;
            if (idx >= 0)
                _ioState.KeyState[idx] = true;
        }

        private void Display_KeyUp(object sender, KeyEventArgs e)
        {
            var idx = KeybindingState.Keybindings.FirstOrDefault(q => q.Keybind == e.KeyCode).Index;
            if (idx >= 0)
                _ioState.KeyState[idx] = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var romChooserDialog = new OpenFileDialog();
            var fileResult = romChooserDialog.ShowDialog();
            if (fileResult == DialogResult.OK)
            {
                var romBytes = File.ReadAllBytes(romChooserDialog.FileName);
                _ioState.Rom = romBytes;
                _ioState.Reset = true;
            }
        }

        private void keybindingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new KeybindingForm(KeybindingState).ShowDialog();
        }

        public void RenderDisplay()
        {
            using (var g = Graphics.FromImage(_displayBuffer))
            {
                g.FillRectangle(new SolidBrush(_backColor), 0, 0, 640, 320);
                for (int lineY = 0; lineY < 32; lineY++)
                {
                    for (int lineX = 0; lineX < 64; lineX++)
                    {
                        var index = lineX + 64 * lineY;
                        if (_ioState.Display[index])
                            g.FillRectangle(Brushes.White, lineX * 10, lineY * 10, 10, 10);

                    }
                }
            }


            DisplayImage.Image = _displayBuffer;
        }

        private void Display_Load(object sender, EventArgs e)
        {

        }
    }
}
