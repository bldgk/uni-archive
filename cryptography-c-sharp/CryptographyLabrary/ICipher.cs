namespace CryptographyLabrary
{
    public interface ICipher
    {
        char[] Alphabet { get; set; }
        string Encryption(string text);
        string Decryption(string text);
    }
}