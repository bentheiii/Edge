using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;

namespace Edge.Series
{
    public abstract class Series<T> : IEnumerable<T>
    {
        /// <summary>
        /// index 0
        /// </summary>
        protected readonly T Initialvalue;
        public abstract T this[int index] { get; }
        public virtual T sum(int count, int startindex = 0)
        {
            return getmemberarray(count, startindex).getSum();
        }
        public virtual IEnumerable<T> getmemberarray(int count, int startindex = 0)
        {
            if (count <= 0)
                throw new Exception("array count must be higher than 0");
            return ArrayExtensions.Fill(count, i => this[i + startindex]);
        }
        internal Series(T a0)
        {
            this.Initialvalue = a0;
        }
        internal Series()
        {
            this.Initialvalue = default(T);
        }
        public virtual IEnumerator<T> GetEnumerator()
        {
            var gaps = Looping.Loops.YieldAggregate(s => s * 2, 1);
            var gapsandlengths = gaps.Zip(0.Enumerate().Concat(gaps.YieldAggregate((val, sum) => val + sum, 0)));
            return gapsandlengths.Select(a => getmemberarray(a.Item1, a.Item2)).Concat().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    public class GeometricSeries<T> : Series<T>
    {
        public T ratio { get; }
        public override T sum(int count, int startindex = 0)
        {
            var r = this.ratio.ToFieldWrapper();
            return (this.Initialvalue / (r - 1)) * (r.pow(count) - r.pow(startindex));
        }
        public override T this[int index]
        {
            get
            {
                return this.Initialvalue * (ratio.ToFieldWrapper()).pow(index);
            }
        }
        public override bool Equals(object obj)
        {
            GeometricSeries<T> series = obj as GeometricSeries<T>;
            if (series != null)
                return (this.ratio.Equals(series.ratio)) && (this.Initialvalue.Equals(series.Initialvalue));
            return false;
        }
        public override int GetHashCode()
        {
            return this.Initialvalue.GetHashCode() ^ this.ratio.GetHashCode();
        }
        public override string ToString()
        {
            return "a(n) = " + this.Initialvalue + " * " + this.ratio + "^n";
        }
        public GeometricSeries(T a0, T r) : base(a0)
        {
            this.ratio = r;
        }
        public static T quicksum(T a0, T rat, int count)
        {
            dynamic r = rat;
            return (a0 / (r - 1)) * (r.pow(count) - 1);
        }
        public static T quickmember(T a0, T rat, int index)
        {
            dynamic r = rat;
            return a0 * r.pow(index);
        }
        public override IEnumerable<T> getmemberarray(int count, int startindex = 0)
        {
            var r = ratio.ToFieldWrapper();
            T[] ret = new T[count];
            ret[0] = this[startindex];
            for (int i = 1; i < count; i++)
            {
                ret[i] = ret[i - 1] * r;
            }
            return ret;
        }
        public bool converges
        {
            get
            {
                return this.ratio.ToFieldWrapper() <= Fields.getField<T>().one;
            }
        }
        public T convergance()
        {
            if (!this.converges)
                throw new Exception("sequence does not converge");
            var f = Fields.getField<T>();
            return this.ratio.ToFieldWrapper().Equals(Fields.getField<T>().one) ? Initialvalue : f.zero;
        }
        public T SumConvergance()
        {
            var f = Fields.getField<T>();
            if (!this.converges || this.ratio.ToFieldWrapper().Equals(Fields.getField<T>().one))
                throw new Exception("series does not converge");
            return this.Initialvalue / (f.one - this.ratio.ToFieldWrapper());
        }
        public override IEnumerator<T> GetEnumerator()
        {
            var i = this[0].ToFieldWrapper();
            while (true)
            {
                yield return i.val;
                i *= ratio;
            }
        }
    }
    public class ArithmeticSeries<T> : Series<T>
    {
        public T difference { get; }
        public override T sum(int count, int startindex = 0)
        {
            var ret = (this.Initialvalue + (count * this.difference.ToFieldWrapper()) / 2).val.ToFieldWrapper();
            if (startindex > 0)
                ret -= this.sum(startindex);
            return ret.val;
        }
        public override T this[int index]
        {
            get
            {
                return this.Initialvalue + this.difference.ToFieldWrapper() * index;
            }
        }
        public override bool Equals(object obj)
        {
            ArithmeticSeries<T> series = obj as ArithmeticSeries<T>;
            if (series != null)
                return (this.difference.Equals(series.difference)) && (this.Initialvalue.Equals(series.Initialvalue));
            return false;
        }
        public override int GetHashCode()
        {
            return this.Initialvalue.GetHashCode() ^ this.difference.GetHashCode();
        }
        public override string ToString()
        {
            return "a(n) = " + this.Initialvalue + " + " + this.difference + "*n";
        }
        public ArithmeticSeries(T a0, T d) : base(a0)
        {
            this.difference = d;
        }
        public static T quicksum(T a0, T d, int count)
        {
            return a0 + (count * d.ToFieldWrapper()) / 2;
        }
        public static T quickmember(T a0, T d, int index)
        {
            return a0 + d.ToFieldWrapper() * index;
        }
        public override IEnumerable<T> getmemberarray(int count, int startindex = 0)
        {
            T[] ret = new T[count];
            ret[0] = this[startindex];
            for (int i = 1; i < count; i++)
            {
                ret[i] = ret[i - 1] + this.difference.ToFieldWrapper();
            }
            return ret;
        }
        public override IEnumerator<T> GetEnumerator()
        {
            var i = this[0].ToFieldWrapper();
            while (true)
            {
                yield return i;
                i += difference;
            }
        }
    }
    public class HarmonicSeries<T> : Series<T>
    {
        private readonly int _pow;
        public HarmonicSeries(int pow)
        {
            _pow = pow;
        }
        public override T this[int index]
        {
            get
            {
                return Fields.getField<T>().fromInt(index + 1).ToFieldWrapper().pow(_pow).Invert();
            }
        }
        public bool converges
        {
            get
            {
                return _pow > 1;
            }
        }
        public T convergance()
        {
            if (!this.converges)
                throw new Exception("sequence does not converge");
            return Fields.getField<T>().zero;
        }
    }
    public class RecursiveLinearSeries<T> : Series<T>
    {
        private readonly T[] _starters;
        private readonly T[] _factors;
        public RecursiveLinearSeries(T[] factors, T[] starters)
        {
            if (factors.Length > starters.Length)
                throw new ArgumentException("can't have more factors than starters");
            this._factors = factors;
            this._starters = starters;
        }
        public override T this[int index]
        {
            get
            {
                return getmemberarray(index).Last();
            }
        }
        public override IEnumerable<T> getmemberarray(int count, int startindex = 0)
        {
            T[] arr = new T[count + startindex];
            var f = Fields.getField<T>();
            for (int i = 0; i < _starters.Length; i++)
            {
                arr[i] = _starters[i];
            }
            for (int i = _starters.Length; i < arr.Length; i++)
            {
                var sum = f.zero.ToFieldWrapper();
                for (int j = 0; j < _factors.Length; j++)
                {
                    sum += arr[i - j].ToFieldWrapper() * _factors[j];
                }
                arr[i] = sum.val;
            }
            T[] ret = new T[count];
            arr.CopyTo(ret, startindex);
            return ret;
        }
    }
    public static class ClassicSeries
    {
        public static readonly Series<int> Fibonachi;
        static ClassicSeries()
        {
            Fibonachi = new RecursiveLinearSeries<int>(new[] {1, 1}, new[] {0, 1});
        }
    }
}