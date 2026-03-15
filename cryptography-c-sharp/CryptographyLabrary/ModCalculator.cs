using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace CryptographyLabrary
{
    public class ModCalculator
    {
        long Base { get; set; }
        long Degree { get; set; }
        double Remainder { get; set; }
        long Divider { get; set; }
        List<char> DegreeBinary { get; set; }
        public ModCalculator(long Base, long Degree, long Divider)
        {
            this.Base = Base;
            this.Degree = Degree;
            this.Divider = Divider;
            Remainder = this.Base;
            DegreeBinary = Convert.ToString(this.Degree, 2).ToList();
            DegreeBinary.ToString();
        }
        public double GetRemainder()
        {
            string B_row = String.Empty;
            string A_row = Base.ToString();
            if(Degree == 1)
            {
                Remainder = Base % Divider;
                return Remainder;
            }
            for (int i = 1; i < DegreeBinary.Count(); i++)
            {
                try
                {
                    Remainder = NextRemainder(Remainder, DegreeBinary[i]);
                }
                catch { }
                A_row += " " + Remainder.ToString();
                B_row += DegreeBinary[i];
            }
            return Remainder;
        }
        public static double GetPowerRemainder(double Base, int Degree, double Divider)
        {
            List<char> DegreeBinary = Convert.ToString(Degree, 2).ToList();
            double Remainder = Base;
            DegreeBinary = Convert.ToString(Degree, 2).ToList();
            DegreeBinary.ToString();
            string B_row = String.Empty;
            string A_row = Base.ToString();
            if (Degree == 1)
            {
                Remainder = Base % Divider;
                return Remainder;
            }
            for (int i = 1; i < DegreeBinary.Count(); i++)
            {
                try
                {
                    Remainder = NextRemainder(Remainder, DegreeBinary[i], Base, Divider);
                }
                catch { }
                A_row += " " + Remainder.ToString();
                B_row += DegreeBinary[i];
            }
            return Remainder;
        }
        public static double GetMultiplyRemainder(double A, double B, double Divider) =>  A * B % Divider;       
        public double NextRemainder(double CurrentRemainder, char B) => (B == '1') ? (Pow(CurrentRemainder, 2) * Base) % Divider : Pow(CurrentRemainder, 2) % Divider;
        public static double NextRemainder(double CurrentRemainder, char B, double Base, double Divider) => (B == '1') ? (Pow(CurrentRemainder, 2) * Base) % Divider : Pow(CurrentRemainder, 2) % Divider;

    }
}