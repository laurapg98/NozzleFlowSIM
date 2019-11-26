using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClassLibrary
{
    public class Rectangulo
    {
            // ATRIBUTS
        double tempP;
        double tempF;
        double velP;
        double velF;
        double densP;
        double densF;
        double presP;
        double presF;
        double altura;

            // CONSTRUCTORS
        public Rectangulo() // constructor vacío
        {

        }

        public Rectangulo(double temp, double vel, double dens, double pres, double altura) // crea el estado presente con la altura
        {
            this.tempP = temp;
            this.velP = vel;
            this.densP = dens;
            this.presP = pres;
            this.altura = altura;
        }

        public Rectangulo(double temp, double vel, double dens, double pres) // crea el estado presente sin la altura
        {
            this.tempP = temp;
            this.velP = vel;
            this.densP = dens;
            this.presP = pres;
        }

        public Rectangulo(Rectangulo rectC) // constructor copia (estado presente)
        {
            this.tempP = rectC.GetTempP();
            this.velP = rectC.GetVelP();
            this.densP = rectC.GetDensP();
            this.presP = rectC.GetPresP();
            this.altura = rectC.GetAltura();
        }

            // GETTERS
        public double GetTempP()
        {
            return this.tempP;
        }

        public double GetTempF()
        {
            return this.tempF;
        }

        public double GetVelP()
        {
            return this.velP;
        }

        public double GetVelF()
        {
            return this.velF;
        }

        public double GetDensP()
        {
            return this.densP;
        }

        public double GetDensF()
        {
            return this.densF;
        }

        public double GetPresP()
        {
            return this.presP;
        }

        public double GetPresF()
        {
            return this.presF;
        }

        public double GetAltura()
        {
            return this.altura;
        }

            // SETTERS
        public void SetTempP(double tempP)
        {
            this.tempP = tempP;
        }

        public void SetTempF(double tempF)
        {
            this.tempF = tempF;
        }

        public void SetVelP(double velP)
        {
            this.velP = velP;
        }

        public void SetVelF(double velF)
        {
            this.velF = velF;
        }

        public void SetDensP(double densP)
        {
            this.densP = densP;
        }

        public void SetDensF(double densF)
        {
            this.densF = densF;
        }

        public void SetPresP(double presP)
        {
            this.presP = presP;
        }

        public void SetPresF(double presF)
        {
            this.presF = presF;
        }

        public void SetAltura(double alt)
        {
            this.altura = alt;
        }

            // ALTRES MÈTODES
        public void ComputeFutureState(double At, double Ax, double gamma, Rectangulo rectD) // utiliza las eqs discretizadas para calcular el estado futuro de la celda, necesita los parámetros de discretización (incrementos de t y x), el parámetro del fluido (gamma) y el estado presente de la célula adyacente)
        {
            double AT = rectD.GetTempP() - this.tempP;
            double AV = rectD.GetVelP() - this.velP;
            double Adens = rectD.GetDensP() - this.densP;
            double relA = (Math.PI*Math.Pow(rectD.GetAltura()/2,2)) / (Math.PI * Math.Pow(this.altura / 2, 2));
            double K = At / Ax;

            this.tempF = this.tempP - (K * (this.velP * AT + (this.tempP * (gamma - 1) * (AV + (this.velP * Math.Log(relA))))));
            this.velF = this.velP - (K * ((this.velP * AV) + (AT / gamma) + ((this.tempP / (gamma * this.densP)) * Adens)));
            this.densF = this.densP - (K * ((this.densP * AV) + (this.densP * this.velP * Math.Log(relA)) + (this.velP * Adens)));
            this.presF = this.densF * this.tempF;
        }

        public void ChangeState() // acutaliza el estado: el estado presente pasa a ser el futuro
        {
            this.tempP = this.tempF;
            this.velP = this.velF;
            this.densP = this.densF;
            this.presP = this.presF;
        }

    }
}
