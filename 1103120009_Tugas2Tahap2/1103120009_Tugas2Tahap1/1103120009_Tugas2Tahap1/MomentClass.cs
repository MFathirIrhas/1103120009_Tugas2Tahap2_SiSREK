using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Data.SqlTypes;


namespace _1103120009_Tugas2Tahap1
{
    public class MomentClass 
    {
        public static BitmapData PolarData;
        public static int Polarstride;
        public static System.IntPtr PolarPtr;
        public Point[] point;

        public static Bitmap image;
        private int area = 0;
        private double centerVertical;
        private double centerHorizontal;
        private int Rows;
        private int Cols;
        private int mark;
        private int r, c;
        public static BitmapData data;
        public static int stride;
        public static System.IntPtr ptr;
        public static int nOffset;

        /*
            urutan eksekusi: 
         * --> 1. Menghitung Moment Area(Moment of Order) :> *MomentArea()
         * --> 2. Menghitung Central Moment :> *CentralMoment()
         * --> 3. Menghitung normalisasi central moment , CentralMoment()/momentarea :> *NormalizeCentralMoment()
         * --> 4. Menghitung Invariant Moment :> InvariantMoment() , dan setelah mendapatkan eta-nya, hitung 7 Invariants moment
        */
        public MomentClass() { }
        public MomentClass(int finalX,int finalY,int width,int height,int m)
        {
            mark = m;
            c = finalX;
            r = finalY;
            this.Rows = height;
            this.Cols = width;
            MomentArea();

        }

        private void MomentArea()
        {
            double momentV = 0;
            double momentH = 0;

            int val;

            unsafe
            {
                byte* p = (byte*)(void*)ptr;

                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Cols; col++)
                    {
                        val = (p + (r + row) * stride + (c + col) * 3)[0];
                        if (val == mark)
                        {
                            ++area;
                            momentV += row;
                            momentH += col;
                        }
                    }
                }
            }
            centerVertical = momentV / area;
            centerHorizontal = momentH / area;

        }

        public double CentralMoment(int p, int q, int objectId)
        {
            double v, h;
            int pv;
            double cm = 0;

            unsafe
            {
                byte* pt = (byte*)(void*)ptr;

                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Cols; col++)
                    {
                        pv = ((pt + (r + row) * stride + (c + col) * 3))[0];
                        if (pv == objectId)
                        {
                            v = row - centerVertical;
                            h = col - centerHorizontal;

                            cm += Math.Pow(v, p) * Math.Pow(h, q);
                        }
                    }
                }
            }
            return cm;
        }


        public double NormalizeCentralMoment(int p, int q, int objectId)
        {
            double cm = CentralMoment(p, q, objectId);
            double gamma = (p + q) / 2.0 + 1;
            double nmc = cm / Math.Pow(area, gamma);

            return nmc;
        }

        public double InvariantMoment(int n, int objectId)
        {
            double invMoment = 0;

            double eta11 = 0, eta02 = 0, eta20 = 0, eta03 = 0, eta30 = 0, eta21 = 0, eta12 = 0;

            if (n <= 2 || n == 6)
            {
                eta20 = NormalizeCentralMoment(2, 0, objectId);
                eta02 = NormalizeCentralMoment(0, 2, objectId);
            }

            if (n >= 3)
            {
                eta12 = NormalizeCentralMoment(1, 2, objectId);
                eta21 = NormalizeCentralMoment(2, 1, objectId);
                eta03 = NormalizeCentralMoment(0, 3, objectId);
                eta30 = NormalizeCentralMoment(3, 0, objectId);
            }

            if (n == 1)
            {
                invMoment = eta20 + eta02;
            }
            else if (n == 2)
            {
                eta11 = NormalizeCentralMoment(1, 1, objectId);

                invMoment = Math.Pow(eta20 - eta02, 2) + 4 * Math.Pow(eta11, 2);
            }
            else if (n == 3)
            {
                invMoment = Math.Pow(eta30 - 3 * eta12, 2) + Math.Pow(eta03 - 3 * eta21, 2);
            }
            else if (n == 4)
            {
                invMoment = Math.Pow(eta30 + 3 * eta12, 2) + Math.Pow(eta03 + 3 * eta21, 2);
            }
            else if (n == 5)
            {
                invMoment = (eta03 - 3 * eta12) * (eta30 + eta12) *
                    (Math.Pow(eta30 + eta12, 2) - 3 * Math.Pow(eta21 + eta03,2))
                    + (3 * eta21 - eta03) * (eta21 + eta03) *
                    (3 * Math.Pow(eta30 + eta12, 2) - Math.Pow(eta21 + eta30,2));
            }
            else if (n == 6)
            {
                invMoment = (eta02 - eta20) *
                    (Math.Pow(eta30 + eta12, 2) - Math.Pow(eta21 + eta03,2))
                    + 4 * eta11 * (eta30 + eta12) * (eta21 + eta03);
            }
            else if (n == 7)
            {
                invMoment = (3 * eta21 - eta03) * (eta30 + eta12) *
                    (Math.Pow(eta30 + eta12,2) - 3 * (eta21 + eta03))
                    + 3 * (eta12 - eta30) * (eta21 + eta03) *
                    (3 * Math.Pow(eta30 + eta12, 2) - Math.Pow(eta21 + eta03,2));
            }

            return invMoment;
        }
    }
}
