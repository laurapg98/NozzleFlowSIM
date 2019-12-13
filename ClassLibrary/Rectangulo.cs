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
        public double[] ComputePredictedFutureState(double At, double Ax, double gamma, Rectangulo rectD) // MACCORMACK'S TECHNIQUE: utiliza las eqs discretizadas para calcular el predicted state de la celda --> lo guardamos todo en futuro 
        {
            // return the derivatives: 
            double[] ders = new double[3];

            // relations
            double AT = rectD.GetTempP() - this.tempP;
            double AV = rectD.GetVelP() - this.velP;
            double Adens = rectD.GetDensP() - this.densP;
            double AlnA = Math.Log(rectD.GetArea()) - Math.Log(this.area);

            // equations
                //derivatives
            double derT = -((this.velP * AT) / Ax) - ((gamma - 1) * this.tempP * ((AV / Ax) + ((this.velP * AlnA) / Ax)));
            double derV = -((this.velP * AV) / Ax) - ((1 / gamma) * ((AT / Ax) + ((this.tempP / this.densP) * (Adens / Ax))));
            double derDENS = -((this.densP * AV) / Ax) - (this.densP * this.velP * (AlnA / Ax)) - (this.velP * (Adens / Ax));
            ders[0] = derDENS;
            ders[1] = derV;
            ders[2] = derT;
                //predicted state
            this.tempF = this.tempP + (derT * At);
            this.velF = this.velP + (derV * At);
            this.densF = this.densP + (derDENS * At);

            // return time derivatives
            return ders;
        }

        public void ComputeFutureState(double At, double Ax, double gamma, double[] ders, Rectangulo rectI) // PREDICTED-CORRECTION METHOD: corrije el estado predicted (que está en futuro) usando también el presente (que está en presente)
        {
            // Predicted state
            double T_F = this.tempF;
            double V_F = this.velF;
            double dens_F = this.densF;

            // relations - Predicted state
            double AT_F = T_F - rectI.GetTempF();
            double AV_F = V_F - rectI.GetVelF();
            double Adens_F = dens_F - rectI.GetDensF();
            double AlnA = Math.Log(this.area) - Math.Log(rectI.GetArea());

            // equations
                //current state derviatives
            double dDENS_I = ders[0];
            double dV_I = ders[1];
            double dT_I = ders[2];
                //predicted future state derivatives
            double dDENS_P = -(dens_F * (AV_F / Ax)) - ((dens_F * V_F * AlnA) / Ax) - ((V_F * Adens_F) / Ax);
            double dV_P = -(V_F * (AV_F / Ax)) - ((1 / gamma) * ((AT_F / Ax) + ((T_F / dens_F) * (Adens_F / Ax))));
            //double dV_P = (-(V_F) * (AV_F / Ax)) - ((1 / gamma) * ((AT_F / Ax) + (T_F / dens_F) * (Adens_F / Ax)));
            double dT_P = -(V_F * (AT_F / Ax)) - ((gamma - 1) * T_F * ((AV_F / Ax) + (V_F * (AlnA / Ax))));
                //average derivatives
            double dDENS_av = 0.5 * (dDENS_I + dDENS_P);
            double dV_av = 0.5 * (dV_I + dV_P);
            double dT_av = 0.5 * (dT_I + dT_P);
                //future state
            this.densF = this.densP + (dDENS_av * At);
            this.velF = this.velP + (dV_av * At);
            this.tempF = this.tempP + (dT_av * At);
            this.presF = this.tempF * this.densF;
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
