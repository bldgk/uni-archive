using System;
using System.Linq;

namespace CryptographyLabrary
{
    public class Cesar: ICipher
    {
        public int Step { get; set; }
        public char[] Alphabet { get; set; }
        public Cesar()
        {
            Step = 2;
            Alphabet = new char[] { };
        }
        public string Encryption(string text)
        {
            string EncryptedText = "";
            foreach (char symbol in text)
            {
                EncryptedText += Alphabet[EncodingCharIndex(Array.IndexOf(Alphabet, symbol))];
            }
            return EncryptedText;
        }
        public int EncodingCharIndex(int CharIndex) => (CharIndex + Step) % Alphabet.Count();
        
        public string Decryption(string text)
        {
            string DecryptedText = "";
            foreach (char symbol in text)
            {
                DecryptedText += Alphabet[DecodingCharIndex(Array.IndexOf(Alphabet, symbol))];
            }
            return DecryptedText;
        }
        public int DecodingCharIndex(int CharIndex)
        {
            int NewCharIndex = 0;
            NewCharIndex = (CharIndex - Step) % Alphabet.Count();
            if (NewCharIndex < 0)
                NewCharIndex = Alphabet.Count() + NewCharIndex;
            return NewCharIndex;
        }
    }
}
