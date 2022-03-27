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
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Windows;
using System.Diagnostics;

namespace GIFtoBIN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap [] bmplist;

        List<byte> output = new List<byte>();

        int H, W;

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = "gif files (*.gif)|*.gif";

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                //Name = ofd.FileName;
               Stream imageStreamSource = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
               GifBitmapDecoder decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

               BitmapSource bitmapSource = decoder.Frames[10];

               H = (int)bitmapSource.Height;
               W = (int)bitmapSource.Width;

               listBox1.Items.Add("H: "+bitmapSource.Height.ToString());
               listBox1.Items.Add("W: "+bitmapSource.Width.ToString());
               pBox.Width  = (int)bitmapSource.Width;
               pBox.Height = (int)bitmapSource.Height;
               listBox1.Items.Add("Кадров: "+decoder.Frames.Count.ToString()); 


               Bitmap bmp = new Bitmap(BitmapFromSource(bitmapSource));

               bmplist = new Bitmap[decoder.Frames.Count];

               for(int i=0; i<decoder.Frames.Count;i++)
               {
                 lbFrames.Items.Add(i.ToString());
                 bmplist[i] = BitmapFromSource(decoder.Frames[i]);
               }

               pBox.Image = bmplist[0];

            }
        }

        private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        private void lbFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            pBox.Image = bmplist[lbFrames.SelectedIndex];
        }

        private void buttonSaveBIN_Click(object sender, EventArgs e)
        {
            if (bmplist == null) return;

            output.Clear();

            foreach (Bitmap bmp in bmplist)
            {
                output.AddRange(ImageToBytesBit(bmp , Convert.ToInt32(comboBox1.SelectedItem)));
            }

            byte[] fileout = output.ToArray();
            listBox1.Items.Add(fileout.Length.ToString());

            File.WriteAllBytes("res.bin", fileout);

            List<string> str = new List<string>();
            str.Add(H.ToString());
            str.Add(W.ToString());
            str.Add(comboBox1.SelectedIndex.ToString());
            str.Add(bmplist.Length.ToString());

            using (TextWriter tw = new StreamWriter("i.txt"))
            {
                foreach (String s in str)
                    tw.WriteLine(s);
            }

        }

        byte[] ImageToBytesBit(Image value, int bit)
        {
            List<byte> arr = new List<byte>();
            Bitmap bmp = new Bitmap(value);
            byte A = 0;
            byte R = 0;
            byte G = 0;
            byte B = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color Pixel = bmp.GetPixel(x, y);
                    A = Pixel.A; R = Pixel.R; G = Pixel.G; B = Pixel.B;

                    if (bit == 32)
                    {
                        arr.Add(A); arr.Add(R); arr.Add(G); arr.Add(B);
                    }

                    if (bit == 16)
                    {
                        arr.AddRange(RGB565(R, G, B));
                    }
                }
            }

            return arr.ToArray();
        }

        byte[] RGB565(byte R, byte G, byte B)
        {
            UInt16 r;
            r = (UInt16)(((R >> 3) << 11) | ((G >> 2) << 5) | (B >> 3));
            byte[] ret = { 0, 0 };
            ret[1] = (byte)(r >> 8);
            ret[0] = (byte)(r & 0xFF);
            return ret;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }


}
