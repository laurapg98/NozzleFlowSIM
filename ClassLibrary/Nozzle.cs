using System;
using System.Collections.Generic;
using System.Text;

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
            double x = 0;
            while(x<=3)
            {
                double A = 1 + (2.2 * Math.Pow(x - 1.5, 2));
                double h = Math.Pow(4 * A / Math.PI, 0.5);

                x = x + Ax;
            }
        }

    }
}
