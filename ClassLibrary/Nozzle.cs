using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ClassLibrary
{
    public class Nozzle
    {
        // ATRIBUTS
        int numRect;
        Rectangulo[] nozzle;
        double k; // nozzle position

        // CONSTRUCTORS
        public Nozzle() // constructor vacío
        {

        }

        public Nozzle(Nozzle nozzleC) // constructor copia
        {
            this.numRect = nozzleC.GetNumRects();
            this.nozzle = new Rectangulo[this.numRect + 2];

            int pos = 0;
            while (pos <= this.numRect + 1)
            {
                this.nozzle[pos] = new Rectangulo(nozzleC.GetRectangulo(pos));
                pos++;
            }
        }

        public Nozzle(int numrect, double Ax) // crea el nozzle con las condiciones iniciales y la altura
        {
            this.numRect = numrect;
            this.nozzle = new Rectangulo[this.numRect + 2];

            double dens;
            double temp;
            double vel;
            double pres;
            int i = 0;
            while (i <= this.numRect + 1)
            {
                // coord x
                double x = i * Ax;

                if (i == this.numRect + 1) // supersonic outflow boundary conditions --> extrapolation
                {
                    dens = (2 * this.nozzle[i - 1].GetDensP()) - this.nozzle[i - 2].GetDensP();
                    temp = (2 * this.nozzle[i - 1].GetTempP()) - this.nozzle[i - 2].GetTempP();
                    vel = (2 * this.nozzle[i - 1].GetVelP()) - this.nozzle[i - 2].GetVelP();
                    pres = dens * temp;
                }
                else if (i == 0)
                {
                    dens = 1;
                    temp = 1;
                    vel = 0;
                    pres = 1;
                }
                else 
                {
                    dens = 1 - (0.3146 * x);
                    temp = 1 - (0.2314 * x);
                    vel = (0.1 + (1.09 * x)) * Math.Sqrt(temp);
                    pres = dens * temp;
                }

                // creamos el rectangulo
                this.nozzle[i] = new Rectangulo(temp, vel, dens, pres);

                // asignamos la altura
                double A = 1 + (2.2 * (x - 1.5) * (x - 1.5)); // Equation: A(X) = 1 + 2.2(x - 1.5) ^ 2
                double h = 2 * Math.Sqrt(A / Math.PI); // area de la circumferencia

                this.nozzle[i].SetAltura(h);
                this.nozzle[i].SetArea(A);

                i++;
            }
            this.nozzle[0].SetVelP((2 * this.nozzle[1].GetVelP()) - this.nozzle[2].GetVelP()); // inflow boundary conditions --> extrapolation

        }

        //Constructor sobrecargado
        public Nozzle(int numrect, double Ax, double K) // crea el nozzle con las condiciones iniciales y la altura + la posicion nueva del cuello 
        {
            this.k = K;
            this.numRect = numrect;
            this.nozzle = new Rectangulo[this.numRect + 2];

            double dens;
            double temp;
            double vel;
            double pres;
            int i = 0;
            while (i <= this.numRect + 1)
            {
                // coord x
                double x = i * Ax;

                if (i == this.numRect + 1) // supersonic outflow boundary conditions --> extrapolation
                {
                    dens = (2 * this.nozzle[i - 1].GetDensP()) - this.nozzle[i - 2].GetDensP();
                    temp = (2 * this.nozzle[i - 1].GetTempP()) - this.nozzle[i - 2].GetTempP();
                    vel = (2 * this.nozzle[i - 1].GetVelP()) - this.nozzle[i - 2].GetVelP();
                    pres = dens * temp;
                }
                else if (i == 0)
                {
                    dens = 1;
                    temp = 1;
                    vel = 0;
                    pres = 1;
                }
                else
                {
                    dens = 1 - (0.3146 * x);
                    temp = 1 - (0.2314 * x);
                    vel = (0.1 + (1.09 * x)) * Math.Sqrt(temp);
                    pres = dens * temp;
                }

                // creamos el rectangulo
                this.nozzle[i] = new Rectangulo(temp, vel, dens, pres);

                // asignamos la altura
                double A = 1 + (2.2 * (x - this.k) * (x - this.k)); // Equation: A(X) = 1 + 2.2(x - 1.5) ^ 2
                double h = 2 * Math.Sqrt(A / Math.PI); // area de la circumferencia

                this.nozzle[i].SetAltura(h);
                this.nozzle[i].SetArea(A);

                i++;
            }
            this.nozzle[0].SetVelP((2 * this.nozzle[1].GetVelP()) - this.nozzle[2].GetVelP()); // inflow boundary conditions --> extrapolation

        }

        // SETTERS
        public void SetNumRects(int nr)
        {
            this.numRect = nr;
        }

        public void SetRectangulo(int pos, Rectangulo rect)
        {
            this.nozzle[pos] = rect;
        }

        public void setK(double k)
        {
            this.k = k;
        }

        // GETTERS
        public int GetNumRects()
        {
            return this.numRect;
        }

        public Rectangulo GetRectangulo(int pos)
        {
            return this.nozzle[pos];
        }

        public Rectangulo[] GetNozzle()
        {
            return this.nozzle;
        }

        public double getK()
        {
            return this.k;
        }

        // ALTRES MÈTODES
        public void EjecutarCiclo(double At, double Ax, double gamma) // calcula el estado futuro de todos los rectangulos
        {
            // predicted
            double[,] derivatives_I = new double[this.numRect, 3];
            int pos = 1;
            while (pos < this.numRect + 1)
            {
                double[] ders=this.nozzle[pos].ComputePredictedFutureState(At, Ax, gamma, this.nozzle[pos + 1]);

                derivatives_I[pos - 1, 0] = ders[0];
                derivatives_I[pos - 1, 1] = ders[1];
                derivatives_I[pos - 1, 2] = ders[2];

                pos++;
            }
            this.nozzle[0].SetVelF(this.nozzle[0].GetVelP());
            this.nozzle[0].SetVelF(this.nozzle[0].GetVelP());
            this.nozzle[0].SetTempF(this.nozzle[0].GetTempP());
            this.nozzle[0].SetDensF(this.nozzle[0].GetDensP());

            // final
            pos = 1;
            while (pos <= this.numRect + 1)
            {
                if (pos == this.numRect + 1)
                    this.ComputeOutflowBoundaryConditions(pos); // outflow boundary conditions --> extrapolation
                else
                {
                    double[] ders = new double[3];
                    ders[0] = derivatives_I[pos - 1, 0];
                    ders[1] = derivatives_I[pos - 1, 1];
                    ders[2] = derivatives_I[pos - 1, 2];

                    this.nozzle[pos].ComputeFutureState(At, Ax, gamma, ders, this.nozzle[pos - 1]);
                }

                pos++;
            }
            this.nozzle[0].SetVelF((2 * this.nozzle[1].GetVelF()) - this.nozzle[2].GetVelF()); // inflow boundary conditions --> extrapolation
        }

        public void ActualizarEstados() // actualiza los estados de todos los rectangulos de las toveras
        {
            int pos = 1;
            while (pos <= this.numRect + 1)
            {
                this.nozzle[pos].ChangeState();
                pos++;
            }
        }

        public void GuardarEstadoFichero(string fichero, double Ax, double At, double gamma, int contadort) // guarda el estado actual del nozzle en un txt
        {
            //obrim fitxer
            StreamWriter W = new StreamWriter(fichero + ".txt");

            //afegim numero de rectangles
            W.WriteLine(Convert.ToString(this.numRect));

            //guardem les dades de cada rectangle en strings
            int i = 0;
            while (i <= this.numRect + 1)
            {
                Rectangulo rect = this.nozzle[i];

                //recollim dades de cada rectangle, separades per ';'
                string dades = Convert.ToString(rect.GetTempP()) + ";" + Convert.ToString(rect.GetVelP()) + ";" + Convert.ToString(rect.GetDensP()) + ";" + Convert.ToString(rect.GetPresP()) + ";" + Convert.ToString(rect.GetAltura()) + ";" + Convert.ToString(rect.GetArea());

                //escribim una linea x cada rectangle
                W.WriteLine(dades);

                i++;
            }

            //afegim la resta de paràmetres
            W.WriteLine(Convert.ToString(Ax));
            W.WriteLine(Convert.ToString(At));
            W.WriteLine(Convert.ToString(gamma));
            W.WriteLine(Convert.ToString(contadort));

            //tanquem fitxer
            W.Close();
        }

        public double[] CargarEstadoFichero(string fichero) // carga como estado actual del nozzle los datos que lee de un fichero txt
        {
            //llegim fitxer
            StreamReader R = new StreamReader(fichero);

            //agafem número de rectagles
            this.numRect = Convert.ToInt32(R.ReadLine());

            //formem el vector de rectangles 
            this.nozzle = new Rectangulo[this.numRect + 2];

            //agafem les propietats del fluid a cada rectangle
            int i = 0;
            while (i <= this.numRect + 1)
            {
                string linea = R.ReadLine();
                string[] trozos = linea.Split(';');

                double temp = Convert.ToDouble(trozos[0]);
                double vel = Convert.ToDouble(trozos[1]);
                double dens = Convert.ToDouble(trozos[2]);
                double pres = Convert.ToDouble(trozos[3]);
                double alt = Convert.ToDouble(trozos[4]);
                double area = Convert.ToDouble(trozos[5]);

                Rectangulo rect = new Rectangulo(temp, vel, dens, pres, alt, area);

                this.nozzle[i] = rect;

                i++;
            }

            //agafem els altres paràmetres
            double[] parametres = new double[4];
            parametres[0] = Convert.ToDouble(R.ReadLine()); // Ax
            parametres[1] = Convert.ToDouble(R.ReadLine()); // At
            parametres[2] = Convert.ToDouble(R.ReadLine()); // gamma
            parametres[3] = Convert.ToDouble(R.ReadLine()); // numero de intervalos de tiempo que han pasado

            return parametres;
        }

        public void ComputeOutflowBoundaryConditions(int i) // calcula los valores del rectángulo extra de la salida del flujo 
        {
            this.nozzle[i].SetDensF((2 * this.nozzle[i - 1].GetDensP()) - this.nozzle[i - 2].GetDensP());
            this.nozzle[i].SetTempF((2 * this.nozzle[i - 1].GetTempP()) - this.nozzle[i - 2].GetTempP());
            this.nozzle[i].SetVelF((2 * this.nozzle[i - 1].GetVelP()) - this.nozzle[i - 2].GetVelP());
            this.nozzle[i].SetPresF(this.nozzle[i].GetDensF() * this.nozzle[i].GetTempF());
        }

        public DataTable GetEstado(double Ax) // devuelve una datatable con los datos del estado actual
        {
            DataTable estado = new DataTable();

            estado.Columns.Add(new DataColumn("x L"));
            estado.Columns.Add(new DataColumn("A A*"));
            estado.Columns.Add(new DataColumn("ρ ρo"));
            estado.Columns.Add(new DataColumn("V ao"));
            estado.Columns.Add(new DataColumn("T To"));
            estado.Columns.Add(new DataColumn("P Po"));
            estado.Columns.Add(new DataColumn("M"));

            int i = 0;
            while (i <= this.numRect)
            {
                Rectangulo rect = this.nozzle[i];

                String position = (i * Ax).ToString(); // coord x
                String area = rect.GetArea().ToString("0.0000");
                String density = rect.GetDensP().ToString("0.0000");
                String velocity = rect.GetVelP().ToString("0.0000");
                String temperature = rect.GetTempP().ToString("0.0000");
                String pressure = rect.GetPresP().ToString("0.0000");
                String mach = (rect.GetVelP() / Math.Sqrt(rect.GetTempP())).ToString("0.0000"); // M = V / a

                estado.Rows.Add(position, area, density, velocity, temperature, pressure, mach);

                i++;
            }

            return estado;
        }

        public double getdt(double C, double dx) 
        {
            double dt = 1.0;
            for (int i = 1; i < this.numRect; i++)
            {
                dt = Math.Min(dt, (C * dx) / (Math.Sqrt(this.nozzle[i].GetTempP()) + this.nozzle[i].GetVelP()));
            }
            
            return dt;
        }

        public List<double> getPressures()
        {
            List<double> pressures = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                pressures.Add(this.nozzle[i].GetPresP());
            }

            return pressures;
        }

        public List<double> getVelocities()
        {
            List<double> velocities = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                velocities.Add(this.nozzle[i].GetVelP());
            }

            return velocities;
        }

        public List<double> getDensities()
        {
            List<double> densities = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                densities.Add(this.nozzle[i].GetDensP());
            }

            return densities;
        }

        public List<double> getTemperatures()
        {
            List<double> temperatures = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                temperatures.Add(this.nozzle[i].GetTempP());
            }

            return temperatures;
        }

        public List<double> getNozzleXL(double dx)
        {
            List<double> dxs = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                dxs.Add(dx * (i));
            }

            return dxs;
        }

        public List<double> getNozzleArea()
        {
            List<double> As = new List<double>();
            
            for(int i = 0; i <= this.numRect; i++)
            {
                As.Add(this.nozzle[i].GetArea());
            }

            return As;
        }

        public List<double> getMassFlow()
        {
            List<double> mf = new List<double>();

            for (int i = 0; i <= this.numRect; i++)
            {
                mf.Add(this.nozzle[i].GetArea()* this.nozzle[i].GetVelP()* this.nozzle[i].GetDensP());
            }

            return mf;
        }

        public bool SimulacionAcabada()
        {
            return false;
        }

        public int getthroatpos()
        {
            int pt = 0;
            foreach(Rectangulo rect in nozzle)
            {
                if (rect.GetArea() == 1.0)
                {
                    return pt;
                }
                pt++;
            }
            return 0;
        }



        

    }
}
