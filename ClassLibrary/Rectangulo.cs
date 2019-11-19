using System;
using System.Collections.Generic;
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

        public Rectangulo(double temp, double vel, double dens, double pres, double altura) // crea el estado presente de la celda, dado como parámetros
        {
            this.tempP = temp;
            this.velP = vel;
            this.densP = dens;
            this.presP = pres;
            this.altura = altura;
        }

        public Rectangulo(Rectangulo celda) // constructor copia (estado presente)
        {
            this.tempP = celda.GetTempP();
            this.velP = celda.GetVelP();
            this.densP = celda.GetDensP();
            this.presP = celda.GetPresP();
            this.altura = celda.GetAltura();
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
        public void ComputeFutureState(double At, double Ax, double gamma, Rectangulo rectD)
        {
            double AT = rectD.GetTempP() - this.tempP;
            double AV = rectD.GetVelP() - this.velP;
            double Adens = rectD.GetDensP() - this.densP;
            double relA = (Math.PI*Math.Pow(rectD.GetAltura()/2,2)) / (Math.PI * Math.Pow(this.altura / 2, 2));
            double K = At / Ax;

            this.tempF = this.tempP - (K * (this.velP * AT + (this.tempP * (gamma - 1) * (AV + (this.velP * Math.Log(relA))))));
            this.velF=this.velP-(K*((this.velP*AV)+(AT/gamma)+()))
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
