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

namespace GIFtoBIN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap [] bmplist;

        private void button1_Click(object sender, EventArgs e)
        {

            Stream imageStreamSource = new FileStream("Q1.gif", FileMode.Open, FileAccess.Read, FileShare.Read);
            GifBitmapDecoder decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            BitmapSource bitmapSource = decoder.Frames[10];
            

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


        private void pBox_Click(object sender, EventArgs e)
        {

        }

        private void lbFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            

            pBox.Image = bmplist[lbFrames.SelectedIndex];



        }
    }


}
