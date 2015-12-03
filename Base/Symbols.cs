using System;
using System.Linq;
using System.Numerics;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Numerics;

namespace Edge.Symbols
{
    public abstract class Symbol
    {
        private class SymbolicField : Field<Symbol>
        {
            public SymbolicField() : base(0, 1, Symbol.e) { }
            public override Symbol add(Symbol a, Symbol b) => a + b;
            public override Symbol pow(Symbol a, Symbol b) => a ^ b;
            public override int Compare(Symbol x, Symbol y) => x.Approximate(0.000001).CompareTo(y.Approximate(0.000001));
            public override Symbol Factorial(int x) => (x.factorial());
            public override Symbol fromInt(int x) => x;
            public override Symbol fromInt(ulong x) => x;
            public override Symbol abs(Symbol x) => new Abs(x);
            public override Symbol Conjugate(Symbol a) => a;
            public override Symbol divide(Symbol a, Symbol b) => a / b;
            public override Symbol mod(Symbol a, Symbol b)
            {
                throw new NotSupportedException();
            }
            public override Symbol fromFraction(int numerator, int denumerator) => numerator / (Symbol)denumerator;
            public override Symbol Invert(Symbol x) => 1 / x;
            public override bool isNegative(Symbol x) => x.Approximate(0.000001) < 0;
            public override Symbol log(Symbol a) => a.log();
            public override Symbol multiply(Symbol a, Symbol b) => a * b;
            public override Symbol Negate(Symbol x) => -x;
            public override Symbol subtract(Symbol a, Symbol b) => a - b;
            public override bool ModduloAble => false;
            public override double? toDouble(Symbol a) => (double)a.Approximate(0.000001);
        }

        // ReSharper disable InconsistentNaming
        public static readonly Symbol e = new InfSum(i => new BigRational(1, i.BigFactorial()),1);
        public static readonly Symbol pi = new InfSum(i => new BigRational((i % 2 == 0 ? 1.0 : -1.0) * 4.0 / (2.0 * i + 1)));
        public static readonly Symbol GoldenRatio = (1 + ((Symbol)5).Root()) / 2;
        public static readonly Symbol InverseGoldenRatio = (-1 + ((Symbol)5).Root()) / 2;
        public static readonly Symbol Ln2 = ((Symbol)2).log();
        // ReSharper restore InconsistentNaming
        public virtual Symbol Multiply(Symbol s2)
        {
            return new Product(this,s2);
        }
        public virtual Symbol Invert()
        {
            return new Whole(1).Divide(this);
        }
        public virtual Symbol Divide(Symbol s2)
        {
            return new Division(this,s2);
        }
        public virtual Symbol Add(Symbol s2)
        {
            return new Sum(this,s2);
        }
        public virtual Symbol Pow(Symbol s2)
        {
            return new Pow(this,s2);
        }
        public abstract BigRational Approximate(int iterations);
        public abstract BigRational Approximate(BigRational maxdiff, out int iterations);
        public virtual BigRational NearApproximate(BigRational maxdiff)
        {
            return Approximate(maxdiff);
        }
        public virtual BigRational NearApproximate(int iterations)
        {
            return Approximate(iterations);
        }
        public BigRational Approximate(BigRational maxdiff)
        {
            int proxy;
            return Approximate(maxdiff, out proxy);
        }
        public BigRational ApproximationDelta(int iteration)
        {
            return (Approximate(iteration) - Approximate(iteration + 1)).abs();
        }
        public Symbol log()
        {
            return new Log(this);
        }
        public Symbol log(Symbol @base)
        {
            return this.log() / @base.log();
        }
        public Symbol Sin()
        {
            return new Sin(this);
        }
        public Symbol Cosin()
        {
            return new Cosin(this);
        }
        public Symbol Root(int pow = 2)
        {
            return this ^ (1.0 / pow);
        }
        public static Symbol operator *(Symbol a, Symbol b)
        {
            return a.Multiply(b);
        }
        public static Symbol operator +(Symbol a, Symbol b)
        {
            return a.Add(b);
        }
        public static Symbol operator /(Symbol a, Symbol b)
        {
            return a.Multiply(b.Invert());
        }
        public static Symbol operator -(Symbol a, Symbol b)
        {
            return a.Add(-b);
        }
        public static Symbol operator ^(Symbol a, Symbol b)
        {
            return a.Pow(b);
        }
        public static Symbol operator ^(Symbol a, int b)
        {
            return a.Pow(b);
        }
        public static Symbol operator ^(Symbol a, double b)
        {
            return a.Pow(b);
        }
        public static Symbol operator -(Symbol a)
        {
            return a.Multiply(new Whole(-1));
        }
        public static implicit operator Symbol(int val)
        {
            return new Whole(val);
        }
        public static implicit operator Symbol(BigInteger val)
        {
            return new Whole(val);
        }
        public static implicit operator Symbol(BigRational val)
        {
            return new Whole(val.Numerator) / new Whole(val.Denominator);
        }
        public static implicit operator Symbol(double val)
        {
            return new BigRational(val);
        }
    }
    public class Whole : Symbol
    {
        internal readonly BigInteger _val;
        public Whole(int val) : this(new BigInteger(val)) { }
        public Whole(BigInteger val)
        {
            this._val = val;
        }
        public override Symbol Add(Symbol s2)
        {
            Whole whole = s2 as Whole;
            return whole != null ? new Whole(this._val + whole._val) : base.Add(s2);
        }
        public override Symbol Pow(Symbol pow)
        {
            Whole whole = pow as Whole;
            return whole != null ? new Whole(this._val.pow(whole._val)) : base.Pow(pow);
        }
        public override BigRational Approximate(int iterations)
        {
            return new BigRational(_val);
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            iterations = 0;
            return Approximate(0);
        }
    }
    public class Sum : Symbol
    {
        internal readonly Symbol[] _summands;
        public Sum(params Symbol[] summands)
        {
            this._summands = summands;
        }
        public override Symbol Add(Symbol s2)
        {
            return new Sum(_summands.Concat(s2.Enumerate()).ToArray());
        }
        public override Symbol Divide(Symbol s2)
        {
            return new Sum(_summands.SelectToArray(a => a.Divide(s2)));
        }
        public override BigRational Approximate(int iterations)
        {
            BigRational sum = new BigRational(0M);
            foreach (Symbol summand in _summands)
            {
                sum += summand.Approximate(iterations);
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            maxdiff /= (_summands.Length+1);
            BigRational sum = new BigRational(0M);
            int max = -1;
            foreach (Symbol summand in _summands)
            {
                sum += summand.Approximate(maxdiff, out iterations);
                if (max < iterations)
                    max = iterations;
            }
            iterations = max;
            return sum;
        }
    }
    public class Product : Symbol
    {
        internal readonly Symbol[] _multiplicands;
        public Product(params Symbol[] multiplicands)
        {
            this._multiplicands = multiplicands;
        }
        public override Symbol Multiply(Symbol s2)
        {
            return new Product(_multiplicands.Concat(s2.Enumerate()).ToArray());
        }
        public override Symbol Pow(Symbol s2)
        {
            return new Product(_multiplicands.SelectToArray(a=>a.Pow(s2)));
        }
        public override BigRational Approximate(int iterations)
        {
            BigRational sum = new BigRational(1M);
            foreach (Symbol summand in _multiplicands)
            {
                sum *= summand.Approximate(iterations);
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            while (true)
            {
                maxdiff /= (_multiplicands.Length+1);
                BigRational sum = new BigRational(1M);
                int max = -1;
                foreach (Symbol summand in _multiplicands)
                {
                    sum *= summand.Approximate(maxdiff, out iterations);
                    if (max < iterations)
                        max = iterations;
                }
                iterations = max;
                var sum2 = NearApproximate(iterations + 1);
                if (BigRational.Abs(sum - sum2) < maxdiff)
                    return sum2;
            }
        }
        public override BigRational NearApproximate(int iterations)
        {
            BigRational sum = new BigRational(1M);
            foreach (Symbol summand in _multiplicands)
            {
                sum *= summand.NearApproximate(iterations);
            }
            return sum;
        }
    }
    public class Division : Symbol
    {
        internal readonly Symbol _num, _den;
        public Division(Symbol num, Symbol den)
        {
            this._num = num;
            this._den = den;
        }
        public override Symbol Multiply(Symbol factor)
        {
            var div = factor as Division;
            return div != null ? new Division(this._num.Multiply(div._num),this._den.Multiply(div._den)) : new Division(this._num.Multiply(factor),this._den);
        }
        public override Symbol Invert()
        {
            return new Division(_den,_num);
        }
        public override Symbol Divide(Symbol s2)
        {
            var div = s2 as Division;
            return div != null ? this.Multiply(s2.Invert()) : base.Divide(s2);
        }
        public override Symbol Add(Symbol s2)
        {
            var div = s2 as Division;
            return div != null ? new Division(this._num.Multiply(div._den).Add(div._num.Multiply(this._den)) , this._den.Multiply(div._den)) : new Division(this._num.Add(s2.Multiply(this._den)),this._den);
        }
        public override Symbol Pow(Symbol pow)
        {
            return new Division(_num.Pow(pow),_den.Pow(pow));
        }
        public override BigRational Approximate(int iterations)
        {
            return _num.Approximate(iterations) / _den.Approximate(iterations);
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            int max;
            var n = _num.Approximate(maxdiff, out max);
            var d = _den.Approximate(maxdiff, out iterations);
            max = Math.Max(max, iterations);
            var sum = n / d;
            var sum2 = Approximate(max);
            while (BigRational.Abs(sum -sum2) > maxdiff)
            {
                max *= 2;
                sum = Approximate(max);
                sum2 = Approximate(max + 1);
            }
            iterations = max + 1;
            return sum2;
        }
        public override BigRational NearApproximate(BigRational maxdiff)
        {
            return _num.Approximate(maxdiff) / _den.Approximate(maxdiff);
        }
    }
    public class Pow : Symbol
    {
        internal readonly Symbol _base, _pow;

        public Pow(Symbol @base, Symbol pow)
        {
            this._base = @base;
            this._pow = pow;
        }
        public override BigRational Approximate(int iterations)
        {
            return _base.Approximate(iterations).pow(_pow.Approximate(iterations),iterations);
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            int max, temp;
            var b = _base.Approximate(maxdiff, out max);
            var p = _pow.Approximate(maxdiff, out temp);
            max = Math.Max(max, temp);
            while (ApproximationDelta(max) > maxdiff)
            {
                max *= 2;
            }
            iterations = max;
            return Approximate(max);
        }
    }
    public class Log : Symbol
    {
        internal readonly Symbol _int;
        public Log(Symbol i)
        {
            this._int = i;
        }
        public override BigRational Approximate(int iterations)
        {
            var sum = BigRational.Zero;
            var @internal = _int.Approximate(iterations);
            var ratio = (@internal - 1) / (@internal + 1);
            var change = ratio * ratio;
            foreach (int i in Loops.Range(iterations))
            {
                var n = new BigInteger(2 * i + 1);
                sum += new BigRational(1,n)*ratio;
                ratio *= change;
            }
            return sum * 2;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            var prev = maxdiff * 10;
            var sum = BigRational.Zero;
            var @internal = _int.Approximate(maxdiff);
            var ratio = (@internal - 1) / (@internal + 1);
            var change = ratio * ratio;
            iterations = 0;
            while((prev-sum).abs() >= maxdiff)
            {
                var n = new BigInteger(2 *  + 1);
                prev = sum;
                sum += new BigRational(1, n) * ratio;
                iterations++;
                ratio *= change;
            }
            return sum * 2;
        }
    }
    public class InfSum : Symbol
    {
        internal readonly Func<int, BigRational> _sequence;
        internal readonly int _start;
        public InfSum(Func<int, BigRational> series, int start = 0)
        {
            this._sequence = series;
            this._start = start;
        }
        public override BigRational Approximate(int iterations)
        {
            BigRational sum = BigRational.Zero;
            foreach (int i in Loops.Range(_start,iterations+_start))
            {
                sum += _sequence(i);
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            iterations = 0;
            BigRational prev = maxdiff*10;
            BigRational sum = BigRational.Zero;
            while ((sum - prev).abs() >= maxdiff)
            {
                prev = sum;
                sum += _sequence(iterations + _start);
                iterations++;
            }
            return sum;
        }
    }
    public class InfProduct : Symbol
    {
        internal readonly Func<int, BigRational> _sequence;
        internal readonly int _start;
        public InfProduct(Func<int, BigRational> series, int start = 0)
        {
            this._sequence = series;
            this._start = start;
        }
        public override BigRational Approximate(int iterations)
        {
            BigRational sum = BigRational.One;
            foreach (int i in Loops.Range(_start, iterations + _start))
            {
                sum += _sequence(i);
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            iterations = 0;
            BigRational prev = maxdiff * 10;
            BigRational sum = BigRational.One;
            while ((sum - prev).abs() >= maxdiff)
            {
                prev = sum;
                sum += _sequence(iterations + _start);
                iterations++;
            }
            return sum;
        }
    }
    public class Abs : Symbol
    {
        internal readonly Symbol _int;
        public Abs(Symbol i)
        {
            this._int = i;
        }
        public override BigRational Approximate(int iterations)
        {
            return _int.Approximate(iterations).abs();
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            return _int.Approximate(maxdiff, out iterations).abs();
        }
    }
    public class Sin : Symbol
    {
        internal readonly Symbol _int;
        public Sin(Symbol i)
        {
            this._int = i;
        }
        public override BigRational Approximate(int iterations)
        {
            var ratio = _int.Approximate(iterations);
            var sum = BigRational.Zero;
            var change = ratio * ratio;
            BigInteger factorial = BigInteger.One;
            foreach (int i in Loops.Range(iterations))
            {
                var n = 2 * i + 1;
                var diff = ratio * (i % 2 == 0 ? 1 : -1) * new BigRational(1, factorial);
                factorial *= (n+1)*(n+2);
                sum += diff;
                ratio *= change;
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            var prev = maxdiff * 10;
            var sum = BigRational.Zero;
            var @internal = _int.Approximate(maxdiff);
            var ratio = _int.Approximate(maxdiff);
            var change = ratio * ratio;
            iterations = 0;
            BigInteger factorial = BigInteger.One;
            while ((prev - sum).abs() >= maxdiff)
            {
                prev = sum;
                var n = 2 * iterations + 1;
                var diff = ratio * (iterations % 2 == 0 ? 1 : -1) * new BigRational(1, factorial);
                factorial *= (n + 1) * (n + 2);
                sum += diff;
                iterations++;
            }
            return sum;
        }
    }
    public class Cosin : Symbol
    {
        internal readonly Symbol _int;
        public Cosin(Symbol i)
        {
            this._int = i;
        }
        public override BigRational Approximate(int iterations)
        {
            var ratio = _int.Approximate(iterations);
            var sum = BigRational.Zero;
            var change = ratio * ratio;
            BigInteger factorial = BigInteger.One;
            foreach (int i in Loops.Range(iterations))
            {
                var n = 2 * i;
                var diff = ratio * (i % 2 == 0 ? 1 : -1) * new BigRational(1, factorial);
                factorial *= (n + 1) * (n + 2);
                sum += diff;
                ratio *= change;
            }
            return sum;
        }
        public override BigRational Approximate(BigRational maxdiff, out int iterations)
        {
            var prev = maxdiff * 10;
            var sum = BigRational.Zero;
            var @internal = _int.Approximate(maxdiff);
            var ratio = _int.Approximate(maxdiff);
            var change = ratio * ratio;
            iterations = 0;
            BigInteger factorial = BigInteger.One;
            while ((prev - sum).abs() >= maxdiff)
            {
                prev = sum;
                var n = 2 * iterations;
                var diff = ratio * (iterations % 2 == 0 ? 1 : -1) * new BigRational(1, factorial);
                factorial *= (n + 1) * (n + 2);
                sum += diff;
                iterations++;
            }
            return sum;
        }
    }
    
}
