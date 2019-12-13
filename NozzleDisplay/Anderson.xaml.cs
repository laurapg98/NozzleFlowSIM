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
        DataTable ErrorDataTable_FirstStep;
        DataTable ErrorDataTable_SteadyState;
        DataTable ErrorDataTable_InitialConditions;


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
            ErrorDataTable_FirstStep = new DataTable();
            ErrorDataTable_SteadyState = new DataTable();
            ErrorDataTable_InitialConditions = new DataTable();
            this.createTables();

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
                    //ERRORS
                    errorgrid.ItemsSource = ErrorDataTable_FirstStep.DefaultView;
                    errorgrid.DataContext = ErrorDataTable_FirstStep.DefaultView;

                }
                if (andersoncombobox.SelectedIndex == 1) // STEADY STATE
                {
                    // ANDERSON
                    andersongrid.ItemsSource = AndersonTabla_SteadyState.DefaultView;
                    andersongrid.DataContext = AndersonTabla_SteadyState.DefaultView;
                    // SIMULATION
                    datasimgrid.ItemsSource = NozzleTabla_SteadyState.DefaultView;
                    datasimgrid.DataContext = NozzleTabla_SteadyState.DefaultView;
                    //ERRORS
                    errorgrid.ItemsSource = ErrorDataTable_SteadyState.DefaultView;
                    errorgrid.DataContext = ErrorDataTable_SteadyState.DefaultView;

                }
                if (andersoncombobox.SelectedIndex == 2) // INITIAL CONDITIONS
                {
                    // ANDERSON
                    andersongrid.ItemsSource = AndersonTabla_InitialConditions.DefaultView;
                    andersongrid.DataContext = AndersonTabla_InitialConditions.DefaultView;
                    // SIMULATION
                    datasimgrid.ItemsSource = NozzleTabla_InitialConditions.DefaultView;
                    datasimgrid.DataContext = NozzleTabla_InitialConditions.DefaultView;
                    //ERRORS
                    errorgrid.ItemsSource = ErrorDataTable_InitialConditions.DefaultView;
                    errorgrid.DataContext = ErrorDataTable_InitialConditions.DefaultView;
                }

                andersongrid.Items.Refresh();
            }
            catch
            {

            }
        }

        private void createTables()
        {
            // FIRST STEP - ANDERSON
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("x L"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("A A*"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("ρ ρo"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("V ao"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("T To"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("P Po"));
            AndersonTabla_FisrtStep.Columns.Add(new DataColumn("M"));
                                                                
            AndersonTabla_FisrtStep.Rows.Add(0.0, 5.950, 1.000, 0.111, 1.000, 1.000, 0.111);
            AndersonTabla_FisrtStep.Rows.Add(0.1, 5.312, 0.955, 0.212, 0.972, 0.928, 0.215);
            AndersonTabla_FisrtStep.Rows.Add(0.2, 4.718, 0.927, 0.312, 0.950, 0.881, 0.320);
            AndersonTabla_FisrtStep.Rows.Add(0.3, 4.168, 0.900, 0.411, 0.929, 0.836, 0.427);
            AndersonTabla_FisrtStep.Rows.Add(0.4, 3.662, 0.872, 0.508, 0.908, 0.791, 0.534);
            AndersonTabla_FisrtStep.Rows.Add(0.5, 3.200, 0.844, 0.603, 0.886, 0.748, 0.640);
            AndersonTabla_FisrtStep.Rows.Add(0.6, 2.782, 0.817, 0.695, 0.865, 0.706, 0.747);
            AndersonTabla_FisrtStep.Rows.Add(0.7, 2.408, 0.789, 0.784, 0.843, 0.665, 0.854);
            AndersonTabla_FisrtStep.Rows.Add(0.8, 2.078, 0.760, 0.870, 0.822, 0.625, 0.960);
            AndersonTabla_FisrtStep.Rows.Add(0.9, 1.792, 0.731, 0.954, 0.800, 0.585, 1.067);
            AndersonTabla_FisrtStep.Rows.Add(1.0, 1.550, 0.701, 1.035, 0.778, 0.545, 1.174);
            AndersonTabla_FisrtStep.Rows.Add(1.1, 1.352, 0.670, 1.113, 0.755, 0.506, 1.281);
            AndersonTabla_FisrtStep.Rows.Add(1.2, 1.198, 0.637, 1.188, 0.731, 0.466, 1.389);
            AndersonTabla_FisrtStep.Rows.Add(1.3, 1.088, 0.603, 1.260, 0.707, 0.426, 1.498);
            AndersonTabla_FisrtStep.Rows.Add(1.4, 1.022, 0.567, 1.328, 0.682, 0.387, 1.609);
            AndersonTabla_FisrtStep.Rows.Add(1.5, 1.000, 0.531, 1.394, 0.656, 0.349, 1.720);
            AndersonTabla_FisrtStep.Rows.Add(1.6, 1.022, 0.494, 1.455, 0.631, 0.312, 1.833);
            AndersonTabla_FisrtStep.Rows.Add(1.7, 1.088, 0.459, 1.514, 0.605, 0.278, 1.945);
            AndersonTabla_FisrtStep.Rows.Add(1.8, 1.198, 0.425, 1.568, 0.581, 0.247, 2.058);
            AndersonTabla_FisrtStep.Rows.Add(1.9, 1.352, 0.392, 1.619, 0.556, 0.218, 2.171);
            AndersonTabla_FisrtStep.Rows.Add(2.0, 1.550, 0.361, 1.666, 0.533, 0.192, 2.282);
            AndersonTabla_FisrtStep.Rows.Add(2.1, 1.792, 0.330, 1.709, 0.510, 0.168, 2.393);
            AndersonTabla_FisrtStep.Rows.Add(2.2, 2.078, 0.301, 1.748, 0.487, 0.146, 2.504);
            AndersonTabla_FisrtStep.Rows.Add(2.3, 2.408, 0.271, 1.782, 0.465, 0.126, 2.614);
            AndersonTabla_FisrtStep.Rows.Add(2.4, 2.782, 0.242, 1.813, 0.443, 0.107, 2.724);
            AndersonTabla_FisrtStep.Rows.Add(2.5, 3.200, 0.213, 1.838, 0.421, 0.090, 2.834);
            AndersonTabla_FisrtStep.Rows.Add(2.6, 3.662, 0.184, 1.858, 0.398, 0.073, 2.944);
            AndersonTabla_FisrtStep.Rows.Add(2.7, 4.168, 0.154, 1.874, 0.376, 0.058, 3.055);
            AndersonTabla_FisrtStep.Rows.Add(2.8, 4.718, 0.125, 1.884, 0.354, 0.044, 3.167);
            AndersonTabla_FisrtStep.Rows.Add(2.9, 5.312, 0.095, 1.890, 0.332, 0.032, 3.281);
            AndersonTabla_FisrtStep.Rows.Add(3.0, 5.950, 0.066, 1.895, 0.309, 0.020, 3.406);


            // STEADY STATE - ANDERSON                                                
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("x L"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("A A*"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("ρ ρo"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("V ao"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("T To"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("P Po"));
            AndersonTabla_SteadyState.Columns.Add(new DataColumn("M"));

            AndersonTabla_SteadyState.Rows.Add(0.0, 5.950, 1.000, 0.099, 1.000, 1.000, 0.099);
            AndersonTabla_SteadyState.Rows.Add(0.1, 5.312, 0.998, 0.112, 0.999, 0.997, 0.112);
            AndersonTabla_SteadyState.Rows.Add(0.2, 4.718, 0.997, 0.125, 0.999, 0.996, 0.125);
            AndersonTabla_SteadyState.Rows.Add(0.3, 4.168, 0.994, 0.143, 0.998, 0.992, 0.143);
            AndersonTabla_SteadyState.Rows.Add(0.4, 3.662, 0.992, 0.162, 0.997, 0.988, 0.163);
            AndersonTabla_SteadyState.Rows.Add(0.5, 3.200, 0.987, 0.187, 0.995, 0.982, 0.187);
            AndersonTabla_SteadyState.Rows.Add(0.6, 2.782, 0.982, 0.215, 0.993, 0.974, 0.216);
            AndersonTabla_SteadyState.Rows.Add(0.7, 2.408, 0.974, 0.251, 0.989, 0.963, 0.252);
            AndersonTabla_SteadyState.Rows.Add(0.8, 2.078, 0.963, 0.294, 0.985, 0.948, 0.296);
            AndersonTabla_SteadyState.Rows.Add(0.9, 1.792, 0.947, 0.346, 0.978, 0.926, 0.350);
            AndersonTabla_SteadyState.Rows.Add(1.0, 1.550, 0.924, 0.409, 0.969, 0.895, 0.416);
            AndersonTabla_SteadyState.Rows.Add(1.1, 1.352, 0.892, 0.485, 0.956, 0.853, 0.496);
            AndersonTabla_SteadyState.Rows.Add(1.2, 1.198, 0.849, 0.575, 0.937, 0.795, 0.594);
            AndersonTabla_SteadyState.Rows.Add(1.3, 1.088, 0.792, 0.678, 0.911, 0.722, 0.710);
            AndersonTabla_SteadyState.Rows.Add(1.4, 1.022, 0.721, 0.793, 0.878, 0.633, 0.846);
            AndersonTabla_SteadyState.Rows.Add(1.5, 1.000, 0.639, 0.914, 0.836, 0.534, 0.099);
            AndersonTabla_SteadyState.Rows.Add(1.6, 1.022, 0.551, 1.037, 0.789, 0.434, 1.167);
            AndersonTabla_SteadyState.Rows.Add(1.7, 1.088, 0.465, 1.155, 0.737, 0.343, 1.345);
            AndersonTabla_SteadyState.Rows.Add(1.8, 1.198, 0.386, 1.263, 0.684, 0.264, 1.528);
            AndersonTabla_SteadyState.Rows.Add(1.9, 1.352, 0.318, 1.361, 0.633, 0.201, 1.710);
            AndersonTabla_SteadyState.Rows.Add(2.0, 1.550, 0.262, 1.446, 0.585, 0.153, 1.890);
            AndersonTabla_SteadyState.Rows.Add(2.1, 1.792, 0.216, 1.519, 0.541, 0.117, 2.065);
            AndersonTabla_SteadyState.Rows.Add(2.2, 2.078, 0.179, 1.582, 0.502, 0.090, 2.233);
            AndersonTabla_SteadyState.Rows.Add(2.3, 2.408, 0.150, 1.636, 0.467, 0.070, 2.394);
            AndersonTabla_SteadyState.Rows.Add(2.4, 2.782, 0.126, 1.683, 0.436, 0.055, 2.549);
            AndersonTabla_SteadyState.Rows.Add(2.5, 3.200, 0.107, 1.723, 0.408, 0.044, 2.696);
            AndersonTabla_SteadyState.Rows.Add(2.6, 3.662, 0.092, 1.759, 0.384, 0.035, 2.839);
            AndersonTabla_SteadyState.Rows.Add(2.7, 4.168, 0.079, 1.789, 0.362, 0.029, 2.972);
            AndersonTabla_SteadyState.Rows.Add(2.8, 4.718, 0.069, 1.817, 0.342, 0.024, 3.105);
            AndersonTabla_SteadyState.Rows.Add(2.9, 5.312, 0.061, 1.839, 0.325, 0.020, 3.225);
            AndersonTabla_SteadyState.Rows.Add(3.0, 5.950, 0.053, 1.862, 0.308, 0.016, 3.353);


            // INITIAL CONDITIONS - ANDERSON
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

            // FIRST STEP - SIMULATION
            NozzleTabla_FirstStep = this.SimularUnCiclo();

            // STEADY STATE - SIMULATION
            NozzleTabla_SteadyState = this.SimularHastaSteady();

            // INITIAL CONDITIONS - SIMULATION
            NozzleTabla_InitialConditions = this.nozzle.GetEstado(this.Ax);


            //TABLAS ERRORES
            ErrorDataTable_FirstStep = getErrorTables(AndersonTabla_FisrtStep, NozzleTabla_FirstStep);
            ErrorDataTable_InitialConditions = getErrorTables(AndersonTabla_InitialConditions, NozzleTabla_InitialConditions);
            ErrorDataTable_SteadyState = getErrorTables(AndersonTabla_SteadyState, NozzleTabla_SteadyState);

        }

        public DataTable SimularHastaSteady() // simula 1400 ciclos y da la tabla con los resultados
        {
            Nozzle newnozzle = new Nozzle(this.nozzle);

            int step = 0;
            while (step <= 1400)
            {
                newnozzle.EjecutarCiclo(this.At, this.Ax, 1.4);
                newnozzle.ActualizarEstados();

                step++;
            }

            return newnozzle.GetEstado(this.Ax);
        }

        public DataTable SimularUnCiclo() // simula un único ciclo y da la tabla con los resultados
        {
            Nozzle newnozzle = new Nozzle(this.nozzle);

            newnozzle.EjecutarCiclo(this.At, this.Ax, 1.4);
            newnozzle.ActualizarEstados();

            return newnozzle.GetEstado(this.Ax);
        }

        
        public DataTable getErrorTables(DataTable Anderson, DataTable Simulated)
        {
            DataTable et = new DataTable();
            et.Columns.Add(new DataColumn("x L"));
            et.Columns.Add(new DataColumn("e (%)"));
            for (int i = 0; i < Simulated.Rows.Count; i++)
            {
                double error_average;
                try
                {
                    double error_pressure = ((Convert.ToDouble(Anderson.Rows[i][5]) - Convert.ToDouble(Simulated.Rows[i][5])) / (Convert.ToDouble(Anderson.Rows[i][5]))) * 100;
                    double error_density = ((Convert.ToDouble(Anderson.Rows[i][2]) - Convert.ToDouble(Simulated.Rows[i][2])) / (Convert.ToDouble(Anderson.Rows[i][2]))) * 100;
                    double error_velocity = ((Convert.ToDouble(Anderson.Rows[i][3]) - Convert.ToDouble(Simulated.Rows[i][3])) / (Convert.ToDouble(Anderson.Rows[i][3]))) * 100;
                    double error_temperature = ((Convert.ToDouble(Anderson.Rows[i][4]) - Convert.ToDouble(Simulated.Rows[i][4])) / (Convert.ToDouble(Anderson.Rows[i][4]))) * 100;

                    error_average = (error_pressure + error_density + error_velocity + error_temperature) / 4;
                }

                catch
                {
                    double error_density = ((Convert.ToDouble(Anderson.Rows[i][2]) - Convert.ToDouble(Simulated.Rows[i][2])) / (Convert.ToDouble(Anderson.Rows[i][2]))) * 100;
                    double error_velocity = ((Convert.ToDouble(Anderson.Rows[i][3]) - Convert.ToDouble(Simulated.Rows[i][3])) / (Convert.ToDouble(Anderson.Rows[i][3]))) * 100;
                    double error_temperature = ((Convert.ToDouble(Anderson.Rows[i][4]) - Convert.ToDouble(Simulated.Rows[i][4])) / (Convert.ToDouble(Anderson.Rows[i][4]))) * 100;

                    error_average = (error_density + error_velocity + error_temperature) / 3;
                }

                et.Rows.Add(Simulated.Rows[i][0], error_average.ToString("0.000"));
            }

            return et;
        }
        

        

    }
}
