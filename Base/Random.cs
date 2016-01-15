using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using Edge.Arrays;
using Edge.Looping;
using Edge.WordsPlay;

namespace Edge.RandomGen
{
    public abstract class RandomGenerator
    {
        public virtual byte[] Bytes(int length)
        {
            return arrayExtensions.Fill(length, () => (byte)Int(0, byte.MaxValue));
        }
        public int Int(int max)
        {
            return Int(0, max);
        }
        public virtual int Int(int min, int max)
        {
            byte[] buf = Bytes(4);
            var @base = BitConverter.ToInt32(buf, 0);
            return (Math.Abs(@base % (max - min)) + min);
        }
        public int Int(int min, int max, bool inclusive)
        {
            return Int(min, max + (inclusive ? 1 : 0));
        }
        public long Long(long max)
        {
            return Long(0, max);
        }
        public virtual long Long(long min, long max)
        {
            byte[] buf = Bytes(8);
            var @base = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(@base % (max - min)) + min);
        }
        public long Long(long min, long max, bool inclusive)
        {
            return Long(min, max + (inclusive ? 1 : 0));
        }
        public ulong ULong(ulong max)
        {
            return ULong(0, max);
        }
        public virtual ulong ULong(ulong min, ulong max)
        {
            byte[] buf = Bytes(8);
            var @base = BitConverter.ToUInt64(buf, 0);
            return (@base % (max - min)) + min;
        }
        public ulong ULong(ulong min, ulong max, bool inclusive)
        {
            return ULong(min, max + (inclusive ? 1U : 0U));
        }
        public virtual double Double()
        {
            return Double(1);
        }
        public double Double(double max)
        {
            return Double(0, max);
        }
        public virtual double Double(double min, double max)
        {
            while (true)
            {
                var @base = ULong(ulong.MaxValue) / (double)ULong(ULong(ulong.MaxValue));
                @base *= max;
                if (double.IsNaN(@base) || double.IsInfinity(@base))
                    continue;
                return (Math.Abs(@base % (max - min)) + min);
            }
        }
        public bool success(double odds)
        {
            if (odds >= 1)
                return true;
            if (odds <= 0)
                return false;
            return Double() <= odds;
        }
        public bool randombool(int trueodds = 1, int falseodds = 1)
        {
            return success(trueodds / (double)(falseodds + trueodds));
        }
        public char randomchar(char min = 'a', char max = 'z')
        {
            return (char)(Int(min, max, true));
        }
        public char randomchar(string allowedchars)
        {
            return allowedchars[Int(0, allowedchars.Length)];
        }
        public object randomenum(Type e)
        {
            if (!e.IsEnum)
                throw new Exception("type is not an enum");
            return Enum.ToObject(e, Int(0, Enum.GetNames(e).Length));
        }
        public Color randomcolor()
        {
            return Color.FromArgb(Int(0, 256), Int(0, 256), Int(0, 256));
        }
        public string String(int length, char[] allowedChars)
        {
            return Loops.Range(length).Select(a => allowedChars[Int(allowedChars.Length)]).convertToString();
        }
    }
    public abstract class ByteEnumeratorGenerator : RandomGenerator
    {
        protected abstract IEnumerator<byte> Bytes();
        public override byte[] Bytes(int length)
        {
            List<byte> ret = new List<byte>(length);
            int c = length;
            while (true)
            {
                var tor = Bytes();
                while (tor.MoveNext())
                {
                    ret.Add(tor.Current);
                    c--;
                    if (c <= 0)
                        return ret.ToArray();
                }
            }
        }
    }
    public class LocalRandomGenerator : RandomGenerator
    {
        private readonly Random _int;
        public LocalRandomGenerator()
        {
            _int = new Random();
        }
        public LocalRandomGenerator(int seed)
        {
            _int = new Random(seed);
        }
        public override byte[] Bytes(int length)
        {
            byte[] ret = new byte[length];
            _int.NextBytes(ret);
            return ret;
        }
        public override double Double()
        {
            return _int.NextDouble();
        }
        public override double Double(double min, double max)
        {
            return (max - min) * _int.NextDouble() + min;
        }
        public override int Int(int min, int max)
        {
            return _int.Next(min,max);
        }
    }
    public class GlobalRandomGenerator : RandomGenerator
    {
        private static readonly IDictionary<Thread, RandomGenerator> _dic = new Dictionary<Thread, RandomGenerator>(1);
        private static void EnsureGeneratorExists(Thread t)
        {
            var ret = _dic.ContainsKey(t);
            if (!ret)
                reset(t);
        }
        public override byte[] Bytes(int length)
        {
            EnsureGeneratorExists(Thread.CurrentThread);
            return _dic[Thread.CurrentThread].Bytes(length);
        }
        public override double Double()
        {
            EnsureGeneratorExists(Thread.CurrentThread);
            return _dic[Thread.CurrentThread].Double();
        }
        public override double Double(double min, double max)
        {
            EnsureGeneratorExists(Thread.CurrentThread);
            return _dic[Thread.CurrentThread].Double(min,max);
        }
        public override int Int(int min, int max)
        {
            EnsureGeneratorExists(Thread.CurrentThread);
            return _dic[Thread.CurrentThread].Int(min,max);
        }
        public static void reset()
        {
            reset(Thread.CurrentThread);
        }
        public static void reset(Thread t)
        {
            reset(DateTime.Now.GetHashCode() ^ Process.GetCurrentProcess().GetHashCode() ^ Thread.CurrentThread.GetHashCode(),t);
        }
        public static void reset(int seed)
        {
            reset(seed, Thread.CurrentThread);
        }
        public static void reset(int seed, Thread t)
        {
            _dic[t] = new LocalRandomGenerator(seed);
        }
    }
    public class ConstantRandomGenerator : RandomGenerator
    {
        public byte val { get; }
        public ConstantRandomGenerator(byte val)
        {
            this.val = val;
        }
        public override byte[] Bytes(int length)
        {
            return arrayExtensions.Fill(length, () => val);
        }
        public override double Double(double min, double max)
        {
            var @base = val;
            return (Math.Abs(@base % (max - min)) + min);
        }
        public override int Int(int min, int max)
        {
            var @base = val;
            return (Math.Abs(@base % (max - min)) + min);
        }
        public override ulong ULong(ulong min, ulong max)
        {
            var @base = val;
            return (@base % (max - min) + min);
        }
        public override long Long(long min, long max)
        {
            var @base = val;
            return (Math.Abs(@base % (max - min)) + min);
        }
    }
    public class ShaGenerator : ByteEnumeratorGenerator, IDisposable
    {
        private readonly SHA256 _sha;
        private readonly IList<byte> _seed;
        public ShaGenerator(byte[] seed)
        {
            _sha = SHA256.Create();
            _seed = new List<byte>(seed);
        }
        protected override IEnumerator<byte> Bytes()
        {
            while (true)
            {
                _sha.ComputeHash(_seed.ToArray().Shuffle());
                _seed.Clear();
                foreach (byte b in _sha.Hash)
                {
                    yield return b;
                    _seed.Add(b);
                }
            }
        }
        public void Dispose()
        {
            ((IDisposable)_sha).Dispose();
        }
    }
    namespace ThreadEntropy
    {
        public class EntropyRandomGenerator : ByteEnumeratorGenerator, IDisposable
        {
            private volatile int _val = 0;
            private readonly Thread[] _runners;
            public EntropyRandomGenerator(int threadCount = 2, ThreadPriority priority = ThreadPriority.Lowest)
            {
                _runners = new Thread[threadCount];
                for (int i = 0; i < threadCount; i++)
                {
                    _runners[i] = new Thread(ThreadProcedure);
                    _runners[i].Priority = priority;
                    _runners[i].Start();
                }
            }
            private void ThreadProcedure()
            {
                while (true)
                    _val++;
            }
            protected virtual void Dispose(bool disposing)
            {
                foreach (Thread runner in _runners)
                {
                    runner.Abort();
                }
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected override IEnumerator<byte> Bytes()
            {
                byte[] intBytes = BitConverter.GetBytes(_val);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(intBytes);
                return (IEnumerator<byte>)intBytes.GetEnumerator();
            }
            public override int Int(int min, int max)
            {
                return (Math.Abs(_val % (max - min)) + min);
            }
        }
    }
}