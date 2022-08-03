using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalismaTakip
{
    public partial class Form1 : Form
    {
        int sayi;
        public Form1()
        {
            InitializeComponent();
            sayi = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sayi++;
            Bitmap bmp = new Bitmap("C:\\Users\\CASPER\\Desktop\\Merve Ders Takip\\" + sayi + ".png");
            pictureBox1.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap("C:\\Users\\CASPER\\Desktop\\Merve Ders Takip\\0.png");
            pictureBox1.Image = bmp;
        }
    }
}
