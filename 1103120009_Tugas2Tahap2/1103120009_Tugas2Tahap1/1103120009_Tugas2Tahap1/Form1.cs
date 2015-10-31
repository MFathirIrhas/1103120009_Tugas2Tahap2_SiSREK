using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections;

namespace _1103120009_Tugas2Tahap1
{
    public partial class Form1 : Form
    {
        Image OriginalImage;
        private Bitmap b;
        private ExtractionClass ec;
        string filepath;

        /// <summary>
        /// 
        /// </summary>

        //From Extraction Class
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
        //end 
        //public Blob ConnectedComponents = new Blob();
        string con = @"Data Source=(localdb)\v11.0;Initial Catalog=DataModel;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select Image";
            open.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(open.FileName);
                textBox1.Text = open.FileName.ToString();
                OriginalImage = pictureBox1.Image;
                filepath = open.FileName;
                b = new Bitmap(pictureBox1.Image);
            }
            ec = new ExtractionClass(b);
        }

        public Bitmap Inverse(Bitmap bmp)
        {

            Color c;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    c = bmp.GetPixel(i, j);
                    bmp.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            ec.Segment(b);
            pictureBox1.Image = b;

            #region |Khusus Untuk Penyimpanan Ke Database|
            //Untuk Penyimpanan Ekstraksi Ciri Ke Database
            //this.Rows = b.Height;//-(2*(b.Height/3));
            //this.Cols = b.Width;//- (2* (b.Width / 3));
            //data = b.LockBits(new Rectangle(0, 0, this.Cols, this.Rows), ImageLockMode.ReadWrite, b.PixelFormat);
            //stride = data.Stride;
            //ptr = data.Scan0;
            //nOffset = stride - b.Width * 3;

            //MomentClass.data = data;
            //MomentClass.stride = data.Stride;
            //MomentClass.ptr = data.Scan0;
            //MomentClass.nOffset = nOffset;
            //unsafe
            //{
            //    byte* p;

            //    p = (byte*)(void*)ptr;
            //    //p+=stride;
            //    for (int y = 1; y < Rows /*- 1*/; y++)
            //    {
            //        //p+=3;											
            //        for (int x = 1; x < Cols/*3 - 3*/; x++)
            //        {
            //            //if (p[0] == 0)
            //            if ((p + stride * y + x * 3)[0] == 0)
            //            {
            //                mark++;
            //                try
            //                {
            //                    startY = 0;//int.MaxValue;//10000;
            //                    finalY = b.Height;//int.MaxValue;
            //                    startX = 0;//int.MaxValue;
            //                    finalX = b.Width;//int.MaxValue;

            //                    search(mark, y, x);
            //                    Blob blob = new Blob();
            //                    blob.StartX = startX;
            //                    blob.StartY = startY;
            //                    blob.FinalX = finalX;
            //                    blob.FinalY = finalY;
            //                    blob.Mark = mark;
            //                    ConnectedComponents.Add(blob); //array yang menampung seluruh connected-component yang ditemukan selama proses scanning image.
            //                }
            //                catch (System.StackOverflowException ex)
            //                {
            //                    MessageBox.Show("Sorry, Cannot Extract Image", "Error");
            //                    return;
            //                }
            //            }

            //            //p += 1;
            //        }
            //        //p += 3;
            //        //p += nOffset;
            //    }
            //    //int t = 0;
            //    //t++;
            //}
            //b.UnlockBits(data);
            /////

            //pictureBox1.Image = b;
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            b = new Bitmap(pictureBox1.Image);
            string namafile = Path.GetFileName(filepath);
            ec.Extraction(namafile);
            MessageBox.Show("Ekstraksi Ciri Berhasil", "Berhasil");

            #region |Khusus untuk penyimpanan ke Database|
            /*
             * Jika ingin menambahkan Model Data untuk template berisi data training, UnComment Kodingan dibawah ini.
             * 
             */
            ////Menyimpan Ektraksi ciri ke Database
            //int i = 1;
            //string con = @"Data Source=(localdb)\v11.0;Initial Catalog=DataModel;Integrated Security=True";
            //SqlConnection sqlcon = new SqlConnection(con);
            //foreach (Blob b in this.ConnectedComponents)
            //{
            //    b.mc = new MomentClass(b.FinalX, b.FinalY, b.getWidth(), b.getHeight(), b.Mark);
            //    sqlcon.Open();
            //    string query = "insert into DataTesting(blob,m1,m2,m3,m4,m5,m6,m7) values(" + i + "," + b.mc.InvariantMoment(1, b.Mark) + "," + b.mc.InvariantMoment(2, b.Mark) + "," + b.mc.InvariantMoment(3, b.Mark) + "," + b.mc.InvariantMoment(4, b.Mark) + "," + b.mc.InvariantMoment(5, b.Mark) + "," + b.mc.InvariantMoment(6, b.Mark) + "," + b.mc.InvariantMoment(7, b.Mark) + ")";
            //    SqlCommand cmd = new SqlCommand(query, sqlcon);
            //    if (!double.IsNaN(b.mc.InvariantMoment(1, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(2, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(3, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(4, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(5, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(6, b.Mark)) && !double.IsNaN(b.mc.InvariantMoment(7, b.Mark)))
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //    else if (double.IsNaN(b.mc.InvariantMoment(1, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(2, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(3, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(4, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(5, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(6, b.Mark)) && double.IsNaN(b.mc.InvariantMoment(7, b.Mark)))
            //    {
            //        string query2 = "insert into DataTesting(blob,m1,m2,m3,m4,m5,m6,m7) values(" + i + ",0,0,0,0,0,0,0)";
            //        SqlCommand cmd2 = new SqlCommand(query2, sqlcon);
            //        cmd2.ExecuteNonQuery();
            //    }

            //    sqlcon.Close();
            //    i++;

            //}
            //MessageBox.Show("Testing Data Fetched Successfully", "Succed");
            #endregion


        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            pictureBox1.Image = Inverse(bmp);
        }


        #region METHOD Mencari Connected-Component dan Menandai Connected-Component(Khusus Untuk Penyimpanan Ekstraksi Ciri ke Database)
        //Method Tambahan
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
                    (p + r * stride + c * 3)[2] = (byte)mark;//red // (mark * 20);

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

        #region Visited Neighbours
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
        //end of method tambahan
        #endregion


        //Deteksi Bentuk
        private void button4_Click(object sender, EventArgs e)
        {
            //richTextBox1.Text = Math.Abs(4-5).ToString();
            //double tes = ec.CariDiApple(5,5,5,5,5,5,5);
            //double tes = CariDiApple(5, 5, 5, 5, 5, 5, 5);
            //richTextBox1.Text = tes.ToString();
            string statement ;
            double n1 = SmallestNApple();
            double n2 = SmallestNBanana();
            double n3 = SmallestNPear();
            double n4 = SmallestNStrawberry();
            List<double> list = new List<double>() {n1,n2,n3,n4 }; // 0, 0 , 0.23, 0.4

            double n12 = SmallestNApple2nd();
            double n22 = SmallestNBanana2nd();
            double n32 = SmallestNPear2nd();
            double n42 = SmallestNStrawberry2nd();
            List<double> list2 = new List<double>() { n12, n22, n32, n42 }; // 

            double n13 = SmallestNApple3rd();
            double n23 = SmallestNBanana3rd();
            double n33 = SmallestNPear3rd();
            double n43 = SmallestNStrawberry3rd();
            List<double> list3 = new List<double>() { n13, n23, n33, n43 };

            /*
             * Apple Threshold = n1 <= x <= n13
             * Banana Threshold = n2 <= x <= n23
             * Pear Threshold = n3 <= x <= n33
             * Strawberry Threshold = n4 <= x <= n43
             */

            List<double> listALL = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            SqlConnection sqlcon3 = new SqlConnection(con);
            SqlConnection sqlcon4 = new SqlConnection(con);
            SqlConnection sqlconT = new SqlConnection(con);

            string sql = "select * from Apple";
            string sql2 = "select * from Banana";
            string sql3 = "select * from Pear";
            string sql4 = "select * from Strawberry";
            string sqlT = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            sqlcon3.Open();
            sqlcon4.Open();
            sqlconT.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmd2 = new SqlCommand(sql2, sqlcon2);
            SqlCommand cmd3 = new SqlCommand(sql3, sqlcon3);
            SqlCommand cmd4 = new SqlCommand(sql4, sqlcon4);
            SqlCommand cmdT = new SqlCommand(sqlT, sqlconT);
            SqlDataReader d = cmd.ExecuteReader();
            SqlDataReader d2 = cmd2.ExecuteReader();
            SqlDataReader d3 = cmd3.ExecuteReader();
            SqlDataReader d4 = cmd4.ExecuteReader();
            SqlDataReader dT = cmdT.ExecuteReader();

            while (d.Read() && d2.Read() && d3.Read() && d4.Read() && dT.Read() )
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double ah1 = Math.Abs(Convert.ToDouble(d["m1"]) - Convert.ToDouble(dT["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double ah2 = Math.Abs(Convert.ToDouble(d["m2"]) - Convert.ToDouble(dT["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double ah3 = Math.Abs(Convert.ToDouble(d["m3"]) - Convert.ToDouble(dT["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double ah4 = Math.Abs(Convert.ToDouble(d["m4"]) - Convert.ToDouble(dT["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double ah5 = Math.Abs(Convert.ToDouble(d["m5"]) - Convert.ToDouble(dT["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double ah6 = Math.Abs(Convert.ToDouble(d["m6"]) - Convert.ToDouble(dT["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double ah7 = Math.Abs(Convert.ToDouble(d["m7"]) - Convert.ToDouble(dT["m7"]));//b.mc.InvariantMoment(7, b.Mark));

                double NhA = Math.Sqrt(Math.Pow(ah1, 2) + Math.Pow(ah2, 2) + Math.Pow(ah3, 2) + Math.Pow(ah4, 2) + Math.Pow(ah5, 2) + Math.Pow(ah6, 2) + Math.Pow(ah7, 2));
                listALL.Add(NhA);


                double bh1 = Math.Abs(Convert.ToDouble(d2["m1"]) - Convert.ToDouble(dT["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double bh2 = Math.Abs(Convert.ToDouble(d2["m2"]) - Convert.ToDouble(dT["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double bh3 = Math.Abs(Convert.ToDouble(d2["m3"]) - Convert.ToDouble(dT["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double bh4 = Math.Abs(Convert.ToDouble(d2["m4"]) - Convert.ToDouble(dT["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double bh5 = Math.Abs(Convert.ToDouble(d2["m5"]) - Convert.ToDouble(dT["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double bh6 = Math.Abs(Convert.ToDouble(d2["m6"]) - Convert.ToDouble(dT["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double bh7 = Math.Abs(Convert.ToDouble(d2["m7"]) - Convert.ToDouble(dT["m7"]));//b.mc.InvariantMoment(7, b.Mark));

                double NhB = Math.Sqrt(Math.Pow(bh1, 2) + Math.Pow(bh2, 2) + Math.Pow(bh3, 2) + Math.Pow(bh4, 2) + Math.Pow(bh5, 2) + Math.Pow(bh6, 2) + Math.Pow(bh7, 2));
                listALL.Add(NhB);

                double ph1 = Math.Abs(Convert.ToDouble(d3["m1"]) - Convert.ToDouble(dT["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double ph2 = Math.Abs(Convert.ToDouble(d3["m2"]) - Convert.ToDouble(dT["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double ph3 = Math.Abs(Convert.ToDouble(d3["m3"]) - Convert.ToDouble(dT["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double ph4 = Math.Abs(Convert.ToDouble(d3["m4"]) - Convert.ToDouble(dT["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double ph5 = Math.Abs(Convert.ToDouble(d3["m5"]) - Convert.ToDouble(dT["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double ph6 = Math.Abs(Convert.ToDouble(d3["m6"]) - Convert.ToDouble(dT["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double ph7 = Math.Abs(Convert.ToDouble(d3["m7"]) - Convert.ToDouble(dT["m7"]));//b.mc.InvariantMoment(7, b.Mark));

                double NhP = Math.Sqrt(Math.Pow(ph1, 2) + Math.Pow(ph2, 2) + Math.Pow(ph3, 2) + Math.Pow(ph4, 2) + Math.Pow(ph5, 2) + Math.Pow(ph6, 2) + Math.Pow(ph7, 2));
                listALL.Add(NhP);

                double sh1 = Math.Abs(Convert.ToDouble(d4["m1"]) - Convert.ToDouble(dT["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double sh2 = Math.Abs(Convert.ToDouble(d4["m2"]) - Convert.ToDouble(dT["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double sh3 = Math.Abs(Convert.ToDouble(d4["m3"]) - Convert.ToDouble(dT["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double sh4 = Math.Abs(Convert.ToDouble(d4["m4"]) - Convert.ToDouble(dT["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double sh5 = Math.Abs(Convert.ToDouble(d4["m5"]) - Convert.ToDouble(dT["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double sh6 = Math.Abs(Convert.ToDouble(d4["m6"]) - Convert.ToDouble(dT["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double sh7 = Math.Abs(Convert.ToDouble(d4["m7"]) - Convert.ToDouble(dT["m7"]));//b.mc.InvariantMoment(7, b.Mark));

                double NhS = Math.Sqrt(Math.Pow(sh1, 2) + Math.Pow(sh2, 2) + Math.Pow(sh3, 2) + Math.Pow(sh4, 2) + Math.Pow(sh5, 2) + Math.Pow(sh6, 2) + Math.Pow(sh7, 2));
                listALL.Add(NhS);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            sqlcon3.Close();
            sqlcon4.Close();
            sqlconT.Close();
            

            
            int counterA = 0;
            int counterB = 0;
            int counterP = 0;
            int counterS = 0;
            int counterU = 0;
            double M;
            //if()
            int k  = 0;
            int ka = 0;
            int kb = 0;
            int kp = 0;
            int ks = 0;
            int l = 0;
            int la = 0;
            int lb = 0;
            int lp = 0;
            int ls = 0;
            string a = "Apple           : ";
            string b = "Banana          : ";
            string p = "Pear            : ";
            string s = "Strawberry      : ";
            string u = "Unknown         : ";
            //while (ka <= listALL.Count && kb <= listALL.Count && kp <= listALL.Count && ks <= listALL.Count && k <= listALL.Count)
            //{
            //    if (listALL.OrderBy(r => r).Skip(la).First() >= n1 && listALL.OrderBy(r => r).Skip(la).First() <= n12)
            //    {
            //        counterA = counterA + 1;
            //        la++;
            //        ka++;
            //        //a += counterA + "\n";
            //        //richTextBox1.Text += counterA + "\n";

            //    }
            //    else if (listALL.OrderBy(r => r).Skip(lb).First() >= n2 && listALL.OrderBy(r => r).Skip(lb).First() <= n23 || listALL.OrderBy(r => r).Skip(lb).First() == n2 || listALL.OrderBy(r => r).Skip(lb).First() == n22 || listALL.OrderBy(r => r).Skip(lb).First() == n23)
            //    {
            //        counterB = counterB + 1;
            //        lb++;
            //        kb++;
            //        //b += counterB + "\n";
            //        //richTextBox1.Text += counterB + "\n";
            //    }
            //    else if (listALL.OrderBy(r => r).Skip(lp).First() >= n3 && listALL.OrderBy(r => r).Skip(lp).First() <= n32)
            //    {
            //        counterP = counterP + 1;
            //        lp++;
            //        kp++;
            //        //p += counterP + "\n";
            //        //richTextBox1.Text += counterP + "\n";
            //    }
            //    else if (listALL.OrderBy(r => r).Skip(ls).First() >= n4 && listALL.OrderBy(r => r).Skip(ls).First() <= n43)
            //    {
            //        counterS = counterS + 1;
            //        ls++;
            //        ks++;
            //        //s += counterS + "\n";
            //        //richTextBox1.Text += counterS + "\n";
            //    }
            //    else if (listALL.OrderBy(r => r).Skip(l).First() >= n13 && listALL.OrderBy(r => r).Skip(l).First() >= n23 && listALL.OrderBy(r => r).Skip(l).First() >= n33 && listALL.OrderBy(r => r).Skip(l).First() >= n43)
            //    {
            //        counterU = counterU + 1;
            //        l++;
            //        k++;
            //    }
                //ka++;
                //l++;
                //l++;
            //}

            while (ka <= 1)//listALL.Count)
            {
                //if (listALL.OrderBy(r => r).Skip(la).First() >= n1 && listALL.OrderBy(r => r).Skip(la).First() <= n12)
                if(n1 <= 1  && n12 <= 1 && n13 <= 1f)
                {
                    counterA = counterA + 1;
                    la++;
                    //break;
                    //a += counterA + "\n";
                    //richTextBox1.Text += counterA + "\n";

                }

                
                ka++;
            }

            while (kb <= 1)//listALL.Count)
            {
                //if (listALL.OrderBy(r => r).Skip(lb).First() >= n2 && listALL.OrderBy(r => r).Skip(lb).First() <= n23)
                if (n2 <= 1 && n22 <= 1 && n23 <= 1f)
                {
                    counterB = counterB + 1;
                    lb++;
                    
                    //b += counterB + "\n";
                    //richTextBox1.Text += counterB + "\n";
                }

                
                //lb = lb + 1 ;
                kb++;
            }

            while (kp <= 1)//listALL.Count)
            {
                //if (listALL.OrderBy(r => r).Skip(lp).First() >= n3 && listALL.OrderBy(r => r).Skip(lp).First() <= n33)
                if (n3 <= 1 && n32 <= 1 && n33 <= 1f)
                {
                    counterP = counterP + 1;
                    lp++;
                    
                    //p += counterP + "\n";
                    //richTextBox1.Text += counterP + "\n";
                }
                kp++;
            }

            while (ks <= 1)//listALL.Count)
            {
                //if (listALL.OrderBy(r => r).Skip(ls).First() == n4 )//&& listALL.OrderBy(r => r).Skip(ls).First() <= n43)
                if (n4 <= 1 && n42 <= 1 && n43 <= 1f)
                {
                    counterS = counterS + 1;
                    ls++;
                    
                    //s += counterS + "\n";
                    //richTextBox1.Text += counterS + "\n";
                }
                ks++;
            }

            while (k <= listALL.Count)
            {
                    if (listALL.OrderBy(r => r).Skip(l).First() >= 160 )//&& listALL.OrderBy(r => r).Skip(l).First() >= n23 && listALL.OrderBy(r => r).Skip(l).First() >= n33 && listALL.OrderBy(r => r).Skip(l).First() >= n43)
                    {
                        counterU = counterU + 1;
                        //l++;
                        //break;
                    }
                    else
                    {
                        l++;
                    }
                    
                k++;
            }

            string all = a + counterA + "\n" + b + counterB + "\n" + p + counterP + "\n" + s + counterS + "\n" + u+counterU+"\n";
            richTextBox1.Text = all;
            //richTextBox1.Text = n4.ToString() + "\n" + n42.ToString() + "\n" + n43.ToString();
            //richTextBox1.Text = "1." + n1.ToString() + "\n " + "2." + n2.ToString() + "\n " + "3." + n3.ToString() + "\n " + "4." + n4.ToString() + "\n ";
            //richTextBox1.Text = n2.ToString();
            
        }


        #region METHOD Mencari 1stSmallest
        public double SmallestNApple()
        {
            
            ///
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUApple = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Apple";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                    double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                    listUApple.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();

            NApple = listUApple.Min();
            
            return NApple;
            
        }

        public double SmallestNBanana()
        {
            double NBanana;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUBanana = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Banana";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUBanana.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();

            NBanana = listUBanana.Min();
            return NBanana;
        }

        public double SmallestNPear()
        {
            double NPear;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUPear = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Pear";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUPear.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();

            NPear = listUPear.Min();
            return NPear;
        }

        public double SmallestNStrawberry()
        {
            double NStrawberry;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUStrawberry = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Strawberry";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUStrawberry.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();

            NStrawberry = listUStrawberry.Min();
            return NStrawberry;
        } 
        #endregion

        #region METHOD Mencari 2ndSmallest
        public double SmallestNApple2nd()
        {
            double NApple;
            //int i = 1;
            //int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUApple2nd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Apple";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUApple2nd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            int i = 0; 
            double secondMax = listUApple2nd.OrderBy(r => r).Skip(1).First();
            double temp;
            return secondMax;
            //while (secondMax == listUApple2nd.OrderBy(r => r).Skip(i).First())
            //{
            //    if (secondMax != listUApple2nd.OrderBy(r => r).Skip(i).First())
            //    {
            //        temp = listUApple2nd.OrderBy(r => r).Skip(i).First();
            //        return temp;
            //    }
                
            //    i++;
            //}

            
                //if (secondMax == SmallestNApple())
                //{
                //    temp = listUApple2nd.OrderBy(r => r).Skip(2).First();
                //    return temp;
                //}
                //else
                //{
                //    return secondMax;
                //}
            
        }

        public double SmallestNBanana2nd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUBanana2nd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Banana";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUBanana2nd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double secondMax = listUBanana2nd.OrderBy(r => r).Skip(1).First();
            double temp;
            //if (secondMax == SmallestNBanana())
            //{
            //    temp = listUBanana2nd.OrderBy(r => r).Skip(2).First();
            //    return temp;
            //}
            //else
            //{
            //    return secondMax;
            //}
            return secondMax;
        }

        public double SmallestNPear2nd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUPear2nd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Pear";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUPear2nd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double secondMax = listUPear2nd.OrderBy(r => r).Skip(1).First();
            double temp;
            //if (secondMax == SmallestNPear())
            //{
            //    temp = listUPear2nd.OrderBy(r => r).Skip(2).First();
            //    return temp;
            //}
            //else
            //{
            //    return secondMax;
            //}
            return secondMax;
        }

        public double SmallestNStrawberry2nd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUStrawberry2nd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Strawberry";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUStrawberry2nd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double secondMax = listUStrawberry2nd.OrderBy(r => r).Skip(1).First();
            double temp;
            //if (secondMax == SmallestNStrawberry())
            //{
            //    temp = listUStrawberry2nd.OrderBy(r => r).Skip(2).First();
            //    return temp;
            //}
            //else
            //{
            //    return secondMax;
            //}
            return secondMax;
        }
        #endregion

        #region Method Mencari 3rdSmallest
        public double SmallestNApple3rd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUApple3rd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Apple";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUApple3rd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double thirdMax = listUApple3rd.OrderBy(r => r).Skip(2).First();
            return thirdMax;
        }

        public double SmallestNBanana3rd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUBanana3rd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Banana";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUBanana3rd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double thirdMax = listUBanana3rd.OrderBy(r => r).Skip(2).First();
            return thirdMax;
        }

        public double SmallestNPear3rd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUPear3rd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Pear";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUPear3rd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double thirdMax = listUPear3rd.OrderBy(r => r).Skip(2).First();
            return thirdMax;
        }

        public double SmallestNStrawberry3rd()
        {
            double NApple;
            int i = 1;
            int j = 0;
            int xxx;
            int jml = HitungApple();
            List<double> listUStrawberry3rd = new List<double>();
            SqlConnection sqlcon = new SqlConnection(con);
            SqlConnection sqlcon2 = new SqlConnection(con);
            string sql = "select * from Strawberry";
            string sqlDTesting = "select * from DataTesting";
            sqlcon.Open();
            sqlcon2.Open();
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlCommand cmdDTesting = new SqlCommand(sqlDTesting, sqlcon2);
            SqlDataReader dr = cmd.ExecuteReader();
            SqlDataReader ds = cmdDTesting.ExecuteReader();
            while (dr.Read() && ds.Read())
            {
                //foreach (Blob b in this.ConnectedComponents)
                //{
                double h1 = Math.Abs(Convert.ToDouble(dr["m1"]) - Convert.ToDouble(ds["m1"]));//b.mc.InvariantMoment(1, b.Mark));
                double h2 = Math.Abs(Convert.ToDouble(dr["m2"]) - Convert.ToDouble(ds["m2"]));//b.mc.InvariantMoment(2, b.Mark));
                double h3 = Math.Abs(Convert.ToDouble(dr["m3"]) - Convert.ToDouble(ds["m3"]));//b.mc.InvariantMoment(3, b.Mark));
                double h4 = Math.Abs(Convert.ToDouble(dr["m4"]) - Convert.ToDouble(ds["m4"]));//b.mc.InvariantMoment(4, b.Mark));
                double h5 = Math.Abs(Convert.ToDouble(dr["m5"]) - Convert.ToDouble(ds["m5"]));//b.mc.InvariantMoment(5, b.Mark));
                double h6 = Math.Abs(Convert.ToDouble(dr["m6"]) - Convert.ToDouble(ds["m6"]));//b.mc.InvariantMoment(6, b.Mark));
                double h7 = Math.Abs(Convert.ToDouble(dr["m7"]) - Convert.ToDouble(ds["m7"]));//b.mc.InvariantMoment(7, b.Mark));
                double Nh = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) + Math.Pow(h3, 2) + Math.Pow(h4, 2) + Math.Pow(h5, 2) + Math.Pow(h6, 2) + Math.Pow(h7, 2));
                listUStrawberry3rd.Add(Nh);
                //}                    
                //i++;
            }
            sqlcon.Close();
            sqlcon2.Close();
            double thirdMax = listUStrawberry3rd.OrderBy(r => r).Skip(2).First();
            return thirdMax;
        }
        #endregion

        #region | Menghitung Jumlah Baris pada database masing |
        private int HitungApple()
        {
            int jmlApple;
            string query = "select count(*) from Apple";
            SqlConnection sqlcon = new SqlConnection(con);
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand(query, sqlcon);
            jmlApple = (int)cmd.ExecuteScalar();
            sqlcon.Close();

            return jmlApple;
        }

        //private int HitungBanana()
        //{

        //}

        //private int HitungPear()
        //{

        //}

        //private int HitungStrawberry()
        //{

        //}

        #endregion
    }
}
