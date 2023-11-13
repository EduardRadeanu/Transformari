using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace ComplexExplorer
{
       
    /***********************************************************************/
    public class TransformariGeometrice : ComplexForm
    {
        delegate Complex Transform(Complex p, Complex q, Complex z);
        static Complex i = new Complex(0, 1);

        Complex Translatie(Complex a, Complex aprim, Complex z) // translatia a -> aprim
        {
            return aprim - a + z;
        }
        Complex Rotatie(Complex z0, Complex omega, Complex z) // rotatie
        {
            return z0 + omega * (z - z0);
        }
        void transforma(Complex[] fig, Transform T, Complex p, Complex q)
        {
            for (int k = 0; k < fig.Length; k++)
            {
                fig[k] = T(p, q, fig[k]);
            }
        }
        void traseaza(Complex[] fig, Color col)
        {
            for (int k = 1; k < fig.Length; k++)
            {
                setLine(fig[k - 1], fig[k], col);

            }
        }
        Complex[] figuraInitiala()
        {
            return new Complex[] { 1 + i, 2 + i / 2, 3 + i / 2,
                5 + i, 4 + 3 * i / 2, 3 + 3 * i / 2, 1 + i };
        }
        public override void makeImage()
        {
            setXminXmaxYminYmax(-5, 10, -5, 10);
            setAxis();

            Complex[] fig = figuraInitiala();
            traseaza(fig, Color.Red);

            Complex a = 0, aprim = (1 + 2 * i) / 100;
            Complex z0 = (1 + i) / 2;
            Complex omega = Complex.setRoTheta(1, Math.PI / 500);

            for (int k = 0; k < 3000; k++)
            {
                initScreen();//stergem bitmap-ul
                setAxis();
                //transforma(fig, Translatie, a, aprim);
                transforma(fig, Rotatie, z0, omega);         
                traseaza(fig, Color.Red);
                if (!resetScreen()) return;//afisam bitmap-ul
            }
        }

    }


    /***********************************************************************/
    public class RadacinileUnitatii : ComplexForm
    {
        static Complex i = new Complex(0, 1);

        public override void makeImage()
        {
            setXminXmaxYminYmax(-2, 2, -2, 2);
            PenColor = Color.LightBlue;
            ScreenColor = Color.WhiteSmoke;
            setAxis();
            int n = 9;
            double fi = 2 * Math.PI / n;
            for (int k = 0; k <= n; k++)
            {
                Complex u = Complex.setRoTheta(1, k * fi);
                Complex v = Complex.setRoTheta(1, (k + 1) * fi);
                Complex w = Complex.setRoTheta(1, (k + 2) * fi);
                setLine(0, u, Color.DarkSlateBlue);
                setLine(u, v, Color.DarkMagenta);
                setLine(u, w, Color.Red);
            }
            resetScreen();
        }

    }


    /*******************************************************************************/




    public class Caleidoscop : ComplexForm
    {
        public struct PixelMapat
        {
            public Complex Z { get; set; }
            public int II { get; set; }
            public int JJ { get; set; }

            public PixelMapat(Complex z, int ii, int jj)
                : this()
            {
                Z = z;
                II = ii;
                JJ = jj;
            }
        }
        public static Complex i = new Complex(0, 1);
        Bitmap myBmp = new Bitmap(Properties.Resources.bile_colorate);
        // Bitmap myBmp = new Bitmap(Properties.Resources.branduse);

        static double fi = Math.PI / 6;
        static double omega = 2 * Math.PI / 3;
        static double ro = 0.5;

        static Complex a0 = Complex.setRoTheta(ro, 0 * omega + fi);
        static Complex b0 = Complex.setRoTheta(ro, 1 * omega + fi);
        static Complex c0 = Complex.setRoTheta(ro, 2 * omega + fi);

        //simetria fata de dreapta PQ
        public Complex simPQ(Complex z, Complex p, Complex q)
        {
            return p + (q - p) / (q - p).Conj * (z - p).Conj;
        }

        //testare daca z este in dreapta laturii a->b:       
        bool esteInStanga(Complex z, Complex a, Complex b)
        {
            if (((z - a) / (b - a)).Theta >= 0) return true;
            return false;
        }

        //testare daca z este in triunghiul abc
        bool esteInTriunghi(Complex z, Complex a, Complex b, Complex c)
        {
            bool ab = esteInStanga(z, a, b);
            bool bc = esteInStanga(z, b, c);
            bool ca = esteInStanga(z, c, a);
            return ab == bc && bc == ca;
        }

        int myGetI(Complex z)
        {
            int ii = getI(z.Re);
            if (ii < imin) return imin;
            if (ii > imax) return imax;
            return ii;
        }
        int myGetJ(Complex z)
        {
            int jj = getJ(z.Im);
            if (jj < jmin) return jmin;
            if (jj > imax) return jmax;
            return jj;
        }

        List<PixelMapat> mapareReflexii(int kmax)
        {
            List<PixelMapat> rez = new List<PixelMapat>();
            Complex[,] Z = new Complex[dimPicBmp, dimPicBmp];
            for (int ii = imin; ii <= imax; ii++)
                for (int jj = jmin; jj <= jmax; jj++)
                {
                    Z[ii, jj] = getZ(ii, jj);
                }
            Complex a = a0;
            Complex b = b0;
            Complex c = c0;
            for (int k = 0; k < kmax; k++)
            {
                for (int ii = 0; ii < dimPicBmp; ii++)
                    for (int jj = 0; jj < dimPicBmp; jj++)
                    {
                        Complex z = getZ(ii, jj);
                        if (esteInStanga(z, a, c))
                        {
                            Complex zz = simPQ(z, a, c);
                            Z[ii, jj] = Z[myGetI(zz), myGetJ(zz)];
                            continue;
                        }
                        if (esteInStanga(z, c, b))
                        {
                            Complex zz = simPQ(z, c, b);
                            Z[ii, jj] = Z[myGetI(zz), myGetJ(zz)];
                            continue;
                        }
                        if (esteInStanga(z, b, a))
                        {
                            Complex zz = simPQ(z, b, a);
                            Z[ii, jj] = Z[myGetI(zz), myGetJ(zz)];
                            continue;
                        }
                    }
                //formam noul triunghi abc  
                Complex anou = simPQ(a, b, c);
                Complex bnou = simPQ(b, c, a);
                Complex cnou = simPQ(c, a, b);
                a = anou;
                b = bnou;
                c = cnou;
            }
            //decupam triunghiul final
            for (int ii = imin; ii <= imax; ii++)
            {
                for (int jj = jmin; jj <= jmax; jj++)
                {
                    Complex z = getZ(ii, jj);
                    if (esteInTriunghi(z, a, b, c)) rez.Add(new PixelMapat(Z[ii, jj], ii, jj));

                }
            }
            return rez;
        }

        public override void makeImage()
        {
            setXminXmaxYminYmax(-4, 4, -3, 5);
            List<PixelMapat> li = mapareReflexii(3);

            //plimbam caleidoscopul prin myBmp:
            Complex z0 = -1.5 * i;
            double delta = Math.PI / 120;
            for (double t = 0; ; t += delta)
            {
                Complex rotor = Complex.setRoTheta(1, t);
                foreach(PixelMapat px in li)
                {
                    Complex z = z0 + rotor * (px.Z - z0);
                    setPixel(px.II, px.JJ, myBmp.GetPixel(myGetI(z), myGetJ(z)));

                }
                setLine(a0, b0, PenColor);
                setLine(b0, c0, PenColor);
                setLine(c0, a0, PenColor);
                if (!resetScreen()) return;
            }
        }
    }
    /*******************************************************************************/

   

}
