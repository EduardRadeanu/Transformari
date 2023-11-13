using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;

namespace ComplexExplorer
{
    public partial class ComplexForm : Form
    {
        public static AboutForm aboutForm = new AboutForm();
        private Graphics dc;
        private Bitmap bmp;
        private SolidBrush sdBrush;
        private Color[] paleta;
        public const int dimPaleta = 1024;
        public const int dimPicBmp = 600;
        public const int imin = 0, imax = dimPicBmp - 1, jmin = 0, jmax = dimPicBmp - 1;
        private double xmin, xmax, ymin, ymax;
        private double dxdi, dydj, didx, djdy;
        private bool doWork = false;
        private Color penColor=Color.White;
        private Color screenColor=Color.Black;

        

        public ComplexForm()
        {
            InitializeComponent();
            this.Height = dimPicBmp + 71;
            this.Width = dimPicBmp + 31;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(dimPicBmp + 25, dimPicBmp + 39);
            this.picBox.Location = new System.Drawing.Point(12, 26);
            this.picBox.Size = new System.Drawing.Size(dimPicBmp + 1, dimPicBmp + 1);
            this.picBox.BackColor = Color.Black;
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Size = new System.Drawing.Size(dimPicBmp + 25, 24);
            this.KeyPreview = true;
            this.startToolStripMenuItem.Enabled = true;
            this.stopToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Enabled = false;


            this.paleta = new Color[dimPaleta];
            for (int k = 0; k < dimPaleta; k++)
            {
                double tcol = 56.123 + 2.0 * Math.PI * k / (double)dimPaleta;
                int rcol = (int)(128 + 128 * Math.Sin(tcol));
                int gcol = (int)(128 + 128 * Math.Sin(2 * tcol));
                int bcol = (int)(128 + 128 * Math.Cos(3 * tcol));
                paleta[k] = Color.FromArgb(rcol % 256, gcol % 256, bcol % 256);
            }
            
            setXminXmaxYminYmax(0.0, 1.0, 0.0, 1.0);
            writeTitleBar("");
        }

        private void FractalForm_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(dimPicBmp, dimPicBmp, PixelFormat.Format24bppRgb);
            bmp.SetResolution(300F, 300F);
            picBox.Image = bmp;
            dc = Graphics.FromImage(bmp);
            sdBrush = new SolidBrush(screenColor);
        }
        private void FractalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
             doWork = false;   //esential: ca sa stopam desenarea 
        }

        public virtual void makeImage()     //trebuie suprascrisa      
        {
             setText("Metoda makeImage()","trebuie  suprascris\u0103!");           
        }
       
        public void setText(params string[] mesaj)
        {
            sdBrush.Color = penColor;
            using (Font font = new Font("Arial", 3.2f))
            {
                for (int k = 0; k < mesaj.Length; k++)
                    dc.DrawString(mesaj[k], font, sdBrush, 0.0F, k*15.0F);
            }

        }

        public void setTextAt(Complex z, string msg)
        {
            sdBrush.Color = penColor;
            using (Font font = new Font("Arial", 3.2f))
            {
                dc.DrawString(msg, font, sdBrush, getI(z.Re), jmax - getJ(z.Im));
            }

        }



        public Color PenColor { get { return penColor; } set { penColor = value; } }

        public Color ScreenColor
        {
            get
            {
                return screenColor;
            }
            set
            {
                screenColor = value;
                initScreen();
                if (penColor.ToArgb() != value.ToArgb()) return;
                penColor = Color.FromArgb(value.A, 255 - value.R, 255 - value.G, 255 - value.B);          
            }
        }
        public void initScreen()
        {
            sdBrush.Color = screenColor;
            dc.FillRectangle(sdBrush, 0, 0, dimPicBmp, dimPicBmp);           
            picBox.Invalidate(); 
        }

        public bool resetScreen()
        {
            picBox.Refresh();           //pentru re-pictarea ferestrei
            Application.DoEvents();     //pentru procesarea mesajelor
            return doWork;              //pentru semnalizarea opririi desenarii
        }


        private void writeTitleBar(string msg)
        {
            this.Text = this.GetType().ToString();
            if (msg != "") this.Text += " (" + msg + ")";
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.startToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Enabled = true;
            this.saveToolStripMenuItem.Enabled = false;
            initScreen();
            doWork = true;
            writeTitleBar("in lucru");
            makeImage();
            if (doWork) writeTitleBar("complet");
            else writeTitleBar("oprit");
            stopToolStripMenuItem_Click(null, null);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doWork = false;
            this.startToolStripMenuItem.Enabled = true;
            this.stopToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bmp == null) return;
            mySaveFileDialog.Filter = "bitmap files (*.bmp)|*.bmp";

            if (mySaveFileDialog.ShowDialog() == DialogResult.OK
                && mySaveFileDialog.FileName.Length > 0)
            {

                bmp.Save(mySaveFileDialog.FileName, ImageFormat.Bmp);
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm.ShowDialog();
        }

        private void FractalForm_KeyPress(object sender, KeyPressEventArgs e)
        {                // tasta <space> lanseaza/opreste desenarea
            if (e.KeyChar != ' ') return;
            e.Handled = true;
            if (!doWork) startToolStripMenuItem_Click(null, null);
            else stopToolStripMenuItem_Click(null, null);

        }
        public void delaySec(double s)
        {
            if (s <= 0 ) return;
            if(s>10) s=10;
            Thread.Sleep((int)(1000 * s));
        }


        public void setXminXmaxYminYmax(double xm, double XM, double ym, double YM)
        {
            xmin = xm;
            xmax = (XM != xm ? XM : XM + 1.0e-10);
            ymin = ym;
            ymax = (YM != ym ? YM : YM + 1.0e-10);
            dxdi = (xmax - xmin) / (imax - imin);
            dydj = (ymax - ymin) / (jmax - jmin);
            didx = (imax - imin) / (xmax - xmin);
            djdy = (jmax - jmin) / (ymax - ymin);

        }
        public double Xmin { get { return xmin; } }
        public double Xmax { get { return xmax; } }
        public double Ymin { get { return ymin; } }
        public double Ymax { get { return ymax; } }

        public int getI(double x) { return (int)(imin + (x - xmin) * didx); }
        public int getJ(double y) { return (int)(jmin + (y - ymin) * djdy); }
        public double getX(int i) { return xmin + (i - imin) * dxdi; }
        public double getY(int j) { return ymin + (j - jmin) * dydj; }
        public Complex getZ(int i, int j) { return new Complex(xmin + (i - imin) * dxdi, ymin + (j - jmin) * dydj); }

        public Color getColor(int k)     //Atentie: k trebuie sa fie pozitiv 
        {
            return paleta[k % dimPaleta];
        }

        public void setPixel(int i, int j, Color c)
        {
            if (i < imin || imax < i || j < jmin || jmax < j) return;
            bmp.SetPixel(i, jmax - j, c);
        }


        public void setPixel(double x, double y, Color c)
        {
            setPixel((int)Math.Round(imin + (x - xmin) * didx), (int)Math.Round(jmin + (y - ymin) * djdy), c);
        }

        public void setPixel(Complex z, Color c)
        {
            setPixel((int)Math.Round(imin + (z.Re - xmin) * didx), (int)Math.Round(jmin + (z.Im - ymin) * djdy), c);

        }

        public void setLine(int i0, int j0, int i1, int j1, Color c)
        {
            int i, j, dir;
            double m;   //panta dreptei
            //linie verticala:
            if (i0 == i1)
            {
                if (j0 <= j1)
                {
                    for (j = j0; j <= j1; j++)
                        setPixel(i0, j, c);
                }
                else
                {
                    for (j = j1; j <= j0; j++)
                        setPixel(i0, j, c);
                }
                return;
            }

            //linie orizontala sau oblica:          

            m = (double)(j1 - j0) / (double)(i1 - i0);
            if (-1 <= m && m <= 1)
            {
                dir = (i0 < i1 ? +1 : -1);
                i = i0;
                while (i != i1)
                {
                    setPixel(i, (int)Math.Round(j0 + m * (i - i0)), c);
                    i += dir;
                }
            }
            else     //m<-1 || m>1
            {
                dir = (j0 < j1 ? +1 : -1);
                j = j0;
                while (j != j1)
                {
                    setPixel((int)Math.Round(i0 + (j - j0) / m), j, c);
                    j += dir;
                }
            }
            setPixel(i1, j1, c);
            return;
        }

        public void setLine(double x0, double y0, double x1, double y1, Color c)
        {
            setLine((int)Math.Round(imin + (x0 - xmin) * didx), (int)Math.Round(jmin + (y0 - ymin) * djdy),
                (int)Math.Round(imin + (x1 - xmin) * didx), (int)Math.Round(jmin + (y1 - ymin) * djdy), c);
        }

        public void setLine(Complex z0, Complex z1, Color c)
        {
            setLine((int)Math.Round(imin + (z0.Re - xmin) * didx), (int)Math.Round(jmin + (z0.Im - ymin) * djdy),
                (int)Math.Round(imin + (z1.Re - xmin) * didx), (int)Math.Round(jmin + (z1.Im - ymin) * djdy), c);
        }
        public void setAxis()
        {
            setLine(xmin, 0.0, xmax, 0.0, penColor);
            setLine(0.0, ymin, 0.0, ymax, penColor);
        }

    }

}
