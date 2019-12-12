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
        int contadordt;
        int numR;
        List<double> listtemp, listdx, listvel, listpre, listden, listtempdt, listdt, listveldt, listpredt, listdendt, listAs;
        Stack<Nozzle> pilaNozzle;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); //Para el tick del timer
        int milisecs = 250;

        int positionThroat;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;

            //escondemos botones etc --> control de errores
            playbut.Visibility = Visibility.Hidden;
            pausebut.Visibility = Visibility.Hidden;
            resetbut.Visibility = Visibility.Hidden;
            onestepbut.Visibility = Visibility.Hidden;
            comboboxcolor.Visibility = Visibility.Hidden;
            PlotsTabControl.Visibility = Visibility.Hidden;
            stepback.Visibility = Visibility.Hidden;

        }

        public void fillCanvasNozzle() // crea el nozzle
        {
            canvasNozzle.Children.Clear();
            sliderthroat.Maximum = this.numR;
            sliderthroat.Minimum = 1;
            for (int i = 0; i < nozzlerectangles.Length; i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                Rectangle rect_canvas = new Rectangle();
                rect_canvas.Height = Math.Min(rect_nozzle.GetAltura()*100,canvasNozzle.Height);
                rect_canvas.Width = canvasNozzle.Width / this.nozzle.GetNumRects();
                rect_canvas.Fill = new SolidColorBrush(Colors.White);
                rect_canvas.StrokeThickness = 0.15;
                rect_canvas.Stroke = Brushes.Black;
                canvasNozzle.Children.Add(rect_canvas);
                Canvas.SetLeft(rect_canvas, i * rect_canvas.Width);
                Canvas.SetTop(rect_canvas, (canvasNozzle.Height / 2) - (rect_canvas.Height / 2));
                nozzlerectangles[i] = rect_canvas;

                sliderthroat.Ticks.Add(Convert.ToDouble(i));
            }
        }

        public void fillCanvasNozzleSlider()
        {
            for (int i = 0; i < nozzlerectangles.Length; i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                nozzlerectangles[i].Height = Math.Min(rect_nozzle.GetAltura() * 100, canvasNozzle.Height);
                canvasNozzle.Children.Add(nozzlerectangles[i]);
                Canvas.SetLeft(nozzlerectangles[i], i * nozzlerectangles[i].Width);
                Canvas.SetTop(nozzlerectangles[i], (canvasNozzle.Height / 2) - (nozzlerectangles[i].Height / 2));
            }
        }

        public Color GetColorMach(double rangeStart, double rangeEnd, double actualValue) // escala de color de la velocidad (Mach)
        {
            if (actualValue >= rangeEnd) return Colors.Red;

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
            if (actualValue >= rangeEnd) return Colors.Blue;

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

        private void parambut_Click(object sender, RoutedEventArgs e) // botón Build
        {
            //paramos el timer en caso de que se construya un nuevo nozzle durante la simulacion
            dispatcherTimer.Stop();

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
                    int numrect = Convert.ToInt32(numrectbox.Text);
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

                            // Courant condition --> stability
                            this.dt = this.nozzle.getdt(this.C, this.dx);

                            nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

                            this.positionThroat = this.nozzle.getthroatpos();
                            this.contadordt = 0;

                            this.listdt = new List<double>(); this.listdt.Add(this.contadordt);
                            this.listdendt = new List<double>(); this.listdendt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetDensP());
                            this.listpredt = new List<double>(); this.listpredt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetPresP());
                            this.listtempdt = new List<double>(); this.listtempdt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetTempP());
                            this.listveldt = new List<double>(); this.listveldt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetVelP());

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
                            stepback.Visibility = Visibility.Visible;

                            //ajustamos el slider
                            sliderthroat.Value = this.numR / 2;
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
                    rect_canvas.Fill = new SolidColorBrush(GetColorMach(0, 2.5, rect_nozzle.GetVelP()));
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
            this.contadordt++;

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

        private void sliderthroat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //funcion que cambia la forma de la tubera
        {
            try
            {


                canvasNozzle.Children.Clear();

                double k = sliderthroat.Value / 10;
                this.nozzle = new Nozzle(this.numR, this.dx, k);
                this.pilaNozzle = new Stack<Nozzle>();

                // Courant condition --> stability
                this.dt = this.nozzle.getdt(this.C, this.dx);

                this.positionThroat = this.nozzle.getthroatpos();
                this.contadordt = 0;

                this.listdt = new List<double>(); this.listdt.Add(this.contadordt);
                this.listdendt = new List<double>(); this.listdendt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetDensP());
                this.listpredt = new List<double>(); this.listpredt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetPresP());
                this.listtempdt = new List<double>(); this.listtempdt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetTempP());
                this.listveldt = new List<double>(); this.listveldt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetVelP());

                fillCanvasNozzleSlider();
                refreshCanvas();
                updateParameterlist();
                crearDataTable();
            }
            catch { }

        }

        private void pausebut_Click(object sender, RoutedEventArgs e) // botón STOP
        {
            dispatcherTimer.Stop();
        }

        private void resetbut_Click(object sender, RoutedEventArgs e) // botón RESET
        {
            //Paramos el timer
            dispatcherTimer.Stop();

            // vaciamos los parámetros
            this.nozzle = new Nozzle();
            this.nozzlerectangles = new Rectangle[0];
            this.datanozzle = new DataTable();
            this.dx = 0;
            this.dt = 0;
            this.C = 0;
            this.numR = 0;
            this.listtemp = new List<double>();
            this.listdx = new List<double>();
            this.listvel = new List<double>();
            this.listpre = new List<double>();
            this.listden = new List<double>();
            this.pilaNozzle = new Stack<Nozzle>();

            // escondemos botones etc        
            playbut.Visibility = Visibility.Hidden;
            pausebut.Visibility = Visibility.Hidden;
            resetbut.Visibility = Visibility.Hidden;
            onestepbut.Visibility = Visibility.Hidden;
            comboboxcolor.Visibility = Visibility.Hidden;
            PlotsTabControl.Visibility = Visibility.Hidden;
            stepback.Visibility = Visibility.Hidden;

            // escondemos canvas
            canvasNozzle.Children.Clear();
            rectanglescale.Fill = new SolidColorBrush(Colors.DarkGray);

            // vaciamos las textbox
            cbox.Text = "";
            dxbox.Text = "";
            numrectbox.Text = "";

            // enseñamos el botón que genera un nuevo nozzle
            parambut.Visibility = Visibility.Visible;
        }

        private void Click_Aboutus(object sender, RoutedEventArgs e) // botón ABOUT US
        {
            Aboutus au = new Aboutus();
            au.ShowDialog();
        }

        private void stepback_Click(object sender, RoutedEventArgs e) // botón STEP BACK
        {
            if (this.pilaNozzle.Count() == 0) // si estamos en el estado inicial no se puede tirar atrás
                MessageBox.Show("There is no data about previous states");
            else // si no:
            {
                // sacamos la última posición de la pila y la asignamos como estado actual
                this.nozzle = this.pilaNozzle.Pop();

                // actualizamos la parte gráfica
                this.refreshCanvas();
                this.updateParameterlist();
                this.crearDataTable();
            }
        }

        private void onestepbut_Click(object sender, RoutedEventArgs e) // botón STEP
        {
            if (this.nozzle.SimulacionAcabada() == true) // si la simulación ha acabado, no se puede seguir
                MessageBox.Show("Simulation has finished");
            else // si no:

                this.EjecutarUnCiclo();
        }

        private void crearDataTable() // crea la tabla de la pestaña 'Data' del tabcontrol
        {
            datanozzle = new DataTable();

            datanozzle.Columns.Add(new DataColumn("X L"));

            for (int i = 0; i <= this.numR; i++)
            {
                datanozzle.Columns.Add(new DataColumn(((i)*this.dx*10).ToString()));
            }

            DataRow dr_a = datanozzle.NewRow(); dr_a[0] = ("A A*");
            DataRow dr_p = datanozzle.NewRow(); dr_p[0] = ("P Po");
            DataRow dr_v = datanozzle.NewRow(); dr_v[0] = ("V Vo");
            DataRow dr_t = datanozzle.NewRow(); dr_t[0] = ("T To");
            DataRow dr_de = datanozzle.NewRow(); dr_de[0] = ("p po");

            for (int i = 1; i < datanozzle.Columns.Count - 1; i++)
            {
                dr_a[i] = listAs[i - 1].ToString();
                dr_p[i] = listpre[i - 1].ToString();
                dr_v[i] = listvel[i - 1].ToString();
                dr_t[i] = listtemp[i - 1].ToString();
                dr_de[i] = listden[i - 1].ToString();
            }

            datanozzle.Rows.Add(dr_a.ItemArray);
            datanozzle.Rows.Add(dr_p.ItemArray);
            datanozzle.Rows.Add(dr_v.ItemArray);
            datanozzle.Rows.Add(dr_t.ItemArray);
            datanozzle.Rows.Add(dr_de.ItemArray);

            gridnozzle.ItemsSource = datanozzle.DefaultView;
            gridnozzle.Items.Refresh();
        }

        private void updateParameterlist() // crea listas con los parámetros del nozzle
        {
            // creamos las listas de datos
            this.listdx = this.nozzle.getNozzleXL(this.dx);
            this.listpre = this.nozzle.getPressures();
            this.listvel = this.nozzle.getVelocities();
            this.listtemp = this.nozzle.getTemperatures();
            this.listden = this.nozzle.getDensities();
            this.listAs = this.nozzle.getNozzleArea();

            this.listdt.Add(this.contadordt);
            this.listdendt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetDensP());
            this.listpredt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetPresP());
            this.listtempdt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetTempP());
            this.listveldt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetVelP());

            // actualizamos el plot
            refreshplotsxl();
            refreshplotstime();
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

        public void refreshplotstime()
        {
            pressureplotdt.Children.Clear();
            var lg = new LineGraph();
            pressureplotdt.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.Red);
            lg.Description = String.Format("Pressure");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdt.ToArray(), this.listpredt.ToArray());

            velocityplotdt.Children.Clear();
            lg = new LineGraph();
            velocityplotdt.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.DarkBlue);
            lg.Description = String.Format("Velocity");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdt.ToArray(), this.listveldt.ToArray());

            temperatureplotdt.Children.Clear();
            lg = new LineGraph();
            temperatureplotdt.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.DarkRed);
            lg.Description = String.Format("Temperature");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdt.ToArray(), this.listtempdt.ToArray());

            densityplotdt.Children.Clear();
            lg = new LineGraph();
            densityplotdt.Children.Add(lg);
            lg.Stroke = new SolidColorBrush(Colors.BlueViolet);
            lg.Description = String.Format("Density");
            lg.StrokeThickness = 2;
            lg.Plot(this.listdt.ToArray(), this.listdendt.ToArray());
        }
    }
}
