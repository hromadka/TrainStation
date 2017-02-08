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

namespace TrainStation
{
    public partial class Form2 : Form
    {
        bool m_bRotateToo = false;  // set this to false because DetectNet doesn't care about rotation after all


        List<Airplane> airplanes = new List<Airplane>();

        String m_inputFname = "";
        String m_path = "";
        int m_bmpEdge = 256;
        String m_log = "";

        Point startPos;      // mouse-down position
        Point currentPos;    // current mouse position
        Point finalPos;
        bool drawingBox;        // busy drawing bounding box
        bool drawingAxis;    // busy drawing axis now
        bool toggleRectangleToRotation;  // time to switch

        Rectangle m_boundingbox = new Rectangle();
        Rectangle DEFAULT_RECTANGLE = new Rectangle(-1, -1, -1, -1);

        List<Rectangle> boxes = new List<Rectangle>(); // will need this if implement Undo stack someday
        List<Rectangle> axes = new List<Rectangle>();


        public Form2()
        {
            InitializeComponent();
            m_boundingbox = DEFAULT_RECTANGLE;
            drawingBox = false;
            drawingAxis = false;
            toggleRectangleToRotation = false;

            loadAirplanes();
            
            comboBox1.DataSource = airplanes;
            comboBox1.DisplayMember = "label";

        }

        private void loadAirplanes()
        {
            airplanes.Add(new Airplane("F16", 4.88f, 9.96f, 15.06f));
            airplanes.Add(new Airplane("F18", 4.7f, 12.3f, 17.1f));
            airplanes.Add(new Airplane("P3", 11.8f, 30.4f, 35.6f));
            airplanes.Add(new Airplane("P8", 12.83f, 37.64f, 39.47f));
            airplanes.Add(new Airplane("V22", 6.73f, 14.0f, 17.5f));
            airplanes.Add(new Airplane("C17", 16.79f, 51.75f, 53f));
            airplanes.Add(new Airplane("C130", 11.6f, 40.4f, 29.8f));
            airplanes.Add(new Airplane("KC135", 12.7f, 39.88f, 41.53f));
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.ShowDialog();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "TrainStation v" + Application.ProductVersion.ToString();
            WindowState = FormWindowState.Maximized;

            btnOpen.Left = 10;
            btnOpen.Top = 30;

            panel1.AutoScroll = true;
            panel1.Width = Convert.ToInt32(this.Width - 200);
            panel1.Height = Convert.ToInt32(this.Height - 100);

            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            label1.Text = "loaded: none";
            lblLog.Text = m_log;  // binding available?
        }

        private Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.tmp;*.jpeg;*.jpg;*.jpeg-large";
            DialogResult res = ofd.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_inputFname = ofd.FileName;
                m_path = System.IO.Path.GetDirectoryName(ofd.FileName);
                label1.Text = "loaded: " + m_inputFname;
                LoadImage();
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            // write training text file 
            String fout = Path.ChangeExtension(m_inputFname, ".txt");
            string[] lines = new string[boxes.Count()];
            int i = 0;

            foreach (Rectangle r in boxes)
            {
            //    lines[i++] = comboBox1.Text + " 0.0 0 0.0 " + r.Left + " " + r.Top + " " + r.Right + " " + r.Bottom + " 0.0 0.0 0.0 0.0 0.0 0.0 0.0";
                // hack for DetectNet bug
                lines[i++] = "Car" + " 0.0 0 0.0 " + r.Left + " " + r.Top + " " + r.Right + " " + r.Bottom + " 0.0 0.0 0.0 0.0 0.0 0.0 0.0";
            }

            System.IO.File.WriteAllLines(fout, lines);

            MessageBox.Show("done " + i + " objects.");

            CleanUp();
        }

        private void CleanUp()
        {
            m_log = "";
            boxes.Clear();
            pictureBox1.Invalidate();
        }

        //private Bitmap GetRegionFromImage(Bitmap srcBitmap, Rectangle srcRegion)
        //{
        //    Bitmap subbmp = new Bitmap(m_bmpEdge, m_bmpEdge);
        //    Rectangle destRegion = new Rectangle(0, 0, m_bmpEdge, m_bmpEdge);
        //    using (Graphics grD = Graphics.FromImage(subbmp))
        //    {
        //        grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
        //    }
        //    return subbmp;
        //}

        private void LoadImage()
        {
            pictureBox1.Image = Image.FromFile(m_inputFname);
            boxes.Clear();
            lblLog.Text = "";
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            currentPos = startPos = e.Location;

            if (m_bRotateToo)
            {
                if (toggleRectangleToRotation)
                {
                    drawingAxis = true;
                }
                else
                {
                    drawingBox = true;
                }
            }
            else
            {
                drawingBox = true;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            currentPos = e.Location;
            if (drawingBox || toggleRectangleToRotation) pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_bRotateToo)
            {

                if (drawingBox)
                {
                    drawingBox = false;
                    toggleRectangleToRotation = true;
                    var rc = getRectangle();
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        m_boundingbox = rc;
                    }
                    else
                    {
                        m_boundingbox = DEFAULT_RECTANGLE;
                    }
                    boxes.Add(rc);
                    pictureBox1.Invalidate();
                }
                else if (drawingAxis)
                {
                    finalPos = e.Location;
                    toggleRectangleToRotation = false;
                    drawingAxis = false;
                    pictureBox1.Invalidate();
                    m_log += Environment.NewLine + comboBox1.Text;
                    lblLog.Text = m_log;
                }
            }
            else
            {
                if (drawingBox)
                {
                    finalPos = e.Location;
                    drawingBox = false;
                    var rc = getRectangle();
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        m_boundingbox = rc;
                    }
                    else
                    {
                        m_boundingbox = DEFAULT_RECTANGLE;
                    }
                    boxes.Add(rc);
                    pictureBox1.Invalidate();
                    m_log += Environment.NewLine + comboBox1.Text;
                    lblLog.Text = m_log;
                }
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs pe)
        {
            if (drawingAxis)
            {
                pe.Graphics.DrawRectangle(Pens.Red, m_boundingbox);
                pe.Graphics.DrawLine(Pens.Lime, startPos.X, startPos.Y, currentPos.X, currentPos.Y );
            }
            else if (drawingBox)
            {
                pe.Graphics.DrawRectangle(Pens.Red, getRectangle());
            }
            else
            {
                foreach (Rectangle box in boxes)
                {
                    pe.Graphics.DrawRectangle(Pens.Blue, box);
                }

                pe.Graphics.DrawRectangle(Pens.Red, m_boundingbox);
                if (finalPos.X + finalPos.Y > 0)
                {
                    pe.Graphics.DrawLine(Pens.Lime, startPos.X, startPos.Y, finalPos.X, finalPos.Y);
                }
            }
            finalPos.X = 0; finalPos.Y = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

    }
}
