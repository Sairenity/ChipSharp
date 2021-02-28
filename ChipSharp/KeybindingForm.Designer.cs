
namespace ChipSharp
{
    partial class KeybindingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(84, 156);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(66, 30);
            this.button17.TabIndex = 1;
            this.button17.Text = "Save";
            this.button17.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(12, 156);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(66, 30);
            this.button18.TabIndex = 1;
            this.button18.Text = "Reset";
            this.button18.UseVisualStyleBackColor = true;
            // 
            // KeybindingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 202);
            this.Controls.Add(this.button18);
            this.Controls.Add(this.button17);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "KeybindingForm";
            this.Text = "Keybindings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeybindingForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
    }
}