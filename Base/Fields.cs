using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using CCDefault.Annotations;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Microsoft.CSharp.RuntimeBinder;
using Numerics;

namespace Edge.Fielding {
    public class FieldWrapper<T> : IComparable<T>, IComparable<FieldWrapper<T>>
    {
        private static readonly Field<T> _field = Fields.getField<T>();
        public FieldWrapper(T val)
        {
            this.val = val;
        }
        public FieldWrapper(int i)
        {
            val = _field.fromInt(i);
        }
        public FieldWrapper(ulong i)
        {
            val = _field.fromInt(i);
        }
        public T val { get; }
        public static implicit operator FieldWrapper<T>(T w)
        {
            return new FieldWrapper<T>(w);
        }
        public static implicit operator FieldWrapper<T>(int w)
        {
            return new FieldWrapper<T>(w);
        }
        public static implicit operator FieldWrapper<T>(ulong w)
        {
            return new FieldWrapper<T>(w);
        }
        public FieldWrapper<T> abs()
        {
            return _field.abs(val);
        }
        public FieldWrapper<T> pow(FieldWrapper<T> p)
        {
            return _field.pow(val, p);
        }
        public FieldWrapper<T> pow(int p)
        {
            return _field.Pow(val, p);
        }
        public FieldWrapper<T> TrueMod(T mod)
        {
            return (mod + (this % mod)) % mod;
        }
        public FieldWrapper<T> Invert()
        {
            return _field.Invert(val);
        } 
        public static implicit operator T(FieldWrapper<T> w)
        {
            return w.val;
        }
        public static explicit operator double?(FieldWrapper<T> w)
        {
            return _field.toDouble(w.val);
        }
        public static explicit operator double (FieldWrapper<T> w)
        {
            var d = _field.toDouble(w.val);
            if (d != null)
                return d.Value;
            throw new InvalidCastException("cannot cast wrapper to double because the field's double conversion method returned null");
        }
        public static FieldWrapper<T> operator +(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.add(w1, w2);
        }
        public static FieldWrapper<T> operator *(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.multiply(w1, w2);
        }
        public static FieldWrapper<T> operator -(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.subtract(w1, w2);
        }
        public static FieldWrapper<T> operator -(FieldWrapper<T> w1)
        {
            return _field.Negate(w1);
        }
        public static FieldWrapper<T> operator /(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.divide(w1, w2);
        }
        public static FieldWrapper<T> operator %(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.mod(w1, w2);
        }
        public static FieldWrapper<T> operator ^(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.pow(w1, w2);
        }
        public static FieldWrapper<T> operator ^(FieldWrapper<T> w1, int w2)
        {
            return _field.Pow(w1, w2);
        }
        public static FieldWrapper<T> operator ~(FieldWrapper<T> w1)
        {
            return _field.Conjugate(w1);
        }
        public static bool operator <=(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.Compare(w1, w2) <= 0;
        }
        public static bool operator >=(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.Compare(w1, w2) >= 0;
        }
        public static bool operator <(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.Compare(w1, w2) < 0;
        }
        public static bool operator >(FieldWrapper<T> w1, FieldWrapper<T> w2)
        {
            return _field.Compare(w1, w2) > 0;
        }
        public override bool Equals(object obj)
        {
            return obj is T && this.CompareTo((T)(obj)) == 0;
        }
        public override int GetHashCode()
        {
            return val.GetHashCode();
        }
        public int CompareTo(T other)
        {
            return _field.Compare(val, other);
        }
        public int CompareTo(FieldWrapper<T> other)
        {
            return _field.Compare(val, other.val);
        }
        public Field<T> Field
        {
            get
            {
                return _field;
            }
        }
    }
    public interface Field
    {
        Type getSubjectType();
    }
    [Flags] public enum OrderType
        {
            ReflexiveZero = 1, AntiSymmetric = 2, Transitive = 4, Total = 8,
            NoOrder = 0, PartialOrder = ReflexiveZero|AntiSymmetric|Transitive, TotalOrder = PartialOrder|Total
        }
    public class Field<T> : IComparer<T>, IEqualityComparer<T>, Field
    {
        public Field() : this(default(T), default(T), default(T)) { }
        public Field(T zero, T one, T naturalbase)
        {
            this.zero = zero;
            this.one = one;
            this.naturalbase = naturalbase;
        }
        public virtual T zero { get; }
        public virtual T one { get; }
        public virtual T naturalbase { get; }
        public virtual T negativeone => Negate(one);
        public virtual T Negate(T x)
        {
            return -((dynamic)x);
        }
        public virtual T Invert(T x)
        {
            return divide(one,x);
        }
        public virtual T multiply(T a, T b)
        {
            return (dynamic)a * b;
        }
        public virtual T divide(T a, T b)
        {
            return (dynamic)a / b;
        }
        public virtual T add(T a, T b)
        {
            return (dynamic)a + b;
        }
        public virtual T subtract(T a, T b)
        {
            return (dynamic)a - b;
        }
        public virtual T Conjugate(T a)
        {
            try
            {
                return ((dynamic) a).conjugate();
            }
            catch (RuntimeBinderException)
            {
                return a;
            }
        }
        public virtual T pow(T a, T b)
        {
            try
            {
                return ((dynamic)a).pow(b);
            }
            catch (RuntimeBinderException)
            {
                return (dynamic)a^b;
            }
        }
        public virtual T log(T a)
        {
            return ((dynamic)a).log();
        }
        public virtual T mod(T a, T b)
        {
            return (dynamic)a % b;
        }
        public virtual double? toDouble(T a)
        {
            return ((dynamic)this.abs(a));
        }
        public virtual string String(T a)
        {
            return a.ToString();
        }
        public virtual bool Invertible
        {
            get
            {
                return true;
            }
        }
        public virtual bool Negatable
        {
            get
            {
                return true;
            }
        }
        public virtual bool ModduloAble
        {
            get
            {
                return true;
            }
        }
        public virtual bool isNegative(T x)
        {
            return this.Compare(zero, x) > 0;
        }
        public virtual OrderType Order => OrderType.TotalOrder;
        public virtual int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(x, y);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Field<T>))
            {
                return false;
            }
            return IdDistributer.getId(this) == IdDistributer.getId(obj);
        }
        public override int GetHashCode()
        {
            return IdDistributer.getId(this).GetHashCode();
        }
        public Type getSubjectType()
        {
            return typeof (T);
        }
        public virtual T abs(T x)
        {
            return isNegative(x) && this.Negatable ? Negate(x) : x;
        }
        public virtual T fromInt(ulong x)
        {
            switch (x)
            {
                case 0:
                    return this.zero;
                case 1:
                    return this.one;
            }
            T h = fromInt(x / 2);
            T sum = add(h, h);
            if (x % 2 == 1)
                sum = add(sum, one);
            return sum;
        }
        public virtual T fromInt(int x)
        {
            return x < 0 ? this.Negate(this.fromInt((ulong)-x)) : this.fromInt((ulong) x);
        }
        public virtual T fromFraction(int numerator, int denumerator)
        {
            return divide(fromInt(numerator), fromInt(denumerator));
        }
        public virtual T Pow(T @base, int x)
        {
            return pow(@base, fromInt(x));
        }
        public virtual T Factorial(int x)
        {
            T ret = one;
            T tomult = zero;
            for(int i = 0; i <= x; i++)
            {
                tomult = add(tomult, one);
                ret = multiply(ret, tomult);
            }
            return ret;
        }
        public bool Equals(T x, T y)
        {
            return Compare(x,y) == 0;
        }
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
        public virtual bool Parsable => false;
        public virtual T Parse(string s)
        {
            throw new NotSupportedException();
        }
    }
    public static class Fields
    {
        #region stock
        private class DoubleField : Field<double>
        {
            public DoubleField() : base(0, 1, Math.E) { }
            public override double add(double a, double b) => a+b;
            public override double pow(double a, double b) => Math.Pow(a,b);
            public override int Compare(double x, double y) => x.CompareTo(y);
            public override double Factorial(int x) => x.factorial();
            public override double fromInt(int x) => x;
            public override double fromInt(ulong x) => x;
            public override double abs(double x) => Math.Abs(x);
            public override double Conjugate(double a) => a;
            public override double divide(double a, double b) => a/b;
            public override double mod(double a, double b) => a%b;
            public override double fromFraction(int numerator, int denumerator) => numerator/(double)denumerator;
            public override double Invert(double x) => 1/x;
            public override bool isNegative(double x) => x < 0;
            public override double log(double a) => Math.Log(a);
            public override double multiply(double a, double b) => a*b;
            public override double Negate(double x) => -x;
            public override double subtract(double a, double b) => a-b;
            public override double? toDouble(double a) => a;
            public override bool Parsable => true;
            public override double Parse(string s)
            {
                return double.Parse(s);
            }
        }
        private class FloatField : Field<float>
        {
            public FloatField() : base(0, 1, (float) Math.E) { }
            public override float add(float a, float b) => a + b;
            public override float pow(float a, float b) => a.pow(b);
            public override int Compare(float x, float y) => x.CompareTo(y);
            public override float Factorial(int x) => x.factorial();
            public override float fromInt(int x) => x;
            public override float fromInt(ulong x) => x;
            public override float abs(float x) => Math.Abs(x);
            public override float Conjugate(float a) => a;
            public override float divide(float a, float b) => a / b;
            public override float mod(float a, float b) => a % b;
            public override float fromFraction(int numerator, int denumerator) => numerator / (float)denumerator;
            public override float Invert(float x) => 1 / x;
            public override bool isNegative(float x) => x < 0;
            public override float log(float a) => (float)Math.Log(a);
            public override float multiply(float a, float b) => a * b;
            public override float Negate(float x) => -x;
            public override float subtract(float a, float b) => a - b;
            public override double? toDouble(float a) => a;
            public override bool Parsable => true;
            public override float Parse(string s)
            {
                return float.Parse(s);
            }
        }
        private class DecimalField : Field<decimal>
        {
            public DecimalField() : base(0, 1, (decimal)Math.E) { }
            public override decimal add(decimal a, decimal b) => a + b;
            public override decimal pow(decimal a, decimal b) => a.pow(b);
            public override int Compare(decimal x, decimal y) => x.CompareTo(y);
            public override decimal Factorial(int x) => x.factorial();
            public override decimal fromInt(int x) => x;
            public override decimal fromInt(ulong x) => x;
            public override decimal abs(decimal x) => Math.Abs(x);
            public override decimal Conjugate(decimal a) => a;
            public override decimal divide(decimal a, decimal b) => a / b;
            public override decimal mod(decimal a, decimal b) => a % b;
            public override decimal fromFraction(int numerator, int denumerator) => numerator / (decimal)denumerator;
            public override decimal Invert(decimal x) => 1 / x;
            public override bool isNegative(decimal x) => x < 0;
            public override decimal log(decimal a) => (decimal)Math.Log((double) a);
            public override decimal multiply(decimal a, decimal b) => a * b;
            public override decimal Negate(decimal x) => -x;
            public override decimal subtract(decimal a, decimal b) => a - b;
            public override double? toDouble(decimal a) => (double) a;
            public override bool Parsable => true;
            public override decimal Parse(string s)
            {
                return decimal.Parse(s);
            }
        }
        private class IntField : Field<int>
        {
            public IntField() : base(0, 1, 2) { }
            public override int add(int a, int b) => a + b;
            public override int pow(int a, int b) => a.pow(b);
            public override int Compare(int x, int y) => x.CompareTo(y);
            public override int Factorial(int x) => (int)x.factorial();
            public override int fromInt(int x) => x;
            public override int fromInt(ulong x) => (int)x;
            public override int abs(int x) => Math.Abs(x);
            public override int Conjugate(int a) => a;
            public override int divide(int a, int b) => a / b;
            public override int fromFraction(int numerator, int denumerator) => numerator / denumerator;
            public override int Invert(int x) => 1 / x;
            public override bool isNegative(int x) => x < 0;
            public override int log(int a) => (int)Math.Log(a, naturalbase);
            public override int multiply(int a, int b) => a * b;
            public override int mod(int a, int b) => a % b;
            public override int Negate(int x) => -x;
            public override int subtract(int a, int b) => a - b;
            public override double? toDouble(int a) => a;
            public override bool Invertible => false;
            public override bool Parsable => true;
            public override int Parse(string s)
            {
                return int.Parse(s);
            }
        }
        private class LongField : Field<long>
        {
            public LongField() : base(0, 1, (long)Math.E) { }
            public override long add(long a, long b) => a + b;
            public override long pow(long a, long b) => a.pow(b);
            public override int Compare(long x, long y) => x.CompareTo(y);
            public override long Factorial(int x) => (long) x.factorial();
            public override long fromInt(int x) => x;
            public override long fromInt(ulong x) => (long) x;
            public override long abs(long x) => Math.Abs(x);
            public override long Conjugate(long a) => a;
            public override long divide(long a, long b) => a / b;
            public override long mod(long a, long b) => a % b;
            public override long fromFraction(int numerator, int denumerator) => numerator / (long)denumerator;
            public override long Invert(long x) => 1 / x;
            public override bool isNegative(long x) => x < 0;
            public override long log(long a) => (long)Math.Log(a, naturalbase);
            public override long multiply(long a, long b) => a * b;
            public override long Negate(long x) => -x;
            public override long subtract(long a, long b) => a - b;
            public override double? toDouble(long a) => a;
            public override bool Invertible => false;
            public override bool Parsable => true;
            public override long Parse(string s)
            {
                return long.Parse(s);
            }
        }
        private class UIntField : Field<uint>
        {
            public UIntField() : base(0, 1, (uint)Math.E) { }
            public override uint add(uint a, uint b) => a + b;
            public override uint pow(uint a, uint b) => a.pow(b);
            public override int Compare(uint x, uint y) => x.CompareTo(y);
            public override uint Factorial(int x) => (uint)x.factorial();
            public override uint fromInt(int x)
            {
                if (x < 0)
                    throw new Exception("cannot get unsigned from negative int");
                return (uint) x;
            }
            public override uint fromInt(ulong x) => (uint)x;
            public override uint abs(uint x) => x;
            public override uint Conjugate(uint a) => a;
            public override uint divide(uint a, uint b) => a / b;
            public override uint mod(uint a, uint b) => a % b;
            public override uint fromFraction(int numerator, int denumerator) => (uint) (numerator / denumerator);
            public override uint Invert(uint x) => 1 / x;
            public override bool isNegative(uint x) => false;
            public override uint log(uint a) => (uint)Math.Log(a, naturalbase);
            public override uint multiply(uint a, uint b) => a * b;
            public override uint Negate(uint x)
            {
                if (x == 0)
                    return 0;
                throw new Exception("cannot negate unsigned");
            }
            public override uint subtract(uint a, uint b) => a - b;
            public override double? toDouble(uint a) => a;
            public override bool Invertible => false;
            public override bool Negatable => false;
            public override bool Parsable => true;
            public override uint Parse(string s)
            {
                return uint.Parse(s);
            }
        }
        private class ULongField : Field<ulong>
        {
            public ULongField() : base(0, 1, (ulong)Math.E) { }
            public override ulong add(ulong a, ulong b) => a + b;
            public override ulong pow(ulong a, ulong b) => a.pow(b);
            public override int Compare(ulong x, ulong y) => x.CompareTo(y);
            public override ulong Factorial(int x) => x.factorial();
            public override ulong fromInt(int x)
            {
                if (x < 0)
                    throw new Exception("cannot get unsigned from negative int");
                return (ulong)x;
            }
            public override ulong fromInt(ulong x) => x;
            public override ulong abs(ulong x) => x;
            public override ulong Conjugate(ulong a) => a;
            public override ulong divide(ulong a, ulong b) => a / b;
            public override ulong mod(ulong a, ulong b) => a % b;
            public override ulong fromFraction(int numerator, int denumerator) => (ulong)(numerator / denumerator);
            public override ulong Invert(ulong x) => 1 / x;
            public override bool isNegative(ulong x) => false;
            public override ulong log(ulong a) => (ulong)Math.Log(a, naturalbase);
            public override ulong multiply(ulong a, ulong b) => a * b;
            public override ulong Negate(ulong x)
            {
                if (x == 0)
                    return 0;
                throw new Exception("cannot negate unsigned");
            }
            public override ulong subtract(ulong a, ulong b) => a - b;
            public override double? toDouble(ulong a) => a;
            public override bool Invertible => false;
            public override bool Negatable => false;
            public override bool Parsable => true;
            public override ulong Parse(string s)
            {
                return ulong.Parse(s);
            }
        }
        private class ByteField : Field<byte>
        {
            public ByteField() : base(0, 1, (byte)Math.E) { }
            public override byte add(byte a, byte b) => (byte)(a + b);
            public override byte pow(byte a, byte b) => a.pow(b);
            public override int Compare(byte x, byte y) => x.CompareTo(y);
            public override byte Factorial(int x) => (byte)x.factorial();
            public override byte fromInt(int x)
            {
                if (x < 0)
                    throw new Exception("cannot get unsigned from negative int");
                return (byte)x;
            }
            public override byte fromInt(ulong x) => (byte)x;
            public override byte abs(byte x) => x;
            public override byte Conjugate(byte a) => a;
            public override byte divide(byte a, byte b) => (byte)(a / b);
            public override byte mod(byte a, byte b) => (byte)(a % b);
            public override byte fromFraction(int numerator, int denumerator) => (byte)(numerator / denumerator);
            public override byte Invert(byte x) => (byte)(1 / x);
            public override bool isNegative(byte x) => false;
            public override byte log(byte a) => (byte)Math.Log(a, naturalbase);
            public override byte multiply(byte a, byte b) => (byte)(a * b);
            public override byte Negate(byte x)
            {
                if (x == 0)
                    return 0;
                throw new Exception("cannot negate unsigned");
            }
            public override byte subtract(byte a, byte b) => (byte)(a - b);
            public override double? toDouble(byte a) => a;
            public override bool Invertible => false;
            public override bool Negatable => false;
            public override bool Parsable => true;
            public override byte Parse(string s)
            {
                return byte.Parse(s);
            }
        }
        private class SbyteField : Field<sbyte>
        {
            public SbyteField() : base(0, 1, (sbyte)Math.E) { }
            public override sbyte add(sbyte a, sbyte b) => (sbyte)(a + b);
            public override sbyte pow(sbyte a, sbyte b) => a.pow(b);
            public override int Compare(sbyte x, sbyte y) => x.CompareTo(y);
            public override sbyte Factorial(int x) => (sbyte)x.factorial();
            public override sbyte fromInt(int x)
            {
                return (sbyte)x;
            }
            public override sbyte fromInt(ulong x) => (sbyte)x;
            public override sbyte abs(sbyte x) => Math.Abs(x);
            public override sbyte Conjugate(sbyte a) => a;
            public override sbyte divide(sbyte a, sbyte b) => (sbyte)(a / b);
            public override sbyte mod(sbyte a, sbyte b) => (sbyte)(a % b);
            public override sbyte fromFraction(int numerator, int denumerator) => (sbyte)(numerator / denumerator);
            public override sbyte Invert(sbyte x) => (sbyte)(1 / x);
            public override bool isNegative(sbyte x) => x < 0;
            public override sbyte log(sbyte a) => (sbyte)Math.Log(a, naturalbase);
            public override sbyte multiply(sbyte a, sbyte b) => (sbyte)(a * b);
            public override sbyte Negate(sbyte x)
            {
                return (sbyte)-x;
            }
            public override sbyte subtract(sbyte a, sbyte b) => (sbyte)(a - b);
            public override double? toDouble(sbyte a) => a;
            public override bool Invertible => false;
            public override bool Negatable => true;
            public override bool Parsable => true;
            public override sbyte Parse(string s)
            {
                return sbyte.Parse(s);
            }
        }
        private class ShortField : Field<short>
        {
            public ShortField() : base(0, 1, (short)Math.E) { }
            public override short add(short a, short b) => (short)(a + b);
            public override short pow(short a, short b) => a.pow(b);
            public override int Compare(short x, short y) => x.CompareTo(y);
            public override short Factorial(int x) => (short)x.factorial();
            public override short fromInt(int x)
            {
                return (short)x;
            }
            public override short fromInt(ulong x) => (short)x;
            public override short abs(short x) => x;
            public override short Conjugate(short a) => a;
            public override short divide(short a, short b) => (short)(a / b);
            public override short mod(short a, short b) => (short)(a % b);
            public override short fromFraction(int numerator, int denumerator) => (short)(numerator / denumerator);
            public override short Invert(short x) => (short)(1 / x);
            public override bool isNegative(short x) => x < 0;
            public override short log(short a) => (short)Math.Log(a, naturalbase);
            public override short multiply(short a, short b) => (short)(a * b);
            public override short Negate(short x)
            {
                return (short)-x;
            }
            public override short subtract(short a, short b) => (short)(a - b);
            public override double? toDouble(short a) => a;
            public override bool Invertible => false;
            public override bool Negatable => true;
            public override bool Parsable => true;
            public override short Parse(string s)
            {
                return short.Parse(s);
            }
        }
        private class UshortField : Field<ushort>
        {
            public UshortField() : base(0, 1, (ushort)Math.E) { }
            public override ushort add(ushort a, ushort b) => (ushort)(a + b);
            public override ushort pow(ushort a, ushort b) => a.pow(b);
            public override int Compare(ushort x, ushort y) => x.CompareTo(y);
            public override ushort Factorial(int x) => (ushort)x.factorial();
            public override ushort fromInt(int x)
            {
                if (x < 0)
                    throw new Exception("cannot get unsigned from negative int");
                return (ushort)x;
            }
            public override ushort fromInt(ulong x) => (ushort)x;
            public override ushort abs(ushort x) => x;
            public override ushort Conjugate(ushort a) => a;
            public override ushort divide(ushort a, ushort b) => (ushort)(a / b);
            public override ushort mod(ushort a, ushort b) => (ushort)(a % b);
            public override ushort fromFraction(int numerator, int denumerator) => (ushort)(numerator / denumerator);
            public override ushort Invert(ushort x) => (ushort)(1 / x);
            public override bool isNegative(ushort x) => false;
            public override ushort log(ushort a) => (ushort)Math.Log(a, naturalbase);
            public override ushort multiply(ushort a, ushort b) => (ushort)(a * b);
            public override ushort Negate(ushort x)
            {
                if (x == 0)
                    return 0;
                throw new Exception("cannot negate unsigned");
            }
            public override ushort subtract(ushort a, ushort b) => (ushort)(a - b);
            public override double? toDouble(ushort a) => a;
            public override bool Invertible => false;
            public override bool Negatable => false;
            public override bool Parsable => true;
            public override ushort Parse(string s)
            {
                return ushort.Parse(s);
            }
        }
        private class StringField : Field<string>
        {
            public StringField() : base("", "", "") { }
            public override string add(string a, string b) => a + b;
            public override string pow(string a, string b)
            {
                throw new NotSupportedException();
            }
            public override int Compare(string x, string y) => x.CompareTo(y);
            public override string Factorial(int x)
            {
                throw new NotSupportedException();
            }
            public override string fromInt(int x)
            {
                return new string(' ',x);
            }
            public override string fromInt(ulong x)
            {
                return new string(' ', (int)x);
            }
            public override string abs(string x) => x;
            public override string Conjugate(string a) => a;
            public override string divide(string a, string b)
            {
                throw new NotSupportedException();
            }
            public override string fromFraction(int numerator, int denumerator)
            {
                throw new NotSupportedException();
            }
            public override string Invert(string x)
            {
                throw new NotSupportedException();
            }
            public override bool isNegative(string x) => false;
            public override string log(string a)
            {
                throw new NotSupportedException();
            }
            public override string multiply(string a, string b)
            {
                throw new NotSupportedException();
            }
            public override string mod(string a, string b)
            {
                throw new NotSupportedException();
            }
            public override string Negate(string x)
            {
                throw new NotSupportedException();
            }
            public override string subtract(string a, string b)
            {
                throw new NotSupportedException();
            }
            public override double? toDouble(string a)
            {
                double o;
                bool suc = double.TryParse(a,out o);
                return suc ? (double?)o : null;
            }
            public override bool Invertible => false;
            public override bool Negatable => false;
            public override bool ModduloAble => false;
            public override bool Parsable => true;
            public override OrderType Order => OrderType.TotalOrder;
            public override string Parse(string s)
            {
                return s;
            }
        }
        private class CharField : Field<char>
        {
            public CharField() : base('\u0000', '\u0001', '\u0002') { }
            public override char add(char a, char b) => (char)(a + b);
            public override char pow(char a, char b)
            {
                throw new NotSupportedException();
            }
            public override int Compare(char x, char y) => x.CompareTo(y);
            public override char Factorial(int x)
            {
                throw new NotSupportedException();
            }
            public override char fromInt(int x)
            {
                return (char)x;
            }
            public override char fromInt(ulong x)
            {
                return (char)x;
            }
            public override char abs(char x) => x;
            public override char Conjugate(char a) => a;
            public override char divide(char a, char b) => (char)(a / b);
            public override char fromFraction(int numerator, int denumerator)
            {
                throw new NotSupportedException();
            }
            public override char Invert(char x)
            {
                throw new NotSupportedException();
            }
            public override bool isNegative(char x) => false;
            public override char log(char a)
            {
                throw new NotSupportedException();
            }
            public override char multiply(char a, char b) => (char)(a * b);
            public override char mod(char a, char b) => (char)(a % b);
            public override char Negate(char x) => (char)(char.MaxValue - x);
            public override char subtract(char a, char b) => (char)(a - b);
            public override double? toDouble(char a)
            {
                return a;
            }
            public override bool Invertible => false;
            public override bool Negatable => true;
            public override bool ModduloAble => true;
            public override bool Parsable => true;
            public override char Parse(string s)
            {
                return s[0];
            }
            public override OrderType Order => OrderType.TotalOrder;
        }
        private class BigRationalField : Field<BigRational>
        {
            public BigRationalField() : base(0, 1, Math.E) { }
            public override BigRational add(BigRational a, BigRational b) => a + b;
            public override BigRational pow(BigRational a, BigRational b) => a.pow(b,new BigRational(1,a.Denominator));
            public override int Compare(BigRational x, BigRational y) => x.CompareTo(y);
            public override BigRational Factorial(int x) => x.factorial();
            public override BigRational fromInt(int x) => x;
            public override BigRational fromInt(ulong x) => x;
            public override BigRational abs(BigRational x) =>  x.abs();
            public override BigRational Conjugate(BigRational a) => a;
            public override BigRational divide(BigRational a, BigRational b) => a / b;
            public override BigRational mod(BigRational a, BigRational b) => a % b;
            public override BigRational fromFraction(int numerator, int denumerator) => numerator / (BigRational)denumerator;
            public override BigRational Invert(BigRational x) => 1 / x;
            public override bool isNegative(BigRational x) => x.Sign < 0;
            public override BigRational log(BigRational a) => (Math.Log((double)a));
            public override BigRational multiply(BigRational a, BigRational b) => a * b;
            public override BigRational Negate(BigRational x) => -x;
            public override BigRational subtract(BigRational a, BigRational b) => a - b;
            public override double? toDouble(BigRational a) => (double)a;
            public override bool Parsable => true;
            public override BigRational Parse(string s)
            {
                return new BigRational(double.Parse(s));
            }
        }
        private class BigIntegerField : Field<BigInteger>
        {
            public BigIntegerField() : base(0, 1, (BigInteger)Math.E) { }
            public override BigInteger add(BigInteger a, BigInteger b) => a + b;
            public override BigInteger pow(BigInteger a, BigInteger b) => a.pow(b);
            public override int Compare(BigInteger x, BigInteger y) => x.CompareTo(y);
            public override BigInteger Factorial(int x) => x.factorial();
            public override BigInteger fromInt(int x) => x;
            public override BigInteger fromInt(ulong x) => x;
            public override BigInteger abs(BigInteger x) => BigInteger.Abs(x);
            public override BigInteger Conjugate(BigInteger a) => a;
            public override BigInteger divide(BigInteger a, BigInteger b) => a / b;
            public override BigInteger mod(BigInteger a, BigInteger b) => a % b;
            public override BigInteger fromFraction(int numerator, int denumerator) => numerator / (BigInteger)denumerator;
            public override BigInteger Invert(BigInteger x) => 1 / x;
            public override bool isNegative(BigInteger x) => x < 0;
            public override BigInteger log(BigInteger a) => (BigInteger) BigInteger.Log(a,(double) this.naturalbase);
            public override BigInteger multiply(BigInteger a, BigInteger b) => a * b;
            public override BigInteger Negate(BigInteger x) => -x;
            public override BigInteger subtract(BigInteger a, BigInteger b) => a - b;
            public override double? toDouble(BigInteger a) => (double) a;
            public override bool Invertible => false;
            public override bool Parsable => true;
            public override BigInteger Parse(string s)
            {
                return BigInteger.Parse(s);
            }
        }
        private class BoolField : Field<bool>
        {
            public BoolField() : base(false, true, true) { }
            public override bool pow(bool a, bool b)
            {
                if (!a && !b)
                    throw new ArithmeticException("can't raise 0 by 0");
                return !b || a;
            }
            public override bool Factorial(int x) => x <= 1;
            public override bool add(bool a, bool b) => a ^ b;
            public override int Compare(bool x, bool y)
            {
                return x == y ? 0 : (x ? 1 : -1);
            }
            public override bool Pow(bool @base, int x) => @base && x%2 == 1;
            public override bool abs(bool x) => x;
            public override bool Conjugate(bool a) => a;
            public override bool divide(bool a, bool b)
            {
                if (!b)
                    throw new ArithmeticException("division by 0");
                return a;
            }
            public override bool mod(bool a, bool b)
            {
                if (!b)
                    throw new ArithmeticException("division by 0");
                return false;
            }
            public override bool fromInt(int x) => x%2==1;
            public override bool Invert(bool x)
            {
                if (!x)
                    throw new ArithmeticException("division by 0");
                return true;
            }
            public override bool fromInt(ulong x) => x % 2 == 1;
            public override bool isNegative(bool x) => false;
            public override bool log(bool a)
            {
                if (!a)
                    throw new ArithmeticException("log of 0");
                return false;
            }
            public override bool multiply(bool a, bool b) => a & b;
            public override bool Negate(bool x) => x;
            public override bool subtract(bool a, bool b) => a ^ !b;
            public override double? toDouble(bool a)
            {
                return a ? 1 : 0;
            }
            public override bool Parsable => true;
            public override OrderType Order => OrderType.ReflexiveZero;
            public override bool Parse(string s)
            {
                return bool.Parse(s);
            }
        }
        
        
        
    #endregion
        public const int DEFAULT_SURROUNDING_FIELD_DEPTH = 2;
        private static readonly IDictionary<Type,Field> _quickFieldDictionary = new Dictionary<Type, Field>();
        static Fields()
        {
            setField(new DoubleField());
            setField(new FloatField());
            setField(new DecimalField());
            setField(new IntField());
            setField(new LongField());
            setField(new UIntField());
            setField(new ULongField());
            setField(new ByteField());
            setField(new SbyteField());
            setField(new ShortField());
            setField(new UshortField());
            setField(new StringField());
            setField(new BigRationalField());
            setField(new BigIntegerField());
            setField(new BoolField());
            setField(new CharField());
        }
        [NotNull] public static Field<T> getField<T>()
        {
            Type t = typeof(T);
            bool constructorcalled = false;
            while (true)
            {
                if (_quickFieldDictionary.ContainsKey(t))
                {
                    return (Field<T>)_quickFieldDictionary[t];
                }
                if (!constructorcalled)
                {
                    try
                    {
                        constructorcalled = true;
                        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                        continue;
                    }
                    catch { }
                }
                return new Field<T>((T)(dynamic)0, (T)(dynamic)1, (T)(dynamic)2);
            }
        }
        public static bool setField<T>(Field<T> f, int surroundingFieldDepth = DEFAULT_SURROUNDING_FIELD_DEPTH)
        {
            var ret = _quickFieldDictionary.ContainsKey(typeof(T));
            _quickFieldDictionary.Add(typeof(T), f);
            if (surroundingFieldDepth > 0)
            {
                setField(new Matrix.MatrixField<T>(f), surroundingFieldDepth - 1);
                setField(new Formulas.FormulaField<T>(f), surroundingFieldDepth - 1);
            }
            return ret;
        }
        public static FieldWrapper<T> ToFieldWrapper<T>(this T @this) => @this;
    }
}
