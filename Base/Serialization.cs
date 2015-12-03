using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Edge.Serializations
{
    
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
