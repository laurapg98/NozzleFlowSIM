
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Data;
using System.Windows.Input;
using System;
using System.Windows;

namespace ClassLibrary
{
    public class ParametricCurve3D
    {

        public delegate Point3D Function(double u, double v);
        private int nu = 30;
        private int nv = 30;
        private double umin = -3;
        private double umax = 3;
        private double vmin = -8;
        private double vmax = 8;
        private double xmin = -1;
        private double xmax = 1;
        private double ymin = -1;
        private double ymax = 1;
        private double zmin = -1;
        private double zmax = 1;
        private Color lineColor = Colors.Black;
        private Color surfaceColor;
        private Point3D center = new Point3D();
        private bool isHiddenLine = false;
        private bool isWireframe = true;
        private Viewport3D viewport3d = new Viewport3D();
        public bool IsWireframe
        {
            get { return isWireframe; }
            set { isWireframe = value; }
        }
        public bool IsHiddenLine
        {
            get { return isHiddenLine; }
            set { isHiddenLine = value; }
        }
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }
        public Color SurfaceColor
        {
            get { return surfaceColor; }
            set { surfaceColor = value; }
        }
        public double Umin
        {
            get { return umin; }
            set { umin = value; }
        }
        public double Umax
        {
            get { return umax; }
            set { umax = value; }
        }
        public double Vmin
        {
            get { return vmin; }
            set { vmin = value; }
        }
        public double Vmax
        {
            get { return vmax; }
            set { vmax = value; }
        }
        public int Nu
        {
            get { return nu; }
            set { nu = value; }
        }
        public int Nv
        {
            get { return nv; }
            set { nv = value; }
        }
        public double Xmin
        {
            get { return xmin; }
            set { xmin = value; }
        }
        public double Xmax
        {
            get { return xmax; }
            set { xmax = value; }
        }
        public double Ymin
        {
            get { return ymin; }
            set { ymin = value; }
        }
        public double Ymax
        {
            get { return ymax; }
            set { ymax = value; }
        }
        public double Zmin
        {
            get { return zmin; }
            set { zmin = value; }
        }
        public double Zmax
        {
            get { return zmax; }
            set { zmax = value; }
        }
        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }
        public Viewport3D Viewport3d
        {
            get { return viewport3d; }
            set { viewport3d = value; }
        }
        public void CreateSurface(Function f)
        {
            double du = (Umax - Umin) / (Nu - 1);
            double dv = (Vmax - Vmin) / (Nv - 1);

            Point3D[,] pts = new Point3D[Nu, Nv];
            for (int i = 0; i < Nu; i++)
            {
                double u = Umin + i * du;
                for (int j = 0; j < Nv; j++)
                {
                    double v = Vmin + j * dv;
                    pts[i, j] = f(u, v);
                    pts[i, j] += (Vector3D)Center;
                    pts[i, j] = Utility.GetNormalize(pts[i, j], Xmin, Xmax,
                    Ymin, Ymax, Zmin, Zmax);
                }
            }
            Point3D[] p = new Point3D[4];
            for (int i = 0; i < Nu - 1; i++)
            {
                for (int j = 0; j < Nv - 1; j++)
                {
                    p[0] = pts[i, j];
                    p[1] = pts[i, j + 1];
                    p[2] = pts[i + 1, j + 1];
                    p[3] = pts[i + 1, j];
                    //Create rectangular face:

                    Utility.CreateRectangleFace(p[0], p[1], p[2], p[3], SurfaceColor, Viewport3d);
                    Utility.CreateRectangleFace(p[3], p[2], p[1], p[0], SurfaceColor, Viewport3d);

                }
            }
        }
    }

    public class Utility
    {
        public static void CreateRectangleFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Color surfaceColor, Viewport3D viewport)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = surfaceColor;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D geometry = new GeometryModel3D(mesh, material);
            ModelVisual3D model = new ModelVisual3D();
            model.Content = geometry;
            viewport.Children.Add(model);
        }

        public static Point3D GetNormalize(Point3D pt, double xmin, double xmax, double ymin, double ymax, double zmin, double zmax)
        {
            pt.X = -1 + 2 * (pt.X - xmin) / (xmax - xmin);
            pt.Y = -1 + 2 * (pt.Y - ymin) / (ymax - ymin);
            pt.Z = -1 + 2 * (pt.Z - zmin) / (zmax - zmin);
            return pt;
        }

        public static void CreateTriangleFace(Point3D p0, Point3D p1, Point3D p2, Color color, bool isWireframe, Viewport3D viewport)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D geometry = new GeometryModel3D(mesh, material);
            ModelUIElement3D model = new ModelUIElement3D();
            model.Model = geometry;
            viewport.Children.Add(model);

        }
    }


}

