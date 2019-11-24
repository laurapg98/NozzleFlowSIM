using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;

namespace NozzleDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Nozzle nozzle;
        Rectangle[] nozzlerectangles;
        public MainWindow()
        {
            InitializeComponent();
            this.nozzle = new Nozzle(60, 1, 0.099, 1, 1);
            this.nozzle.AsignarAlturas(0.05);
            
            nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

            fillCanvasNozzle();
        }


        public void fillCanvasNozzle()
        {

            for (int i = 0; i < this.nozzle.GetNumRects(); i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                Rectangle rect_canvas = new Rectangle();
                rect_canvas.Height = rect_nozzle.GetAltura()*100;
                rect_canvas.Width = canvasNozzle.Width / this.nozzle.GetNumRects();
                rect_canvas.Fill = new SolidColorBrush(Colors.Red);
                rect_canvas.StrokeThickness = 0;
                rect_canvas.Stroke = Brushes.Black;
                canvasNozzle.Children.Add(rect_canvas);
                Canvas.SetLeft(rect_canvas, i * rect_canvas.Width);
                Canvas.SetTop(rect_canvas, (canvasNozzle.Height / 2) - (rect_canvas.Height / 2));
                nozzlerectangles[i] = rect_canvas;
            }
        }

        public Color GetColor(double rangeStart /*Complete Red*/, double rangeEnd /*Complete Green*/, double actualValue)
        {
            if (rangeStart >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double green = (255 * value) / max; // calculate green (the closer the value is to max, the greener it gets)
            double red = 255 - green; // set red as inverse of green

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)0);
        }
    }
}
