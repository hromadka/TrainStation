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
    public partial class Form3 : Form
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


        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
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



        private Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
        }




        private void CleanUp()
        {
            m_log = "";
            boxes.Clear();
            pictureBox1.Invalidate();
        }



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
                pe.Graphics.DrawLine(Pens.Lime, startPos.X, startPos.Y, currentPos.X, currentPos.Y);
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

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.tmp;*.jpeg;*.jpg;*.jpeg-large";
            DialogResult res = ofd.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_inputFname = ofd.FileName;
                m_path = System.IO.Path.GetDirectoryName(ofd.FileName);
                label1.Text = @"loaded: " + m_inputFname;
                LoadImage();
            }
        }

        private void btnSaveImage_Click_1(object sender, EventArgs e)
        {
            // write training text file 
            String fout = Path.ChangeExtension(m_inputFname, ".json");
            string[] lines = new string[boxes.Count()*6 + 5];
            int i = 0;

            lines[i++] = "{";
            lines[i++] = @"""image_path"": """ + m_inputFname + @""",";
            lines[i++] = @"""rects"": [";

            foreach (Rectangle r in boxes)
            {
                lines[i++] = "{";
                lines[i++] = @"""x1"": " + r.Left + ",";
                lines[i++] = @"""x2"": " + r.Right + ",";
                lines[i++] = @"""y1"": " + r.Top + ",";
                lines[i++] = @"""y2"": " + r.Bottom;
                lines[i++] = "},";
            }
            lines[i++] = "]";
            lines[i++] = "},";


            System.IO.File.WriteAllLines(fout, lines);

            MessageBox.Show(@"done " + boxes.Count() + " objects.");

            CleanUp();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


    }
}
