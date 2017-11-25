using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIS_Downloader
{
    public partial class Form1 : Form
    {

        Dictionary<string, string> cbbItem = new Dictionary<string, string>();
        List<ItemModel> jsItems = new List<ItemModel>();
        public Form1()
        {
            InitializeComponent();
            cbbItem.Add("", "Mọi kích thước");
            cbbItem.Add("&tbs=isz:i", "Biểu tượng");
            cbbItem.Add("&tbs=isz:l", "Lớn");
            cbbItem.Add("&tbs=isz:lt,islt:qsvga", "Lớn hơn 400×300");
            cbbItem.Add("&tbs=isz:lt,islt:vga", "Lớn hơn 640×480");
            cbbItem.Add("&tbs=isz:lt,islt:svga", "Lớn hơn 800×600");
            cbbItem.Add("&tbs=isz:lt,islt:xga", "Lớn hơn 1024×768");
            cbbItem.Add("&tbs=isz:lt,islt:2mp", "Lớn hơn 1600×1200");
            cbbItem.Add("&tbs=isz:lt,islt:4mp", "Lớn hơn 2272×1704");
            cbbItem.Add("&tbs=isz:lt,islt:6mp", "Lớn hơn 2816×2112");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbbSize.DataSource = new BindingSource(cbbItem, null);
            cbbSize.DisplayMember = "Value";
            cbbSize.ValueMember = "Key";
            //List<Image> list = new List<Image>();
        }

        private void cbbSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtKey.Text.Length < 3)
                return;
            SetImage();
        }

        public async void SetImage()
        {
            string value = cbbSize.SelectedValue.ToString();
            progressBar1.Value = 0;
            lvMain.Items.Clear();
            ImageList iml = new ImageList();
            iml.ImageSize = new Size(100, 100);
            iml.ColorDepth = ColorDepth.Depth16Bit;
            lvMain.LargeImageList = iml;
            timer1.Start();
            Crawler c = await Crawler.Load(txtKey.Text, value);
            List<string> img = c.GetImages();
            foreach(var it in img)
            {
                ItemModel jsItem = JsonConvert.DeserializeObject<ItemModel>(it);
                jsItems.Add(jsItem);
                Image _img = await LoadImage.Get(jsItem.tu);
                iml.Images.Add(jsItem.id, _img);
                var listViewItem = lvMain.Items.Add("");
                listViewItem.ImageKey = jsItem.id;
            }
            progressBar1.Value = 1000;
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(progressBar1.Value < 800)
            progressBar1.Value += 5;
        }

        private void lvMain_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool val = lvMain.Items[e.Index].Checked;
            if (val)
                cbAll.Checked = false;
        }

        private void cbAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.lvMain.Items)
            {
                item.Checked = cbAll.Checked;
            }
            btnDownload.Enabled = cbAll.Checked;
        }

        private void lvMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string url = "";
            foreach(var i in jsItems)
            {
                if(i.id == lvMain.SelectedItems[0].ImageKey)
                {
                    url = i.ou;
                    break;
                }
            }
            Preview preview = new Preview();
            preview.url = url;
            preview.ShowDialog(this);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            int d = 0;
            foreach (ListViewItem item in this.lvMain.Items)
            {
                if (item.Checked)
                {
                    foreach(var js in jsItems)
                    {
                        if(js.id == item.ImageKey)
                        {
                            Process.Start("C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe", "/a /d " + js.ou);
                            d++;
                        }
                    }
                }
            }
            if (d > 0)
            {
                Process.Start("C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe", "/s");
            }
        }
    }
}
