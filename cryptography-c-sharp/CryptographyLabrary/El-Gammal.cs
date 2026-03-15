using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
namespace CryptographyLabrary
{
    public class El_Gammal : ICipher
    {
        public char[] Alphabet { get; set; }
        private int PrivateKey { get; set; }
        private double PublicKey { get; set; }
        private int SessionKey { get; set; }
        public Random Random { get; set; }
        private int P { get; set; }
        private int G { get; set; }

        public El_Gammal()
        {
            Alphabet = new char[] { };
            Random = RandomProvider.GetThreadRandom();
            PrivateKey = GeneratePrivateKey();
        }
        public string Encryption(string Text)
        {
            string EncryptedText = String.Empty;
            SessionKey = GenerateSessionKey();
            List<int> CharsInText = new List<int>();
            foreach (char Char in Text)
            {
                double A = ModCalculator.GetPowerRemainder(G, SessionKey, P);
                double B = ModCalculator.GetMultiplyRemainder(ModCalculator.GetPowerRemainder(PublicKey, SessionKey, P), Char, P);
                EncryptedText += String.Format("{0} {1},", A, B);
            }
            return EncryptedText;
        }
        public string Decryption(string Text)
        {
            string DecryptedText = String.Empty;
            List<string> CryptText = Text.Split(',').ToList();
            CryptText.RemoveAt(CryptText.Count - 1);
            foreach (string Char in CryptText)
            {

                double A = Convert.ToDouble(Char.Split(' ').ToList()[0]);
                double B = Convert.ToDouble(Char.Split(' ').ToList()[1]);
                DecryptedText += Convert.ToChar(Convert.ToInt64(ModCalculator.GetMultiplyRemainder(B, ModCalculator.GetPowerRemainder(A, P - 1 - PrivateKey, P), P)));// m=b*(a^x)^(-1)mod p =b*a^(p-1-x)mod p - трудно было  найти нормальную формулу, в ней вся загвоздка 
            }
            return DecryptedText;
        }
        public void CreateLink(El_Gammal El_Gammal)
        {
            GeneratePublicKey();
            int SetG;
            int SetP;
            double SetY;
            GetPublickey(out SetG, out SetP, out SetY);
            El_Gammal.SetPublicKey(SetG, SetP, SetY);
        }
        public void GeneratePublicKey()
        {
            do
            {
                P = GenerateP();
            }
            while (GetPrimitiveRoot(P) == 0);
            G = GetPrimitiveRoot(P);
            PublicKey = ModCalculator.GetPowerRemainder(G, PrivateKey, P);
        }
        public void SetPublicKey(int GNew, int PNew, double Publickey)
        {
            G = GNew;
            P = PNew;
            PublicKey = Publickey;
        }
        public void GetPublickey(out int GetG, out int GetP, out double GetY)
        {
            GetG = G;
            GetP = P;
            GetY = PublicKey;
        }
        public int GeneratePrivateKey() => Random.Next(RandomSize(0), RandomSize(1));
        public int GenerateSessionKey()
        {
            int k = 0;
            do
            {
                k = Random.Next(1, P - 1);
            }
            while (IsCoprime(k, P) != true);
            return k;
        }
        public int GenerateP()
        {
            int KeysSize = 3;
            do
            {
                P = Random.Next(RandomSize(KeysSize - 1), RandomSize(KeysSize));
            }
            while (IsPrime(P) == false);
            return P;
        }
        public int RandomSize(int Size) => Convert.ToInt32(1.ToString().PadRight(Size + 1, '0'));
        #region Checks
        public bool IsPrime(long Number)
        {
            bool Prime = true;
            for (int i = 2; i <= Number / 2; i++)
            {
                if (Number % i == 0)
                {
                    Prime = false;
                    break;
                }
            }
            return Prime;
        }
        public int GetPrimitiveRoot(long p)
        {
            for (int i = 0; i < p; i++)
                if (IsPrimitiveRoot(i, p))
                    return i;
            return 0;
        }
        public bool IsPrimitiveRoot(long G, long P)
        {
            if (G == 0 || G == 1)
                return false;
            long Phi = P - 1;
            List<double> Remainders = new List<double>();
            for (int i = 0; (i < Phi); i++)
            {
                double Remainder = Pow(G, i) % P;
                if (Remainders.Contains(Remainder))
                    return false;
                Remainders.Add(Remainder);
            }
            return true;
        }
        public bool IsCoprime(long A, long B) => (GCD(A, B) == 1) ? true : false;
        public long GCD(long A, long B)
        {

            while (B != 0)
                B = A % (A = B);
            return A;
        }
        #endregion
    }
}
