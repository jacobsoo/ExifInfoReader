using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;


namespace ExifInfoReader
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private ExifTagCollection _exif;

        private void AddTagToList(ExifTag tag)
        {
            ListViewItem item = listExif.Items.Add(tag.Id.ToString());
            item.SubItems.Add(tag.FieldName);
            item.SubItems.Add(tag.Description);
            item.SubItems.Add(tag.Value);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPEG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tssLabel.Text = ofd.FileName;
                bool bRes = false;
                picPreview.Image = Image.FromFile(ofd.FileName);
                listExif.Items.Clear();
                _exif = new ExifTagCollection(ofd.FileName);

                foreach (ExifTag tag in _exif)
                {
                    if (tag.Id == 1)
                        bRes = true;
                    AddTagToList(tag);
                }
                if (bRes == true)
                {
                    ExifTag NSTag = _exif[1];
                    ExifTag LatTag = _exif[2];
                    ExifTag EWTag = _exif[3];
                    ExifTag LongTag = _exif[4];
                    string lat = LatTag.Value.ToString();
                    string[] lat_split = lat.Split(' ');
                    if (lat != "")
                    {
                        StringBuilder queryAddress = new StringBuilder();
                        queryAddress.Append("http://maps.googleapis.com/maps/api/staticmap?center=");
                        string latNum1 = Regex.Replace(lat_split[0], "[^.0-9]", "");
                        string latNum2 = Regex.Replace(lat_split[1], "[^.0-9]", "");
                        string latNum3 = Regex.Replace(lat_split[2], "[^.0-9]", "");
                        double latVal1 = double.Parse(latNum1);
                        double latVal2 = double.Parse(latNum2);
                        double latVal3 = double.Parse(latNum3);
                        double lat_value = latVal1 + (latVal2 / 60) + (latVal3 / 3600);
                        if (NSTag.Value.ToString().Substring(0, 5) == "South")
                        {
                            queryAddress.Append("-");
                        }
                        queryAddress.Append(lat_value);

                        string lon = LongTag.Value.ToString();
                        string[] lon_split = lon.Split(' ');
                        string lonNum1 = Regex.Replace(lon_split[0], "[^.0-9]", "");
                        string lonNum2 = Regex.Replace(lon_split[1], "[^.0-9]", "");
                        string lonNum3 = Regex.Replace(lon_split[2], "[^.0-9]", "");
                        double lonVal1 = double.Parse(lonNum1);
                        double lonVal2 = double.Parse(lonNum2);
                        double lonVal3 = double.Parse(lonNum3);
                        double lon_value = lonVal1 + (lonVal2 / 60) + (lonVal3 / 3600);
                        queryAddress.Append(",");

                        if (EWTag.Value.ToString().Substring(0, 4) == "west")
                        {
                            queryAddress.Append("-");
                        }
                        queryAddress.Append(lon_value);
                        queryAddress.Append("&zoom=10&size=400x400&maptype=terrain&sensor=true&key=ABQIAAAAaHAby4XeLCIadFkAUW4vmRSkJGe9mG57rOapogjk9M-sm4TzXxR2I7bi2Qkj-opZe16CdmDs7_dNrQ");

                        webBrowser1.Navigate(queryAddress.ToString());
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
