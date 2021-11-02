using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickHull
{
    public partial class quickHullForm : Form
    {
        public quickHullForm()
        {
            InitializeComponent();
        }

        Graphics g;

        private List<Point> Points = new List<Point>();

        private List<Point> Hull = new List<Point>();

        private void quickHullForm_Load(object sender, EventArgs e)
        {
            g = CreateGraphics();
            quickhullButton.Enabled = false;
        }

        private void quickHullForm_MouseClick(object sender, MouseEventArgs e)
        {
            Brush brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, e.X, e.Y, 5, 5);
            Points.Add(new Point(e.X, e.Y));
            if (Points.Count >= 3)
            {
                quickhullButton.Enabled = true;
            }
        }

        private void quickhullButton_Click(object sender, EventArgs e)
        {
            Hull.Clear();
            quickHull();
            g.DrawPolygon(new Pen(Color.Blue), Hull.ToArray());
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            quickhullButton.Enabled = false;
            Hull.Clear();
            Points.Clear();
        }

        /*
         1 -> left
        -1 -> right
         0 -> ? 
        */
        private int sideDetermination(Point p0, Point p1, Point p)
        {
            int side = (p.Y - p0.Y) * (p1.X - p0.X) - (p1.Y - p0.Y) * (p.X - p0.X);

            if (side > 0)
                return 1;

            if (side < 0)
                return -1;

            return 0;
        }

        private int distance(Point p1, Point p2, Point p)
        {
            return Math.Abs((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X));
        }


        private void quickHull()
        {
            if (Points.Count < 4)
            {
                foreach (var p in Points)
                {
                    Hull.Add(p);
                }
                return;
            }

            var minPoint = Points.Aggregate((p1, p2) => p1.X < p2.X ? p1 : p2);
            Hull.Add(minPoint);

            var maxPoint = Points.Aggregate((p1, p2) => p1.X > p2.X ? p1 : p2);
            Hull.Add(maxPoint);

            var leftPoints = new List<Point>();

            var rightPoints = new List<Point>();

            foreach (var p in Points)
            {
                if (sideDetermination(minPoint, maxPoint, p) == -1)
                    rightPoints.Add(p);
                else if (sideDetermination(minPoint, maxPoint, p) == 1)
                    leftPoints.Add(p);
            }

            findHull(minPoint, maxPoint, leftPoints);

            findHull(maxPoint, minPoint, rightPoints);
        }

        private void findHull(Point a, Point b, List<Point> Points)
        {
            if (Points.Count == 0)
                return;

            int pos = Hull.IndexOf(b);

            if (Points.Count == 1)
            {
                Point pp = Points[0];
                Hull.Insert(pos, pp);
                return;
            }

            int maxDist = int.MinValue;
            int maxPointIndex = 0;

            foreach (var p in Points)
            {
                int dist = distance(a, b, p);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    maxPointIndex = Points.IndexOf(p);
                }
            }

            var pFurthest = Points[maxPointIndex];
            Hull.Insert(pos, pFurthest);

            var apFurthest = new List<Point>();

            foreach (var p in Points)
            {
                if (sideDetermination(a, pFurthest, p) == 1)
                {
                    apFurthest.Add(p);
                }
            }

            var pFurthestB = new List<Point>();

            foreach (var p in Points)
            {
                if (sideDetermination(pFurthest, b, p) == 1)
                {
                    pFurthestB.Add(p);
                }
            }

            findHull(a, pFurthest, apFurthest);
            findHull(pFurthest, b, pFurthestB);
        }
    }
    }
   
