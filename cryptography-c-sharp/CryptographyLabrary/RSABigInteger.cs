using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabrary
{
    public class RSABigInteger : ICipher
    {
        public BigInteger P { get; set; }
        public BigInteger N { get; set; }
        public BigInteger E { get; set; }
        public BigInteger Phi { get; set; }
        public BigInteger Q { get; set; }
        public BigInteger D { get; set; }

        public char[] Alphabet
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        public string Encryption(string text)
        {
            GenerateP();
            GenerateQ();
            CalculateN();
            CalculatePhi();
            GenerateE();

            var EncryptedText = String.Empty;
            text.ToUTF8().ToList().ForEach((Action<byte>)(c => EncryptedText += System.Numerics.BigInteger.ModPow(c, (BigInteger)E, (BigInteger)N) + ","));
            return EncryptedText;

        }

        public string Decryption(string text)
        {
            CalculateD();
            var DecryptedText = String.Empty;
            text.Remove(text.Length - 1).Split(',').ToList().ForEach((Action<string>)(part => DecryptedText += (char)System.Numerics.BigInteger.ModPow(System.Numerics.BigInteger.Parse(part), (BigInteger)D, (BigInteger)N)));
            return DecryptedText;
        }

        private void GenerateP()
        {
            BigInteger p = RandomIntegerBelow(10000000000);
            while (!IsProbabilyPrime(p, 20))
            {
                p = RandomIntegerBelow(10000000000);
            }
            P = p;
        }

        private void GenerateQ()
        {
            BigInteger q = RandomIntegerBelow(10000000000);
            while (!IsProbabilyPrime(q, 20))
            {
                q = RandomIntegerBelow(10000000000);
            }
            Q = q;
        }

        private void CalculateD()
        {
            BigInteger[] result = new BigInteger[3];
            result = Extended_GCD(Phi, E);
            if (result[2] < 0)
                result[2] = result[2] + Phi;
            D = result[2];
        }

        private void CalculateN() =>
            N = System.Numerics.BigInteger.Multiply((BigInteger)P, (BigInteger)Q);

        private void CalculatePhi() =>
            Phi = System.Numerics.BigInteger.Multiply((BigInteger)(P - 1), (BigInteger)(Q - 1));

        private void GenerateE()
        {
            BigInteger temp = 0;
            while (GCD_Euclidean(temp, Phi) != 1)
            {
                temp = RandomIntegerBelow(Phi);
            }
            E = temp;
        }


        #region helpers


        public static BigInteger GCD_Loop(BigInteger A, BigInteger B)
        {
            BigInteger R = System.Numerics.BigInteger.One;
            while (B != 0)
            {
                R = A % B;
                A = B;
                B = R;
            }
            return A;
        }

        public BigInteger GCD_Euclidean(BigInteger A, BigInteger B)
        {
            if (B == 0)
                return A;
            if (A == 0)
                return B;
            if (A > B)
                return GCD_Euclidean(B, A % B);
            else
                return GCD_Euclidean(B % A, A);
        }

        public bool IsProbabilyPrime(BigInteger n, int k)
        {
            bool result = false;
            if (n < 2)
                return false;
            if (n == 2)
                return true;
            // return false if n is even -> divisbla by 2
            if (n % 2 == 0)
                return false;
            //writing n-1 as 2^s.d
            BigInteger d = n - 1;
            BigInteger s = 0;
            while (d % 2 == 0)
            {
                d >>= 1;
                s = s + 1;
            }
            for (int i = 0; i < k; i++)
            {
                BigInteger a;
                do
                {
                    a = RandomIntegerBelow(n - 2);
                }
                while (a < 2 || a >= n - 2);

                if (System.Numerics.BigInteger.ModPow(a, d, n) == 1) return true;
                for (int j = 0; j < s - 1; j++)
                {
                    if (System.Numerics.BigInteger.ModPow(a, 2 * j * d, n) == n - 1)
                        return true;
                }
                result = false;
            }
            return result;
        }

        public BigInteger RandomIntegerBelow(int n)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[n / 8];

            rng.GetBytes(bytes);

            var msb = bytes[n / 8 - 1];
            var mask = 0;
            while (mask < msb)
                mask = (mask << 1) + 1;

            bytes[n - 1] &= Convert.ToByte(mask);
            BigInteger p = new BigInteger(bytes);
            return p;
        }

        public BigInteger RandomIntegerBelow(BigInteger bound)
        {
            var rng = new RNGCryptoServiceProvider();
            //Get a byte buffer capable of holding any value below the bound
            var buffer = (bound << 16).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

            //Compute where the last partial fragment starts, in order to retry if we end up in it
            var generatedValueBound = System.Numerics.BigInteger.One << (buffer.Length * 8 - 1); //-1 accounts for the sign bit
            var validityBound = generatedValueBound - generatedValueBound % bound;

            while (true)
            {
                //generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
                rng.GetBytes(buffer);
                buffer[buffer.Length - 1] &= 0x7F; //force sign bit to positive
                var r = new BigInteger(buffer);

                //return unless in the partial fragment
                if (r >= validityBound) continue;
                return r % bound;
            }
        }

        public BigInteger[] Extended_GCD(BigInteger A, BigInteger B)
        {
            BigInteger[] result = new BigInteger[3];
            bool reverse = false;
            if (A < B) //if A less than B, switch them
            {
                BigInteger temp = A;
                A = B;
                B = temp;
                reverse = true;
            }
            BigInteger r = B;
            BigInteger q = 0;
            BigInteger x0 = 1;
            BigInteger y0 = 0;
            BigInteger x1 = 0;
            BigInteger y1 = 1;
            BigInteger x = 0, y = 0;
            while (A % B != 0)
            {
                r = A % B;
                q = A / B;
                x = x0 - q * x1;
                y = y0 - q * y1;
                x0 = x1;
                y0 = y1;
                x1 = x;
                y1 = y;
                A = B;
                B = r;
            }
            result[0] = r;
            if (reverse)
            {
                result[1] = y;
                result[2] = x;
            }
            else
            {
                result[1] = x;
                result[2] = y;
            }
            return result;
        }

        public BigInteger Extended_GCD2(BigInteger n, BigInteger m)
        {
            BigInteger[] Quot = new BigInteger[50];
            bool reverse = false;
            if (n < m)
            {
                BigInteger z;
                z = n;
                n = m;
                m = z;
                reverse = true;
            }
            BigInteger originaln = n;
            BigInteger originalm = m;
            int xstep = 1;
            BigInteger r = 1;
            while (r != 0)
            {
                BigInteger q = n / m;
                r = n - m * q;
                n = m;
                m = r;
                Quot[xstep] = q;
                ++xstep;
            }
            //setgcd(n)
            BigInteger gcd = n;
            BigInteger a = 1;
            BigInteger b = 0;
            for (int i = xstep; i > 0; i--)
            {
                BigInteger z = b - Quot[i] * a;
                b = a;
                a = z;
            }

            return a;
        }


        #endregion
    }
}
