using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Credentials;
using Edge.Looping;
using Edge.Path;
using Edge.Ports;
using Edge.Ports.MultiSocket;
using Edge.Serializations;
using Edge.WordsPlay;

namespace Edge.PermanentObject
{
    public interface IPermaObject<T> : IDisposable
    {
        T tryParse(out Exception ex);
        T value { get; set; }
        string name { get; }
    }
    public static class PermaObject
    {
        internal static readonly string RoamingFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CobraCore\PermaObjects";
        internal const string PERMA_OBJ_TRACKER_FILE = "perma_obj_tracker";
        internal const string PERMA_OBJ_EXTENSION = ".permaobj";
        internal const string PERMA_PORT_EXTENSION = ".permaport";
        static PermaObject()
        {
            ensurePermaObjData();
        }
        public static void ensurePermaObjData()
        {
            Directory.CreateDirectory(RoamingFolderPath);
            if (!File.Exists(RoamingFolderPath + @"\" + PERMA_OBJ_TRACKER_FILE))
            {
                TextWriter w = new StreamWriter(RoamingFolderPath + @"\" + PERMA_OBJ_TRACKER_FILE);
                w.Write("0");
                w.Close();
            }
        }
        public static bool Exists(string name, string permaObjExtension = PERMA_OBJ_EXTENSION)
        {
            return File.Exists(RoamingFolderPath + @"\" + name + permaObjExtension);
        }
        internal static string getnextFilename()
        {
            TextReader r = new StreamReader(RoamingFolderPath + @"\" + PERMA_OBJ_TRACKER_FILE);
            BigInteger s = BigInteger.Parse(r.ReadToEnd());
            r.Close();
            TextWriter w = new StreamWriter(RoamingFolderPath + @"\" + PERMA_OBJ_TRACKER_FILE);
            w.Write((s + 1).ToString());
            w.Close();
            return s.ToString("X");
        }
        public static void Mutaute<T>(this IPermaObject<T> @this, Func<T, T> mutation)
        {
            @this.value = mutation(@this.value);
        }
        public static void Mutaute<T>(this IPermaObject<T> @this, Action<T> mutation)
        {
            var temp = @this.value;
            mutation(temp);
            @this.value = temp;
        }
        public static string LocalName<T>(this IPermaObject<T> @this)
        {
            var s = @this.name;
            return s.Substringbetween(s.LastIndexOf(@"\"),s.LastIndexOf("."));
        }
        public static void delete<T>(this IPermaObject<T> @this)
        {
            if (FilePath.IsFileAccessible(@this.name, FileAccess.Write))
                File.Delete(@this.name);
        }
    }
    public class PermaObject<T> : IPermaObject<T>
    {
        private readonly FileStream _stream;
        private readonly bool _preserve;
        private readonly Func<byte[], T> _read;
        private readonly Func<T, byte[]> _write;
        public PermaObject() : this(null) { }
        public PermaObject([CanBeNull] string name) : this(name,false) { }
        public PermaObject([CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(a => (T)Serialization.Deserialize(a), a=>Serialization.Serialize(a), name, preserve,extension) { }
        public PermaObject(Func<byte[], T> read, Func<T, byte[]> write) : this(read, write, null,false) { }
        public PermaObject(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION)
        {
            this._read = read;
            this._write = write;
            this._preserve = preserve;
            name = (name ?? PermaObject.getnextFilename())+ extension;
            _stream = new FileStream(PermaObject.RoamingFolderPath + @"\" +name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

        }
        public T tryParse(out Exception ex)
        {
            const int buffersize = 255;
            _stream.Seek(0, SeekOrigin.Begin);
            List<byte> b = new List<byte>(buffersize);
            byte[] buffer = new byte[buffersize];
            int grabbed;
            while ((grabbed = _stream.Read(buffer, 0, buffersize)) > 0)
            {
                b.AddRange(buffer.Take(grabbed));
            }
            try
            {
                ex = null;
                return _read(b.ToArray());
            }
            catch (Exception e)
            {
                ex = e;
                return default(T);
            }
        }
        public T value
        {
            get
            {
                Exception prox;
                T ret = tryParse(out prox);
                if (prox != null)
                    throw prox;
                return ret;
            }
            set
            {
                byte[] buffer = _write(value);
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.SetLength(0);
                _stream.Write(buffer,0,buffer.Length);
                _stream.Flush(true);
            }
        }
        public string name
        {
            get
            {
                return _stream.Name;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            (this._stream as IDisposable).Dispose();
            if (disposing && !_preserve && FilePath.IsFileAccessible(name, FileAccess.Write))
                File.Delete(_stream.Name);
        }
    }
    public interface ISyncPermaObject<T> : IPermaObject<T>
    {
        T getFresh(DateTime earliestTime);
        DateTime getLatestUpdateTime();
    }
    public class SyncPermaObject<T> : ISyncPermaObject<T>
    {
        internal const string PERMA_OBJ_UPDATE_EXTENSION = ".permaobjupdate";
        private readonly PermaObject<T> _int;
        private readonly PermaObject<DateTime> _update;
        public SyncPermaObject() : this(null) { }
        public SyncPermaObject([CanBeNull] string name) : this(name,false) { }
        public SyncPermaObject([CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION, string updateext = PERMA_OBJ_UPDATE_EXTENSION) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, preserve, extension, updateext) { }
        public SyncPermaObject(Func<byte[], T> read, Func<T, byte[]> write,[CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION, string updateext = PERMA_OBJ_UPDATE_EXTENSION)
        {
            _int = new PermaObject<T>(read,write,name,preserve,extension);
            _update = new PermaObject<DateTime>(name, preserve, updateext);
        }
        public DateTime getLatestUpdateTime()
        {
            Exception e;
            var a = _update.tryParse(out e);
            return (e == null) ? a : DateTime.MinValue;
        }
        public T getFresh(DateTime earliestTime)
        {
            return getFresh(earliestTime, TimeSpan.FromSeconds(0.5));
        }
        public T getFresh(DateTime earliestTime, TimeSpan checkinterval)
        {
            while (getLatestUpdateTime() < earliestTime)
            {
                Thread.Sleep(checkinterval);
            }
            return this.value;
        }
        public T tryParse(out Exception ex)
        {
            return _int.tryParse(out ex);
        }
        public T value
        {
            get
            {
                return _int.value;
            }
            set
            {
                _update.value = DateTime.Now;
                _int.value = value;
            }
        }
        public string name
        {
            get
            {
                return _int.name;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            _int.Dispose();
            _update.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class PortBoundPermaObject<T> : ISyncPermaObject<T>
    {
        [Serializable]
        private class ValChangedNotification
        {}
        private readonly SyncPermaObject<T> _int;
        private readonly IConnection _conn;
        private readonly PermaObject<EndPoint> _permaPort;
        public PortBoundPermaObject([CanBeNull] string name, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(name, false, extension) { }
        public PortBoundPermaObject([CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, preserve, extension) { }
        public PortBoundPermaObject(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION)
        {
            Exception ex;
            this._permaPort = new PermaObject<EndPoint>("name"+extension,false,PermaObject.PERMA_PORT_EXTENSION);
            EndPoint port = this._permaPort.tryParse(out ex);
            if (ex != null)
            {
                //port is undefined
                _conn = MultiSocket.ConnectToStrippedMultiSocket(out port, new OpenCredentialValidator(),
                    new Credential());
                this._permaPort.value = port;
            }
            else
            {
                //port is defined
                _conn = MultiSocket.ConnectToStrippedMultiSocket(port, new OpenCredentialValidator(),
                    new Credential());
            }
            _int = new SyncPermaObject<T>(read,write,name,preserve,extension);

        }
        protected virtual void Dispose(bool disposing)
        {
            _conn.Dispose();
            _int.Dispose();
            _permaPort.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public T tryParse(out Exception ex)
        {
            return _int.tryParse(out ex);
        }
        public T value
        {
            get
            {
                return _int.value;
            }
            set
            {
                _int.value = value;
                _conn.Target = _conn.Source;
                _conn.Send(new ValChangedNotification());
            }
        }
        public string name
        {
            get
            {
                return _int.name;
            }
        }
        public DateTime getLatestUpdateTime()
        {
            return _int.getLatestUpdateTime();
        }
        public T getFresh(DateTime earliestTime)
        {
            while (getLatestUpdateTime() < earliestTime)
            {
                object pack = _conn.recieve();
                if (!(pack is ValChangedNotification))
                {
                    throw new InvalidDataException("PortBoundPermaObject received a datagram that it cannot handle");
                }
            }
            return this.value;
        }
    }
    public static class PermaExtensions
    {
        public static TimeSpan timeSinceUpdate<T>(this ISyncPermaObject<T> @this)
        {
            return DateTime.Now.Subtract(@this.getLatestUpdateTime());
        }
    }
    namespace Enumerable
    {
        public class PermaObjectArray<T> : IEnumerable<T>, IDisposable
        {
            internal const string PERMA_ARRAY_ITEM = ".member";
            internal const string PERMA_ARRAY_METADATA_EXTENSION = ".permarrmeta";
            [Serializable]
            private class PermaObjArrayData
            {
                public string ext { get; }
                public int length { get; }
                public PermaObjArrayData(int length, string ext)
                {
                    this.length = length;
                    this.ext = ext;
                }
            }
            private IPermaObject<T>[] _array;
            private readonly IPermaObject<PermaObjArrayData> _length;
            private readonly bool _preserve;
            private readonly Func<byte[], T> _read;
            private readonly Func<T, byte[]>_write;
            public bool SupportMultiAccess { get; set; } = false;
            public PermaObjectArray(int length, Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(length,read, write, name, false, extension) { }
            public PermaObjectArray(int length) : this(length, null, false) { }
            public PermaObjectArray(int length, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(length,a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, preserve, extension) { }
            //if array already exists, the length and extension parameters are ignored
            public PermaObjectArray(int length,Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION)
            {
                _length = new PermaObject<PermaObjArrayData>(name,preserve,PERMA_ARRAY_METADATA_EXTENSION);
                Exception ex;
                _preserve = preserve;
                _length.tryParse(out ex);
                _read = read;
                _write = write;
                if (ex != null)
                    _length.value = new PermaObjArrayData(length,extension);
                this.updateArr(true);
            }
            private void updateArr(bool overridemulti = false)
            {
                if (!SupportMultiAccess | overridemulti)
                    return;
                this._array = Loops.Range(this._length.value.length).SelectToArray( a => new PermaObject<T>(_read, _write, this._length.LocalName() + PERMA_ARRAY_ITEM + a, _preserve, this._length.value.ext));
            }
            public T this[int i]
            {
                get
                {
                    this.updateArr();
                    if (i < 0 || i >= _length.value.length)
                        throw new ArgumentOutOfRangeException("index "+i+" is outside bounds of permaArray");
                    return _array[i].value;
                }
                set
                {
                    this.updateArr();
                    if (i < 0 || i >= _length.value.length)
                        throw new ArgumentOutOfRangeException("index " + i + " is outside bounds of permaArray");
                    _array[i].value = value;
                }
            }
            public void Mutaute(int i, Func<T, T> mutation)
            {
                this.updateArr();
                _array[i].Mutaute(mutation);
            }
            public void Mutaute(int i, Action<T> mutation)
            {
                this.updateArr();
                _array[i].Mutaute(mutation);
            }
            public T tryParse(int i, out Exception ex)
            {
                this.updateArr();
                return _array[i].tryParse(out ex);
            }
            public string name => _length.name;
            public string LocalName() => _length.LocalName();
            public IEnumerator<T> GetEnumerator()
            {
                this.updateArr();
                return _array.Select(a => a.value).GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<T>) this).GetEnumerator();
            }
            protected virtual void Dispose(bool disposing)
            {
                this.updateArr();
                if (disposing)
                {
                    foreach (IPermaObject<T> iPermaObject in _array)
                    {
                        iPermaObject.Dispose();
                    }
                    _length.Dispose();
                }
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            public void resize(int newsize)
            {
                this.updateArr();
                int oldlength = _length.value.length;
                if (oldlength == newsize)
                    return;
                _length.value = new PermaObjArrayData(newsize, _length.value.ext);
                if (oldlength > newsize)
                {
                    foreach (IPermaObject<T> permaObject in _array.Skip(newsize))
                    {
                        permaObject.Dispose();
                        permaObject.delete();
                    }
                }
                _array = Loops.Range(_length.value.length).SelectToArray(a => new PermaObject<T>(_read, _write, _length.LocalName() + PERMA_ARRAY_ITEM + a, _preserve, _length.value.ext));

            }
            public int length
            {
                get
                {
                    this.updateArr();
                    return _length.value.length;
                }
            }
        }
        public class PermaObjectDictionary<T> : IDisposable, IDictionary<string,T>
        {
            internal const string PERMA_DIC_ITEM = ".definition";
            internal const string PERMA_DIC_METADATA_EXTENSION = ".permdicmeta";
            private readonly IPermaObject<string> _definitions;
            private IDictionary<string, IPermaObject<T>> _dic;
            private readonly bool _preserve;
            private readonly Func<byte[], T> _read;
            private readonly Func<T, byte[]> _write;
            private readonly string _extension;
            public bool SupportMultiAccess { get; set; } = false;
            public PermaObjectDictionary() : this(null, false) { }
            public PermaObjectDictionary([CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, preserve, extension) { }
            public PermaObjectDictionary(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(read, write, name, false, extension) { }
            public PermaObjectDictionary(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION)
            {
                _definitions = new PermaObject<string>(name, preserve, PERMA_DIC_METADATA_EXTENSION);
                _preserve = preserve;
                _read = read;
                _write = write;
                _extension = extension;
                this.RefreshDefinitions(true);
            }
            private void RefreshDefinitions(bool overridemulti = false)
            {
                if (!SupportMultiAccess || overridemulti)
                    return;
                Exception ex;
                this._definitions.tryParse(out ex);
                if (ex != null)
                {
                    this._definitions.value = "";
                }
                this._dic = new Dictionary<string, IPermaObject<T>>(this._definitions.value.countappearances(Environment.NewLine));
                foreach (string s in this._definitions.value.Split(Environment.NewLine.ToCharArray()))
                {
                    if (s.Length == 0)
                    {
                        continue;
                    }
                    this._dic[s] = new PermaObject<T>(_read, _write, _definitions.LocalName() + PERMA_DIC_ITEM + s, _preserve, _extension);
                }
            }
            public void Mutaute(string i, Func<T, T> mutation)
            {
                _dic[i].Mutaute(mutation);
            }
            public void Mutaute(string i, Action<T> mutation)
            {
                _dic[i].Mutaute(mutation);
            }
            public T tryParse(string i, out Exception ex)
            {
                this.RefreshDefinitions();
                return _dic[i].tryParse(out ex);
            }
            public bool ContainsKey(string key)
            {
                this.RefreshDefinitions();
                return _dic.ContainsKey(key);
            }
            public void Add(string key, T value)
            {
                this.RefreshDefinitions();
                this[key] = value;
            }
            public bool Remove(string key)
            {
                this.RefreshDefinitions();
                if (!_dic.ContainsKey(key))
                    return false;
                StringBuilder newdef = new StringBuilder(_definitions.value.Length + Environment.NewLine.Length * 2);
                foreach (string s in _definitions.value.Split(Environment.NewLine.ToCharArray()))
                {
                    if (s.Equals(key))
                        continue;
                    newdef.Append(s + Environment.NewLine);
                }
                _definitions.value = newdef.ToString();
                _dic[key].Dispose();
                _dic[key].delete();
                _dic.Remove(key);
                return true;
            }
            public bool TryGetValue(string key, out T value)
            {
                this.RefreshDefinitions();
                value = default(T);
                if (!ContainsKey(key))
                    return false;
                Exception e;
                value = tryParse(key, out e);
                return e == null;
            }
            public T this[string identifier]
            {
                get
                {
                    this.RefreshDefinitions();
                    return _dic[identifier].value;
                }
                set
                {
                    this.RefreshDefinitions();
                    if (identifier.Contains(Environment.NewLine))
                        throw new Exception("cannot create entry with newline in it");
                    if (!_dic.ContainsKey(identifier))
                    {
                        _dic[identifier] = new PermaObject<T>(_read,_write, _definitions.LocalName() + PERMA_DIC_ITEM + identifier,_preserve,_extension);
                        _definitions.value += identifier + Environment.NewLine;
                    }
                    _dic[identifier].value = value;
                }
            }
            public ICollection<string> Keys
            {
                get
                {
                    this.RefreshDefinitions();
                    return _dic.Keys;
                }
            }
            public ICollection<T> Values
            {
                get
                {
                    this.RefreshDefinitions();
                    return _dic.Values.SelectToArray(a => a.value);
                }
            }
            public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
            {
                this.RefreshDefinitions();
                return _dic.Select(a => new KeyValuePair<string,T>(a.Key, a.Value.value)).GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                this.RefreshDefinitions();
                return this.GetEnumerator();
            }
            public void Add(KeyValuePair<string, T> item)
            {
                this.RefreshDefinitions();
                Add(item.Key,item.Value);
            }
            public void Clear()
            {
                this.RefreshDefinitions();
                foreach (string key in Keys)
                {
                    this.Remove(key);
                }
            }
            public bool Contains(KeyValuePair<string, T> item)
            {
                this.RefreshDefinitions();
                return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
            }
            public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
            {
                this.RefreshDefinitions();
                foreach (var key in Keys.CountBind(arrayIndex))
                {
                    array[key.Item2] = new KeyValuePair<string, T>(key.Item1,this[key.Item1]);
                }
            }
            public bool Remove(KeyValuePair<string, T> item)
            {
                this.RefreshDefinitions();
                if (Contains(item))
                {
                    Remove(item.Key);
                    return true;
                }
                return false;
            }
            public int Count
            {
                get
                {
                    this.RefreshDefinitions();
                    return _dic.Count;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    this.RefreshDefinitions();
                    return false;
                }
            }
            protected virtual void Dispose(bool disposing)
            {
                this.RefreshDefinitions();
                if (disposing)
                {
                    foreach (KeyValuePair<string, IPermaObject<T>> iPermaObject in _dic)
                    {
                        iPermaObject.Value.Dispose();
                    }
                    _definitions.Dispose();
                }
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
        public class PermaObjectCollection<T> : ICollection<T>, IDisposable
        {
            internal const string PERMA_COL_METADATA_EXTENSION = ".permcolmeta";
            private readonly PermaObjectDictionary<T> _int;
            private readonly PermaObject<BigInteger> _maxname;
            public PermaObjectCollection() : this(null, false) { }
            public PermaObjectCollection([CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, preserve, extension) { }
            public PermaObjectCollection(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, string extension = PermaObject.PERMA_OBJ_EXTENSION) : this(read, write, name, false, extension) { }
            public PermaObjectCollection(Func<byte[], T> read, Func<T, byte[]> write, [CanBeNull] string name, bool preserve, string extension = PermaObject.PERMA_OBJ_EXTENSION)
            {
                _int = new PermaObjectDictionary<T>(read,write,name,preserve,extension);
                _maxname = new PermaObject<BigInteger>(name,preserve,PERMA_COL_METADATA_EXTENSION);
                Exception ex;
                _maxname.tryParse(out ex);
                if (ex != null)
                    _maxname.value = BigInteger.Zero;
            }
            public IEnumerator<T> GetEnumerator()
            {
                return _int.Select(a=>a.Value).GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            public void Add(T item)
            {
                BigInteger i = _maxname.value++;
                _int[i.ToString("X")] = item;
            }
            public void Clear()
            {
                foreach (string key in _int.Keys)
                {
                    _int.Remove(key);
                }
            }
            public bool Contains(T item)
            {
                return _int.Values.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                _int.Values.CopyTo(array,arrayIndex);
            }
            public bool Remove(T item)
            {
                foreach (var p in _int)
                {
                    if (p.Value.Equals(item))
                    {
                        _int.Remove(p);
                        return true;
                    }
                }
                return false;
            }
            public int Count
            {
                get
                {
                    return _int.Count;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }
            protected virtual void Dispose(bool disposing)
            {
                _int.Dispose();
                _maxname.Dispose();
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
