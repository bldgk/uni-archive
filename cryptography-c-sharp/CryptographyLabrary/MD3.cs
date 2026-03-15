using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabrary
{
    public sealed class MD3
    {
        private const int BufShiftBits = 6;
        private const int BufSize = 0x40; // 2 ** 6
        private const int BufSizeModuloMask = BufSize - 1;
        private UInt32 A;
        private UInt32 B;
        private UInt32 C;
        private UInt32 D;

        private static byte[] Padding = new byte[BufSize];

        private byte[] Buffer;
        private UInt64 Count;   // count the bytes, not the bits
        private int BytesInBuffer { get { return (int)Count & BufSizeModuloMask; } }

        private enum SS
        {
            S11 = 7,
            S12 = 12,
            S13 = 17,
            S14 = 22,
            S21 = 5,
            S22 = 9,
            S23 = 14,
            S24 = 20,
            S31 = 4,
            S32 = 11,
            S33 = 16,
            S34 = 23,
            S41 = 6,
            S42 = 10,
            S43 = 15,
            S44 = 21

        };
        public MD3()
            : base()
        {
            Initialize();
        }
        static MD3()
        {
            Padding[0] = 0x80;
        }
        public byte[] Hash(byte[] input, int index, int count)
        {
            Initialize();
            Update(input, index, count);
            return Final();
        }
        public byte[] Hash(byte[] input)
        {
            return Hash(
                            input,
                            input.GetLowerBound(0),
                            input.GetUpperBound(0) - input.GetLowerBound(0) + 1
                        );
        }
        public byte[] Hash(string input)
        {
            return Hash(input.ToUTF8());
        }
        public byte[] Hash(string input, int index, int count)
        {
            return Hash(input.ToUTF8(index, count));
        }
        private const int ReadBufSize = 1024 * 8;

        public byte[] Hash(Stream stream)
        {
            byte[] buf = new byte[ReadBufSize];
            int nRead;

            Initialize();
            while ((nRead = stream.Read(buf, 0, ReadBufSize)) > 0)
            {
                Update(buf, 0, nRead);
            }
            return Final();
        }

        private void Initialize()
        {
            A = 0x67452301;
            B = 0xefcdab89;
            C = 0x98badcfe;
            D = 0x10325476;

            Count = 0;
            if (Buffer == null)
                Buffer = new byte[BufSize];
        }
        private void Update(byte[] input, int inputIndex, int inputCount)
        {
            int bytesInBuffer = BytesInBuffer;
            int bytesFreeInBuffer = BufSize - bytesInBuffer;

            Count += (ulong)inputCount;

            if (bytesInBuffer > 0)
            {
                if (inputCount >= bytesFreeInBuffer)
                {
                    Array.Copy(input, inputIndex, Buffer, bytesInBuffer, bytesFreeInBuffer);
                    inputIndex += bytesFreeInBuffer;
                    inputCount -= bytesFreeInBuffer;

                    Transform(Buffer, 0);
                    bytesInBuffer = 0;
                }

            }

            for (int i = inputCount >> BufShiftBits; --i >= 0;)
            {
                Transform(input, inputIndex);
                inputIndex += BufSize;
                inputCount -= BufSize;
            }

            if (inputCount > 0)
            {
                Array.Copy(input, inputIndex, Buffer, bytesInBuffer, inputCount);
            }

        }
        private byte[] Final()
        {
            int bytesInBuffer = BytesInBuffer;

            byte[] bits = new byte[8];

            UInt64 count = Count << 3; // # bits

            Encode((UInt32)(count), bits, 0);
            Encode((UInt32)(count >> 32), bits, 4);

            int padLen = (bytesInBuffer < 56) ? (56 - bytesInBuffer) : (120 - bytesInBuffer);
            Update(Padding, 0, padLen);

            Update(bits, 0, 8);

            byte[] digest = new byte[16];

            Encode(A, digest, 0);
            Encode(B, digest, 4);
            Encode(C, digest, 8);
            Encode(D, digest, 12);

            return digest;
        }
        private UInt32 F(UInt32 x, UInt32 y, UInt32 z)
        {
            return (((x) & (y)) | ((~x) & (z)));
        }
        private UInt32 G(UInt32 x, UInt32 y, UInt32 z)
        {
            return (((x) & (z)) | ((y) & (~z)));
        }
        private UInt32 H(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((x) ^ (y) ^ (z));
        }
        private UInt32 I(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((y) ^ ((x) | (~z)));
        }
        private void FF(ref UInt32 a, UInt32 b, UInt32 c,
                            UInt32 d, UInt32 x, SS s, UInt32 ac
                        )
        {
            unchecked
            {
#if !XX_inOneInstruction
                a += F(b, c, d) + x + ac;
                a = a.RotateLeft((int)s);
                a += b;
#else
				a = (a += F(b, c, d) + x + ac).RotateLeft((int)s) + b;
#endif
            }
        }
        private void GG(ref UInt32 a, UInt32 b, UInt32 c,
                            UInt32 d, UInt32 x, SS s, UInt32 ac
                        )
        {
            unchecked
            {
#if !XX_inOneInstruction
                a += G(b, c, d) + x + ac;
                a = a.RotateLeft((int)s);
                a += b;
#else
				a = (a += G(b, c, d) + x + ac).RotateLeft((int)s) + b;
#endif
            }
        }
        private void HH(ref UInt32 a, UInt32 b, UInt32 c,
                            UInt32 d, UInt32 x, SS s, UInt32 ac
                        )
        {
            unchecked
            {
#if !XX_inOneInstruction
                a += H(b, c, d) + x + ac;
                a = a.RotateLeft((int)s);
                a += b;
#else
				a = (a += H(b, c, d) + x + ac).RotateLeft((int)s) + b;
#endif
            }
        }
        private void II(ref UInt32 a, UInt32 b, UInt32 c,
                            UInt32 d, UInt32 x, SS s, UInt32 ac
                        )
        {
            unchecked
            {
#if !XX_inOneInstruction
                a += I(b, c, d) + x + ac;
                a = a.RotateLeft((int)s);
                a += b;
#else
				a = (a += I(b, c, d) + x + ac).RotateLeft((int)s) + b;
#endif
            }
        }
        private void Transform(byte[] input, int index)
        {
            UInt32 a = A;
            UInt32 b = B;
            UInt32 c = C;
            UInt32 d = D;
            UInt32[] x = new UInt32[16];

            Decode(x, input, index, 16);

            /* Round 1 */
            FF(ref a, b, c, d, x[0], SS.S11, 0xd76aa478); /* 1 */
            FF(ref d, a, b, c, x[1], SS.S12, 0xe8c7b756); /* 2 */
            FF(ref c, d, a, b, x[2], SS.S13, 0x242070db); /* 3 */
            FF(ref b, c, d, a, x[3], SS.S14, 0xc1bdceee); /* 4 */
            FF(ref a, b, c, d, x[4], SS.S11, 0xf57c0faf); /* 5 */
            FF(ref d, a, b, c, x[5], SS.S12, 0x4787c62a); /* 6 */
            FF(ref c, d, a, b, x[6], SS.S13, 0xa8304613); /* 7 */
            FF(ref b, c, d, a, x[7], SS.S14, 0xfd469501); /* 8 */
            FF(ref a, b, c, d, x[8], SS.S11, 0x698098d8); /* 9 */
            FF(ref d, a, b, c, x[9], SS.S12, 0x8b44f7af); /* 10 */
            FF(ref c, d, a, b, x[10], SS.S13, 0xffff5bb1); /* 11 */
            FF(ref b, c, d, a, x[11], SS.S14, 0x895cd7be); /* 12 */
            FF(ref a, b, c, d, x[12], SS.S11, 0x6b901122); /* 13 */
            FF(ref d, a, b, c, x[13], SS.S12, 0xfd987193); /* 14 */
            FF(ref c, d, a, b, x[14], SS.S13, 0xa679438e); /* 15 */
            FF(ref b, c, d, a, x[15], SS.S14, 0x49b40821); /* 16 */

            /* Round 2 */
            GG(ref a, b, c, d, x[1], SS.S21, 0xf61e2562); /* 17 */
            GG(ref d, a, b, c, x[6], SS.S22, 0xc040b340); /* 18 */
            GG(ref c, d, a, b, x[11], SS.S23, 0x265e5a51); /* 19 */
            GG(ref b, c, d, a, x[0], SS.S24, 0xe9b6c7aa); /* 20 */
            GG(ref a, b, c, d, x[5], SS.S21, 0xd62f105d); /* 21 */
            GG(ref d, a, b, c, x[10], SS.S22, 0x2441453); /* 22 */
            GG(ref c, d, a, b, x[15], SS.S23, 0xd8a1e681); /* 23 */
            GG(ref b, c, d, a, x[4], SS.S24, 0xe7d3fbc8); /* 24 */
            GG(ref a, b, c, d, x[9], SS.S21, 0x21e1cde6); /* 25 */
            GG(ref d, a, b, c, x[14], SS.S22, 0xc33707d6); /* 26 */
            GG(ref c, d, a, b, x[3], SS.S23, 0xf4d50d87); /* 27 */
            GG(ref b, c, d, a, x[8], SS.S24, 0x455a14ed); /* 28 */
            GG(ref a, b, c, d, x[13], SS.S21, 0xa9e3e905); /* 29 */
            GG(ref d, a, b, c, x[2], SS.S22, 0xfcefa3f8); /* 30 */
            GG(ref c, d, a, b, x[7], SS.S23, 0x676f02d9); /* 31 */
            GG(ref b, c, d, a, x[12], SS.S24, 0x8d2a4c8a); /* 32 */

            /* Round 3 */
            HH(ref a, b, c, d, x[5], SS.S31, 0xfffa3942); /* 33 */
            HH(ref d, a, b, c, x[8], SS.S32, 0x8771f681); /* 34 */
            HH(ref c, d, a, b, x[11], SS.S33, 0x6d9d6122); /* 35 */
            HH(ref b, c, d, a, x[14], SS.S34, 0xfde5380c); /* 36 */
            HH(ref a, b, c, d, x[1], SS.S31, 0xa4beea44); /* 37 */
            HH(ref d, a, b, c, x[4], SS.S32, 0x4bdecfa9); /* 38 */
            HH(ref c, d, a, b, x[7], SS.S33, 0xf6bb4b60); /* 39 */
            HH(ref b, c, d, a, x[10], SS.S34, 0xbebfbc70); /* 40 */
            HH(ref a, b, c, d, x[13], SS.S31, 0x289b7ec6); /* 41 */
            HH(ref d, a, b, c, x[0], SS.S32, 0xeaa127fa); /* 42 */
            HH(ref c, d, a, b, x[3], SS.S33, 0xd4ef3085); /* 43 */
            HH(ref b, c, d, a, x[6], SS.S34, 0x4881d05); /* 44 */
            HH(ref a, b, c, d, x[9], SS.S31, 0xd9d4d039); /* 45 */
            HH(ref d, a, b, c, x[12], SS.S32, 0xe6db99e5); /* 46 */
            HH(ref c, d, a, b, x[15], SS.S33, 0x1fa27cf8); /* 47 */
            HH(ref b, c, d, a, x[2], SS.S34, 0xc4ac5665); /* 48 */

            /* Round 4 */
            II(ref a, b, c, d, x[0], SS.S41, 0xf4292244); /* 49 */
            II(ref d, a, b, c, x[7], SS.S42, 0x432aff97); /* 50 */
            II(ref c, d, a, b, x[14], SS.S43, 0xab9423a7); /* 51 */
            II(ref b, c, d, a, x[5], SS.S44, 0xfc93a039); /* 52 */
            II(ref a, b, c, d, x[12], SS.S41, 0x655b59c3); /* 53 */
            II(ref d, a, b, c, x[3], SS.S42, 0x8f0ccc92); /* 54 */
            II(ref c, d, a, b, x[10], SS.S43, 0xffeff47d); /* 55 */
            II(ref b, c, d, a, x[1], SS.S44, 0x85845dd1); /* 56 */
            II(ref a, b, c, d, x[8], SS.S41, 0x6fa87e4f); /* 57 */
            II(ref d, a, b, c, x[15], SS.S42, 0xfe2ce6e0); /* 58 */
            II(ref c, d, a, b, x[6], SS.S43, 0xa3014314); /* 59 */
            II(ref b, c, d, a, x[13], SS.S44, 0x4e0811a1); /* 60 */
            II(ref a, b, c, d, x[4], SS.S41, 0xf7537e82); /* 61 */
            II(ref d, a, b, c, x[11], SS.S42, 0xbd3af235); /* 62 */
            II(ref c, d, a, b, x[2], SS.S43, 0x2ad7d2bb); /* 63 */
            II(ref b, c, d, a, x[9], SS.S44, 0xeb86d391); /* 64 */

            unchecked
            {
                A += a;
                B += b;
                C += c;
                D += d;
            }
        }

        private void Decode(UInt32[] output, byte[] input, int inputIndex, uint count)
        {
            for (int i = -1; ++i < count;)
            {
                output[i] =
                        (UInt32)input[inputIndex++]
                    | ((UInt32)input[inputIndex++] << 8)
                    | ((UInt32)input[inputIndex++] << 16)
                    | ((UInt32)input[inputIndex++] << 24);
            }
        }

        private void Encode(UInt32 input, byte[] output, int outputIndex)
        {
            output[outputIndex++] = (byte)(input & 0xff);
            output[outputIndex++] = (byte)((input >> 8) & 0xff);
            output[outputIndex++] = (byte)((input >> 16) & 0xff);
            output[outputIndex++] = (byte)((input >> 24) & 0xff);
        }
    }


    public static class Encoding
    {

        public static byte[] ToUTF8(this string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);

        }
        public static byte[] ToUTF8(this string s, int index, int count)
        {
            return System.Text.Encoding.UTF8.GetBytes(s.Substring(index, count));
        }
        public static string FromUTF8(this byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);

        }
    }


    public static class HexConversion
    {
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        public static byte[] FromHex(this string s)
        {
            if (s.Length % 2 != 0)
                throw new ArgumentException("Length must be even");

            var bb = new byte[s.Length >> 1];

            for (int i = 0, j = 0; i < s.Length; i += 2, j++)
            {
                bb[j] = Byte.Parse(s.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return bb;
        }
    }


    public static class BitRotate
    {
        public static UInt32 RotateLeft(this UInt32 x, int nBits)
        {
            nBits &= 0x1f;

            return (x << nBits) | (x >> (32 - nBits));
        }
    }
}