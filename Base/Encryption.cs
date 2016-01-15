using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Edge.Crypto
{
    public static class Encryption
    {
        public enum EncryptionType
        {
            TripleDes,AES
        }
        public static byte[] GenValidKey(string original)
        {
            original = Sha2Hashing.Hash(original).Substring(0,24);
            return Encoding.UTF32.GetBytes(original).Take(original.Length).ToArray();
        }
        public static SymmetricAlgorithm getProvider(byte[] key, EncryptionType type)
        {
            SymmetricAlgorithm ret = null;
            switch (type)
            {
                case EncryptionType.TripleDes:
                    ret = new TripleDESCryptoServiceProvider();
                    break;
                case EncryptionType.AES:
                    ret = new AesCryptoServiceProvider();
                    break;
            }
            ret.Mode = CipherMode.ECB;
            ret.Padding = PaddingMode.ANSIX923;
            ret.Key = key;
            return ret;
        }
        public static SymmetricAlgorithm getProvider(string key, EncryptionType type)
        {
            return getProvider(GenValidKey(key),type);
        }
        public static string Encrypt(string input, string key, EncryptionType type)
        {
            return Encrypt(input, getProvider(key, type));
        }
        public static string Decrypt(string input, string key, EncryptionType type)
        {
            return Decrypt(input, getProvider(key, type));
        }
        public static string Encrypt(string input, SymmetricAlgorithm algorithm)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(input);
            ICryptoTransform cTransform = algorithm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            algorithm.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input, SymmetricAlgorithm algorithm)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            ICryptoTransform cTransform = algorithm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            algorithm.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }
    }
    public static class Sha2Hashing
    {
        public static string Hash(string input)
        {
            var alg = SHA512.Create();
            alg.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Encoding.UTF8.GetString(alg.Hash);
        }
    }
}
