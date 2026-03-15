using System;
using System.Collections.Generic;
using static System.Math;
namespace CryptographyLabrary
{
    public class CDH : ICipher
    {
        public int G { get; set; }
        public long P { get; set; }
        public char[] Alphabet { get; set; }
        private int PrivateKey { get; set; }
        private double PublicKey { get; set; }
        private int SecretKey { get; set; }
        Random Random { get; set; }
        public CDH()
        {
            Alphabet = new char[] { };
            Random = RandomProvider.GetThreadRandom();
            PrivateKey = GeneratePrivateKey();
        }
        public string Encryption(string Text)
        {
            string EncryptedText = String.Empty;

            return EncryptedText;
        }
        public string Decryption(string Text)
        {
            string DecryptedText = String.Empty;

            return DecryptedText;
        }
        public int GeneratePrivateKey() => Random.Next(RandomSize(0), RandomSize(1));
        public void CreateLink(CDH CDH)
        {
            SetParameters();
            int SetG;
            long SetP;
            GetParameters(out SetG, out SetP);
            CDH.SetParameters(SetG, SetP);
            SetPublicKey(CDH.MakePublicKey());
            CDH.SetPublicKey(MakePublicKey());
            MakeSecretKey();
            CDH.MakeSecretKey();
        }
        public double MakePublicKey() => ModCalculator.GetPowerRemainder(G, PrivateKey, P);
         public void SetPublicKey(double Publickey) => PublicKey = Publickey;
   
        public void MakeSecretKey()
        {
            SecretKey = Convert.ToInt32(ModCalculator.GetPowerRemainder(Convert.ToInt64(PublicKey), PrivateKey, P));
            // Secret key computed
        }
        public void SetParameters()
        {
            do
            {
                P = GenerateP();
            }
            while (GetPrimitiveRoot(P) == 0);
            G = GetPrimitiveRoot(P);
        }
        public void SetParameters(int GNew, long PNew)
        {
            G = GNew;
            P = PNew;
        }
        public void GetParameters(out int GetG, out long GetP)
        {
            GetG = G;
            GetP = P;
        }
        public long GenerateP()
        {
            int KeysSize = 2;
            do
            {
                P = Random.Next(RandomSize(KeysSize - 1), RandomSize(KeysSize));
            }
            while (IsPrime(P) == false);
            return P;
        }
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
        public int RandomSize(int Size) => Convert.ToInt32(1.ToString().PadRight(Size + 1, '0'));
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
    }
}