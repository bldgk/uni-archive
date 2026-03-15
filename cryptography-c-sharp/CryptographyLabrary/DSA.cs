using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabrary
{
    public struct Sign
    {
        public int R, S;
    };

    public struct PublicKey
    {
        public int Q { get; set; }
        public int P { get; set; }
        public int G { get; set; }
        public int Y { get; set; }
        //public int Q, P, G, Y;
    };

    public class DSA : ICipher
    {
        public PublicKey PublicKey;
        public Sign Signature;
        public DSA() { }

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

        public int Message { get; set; } = 31;

        private int X { get; set; }

        private int H { get; set; }

        private int Hash { get; set; }

        public int Result { get; set; }

        Random Random { get; set; } = RandomProvider.GetThreadRandom();

        public string Decryption(string text)
        {
            throw new NotImplementedException();
        }

        public string Encryption(string text)
        {
            throw new NotImplementedException();
        }

        public void GeneratePQ()
        {
            PublicKey.P = 21599;
            PublicKey.Q = 10799;
        }

        private void GeneratePrivateKey()
        {
            X = 1 + Random.Next(PublicKey.Q - 1);
            H = 1 + Random.Next(PublicKey.P - 1);
        }

        public void GetHash()
        {

        }
        public void GeneratePublicKey()
        {
            GeneratePQ();
            GeneratePrivateKey();
            GetHash();

            //while (mod_pow(H, (PublicKey.P - 1) / PublicKey.Q, PublicKey.P) == 1)
            //{
            //    H++;
            //    H = H % PublicKey.P;
            //}

            PublicKey.G = ModPower(H, (PublicKey.P - 1) / PublicKey.Q, PublicKey.P);
            PublicKey.Y = ModPower(PublicKey.G, X, PublicKey.P);

        }

        public bool Validate(Sign si, PublicKey publicKey, int m)
        {
            int w, u1, u2, v;
            w = ModDivide(si.S, publicKey.Q);
            u1 = ModMultiply(m, w, publicKey.Q);
            u2 = ModMultiply(si.R, w, publicKey.Q);
            v = ModMultiply(ModPower(publicKey.G, u1, publicKey.P), ModPower(publicKey.Y, u2, publicKey.P), publicKey.P) % publicKey.Q;
            return v == si.R;
        }

        private int ModAdd(int x, int y, int p)
        {
            if ((p - x) > y)
                return x + y;
            else
                return y - (p - x);
        }

        private int ModSubstruct(int x, int y, int p)
        {
            return ModAdd(x, p - y, p);
        }

        private int ModMultiply(int x, int y, int p)
        {
            int x1, x2;
            int y1, y2;
            int s = (int)Math.Sqrt(p);
            if (x == 0 || y == 0) return 0;
            if (x == 1) return y;
            if (y == 1) return x;
            if (x <= s && y <= s)
            {
                return x * y;
            }
            else
            {
                x1 = x >> 1;
                x2 = x - x1;
                y1 = y >> 1;
                y2 = y - y1;
                return ModAdd(ModAdd(ModAdd(ModMultiply(x1, y1, p), ModMultiply(x1, y2, p), p), ModMultiply(x2, y1, p), p), ModMultiply(x2, y2, p), p);
            }
        }

        private int ModDivide(int pub, int p)
        {
            int i = p;
            int j = pub;
            int y = p;
            int y2 = 0;
            int y1 = 1;
            int quotient = p;
            int remainder = p;
            if (pub >= p)
                return p;
            while (j > 0)
            {
                quotient = i / j;
                remainder = i - (j * quotient);
                y = y2 - (y1 * quotient);
                i = j;
                j = remainder;
                y2 = y1;
                y1 = y;
            }
            if (i != 1)
                return p;
            if (y2 > 0)
                return y2 % p;
            else
                return p + y2;
        }

        private int ModPower(int x, int n, int p)
        {
            int r = 1;
            while (n == 1)
            {
                if ((n & 1) == 1)
                    r = ModMultiply(x, r, p);
                n >>= 1;
                x = ModMultiply(x, x, p);
            }
            return r;
        }

        public void CreateSignature()
        {
            GeneratePublicKey();
            int K = 1 + Random.Next(PublicKey.Q - 1);
            Signature.R = ModPower(PublicKey.G, K, PublicKey.P) % PublicKey.Q;
            Signature.S = ModMultiply(ModDivide(K, PublicKey.Q), ModAdd(Message, ModMultiply(Signature.R, X, PublicKey.Q), PublicKey.Q), PublicKey.Q);
        }

        public string GetSignature()
        {
            return Signature.R.ToString() + " " + Signature.S.ToString();
        }

        public override string ToString()
        {
            return
                String.Format("PublicKey: \n\tP: {0}\n\tQ: {1}\n\tG: {2}\n\tY: {3}", PublicKey.P, PublicKey.Q, PublicKey.G, PublicKey.Y);
        }

    }
}