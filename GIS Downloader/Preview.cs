using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIS_Downloader
{
    public partial class Preview : Form
    {
        public string url;
        public Preview()
        {
            InitializeComponent();
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            SetImage();
        }

        private async void SetImage()
        {
            Image _img = await LoadImage.Get(url);
            pictureBox1.Image = _img;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(pictureBox1.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }
    }
}
