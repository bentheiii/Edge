using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Edge.Arrays;
using Edge.Serializations;
using Edge.Streams;

namespace Edge.Crypto
{
    public static class Encryption
    {
        public const int IV_LENGTH = 128/8, KEY_LENGTH = 256/8;
        public static byte[] GenValidKey(string original)
        {
            return Sha2Hashing.Hash(original).Take(KEY_LENGTH).ToArray(KEY_LENGTH);
        }
        public static byte[] Encrypt(byte[] plainText,string key, out byte[] iv)
        {
            return Encrypt(plainText, GenValidKey(key), out iv);
        }
        public static byte[] Encrypt(byte[] plainText, byte[] key, out byte[] iv)
        {
            using (var r = new AesManaged())
            {
                r.GenerateIV();
                iv = r.IV;
            }
            return Encrypt(plainText, key, iv);
        }
        public static byte[] Encrypt(byte[] plainText, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (AesManaged rijAlg = new AesManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainText,0,plainText.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }
        public static byte[] Decrypt(byte[] cipherText, string key, byte[] iv)
        {
            return Decrypt(cipherText, GenValidKey(key), iv);
        }
        public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (AesManaged rijAlg = new AesManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        /*using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }*/
                        return csDecrypt.ReadAll();
                    }
                }

            }

        }
    }
    public static class Sha2Hashing
    {
        public const int HASH_LENGTH = 512/8;
        public static IEnumerable<byte> Hash(string input)
        {
            return Hash(Encoding.Unicode.GetBytes(input));
        }
        public static byte[] Hash(IEnumerable<byte> input)
        {
            var alg = SHA512.Create();
            Stream s = new EnumerationStream(input);
            alg.ComputeHash(s);
            return alg.Hash;
        }
        public static IEnumerable<byte> Hash(byte[] input)
        {
            var alg = SHA512.Create();
            alg.ComputeHash(input);
            return alg.Hash;
        }
    }
    public static class SecureEncryption
    {
        public const int ORIGINALSIZELENGTH = 16;
        /*secure cypher scheme:
        64 bytes-hash of everything after this
        16 bits-iv of encryption
        16 (encrypted) bits of plaintext length (serialized in base-256, max input size is 2^64)
        the rest is encrypted plaintext+padding
        size overhead: 96 bytes
        */
        public static byte[] Encrypt(byte[] plainText, byte[] key,int padding=0, Func<byte> padGenerator = null)
        {
            if (padGenerator != null && padding == 0)
                throw new ArgumentException();
            var padded = padding == 0 ? plainText : plainText.Concat(ArrayExtensions.Fill(padding,padGenerator)).ToArray(plainText.Length+padding);
            var orglength = NumberSerialization.FullCodeSerializer.ToBytes(plainText.Length).ToArray();
            if (orglength.Length > ORIGINALSIZELENGTH)
                throw new ArgumentException("plaintext too long");
            orglength = orglength.Concat(ArrayExtensions.Fill(ORIGINALSIZELENGTH - orglength.Length, (byte)0)).ToArray(ORIGINALSIZELENGTH);
            byte[] iv;
            byte[] cypher = Encryption.Encrypt(orglength.Concat(padded).ToArray(), key, out iv);
            var hash = Sha2Hashing.Hash(iv.Concat(cypher));
            return hash.Concat(iv).Concat(cypher).ToArray(iv.Length + cypher.Length + hash.Length);
        }
        public static byte[] Decrypt(byte[] enc, byte[] key)
        {
            if (!enc.Take(Sha2Hashing.HASH_LENGTH).SequenceEqual(Sha2Hashing.Hash(enc.Skip(Sha2Hashing.HASH_LENGTH))))
                throw new FormatException("hash mismatch");
            var padded = Encryption.Decrypt(enc.Skip(Sha2Hashing.HASH_LENGTH + Encryption.IV_LENGTH).ToArray(), key,
                enc.Skip(Sha2Hashing.HASH_LENGTH).Take(Encryption.IV_LENGTH).ToArray(Encryption.IV_LENGTH));
            var orglen = (int)NumberSerialization.FullCodeSerializer.FromBytes(padded.Take(ORIGINALSIZELENGTH));
            return padded.Skip(ORIGINALSIZELENGTH).Take(orglen).ToArray(orglen);
        }
    }
}
