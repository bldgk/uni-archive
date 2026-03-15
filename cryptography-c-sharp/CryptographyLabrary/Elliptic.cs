using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLabrary
{
    public class Elliptic
    {
    }
    public class BInteger
    {

        private const int maxLength = 70;

        public static readonly int[] primesBelow2000 = {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
        101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
    211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
    307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
    401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
    503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
    601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
    701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
    809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
    907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
    1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
    1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
    1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
    1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
    1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
    1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
    1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
    1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
    1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
    1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };


        private uint[] data = null;             // stores bytes from the Big Integer
        public int dataLength;                 // number of actual chars used


        public BInteger()
        {
            data = new uint[maxLength];
            dataLength = 1;
        }
        public BInteger(long value)
        {
            data = new uint[maxLength];
            long tempVal = value;
            dataLength = 0;
            while (value != 0 && dataLength < maxLength)
            {
                data[dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                dataLength++;
            }

            if (tempVal > 0)         // overflow check for +ve value
            {
                if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }
            else if (tempVal < 0)    // underflow check for -ve value
            {
                if (value != -1 || (data[dataLength - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }

            if (dataLength == 0)
                dataLength = 1;
        }


        public BInteger(ulong value)
        {
            data = new uint[maxLength];

            dataLength = 0;
            while (value != 0 && dataLength < maxLength)
            {
                data[dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                dataLength++;
            }

            if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive overflow in constructor."));

            if (dataLength == 0)
                dataLength = 1;
        }


        public BInteger(BInteger bi)
        {
            data = new uint[maxLength];

            dataLength = bi.dataLength;

            for (int i = 0; i < dataLength; i++)
                data[i] = bi.data[i];
        }



        public BInteger(string value, int radix)
        {
            BInteger multiplier = new BInteger(1);
            BInteger result = new BInteger();
            value = (value.ToUpper()).Trim();
            int limit = 0;

            if (value[0] == '-')
                limit = 1;

            for (int i = value.Length - 1; i >= limit; i--)
            {
                int posVal = value[i];

                if (posVal >= '0' && posVal <= '9')
                    posVal -= '0';
                else if (posVal >= 'A' && posVal <= 'Z')
                    posVal = (posVal - 'A') + 10;
                else
                    posVal = 9999999;       // arbitrary large


                if (posVal >= radix)
                    throw (new ArithmeticException("Invalid string in constructor."));
                else
                {
                    if (value[0] == '-')
                        posVal = -posVal;

                    result = result + (multiplier * posVal);

                    if ((i - 1) >= limit)
                        multiplier = multiplier * radix;
                }
            }

            if (value[0] == '-')     // negative values
            {
                if ((result.data[maxLength - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }
            else    // positive values
            {
                if ((result.data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }

            data = new uint[maxLength];
            for (int i = 0; i < result.dataLength; i++)
                data[i] = result.data[i];

            dataLength = result.dataLength;
        }

        public BInteger(byte[] inData)
        {
            dataLength = inData.Length >> 2;

            int leftOver = inData.Length & 0x3;
            if (leftOver != 0)         // length not multiples of 4
                dataLength++;


            if (dataLength > maxLength)
                throw (new ArithmeticException("Byte overflow in constructor."));

            data = new uint[maxLength];

            for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
            {
                data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
                data[dataLength - 1] = inData[0];
            else if (leftOver == 2)
                data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            else if (leftOver == 3)
                data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;


        }

        public BInteger(byte[] inData, int inLen)
        {
            dataLength = inLen >> 2;

            int leftOver = inLen & 0x3;
            if (leftOver != 0)         // length not multiples of 4
                dataLength++;

            if (dataLength > maxLength || inLen > inData.Length)
                throw (new ArithmeticException("Byte overflow in constructor."));


            data = new uint[maxLength];

            for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
            {
                data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                                 (inData[i - 1] << 8) + inData[i]);
            }

            if (leftOver == 1)
                data[dataLength - 1] = inData[0];
            else if (leftOver == 2)
                data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
            else if (leftOver == 3)
                data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


            if (dataLength == 0)
                dataLength = 1;

            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;


        }


        public BInteger(uint[] inData)
        {
            dataLength = inData.Length;

            if (dataLength > maxLength)
                throw (new ArithmeticException("Byte overflow in constructor."));

            data = new uint[maxLength];

            for (int i = dataLength - 1, j = 0; i >= 0; i--, j++)
                data[j] = inData[i];

            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;


        }
        public static implicit operator BInteger(long value)
        {
            return (new BInteger(value));
        }

        public static implicit operator BInteger(ulong value)
        {
            return (new BInteger(value));
        }

        public static implicit operator BInteger(int value)
        {
            return (new BInteger(value));
        }

        public static implicit operator BInteger(uint value)
        {
            return (new BInteger((ulong)value));
        }

        public static BInteger operator +(BInteger bi1, BInteger bi2)
        {
            BInteger result = new BInteger();

            result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            long carry = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long sum = bi1.data[i] + (long)bi2.data[i] + carry;
                carry = sum >> 32;
                result.data[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result.dataLength < maxLength)
            {
                result.data[result.dataLength] = (uint)(carry);
                result.dataLength++;
            }

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;


            // overflow check
            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }


        public static BInteger operator ++(BInteger bi1)
        {
            BInteger result = new BInteger(bi1);

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < maxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if (index > result.dataLength)
                result.dataLength = index;
            else
            {
                while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                    result.dataLength--;
            }
            int lastPos = maxLength - 1;


            if ((bi1.data[lastPos] & 0x80000000) == 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Overflow in ++."));
            }
            return result;
        }

        public static BInteger operator -(BInteger bi1, BInteger bi2)
        {
            BInteger result = new BInteger();

            result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            long carryIn = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long diff;

                diff = bi1.data[i] - (long)bi2.data[i] - carryIn;
                result.data[i] = (uint)(diff & 0xFFFFFFFF);

                if (diff < 0)
                    carryIn = 1;
                else
                    carryIn = 0;
            }

            if (carryIn != 0)
            {
                for (int i = result.dataLength; i < maxLength; i++)
                    result.data[i] = 0xFFFFFFFF;
                result.dataLength = maxLength;
            }
            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }

        public static BInteger operator --(BInteger bi1)
        {
            BInteger result = new BInteger(bi1);

            long val;
            bool carryIn = true;
            int index = 0;

            while (carryIn && index < maxLength)
            {
                val = result.data[index];
                val--;

                result.data[index] = (uint)(val & 0xFFFFFFFF);

                if (val >= 0)
                    carryIn = false;

                index++;
            }

            if (index > result.dataLength)
                result.dataLength = index;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            int lastPos = maxLength - 1;


            if ((bi1.data[lastPos] & 0x80000000) != 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Underflow in --."));
            }

            return result;
        }


        public static BInteger operator *(BInteger bi1, BInteger bi2)
        {
            int lastPos = maxLength - 1;
            bool bi1Neg = false, bi2Neg = false;
            try
            {
                if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
                {
                    bi1Neg = true; bi1 = -bi1;
                }
                if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
                {
                    bi2Neg = true; bi2 = -bi2;
                }
            }
            catch (Exception) { }

            BInteger result = new BInteger();
            try
            {
                for (int i = 0; i < bi1.dataLength; i++)
                {
                    if (bi1.data[i] == 0) continue;

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
                    {
                        // k = i + j
                        ulong val = bi1.data[i] * (ulong)bi2.data[j] +
                                     result.data[k] + mcarry;

                        result.data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = (val >> 32);
                    }

                    if (mcarry != 0)
                        result.data[i + bi2.dataLength] = (uint)mcarry;
                }
            }
            catch (Exception)
            {
                throw (new ArithmeticException("Multiplication overflow."));
            }


            result.dataLength = bi1.dataLength + bi2.dataLength;
            if (result.dataLength > maxLength)
                result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            if ((result.data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)    // different sign
                {

                    if (result.dataLength == 1)
                        return result;
                    else
                    {
                        bool isMaxNeg = true;
                        for (int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
                        {
                            if (result.data[i] != 0)
                                isMaxNeg = false;
                        }

                        if (isMaxNeg)
                            return result;
                    }
                }

                throw (new ArithmeticException("Multiplication overflow."));
            }

            if (bi1Neg != bi2Neg)
                return -result;

            return result;
        }




        public static BInteger operator <<(BInteger bi1, int shiftVal)
        {
            BInteger result = new BInteger(bi1);
            result.dataLength = shiftLeft(result.data, shiftVal);

            return result;
        }


        private static int shiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                    shiftAmount = count;

                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }
                count -= shiftAmount;
            }
            return bufLen;
        }



        public static BInteger operator >>(BInteger bi1, int shiftVal)
        {
            BInteger result = new BInteger(bi1);
            result.dataLength = shiftRight(result.data, shiftVal);


            if ((bi1.data[maxLength - 1] & 0x80000000) != 0) // negative
            {
                for (int i = maxLength - 1; i >= result.dataLength; i--)
                    result.data[i] = 0xFFFFFFFF;

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data[result.dataLength - 1] & mask) != 0)
                        break;

                    result.data[result.dataLength - 1] |= mask;
                    mask >>= 1;
                }
                result.dataLength = maxLength;
            }

            return result;
        }


        private static int shiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;


            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong)buffer[i]) << invShift;
                    buffer[i] = (uint)(val);
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            return bufLen;
        }


        public static BInteger operator ~(BInteger bi1)
        {
            BInteger result = new BInteger(bi1);

            for (int i = 0; i < maxLength; i++)
                result.data[i] = ~(bi1.data[i]);

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        public static BInteger operator -(BInteger bi1)
        {
            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.

            if (bi1.dataLength == 1 && bi1.data[0] == 0)
                return (new BInteger());

            BInteger result = new BInteger(bi1);

            // 1's complement
            for (int i = 0; i < maxLength; i++)
                result.data[i] = ~(bi1.data[i]);

            // add one to result of 1's complement
            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < maxLength)
            {
                val = result.data[index];
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1.data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
                throw (new ArithmeticException("Overflow in negation.\n"));

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;
            return result;
        }


        public static bool operator ==(BInteger bi1, BInteger bi2)
        {
            return bi1.Equals(bi2);
        }


        public static bool operator !=(BInteger bi1, BInteger bi2)
        {
            return !(bi1.Equals(bi2));
        }


        public override bool Equals(object o)
        {
            BInteger bi = (BInteger)o;

            if (this.dataLength != bi.dataLength)
                return false;

            for (int i = 0; i < this.dataLength; i++)
            {
                if (this.data[i] != bi.data[i])
                    return false;
            }
            return true;
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }



        public static bool operator >(BInteger bi1, BInteger bi2)
        {
            int pos = maxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return false;

            // bi1 is positive, bi2 is negative
            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return true;

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] > bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }


        public static bool operator <(BInteger bi1, BInteger bi2)
        {
            int pos = maxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return true;

            // bi1 is positive, bi2 is negative
            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return false;

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] < bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }


        public static bool operator >=(BInteger bi1, BInteger bi2)
        {
            return (bi1 == bi2 || bi1 > bi2);
        }


        public static bool operator <=(BInteger bi1, BInteger bi2)
        {
            return (bi1 == bi2 || bi1 < bi2);
        }



        private static void multiByteDivide(BInteger bi1, BInteger bi2,
                                            BInteger outQuotient, BInteger outRemainder)
        {
            uint[] result = new uint[maxLength];

            int remainderLen = bi1.dataLength + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2.dataLength - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++; mask >>= 1;
            }

            for (int i = 0; i < bi1.dataLength; i++)
                remainder[i] = bi1.data[i];
            shiftLeft(remainder, shift);
            bi2 = bi2 << shift;


            int j = remainderLen - bi2.dataLength;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2.dataLength - 1];
            ulong secondDivisorByte = bi2.data[bi2.dataLength - 2];

            int divisorLen = bi2.dataLength + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];
                //Console.WriteLine("dividend = {0}", dividend);

                ulong q_hat = dividend / firstDivisorByte;
                ulong r_hat = dividend % firstDivisorByte;

                //Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (q_hat == 0x100000000 ||
                       (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
                    {
                        q_hat--;
                        r_hat += firstDivisorByte;

                        if (r_hat < 0x100000000)
                            done = false;
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[pos - h];

                BInteger kk = new BInteger(dividendPart);
                BInteger ss = bi2 * (long)q_hat;

                //Console.WriteLine("ss before = " + ss);
                while (ss > kk)
                {
                    q_hat--;
                    ss -= bi2;
                    //Console.WriteLine(ss);
                }
                BInteger yy = kk - ss;

                for (int h = 0; h < divisorLen; h++)
                    remainder[pos - h] = yy.data[bi2.dataLength - h];

                result[resultPos++] = (uint)q_hat;

                pos--;
                j--;
            }

            outQuotient.dataLength = resultPos;
            int y = 0;
            for (int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
                outQuotient.data[y] = result[x];
            for (; y < maxLength; y++)
                outQuotient.data[y] = 0;

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
                outQuotient.dataLength--;

            if (outQuotient.dataLength == 0)
                outQuotient.dataLength = 1;

            outRemainder.dataLength = shiftRight(remainder, shift);

            for (y = 0; y < outRemainder.dataLength; y++)
                outRemainder.data[y] = remainder[y];
            for (; y < maxLength; y++)
                outRemainder.data[y] = 0;
        }



        private static void singleByteDivide(BInteger bi1, BInteger bi2,
                                             BInteger outQuotient, BInteger outRemainder)
        {
            uint[] result = new uint[maxLength];
            int resultPos = 0;

            // copy dividend to reminder
            for (int i = 0; i < maxLength; i++)
                outRemainder.data[i] = bi1.data[i];
            outRemainder.dataLength = bi1.dataLength;

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
                outRemainder.dataLength--;

            ulong divisor = bi2.data[0];
            int pos = outRemainder.dataLength - 1;
            ulong dividend = outRemainder.data[pos];

            //Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
            //Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }
            pos--;

            while (pos >= 0)
            {
                //Console.WriteLine(pos);

                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
                //Console.WriteLine(">>>> " + bi1);
            }

            outQuotient.dataLength = resultPos;
            int j = 0;
            for (int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
                outQuotient.data[j] = result[i];
            for (; j < maxLength; j++)
                outQuotient.data[j] = 0;

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
                outQuotient.dataLength--;

            if (outQuotient.dataLength == 0)
                outQuotient.dataLength = 1;

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
                outRemainder.dataLength--;
        }


        public static BInteger operator /(BInteger bi1, BInteger bi2)
        {
            BInteger quotient = new BInteger();
            BInteger remainder = new BInteger();

            int lastPos = maxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            else
            {
                if (bi2.dataLength == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg != divisorNeg)
                    return -quotient;

                return quotient;
            }
        }

        public static BInteger operator %(BInteger bi1, BInteger bi2)
        {
            BInteger quotient = new BInteger();
            BInteger remainder = new BInteger(bi1);

            int lastPos = maxLength - 1;
            bool dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
                bi2 = -bi2;

            if (bi1 < bi2)
            {
                return remainder;
            }

            else
            {
                if (bi2.dataLength == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg)
                    return -remainder;

                return remainder;
            }
        }


        public static BInteger operator &(BInteger bi1, BInteger bi2)
        {
            BInteger result = new BInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] & bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        public static BInteger operator |(BInteger bi1, BInteger bi2)
        {
            BInteger result = new BInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] | bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }

        public static BInteger operator ^(BInteger bi1, BInteger bi2)
        {
            BInteger result = new BInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = bi1.data[i] ^ bi2.data[i];
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        public BInteger max(BInteger bi)
        {
            if (this > bi)
                return (new BInteger(this));
            else
                return (new BInteger(bi));
        }



        public BInteger min(BInteger bi)
        {
            if (this < bi)
                return (new BInteger(this));
            else
                return (new BInteger(bi));

        }



        public BInteger abs()
        {
            if ((this.data[maxLength - 1] & 0x80000000) != 0)
                return (-this);
            else
                return (new BInteger(this));
        }


        public override string ToString()
        {
            return ToString(10);
        }


        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw (new ArgumentException("Radix must be >= 2 and <= 36"));

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";

            BInteger a = this;

            bool negative = false;
            if ((a.data[maxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception) { }
            }

            BInteger quotient = new BInteger();
            BInteger remainder = new BInteger();
            BInteger biRadix = new BInteger(radix);

            if (a.dataLength == 1 && a.data[0] == 0)
                result = "0";
            else
            {
                while (a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
                {
                    singleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder.data[0] < 10)
                        result = remainder.data[0] + result;
                    else
                        result = charSet[(int)remainder.data[0] - 10] + result;

                    a = quotient;
                }
                if (negative)
                    result = "-" + result;
            }

            return result;
        }


        public string ToHexString()
        {
            string result = data[dataLength - 1].ToString("X");

            for (int i = dataLength - 2; i >= 0; i--)
            {
                result += data[i].ToString("X8");
            }

            return result;
        }


        public BInteger modPow(BInteger exp, BInteger n)
        {
            if ((exp.data[maxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive exponents only."));

            BInteger resultNum = 1;
            BInteger tempNum;
            bool thisNegative = false;

            if ((this.data[maxLength - 1] & 0x80000000) != 0)   // negative this
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
                tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)

            if ((n.data[maxLength - 1] & 0x80000000) != 0)   // negative n
                n = -n;

            // calculate constant = b^(2k) / m
            BInteger constant = new BInteger();

            int i = n.dataLength << 1;
            constant.data[i] = 0x00000001;
            constant.dataLength = i + 1;

            constant = constant / n;
            int totalBits = exp.bitCount();
            int count = 0;

            // perform squaring and multiply exponentiation
            for (int pos = 0; pos < exp.dataLength; pos++)
            {
                uint mask = 0x01;
                //Console.WriteLine("pos = " + pos);

                for (int index = 0; index < 32; index++)
                {
                    if ((exp.data[pos] & mask) != 0)
                        resultNum = BarrettReduction(resultNum * tempNum, n, constant);

                    mask <<= 1;

                    tempNum = BarrettReduction(tempNum * tempNum, n, constant);


                    if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
                    {
                        if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
                            return -resultNum;
                        return resultNum;
                    }
                    count++;
                    if (count == totalBits)
                        break;
                }
            }

            if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
                return -resultNum;

            return resultNum;
        }


        private BInteger BarrettReduction(BInteger x, BInteger n, BInteger constant)
        {
            int k = n.dataLength,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            BInteger q1 = new BInteger();

            // q1 = x / b^(k-1)
            for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
                q1.data[j] = x.data[i];
            q1.dataLength = x.dataLength - kMinusOne;
            if (q1.dataLength <= 0)
                q1.dataLength = 1;


            BInteger q2 = q1 * constant;
            BInteger q3 = new BInteger();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
                q3.data[j] = q2.data[i];
            q3.dataLength = q2.dataLength - kPlusOne;
            if (q3.dataLength <= 0)
                q3.dataLength = 1;


            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            BInteger r1 = new BInteger();
            int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
            for (int i = 0; i < lengthToCopy; i++)
                r1.data[i] = x.data[i];
            r1.dataLength = lengthToCopy;


            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n

            BInteger r2 = new BInteger();
            for (int i = 0; i < q3.dataLength; i++)
            {
                if (q3.data[i] == 0) continue;

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
                {
                    // t = i + j
                    ulong val = q3.data[i] * (ulong)n.data[j] +
                                 r2.data[t] + mcarry;

                    r2.data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }

                if (t < kPlusOne)
                    r2.data[t] = (uint)mcarry;
            }
            r2.dataLength = kPlusOne;
            while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
                r2.dataLength--;

            r1 -= r2;
            if ((r1.data[maxLength - 1] & 0x80000000) != 0)        // negative
            {
                BInteger val = new BInteger();
                val.data[kPlusOne] = 0x00000001;
                val.dataLength = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }

        public BInteger gcd(BInteger bi)
        {
            BInteger x;
            BInteger y;

            if ((data[maxLength - 1] & 0x80000000) != 0)     // negative
                x = -this;
            else
                x = this;

            if ((bi.data[maxLength - 1] & 0x80000000) != 0)     // negative
                y = -bi;
            else
                y = bi;

            BInteger g = y;

            while (x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }

        public void genRandomBits(int bits, Random rand)
        {
            int dwords = bits >> 5;
            int remBits = bits & 0x1F;

            if (remBits != 0)
                dwords++;

            if (dwords > maxLength)
                throw (new ArithmeticException("Number of required bits > maxLength."));

            for (int i = 0; i < dwords; i++)
                data[i] = (uint)(rand.NextDouble() * 0x100000000);

            for (int i = dwords; i < maxLength; i++)
                data[i] = 0;

            if (remBits != 0)
            {
                uint mask = (uint)(0x01 << (remBits - 1));
                data[dwords - 1] |= mask;

                mask = 0xFFFFFFFF >> (32 - remBits);
                data[dwords - 1] &= mask;
            }
            else
                data[dwords - 1] |= 0x80000000;

            dataLength = dwords;

            if (dataLength == 0)
                dataLength = 1;
        }

        public int bitCount()
        {
            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;

            uint value = data[dataLength - 1];
            uint mask = 0x80000000;
            int bits = 32;

            while (bits > 0 && (value & mask) == 0)
            {
                bits--;
                mask >>= 1;
            }
            bits += ((dataLength - 1) << 5);

            return bits;
        }



        public bool FermatLittleTest(int confidence)
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;

            int bits = thisVal.bitCount();
            BInteger a = new BInteger();
            BInteger p_sub1 = thisVal - (new BInteger(1));
            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.genRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                BInteger gcdTest = a.gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                // calculate a^(p-1) mod p
                BInteger expResult = a.modPow(p_sub1, thisVal);

                int resultLen = expResult.dataLength;

                // is NOT prime is a^(p-1) mod p != 1

                if (resultLen > 1 || (resultLen == 1 && expResult.data[0] != 1))
                {
                    //Console.WriteLine("a = " + a.ToString());
                    return false;
                }
            }

            return true;
        }


        public bool RabinMillerTest(int confidence)
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;


            // calculate values of s and t
            BInteger p_sub1 = thisVal - (new BInteger(1));
            int s = 0;

            for (int index = 0; index < p_sub1.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_sub1.data[index] & mask) != 0)
                    {
                        index = p_sub1.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            BInteger t = p_sub1 >> s;

            int bits = thisVal.bitCount();
            BInteger a = new BInteger();
            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.genRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                BInteger gcdTest = a.gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                BInteger b = a.modPow(t, thisVal);


                bool result = false;

                if (b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
                    result = true;

                for (int j = 0; result == false && j < s; j++)
                {
                    if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                    {
                        result = true;
                        break;
                    }

                    b = (b * b) % thisVal;
                }

                if (result == false)
                    return false;
            }
            return true;
        }



        public bool SolovayStrassenTest(int confidence)
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;


            int bits = thisVal.bitCount();
            BInteger a = new BInteger();
            BInteger p_sub1 = thisVal - 1;
            BInteger p_sub1_shift = p_sub1 >> 1;

            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.genRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                BInteger gcdTest = a.gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                // calculate a^((p-1)/2) mod p

                BInteger expResult = a.modPow(p_sub1_shift, thisVal);
                if (expResult == p_sub1)
                    expResult = -1;

                // calculate Jacobi symbol
                BInteger jacob = Jacobi(a, thisVal);

                //Console.WriteLine("a = " + a.ToString(10) + " b = " + thisVal.ToString(10));
                //Console.WriteLine("expResult = " + expResult.ToString(10) + " Jacob = " + jacob.ToString(10));

                // if they are different then it is not prime
                if (expResult != jacob)
                    return false;
            }

            return true;
        }


        public bool LucasStrongTest()
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;

            return LucasStrongTestHelper(thisVal);
        }


        private bool LucasStrongTestHelper(BInteger thisVal)
        {

            long D = 5, sign = -1, dCount = 0;
            bool done = false;

            while (!done)
            {
                int Jresult = BInteger.Jacobi(D, thisVal);

                if (Jresult == -1)
                    done = true;    // J(D, this) = 1
                else
                {
                    if (Jresult == 0 && Math.Abs(D) < thisVal)       // divisor found
                        return false;

                    if (dCount == 20)
                    {
                        // check for square
                        BInteger root = thisVal.sqrt();
                        if (root * root == thisVal)
                            return false;
                    }

                    //Console.WriteLine(D);
                    D = (Math.Abs(D) + 2) * sign;
                    sign = -sign;
                }
                dCount++;
            }

            long Q = (1 - D) >> 2;

            BInteger p_add1 = thisVal + 1;
            int s = 0;

            for (int index = 0; index < p_add1.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_add1.data[index] & mask) != 0)
                    {
                        index = p_add1.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            BInteger t = p_add1 >> s;

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            BInteger constant = new BInteger();

            int nLen = thisVal.dataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.dataLength = nLen + 1;

            constant = constant / thisVal;

            BInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
            bool isPrime = false;

            if ((lucas[0].dataLength == 1 && lucas[0].data[0] == 0) ||
               (lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
            {
                // u(t) = 0 or V(t) = 0
                isPrime = true;
            }

            for (int i = 1; i < s; i++)
            {
                if (!isPrime)
                {
                    // doubling of index
                    lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
                    lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

                    //lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;

                    if ((lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
                        isPrime = true;
                }

                lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
            }


            if (isPrime)     // additional checks for composite numbers
            {
                // If n is prime and gcd(n, Q) == 1, then
                // Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

                BInteger g = thisVal.gcd(Q);
                if (g.dataLength == 1 && g.data[0] == 1)         // gcd(this, Q) == 1
                {
                    if ((lucas[2].data[maxLength - 1] & 0x80000000) != 0)
                        lucas[2] += thisVal;

                    BInteger temp = (Q * BInteger.Jacobi(Q, thisVal)) % thisVal;
                    if ((temp.data[maxLength - 1] & 0x80000000) != 0)
                        temp += thisVal;

                    if (lucas[2] != temp)
                        isPrime = false;
                }
            }

            return isPrime;
        }

        public bool isProbablePrime(int confidence)
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;


            // test for divisibility by primes < 2000
            for (int p = 0; p < primesBelow2000.Length; p++)
            {
                BInteger divisor = primesBelow2000[p];

                if (divisor >= thisVal)
                    break;

                BInteger resultNum = thisVal % divisor;
                if (resultNum.IntValue() == 0)
                {
                    /*
    Console.WriteLine("Not prime!  Divisible by {0}\n",
                                      primesBelow2000[p]);
                    */
                    return false;
                }
            }

            if (thisVal.RabinMillerTest(confidence))
                return true;
            else
            {
                //Console.WriteLine("Not prime!  Failed primality test\n");
                return false;
            }
        }


        public bool isProbablePrime()
        {
            BInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;


            // test for divisibility by primes < 2000
            for (int p = 0; p < primesBelow2000.Length; p++)
            {
                BInteger divisor = primesBelow2000[p];

                if (divisor >= thisVal)
                    break;

                BInteger resultNum = thisVal % divisor;
                if (resultNum.IntValue() == 0)
                {
                    //Console.WriteLine("Not prime!  Divisible by {0}\n",
                    //                  primesBelow2000[p]);

                    return false;
                }
            }

            // Perform BASE 2 Rabin-Miller Test

            // calculate values of s and t
            BInteger p_sub1 = thisVal - (new BInteger(1));
            int s = 0;

            for (int index = 0; index < p_sub1.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_sub1.data[index] & mask) != 0)
                    {
                        index = p_sub1.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            BInteger t = p_sub1 >> s;

            int bits = thisVal.bitCount();
            BInteger a = 2;

            // b = a^t mod p
            BInteger b = a.modPow(t, thisVal);
            bool result = false;

            if (b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
                result = true;

            for (int j = 0; result == false && j < s; j++)
            {
                if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                {
                    result = true;
                    break;
                }

                b = (b * b) % thisVal;
            }

            // if number is strong pseudoprime to base 2, then do a strong lucas test
            if (result)
                result = LucasStrongTestHelper(thisVal);

            return result;
        }



        public int IntValue()
        {
            return (int)data[0];
        }


        public long LongValue()
        {
            long val = 0;

            val = data[0];
            try
            {       // exception if maxLength = 1
                val |= (long)data[1] << 32;
            }
            catch (Exception)
            {
                if ((data[0] & 0x80000000) != 0) // negative
                    val = data[0];
            }

            return val;
        }


        public static int Jacobi(BInteger a, BInteger b)
        {
            // Jacobi defined only for odd integers
            if ((b.data[0] & 0x1) == 0)
                throw (new ArgumentException("Jacobi defined only for odd integers."));

            if (a >= b) a %= b;
            if (a.dataLength == 1 && a.data[0] == 0) return 0;  // a == 0
            if (a.dataLength == 1 && a.data[0] == 1) return 1;  // a == 1

            if (a < 0)
            {
                if ((((b - 1).data[0]) & 0x2) == 0)       //if( (((b-1) >> 1).data[0] & 0x1) == 0)
                    return Jacobi(-a, b);
                else
                    return -Jacobi(-a, b);
            }

            int e = 0;
            for (int index = 0; index < a.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((a.data[index] & mask) != 0)
                    {
                        index = a.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    e++;
                }
            }

            BInteger a1 = a >> e;

            int s = 1;
            if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
                s = -1;

            if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
                s = -s;

            if (a1.dataLength == 1 && a1.data[0] == 1)
                return s;
            else
                return (s * Jacobi(b % a1, a1));
        }


        public static BInteger genPseudoPrime(int bits, int confidence, Random rand)
        {
            BInteger result = new BInteger();
            bool done = false;

            while (!done)
            {
                result.genRandomBits(bits, rand);
                result.data[0] |= 0x01;     // make it odd

                // prime test
                done = result.isProbablePrime(confidence);
            }
            return result;
        }

        public BInteger genCoPrime(int bits, Random rand)
        {
            bool done = false;
            BInteger result = new BInteger();

            while (!done)
            {
                result.genRandomBits(bits, rand);
                //Console.WriteLine(result.ToString(16));

                // gcd test
                BInteger g = result.gcd(this);
                if (g.dataLength == 1 && g.data[0] == 1)
                    done = true;
            }

            return result;
        }


        public BInteger modInverse(BInteger modulus)
        {
            BInteger[] p = { 0, 1 };
            BInteger[] q = new BInteger[2];    // quotients
            BInteger[] r = { 0, 0 };             // remainders

            int step = 0;

            BInteger a = modulus;
            BInteger b = this;

            while (b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
            {
                BInteger quotient = new BInteger();
                BInteger remainder = new BInteger();

                if (step > 1)
                {
                    BInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b.dataLength == 1)
                    singleByteDivide(a, b, quotient, remainder);
                else
                    multiByteDivide(a, b, quotient, remainder);

                /*
                Console.WriteLine(quotient.dataLength);
                Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
                                  b.ToString(10), quotient.ToString(10), remainder.ToString(10),
                                  p[1].ToString(10));
                */

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient; r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }

            if (r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
                throw (new ArithmeticException("No inverse!"));

            BInteger result = ((p[0] - (p[1] * q[0])) % modulus);

            if ((result.data[maxLength - 1] & 0x80000000) != 0)
                result += modulus;  // get the least positive modulus

            return result;
        }


        public byte[] getBytes()
        {
            int numBits = bitCount();

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
                numBytes++;

            byte[] result = new byte[numBytes];

            //Console.WriteLine(result.Length);

            int pos = 0;
            uint tempVal, val = data[dataLength - 1];

            if ((tempVal = (val >> 24 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val >> 16 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val >> 8 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;

            for (int i = dataLength - 2; i >= 0; i--, pos += 4)
            {
                val = data[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }



        public void setBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;             // divide by 32
            byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

            uint mask = (uint)1 << bitPos;
            this.data[bytePos] |= mask;

            if (bytePos >= this.dataLength)
                this.dataLength = (int)bytePos + 1;
        }



        public void unsetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;

            if (bytePos < this.dataLength)
            {
                byte bitPos = (byte)(bitNum & 0x1F);

                uint mask = (uint)1 << bitPos;
                uint mask2 = 0xFFFFFFFF ^ mask;

                this.data[bytePos] &= mask2;

                if (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
                    this.dataLength--;
            }
        }


        public BInteger sqrt()
        {
            uint numBits = (uint)this.bitCount();

            if ((numBits & 0x1) != 0)        // odd number of bits
                numBits = (numBits >> 1) + 1;
            else
                numBits = (numBits >> 1);

            uint bytePos = numBits >> 5;
            byte bitPos = (byte)(numBits & 0x1F);

            uint mask;

            BInteger result = new BInteger();
            if (bitPos == 0)
                mask = 0x80000000;
            else
            {
                mask = (uint)1 << bitPos;
                bytePos++;
            }
            result.dataLength = (int)bytePos;

            for (int i = (int)bytePos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // guess
                    result.data[i] ^= mask;

                    // undo the guess if its square is larger than this
                    if ((result * result) > this)
                        result.data[i] ^= mask;

                    mask >>= 1;
                }
                mask = 0x80000000;
            }
            return result;
        }


        public static BInteger[] LucasSequence(BInteger P, BInteger Q,
                                                 BInteger k, BInteger n)
        {
            if (k.dataLength == 1 && k.data[0] == 0)
            {
                BInteger[] result = new BInteger[3];

                result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
                return result;
            }

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            BInteger constant = new BInteger();

            int nLen = n.dataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.dataLength = nLen + 1;

            constant = constant / n;

            // calculate values of s and t
            int s = 0;

            for (int index = 0; index < k.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((k.data[index] & mask) != 0)
                    {
                        index = k.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            BInteger t = k >> s;

            //Console.WriteLine("s = " + s + " t = " + t);
            return LucasSequenceHelper(P, Q, t, n, constant, s);
        }


        private static BInteger[] LucasSequenceHelper(BInteger P, BInteger Q,
                                                        BInteger k, BInteger n,
                                                        BInteger constant, int s)
        {
            BInteger[] result = new BInteger[3];

            if ((k.data[0] & 0x00000001) == 0)
                throw (new ArgumentException("Argument k must be odd."));

            int numbits = k.bitCount();
            uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

            // v = v0, v1 = v1, u1 = u1, Q_k = Q^0

            BInteger v = 2 % n, Q_k = 1 % n,
                       v1 = P % n, u1 = Q_k;
            bool flag = true;

            for (int i = k.dataLength - 1; i >= 0; i--)     // iterate on the binary expansion of k
            {
                //Console.WriteLine("round");
                while (mask != 0)
                {
                    if (i == 0 && mask == 0x00000001)        // last bit
                        break;

                    if ((k.data[i] & mask) != 0)             // bit is set
                    {
                        // index doubling with addition

                        u1 = (u1 * v1) % n;

                        v = ((v * v1) - (P * Q_k)) % n;
                        v1 = n.BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((Q_k * Q) << 1)) % n;

                        if (flag)
                            flag = false;
                        else
                            Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

                        Q_k = (Q_k * Q) % n;
                    }
                    else
                    {
                        // index doubling
                        u1 = ((u1 * v) - Q_k) % n;

                        v1 = ((v * v1) - (P * Q_k)) % n;
                        v = n.BarrettReduction(v * v, n, constant);
                        v = (v - (Q_k << 1)) % n;

                        if (flag)
                        {
                            Q_k = Q % n;
                            flag = false;
                        }
                        else
                            Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
                    }

                    mask >>= 1;
                }
                mask = 0x80000000;
            }

            // at this point u1 = u(n+1) and v = v(n)
            // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

            u1 = ((u1 * v) - Q_k) % n;
            v = ((v * v1) - (P * Q_k)) % n;
            if (flag)
                flag = false;
            else
                Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

            Q_k = (Q_k * Q) % n;


            for (int i = 0; i < s; i++)
            {
                // index doubling
                u1 = (u1 * v) % n;
                v = ((v * v) - (Q_k << 1)) % n;

                if (flag)
                {
                    Q_k = Q % n;
                    flag = false;
                }
                else
                    Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
            }

            result[0] = u1;
            result[1] = v;
            result[2] = Q_k;

            return result;
        }

        public static void MulDivTest(int rounds)
        {
            Random rand = new Random();
            byte[] val = new byte[64];
            byte[] val2 = new byte[64];

            for (int count = 0; count < rounds; count++)
            {
                // generate 2 numbers of random length
                int t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                int t2 = 0;
                while (t2 == 0)
                    t2 = (int)(rand.NextDouble() * 65);

                bool done = false;
                while (!done)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (i < t1)
                            val[i] = (byte)(rand.NextDouble() * 256);
                        else
                            val[i] = 0;

                        if (val[i] != 0)
                            done = true;
                    }
                }

                done = false;
                while (!done)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (i < t2)
                            val2[i] = (byte)(rand.NextDouble() * 256);
                        else
                            val2[i] = 0;

                        if (val2[i] != 0)
                            done = true;
                    }
                }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);
                while (val2[0] == 0)
                    val2[0] = (byte)(rand.NextDouble() * 256);

                Console.WriteLine(count);
                BInteger bn1 = new BInteger(val, t1);
                BInteger bn2 = new BInteger(val2, t2);


                // Determine the quotient and remainder by dividing
                // the first number by the second.

                BInteger bn3 = bn1 / bn2;
                BInteger bn4 = bn1 % bn2;

                // Recalculate the number
                BInteger bn5 = (bn3 * bn2) + bn4;

                // Make sure they're the same
                if (bn5 != bn1)
                {
                    Console.WriteLine("Error at " + count);
                    Console.WriteLine(bn1 + "\n");
                    Console.WriteLine(bn2 + "\n");
                    Console.WriteLine(bn3 + "\n");
                    Console.WriteLine(bn4 + "\n");
                    Console.WriteLine(bn5 + "\n");
                    return;
                }
            }
        }


        public static void RSATest(int rounds)
        {
            Random rand = new Random(1);
            byte[] val = new byte[64];

            // private and public key
            BInteger bi_e = new BInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7", 16);
            BInteger bi_d = new BInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7", 16);
            BInteger bi_n = new BInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f", 16);

            Console.WriteLine("e =\n" + bi_e.ToString(10));
            Console.WriteLine("\nd =\n" + bi_d.ToString(10));
            Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

            for (int count = 0; count < rounds; count++)
            {
                // generate data of random length
                int t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                bool done = false;
                while (!done)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (i < t1)
                            val[i] = (byte)(rand.NextDouble() * 256);
                        else
                            val[i] = 0;

                        if (val[i] != 0)
                            done = true;
                    }
                }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);

                Console.Write("Round = " + count);

                // encrypt and decrypt data
                BInteger bi_data = new BInteger(val, t1);
                BInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
                BInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

                // compare
                if (bi_decrypted != bi_data)
                {
                    Console.WriteLine("\nError at round " + count);
                    Console.WriteLine(bi_data + "\n");
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }

        }


        public static void RSATest2(int rounds)
        {
            Random rand = new Random();
            byte[] val = new byte[64];

            byte[] pseudoPrime1 = {
                        0x85, 0x84, 0x64, 0xFD, 0x70, 0x6A,
                        0x9F, 0xF0, 0x94, 0x0C, 0x3E, 0x2C,
                        0x74, 0x34, 0x05, 0xC9, 0x55, 0xB3,
                        0x85, 0x32, 0x98, 0x71, 0xF9, 0x41,
                        0x21, 0x5F, 0x02, 0x9E, 0xEA, 0x56,
                        0x8D, 0x8C, 0x44, 0xCC, 0xEE, 0xEE,
                        0x3D, 0x2C, 0x9D, 0x2C, 0x12, 0x41,
                        0x1E, 0xF1, 0xC5, 0x32, 0xC3, 0xAA,
                        0x31, 0x4A, 0x52, 0xD8, 0xE8, 0xAF,
                        0x42, 0xF4, 0x72, 0xA1, 0x2A, 0x0D,
                        0x97, 0xB1, 0x31, 0xB3,
                };

            byte[] pseudoPrime2 = {
                        0x99, 0x98, 0xCA, 0xB8, 0x5E, 0xD7,
                        0xE5, 0xDC, 0x28, 0x5C, 0x6F, 0x0E,
                        0x15, 0x09, 0x59, 0x6E, 0x84, 0xF3,
                        0x81, 0xCD, 0xDE, 0x42, 0xDC, 0x93,
                        0xC2, 0x7A, 0x62, 0xAC, 0x6C, 0xAF,
                        0xDE, 0x74, 0xE3, 0xCB, 0x60, 0x20,
                        0x38, 0x9C, 0x21, 0xC3, 0xDC, 0xC8,
                        0xA2, 0x4D, 0xC6, 0x2A, 0x35, 0x7F,
                        0xF3, 0xA9, 0xE8, 0x1D, 0x7B, 0x2C,
                        0x78, 0xFA, 0xB8, 0x02, 0x55, 0x80,
                        0x9B, 0xC2, 0xA5, 0xCB,
                };


            BInteger bi_p = new BInteger(pseudoPrime1);
            BInteger bi_q = new BInteger(pseudoPrime2);
            BInteger bi_pq = (bi_p - 1) * (bi_q - 1);
            BInteger bi_n = bi_p * bi_q;

            for (int count = 0; count < rounds; count++)
            {
                // generate private and public key
                BInteger bi_e = bi_pq.genCoPrime(512, rand);
                BInteger bi_d = bi_e.modInverse(bi_pq);

                Console.WriteLine("\ne =\n" + bi_e.ToString(10));
                Console.WriteLine("\nd =\n" + bi_d.ToString(10));
                Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

                // generate data of random length
                int t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 65);

                bool done = false;
                while (!done)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (i < t1)
                            val[i] = (byte)(rand.NextDouble() * 256);
                        else
                            val[i] = 0;

                        if (val[i] != 0)
                            done = true;
                    }
                }

                while (val[0] == 0)
                    val[0] = (byte)(rand.NextDouble() * 256);

                Console.Write("Round = " + count);

                // encrypt and decrypt data
                BInteger bi_data = new BInteger(val, t1);
                BInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
                BInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

                // compare
                if (bi_decrypted != bi_data)
                {
                    Console.WriteLine("\nError at round " + count);
                    Console.WriteLine(bi_data + "\n");
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }

        }

        public static void SqrtTest(int rounds)
        {
            Random rand = new Random();
            for (int count = 0; count < rounds; count++)
            {
                // generate data of random length
                int t1 = 0;
                while (t1 == 0)
                    t1 = (int)(rand.NextDouble() * 1024);

                Console.Write("Round = " + count);

                BInteger a = new BInteger();
                a.genRandomBits(t1, rand);

                BInteger b = a.sqrt();
                BInteger c = (b + 1) * (b + 1);

                // check that b is the largest integer such that b*b <= a
                if (c <= a)
                {
                    Console.WriteLine("\nError at round " + count);
                    Console.WriteLine(a + "\n");
                    return;
                }
                Console.WriteLine(" <PASSED>.");
            }
        }
    }
    public class ECPoint
    {
        public BInteger x;
        public BInteger y;
        public BInteger a;
        public BInteger b;
        public BInteger FieldChar;

        public ECPoint(ECPoint p)
        {
            x = p.x;
            y = p.y;
            a = p.a;
            b = p.b;
            FieldChar = p.FieldChar;
        }

        public ECPoint()
        {
            x = new BInteger();
            y = new BInteger();
            a = new BInteger();
            b = new BInteger();
            FieldChar = new BInteger();
        }
        //сложение двух точек P1 и P2
        public static ECPoint operator +(ECPoint p1, ECPoint p2)
        {
            ECPoint p3 = new ECPoint();
            p3.a = p1.a;
            p3.b = p1.b;
            p3.FieldChar = p1.FieldChar;

            BInteger dy = p2.y - p1.y;
            BInteger dx = p2.x - p1.x;

            if (dx < 0)
                dx += p1.FieldChar;
            if (dy < 0)
                dy += p1.FieldChar;

            BInteger m = (dy * dx.modInverse(p1.FieldChar)) % p1.FieldChar;
            if (m < 0)
                m += p1.FieldChar;
            p3.x = (m * m - p1.x - p2.x) % p1.FieldChar;
            p3.y = (m * (p1.x - p3.x) - p1.y) % p1.FieldChar;
            if (p3.x < 0)
                p3.x += p1.FieldChar;
            if (p3.y < 0)
                p3.y += p1.FieldChar;
            return p3;
        }
        //сложение точки P c собой же
        public static ECPoint Double(ECPoint p)
        {
            ECPoint p2 = new ECPoint();
            p2.a = p.a;
            p2.b = p.b;
            p2.FieldChar = p.FieldChar;

            BInteger dy = 3 * p.x * p.x + p.a;
            BInteger dx = 2 * p.y;

            if (dx < 0)
                dx += p.FieldChar;
            if (dy < 0)
                dy += p.FieldChar;

            BInteger m = (dy * dx.modInverse(p.FieldChar)) % p.FieldChar;
            p2.x = (m * m - p.x - p.x) % p.FieldChar;
            p2.y = (m * (p.x - p2.x) - p.y) % p.FieldChar;
            if (p2.x < 0)
                p2.x += p.FieldChar;
            if (p2.y < 0)
                p2.y += p.FieldChar;

            return p2;
        }
        //умножение точки на число x, по сути своей представляет x сложений точки самой с собой
        public static ECPoint multiply(BInteger x, ECPoint p)
        {
            ECPoint temp = p;
            x = x - 1;
            while (x != 0)
            {

                if ((x % 2) != 0)
                {
                    if ((temp.x == p.x) || (temp.y == p.y))
                        temp = Double(temp);
                    else
                        temp = temp + p;
                    x = x - 1;
                }
                x = x / 2;
                p = Double(p);
            }
            return temp;
        }
    }
    public class GOST
    {
        // Matrix A for MixColumns (L) function
        private ulong[] A = {
        0x8e20faa72ba0b470, 0x47107ddd9b505a38, 0xad08b0e0c3282d1c, 0xd8045870ef14980e,
        0x6c022c38f90a4c07, 0x3601161cf205268d, 0x1b8e0b0e798c13c8, 0x83478b07b2468764,
        0xa011d380818e8f40, 0x5086e740ce47c920, 0x2843fd2067adea10, 0x14aff010bdd87508,
        0x0ad97808d06cb404, 0x05e23c0468365a02, 0x8c711e02341b2d01, 0x46b60f011a83988e,
        0x90dab52a387ae76f, 0x486dd4151c3dfdb9, 0x24b86a840e90f0d2, 0x125c354207487869,
        0x092e94218d243cba, 0x8a174a9ec8121e5d, 0x4585254f64090fa0, 0xaccc9ca9328a8950,
        0x9d4df05d5f661451, 0xc0a878a0a1330aa6, 0x60543c50de970553, 0x302a1e286fc58ca7,
        0x18150f14b9ec46dd, 0x0c84890ad27623e0, 0x0642ca05693b9f70, 0x0321658cba93c138,
        0x86275df09ce8aaa8, 0x439da0784e745554, 0xafc0503c273aa42a, 0xd960281e9d1d5215,
        0xe230140fc0802984, 0x71180a8960409a42, 0xb60c05ca30204d21, 0x5b068c651810a89e,
        0x456c34887a3805b9, 0xac361a443d1c8cd2, 0x561b0d22900e4669, 0x2b838811480723ba,
        0x9bcf4486248d9f5d, 0xc3e9224312c8c1a0, 0xeffa11af0964ee50, 0xf97d86d98a327728,
        0xe4fa2054a80b329c, 0x727d102a548b194e, 0x39b008152acb8227, 0x9258048415eb419d,
        0x492c024284fbaec0, 0xaa16012142f35760, 0x550b8e9e21f7a530, 0xa48b474f9ef5dc18,
        0x70a6a56e2440598e, 0x3853dc371220a247, 0x1ca76e95091051ad, 0x0edd37c48a08a6d8,
        0x07e095624504536c, 0x8d70c431ac02a736, 0xc83862965601dd1b, 0x641c314b2b8ee083
        };

        // Substitution for SubBytes function
        private byte[] Sbox ={
        0xFC, 0xEE, 0xDD, 0x11, 0xCF, 0x6E, 0x31, 0x16, 0xFB, 0xC4, 0xFA, 0xDA, 0x23, 0xC5, 0x04, 0x4D,
        0xE9, 0x77, 0xF0, 0xDB, 0x93, 0x2E, 0x99, 0xBA, 0x17, 0x36, 0xF1, 0xBB, 0x14, 0xCD, 0x5F, 0xC1,
        0xF9, 0x18, 0x65, 0x5A, 0xE2, 0x5C, 0xEF, 0x21, 0x81, 0x1C, 0x3C, 0x42, 0x8B, 0x01, 0x8E, 0x4F,
        0x05, 0x84, 0x02, 0xAE, 0xE3, 0x6A, 0x8F, 0xA0, 0x06, 0x0B, 0xED, 0x98, 0x7F, 0xD4, 0xD3, 0x1F,
        0xEB, 0x34, 0x2C, 0x51, 0xEA, 0xC8, 0x48, 0xAB, 0xF2, 0x2A, 0x68, 0xA2, 0xFD, 0x3A, 0xCE, 0xCC,
        0xB5, 0x70, 0x0E, 0x56, 0x08, 0x0C, 0x76, 0x12, 0xBF, 0x72, 0x13, 0x47, 0x9C, 0xB7, 0x5D, 0x87,
        0x15, 0xA1, 0x96, 0x29, 0x10, 0x7B, 0x9A, 0xC7, 0xF3, 0x91, 0x78, 0x6F, 0x9D, 0x9E, 0xB2, 0xB1,
        0x32, 0x75, 0x19, 0x3D, 0xFF, 0x35, 0x8A, 0x7E, 0x6D, 0x54, 0xC6, 0x80, 0xC3, 0xBD, 0x0D, 0x57,
        0xDF, 0xF5, 0x24, 0xA9, 0x3E, 0xA8, 0x43, 0xC9, 0xD7, 0x79, 0xD6, 0xF6, 0x7C, 0x22, 0xB9, 0x03,
        0xE0, 0x0F, 0xEC, 0xDE, 0x7A, 0x94, 0xB0, 0xBC, 0xDC, 0xE8, 0x28, 0x50, 0x4E, 0x33, 0x0A, 0x4A,
        0xA7, 0x97, 0x60, 0x73, 0x1E, 0x00, 0x62, 0x44, 0x1A, 0xB8, 0x38, 0x82, 0x64, 0x9F, 0x26, 0x41,
        0xAD, 0x45, 0x46, 0x92, 0x27, 0x5E, 0x55, 0x2F, 0x8C, 0xA3, 0xA5, 0x7D, 0x69, 0xD5, 0x95, 0x3B,
        0x07, 0x58, 0xB3, 0x40, 0x86, 0xAC, 0x1D, 0xF7, 0x30, 0x37, 0x6B, 0xE4, 0x88, 0xD9, 0xE7, 0x89,
        0xE1, 0x1B, 0x83, 0x49, 0x4C, 0x3F, 0xF8, 0xFE, 0x8D, 0x53, 0xAA, 0x90, 0xCA, 0xD8, 0x85, 0x61,
        0x20, 0x71, 0x67, 0xA4, 0x2D, 0x2B, 0x09, 0x5B, 0xCB, 0x9B, 0x25, 0xD0, 0xBE, 0xE5, 0x6C, 0x52,
        0x59, 0xA6, 0x74, 0xD2, 0xE6, 0xF4, 0xB4, 0xC0, 0xD1, 0x66, 0xAF, 0xC2, 0x39, 0x4B, 0x63, 0xB6
        };

        // Substitution for Transposition (P) function
        private byte[] Tau ={
        0, 8, 16, 24, 32, 40, 48, 56,
        1, 9, 17, 25, 33, 41, 49, 57,
        2, 10, 18, 26, 34, 42, 50, 58,
        3, 11, 19, 27, 35, 43, 51, 59,
        4, 12, 20, 28, 36, 44, 52, 60,
        5, 13, 21, 29, 37, 45, 53, 61,
        6, 14, 22, 30, 38, 46, 54, 62,
        7, 15, 23, 31, 39, 47, 55, 63
        };

        // Constant values for KeySchedule function
        private byte[][] C = {
        new byte[64]{
        0xb1,0x08,0x5b,0xda,0x1e,0xca,0xda,0xe9,0xeb,0xcb,0x2f,0x81,0xc0,0x65,0x7c,0x1f,
        0x2f,0x6a,0x76,0x43,0x2e,0x45,0xd0,0x16,0x71,0x4e,0xb8,0x8d,0x75,0x85,0xc4,0xfc,
        0x4b,0x7c,0xe0,0x91,0x92,0x67,0x69,0x01,0xa2,0x42,0x2a,0x08,0xa4,0x60,0xd3,0x15,
        0x05,0x76,0x74,0x36,0xcc,0x74,0x4d,0x23,0xdd,0x80,0x65,0x59,0xf2,0xa6,0x45,0x07
        },
        new byte[64]{
        0x6f,0xa3,0xb5,0x8a,0xa9,0x9d,0x2f,0x1a,0x4f,0xe3,0x9d,0x46,0x0f,0x70,0xb5,0xd7,
        0xf3,0xfe,0xea,0x72,0x0a,0x23,0x2b,0x98,0x61,0xd5,0x5e,0x0f,0x16,0xb5,0x01,0x31,
        0x9a,0xb5,0x17,0x6b,0x12,0xd6,0x99,0x58,0x5c,0xb5,0x61,0xc2,0xdb,0x0a,0xa7,0xca,
        0x55,0xdd,0xa2,0x1b,0xd7,0xcb,0xcd,0x56,0xe6,0x79,0x04,0x70,0x21,0xb1,0x9b,0xb7
        },
        new byte[64]{
        0xf5,0x74,0xdc,0xac,0x2b,0xce,0x2f,0xc7,0x0a,0x39,0xfc,0x28,0x6a,0x3d,0x84,0x35,
        0x06,0xf1,0x5e,0x5f,0x52,0x9c,0x1f,0x8b,0xf2,0xea,0x75,0x14,0xb1,0x29,0x7b,0x7b,
        0xd3,0xe2,0x0f,0xe4,0x90,0x35,0x9e,0xb1,0xc1,0xc9,0x3a,0x37,0x60,0x62,0xdb,0x09,
        0xc2,0xb6,0xf4,0x43,0x86,0x7a,0xdb,0x31,0x99,0x1e,0x96,0xf5,0x0a,0xba,0x0a,0xb2
        },
        new byte[64]{
        0xef,0x1f,0xdf,0xb3,0xe8,0x15,0x66,0xd2,0xf9,0x48,0xe1,0xa0,0x5d,0x71,0xe4,0xdd,
        0x48,0x8e,0x85,0x7e,0x33,0x5c,0x3c,0x7d,0x9d,0x72,0x1c,0xad,0x68,0x5e,0x35,0x3f,
        0xa9,0xd7,0x2c,0x82,0xed,0x03,0xd6,0x75,0xd8,0xb7,0x13,0x33,0x93,0x52,0x03,0xbe,
        0x34,0x53,0xea,0xa1,0x93,0xe8,0x37,0xf1,0x22,0x0c,0xbe,0xbc,0x84,0xe3,0xd1,0x2e
        },
        new byte[64]{
        0x4b,0xea,0x6b,0xac,0xad,0x47,0x47,0x99,0x9a,0x3f,0x41,0x0c,0x6c,0xa9,0x23,0x63,
        0x7f,0x15,0x1c,0x1f,0x16,0x86,0x10,0x4a,0x35,0x9e,0x35,0xd7,0x80,0x0f,0xff,0xbd,
        0xbf,0xcd,0x17,0x47,0x25,0x3a,0xf5,0xa3,0xdf,0xff,0x00,0xb7,0x23,0x27,0x1a,0x16,
        0x7a,0x56,0xa2,0x7e,0xa9,0xea,0x63,0xf5,0x60,0x17,0x58,0xfd,0x7c,0x6c,0xfe,0x57
        },
        new byte[64]{
        0xae,0x4f,0xae,0xae,0x1d,0x3a,0xd3,0xd9,0x6f,0xa4,0xc3,0x3b,0x7a,0x30,0x39,0xc0,
        0x2d,0x66,0xc4,0xf9,0x51,0x42,0xa4,0x6c,0x18,0x7f,0x9a,0xb4,0x9a,0xf0,0x8e,0xc6,
        0xcf,0xfa,0xa6,0xb7,0x1c,0x9a,0xb7,0xb4,0x0a,0xf2,0x1f,0x66,0xc2,0xbe,0xc6,0xb6,
        0xbf,0x71,0xc5,0x72,0x36,0x90,0x4f,0x35,0xfa,0x68,0x40,0x7a,0x46,0x64,0x7d,0x6e
        },
        new byte[64]{
        0xf4,0xc7,0x0e,0x16,0xee,0xaa,0xc5,0xec,0x51,0xac,0x86,0xfe,0xbf,0x24,0x09,0x54,
        0x39,0x9e,0xc6,0xc7,0xe6,0xbf,0x87,0xc9,0xd3,0x47,0x3e,0x33,0x19,0x7a,0x93,0xc9,
        0x09,0x92,0xab,0xc5,0x2d,0x82,0x2c,0x37,0x06,0x47,0x69,0x83,0x28,0x4a,0x05,0x04,
        0x35,0x17,0x45,0x4c,0xa2,0x3c,0x4a,0xf3,0x88,0x86,0x56,0x4d,0x3a,0x14,0xd4,0x93
        },
        new byte[64]{
        0x9b,0x1f,0x5b,0x42,0x4d,0x93,0xc9,0xa7,0x03,0xe7,0xaa,0x02,0x0c,0x6e,0x41,0x41,
        0x4e,0xb7,0xf8,0x71,0x9c,0x36,0xde,0x1e,0x89,0xb4,0x44,0x3b,0x4d,0xdb,0xc4,0x9a,
        0xf4,0x89,0x2b,0xcb,0x92,0x9b,0x06,0x90,0x69,0xd1,0x8d,0x2b,0xd1,0xa5,0xc4,0x2f,
        0x36,0xac,0xc2,0x35,0x59,0x51,0xa8,0xd9,0xa4,0x7f,0x0d,0xd4,0xbf,0x02,0xe7,0x1e
        },
        new byte[64]{
        0x37,0x8f,0x5a,0x54,0x16,0x31,0x22,0x9b,0x94,0x4c,0x9a,0xd8,0xec,0x16,0x5f,0xde,
        0x3a,0x7d,0x3a,0x1b,0x25,0x89,0x42,0x24,0x3c,0xd9,0x55,0xb7,0xe0,0x0d,0x09,0x84,
        0x80,0x0a,0x44,0x0b,0xdb,0xb2,0xce,0xb1,0x7b,0x2b,0x8a,0x9a,0xa6,0x07,0x9c,0x54,
        0x0e,0x38,0xdc,0x92,0xcb,0x1f,0x2a,0x60,0x72,0x61,0x44,0x51,0x83,0x23,0x5a,0xdb
        },
        new byte[64]{
        0xab,0xbe,0xde,0xa6,0x80,0x05,0x6f,0x52,0x38,0x2a,0xe5,0x48,0xb2,0xe4,0xf3,0xf3,
        0x89,0x41,0xe7,0x1c,0xff,0x8a,0x78,0xdb,0x1f,0xff,0xe1,0x8a,0x1b,0x33,0x61,0x03,
        0x9f,0xe7,0x67,0x02,0xaf,0x69,0x33,0x4b,0x7a,0x1e,0x6c,0x30,0x3b,0x76,0x52,0xf4,
        0x36,0x98,0xfa,0xd1,0x15,0x3b,0xb6,0xc3,0x74,0xb4,0xc7,0xfb,0x98,0x45,0x9c,0xed
        },
        new byte[64]{
        0x7b,0xcd,0x9e,0xd0,0xef,0xc8,0x89,0xfb,0x30,0x02,0xc6,0xcd,0x63,0x5a,0xfe,0x94,
        0xd8,0xfa,0x6b,0xbb,0xeb,0xab,0x07,0x61,0x20,0x01,0x80,0x21,0x14,0x84,0x66,0x79,
        0x8a,0x1d,0x71,0xef,0xea,0x48,0xb9,0xca,0xef,0xba,0xcd,0x1d,0x7d,0x47,0x6e,0x98,
        0xde,0xa2,0x59,0x4a,0xc0,0x6f,0xd8,0x5d,0x6b,0xca,0xa4,0xcd,0x81,0xf3,0x2d,0x1b
        },
        new byte[64]{
        0x37,0x8e,0xe7,0x67,0xf1,0x16,0x31,0xba,0xd2,0x13,0x80,0xb0,0x04,0x49,0xb1,0x7a,
        0xcd,0xa4,0x3c,0x32,0xbc,0xdf,0x1d,0x77,0xf8,0x20,0x12,0xd4,0x30,0x21,0x9f,0x9b,
        0x5d,0x80,0xef,0x9d,0x18,0x91,0xcc,0x86,0xe7,0x1d,0xa4,0xaa,0x88,0xe1,0x28,0x52,
        0xfa,0xf4,0x17,0xd5,0xd9,0xb2,0x1b,0x99,0x48,0xbc,0x92,0x4a,0xf1,0x1b,0xd7,0x20
        }
        };

        private byte[] iv = new byte[64];

        private byte[] N = new byte[64];

        private byte[] Sigma = new byte[64];

        public int outLen = 0;

        public GOST(int outputLenght)
        {
            if (outputLenght == 512)
            {
                for (int i = 0; i < 64; i++)
                {
                    N[i] = 0x00;
                    Sigma[i] = 0x00;
                    iv[i] = 0x00;
                }
                outLen = 512;
            }
            else if (outputLenght == 256)
            {
                for (int i = 0; i < 64; i++)
                {
                    N[i] = 0x00;
                    Sigma[i] = 0x00;
                    iv[i] = 0x01;
                }
                outLen = 256;
            }
        }

        private byte[] AddModulo512(byte[] a, byte[] b)
        {
            byte[] temp = new byte[64];
            int i = 0, t = 0;
            byte[] tempA = new byte[64];
            byte[] tempB = new byte[64];
            Array.Copy(a, 0, tempA, 64 - a.Length, a.Length);
            Array.Copy(b, 0, tempB, 64 - b.Length, b.Length);
            for (i = 63; i >= 0; i--)
            {
                t = tempA[i] + tempB[i] + (t >> 8);
                temp[i] = (byte)(t & 0xFF);
            }
            return temp;
        }

        private byte[] AddXor512(byte[] a, byte[] b)
        {
            byte[] c = new byte[64];
            for (int i = 0; i < 64; i++)
                c[i] = (byte)(a[i] ^ b[i]);
            return c;
        }

        private byte[] S(byte[] state)
        {
            byte[] result = new byte[64];
            for (int i = 0; i < 64; i++)
                result[i] = Sbox[state[i]];
            return result;
        }

        private byte[] P(byte[] state)
        {
            byte[] result = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                result[i] = state[Tau[i]];
            }
            return result;
        }

        private byte[] L(byte[] state)
        {
            byte[] result = new byte[64];
            for (int i = 0; i < 8; i++)
            {
                ulong t = 0;
                byte[] tempArray = new byte[8];
                Array.Copy(state, i * 8, tempArray, 0, 8);
                tempArray = tempArray.Reverse().ToArray();
                BitArray tempBits1 = new BitArray(tempArray);
                bool[] tempBits = new bool[64];
                tempBits1.CopyTo(tempBits, 0);
                tempBits = tempBits.Reverse().ToArray();
                for (int j = 0; j < 64; j++)
                {
                    if (tempBits[j] != false)
                        t = t ^ A[j];
                }
                byte[] ResPart = BitConverter.GetBytes(t).Reverse().ToArray();
                Array.Copy(ResPart, 0, result, i * 8, 8);
            }
            return result;
        }

        private byte[] KeySchedule(byte[] K, int i)
        {
            K = AddXor512(K, C[i]);
            K = S(K);
            K = P(K);
            K = L(K);
            return K;
        }

        private byte[] E(byte[] K, byte[] m)
        {
            byte[] state = AddXor512(K, m);
            for (int i = 0; i < 12; i++)
            {
                state = S(state);
                state = P(state);
                state = L(state);
                K = KeySchedule(K, i);
                state = AddXor512(state, K);
            }
            return state;
        }

        private byte[] G_n(byte[] N, byte[] h, byte[] m)
        {
            byte[] K = AddXor512(h, N);
            K = S(K);
            K = P(K);
            K = L(K);
            byte[] t = E(K, m);
            t = AddXor512(t, h);
            byte[] newh = AddXor512(t, m);
            return newh;
        }

        public byte[] GetHash(byte[] message)
        {
            byte[] paddedMes = new byte[64];
            int len = message.Length * 8;
            byte[] h = new byte[64];
            Array.Copy(iv, h, 64);
            byte[] N_0 ={
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            };
            if (outLen == 512)
            {
                for (int i = 0; i < 64; i++)
                {
                    N[i] = 0x00;
                    Sigma[i] = 0x00;
                    iv[i] = 0x00;
                }
            }
            else if (outLen == 256)
            {
                for (int i = 0; i < 64; i++)
                {
                    N[i] = 0x00;
                    Sigma[i] = 0x00;
                    iv[i] = 0x01;
                }
            }
            byte[] N_512 = BitConverter.GetBytes(512);
            int inc = 0;
            while (len >= 512)
            {
                inc++;
                byte[] tempMes = new byte[64];
                Array.Copy(message, message.Length - inc * 64, tempMes, 0, 64);
                h = G_n(N, h, tempMes);
                N = AddModulo512(N, N_512.Reverse().ToArray());
                Sigma = AddModulo512(Sigma, tempMes);
                len -= 512;
            }
            byte[] message1 = new byte[message.Length - inc * 64];
            Array.Copy(message, 0, message1, 0, message.Length - inc * 64);
            if (message1.Length < 64)
            {
                for (int i = 0; i < (64 - message1.Length - 1); i++)
                {
                    paddedMes[i] = 0;
                }
                paddedMes[64 - message1.Length - 1] = 0x01;
                Array.Copy(message1, 0, paddedMes, 64 - message1.Length, message1.Length);
            }
            h = G_n(N, h, paddedMes);
            byte[] MesLen = BitConverter.GetBytes(message1.Length * 8);
            N = AddModulo512(N, MesLen.Reverse().ToArray());
            Sigma = AddModulo512(Sigma, paddedMes);
            h = G_n(N_0, h, N);
            h = G_n(N_0, h, Sigma);
            if (outLen == 512)
                return h;
            else
            {
                byte[] h256 = new byte[32];
                Array.Copy(h, 0, h256, 0, 32);
                return h256;
            }
        }
    }
    public class DSGost
    {
        private BInteger p = new BInteger();
        private BInteger a = new BInteger();
        private BInteger b = new BInteger();
        private BInteger n = new BInteger();
        private byte[] xG;
        private ECPoint G = new ECPoint();

        public DSGost(BInteger p, BInteger a, BInteger b, BInteger n, byte[] xG)
        {
            this.a = a;
            this.b = b;
            this.n = n;
            this.p = p;
            this.xG = xG;
        }

        //Генерируем секретный ключ заданной длины
        public BInteger GenPrivateKey(int BitSize)
        {
            BInteger d = new BInteger();
            do
            {
                d.genRandomBits(BitSize, new Random());
            } while ((d < 0) || (d > n));
            return d;
        }

        //С помощью секретного ключа d вычисляем точку Q=d*G, это и будет наш публичный ключ
        public ECPoint GenPublicKey(BInteger d)
        {
            ECPoint G = GDecompression();
            ECPoint Q = ECPoint.multiply(d, G);
            return Q;
        }

        //Восстанавливаем координату y из координаты x и бита четности y 
        private ECPoint GDecompression()
        {
            byte y = xG[0];
            byte[] x = new byte[xG.Length - 1];
            Array.Copy(xG, 1, x, 0, xG.Length - 1);
            BInteger Xcord = new BInteger(x);
            BInteger temp = (Xcord * Xcord * Xcord + a * Xcord + b) % p;
            BInteger beta = ModSqrt(temp, p);
            BInteger Ycord = new BInteger();
            if ((beta % 2) == (y % 2))
                Ycord = beta;
            else
                Ycord = p - beta;
            ECPoint G = new ECPoint();
            G.a = a;
            G.b = b;
            G.FieldChar = p;
            G.x = Xcord;
            G.y = Ycord;
            this.G = G;
            return G;
        }

        //функция вычисления квадратоного корня по модулю простого числа q
        public BInteger ModSqrt(BInteger a, BInteger q)
        {
            BInteger b = new BInteger();
            do
            {
                b.genRandomBits(255, new Random());
            } while (Legendre(b, q) == 1);
            BInteger s = 0;
            BInteger t = q - 1;
            while ((t & 1) != 1)
            {
                s++;
                t = t >> 1;
            }
            BInteger InvA = a.modInverse(q);
            BInteger c = b.modPow(t, q);
            BInteger r = a.modPow(((t + 1) / 2), q);
            BInteger d = new BInteger();
            for (int i = 1; i < s; i++)
            {
                BInteger temp = 2;
                temp = temp.modPow((s - i - 1), q);
                d = (r.modPow(2, q) * InvA).modPow(temp, q);
                if (d == (q - 1))
                    r = (r * c) % q;
                c = c.modPow(2, q);
            }
            return r;
        }

        //Вычисляем символ Лежандра
        public BInteger Legendre(BInteger a, BInteger q)
        {
            return a.modPow((q - 1) / 2, q);
        }

        //подписываем сообщение
        public string SingGen(byte[] h, BInteger d)
        {
            BInteger alpha = new BInteger(h);
            BInteger e = alpha % n;
            if (e == 0)
                e = 1;
            BInteger k = new BInteger();
            ECPoint C = new ECPoint();
            BInteger r = new BInteger();
            BInteger s = new BInteger();
            do
            {
                do
                {
                    k.genRandomBits(n.bitCount(), new Random());
                } while ((k < 0) || (k > n));
                C = ECPoint.multiply(k, G);
                r = C.x % n;
                s = ((r * d) + (k * e)) % n;
            } while ((r == 0) || (s == 0));
            string Rvector = padding(r.ToHexString(), n.bitCount() / 4);
            string Svector = padding(s.ToHexString(), n.bitCount() / 4);
            return Rvector + Svector;
        }

        //проверяем подпись 
        public bool SingVer(byte[] H, string sing, ECPoint Q)
        {
            string Rvector = sing.Substring(0, n.bitCount() / 4);
            string Svector = sing.Substring(n.bitCount() / 4, n.bitCount() / 4);
            BInteger r = new BInteger(Rvector, 16);
            BInteger s = new BInteger(Svector, 16);
            if ((r < 1) || (r > (n - 1)) || (s < 1) || (s > (n - 1)))
                return false;
            BInteger alpha = new BInteger(H);
            BInteger e = alpha % n;
            if (e == 0)
                e = 1;
            BInteger v = e.modInverse(n);
            BInteger z1 = (s * v) % n;
            BInteger z2 = n + ((-(r * v)) % n);
            this.G = GDecompression();
            ECPoint A = ECPoint.multiply(z1, G);
            ECPoint B = ECPoint.multiply(z2, Q);
            ECPoint C = A + B;
            BInteger R = C.x % n;
            if (R == r)
                return true;
            else
                return false;
        }

        //дополняем подпись нулями слева до длины n, где n - длина модуля в битах
        private string padding(string input, int size)
        {
            if (input.Length < size)
            {
                do
                {
                    input = "0" + input;
                } while (input.Length < size);
            }
            return input;
        }
    }
}