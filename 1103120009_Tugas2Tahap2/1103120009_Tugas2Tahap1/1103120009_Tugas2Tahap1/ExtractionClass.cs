using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;


namespace _1103120009_Tugas2Tahap1
{
    public class ExtractionClass
    {
        private BitmapData data;
        private int stride;
        private System.IntPtr ptr;
        private int nOffset;
        public int line = 0;
        private int startX, finalX;
        private int startY = int.MaxValue;
        private int finalY = int.MinValue;

        public Blob ConnectedComponents = new Blob();
        public readonly static int neighborhoodV = 1;

        private Bitmap image;
        public Bitmap BinaryImage;
        private int Rows;
        private int Cols;
        private int mark;
        public static System.IO.StreamWriter finalFile;

        //SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\DataModel.mdf;Integrated Security=True;");
        string con = @"Data Source=(localdb)\v11.0;Initial Catalog=DataModel;Integrated Security=True";
        string sql;

        //
        double NApple, NBanana, NPear, NStrawberry;
        public ExtractionClass(Bitmap b)
        {
            this.image = b;
        }

        /*
         * Melakukan ekstraksi pada gambar dengan cara memberi tanda pada gambar yang bertanda hitam(pointer menunjuk nilai dengan byte = 0), karena hitam bernilai 0,
         * dan konsentrasi objek gambar adalah hitam, setelah menandai objek dengan penanda(mark), maka objek hasil tanda ditambah ke dalam list objek Buah
         * Sedang proses ektraksi berjalan, data-data objek seperti hasil lockbits,scan0,stride dan offset, di transfer ke kelas Moment untuk pemosresan Invariant Moment.
         */
        /*
         * Metode Segmentasi menggunakan konsep CONNECTED-COMPONENT ANALYSIS, yaitu mencari suatu pixel di gambar yang merupakan bagian dari objek(nilai pixel=0), setelah ketemu, mencari ketetanggaannya, 8 tetangga yang konek dengannya
         * akan di label jika belum pernah dikunjungi.
         */

        #region | SEGMENT |
        //Proses segmentasi image dengan cara memberi label untuk setiap Connected-Component yang ada pada Image.
        public void Segment(Bitmap b)
        {
            this.Rows = b.Height;//-(2*(b.Height/3));
            this.Cols = b.Width;//- (2* (b.Width / 3));
            data = b.LockBits(new Rectangle(0, 0, this.Cols, this.Rows), ImageLockMode.ReadWrite, b.PixelFormat);
            stride = data.Stride;
            ptr = data.Scan0;
            nOffset = stride - b.Width * 3;

            MomentClass.data = data;
            MomentClass.stride = data.Stride;
            MomentClass.ptr = data.Scan0;
            MomentClass.nOffset = nOffset;
            unsafe
            {
                byte* p;

                p = (byte*)(void*)ptr;
                //p+=stride;
                for (int y = 1; y < Rows /*- 1*/; y++)
                {
                    //p+=3;											
                    for (int x = 1; x < Cols/*3 - 3*/; x++)
                    {
                        //if (p[0] == 0)
                        if ((p + stride * y + x * 3)[0] == 0)
                        {
                            mark++;
                            try
                            {
                                startY = 0;//int.MaxValue;//10000;
                                finalY = b.Height;//int.MaxValue;
                                startX = 0;//int.MaxValue;
                                finalX = b.Width;//int.MaxValue;
                                
                                search(mark, y, x);
                                Blob blob = new Blob();
                                blob.StartX = startX;
                                blob.StartY = startY;
                                blob.FinalX = finalX;
                                blob.FinalY = finalY;
                                blob.Mark = mark;
                                ConnectedComponents.Add(blob); //array yang menampung seluruh connected-component yang ditemukan selama proses scanning image.
                            }
                            catch (System.StackOverflowException e)
                            {
                                MessageBox.Show("Sorry, Cannot Extract Image", "Error");
                                return;
                            }
                        }

                        //p += 1;
                    }
                    //p += 3;
                    //p += nOffset;
                }
                //int t = 0;
                //t++;
            }
            b.UnlockBits(data);
        }
        #endregion

        #region | SEGMENT 2 |
        public void Segment2(Bitmap b, int xx)
        {
            this.Rows = b.Height ;
            this.Cols = b.Width- (2* (b.Width / 3)) ;
            data = b.LockBits(new Rectangle(xx*Cols, Rows, this.Cols, this.Rows), ImageLockMode.ReadWrite, b.PixelFormat);
            stride = data.Stride;
            ptr = data.Scan0;
            nOffset = stride - b.Width * 3;

            MomentClass.data = data;
            MomentClass.stride = data.Stride;
            MomentClass.ptr = data.Scan0;
            MomentClass.nOffset = nOffset;
            unsafe
            {
                byte* p;

                p = (byte*)(void*)ptr;
                //p+=stride;
                for (int y = 1; y < Rows /*- 1*/; y++)
                {
                    //p+=3;											
                    for (int x = 1; x < Cols /*- 3*/; x++)
                    {
                        if ((p + x * 3 + stride * y)[0] == 0)
                        {
                            mark++;
                            try
                            {
                                startY = 0;//int.MaxValue;//10000;
                                finalY = b.Height;//int.MaxValue;
                                startX = 0;//int.MaxValue;
                                finalX = b.Width;//int.MaxValue;

                                search(mark, y, x);
                                Blob blob = new Blob();
                                blob.StartX = startX;
                                blob.StartY = startY;
                                blob.FinalX = finalX;
                                blob.FinalY = finalY;
                                blob.Mark = mark;
                                ConnectedComponents.Add(blob); //array yang menampung seluruh connected-component yang ditemukan selama proses scanning image.
                            }
                            catch (System.StackOverflowException e)
                            {
                                MessageBox.Show("Sorry, Cannot Extract Image", "Error");
                                return;
                            }
                        }

                    }
                }
                int t = 0;
                t++;
            }
            b.UnlockBits(data);
        }
        #endregion

        #region | Hitung Objek Segmentasi |
        private void Segmentasi(Bitmap b)
        {
            for (int xx = 0; xx < 3; xx++)
            {
                   
            }
        }
        #endregion
        //Mencari pixel-pixel tetangga
        private void search(int mark, int r, int c)
        {
            //if (r < startY)
            //    startY = r;

            //if (r > finalY)
            //    finalY = r;

            //if (c > finalX)
            //    finalX = c;

            //if (c < startX)
            //    startX = c;
            if (r > startY)
                startY = r;
            if (r < finalY)
                finalY = r;
            if (c < finalX)
                finalX = c;
            if (c > startX)
                startX = c;


            try
            {

                int[] nb = { r, c };
                ArrayList nbList = findNeighbours(nb);
                unsafe
                {
                    byte* p = (byte*)(void*)ptr;
                    (p + r * stride + c * 3)[0] = (byte)mark;//blue
                    (p + r * stride + c * 3)[1] = (byte)mark;//green
                    (p + r * stride + c * 3)[2] = (byte)mark;//red 

                    for (int i = 0; i < nbList.Count; i++)
                    {
                        int[] pos = (int[])nbList[i];
                        if ((p + pos[0] * stride + pos[1] * 3)[0] == 0)
                            search(mark, pos[0], pos[1]);
                    }
                }
            }
            catch (System.StackOverflowException e)
            {
                return;
            }
        }

        private ArrayList findNeighbours(int[] pos)
        {
            ArrayList nbList;
            //nbList = find4Neighbours(pos);
            nbList = find8ConnectedN(pos);
            //nbList = findDNeighbours(pos);
            return nbList;
        }

        #region Visited Neighbours Algorithm for 4 Neighbourhood 
        private ArrayList find4ConnectedN(int[] pos)
        {
            ArrayList nbList = new ArrayList();

            if (pos[0] > 0)
                addNeighbour(pos[0] - 1, pos[1], nbList);
            if (pos[1] > 0)
                addNeighbour(pos[0], pos[1] - 1, nbList);
            if (pos[1] < Cols - 1)
                addNeighbour(pos[0], pos[1] + 1, nbList);
            if (pos[0] < Rows - 1)
                addNeighbour(pos[0] + 1, pos[1], nbList);

            return nbList;
        }

        private ArrayList find4ConnectedN2(int[] pos)
        {
            ArrayList nbList = new ArrayList();

            if ((pos[0] > 0) && (pos[1] > 0))
                addNeighbour(pos[0] - 1, pos[1] - 1, nbList);
            if ((pos[0] > 0) && (pos[1] < Cols - 1))
                addNeighbour(pos[0] - 1, pos[1] + 1, nbList);
            if ((pos[0] < Rows - 1) && (pos[1] > 0))
                addNeighbour(pos[0] + 1, pos[1] - 1, nbList);
            if ((pos[0] < Rows - 1) && (pos[1] < Cols - 1))
                addNeighbour(pos[0] + 1, pos[1] + 1, nbList);



            return nbList;
        }
        #endregion

        //Mencari 8 tetangga dari suatu pixel yang dikunjungi 
        private ArrayList find8ConnectedN(int[] pos)
        {
            ArrayList nbList = new ArrayList();

            if ((pos[0] > 0) && (pos[1] > 0))
                addNeighbour(pos[0] - 1, pos[1] - 1, nbList); //barat laut
            if (pos[0] > 0)
                addNeighbour(pos[0] - 1, pos[1], nbList); //utara
            if ((pos[0] > 0) && (pos[1] < Cols - 1))
                addNeighbour(pos[0] - 1, pos[1] + 1, nbList); //timur laut
            if (pos[1] > 0)
                addNeighbour(pos[0], pos[1] - 1, nbList);//barat
            if (pos[1] < Cols - 1)
                addNeighbour(pos[0], pos[1] + 1, nbList); // timur
            if ((pos[0] < Rows - 1) && (pos[1] > 0))
                addNeighbour(pos[0] + 1, pos[1] - 1, nbList); //barat daya
            if (pos[0] < Rows - 1)
                addNeighbour(pos[0] + 1, pos[1], nbList); //selatan
            if ((pos[0] < Rows - 1) && (pos[1] < Cols - 1))
                addNeighbour(pos[0] + 1, pos[1] + 1, nbList); //tenggara

            return nbList;
        }

        private void addNeighbour(int r, int c, ArrayList list)
        {
            int[] nb = { r, c };
            list.Add(nb);
        }

        #region | Extraction |
        public void Extraction(string namafile)
        {

            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            //System.IO.StreamWriter sr = new System.IO.StreamWriter(projectPath + @"/DatabaseEkstraksi/"+namafile+".rtf");
            //System.IO.StreamWriter sr = new System.IO.StreamWriter(@"F:/TestingSisrek/"+namafile+".rtf");
            System.IO.StreamWriter sr = new System.IO.StreamWriter(@"F:/TestingSisrek/TESTING.rtf");
            string str = "";
            str += "Invariant Moments: " + namafile + " \n\n";
            int i = 1;
            //Buah b = new Buah();
            foreach (Blob b in this.ConnectedComponents)
            {
                b.mc = new MomentClass(b.FinalX, b.FinalY, b.getWidth(), b.getHeight(), b.Mark);
                str += i + ":";
                str += "\n";
                for (int u = 1; u <= 7; u++)
                    str += "M" + u + "   :" + b.mc.InvariantMoment(u, b.Mark).ToString() + "  \n";
                str += "\n\n";
                sr.Write(str);
                str = null;
                i++;
            }


            sr.Flush();
            sr.Close();
        }
        #endregion 

        #region //tidak jadi digunakan
        public double CariDiApple(double imApple1,double imApple2,double imApple3,double imApple4,double imApple5,double imApple6,double imApple7)
        {
            List<double> listUApple = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            sql = "select * from Apple";
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) 
            {
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - imApple1);
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - imApple2);
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - imApple3);
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - imApple4);
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - imApple5);
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - imApple6);
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - imApple7);
                double Nh = Math.Sqrt(Math.Pow(h1,2)+Math.Pow(h2,2)+Math.Pow(h3,2)+Math.Pow(h4,2)+Math.Pow(h5,2)+Math.Pow(h6,2)+Math.Pow(h7,2));
                listUApple.Add(Nh);

            }
            sqlcon.Close();
            
            NApple = listUApple.Min();

            return NApple;
        }

        public double CariDiBanana()
        {
            return NBanana;
        }

        public double CariDiPear()
        {
            return NPear;
        }

        public double CariDiStrawberry()
        {
            return NStrawberry;
        }
        #endregion
    }
}
