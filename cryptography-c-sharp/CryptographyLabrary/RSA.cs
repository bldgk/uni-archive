using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace CryptographyLabrary
{
    public class RSA : ICipher
    {
        public char[] Alphabet { get; set; }
        public List<long> KeysPQ { get; set; }
        List<long> PrivateKey { get; set; }
        List<long> PublicKey { get; set; }
        Random Random { get; set; }
        public RSA()
        {
            Alphabet = new char[] { };
            KeysPQ = new List<long>();
            Random = RandomProvider.GetThreadRandom();
            KeysPQ = GeneratePQKeys();
        }
        public string Encryption(string text)
        {
            PublicKey = MakePublicKey();
            string EncryptedText = String.Empty;
            foreach (char Char in text)
            {
                ModCalculator Calculation = new ModCalculator(Char, PublicKey[0], PublicKey[1]);
                EncryptedText += Calculation.GetRemainder().ToString() + ",";
            }
            return EncryptedText;
        }
        public string Decryption(string Text)
        {
            List<long> Publickey = GetPublicKey();
            List<long> PrivateKey = MakePrivateKey(Publickey);
            string DecryptedText = String.Empty;
            List<string> IntegersInText = Text.Split(',').ToList();
            IntegersInText.RemoveAt(IntegersInText.Count - 1);
            List<long> Chars = new List<long>();
            foreach (string Integer in IntegersInText)
                Chars.Add(Convert.ToInt64(Integer));
            foreach (long Char in Chars)
            {
                ModCalculator Calculation = new ModCalculator(Char, PrivateKey[0], PrivateKey[1]);
                DecryptedText += Convert.ToChar(Convert.ToInt64(Calculation.GetRemainder()));//.ToString();
            }
            return DecryptedText;
        }
        public List<long> MakePrivateKey(List<long> PublicKey)
        {
            List<long> Privatekey = new List<long>();
            Privatekey.Add(D(PublicKey[0], Phi(PublicKey[1])));
            Privatekey.Add(PublicKey[1]);
            return Privatekey;
        }
        public List<long> GetPublicKey() => PublicKey;
        public List<long> MakePublicKey()
        {
            List<long> Publickey = new List<long>();
            long phi = Phi(N(KeysPQ[0], KeysPQ[1]));
            long e = E(phi);
            Publickey.Add(e);
            Publickey.Add(N(KeysPQ[0], KeysPQ[1]));
            return Publickey;
        }
        public List<long> GeneratePQKeys()
        {
            List<long> KeysPQs = new List<long>();

            int KeysSize = Random.Next(2, 3);
            long P, Q = 0;
            do
            {
                P = Random.Next(RandomSize(KeysSize - 1), RandomSize(KeysSize));
            }
            while (IsPrime(P) == false);
            do
            {
                Q = Random.Next(RandomSize(KeysSize - 1), RandomSize(KeysSize));
            }
            while (IsPrime(Q) == false);
            KeysPQs.Add(P);
            KeysPQs.Add(Q);
            return KeysPQs;
        }
        public long E(long Phi)
        {
            long e = 0;
            do
            {
                e = Random.Next(1, Convert.ToInt32(Phi));
            }
            while (IsCoprime(e, Phi) != true);
            return e;
        }
        public long D(long e, long Phi)
        {
            double D = 0;
            long k = 1;
            while (true)
            {
                D = (1 + (k * Phi)) / (double)e;
                if ((Round(D, 5) % 1) == 0) //integer
                {
                    return (long)D;
                }
                else
                {
                    k++;
                }
            }
        }
        public long GCD(long A, long B)
        {

            while (B != 0)
                B = A % (A = B);
            return A;
        }
        public bool IsCoprime(long A, long B) => (GCD(A, B) == 1) ? true : false;
        public long N(long P, long Q) => P * Q;
        public long Phi(long P, long Q) => (P - 1) * (Q - 1);
        public long Phi(long N)
        {
            long Phi = 1;
            foreach (long PrimeNumber in ToFactor(N))
                Phi *= PrimeNumber - 1;
            return Phi;
        }
        public int RandomSize(int Size)=> Convert.ToInt32(1.ToString().PadRight(Size + 1, '0'));
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
        public List<long> ToFactor(long Number)
        {
            List<long> Multipliers = new List<long>();
            long B, C;

            while ((Number % 2) == 0)
            {
                Number = Number / 2;
                Multipliers.Add(2);
            }
            B = 3; C = (int)Math.Sqrt(Number) + 1;
            while (B < C)
            {
                if ((Number % B) == 0)
                {
                    if (Number / B * B - Number == 0)
                    {
                        Multipliers.Add(B);
                        Number = Number / B;
                        C = (int)Math.Sqrt(Number) + 1;
                    }
                    else
                        B += 2;
                }
                else
                    B += 2;
            }
            Multipliers.Add(Number);
            return Multipliers;
        }
    }
    
}
