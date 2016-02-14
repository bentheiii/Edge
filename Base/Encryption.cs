using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Edge.Arrays;
using Edge.Streams;

namespace Edge.Crypto
{
    public static class Encryption
    {
        public const int IV_LENGTH = 16;
        public static byte[] GenValidKey(string original)
        {
            return Sha2Hashing.Hash(original).Take(32).ToArray(32);
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
        public const int HASH_LENGTH = 64;
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
        /*secure cypher scheme:
        64 bytes-hash of everything after this
        16 bits-iv of encryption
        the rest is cyphertext, the original input is padded by
        */
        public static byte[] Encrypt(byte[] plainText, byte[] key)
        {
            byte[] iv;
            byte[] cypher = Encryption.Encrypt(plainText, key, out iv);
            var hash = Sha2Hashing.Hash(iv.Concat(cypher));
            return hash.Concat(iv).Concat(cypher).ToArray(iv.Length + cypher.Length + hash.Length);
        }
        public static byte[] Decrypt(byte[] enc, byte[] key)
        {
            return !enc.Take(Sha2Hashing.HASH_LENGTH).SequenceEqual(Sha2Hashing.Hash(enc.Skip(Sha2Hashing.HASH_LENGTH)))
                ? null
                : Encryption.Decrypt(enc.Skip(Sha2Hashing.HASH_LENGTH + Encryption.IV_LENGTH).ToArray(), key,
                    enc.Skip(Sha2Hashing.HASH_LENGTH).Take(Encryption.IV_LENGTH).ToArray(Encryption.IV_LENGTH));
        }
    }
}
