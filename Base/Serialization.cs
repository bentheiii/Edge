using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Input;
using Edge.Arrays;
using Edge.Looping;
using Edge.WordsPlay;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edge.Serializations
{
    public static class NumberSerialization
    {
        public interface INumberSerializer
        {
            ulong FromBytes(IEnumerable<byte> bytes);
            IEnumerable<byte> ToBytes(ulong num);
        }
        public class FullcodeNumberSerializer : INumberSerializer
        {
            public IEnumerable<byte> ToBytes(ulong num)
            {
                while (num != 0)
                {
                    yield return (byte)(num % 256);
                    num /= 256;
                }
            }
            public ulong FromBytes(IEnumerable<byte> bytes)
            {
                ulong ret = 0;
                ulong pow = 1;
                foreach (byte b in bytes)
                {
                    ret += (b * pow);
                    pow <<= 8;
                }
                return ret;
            }
        }
        public class ClosedListNumberSerializer : INumberSerializer
        {
            private readonly byte[] _closed;
            public ClosedListNumberSerializer(IEnumerable<char> closedlist) : this(closedlist.SelectToArray(a=>(byte)a)) { }
            public ClosedListNumberSerializer(byte[] closedList)
            {
                if (closedList.Duplicates().Any() || !closedList.Any())
                    throw new ArgumentException("closed list must be unique and non-empty");
                _closed = closedList.Sort();
            }
            public ulong FromBytes(IEnumerable<byte> bytes)
            {
                ulong ret = 0;
                ulong pow = 1;
                foreach (byte b in bytes)
                {
                    ret += ((uint)_closed.binSearch(b) * pow);
                    pow *= (uint)_closed.Length;
                }
                return ret;
            }
            public IEnumerable<byte> ToBytes(ulong num)
            {
                while (num != 0)
                {
                    yield return _closed[num % (uint)_closed.Length];
                    num /= (uint)_closed.Length;
                }
            }
        }
        public static readonly INumberSerializer FullCodeSerializer = new FullcodeNumberSerializer();
        public static readonly INumberSerializer AlphaNumbreicSerializer = new ClosedListNumberSerializer("0123456789abcdefghijklmnopqrstuvwxyz");
        public static ulong FromString(this INumberSerializer @this, string s)
        {
            return @this.FromBytes(s.Select(a => (byte)a));
        }
        public static string ToString(this INumberSerializer @this, ulong s)
        {
            return @this.ToBytes(s).Select(a => (char)a).convertToString();
        }
        public static string EncodeSpecificLength(this INumberSerializer @this, string s, int maxlengthlengthlength = 1)
        {
            int length = s.Length;
            var lenstring = @this.ToString((ulong)length);
            int lengthlength = lenstring.Length;
            var lenlenstring = @this.ToString((ulong)lengthlength);
            int lengthlengthlength = lenlenstring.Length;
            if (lengthlengthlength > maxlengthlengthlength)
                throw new ArgumentException("message too large");
            if (lengthlengthlength < maxlengthlengthlength)
                lenlenstring = lenlenstring + new string((char)0, maxlengthlengthlength - lengthlengthlength);
            return lenlenstring + lenstring + s;
        }
        public static string DecodeSpecifiedLength(this INumberSerializer @this, string s, int lengthlengthlength = 1)
        {
            string remainder;
            return DecodeSpecifiedLength(@this, s, out remainder, lengthlengthlength);
        }
        public static string DecodeSpecifiedLength(this INumberSerializer @this, string s, out string remainder, int lengthlengthlength = 1)
        {
            int lengthlength = (int)@this.FromString(s.Substring(0, lengthlengthlength));
            s = s.Remove(0, lengthlengthlength);
            int length = (int)@this.FromString(s.Substring(0, lengthlength));
            s = s.Remove(0, lengthlength);
            remainder = s.Substring(length);
            return s.Substring(0, length);
        }
    }
    public static class Serialization
    {
        
        public static object Deserialize(byte[] arr)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arr, 0, arr.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return binForm.Deserialize(memStream);
        }
        public static byte[] Serialize(object o)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
    }
	
    public static class Hashing
    {
        public static ulong CalculateHash(string read)
        {
            ulong hashedValue = 3074457345618258791ul;
            foreach (char t in read) {
                hashedValue += t;
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }
    }
}
