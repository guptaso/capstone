using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyStrokes
{
    public partial class Help : Form
    {

        private GamingUseCase GameWindow;

        public Help(GamingUseCase window)
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();

            GameWindow = window;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1600, 865);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Help";
            this.Text = "How to Use This Widget";
            this.ResumeLayout(false);

        }
    }
}
