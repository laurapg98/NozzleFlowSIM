using System;
using System.Collections.Generic;
using System.Data;
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
using InteractiveDataDisplay.WPF;

namespace NozzleDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Nozzle nozzle;
        Rectangle[] nozzlerectangles;
        DataTable datanozzle;
        double dx;
        double dt;
        double C;
        int numR;
        List<double> listtemp, listdx, listvel, listpre, listden;
        Stack<Nozzle> pilaNozzle;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); //Para el tick del timer
        int milisecs = 1000;
        Nozzle nozzle_CI;

        public MainWindow()
        {
            InitializeComponent();

            //escondemos botones etc --> control de errores
            playbut.Visibility = Visibility.Hidden;
            pausebut.Visibility = Visibility.Hidden;
            resetbut.Visibility = Visibility.Hidden;
            onestepbut.Visibility = Visibility.Hidden;
            comboboxcolor.Visibility = Visibility.Hidden;
            PlotsTabControl.Visibility = Visibility.Hidden;
        }

        public void fillCanvasNozzle() // crea el nozzle
        {
            for (int i = 0; i < nozzlerectangles.Length; i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                Rectangle rect_canvas = new Rectangle();
                rect_canvas.Height = Math.Min(rect_nozzle.GetAltura()*100,canvasNozzle.Height);
                rect_canvas.Width = canvasNozzle.Width / this.nozzle.GetNumRects();
                rect_canvas.Fill = new SolidColorBrush(Colors.White);
                rect_canvas.StrokeThickness = 0.1;
                rect_canvas.Stroke = Brushes.Black;
                canvasNozzle.Children.Add(rect_canvas);
                Canvas.SetLeft(rect_canvas, i * rect_canvas.Width);
                Canvas.SetTop(rect_canvas, (canvasNozzle.Height / 2) - (rect_canvas.Height / 2));
                nozzlerectangles[i] = rect_canvas;
            }
        }

        public Color GetColorMach(double rangeStart, double rangeEnd, double actualValue) // escala de color de la velocidad (Mach)
        {
            if (rangeStart >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double red = (255 * value) / max; 
            double blue = 255 - red; 

            return Color.FromRgb((Byte)red, (Byte)0, (Byte)blue);
        }

        public Color GetColorTemp(double rangeStart, double rangeEnd, double actualValue) // escala de color de la temperatura
        {

            if (actualValue >= rangeEnd) return Color.FromRgb((Byte)255, (Byte)0, (Byte)0);

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double red = (255 * value) / max; 
            double blue = 255 - red;
            double green = blue / 2;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        public Color GetColorPressure(double rangeStart, double rangeEnd, double actualValue) // escala de color de la presión
        {
            if (rangeStart >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double blue = (255 * value) / max;
            double red = 255 - blue;
            double green = red / 2;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        public Color GetColorDensity(double rangeStart, double rangeEnd, double actualValue) // escala de color de la densidad
        {
            if (actualValue >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double blue = 255 - ((255 * value) / max); 
            double red = Math.Min(255,blue*2*value);
            double green = 0;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        private void parambut_Click(object sender, RoutedEventArgs e) // botón SAVE
        {
            //try
            //{
            //    canvasNozzle.Children.Clear();

            //    this.C = Convert.ToSingle(cbox.Text);
            //    this.dx = Convert.ToDouble(dxbox.Text);
            //    this.numR = Convert.ToInt32(numrectbox.Text);

            //    this.nozzle = new Nozzle(this.numR, this.dx);
            //    this.dt = this.nozzle.getdt(this.C, this.dx);

            //    nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

            //    fillCanvasNozzle();
            //    refreshCanvas();
            //    updateParameterlist();
            //    crearDataTable();
            //}
            //catch
            //{
            //    MessageBox.Show("Error");
            //}

            // comprobamos que no estén vacíos
            if (dxbox.Text == "" || cbox.Text == "" || numrectbox.Text == "")
                MessageBox.Show("All parameters should be established\n(^t, ^x & number of rectangles)");
            else
            {
                // comprobamos que tengan el formato que han de tener
                try
                {
                    // comprobamos que sean positivos y diferentes de 0
                    double C = Convert.ToDouble(cbox.Text);
                    double dx = Convert.ToDouble(dxbox.Text);
                    int numrect = Convert.ToInt32(numrectbox.Text) + 1;
                    if (C <= 0 || dx <= 0 || numrect <= 0)
                        MessageBox.Show("All parameters should be positive and different from 0\n(^t, ^x & number of rectangles)");
                    else
                    {
                        // comprobamos que se cumpla la condicion de Courant --> estabilidad
                        if (C > 1)
                        {
                            MessageBox.Show("In order to get a stable simulation, the parameter C should not be bigger than 1");
                        }
                        else
                        {
                            // guardamos los parámetros
                            this.C = C;
                            this.dx = dx;
                            this.numR = numrect;

                            // creamos el nozzle
                            this.nozzle = new Nozzle(this.numR, this.dx);
                            this.pilaNozzle = new Stack<Nozzle>();

                            // guardamos el nozzle solo con las condiciones iniciales
                            this.nozzle_CI = new Nozzle(this.numR, this.dx);

                            // Courant condition --> stability
                            this.dt = this.nozzle.getdt(this.C, this.dx);

                            nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

                            fillCanvasNozzle();
                            refreshCanvas();
                            updateParameterlist();
                            crearDataTable();

                            //enseñamos botones etc
                            playbut.Visibility = Visibility.Visible;
                            pausebut.Visibility = Visibility.Visible;
                            resetbut.Visibility = Visibility.Visible;
                            onestepbut.Visibility = Visibility.Visible;
                            comboboxcolor.Visibility = Visibility.Visible;
                            PlotsTabControl.Visibility = Visibility.Visible;
                        }
                    }
                }
                
                catch
                {
                    MessageBox.Show("The parameters ^t and ^x can be decimal numbers, but the number of rectangles not\nPlease check it");
                }
                
            }
        }

        private void comboboxcolor_SelectionChanged(object sender, SelectionChangedEventArgs e) // click en el combobox de escoger propiedad a mostrar
        {
            try
            {
                refreshCanvas();
            }
            catch
            {

            }
        }

        private void refreshCanvas() // actualiza la visualización (colores)
        {
            if (comboboxcolor.SelectedIndex == 0) //Pressure
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                    rect_canvas.Fill = new SolidColorBrush(GetColorPressure(0, 1, rect_nozzle.GetPresP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorPressure(0, 1, 1), GetColorPressure(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorPressure(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 1) //Velocity
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                    rect_canvas.Fill = new SolidColorBrush(GetColorMach(0, 4, rect_nozzle.GetVelP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorMach(0, 2.5, 2.5), GetColorMach(0, 2.5, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorMach(0, 2.5, 1.25);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 2) //Temperature
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                    rect_canvas.Fill = new SolidColorBrush(GetColorTemp(0, 1, rect_nozzle.GetTempP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorTemp(0, 1, 1), GetColorTemp(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorTemp(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 3) //Density
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                    rect_canvas.Fill = new SolidColorBrush(GetColorDensity(0, 1, rect_nozzle.GetDensP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorDensity(0, 1, 1), GetColorDensity(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorDensity(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) // botón ANDERSON
        {
            Anderson a = new Anderson();
            a.GetAx(this.dx);
            a.GetAt(this.dt);
            a.GetEstadoInicial(this.nozzle_CI);
            a.ShowDialog();
        }

        private void EjecutarUnCiclo() // función que ejecuta un ciclo
        {
            // copiamos el estado actual del nozzle y lo metemos en la pila
            Nozzle nozzlecopia = new Nozzle(this.nozzle);
            pilaNozzle.Push(nozzlecopia);

            // ejecutamos un ciclo
            this.nozzle.EjecutarCiclo(this.dt, this.dx, 1.4);
            this.nozzle.ActualizarEstados();

            // actualizamos la parte gráfica
            this.refreshCanvas();
            this.updateParameterlist();
            this.crearDataTable();
        }

        private void playbut_Click(object sender, RoutedEventArgs e) // botón START
        {
            // configuramos el timer
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, this.milisecs);

            //iniciamos timer
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) // lo que hace cada interval del timer
        {
            if (this.nozzle.SimulacionAcabada() == true)
            {
                MessageBox.Show("Simulation has finished");
                dispatcherTimer.Stop();
            }
            else
                this.EjecutarUnCiclo();
        }

        private void pausebut_Click(object sender, RoutedEventArgs e) // botón STOP
        {
            dispatcherTimer.Stop();
        }

        private void resetbut_Click(object sender, RoutedEventArgs e) // botón RESET
        {

        }

        private void Click_Aboutus(object sender, RoutedEventArgs e) // botón ABOUT US
        {
            Aboutus au = new Aboutus();
            au.ShowDialog();
        }

        private void onestepbut_Click(object sender, RoutedEventArgs e) // botón STEP
        {
            if (this.nozzle.SimulacionAcabada() == true)
                MessageBox.Show("Simulation has finished");
            else
                this.EjecutarUnCiclo();
        }

        private void crearDataTable()
        {
            datanozzle = new DataTable();

            datanozzle.Columns.Add(new DataColumn(" "));

            for (int i = 0; i < this.numR; i++)
            {
                datanozzle.Columns.Add(new DataColumn((i + 1).ToString()));
            }

            DataRow dr_dx = datanozzle.NewRow(); dr_dx[0] = ("X L");
            DataRow dr_p = datanozzle.NewRow(); dr_p[0] = ("P Po");
            DataRow dr_v = datanozzle.NewRow(); dr_v[0] = ("V Vo");
            DataRow dr_t = datanozzle.NewRow(); dr_t[0] = ("T To");
            DataRow dr_de = datanozzle.NewRow(); dr_de[0] = ("p po");

            for (int i = 1; i < datanozzle.Columns.Count; i++)
            {
                dr_dx[i] = listdx[i - 1];
                dr_p[i] = listpre[i - 1];
                dr_v[i] = listvel[i - 1];
                dr_t[i] = listtemp[i - 1];
                dr_de[i] = listden[i - 1];
            }

            datanozzle.Rows.Add(dr_dx);
            datanozzle.Rows.Add(dr_p);
            datanozzle.Rows.Add(dr_v);
            datanozzle.Rows.Add(dr_t);
            datanozzle.Rows.Add(dr_de);


            gridnozzle.ItemsSource = datanozzle.DefaultView;
            gridnozzle.DataContext = datanozzle.DefaultView;
            gridnozzle.Items.Refresh();
        }

        private void updateParameterlist()
        {
            this.listdx = this.nozzle.getNozzleXL(this.dx);
            this.listpre = this.nozzle.getPressures();
            this.listvel = this.nozzle.getVelocities();
            this.listtemp = this.nozzle.getTemperatures();
            this.listden = this.nozzle.getDensities();

            refreshplotsxl();
        }

        private void refreshplotsxl()
        {
            pressureplot.Children.Clear();
            var lg = new LineGraph();
            pressureplot.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.Red);
            lg.Description = String.Format("Pressure");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdx.ToArray(), this.listpre.ToArray());

            velocityplot.Children.Clear();
            lg = new LineGraph();
            velocityplot.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.DarkBlue);
            lg.Description = String.Format("Velocity");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdx.ToArray(), this.listvel.ToArray());

            temperatureplot.Children.Clear();
            lg = new LineGraph();
            temperatureplot.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.DarkRed);
            lg.Description = String.Format("Temperature");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdx.ToArray(), this.listtemp.ToArray());

            densityplot.Children.Clear();
            lg = new LineGraph();
            densityplot.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.BlueViolet);
            lg.Description = String.Format("Density");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdx.ToArray(), this.listden.ToArray());

        }

    }
}
