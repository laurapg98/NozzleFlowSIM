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
using System.Windows.Media.Media3D;
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
        List<double> listtemp, listdx, listvel, listpre, listden, listtempdt, listdt, listveldt, listpredt, listdendt, listAs, listmassflowdt;
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
            padlockimg.Visibility = Visibility.Hidden;
            contadortxt.Visibility = Visibility.Hidden;
            unitbox.Visibility = Visibility.Hidden;

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
                rect_canvas.Height = Math.Min(rect_nozzle.GetAltura()*100,canvasNozzle.ActualHeight);
                rect_canvas.Width = canvasNozzle.ActualWidth / this.nozzle.GetNumRects();
                rect_canvas.Fill = new SolidColorBrush(Colors.White);
                rect_canvas.StrokeThickness = 0.15;
                rect_canvas.Stroke = Brushes.Black;
                canvasNozzle.Children.Add(rect_canvas);
                Canvas.SetLeft(rect_canvas, i * rect_canvas.Width);
                Canvas.SetTop(rect_canvas, (canvasNozzle.ActualWidth / 2) - (rect_canvas.Height / 2));
                nozzlerectangles[i] = rect_canvas;

                sliderthroat.Ticks.Add(Convert.ToDouble(i));
            }
        }

        public void fillCanvasNozzleSlider()
        {
            for (int i = 0; i < nozzlerectangles.Length; i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i + 1);
                nozzlerectangles[i].Height = Math.Min(rect_nozzle.GetAltura() * 100, canvasNozzle.ActualHeight);
                canvasNozzle.Children.Add(nozzlerectangles[i]);
                Canvas.SetLeft(nozzlerectangles[i], i * nozzlerectangles[i].Width);
                Canvas.SetTop(nozzlerectangles[i], (canvasNozzle.ActualHeight / 2) - (nozzlerectangles[i].Height / 2));
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
                MessageBox.Show("All parameters should be established\n(Courant parameter, horizontal axis steps & number of rectangles)");
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
                        MessageBox.Show("All parameters should be positive and different from 0\n(Courant parameter, horizontal axis steps & number of rectangles)", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                    {
                        // comprobamos que se cumpla la condicion de Courant --> estabilidad
                        if (C > 1)
                        {
                            MessageBox.Show("In order to get a stable simulation, the parameter C should not be bigger than 1","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
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

                            // Courant condition --> stability
                            this.dt = this.nozzle.getdt(this.C, this.dx);

                            nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

                            this.positionThroat = this.nozzle.getthroatpos();
                            this.contadordt = 0;
                            contadortxt.Text = " Contador: " + this.contadordt.ToString() + " Δt";

                            this.listdt = new List<double>(); this.listdt.Add(this.contadordt);
                            this.listdendt = new List<double>(); this.listdendt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetDensP());
                            this.listpredt = new List<double>(); this.listpredt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetPresP());
                            this.listtempdt = new List<double>(); this.listtempdt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetTempP());
                            this.listveldt = new List<double>(); this.listveldt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetVelP());
                            this.listmassflowdt = new List<double>(); this.listmassflowdt.Add(listdendt[this.contadordt] * this.nozzle.GetRectangulo(this.positionThroat).GetArea() * this.listveldt[this.contadordt]);

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
                            contadortxt.Visibility = Visibility.Visible;
                            unitbox.Visibility = Visibility.Visible;

                            //ajustamos el slider
                            sliderthroat.Maximum = this.numR;
                            sliderthroat.Value = this.numR / 2;
                            unlockSlider();
                        }
                    }
                }


                catch
                {
                    MessageBox.Show("The parameters ^t and ^x can be decimal numbers, but the number of rectangles not\nPlease check it","Error",MessageBoxButton.OK,MessageBoxImage.Error);
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
                unitbox.Text = "[P/Po]";
                maxlabel.Text = "1";
                minlabel.Text = "0";
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
                unitbox.Text = "[V/Vo]";
                maxlabel.Text = "2.5";
                minlabel.Text = "0";
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
                unitbox.Text = "[T/To]";
                maxlabel.Text = "1";
                minlabel.Text = "0";
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
                unitbox.Text = "[ρ/ρo]";
                maxlabel.Text = "1";
                minlabel.Text = "0";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) // botón ANDERSON
        {
            if (this.C == 0.5 && this.dx == 0.1 && this.numR == 30)
            {
                Anderson a = new Anderson();
                a.ShowDialog();
            }
            else
            {
                MessageBox.Show("Anderson validation only availabe for default values", "Warning",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
            contadortxt.Text = " Contador: " + this.contadordt.ToString() + " Δt";

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
            lockSlider();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) // lo que hace cada interval del timer
        {
            this.EjecutarUnCiclo();
        }

        private void sliderthroat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //funcion que cambia la forma de la tubera
        {
            try
            {
                canvasNozzle.Children.Clear();

                double k = (sliderthroat.Value)*this.dx;
                this.nozzle = new Nozzle(this.numR, this.dx, k);
                this.pilaNozzle = new Stack<Nozzle>();

                // Courant condition --> stability
                this.dt = this.nozzle.getdt(this.C, this.dx);

                this.positionThroat = this.nozzle.getthroatpos();
                this.contadordt = 0;
                contadortxt.Text = " Contador: " + this.contadordt.ToString() + " Δt";

                this.listdt = new List<double>(); this.listdt.Add(this.contadordt);
                this.listdendt = new List<double>(); this.listdendt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetDensP());
                this.listpredt = new List<double>(); this.listpredt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetPresP());
                this.listtempdt = new List<double>(); this.listtempdt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetTempP());
                this.listveldt = new List<double>(); this.listveldt.Add(this.nozzle.GetRectangulo(this.positionThroat).GetVelP());
                this.listmassflowdt = new List<double>(); this.listmassflowdt.Add(listdendt[this.contadordt] * this.nozzle.GetRectangulo(this.positionThroat).GetArea() * this.listveldt[this.contadordt]);

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

        private void Defaultbut_Click(object sender, RoutedEventArgs e)
        {
            cbox.Text= (0.5).ToString();
            dxbox.Text = (0.1).ToString();
            numrectbox.Text = (30).ToString();
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
            contadortxt.Visibility = Visibility.Hidden;
            unitbox.Visibility = Visibility.Hidden;

            // escondemos canvas
            canvasNozzle.Children.Clear();
            rectanglescale.Fill = new SolidColorBrush(Colors.DarkGray);

            // vaciamos las textbox
            cbox.Text = "";
            dxbox.Text = "";
            numrectbox.Text = "";

            // enseñamos el botón que genera un nuevo nozzle
            parambut.Visibility = Visibility.Visible;

            unlockSlider();

            myViewport.Children.Clear();
            massflowxlplot.Children.Clear();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //Cambio al 3D
        {
            if (this.nozzle != null)
            {
                //Controles para restablecer el viewport3d para ir borrando las anteriores tuberas
                myViewport.Children.Clear();
                DirectionalLight dl = new DirectionalLight();
                dl.Color = Colors.White;
                dl.Direction = new Vector3D(-1, -1, -1);
                DirectionalLight dl_2 = new DirectionalLight();
                dl_2.Color = Colors.White;
                dl_2.Direction = new Vector3D(1, 1, 1);
                Model3DGroup myModel3DGroup = new Model3DGroup();
                myModel3DGroup.Children.Add(dl);
                myModel3DGroup.Children.Add(dl_2);
                ModelVisual3D myModelVisual3D = new ModelVisual3D();
                myModelVisual3D.Content = (myModel3DGroup);
                myViewport.Children.Add(myModelVisual3D);

                ParametricCurve3D ps = new ParametricCurve3D();
                ps.SurfaceColor = Colors.LightBlue;
                ps.IsHiddenLine = false;
                ps.Viewport3d = myViewport;

                ps.Vmin = -this.nozzle.getK();
                ps.Vmax = (this.numR*this.dx) - this.nozzle.getK();
                ps.Umin = 0;
                ps.Umax = 2 * Math.PI;
                ps.Nu = this.numR;
                ps.Nv = this.numR;
                ps.CreateSurface(Hyperboloid);
            }

            else
            {
                MessageBox.Show("No nozzle to mesh 3D", "Error");
                tab2d.IsSelected = true;
                tab3d.IsSelected = false;
            }
        }

        private Point3D Hyperboloid(double u, double v) //Funcion de nuestra tubera 3D (hiperboloide de una hoja)
        {
            double x = Math.Cosh(v) * Math.Cos(u);
            double y = Math.Cosh(v) * Math.Sin(u);
            double z = 2.2 * Math.Sinh(v);
            return new Point3D(x, y, z);
        }

        private void click_openfile(object sender, RoutedEventArgs e) // click en FILE --> OPEN FILE
        {
            //paramos el timer en caso de que se construya un nuevo nozzle durante la simulacion
            dispatcherTimer.Stop();

            //abrimos el form
            OpenFile openfilewindow = new OpenFile();
            openfilewindow.ShowDialog();

            //leemos el nombre del fichero seleccionado en el form
            string fichero = openfilewindow.getFichero();

            if (fichero == null) //si no lo coge bien o se cierra antes de cogerlo
                MessageBox.Show("Any file selected. Try again.");
            else //si no:
            {
                // vaciamos nozzle y la pila
                this.nozzle = new Nozzle();
                this.pilaNozzle = new Stack<Nozzle>();

                try
                {
                    // leemos fichero
                    double[,] param = this.nozzle.CargarEstadoFichero(fichero);

                    // guardamos los parámetros
                    this.contadordt = Convert.ToInt32(param[0, 0]);
                    this.dx = param[1, 0];
                    this.dt = param[1, 1];
                    this.C = param[0, 2];
                    this.numR = this.nozzle.GetNumRects();
                    this.positionThroat = this.nozzle.getthroatpos();

                    // escribimos en los textbox
                    cbox.Text = Convert.ToString(this.C);
                    dxbox.Text = Convert.ToString(this.dx);
                    numrectbox.Text = Convert.ToString(this.numR);

                    // escribimos n la label del contador
                    contadortxt.Text = " Contador: " + this.contadordt.ToString() + " Δt";

                    // creamos el mass flow 
                    this.nozzle.getMassFlow();

                    // creamos los rectangulos para dibujar
                    nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];
                    
                    // creamos las listas para los plots respecto del tiempo
                    if (this.contadordt != 0)
                    {
                        this.listdt = new List<double>();
                        this.listdendt = new List<double>();
                        this.listpredt = new List<double>();
                        this.listtempdt = new List<double>();
                        this.listveldt = new List<double>();
                        int cont = 0;
                        while (cont <= contadordt + 1)
                        {
                            listdt.Add(param[2, cont]);
                            listdendt.Add(param[3, cont]);
                            listtempdt.Add(param[4, cont]);
                            listveldt.Add(param[5, cont]);
                            listpredt.Add(param[6, cont]);
                            cont++;
                        }
                    }

                    // creamos las listas para los plots respecto el eje X
                    this.listdx = this.nozzle.getNozzleXL(this.dx);
                    this.listpre = this.nozzle.getPressures();
                    this.listvel = this.nozzle.getVelocities();
                    this.listtemp = this.nozzle.getTemperatures();
                    this.listden = this.nozzle.getDensities();
                    this.listAs = this.nozzle.getNozzleArea();

                    // parte gráfica:
                    fillCanvasNozzle();
                    refreshCanvas();
                    refreshplotsxl();
                    refreshplotstime();
                    refreshplotmassflow();
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
                    sliderthroat.Value = this.positionThroat;
                    lockSlider();
                }
                catch 
                {
                    MessageBox.Show("Could not open the file.");
                }
            }
        }

        private void click_savefile(object sender, RoutedEventArgs e) // click en FILE --> SAVE FILE
        {
            if (this.nozzle == null) //comprobamos que haya algo
                MessageBox.Show("There is no state in order to save");
            else //si hay:
            {
                //guardamos los parámetros actuales
                double[] param = { this.dx, this.dt, 1.4, this.contadordt, this.C };

                //abrimos el form y le pasamos los parámetros y el estado actual
                SaveFile savefilewindow = new SaveFile();
                savefilewindow.getParametros(param, this.nozzle, listdt, listdendt, listtempdt, listveldt, listpredt);
                savefilewindow.ShowDialog();
            }
        }

        private void Click_Aboutus(object sender, RoutedEventArgs e) // botón ABOUT US
        {
            Aboutus au = new Aboutus();
            au.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) // botón PDF
        {
            Report r = new Report();
            r.ShowDialog();
        }

        private void zoominbut_Click(object sender, RoutedEventArgs e) // ZOOM IN en el 3D
        {
            Point3D pt = camera.Position;
            camera.Position = new Point3D(pt.X + 5, pt.Y - 5, pt.Z - 5);
        }

        private void zoomoutbut_Click(object sender, RoutedEventArgs e) // ZOOM OUT en el 3D
        {
            Point3D pt = camera.Position;
            camera.Position = new Point3D(pt.X - 5, pt.Y + 5, pt.Z + 5);
        }

        private void stepback_Click(object sender, RoutedEventArgs e) // botón STEP BACK
        {
            if (this.pilaNozzle.Count() == 0) // si estamos en el estado inicial no se puede tirar atrás
                MessageBox.Show("There is no data about previous states");
            else // si no:
            {
                // sacamos la última posición de la pila y la asignamos como estado actual
                this.nozzle = this.pilaNozzle.Pop();

                // actualizamos las listas para el plot respecto al eje X
                this.listdx = this.nozzle.getNozzleXL(this.dx);
                this.listpre = this.nozzle.getPressures();
                this.listvel = this.nozzle.getVelocities();
                this.listtemp = this.nozzle.getTemperatures();
                this.listden = this.nozzle.getDensities();
                this.listAs = this.nozzle.getNozzleArea();

                // eliminamos la última posición de las listas para el plot respecto al tiempo
                contadordt = contadordt - 1;
                listtempdt.RemoveAt(this.contadordt);
                listdendt.RemoveAt(this.contadordt);
                listpredt.RemoveAt(this.contadordt);
                listveldt.RemoveAt(this.contadordt);
                listdt.RemoveAt(this.contadordt);

                // escribimos en la label del contador
                contadortxt.Text = " Contador: " + this.contadordt.ToString() + " Δt";

                // actualizamos la parte gráfica
                this.refreshCanvas();
                refreshplotsxl();
                refreshplotstime();
                refreshplotmassflow();
                this.crearDataTable();
            }
        }

        private void onestepbut_Click(object sender, RoutedEventArgs e) // botón STEP
        {
            this.EjecutarUnCiclo();
            lockSlider();
        }

        private void crearDataTable() // crea la tabla de la pestaña 'Data' del tabcontrol
        {
            datanozzle = new DataTable();

            datanozzle.Columns.Add(new DataColumn("X L"));

            for (int i = 0; i <= this.numR; i++)
            {
                datanozzle.Columns.Add(new DataColumn(((i * this.dx)).ToString()));
            }

            DataRow dr_a = datanozzle.NewRow(); dr_a[0] = ("A A*");
            DataRow dr_p = datanozzle.NewRow(); dr_p[0] = ("P Po");
            DataRow dr_v = datanozzle.NewRow(); dr_v[0] = ("V Vo");
            DataRow dr_t = datanozzle.NewRow(); dr_t[0] = ("T To");
            DataRow dr_de = datanozzle.NewRow(); dr_de[0] = ("p po");

            for (int i = 1; i <= datanozzle.Columns.Count - 1; i++)
            {
                dr_a[i] = listAs[i - 1].ToString("0.000");
                dr_p[i] = listpre[i - 1].ToString("0.000");
                dr_v[i] = listvel[i - 1].ToString("0.000");
                dr_t[i] = listtemp[i - 1].ToString("0.000");
                dr_de[i] = listden[i - 1].ToString("0.000");
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
            this.listmassflowdt.Add(listdendt[this.contadordt] * this.nozzle.GetRectangulo(this.positionThroat).GetArea() * this.listveldt[this.contadordt]);
            
            // actualizamos los plots
            refreshplotsxl();
            refreshplotstime();
            refreshplotmassflow();
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

        public void refreshplotmassflow()
        {
            if (this.contadordt == 1)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.Red);
                lg.Description = String.Format("0 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }
            if (this.contadordt == 50)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.DarkBlue);
                lg.Description = String.Format("50 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }
            if (this.contadordt == 100)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.DarkGreen);
                lg.Description = String.Format("100 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }
            if (this.contadordt == 150)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.Purple);
                lg.Description = String.Format("150 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }
            if (this.contadordt == 200)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.Orange);
                lg.Description = String.Format("200 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }
            if (this.contadordt == 700)
            {
                var lg = new LineGraph();
                massflowxlplot.Children.Add(lg);
                lg.Stroke = new SolidColorBrush(Colors.Yellow);
                lg.Description = String.Format("700 Δt");
                lg.StrokeThickness = 2;
                lg.Plot(this.listdx.ToArray(), this.nozzle.getMassFlow());
            }

            massflowxlplotdt.Children.Clear();
            var lg_2 = new LineGraph();
            massflowxlplotdt.Children.Add(lg_2);
            lg_2.Stroke = new SolidColorBrush(Colors.Blue);
            lg_2.Description = String.Format("Mass Flow at throat");
            lg_2.StrokeThickness = 2;
            lg_2.Plot(this.listdt.ToArray(), this.listmassflowdt.ToArray());


        }

        public void lockSlider()
        {
            sliderthroat.IsEnabled = false;
            padlockimg.Visibility = Visibility.Visible;
        }

        public void unlockSlider()
        {
            sliderthroat.IsEnabled = true;
            padlockimg.Visibility = Visibility.Hidden;
        }

        private void ScrollBar_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < myViewport.Children.Count; i++)
            {
                myViewport.Children[i].Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), scrolh.Value));
            }         
        }

    }
}
