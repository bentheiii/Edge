using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Edge.Comparison
{
    public class DynamicComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            dynamic d = x;
            if (d < y)
                return -1;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (d > y)
                return 1;
            return 0;
        }
    }
    public class FunctionComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _c;
        public int Compare(T x, T y)
        {
            return _c(x,y);
        }
        public FunctionComparer(Func<T, T, int> c)
        {
            this._c = c;
        }
        public FunctionComparer(Func<T, IComparable> c)
        {
            this._c = (a, b) => c(a).CompareTo(c(b));
        }
        public FunctionComparer(Func<T, object> f, IComparer c)
        {
            this._c = (a, b) => c.Compare(f(a), f(b));
        } 
    }
    public class EqualityFunctionComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _func;
        private readonly Func<T, int> _hash;
        public EqualityFunctionComparer(Func<T, T, bool> func, Func<T, int> hash)
        {
            this._func = func;
            this._hash = hash;
        }
        public EqualityFunctionComparer(Func<T, object> c)
        {
            this._hash = a=>c(a).GetHashCode();
            this._func = (a,b) => c(a).Equals(c(b));
        }
        public EqualityFunctionComparer(Func<T, object> c, IEqualityComparer e)
        {
            this._hash = a => c(a).GetHashCode();
            this._func = (a, b) => e.Equals(c(a),(c(b)));
        }
        public bool Equals(T x, T y)
        {
            return this._func(x,y);
        }
        public int GetHashCode(T obj)
        {
            return _hash(obj);
        }
    }
    public class PriorityComparer<T> : IComparer<T>
    {
        private readonly IEnumerable<IComparer<T>> _comps;
        private PriorityComparer(IEnumerable<IComparer<T>> c)
        {
            this._comps = c;
        }
        public PriorityComparer(params IComparer<T>[] c)
        {
            this._comps = c.ToArray();
        }
        public PriorityComparer(params Func<T, T, int>[] c) : this(c.Select(a=>((IComparer<T>)new FunctionComparer<T>(a))))
        {
        }
        public PriorityComparer(params Func<T, IComparable>[] c)
            : this(c.Select(a => new FunctionComparer<T>(a)))
        {
        }
        public int Compare(T x, T y)
        {
            foreach (IComparer<T> c in this._comps)
            {
                int ret = c.Compare(x, y);
                if (ret != 0)
                    return ret;
            }
            return 0;
        }
    }
    public static class ReverseComparer
    {
        public static IComparer<T> Reverse<T>(this IComparer<T> comp)
        {
            return new ReverseComparerClass<T>(comp);
        }
        private class ReverseComparerClass<T> : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                return -this._comp.Compare(x, y);
            }
            private readonly IComparer<T> _comp;
            public ReverseComparerClass(IComparer<T> c)
            {
                this._comp = c;
            }
        }
    }
    public static class NonGenericComparer
    {
        public static IComparer ToNonGeneric<T>(this IComparer<T> comp)
        {
            return new NonGenericComparerClass<T>(comp);
        }
        public static IEqualityComparer ToNonGeneric<T>(this IEqualityComparer<T> comp)
        {
            return new NonGenericEqualityComparerClass<T>(comp);
        }
        private class NonGenericComparerClass<T> : IComparer
        {
            public int Compare(object x, object y)
            {
                return this._comp.Compare((T)x, (T)y);
            }
            private readonly IComparer<T> _comp;
            public NonGenericComparerClass(IComparer<T> c)
            {
                this._comp = c;
            }
        }
        private class NonGenericEqualityComparerClass<T> : IEqualityComparer
        {
            private readonly IEqualityComparer<T> _comp;
            public NonGenericEqualityComparerClass(IEqualityComparer<T> c)
            {
                this._comp = c;
            }
            bool IEqualityComparer.Equals(object x, object y)
            {
                return this._comp.Equals((T)x, (T)y);
            }
            public int GetHashCode(object obj)
            {
                return _comp.GetHashCode((T)obj);
            }
        }
    }
    public static class ComparerToEquator
    {
        public static IEqualityComparer<T> ToEqualityComparer<T>(this IComparer<T> comp)
        {
            return ToEqualityComparer<T>(comp, a=>a.GetHashCode());
        }
        public static IEqualityComparer<T> ToEqualityComparer<T>(this IComparer<T> comp, Func<T,int> hash)
        {
            return new WrappedComparerclass<T>(comp, hash);
        }
        private class WrappedComparerclass<T> : IEqualityComparer<T>
        {
            private readonly IComparer<T> _comp;
            private readonly Func<T, int> _hash;
            public WrappedComparerclass(IComparer<T> c, Func<T,int> hash)
            {
                this._comp = c;
                _hash = hash;
            }
            public bool Equals(T x, T y)
            {
                return _comp.Compare(x,y)==0;
            }
            public int GetHashCode(T obj)
            {
                return _hash(obj);
            }
        }
    }
    public class EnumerableCompararer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private readonly IEqualityComparer<T> _int;
        public EnumerableCompararer() : this(EqualityComparer<T>.Default) { }
        public EnumerableCompararer(IEqualityComparer<T> i)
        {
            this._int = i;
        }
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            return x.SequenceEqual(y,_int);
        }
        public int GetHashCode(IEnumerable<T> obj)
        {
            int ret = 0;
            int i=1;
            foreach (var v in obj)
            {
                ret ^= _int.GetHashCode(v)*i;
                i++;
            }
            return ret;
        }
    }
    public class ConstantCompararer<T> : IEqualityComparer<T>
    {
        private int _hash = 0;
        public ConstantCompararer(bool value)
        {
            this.value = value;
        }
        public bool value { get; }
        public bool Equals(T x, T y)
        {
            return value;
        }
        public int GetHashCode(T obj)
        {
            if (!value)
                _hash++;
            return _hash;
        }
    }
}
