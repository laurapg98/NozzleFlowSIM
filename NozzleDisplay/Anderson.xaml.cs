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
using System.Windows.Shapes;
using ClassLibrary;

namespace NozzleDisplay
{
    /// <summary>
    /// Interaction logic for Anderson.xaml
    /// </summary>
    public partial class Anderson : Window
    {
        DataTable AndersonTabla_FisrtStep;
        DataTable AndersonTabla_SteadyState;
        DataTable AndersonTabla_InitialConditions;
        double Ax;
        double At;
        Nozzle nozzle;
        DataTable NozzleTabla_FirstStep;
        DataTable NozzleTabla_SteadyState;
        DataTable NozzleTabla_InitialConditions;

        public Anderson()
        {
            InitializeComponent();

            // creamos el nozzle con las condiciones iniciales
            this.Ax = 0.1;
            this.nozzle = new Nozzle(30, this.Ax);
            this.At = this.nozzle.getdt(0.5, this.Ax);

            // creamos las tablas
            AndersonTabla_FisrtStep = new DataTable();
            AndersonTabla_SteadyState = new DataTable();
            AndersonTabla_InitialConditions = new DataTable();
            NozzleTabla_FirstStep = new DataTable();
            NozzleTabla_SteadyState = new DataTable();
            NozzleTabla_InitialConditions = new DataTable();
            createAndersonTables();
            this.CreateSimulationTables();

            // por defecto: first step table
            andersongrid.ItemsSource = AndersonTabla_FisrtStep.DefaultView;
            andersongrid.DataContext = AndersonTabla_FisrtStep.DefaultView;
            andersongrid.Items.Refresh();
            datasimgrid.ItemsSource = NozzleTabla_FirstStep.DefaultView;
            datasimgrid.DataContext = NozzleTabla_FirstStep.DefaultView;
            datasimgrid.Items.Refresh();
        }

        private void andersoncombobox_SelectionChanged(object sender, SelectionChangedEventArgs e) // click en cualquier desplegable --> actualiza la tabla
        {
            try
            {
                if (andersoncombobox.SelectedIndex == 0) // FIRST STEP
                { 
                    // ANDERSON
                    andersongrid.ItemsSource = AndersonTabla_FisrtStep.DefaultView; 
                    andersongrid.DataContext = AndersonTabla_FisrtStep.DefaultView;
                    // SIMULATION
                    datasimgrid.ItemsSource = NozzleTabla_FirstStep.DefaultView;
                    datasimgrid.DataContext = NozzleTabla_FirstStep.DefaultView;
                }
                if (andersoncombobox.SelectedIndex == 1) // STEADY STATE
                {
                    // ANDERSON
                    andersongrid.ItemsSource = AndersonTabla_SteadyState.DefaultView;
                    andersongrid.DataContext = AndersonTabla_SteadyState.DefaultView;
                    // SIMULATION
                    datasimgrid.ItemsSource = NozzleTabla_SteadyState.DefaultView;
                    datasimgrid.DataContext = NozzleTabla_SteadyState.DefaultView;
                }
                if (andersoncombobox.SelectedIndex == 2) // INITIAL CONDITIONS
                {
                    // ANDERSON
                    andersongrid.ItemsSource = AndersonTabla_InitialConditions.DefaultView;
                    andersongrid.DataContext = AndersonTabla_InitialConditions.DefaultView;
                    // SIMULATION
                    datasimgrid.ItemsSource = NozzleTabla_InitialConditions.DefaultView;
                    datasimgrid.DataContext = NozzleTabla_InitialConditions.DefaultView;
                }

                andersongrid.Items.Refresh();
            }
            catch
            {

            }
        }

        private void createAndersonTables()
        {
            // FIRST STEP
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("x L"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("A A*"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("ρ ρo"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("V ao"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("T To"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("P Po"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("M"));
                                                                // FALTA CAMBIAR COLUMNAS DE VEL Y TEMP (LAS 2 DE ENMEDIO) --> ENTERAS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AndersonTabla_FisrtStep.Rows.Add(0.0, 5.950, 1.000, 0.111, 1.000, 1.000, 0.111);
            AndersonTabla_FisrtStep.Rows.Add(0.1, 5.312, 0.955, 0.212, 0.972, 0.928, 0.215);
            AndersonTabla_FisrtStep.Rows.Add(0.2, 4.718, 1.000, 0.125, 1.000, 0.881, 0.320);
            AndersonTabla_FisrtStep.Rows.Add(0.3, 4.168, 1.000, 0.141, 1.000, 0.836, 0.427);
            AndersonTabla_FisrtStep.Rows.Add(0.4, 3.662, 1.000, 0.160, 1.000, 0.791, 0.534);
            AndersonTabla_FisrtStep.Rows.Add(0.5, 3.200, 0.999, 0.187, 1.000, 0.748, 0.640);
            AndersonTabla_FisrtStep.Rows.Add(0.6, 2.782, 0.963, 0.228, 0.983, 0.706, 0.747);
            AndersonTabla_FisrtStep.Rows.Add(0.7, 2.408, 0.927, 0.271, 0.967, 0.665, 0.854);
            AndersonTabla_FisrtStep.Rows.Add(0.8, 2.078, 0.891, 0.325, 0.950, 0.625, 0.960);
            AndersonTabla_FisrtStep.Rows.Add(0.9, 1.792, 0.854, 0.389, 0.934, 0.585, 1.067);
            AndersonTabla_FisrtStep.Rows.Add(1.0, 1.550, 0.818, 0.467, 0.917, 0.545, 1.174);
            AndersonTabla_FisrtStep.Rows.Add(1.1, 1.352, 0.781, 0.557, 0.900, 0.506, 1.281);
            AndersonTabla_FisrtStep.Rows.Add(1.2, 1.198, 0.744, 0.656, 0.883, 0.466, 1.389);
            AndersonTabla_FisrtStep.Rows.Add(1.3, 1.088, 0.707, 0.759, 0.866, 0.426, 1.498);
            AndersonTabla_FisrtStep.Rows.Add(1.4, 1.022, 0.670, 0.854, 0.849, 0.387, 1.609);
            AndersonTabla_FisrtStep.Rows.Add(1.5, 1.000, 0.633, 0.930, 0.833, 0.349, 1.720);
            AndersonTabla_FisrtStep.Rows.Add(1.6, 1.022, 0.594, 0.979, 0.800, 0.312, 1.833);
            AndersonTabla_FisrtStep.Rows.Add(1.7, 1.088, 0.555, 0.992, 0.766, 0.278, 1.945);
            AndersonTabla_FisrtStep.Rows.Add(1.8, 1.198, 0.517, 0.975, 0.731, 0.247, 2.058);
            AndersonTabla_FisrtStep.Rows.Add(1.9, 1.352, 0.478, 0.939, 0.695, 0.218, 2.171);
            AndersonTabla_FisrtStep.Rows.Add(2.0, 1.550, 0.440, 0.893, 0.660, 0.192, 2.282);
            AndersonTabla_FisrtStep.Rows.Add(2.1, 1.792, 0.401, 0.848, 0.625, 0.168, 2.393);
            AndersonTabla_FisrtStep.Rows.Add(2.2, 2.078, 0.362, 0.809, 0.590, 0.146, 2.504);
            AndersonTabla_FisrtStep.Rows.Add(2.3, 2.408, 0.324, 0.781, 0.554, 0.126, 2.614);
            AndersonTabla_FisrtStep.Rows.Add(2.4, 2.782, 0.285, 0.766, 0.519, 0.107, 2.724);
            AndersonTabla_FisrtStep.Rows.Add(2.5, 3.200, 0.246, 0.768, 0.484, 0.090, 2.834);
            AndersonTabla_FisrtStep.Rows.Add(2.6, 3.662, 0.208, 0.791, 0.448, 0.073, 2.944);
            AndersonTabla_FisrtStep.Rows.Add(2.7, 4.168, 0.169, 0.846, 0.412, 0.058, 3.055);
            AndersonTabla_FisrtStep.Rows.Add(2.8, 4.718, 0.131, 0.949, 0.375, 0.044, 3.167);
            AndersonTabla_FisrtStep.Rows.Add(2.9, 5.312, 0.093, 1.133, 0.324, 0.032, 3.281);
            AndersonTabla_FisrtStep.Rows.Add(3.0, 5.950, 0.063, 1.438, 0.200, 0.020, 3.406);


            // STEADY STATE
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("x L"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("A A*"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("ρ ρo"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("V ao"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("T To"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("P Po"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("M"));

            AndersonTabla_SteadyState.Rows.Add(0.0, 5.950, 1.000, 0.098, 1.000, 1.000, 0.098);
            AndersonTabla_SteadyState.Rows.Add(0.1, 5.312, 0.999, 0.110, 0.999, 0.998, 0.110);
            AndersonTabla_SteadyState.Rows.Add(0.2, 4.718, 0.997, 0.124, 0.999, 0.996, 0.124);
            AndersonTabla_SteadyState.Rows.Add(0.3, 4.168, 0.995, 0.141, 0.998, 0.993, 0.141);
            AndersonTabla_SteadyState.Rows.Add(0.4, 3.662, 0.992, 0.161, 0.997, 0.989, 0.161);
            AndersonTabla_SteadyState.Rows.Add(0.5, 3.200, 0.988, 0.184, 0.995, 0.983, 0.185);
            AndersonTabla_SteadyState.Rows.Add(0.6, 2.782, 0.982, 0.213, 0.993, 0.975, 0.214);
            AndersonTabla_SteadyState.Rows.Add(0.7, 2.408, 0.974, 0.249, 0.989, 0.964, 0.250);
            AndersonTabla_SteadyState.Rows.Add(0.8, 2.078, 0.962, 0.292, 0.985, 0.948, 0.294);
            AndersonTabla_SteadyState.Rows.Add(0.9, 1.792, 0.946, 0.344, 0.978, 0.926, 0.348);
            AndersonTabla_SteadyState.Rows.Add(1.0, 1.550, 0.923, 0.408, 0.969, 0.894, 0.415);
            AndersonTabla_SteadyState.Rows.Add(1.1, 1.352, 0.891, 0.485, 0.955, 0.851, 0.496);
            AndersonTabla_SteadyState.Rows.Add(1.2, 1.198, 0.847, 0.577, 0.935, 0.792, 0.596);
            AndersonTabla_SteadyState.Rows.Add(1.3, 1.088, 0.789, 0.682, 0.909, 0.718, 0.715);
            AndersonTabla_SteadyState.Rows.Add(1.4, 1.022, 0.718, 0.798, 0.874, 0.628, 0.854);
            AndersonTabla_SteadyState.Rows.Add(1.5, 1.000, 0.648, 0.904, 0.839, 0.544, 0.987);
            AndersonTabla_SteadyState.Rows.Add(1.6, 1.022, 0.548, 1.046, 0.783, 0.429, 1.182);
            AndersonTabla_SteadyState.Rows.Add(1.7, 1.088, 0.462, 1.164, 0.731, 0.338, 1.361);
            AndersonTabla_SteadyState.Rows.Add(1.8, 1.198, 0.384, 1.272, 0.679, 0.261, 1.544);
            AndersonTabla_SteadyState.Rows.Add(1.9, 1.352, 0.316, 1.368, 0.628, 0.198, 1.726);
            AndersonTabla_SteadyState.Rows.Add(2.0, 1.550, 0.260, 1.452, 0.581, 0.151, 1.905);
            AndersonTabla_SteadyState.Rows.Add(2.1, 1.792, 0.214, 1.524, 0.538, 0.115, 2.077);
            AndersonTabla_SteadyState.Rows.Add(2.2, 2.078, 0.177, 1.586, 0.500, 0.088, 2.243);
            AndersonTabla_SteadyState.Rows.Add(2.3, 2.408, 0.148, 1.639, 0.466, 0.069, 2.402);
            AndersonTabla_SteadyState.Rows.Add(2.4, 2.782, 0.124, 1.685, 0.436, 0.054, 2.544);
            AndersonTabla_SteadyState.Rows.Add(2.5, 3.200, 0.106, 1.725, 0.409, 0.043, 2.698);
            AndersonTabla_SteadyState.Rows.Add(2.6, 3.662, 0.090, 1.760, 0.384, 0.035, 2.838);
            AndersonTabla_SteadyState.Rows.Add(2.7, 4.168, 0.078, 1.790, 0.363, 0.028, 2.969);
            AndersonTabla_SteadyState.Rows.Add(2.8, 4.718, 0.068, 1.817, 0.344, 0.023, 3.100);
            AndersonTabla_SteadyState.Rows.Add(2.9, 5.312, 0.060, 1.840, 0.327, 0.019, 3.216);
            AndersonTabla_SteadyState.Rows.Add(3.0, 5.950, 0.052, 1.863, 0.310, 0.016, 3.345);


            // INITIAL CONDITIONS
            AndersonTabla_InitialConditions.Columns.Add(new DataColumn("x L"));
            AndersonTabla_InitialConditions.Columns.Add(new DataColumn("A A*"));
            AndersonTabla_InitialConditions.Columns.Add(new DataColumn("ρ ρo"));
            AndersonTabla_InitialConditions.Columns.Add(new DataColumn("V ao"));
            AndersonTabla_InitialConditions.Columns.Add(new DataColumn("T To"));

            AndersonTabla_InitialConditions.Rows.Add(0.0, 5.950, 1.000, 0.100, 1.000);
            AndersonTabla_InitialConditions.Rows.Add(0.1, 5.312, 0.969, 0.207, 0.977);
            AndersonTabla_InitialConditions.Rows.Add(0.2, 4.718, 0.937, 0.311, 0.954);
            AndersonTabla_InitialConditions.Rows.Add(0.3, 4.168, 0.906, 0.412, 0.931);
            AndersonTabla_InitialConditions.Rows.Add(0.4, 3.662, 0.874, 0.511, 0.907);
            AndersonTabla_InitialConditions.Rows.Add(0.5, 3.200, 0.843, 0.607, 0.884);
            AndersonTabla_InitialConditions.Rows.Add(0.6, 2.782, 0.811, 0.700, 0.861);
            AndersonTabla_InitialConditions.Rows.Add(0.7, 2.408, 0.780, 0.790, 0.838);
            AndersonTabla_InitialConditions.Rows.Add(0.8, 2.078, 0.748, 0.877, 0.815);
            AndersonTabla_InitialConditions.Rows.Add(0.9, 1.792, 0.717, 0.962, 0.792);
            AndersonTabla_InitialConditions.Rows.Add(1.0, 1.550, 0.685, 1.043, 0.769);
            AndersonTabla_InitialConditions.Rows.Add(1.1, 1.352, 0.654, 1.122, 0.745);
            AndersonTabla_InitialConditions.Rows.Add(1.2, 1.198, 0.622, 1.197, 0.722);
            AndersonTabla_InitialConditions.Rows.Add(1.3, 1.088, 0.591, 1.268, 0.699);
            AndersonTabla_InitialConditions.Rows.Add(1.4, 1.022, 0.560, 1.337, 0.676);
            AndersonTabla_InitialConditions.Rows.Add(1.5, 1.000, 0.528, 1.402, 0.653);
            AndersonTabla_InitialConditions.Rows.Add(1.6, 1.022, 0.497, 1.463, 0.630);
            AndersonTabla_InitialConditions.Rows.Add(1.7, 1.088, 0.465, 1.521, 0.607);
            AndersonTabla_InitialConditions.Rows.Add(1.8, 1.198, 0.434, 1.575, 0.583);
            AndersonTabla_InitialConditions.Rows.Add(1.9, 1.352, 0.402, 1.625, 0.560);
            AndersonTabla_InitialConditions.Rows.Add(2.0, 1.550, 0.371, 1.671, 0.537);
            AndersonTabla_InitialConditions.Rows.Add(2.1, 1.792, 0.339, 1.713, 0.514);
            AndersonTabla_InitialConditions.Rows.Add(2.2, 2.078, 0.308, 1.750, 0.491);
            AndersonTabla_InitialConditions.Rows.Add(2.3, 2.408, 0.276, 1.783, 0.468);
            AndersonTabla_InitialConditions.Rows.Add(2.4, 2.782, 0.245, 1.811, 0.445);
            AndersonTabla_InitialConditions.Rows.Add(2.5, 3.200, 0.214, 1.834, 0.422);
            AndersonTabla_InitialConditions.Rows.Add(2.6, 3.662, 0.182, 1.852, 0.398);
            AndersonTabla_InitialConditions.Rows.Add(2.7, 4.168, 0.151, 1.864, 0.375);
            AndersonTabla_InitialConditions.Rows.Add(2.8, 4.718, 0.119, 1.870, 0.352);
            AndersonTabla_InitialConditions.Rows.Add(2.9, 5.312, 0.088, 1.870, 0.329);
            AndersonTabla_InitialConditions.Rows.Add(3.0, 5.950, 0.056, 1.864, 0.306);
        }

        private void CreateSimulationTables()
        {
            // FIRST STEP 
            NozzleTabla_FirstStep = this.SimularUnCiclo();

            // STEADY STATE 
            NozzleTabla_SteadyState = this.SimularHastaSteady();

            // INITIAL CONDITIONS 
            NozzleTabla_InitialConditions = this.nozzle.GetEstado(this.Ax);
        }

        public DataTable SimularHastaSteady()
        {
            Nozzle newnozzle = new Nozzle(this.nozzle);

            // hacer simulacion -------------------------- FALTA HACER !!!!!!!!!!!!!

            return newnozzle.GetEstado(this.Ax);
        }

        public DataTable SimularUnCiclo()
        {
            Nozzle newnozzle = new Nozzle(this.nozzle);

            newnozzle.EjecutarCiclo(this.At, this.Ax, 1.4);
            newnozzle.ActualizarEstados();

            return newnozzle.GetEstado(this.Ax);
        }
    }
}
