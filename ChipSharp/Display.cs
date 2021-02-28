using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        
        private string _originalText;

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

        public void SetImage(Bitmap image)
        {
            DisplayImage.Image = image;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Display_Load(object sender, EventArgs e)
        {

        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
    }
}
