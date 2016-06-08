using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edge.Crypto;
using Edge.Looping;
using Edge.WordsPlay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    public static class AssertEnc
    {
        public static void CheckEncryptor(IEnumerable<string> inputs, IEnumerable<string> keys)
        {
            foreach (var i in inputs.Join(inputs.Concat(keys)))
            {
                var input = i.Item1;
                var key = i.Item2;
                byte[] iv;
                var b = Encoding.Unicode.GetBytes(input);
                var cypher = Encryption.Encrypt(b, key, out iv);
                var d = Encryption.Decrypt(cypher, key, iv);
                var s = Encoding.Unicode.GetString(d);
                AreEqual(input,s);
            }
        }
        public static void CheckSecureEncryptor(IEnumerable<string> inputs, IEnumerable<string> keys)
        {
            foreach (var i in inputs.Join(inputs.Concat(keys)).Join(Loops.IRange(20)))
            {
                var input = i.Item1.Item1;
                var key = i.Item1.Item2;
                var pad = i.Item2;
                var valKey = Encryption.GenValidKey(key);
                var b = Encoding.Unicode.GetBytes(input);
                var cypher = SecureEncryption.Encrypt(b, valKey);
                var d = SecureEncryption.Decrypt(cypher, valKey);
                var s = Encoding.Unicode.GetString(d);
                AreEqual(input, s);
            }
        }
    }
    [TestClass]
    public class AesTest
    {
        [TestMethod] public void Simple()
        {
            AssertEnc.CheckEncryptor(new[] {"aaa", "abc", "secret", "this is a secret","אבג"},
                new[] {"key", "this is a key", "i have a key", "keykeykeykey", ""});
        }
    }
    [TestClass]
    public class SecureAesTest
    {
        [TestMethod]
        public void Simple()
        {
            AssertEnc.CheckSecureEncryptor(new string[] { "aaa", "abc", "secret", "this is a secret","אבג" },
                new string[] { "key", "this is a key", "i have a key", "keykeykeykey", "" });
        }
    }
}
