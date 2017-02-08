using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainStation
{
    public partial class Form1 : Form
    {

        String m_inputFname = "";
        String m_path = "";
        int m_bmpEdge = 256;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "TrainStation v" + Application.ProductVersion.ToString();
            WindowState = FormWindowState.Maximized;

            btnOpen.Left = 10;
            btnOpen.Top = 30;

            panel1.AutoScroll = true;
            panel1.Width = Convert.ToInt32(this.Width - 200);
            panel1.Height = Convert.ToInt32(this.Height - 100);

            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            radioButton2.PerformClick();

            label1.Text = "loaded: none";

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"Image Files|*.png;*.tmp;*.jpeg;*.jpg;*.jpeg-large";
            DialogResult res = ofd.ShowDialog();
            if (res == DialogResult.OK) {
                m_inputFname = ofd.FileName;
                m_path = System.IO.Path.GetDirectoryName(ofd.FileName);
                label1.Text = "loaded: " + m_inputFname;
                LoadImage();
            }

        }

        private void LoadImage()
        {
            pictureBox1.Image = Image.FromFile(m_inputFname);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Rectangle srcRegion = new Rectangle((me.X - m_bmpEdge / 2), (me.Y - m_bmpEdge / 2), m_bmpEdge, m_bmpEdge);
            pictureBox2.Image = GetRegionFromImage((Bitmap)pictureBox1.Image, srcRegion);
        }

        private Bitmap GetRegionFromImage(Bitmap srcBitmap, Rectangle srcRegion)
        {
            Bitmap subbmp = new Bitmap(m_bmpEdge,m_bmpEdge);
            Rectangle destRegion = new Rectangle(0, 0, m_bmpEdge, m_bmpEdge);
            using (Graphics grD = Graphics.FromImage(subbmp))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
            return subbmp;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            m_bmpEdge = 128;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            m_bmpEdge = 256;
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            int i = 0;
            String fname = System.IO.Path.Combine(m_path, textBox1.Text + i.ToString() + ".png");
            while (System.IO.File.Exists(fname))
            {
                i++;
                fname = System.IO.Path.Combine(m_path, textBox1.Text + i.ToString() + ".png");

                //for safety
                if (i > 1000)
                {
                    MessageBox.Show("counted over 1000 files of this type... cancelling");
                    break;
                }
            }

            pictureBox2.Image.Save(fname, System.Drawing.Imaging.ImageFormat.Png);
            MessageBox.Show("done.");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            form2.ShowDialog();
            this.Close();
        }

    }
}
