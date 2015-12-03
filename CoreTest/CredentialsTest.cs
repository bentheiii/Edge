using System;
using Edge.Credentials;
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
}