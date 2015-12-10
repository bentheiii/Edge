using System;

namespace Edge.Credentials
{
    [Serializable]
    public class Credential {}
    [Serializable]
    public class UnRefCredential<T> : Credential
    {
        public UnRefCredential(T value)
        {
            this.value = value;
        }
        public T value { get; }
        public override string ToString()
        {
            return value.ToString();
        }
    }
    [Serializable]
    public class PasswordCredential : UnRefCredential<string>
    {
        public PasswordCredential(string value) : base(value) {}
        public override string ToString()
        {
            return value;
        }
    }
    public interface ICredentialValidator
    {
        bool isValid(Credential c);
    }
    [Serializable]
    public class CredentialValidator : ICredentialValidator
    {
        public Credential valid { get; }
        public CredentialValidator() : this(new Credential()) { }
        public CredentialValidator(Credential valid)
        {
            if (valid == null)
                throw new ArgumentException("cannot be null",nameof(valid));
            this.valid = valid;
        }
        public CredentialValidator(out Credential valid) : this(valid = new Credential()) { }
        public bool isValid(Credential c)
        {
            return ReferenceEquals(this.valid, c);
        }
    }
    [Serializable]
    public class OpenCredentialValidator : ICredentialValidator
    {
        public bool isValid(Credential c)
        {
            return true;
        }
    }
    [Serializable]
    public class ClosedCredentialValidator : ICredentialValidator
    {
        public bool isValid(Credential c)
        {
            return false;
        }
    }
    [Serializable]
    public class UnRefCredentialsValidator<T> : ICredentialValidator
    {
        private readonly Func<T, bool> _validator;
        public UnRefCredentialsValidator(Func<T, bool> validator)
        {
            this._validator = validator;
        }
        public bool isValid(Credential c)
        {
            var credentials = c as UnRefCredential<T>;
            return credentials != null && this._validator(credentials.value);
        }
    }
    public static class ValidationExtentions
    {
        public static void ThrowIfInvalid(this ICredentialValidator v, Credential c)
        {
            if(!v.isValid(c))
                throw new UnauthorizedAccessException("the credentials are not valid");
        }
    }
}