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
using Edge.Units.Time;
using Edge.WordsPlay;

namespace Edge.PermanentObject
{
    public interface IPermaObject<T> : IDisposable
    {
        T tryParse(out Exception ex);
        T value { get; set; }
        string name { get; }
        FileAccess access { get; }
        FileShare share { get; }
    }
    public static class PermaObject
    {
        public static bool Exists(string name)
        {
            return File.Exists(name);
        }
        public static void MutauteValue<T>(this IPermaObject<T> @this, Func<T, T> mutation)
        {
            @this.value = mutation(@this.value);
        }
        public static string LocalName<T>(this IPermaObject<T> @this)
        {
            var s = @this.name;
            return System.IO.Path.GetFileName(s);
        }
        public static bool Readable<T>(this IPermaObject<T> @this)
        {
            Exception ex;
            var temp = @this.tryParse(out ex);
            return ex == null;
        }
        public static TimeSpan timeSinceUpdate<T>(this ISyncPermaObject<T> @this)
        {
            return DateTime.Now.Subtract(@this.getLatestUpdateTime());
        }
    }
    public class PermaObject<T> : IPermaObject<T>
    {
        private readonly FileStream _stream;
        public FileAccess access { get; }
        public FileShare share { get; }
        private readonly Func<byte[], T> _read;
        private readonly Func<T, byte[]> _write;
        public PermaObject(string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T)) : this(a => (T)Serialization.Deserialize(a), a=>Serialization.Serialize(a), name, deleteOnDispose, access, share, mode, valueIfCreated) { }
        public PermaObject(Func<byte[], T> read, Func<T, byte[]> write, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T))
        {
            if (mode == FileMode.Truncate || mode == FileMode.Append)
                throw new ArgumentException("truncate and append modes are not supported", nameof(mode));
            bool create = !PermaObject.Exists(name);
            if (mode != FileMode.Open && valueIfCreated == null)
                throw new ArgumentException("is the default value is null, the PermaObject cannot be newly created");
            if (deleteOnDispose && (!create && share != FileShare.None))
                throw new ArgumentException("delete on dispose demands the file not previously exist or that sharing will be none", nameof(deleteOnDispose));
            this._read = read;
            this._write = write;
            this.access = access;
            this.share = share;
            FileOptions options = FileOptions.SequentialScan;
            if (share != FileShare.None)
                options |= FileOptions.Asynchronous;
            if (deleteOnDispose)
                options |= FileOptions.DeleteOnClose;
            _stream = new FileStream(name, mode, access, share,4096,options);
            if (create)
                this.value = valueIfCreated;

        }
        public T tryParse(out Exception ex)
        {
            if (!access.HasFlag(FileAccess.Read))
                throw new AccessViolationException("permaobject is set not to read");
            const int buffersize = 4096;
            try
            {
                ex = null;
                var b = LoadFiles.loadAsBytes(_stream, buffersize);
                return _read(b);
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
                if (!access.HasFlag(FileAccess.Write))
                    throw new AccessViolationException("permaobject is set not to write");
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
            if (disposing)
                (this._stream as IDisposable).Dispose();
        }
    }
    public interface ISyncPermaObject<T> : IPermaObject<T>
    {
        T getFresh(DateTime earliestTime);
        T getFresh(TimeSpan maxInterval);
        DateTime getLatestUpdateTime();
    }
    public class SyncPermaObject<T> : ISyncPermaObject<T>
    {
        internal const string PERMA_OBJ_UPDATE_EXTENSION = ".permaobjupdate";
        private readonly PermaObject<T> _int;
        private readonly PermaObject<DateTime> _update;
        public SyncPermaObject([CanBeNull] string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T)) : this(a => (T)Serialization.Deserialize(a), a=>Serialization.Serialize(a), name, deleteOnDispose, access, share, mode, valueIfCreated) { }
        public SyncPermaObject(Func<byte[], T> read, Func<T, byte[]> write, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T))
        {
            _int = new PermaObject<T>(read,write,name,deleteOnDispose, access, share, mode, valueIfCreated);
            _update = new PermaObject<DateTime>(FilePath.MutateFileName(name, a=> "__LATESTUPDATE_"+a), deleteOnDispose, access, share, mode, DateTime.Now);
        }
        public T getFresh(TimeSpan maxInterval)
        {
            return getFresh(maxInterval, maxInterval.Divide(2));
        }
        public T getFresh(TimeSpan maxInterval, TimeSpan checkinterval)
        {
            while (this.timeSinceUpdate() > maxInterval)
            {
                Thread.Sleep(checkinterval);
            }
            return this.value;
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
        public DateTime getLatestUpdateTime()
        {
            Exception e;
            var a = _update.tryParse(out e);
            return (e == null) ? a : DateTime.MinValue;
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
        public FileAccess access
        {
            get
            {
                return _int.access;
            }
        }
        public FileShare share
        {
            get
            {
                return _int.share;
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
        public PortBoundPermaObject([CanBeNull] string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T)) : this(a => (T)Serialization.Deserialize(a), a=>Serialization.Serialize(a), name, deleteOnDispose, access, share, mode, valueIfCreated) { }
        public PortBoundPermaObject(Func<byte[], T> read, Func<T, byte[]> write, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T))
        {
            Exception ex;
            this._permaPort = new PermaObject<EndPoint>(FilePath.MutateFileName(name, a => "__PORT_" + a), deleteOnDispose,FileAccess.ReadWrite,FileShare.ReadWrite,mode, new IPEndPoint(IPAddress.None, 0));
            EndPoint port = this._permaPort.tryParse(out ex);
            if (ex != null || ((IPEndPoint)port).Address == IPAddress.None)
            {
                //port is undefined
                _conn = MultiSocket.ConnectToStrippedMultiSocket(out port, new OpenCredentialValidator(), new Credential());
                this._permaPort.value = port;
            }
            else
            {
                //port is defined
                _conn = MultiSocket.ConnectToStrippedMultiSocket(port, new OpenCredentialValidator(), new Credential());
            }
            _int = new SyncPermaObject<T>(read,write,name,deleteOnDispose,access,share,mode,valueIfCreated);

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
        public FileAccess access
        {
            get
            {
                return _int.access;
            }
        }
        public FileShare share
        {
            get
            {
                return _int.share;
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
        public T getFresh(TimeSpan maxInterval)
        {
            while (this.timeSinceUpdate() > maxInterval)
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
    namespace Enumerable
    {
        public class PermaArray<T> : IList<T>, IDisposable
        {
            [Serializable]
            private class PermaObjArrayData
            {
                public int length { get; }
                public PermaObjArrayData(int length)
                {
                    this.length = length;
                }
            }
            private IPermaObject<T>[] _array;
            private readonly IPermaObject<PermaObjArrayData> _length;
            private readonly Func<byte[], T> _read;
            private readonly Func<T, byte[]>_write;
            private readonly bool _deleteOnDispose;
            private readonly T _valueIfCreated;
            public bool SupportMultiAccess => _length.share != FileShare.None;
            public PermaArray(int length, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T)) : this(length,a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, deleteOnDispose, access, share, mode, valueIfCreated) { }
            //if array already exists, the length parameter are ignored
            public PermaArray(int length,Func<byte[], T> read, Func<T, byte[]> write, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate, T valueIfCreated = default(T))
            {
                _read = read;
                _write = write;
                _deleteOnDispose = deleteOnDispose;
                _valueIfCreated = valueIfCreated;
                this.name = name;
                _length = new PermaObject<PermaObjArrayData>(FilePath.MutateFileName(name, a => "__ARRAYDATA_" + a), deleteOnDispose, access, share, mode, new PermaObjArrayData(length));
                this.updateArr(true);
            }
            private void updateArr(bool overridemulti = false)
            {
                if (SupportMultiAccess || overridemulti)
                    this._array = Loops.Range(this._length.value.length).SelectToArray( a => new PermaObject<T>(_read, _write,
                                    FilePath.MutateFileName(name, k => "__ARRAYMEMBER_"+a+"_" + k), _deleteOnDispose,
                                    _length.access, _length.share, valueIfCreated: _valueIfCreated));
            }
            public int IndexOf(T item)
            {
                return (_array.CountBind().FirstOrDefault(a => a.Item1.value.Equals(item)) ?? Tuple.Create((IPermaObject<T>)null, -1)).Item2;
            }
            void IList<T>.Insert(int index, T item)
            {
                throw new NotSupportedException();
            }
            void IList<T>.RemoveAt(int index)
            {
                throw new NotSupportedException();
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
            public void MutauteValue(int i, Func<T, T> mutation)
            {
                this.updateArr();
                _array[i].MutauteValue(mutation);
            }
            public T tryParse(int i, out Exception ex)
            {
                this.updateArr();
                return _array[i].tryParse(out ex);
            }
            public string name { get; }
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
                _length.value = new PermaObjArrayData(newsize);
                if (oldlength > newsize)
                {
                    foreach (IPermaObject<T> permaObject in _array.Skip(newsize))
                    {
                        permaObject.Dispose();
                        if (!_deleteOnDispose)
                            File.Delete(permaObject.name);
                    }
                    Array.Resize(ref _array,newsize);
                }
                else
                {
                    Array.Resize(ref _array, newsize);
                    _array.Fill(a => new PermaObject<T>(_read, _write,
                                        FilePath.MutateFileName(name, k => "__ARRAYMEMBER_" + a + "_" + k), _deleteOnDispose,
                                        _length.access, _length.share, valueIfCreated: _valueIfCreated), oldlength);
                }

            }
            public int length
            {
                get
                {
                    this.updateArr();
                    return _length.value.length;
                }
            }
            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException();
            }
            void ICollection<T>.Clear()
            {
                throw new NotSupportedException();
            }
            public bool Contains(T item)
            {
                return IndexOf(item) > 0;
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                foreach (var t in _array.CountBind(arrayIndex))
                {
                    array[t.Item2] = t.Item1.value;
                }
            }
            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }
            public int Count
            {
                get
                {
                    return _length.value.length;
                }
            }
            public bool IsReadOnly => false;
        }
        public class PermaDictionary<T> : IDisposable, IDictionary<string,T>
        {
            private readonly IPermaObject<string> _definitions;
            private IDictionary<string, IPermaObject<T>> _dic;
            private readonly bool _deleteondispose;
            private readonly Func<byte[], T> _read;
            private readonly Func<T, byte[]> _write;
            private readonly string _defSeperator;
            private int _holdUpdateFlag = 0;
            public string name { get; }
            public bool SupportMultiAccess => (_definitions.share != FileShare.None);
            public PermaDictionary(string name, string defSeperator=null, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, defSeperator, deleteOnDispose,access,share,mode) { }
            public PermaDictionary(Func<byte[], T> read, Func<T, byte[]> write, string name, string defSeperator=null, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate)
            {
                _definitions = new PermaObject<string>(name, deleteOnDispose,access,share, mode,"");
                _deleteondispose = deleteOnDispose;
                _read = read;
                _write = write;
                this.name = name;
                _defSeperator = defSeperator ?? Environment.NewLine;
                this.RefreshDefinitions(true);
            }
            private void RefreshDefinitions(bool overridemulti = false)
            {
                if ((SupportMultiAccess || overridemulti) && (_holdUpdateFlag == 0))
                {
                    Exception ex;
                    this._definitions.tryParse(out ex);
                    if (ex != null)
                        this._definitions.value = "";
                    var defstring = this._definitions.value;
                    this._dic = new Dictionary<string, IPermaObject<T>>(defstring.countappearances(Environment.NewLine));
                    var keys = (defstring == ""
                        ? System.Linq.Enumerable.Empty<string>() : defstring.Split(new string[] {Environment.NewLine}, StringSplitOptions.None));
                    foreach (string s in keys.Take(Math.Max(0,keys.Count()-1)))
                    {
                        this._dic[s] = new PermaObject<T>(_read, _write,
                            FilePath.MutateFileName(name, k => "__DICTIONARYMEMBER_" + s + "_" + k), _deleteondispose,_definitions.access, _definitions.share, FileMode.Open );
                    }
                }
            }
            public void MutauteValue(string i, Func<T, T> mutation)
            {
                _dic[i].MutauteValue(mutation);
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
                _holdUpdateFlag++;
                this[key] = value;
                _holdUpdateFlag--;
            }
            public bool Remove(string key)
            {
                this.RefreshDefinitions();
                if (!_dic.ContainsKey(key))
                    return false;
                StringBuilder newdef = new StringBuilder(_definitions.value.Length + Environment.NewLine.Length * 2);
                foreach (string s in _definitions.value.Split(new string[] {_defSeperator}, StringSplitOptions.None))
                {
                    if (s.Equals(key))
                        continue;
                    newdef.Append(s + _defSeperator);
                }
                _definitions.value = newdef.ToString();
                _dic[key].Dispose();
                if (!_deleteondispose)
                    File.Delete(_dic[key].name);
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
                    if (identifier.Contains(_defSeperator))
                        throw new Exception("cannot create entry with the separator in it");
                    if (!_dic.ContainsKey(identifier))
                    {
                        _dic[identifier] = new PermaObject<T>(_read, _write,
                            FilePath.MutateFileName(name, k => "__DICTIONARYMEMBER_" + identifier + "_" + k), _deleteondispose, _definitions.access, _definitions.share, FileMode.Create);
                        _definitions.value += identifier + _defSeperator;
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
                _holdUpdateFlag++;
                Add(item.Key,item.Value);
                _holdUpdateFlag--;
            }
            public void Clear()
            {
                this.RefreshDefinitions();
                _holdUpdateFlag++;
                foreach (string key in Keys)
                {
                    this.Remove(key);
                }
                _holdUpdateFlag--;
            }
            public bool Contains(KeyValuePair<string, T> item)
            {
                this.RefreshDefinitions();
                _holdUpdateFlag++;
                var contains = ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
                _holdUpdateFlag--;
                return contains;
            }
            public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
            {
                this.RefreshDefinitions();
                _holdUpdateFlag++;
                foreach (var key in Keys.CountBind(arrayIndex))
                {
                    array[key.Item2] = new KeyValuePair<string, T>(key.Item1,this[key.Item1]);
                }
                _holdUpdateFlag--;
            }
            public bool Remove(KeyValuePair<string, T> item)
            {
                this.RefreshDefinitions();
                _holdUpdateFlag++;
                var t = Contains(item);
                if (t)
                {
                    Remove(item.Key);
                    _holdUpdateFlag--;
                    return true;
                }
                _holdUpdateFlag--;
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
                    return !_definitions.access.HasFlag(FileAccess.Write);
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
        public class PermaCollection<T> : ICollection<T>, IDisposable
        {
            private readonly PermaDictionary<T> _int;
            private readonly PermaObject<long> _maxname;
            public PermaCollection(string name,  bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate) : this(a => (T)Serialization.Deserialize(a), a => Serialization.Serialize(a), name, deleteOnDispose,access,share,mode) { }
            public PermaCollection(Func<byte[], T> read, Func<T, byte[]> write, string name, bool deleteOnDispose = false, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.None, FileMode mode = FileMode.OpenOrCreate)
            {
                _int = new PermaDictionary<T>(read,write,name,null, deleteOnDispose, access, share, mode);
                _maxname = new PermaObject<long>(FilePath.MutateFileName(name, k => "__COLLECTIONMAXINDEX_" + k), deleteOnDispose, access, share, mode);
            }
            public IEnumerator<T> GetEnumerator()
            {
                return _int.Values.GetEnumerator();
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
                _int.Clear();
                _maxname.value = 0;
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
                    return _int.IsReadOnly;
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
