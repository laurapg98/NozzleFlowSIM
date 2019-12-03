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
                this.nozzle[pos] = nozzleC.GetRectangulo(pos);
                pos++;
            }
        }

        public Nozzle(int numrect, double Ax) // crea el nozzle con las condiciones iniciales y la altura
        {
            this.numRect = numrect;
            this.nozzle = new Rectangulo[this.numRect + 2];

            int i = 0;
            while (i <= this.numRect + 1)
            {
                // coord x
                double x = i * Ax;

                if (i == this.numRect + 1) // supersonic outflow boundary conditions --> extrapolation
                    this.ComputeOutflowBoundaryConditions(i);
                else if (i == 0)
                {
                    double dens = 1;
                    double temp = 1;
                    double vel = 0;
                    double pres = 1;
                    this.nozzle[i] = new Rectangulo(temp, vel, dens, pres);
                }
                else
                {
                    // asignamos las condiciones iniciales
                    double dens = 1 - (0.3146 * x);
                    double temp = 1 - (0.2314 * x);
                    double vel = (0.1 + (1.09 * x)) * Math.Sqrt(temp);
                    double pres = dens * temp;

                    this.nozzle[i] = new Rectangulo(temp, vel, dens, pres);
                }

                // asignamos la altura
                double A = 1 + (2.2 * (x - 1.5) * (x - 1.5)); // Equation: A(X) = 1 + 2.2(x - 1.5) ^ 2
                double h = 2 * Math.Sqrt(A / Math.PI); // area de la circumferencia

                this.nozzle[i].SetAltura(h);
                this.nozzle[i].SetArea(A);

                i++;
            }
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

        // GETTERS
        public int GetNumRects()
        {
            return this.numRect;
        }

        public Rectangulo GetRectangulo(int pos)
        {
            return this.nozzle[pos + 1];
        }

        // ALTRES MÈTODES
        public void EjecutarCiclo(double At, double Ax, double gamma) // calcula el estado futuro de todos los rectangulos
        {
            int pos = 1;
            while (pos <= this.numRect + 1)
            {
                if (pos == this.numRect + 1)
                    this.ComputeOutflowBoundaryConditions(pos);
                else
                    this.nozzle[pos].ComputeFutureState(At, Ax, gamma, this.nozzle[pos + 1]);

                pos++;
            }
        }

        public void ActualizarEstados() // actualiza los estados de todos los rectangulos de las toveras
        {
            int pos = 1;
            while (pos <= this.numRect)
            {
                this.nozzle[pos].ChangeState();
                pos++;
            }
        }

        public void GuardarEstadoFichero(string fichero, double Ax, double At, double gamma) // guarda el estado actual del nozzle en un txt
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
            this.nozzle = new Rectangulo[this.numRect];

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
            double[] parametres = new double[3];
            parametres[0] = Convert.ToDouble(R.ReadLine());
            parametres[1] = Convert.ToDouble(R.ReadLine());
            parametres[2] = Convert.ToDouble(R.ReadLine());

            return parametres;
        }

        public void ComputeOutflowBoundaryConditions(int i) // calcula los valores del rectángulo extra de la salida del dlujo 
        {
            double dens = (2 * this.nozzle[i - 1].GetDensP()) - this.nozzle[i - 2].GetDensP();
            double temp = (2 * this.nozzle[i - 1].GetTempP()) - this.nozzle[i - 2].GetTempP();
            double vel = (2 * this.nozzle[i - 1].GetVelP()) - this.nozzle[i - 2].GetVelP();
            double pres = dens * temp;
            this.nozzle[i] = new Rectangulo(temp, vel, dens, pres);
        }

        public DataTable GetEstado(double Ax) // devuelve una datatable con los datos del estado actual
        {
            DataTable estado = new DataTable();

            estado.Rows.Add("Position", "Area", "Density", "Velocity", "Temperature", "Pressure", "Mach number");

            int i = 0;
            while (i <= this.numRect + 1)
            {
                Rectangulo rect = this.nozzle[i];

                double position = i * Ax; // coord x
                double area = rect.GetArea();
                double density = rect.GetDensP();
                double velocity = rect.GetVelP();
                double temperature = rect.GetTempP();
                double pressure = rect.GetPresP();
                double mach = velocity / Math.Sqrt(temperature);

                estado.Rows.Add(position, area, density, velocity, temperature, pressure, mach);

                i++;
            }

            return estado;
        }

        public double getdt(double C, double dx)
        {
            double dt = 1.0;
            for(int i = 1; i <= this.numRect; i++)
            {
                dt = Math.Min(dt, (C * dx) / (Math.Sqrt(this.nozzle[i].GetTempP()) + this.nozzle[i].GetVelP()));
            }

            return dt;
        }
    }
}
