using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabrary
{
    public class SHA1 : ICipher
    {
        public SHA1() { Initialization(); }
        public byte[] Buffer { get; set; }
        public ulong Length { get; set; }
        public ulong Blocks { get; set; }
        public string Text { get; set; }
        public byte[] CompleteText { get; set; }
        public byte[] ByteText { get; set; }
        public uint[] W { get; set; }
        private static readonly uint[] K = {
            0x5A827999,0x6ED9EBA1,0x8F1BBCDC,0xCA62C1D6
        };
        DIGEST Digest;
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

        //Инициализация переменных:
        public void Initialization()
        {
            Digest.H0 =  0x67452301;
            Digest.H1 = 0xEFCDAB89;
            Digest.H2 =  0x98BADCFE;
            Digest.H3 = 0x10325476;
            Digest.H4 = 0xC3D2E1F0;
            W = new uint[80]; ;
        }
        public void PreliminaryProcessing()
        {
            Buffer = new byte[ByteText.LongLength];
            Array.Copy(ByteText, Buffer, ByteText.LongLength);
            Length = (ulong)ByteText.LongLength;

            int zeroBitsToAddQty = 512 - (int)(((ulong)Buffer.LongLength * 8 + 1 + 64) % 512);
            CompleteText = new byte[((ulong)Buffer.LongLength * 8 + 1 + 64 + (ulong)zeroBitsToAddQty) / 8];
            Array.Copy(Buffer, CompleteText, Buffer.LongLength);

            CompleteText[Buffer.LongLength] = 128; //set 1-st bit to "1", 7 remaining to "0" (may not work with bit-multiple message!!)

            for (ulong i = (Length + 1); i < (ulong)CompleteText.LongLength; i++)
            {
                CompleteText[i] = 0;
            }

            byte[] messageBitLength_little_endian = BitConverter.GetBytes(Length * 8);
            byte[] messageBitLength_big_endian = new byte[messageBitLength_little_endian.Length];
            for (int i = 0, j = messageBitLength_little_endian.Length - 1; i < messageBitLength_little_endian.Length; i++, j--)
            {
                messageBitLength_big_endian[i] = messageBitLength_little_endian[j];
            }
            Array.Copy(messageBitLength_big_endian, 0, CompleteText, CompleteText.LongLength - 8, 8);
        }

        public string Hash(byte[] input)
        {
            //operates only with byte-multiple messages
            ByteText = input;
            return Hash();
        }
        public string Hash()
        {
            PreliminaryProcessing();
            Blocks = (ulong)CompleteText.LongLength / 64;
            for (ulong i = 0; i < Blocks; i++)
            {
                Expanding(i);
                Processing();
            }

            return String.Format("{0:X}", Digest.H0) + " " +
                   String.Format("{0:X}", Digest.H1) + " " +
                    String.Format("{0:X}", Digest.H2) + " " +
                    String.Format("{0:X}", Digest.H3) + " " +
                    string.Format("{0:X}", Digest.H4);
        }
        public string Hash(string input)
        {
            Text = input;
            ByteText = Text.ToUTF8();
            return Hash();
        }
        public string Encryption(string text) => String.Empty;

        public string Decryption(string text) => String.Empty;

        private void Expanding(ulong blockNumber)
        {
            for (int i = 0; i < 16; i++)
            {
                W[i] = BytesToUInt32(CompleteText, blockNumber * 64 + (ulong)i * 4);
            }

            for (int i = 16; i <= 79; i++)
            {
                W[i] = BitRotate.RotateLeft(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
            }
        }

        internal static uint BytesToUInt32(byte[] bs, ulong off)
        {
            uint n = (uint)bs[off] << 24;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off];
            return n;
        }
        private uint F(int S, uint b, uint c, uint d)
        {
            switch (S)
            {
                case 0:
                    return (b & c) | (~b & d);
                case 1:
                    return b ^ c ^ d;
                case 2:
                    return (b & c) | (b & d) | (c & d);
                case 3:
                    return b ^ c ^ d;
                default:
                    return 0xfffffff;
            }
        }
        private void Processing()
        {
            uint a = Digest.H0;
            uint b = Digest.H1;
            uint c = Digest.H2;
            uint d = Digest.H3;
            uint e = Digest.H4;
            uint T = 0;
            for (int i = 0; i < 80; i++)
            {
                int S = (int)Math.Floor(i / 20.0);
                T = BitRotate.RotateLeft(a, 5) + F(S, b, c, d) + e + K[S] + W[i];
                e = d;
                d = c;
                c = BitRotate.RotateLeft(b, 30);
                b = a;
                a = T;
            }
            Digest.H0 += a;
            Digest.H1 += b;
            Digest.H2 += c;
            Digest.H3 += d;
            Digest.H4 += e;
        }
       
    }
    public struct DIGEST
    {
        public uint H0, H1, H2, H3, H4;
    }
}
