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
        double area;

            // CONSTRUCTORS
        public Rectangulo() // constructor vacío
        {

        }

        public Rectangulo(double temp, double vel, double dens, double pres, double altura, double area) // crea el estado presente con la altura
        {
            this.tempP = temp;
            this.velP = vel;
            this.densP = dens;
            this.presP = pres;
            this.altura = altura;
            this.area = area;
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
            this.area = rectC.GetArea();
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

        public double GetArea()
        {
            return this.area;
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

        public void SetArea(double area)
        {
            this.area = area;
        }

            // ALTRES MÈTODES
        public void ComputePredictedFutureState(double At, double Ax, double gamma, Rectangulo rectD) // MACCORMACK'S TECHNIQUE: utiliza las eqs discretizadas para calcular el predicted state de la celda --> lo guardamos todo en futuro 
        {
            // constant
            double K = At / Ax;

            // relations
            double AT = rectD.GetTempP() - this.tempP;
            double AV = rectD.GetVelP() - this.velP;
            double Adens = rectD.GetDensP() - this.densP;
            double relA = rectD.GetArea() / this.area;

            // equations --> predicted state
            this.tempF = this.tempP - (K * (this.velP * AT + (this.tempP * (gamma - 1) * (AV + (this.velP * Math.Log(relA))))));
            this.velF = this.velP - (K * ((this.velP * AV) + (AT / gamma) + ((this.tempP / (gamma * this.densP)) * Adens)));
            this.densF = this.densP - (K * ((this.densP * AV) + (this.densP * this.velP * Math.Log(relA)) + (this.velP * Adens)));
        }

        public void ComputeFutureState(double At, double Ax, double gamma, Rectangulo rectD) // PREDICTED-CORRECTION TECHNIQUE: corrije el estado predicted (que está en futuro) usando también el presente (que está en presente)
        {
            // constant
            double K = At / Ax;
            double relA = rectD.GetArea() / this.area;

            // Current state
            double T_P = this.tempP;
            double V_P = this.velP;
            double dens_P = this.densP;

            // Predicted state
            double T_F = this.tempF;
            double V_F = this.velF;
            double dens_F = this.densF;

            // relations - Current state
            double AT_P = rectD.GetTempP() - this.tempP;
            double AV_P = rectD.GetVelP() - this.velP;
            double Adens_P = rectD.GetDensP() - this.densP;

            // relations - Predicted state
            double AT_F = rectD.GetTempF() - this.tempF;
            double AV_F = rectD.GetVelF() - this.velF;
            double Adens_F = rectD.GetDensF() - this.densF;

            // equations --> future state
            double parteP_V = (V_P * AV_P) + (AT_P / gamma) + ((T_P * Adens_P) / (gamma * dens_P));
            double parteF_V = (V_F * AV_F) + (AT_F / gamma) + ((T_F * Adens_F) / (gamma * dens_F));
            this.velF = V_P - ((K / 2) * (parteP_V + parteF_V));
            double parteP_dens = (dens_P * AV_P) + (dens_P * V_P * Math.Log(relA)) + (V_P * dens_P);
            double parteF_dens = (dens_F * AV_F) + (densF * V_F * Math.Log(relA)) + (V_F * dens_F);
            this.densF = dens_P - ((K / 2) * (parteP_dens + parteF_dens));
            double parteP_T = (V_P * AT_P) + (T_P * (gamma - 1) * (AV_P - (V_P * Math.Log(relA))));
            double parteF_T = (V_F * AT_F) + (T_F * (gamma - 1) * (AV_F - (V_F * Math.Log(relA))));
            this.tempF = T_P - ((K / 2) * (parteP_T + parteF_T));
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
