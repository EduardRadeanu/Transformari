using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ComplexExplorer
{
    public  class AboutForm : Form
    {
        private string[] texte;
        private RichTextBox txtInfo;
        private Button btnOK;
        public AboutForm()
        {
            texte = new string[]{ 
            "Aplica\u021Bie \u00EEn Microsoft Visual C# pentru grafic\u0103 2D",
            "                   --- uz didactic ---",
            "Cursul \"Elemente de analiz\u0103 complex\u0103 \u0219i aplica\u021Bii\"",          
            "Anul II, sec\u021Bia Matematic\u0103-informatic\u0103",
            "Facultatea de Matematic\u0103",
            "Universitatea \"Al. I. Cuza\" din Ia\u0219i", "Rom\u00E2nia"         
            };


            this.Text = "About";
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Height = 10 * this.Width / 16;
            this.ControlBox = false;


            txtInfo = new RichTextBox();
            txtInfo.Left = 15;
            txtInfo.Top = 18;
            txtInfo.Width = this.Width - 40;
            txtInfo.Height = 6 + this.Height / 2;
            txtInfo.Lines = texte;
            txtInfo.BackColor = Color.Navy;
            txtInfo.ForeColor = Color.WhiteSmoke;
            txtInfo.ReadOnly = true;
            txtInfo.Visible = true;
            this.Controls.Add(txtInfo);

            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Left = this.ClientRectangle.Width - btnOK.Width - 20;
            btnOK.Top = this.ClientRectangle.Height - btnOK.Height - 8;

            btnOK.Visible = true;

            btnOK.Click += new EventHandler(btnOK_Click);
            this.Controls.Add(btnOK);
            btnOK.Select();
        }

        private void btnOK_Click(object sender, EventArgs eArgs)
        {

            this.Close();

        }
    }
}

