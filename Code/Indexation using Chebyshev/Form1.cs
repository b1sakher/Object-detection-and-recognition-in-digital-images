using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;


namespace My_EMGU_Program
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Image<Gray, Byte> grayImage;
        Image<Bgr, Byte> My_Image;
        Image<Gray, Byte> Gauss;
        Image<Gray, Byte> cann;
        //****************
        //**************
        List<double> myList = new List<double>();
        String text = "";



        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                //Load the Image
                My_Image = new Image<Bgr, byte>(Openfile.FileName);

               this.Text =""+ My_Image.Width;

                //convert to  gray
                grayImage = My_Image.Convert<Gray, byte>();

                //apply Gaussien Filter
                Gauss = grayImage.SmoothGaussian(3);

                //detect edges with Canny
                cann = grayImage.Canny(new Gray(128), new Gray(255));



                //Display the Image
                //pictureBox1.Image = hsvimg.ToBitmap();
                pictureBox1.Image = cann.ToBitmap();
                
            }
        }


        static double _t(int p, int x, int N)
        {
            if (p == 0)
                return 1;
            else if (p == 1)
            {
                return (2 * x + 1 - N) * 1.0 / N * 1.0;
            }
            else
            {
                return ((2 * p - 1) * _t(1, x, N) * _t(p - 1, x, N) - (p - 1) * (1 - (Math.Pow(p - 1, 2) / Math.Pow(N, 2))) * _t(p - 2, x, N)) * 1.0 / p * 1.0;
            }
        }

        static double _p(int q, int N)
        {
            double temp = N;
            for (int i = 1; i <= q; i++)
            {
                temp *= 1 - (Math.Pow(i, 2) / Math.Pow(N, 2));
            }
            return temp * 1.0 / (2 * q + 1) * 1.0;
        }

        public double _T(int p, int q, int N, Image<Gray, byte> img)
        {
            double somme = 0.0;
            double scal = 0;
            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    if (img.Data[x, y, 0]==0) continue;
                    else {

                        scal = _t(p, x, N) * _t(q, y, N) * img.Data[x, y, 0];
                        somme += _t(p, x, N) * _t(q, y, N) * img.Data[x, y, 0];
                    }
                    
                }
            }
            return somme / (_p(q, N) * _p(p, N)) * 1.0;
        }
        Stopwatch segmentation_time = new Stopwatch();
        private void button2_Click(object sender, EventArgs e)
        {
            segmentation_time.Reset();
            segmentation_time.Start(); // début de la mesure
           

            //delete the content, in case of multiple use of the software
            myList.Clear();
            textBox1.Clear();
            text = "";
            int ordre = Convert.ToInt32(textBox2.Text);
            for (int i = 0, j = ordre; i <= ordre; i++, j--)
            {
                myList.Add(Math.Abs(_T(i, j, cann.Height, cann)));
                
            }
            segmentation_time.Stop();// Fin de la mesure
            this.Text="fait en : " + segmentation_time.ElapsedMilliseconds + " ms\n";

            for (int k = 0; k < myList.Count; k++)
            {
                //textBox1.AppendText("sakher");
                textBox1.AppendText(myList.ElementAt(k) + "\n");
                text += myList.ElementAt(k) + " ";
            }

            text+= calcule_couleur(My_Image);
			text=text.Replace(',', '.');
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (text != "")
            {
                text.Replace(',', '.');

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Sakher\Desktop\input.txt", true))
                {
                    file.WriteLine(text);
                }

                MessageBox.Show("donnée enregistrée !", "Information");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= 20; i++)
            {
                for (int j = 0; j <= 295; j = j + 5)
                {
                    text = "" + i;
                    My_Image = new Image<Bgr, byte>("C:/Users/Sakher/Desktop/coil-100/" + text + "__" + j + ".png");
                    //convert to  gray
                    grayImage = My_Image.Convert<Gray, byte>();

                    //apply Gaussien Filter
                    Gauss = grayImage.SmoothGaussian(3);

                    //detect edges with Canny
                    cann = Gauss.Canny(new Gray(0), new Gray(255));

                    int ordre = Convert.ToInt32(textBox2.Text);
                    for (int k = 0, l = ordre; k <= ordre; k++, l--)
                    {
                        text += " " + Math.Abs(_T(k, l, cann.Height, cann));
                    }

                    text+=calcule_couleur(My_Image);
                    //text += " 128";
                    text=text.Replace(',', '.');
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Sakher\Desktop\temp_TChebyshev_ordre" + ordre + ".txt", true))
                    {
                        file.WriteLine(text);
                    }

                }
            }

            MessageBox.Show("Terminé !", "Information");

        }

        private String calcule_couleur(Image<Bgr, byte> img)
        {
            double red=0;
            double green = 0;
            double blue = 0;
            int total_px = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    blue += img.Data[i, j, 0];
                    green += img.Data[i, j, 1];
                    red+= img.Data[i, j, 2];
                    total_px++;
                }
            }
            textBox1.AppendText("\n blue=" + (blue / total_px) + " green=" + (green / total_px) +" red=" + (red / total_px) +  " px=" + total_px);
            return (" " +(blue / total_px) + " " + (green / total_px) + " " + (red / total_px));
        }


    }
}