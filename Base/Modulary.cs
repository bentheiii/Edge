using System;
using System.Collections.Generic;
using Edge.Fielding;
using Edge.NumbersMagic;
using Edge.SystemExtensions;

namespace Edge.Modular
{
    public class Modulary
    {
        private class ModField : Field<Modulary>
        {
            public ModField() : base(new Zero(), new Unit(), new NegUnit()) { }
            public override Modulary add(Modulary a, Modulary b) => a + b;
            public override Modulary pow(Modulary a, Modulary b) => a.Pow(b);
            public override int Compare(Modulary x, Modulary y) => x.val.CompareTo(y.val);
            public override Modulary fromInt(int x)
            {
                switch (x)
                {
                    case 0:
                        return this.zero;
                    case 1:
                        return this.one;
                }
                throw new Exception("cannot get modulary from int");
            }
            public override Modulary fromInt(ulong x) => fromInt((int)x);
            public override Modulary abs(Modulary x) => x;
            public override Modulary Conjugate(Modulary a) => a;
            public override Modulary divide(Modulary a, Modulary b) => a / b;
            public override Modulary mod(Modulary a, Modulary b)
            {
                throw new NotSupportedException();
            }
            public override Modulary Invert(Modulary x) => x.Invert();
            public override bool isNegative(Modulary x) => false;
            public override Modulary log(Modulary a) => a.Log(naturalbase);
            public override Modulary multiply(Modulary a, Modulary b) => a * b;
            public override Modulary Negate(Modulary x) => -x;
            public override Modulary subtract(Modulary a, Modulary b) => a - b;
            public override double? toDouble(Modulary a) => a.val;
            public override bool ModduloAble => false;
            public override OrderType Order => OrderType.ReflexiveZero;
            public override RandomGenType RandGen => RandomGenType.Special;
            public override FieldShape shape => FieldShape.Finite;
            public override Modulary Random(IEnumerable<byte> bytes, Tuple<Modulary, Modulary> bounds = null, object special = null)
            {
                int max = special as int? ?? 0;
                return max == 0 ? new Zero() : new Modulary(Fields.getField<int>().Random(bytes,Tuple.Create(0,max)),max);
            }
        }
        static Modulary()
        {
            Fields.setField(new ModField());
        }
        protected bool Equals(Modulary other)
        {
            return this.val == other.val;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.max * 397) ^ this.val;
            }
        }
        public int max { get; }
        public int val { get; }
        public Modulary(int val, int max)
        {
            this.val = val.TrueMod(max);
            this.max = max;
        }
        public virtual bool isWellDefined
        {
            get
            {
                return true;
            }
        }
        public Modulary resize(int newsize)
        {
            return new Modulary(val, newsize);
        }
        public virtual Modulary Add(Modulary b)
        {
            return new Modulary((this.val + b.val), max);
        }
        public virtual Modulary Multiply(Modulary b)
        {
            return new Modulary((this.val*b.val), max);
        }
        public virtual Modulary Pow(Modulary p)
        {
            return new Modulary(val.pow(p.val),max);
        }
        public Modulary Add(int b)
        {
            return new Modulary(val+b,max);
        }
        public Modulary Multiply(int b)
        {
            return new Modulary(val*b, max);
        }
        public Modulary Pow(int b)
        {
            return b < 0 ? this.Pow(-b).Invert() : new Modulary(val.pow(b), max);
        }
        public virtual Modulary Invert()
        {
            if (val == 0)
                throw new Exception("cannot invert zero");
            return this.Pow(max - 2);
        }
        public virtual Modulary Negate()
        {
            return new Modulary(-val, max);
        }
        public virtual Modulary Log(Modulary b)
        {
            int ret = 0;
            Modulary x = new Unit();
            while (!x.Equals(this))
            {
                ret++;
                x *= b;
            }
            return new Modulary(ret,max);
        }
        public static implicit operator int(Modulary w)
        {
            return w.val;
        }
        public static Modulary operator -(Modulary a)
        {
            return a.Negate();
        }
        public static Modulary operator +(Modulary a, Modulary b)
        {
            return a.Add(b);
        }
        public static Modulary operator +(Modulary a, int b)
        {
            return a.Add(b);
        }
        public static Modulary operator -(Modulary a, Modulary b)
        {
            return a.Add(-b);
        }
        public static Modulary operator -(Modulary a, int b)
        {
            return a.Add(-b);
        }
        public static Modulary operator *(Modulary a, Modulary b)
        {
            return a.Multiply(b);
        }
        public static Modulary operator *(Modulary a, int b)
        {
            return a.Multiply(b);
        }
        public static Modulary operator /(Modulary a, Modulary b)
        {
            return a.Multiply(b.Invert());
        }
        public override bool Equals(object obj)
        {
            Modulary modulary = obj as Modulary;
            if (modulary != null)
                return this.Equals(modulary);
            if (obj is int)
                return val.Equals(obj);
            return false;
        }
    }
    public class Unit : Modulary
    {
        public Unit() : base(1, int.MaxValue) { }
        private const string NOT_SUPPORTED_STRING = "function not supported for unbound modulary, try resizing it first";
        public override Modulary Add(Modulary b)
        {
            if (b.isWellDefined || b is Zero)
                return b.Add(1);
            if (b is NegUnit)
                return new Zero();
            throw new NotSupportedException(NOT_SUPPORTED_STRING);
        }
        public override Modulary Invert()
        {
            return this;
        }
        public override Modulary Log(Modulary b)
        {
            return new Zero();
        }
        public override Modulary Multiply(Modulary b)
        {
            return b;
        }
        public override Modulary Negate()
        {
            return new NegUnit();
        }
        public override Modulary Pow(Modulary p)
        {
            return this;
        }
        public override bool isWellDefined => false;
    }
    public class NegUnit : Modulary {
        public NegUnit() : base(-1, int.MaxValue) { }
        public override Modulary Multiply(Modulary b)
        {
            return b.Negate();
        }
        public override Modulary Pow(Modulary p)
        {
            if (p.val%2 == 0)
                return new Unit();
            return this;
        }
        private const string NOT_SUPPORTED_STRING = "function not supported for unbound modulary, try resizing it first";
        public override Modulary Add(Modulary b)
        {
            if (b.isWellDefined || b is Zero)
                return b.Add(-1);
            if (b is Unit)
                return new Zero();
            throw new NotSupportedException(NOT_SUPPORTED_STRING);
        }
        public override Modulary Invert()
        {
            return this;
        }
        public override Modulary Log(Modulary b)
        {
            return (new Modulary(-1,b.max)).Log(b);
        }
        public override Modulary Negate()
        {
            return new Unit();
        }
        public override bool isWellDefined => false;
    }
    public class Zero : Modulary {
        public Zero() : base(0, int.MaxValue) {}
        public override Modulary Negate()
        {
            return this;
        }
        public override bool isWellDefined => false;
        public override Modulary Add(Modulary b)
        {
            return b;
        }
        public override Modulary Invert()
        {
            throw new DivideByZeroException("cannot invert zero");
        }
        public override Modulary Log(Modulary b)
        {
            throw new ArithmeticException("cannot log zero");
        }
        public override Modulary Multiply(Modulary b)
        {
            return this;
        }
        public override Modulary Pow(Modulary p)
        {
            return this;
        }
    }
    
}
