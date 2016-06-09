using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Edge.Credentials;
using Edge.Looping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace CoreTest
{
    [TestClass]
    public class SimpleValidator
    {
        [TestMethod] public void Simple()
        {
            Credential cred;
            var val = new CredentialValidator(out cred);
            IsTrue(val.isValid(cred));
            IsFalse(val.isValid(new Credential()));
        }
        [TestMethod] public void PreConstructed()
        {
            Credential cred = new Credential();
            var val = new CredentialValidator(cred);
            IsTrue(val.isValid(cred));
            IsFalse(val.isValid(new Credential()));
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))] public void NullConstructed()
        {
            Credential cred = null;
            var val = new CredentialValidator(cred);
            Fail();
        }
    }
    [TestClass]
    public class SpecialValidatorTest
    {
        [TestMethod] public void SimpleOpen()
        {
            var val = new OpenCredentialValidator();
            IsTrue(val.isValid(new Credential()));
            IsTrue(val.isValid(null));
        }
        [TestMethod] public void SimpleClosed()
        {
            var val = new ClosedCredentialValidator();
            IsFalse(val.isValid(new Credential()));
            IsFalse(val.isValid(null));
        }
    }
    [TestClass]
    public class UnrefValidatorTest
    {
        [TestMethod] public void Simple()
        {
            var val = new UnRefCredentialsValidator<int>(i => i % 3 == 0);
            IsTrue(val.isValid(new UnRefCredential<int>(6)));
            IsFalse(val.isValid(new UnRefCredential<int>(1)));
        }
    }
    [TestClass]
    public class RsaValidation
    {
        [TestMethod] public void Simple()
        {
            var messages = new[] {"anc", "abc", "hi there", "", "my my", "there's choclate behind this string"};
            foreach (var message in messages)
            {
                RSAParameters pub;
                RSAParameters priv;
                RSACredential.GetKey(out priv, out pub);
                var mb = Encoding.Unicode.GetBytes(message);
                Credential cred = RSACredential.Create(mb, priv);
                ICredentialValidator val = new RSAValidator(mb,pub);
                IsTrue(val.isValid(cred));
                foreach (string source in messages.Except(message))
                {
                    var sb = Encoding.Unicode.GetBytes(source);
                    var sval = new RSAValidator(sb, pub);
                    IsFalse(sval.isValid(cred));
                    var scred = RSACredential.Create(sb, priv);
                    IsFalse(val.isValid(scred));
                }
            }
        }
    }
}