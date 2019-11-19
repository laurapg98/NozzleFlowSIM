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
        Rectangulo[,] nozzle;

            // CONSTRUCTORS
        public Nozzle() // constructor vacío
        {
            
        }

        public Nozzle(Nozzle nozzleC) // constructor copia
        {
            this.numRect = nozzleC.GetNumRects();
            this.nozzle = new Rectangulo[1, this.numRect + 2];

            int pos = 0;
            while (pos <= this.numRect + 2)
            {
                this.nozzle[1, pos] = nozzleC.GetRectangulo(pos);
                pos++;
            }
        }

        public Nozzle(int numrect, double temp, double vel, double dens, double pres) // crea el nozzle con las condiciones iniciales que se dan como input - no asigna altura
        {
            this.numRect = numrect;
            this.nozzle = new Rectangulo[1, this.numRect + 2];

            int pos = 0;
            while (pos <= this.numRect + 2)
            {
                this.nozzle[1, pos] = new Rectangulo(temp, vel, dens, pres);
                pos++;
            }
        }

            // SETTERS
        public void SetNumRects(int nr)
        {
            this.numRect = nr;
        }

        public void SetRectangulo(int pos, Rectangulo rect)
        {
            this.nozzle[1, pos] = rect;
        }

            // GETTERS
        public int GetNumRects()
        {
            return this.numRect;
        }

        public Rectangulo GetRectangulo(int pos)
        {
            return this.nozzle[1, pos];
        }

            // ALTRES MÈTODES
        public void EjecutarCiclo(double At, double Ax, double gamma) // calcula el estado futuro de todos los rectangulos
        {
            int pos = 1;
            while (pos <= this.numRect)
            {
                this.nozzle[1, pos].ComputeFutureState(At, Ax, gamma, this.nozzle[1, pos + 1]);
                pos++;
            }
        }

        public void ActualizarEstados() // actualiza los estados de todos los rectangulos de las toveras
        {
            int pos = 1;
            while (pos <= this.numRect)
            {
                this.nozzle[1, pos].ChangeState();
                pos++;
            }
        }

        public void AsignarAlturas(double Ax) // asigna las alturas a cada rectángulo
        {
            // Equation: A(X)=1+2.2(x-1.5)^2
            int i = 1;
            while (i <= this.numRect)
            {
                // coord x
                double x = i * Ax;

                // area
                double A = 1 + (2.2 * Math.Pow(x - 1.5, 2));

                // altura
                double h = Math.Pow(4 * A / Math.PI, 0.5);

                // asignamos la altura
                this.nozzle[1, i].SetAltura(h);

                i++;
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
            while (i <= this.numRect + 2)
            {
                Rectangulo rect = this.nozzle[1, i];

                //recollim dades de cada rectangle, separades per ';'
                string dades = Convert.ToString(rect.GetTempP()) + ";" + Convert.ToString(rect.GetVelP()) + ";" + Convert.ToString(rect.GetDensP()) + ";" + Convert.ToString(rect.GetPresP()) + ";" + rect.GetAltura();

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

        public double[,] CargarEstadoFichero(string fichero)
        {
            //llegim fitxer
            StreamReader R = new StreamReader(fichero);

            //agafem número de rectagles
            this.numRect = Convert.ToInt32(R.ReadLine());

            //formem el vector de rectangles 
            this.nozzle = new Rectangulo[1, this.numRect];

            //agafem les propietats del fluid a cada rectangle
            int i = 0;
            while (i <= this.numRect + 2)
            {
                string linea = R.ReadLine();
                string[] trozos = linea.Split(';');

                double temp = Convert.ToDouble(trozos[0]);
                double vel = Convert.ToDouble(trozos[1]);
                double dens = Convert.ToDouble(trozos[2]);
                double pres = Convert.ToDouble(trozos[3]);
                double alt = Convert.ToDouble(trozos[4]);

                Rectangulo rect = new Rectangulo(temp, vel, dens, pres, alt);

                this.nozzle[1, i] = rect;

                i++;
            }

            //agafem els altres paràmetres
            double[,] parametres = new double[1, 3];
            parametres[1, 0] = Convert.ToDouble(R.ReadLine());
            parametres[1, 1] = Convert.ToDouble(R.ReadLine());
            parametres[1, 2] = Convert.ToDouble(R.ReadLine());

            return parametres;
        }
    }
}
