using DXFReaderNET.Entities;
using DXFReaderNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXFThumbNail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "DXF";
            openFileDialog1.Filter = "DXF|*.dxf";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dxfReaderNETControl1.ReadDXF(openFileDialog1.FileName);
                dxfReaderNETControl1.HighlightNotContinuous = true;
                dxfReaderNETControl1.ZoomCenter();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DXFReaderNETControl _dxfControl;

            _dxfControl = new DXFReaderNETControl();

            _dxfControl.ReadDXF(dxfReaderNETControl1.FileName);

            _dxfControl.Width = 1000;
            _dxfControl.Height = 1000;

            _dxfControl.AntiAlias = false;
            _dxfControl.ShowAxes = false;
            _dxfControl.ShowGrid = false;
            _dxfControl.BackColor = System.Drawing.Color.White;
            _dxfControl.ForeColor = System.Drawing.Color.Black;


            ShowFilledAreas(_dxfControl);

            _dxfControl.ZoomCenter();
            pictureBox1.Image = _dxfControl.Image;
          
            _dxfControl.Dispose();

            int InternalCountoursNumber = 0;
            double ExternalLenght = 0;
            double ExternalArea = 0;
            double InternalLenght = 0;
            double InternalArea = 0;
            bool ret = MathHelper.FindClosedAreaData(dxfReaderNETControl1.DXF.Entities, out ExternalLenght, out ExternalArea, out InternalLenght, out InternalArea, out InternalCountoursNumber);

            if (ret)
            {

                label1.Text = "External lenght: " + dxfReaderNETControl1.DXF.ToFormattedUnit(ExternalLenght) + "\r\n";
                label1.Text += "External area: " + dxfReaderNETControl1.DXF.ToFormattedUnit(ExternalArea) + "\r\n";
                label1.Text += "Internal lenght: " + dxfReaderNETControl1.DXF.ToFormattedUnit(InternalLenght) + "\r\n";
                label1.Text += "Internal area: " + dxfReaderNETControl1.DXF.ToFormattedUnit(InternalArea) + "\r\n";
                label1.Text += "Filled area: " + dxfReaderNETControl1.DXF.ToFormattedUnit(ExternalArea - InternalArea) + "\r\n";
                label1.Text += "Internal countours #: " + InternalCountoursNumber.ToString() + "\r\n";

                Vector2 extension = new Vector2(dxfReaderNETControl1.DXF.DrawingVariables.ExtMax.X - dxfReaderNETControl1.DXF.DrawingVariables.ExtMin.X, dxfReaderNETControl1.DXF.DrawingVariables.ExtMax.Y - dxfReaderNETControl1.DXF.DrawingVariables.ExtMin.Y);
                label1.Text += "Extension : " + dxfReaderNETControl1.DXF.ToFormattedUnit(extension.X) + " x " + dxfReaderNETControl1.DXF.ToFormattedUnit(extension.Y);

            }
        }

        private void ShowFilledAreas(DXFReaderNETControl myDXF)
        {


            List<EntityObject> externalContour = MathHelper.ExternalContour(myDXF.DXF.Entities);

            List<EntityObject> allEntities = new List<EntityObject>();
            foreach (EntityObject entity in myDXF.DXF.Entities)
            {
                if (!externalContour.Contains(entity))
                    allEntities.Add(entity);
            }

            List<LwPolyline> lws = dxfReaderNETControl1.EntitiesToLwPolylines(allEntities);

            List<EntityObject> BoundaryOutermost = new List<EntityObject>();
            foreach (LwPolyline l in lws)
            {

                BoundaryOutermost.Add(l);
            }


            EntityObject myEnt = myDXF.AddGradientHatch(HatchGradientPatternType.Cylinder, externalContour, BoundaryOutermost, 140, 180, 30, true, "SERVICE");

         

        }

    }
}
