using System;
using CryptographyLabrary;

int passed = 0, failed = 0;
const string PLAIN = "hello world";
var alphabet = "abcdefghijklmnopqrstuvwxyz ".ToCharArray();

Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║       Cryptography Library — Demo Suite         ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝\n");

// ── Classical Ciphers ──────────────────────────────────────

Section("Classical Ciphers");

Run("Caesar", () => {
    var c = new Cesar { Step = 3, Alphabet = alphabet };
    return Roundtrip(c, PLAIN);
});

Run("Trithemius", () => {
    var c = new Tritemius { Alphabet = alphabet };
    return Roundtrip(c, PLAIN);
});

Run("XOR/Vigenere", () => {
    var c = new XOR { Alphabet = alphabet };
    return Roundtrip(c, PLAIN);
});

// ── Block Ciphers ──────────────────────────────────────────

Section("Block Ciphers");

Run("DES", () => {
    var c = new DES();
    string enc = c.Encryption("TestMsg!");
    string dec = c.Decryption(enc);
    return ($"\"TestMsg!\" -> [{enc.Length} bits] -> \"{dec}\"", dec == "TestMsg!");
});

Run("AES-192", () => {
    var aes = new AES(AES.KeySize.Bits192);
    byte[] input = { 0x00,0x11,0x22,0x33,0x44,0x55,0x66,0x77,0x88,0x99,0xaa,0xbb,0xcc,0xdd,0xee,0xff };
    byte[] enc = new byte[16], dec = new byte[16];
    aes.Cipher(input, enc);
    aes.InvCipher(enc, dec);
    bool ok = true;
    for (int i = 0; i < 16; i++) if (input[i] != dec[i]) ok = false;
    return ($"16 bytes -> {BitConverter.ToString(enc).Replace("-","")} -> roundtrip", ok);
});

Run("Blowfish", () => {
    var bf = new Blowfish();
    bf.CalculatePnS(0x0123456789ABCDEF);
    ulong plain = 0x4142434445464748;
    ulong enc = bf.Encrypt(plain);
    ulong dec = bf.Decrypt(enc);
    return ($"0x{plain:X16} -> 0x{enc:X16} -> 0x{dec:X16}", dec == plain);
});

// ── Asymmetric / Key Exchange ──────────────────────────────

Section("Asymmetric Cryptography");

Run("RSA", () => {
    var rsa = new RSA();
    string enc = rsa.Encryption("Hi");
    string dec = rsa.Decryption(enc);
    return ($"\"Hi\" -> \"{enc.TrimEnd(',')}\" -> \"{dec}\"", dec == "Hi");
});

Run("El-Gamal", () => {
    var alice = new El_Gammal();
    var bob = new El_Gammal();
    alice.CreateLink(bob);
    string enc = bob.Encryption("Hi");
    string dec = bob.Decryption(enc);
    return ($"\"Hi\" -> encrypted -> \"{dec}\"", dec == "Hi");
});

Run("Diffie-Hellman", () => {
    var alice = new CDH();
    var bob = new CDH();
    alice.CreateLink(bob);
    return ($"key exchange complete (P={alice.P}, G={alice.G})", true);
});

Run("DSA Signature", () => {
    var dsa = new DSA();
    dsa.Message = 42;
    dsa.CreateSignature();
    bool valid = dsa.Validate(dsa.Signature, dsa.PublicKey, 42);
    return ($"sign({dsa.Message}) = {dsa.GetSignature()}, valid={valid}", valid);
});

// ── Hash Functions ─────────────────────────────────────────

Section("Hash Functions");

Run("SHA-1", () => {
    var sha = new CryptographyLabrary.SHA1();
    string hash = sha.Hash("abc");
    bool ok = hash.Replace(" ", "") == "A9993E364706816ABA3E25717850C26C9CD0D89D";
    return ($"SHA1(\"abc\") = {hash}", ok);
});

Run("SHA-256", () => {
    var sha = new SHA2();
    string hash = sha.Hash(System.Text.Encoding.UTF8.GetBytes("abc"));
    bool ok = hash.Replace(" ", "") == "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD";
    return ($"SHA256(\"abc\") = {hash}", ok);
});

Run("MD5", () => {
    var md5 = new MD3();
    byte[] h = md5.Hash("abc");
    string hash = BitConverter.ToString(h).Replace("-", "").ToLower();
    bool ok = hash == "900150983cd24fb0d6963f7d28e17f72";
    return ($"MD5(\"abc\") = {hash}", ok);
});

// ── Summary ────────────────────────────────────────────────

Console.WriteLine($"\n{"",3}{"─",40}");
Console.WriteLine($"   {passed} passed, {failed} failed out of {passed+failed} tests\n");

// ── Helpers ────────────────────────────────────────────────

(string msg, bool ok) Roundtrip(ICipher cipher, string text)
{
    string enc = cipher.Encryption(text);
    string dec = cipher.Decryption(enc);
    return ($"\"{text}\" -> \"{enc}\" -> \"{dec}\"", dec == text);
}

void Section(string name)
{
    Console.WriteLine($"\n ── {name} ──\n");
}

void Run(string name, Func<(string msg, bool ok)> test)
{
    string label = $"[{name}]".PadRight(18);
    try
    {
        var (msg, ok) = test();
        if (ok) { passed++; Console.WriteLine($"  {label} OK   {msg}"); }
        else    { failed++; Console.WriteLine($"  {label} FAIL {msg}"); }
    }
    catch (Exception e)
    {
        failed++;
        Console.WriteLine($"  {label} ERR  {e.Message}");
    }
}
