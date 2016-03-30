using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Guard;
using Edge.NumbersMagic;
using Edge.SystemExtensions;
using Edge.Arrays.Arr2D;
using Edge.Tuples;

namespace Edge.Looping
{
    public static class Loops
    {
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> arr, ulong minoccurances = 2)
        {
            return Duplicates(arr, EqualityComparer<T>.Default, minoccurances);
        }
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> arr, IEqualityComparer<T> comp, ulong minoccurances = 2)
        {
            var occurances = new Dictionary<T, ulong>(comp);
            foreach (var t in arr)
            {
                if (occurances.EnsureDefinition(t) && occurances[t] == 0)
                    continue;
                occurances[t]++;
                if (occurances[t] >= minoccurances)
                {
                    yield return t;
                    occurances[t] = 0;
                }
            }
        }
        public static IEnumerable<T> Uniques<T>(this IEnumerable<T> arr, ulong maxoccurances = 1)
        {
            return Uniques(arr, EqualityComparer<T>.Default, maxoccurances);
        }
        public static IEnumerable<T> Uniques<T>(this IEnumerable<T> arr, IEqualityComparer<T> comp, ulong maxoccurances = 1)
        {
            return arr.ToOccurances(comp).Where(a => a.Value <= maxoccurances).Select(a => a.Key);
        }
        public static IEnumerable<T> Range<T>(T start, T max, T step)
        {
            var field = Fields.getField<T>();
            if (field.Equals(field.zero, step) && !field.Equals(start,max))
            {
                throw new ArgumentException("cannot be zero",nameof(step));
            }
            if ((field.Negatable && (field.isNegative(max.ToFieldWrapper() - start) != field.isNegative(step))) ||
                (!field.Negatable && (max.ToFieldWrapper() < start)))
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = field.isNegative(step);
            for (var i = start; (field.Compare(i, max) < 0) || (negstep && field.Compare(i, max) > 0 && field.Negatable);
                 i = field.add(i, step))
            {
                yield return i;
            }
        }
        public static IEnumerable<T> Range<T>(T start, T max)
        {
            var f = Fields.getField<T>();
            return Range(start, max, f.Compare(start, max) < 0 ? f.one : f.zero);
        }
        public static IEnumerable<T> Range<T>(T max)
        {
            return Range(Fields.getField<T>().zero, max);
        }
        public static IEnumerable<double> Range(double start, double max, double step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max != start && (max - start < 0 != step < 0))
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = step < 0;
            for (var i = start; i < max || (negstep && i > max); i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<double> Range(double start, double max)
        {
            return Range(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<double> Range(double max)
        {
            return Range(0.0, max);
        }
        public static IEnumerable<ulong> Range(ulong start, ulong max, long step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max < start)
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)}");
            }
            for (var i = start; i < max; i = (ulong)((long)i+step))
            {
                yield return i;
            }
        }
        public static IEnumerable<ulong> Range(ulong start, ulong max)
        {
            return Range(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<ulong> Range(ulong max)
        {
            return Range(0, max);
        }
        public static IEnumerable<int> Range(int start, int max, int step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max != start && (max - start < 0 != step < 0))
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = step < 0;
            for (var i = start; i < max || (negstep && i > max); i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<int> Range(int start, int max)
        {
            return Range(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<int> Range(int max)
        {
            return Range(0, max);
        }
        public static IEnumerable<T> IRange<T>(T start, T max, T step)
        {
            var field = Fields.getField<T>();
            if (field.Equals(field.zero, step) && !field.Equals(start,max))
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (!field.Equals(max,start) && (field.Negatable && (field.isNegative(max.ToFieldWrapper() - start) != field.isNegative(step))) ||
                (!field.Negatable && (max.ToFieldWrapper() < start)))
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = field.isNegative(step);
            for (var i = start; (!negstep && field.Compare(i, max) <= 0) || (negstep && field.Compare(i, max) >= 0);
                 i = field.add(i, step))
            {
                yield return i;
            }
        }
        public static IEnumerable<T> IRange<T>(T start, T max)
        {
            var f = Fields.getField<T>();
            return IRange(start, max, f.Compare(start, max) < 0 ? f.one : f.zero);
        }
        public static IEnumerable<T> IRange<T>(T max)
        {
            return IRange(Fields.getField<T>().zero, max);
        }
        public static IEnumerable<double> IRange(double start, double max, double step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max != start && max - start < 0 != step < 0)
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = step < 0;
            for (var i = start; (!negstep && i <= max) || (negstep && i >= max); i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<double> IRange(double start, double max)
        {
            return IRange(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<double> IRange(double max)
        {
            return IRange(0, max);
        }
        public static IEnumerable<ulong> IRange(ulong start, ulong max, long step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max < start)
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)}");
            }
            for (var i = start; i <= max; i = (ulong)((long)i+ step))
            {
                yield return i;
            }
        }
        public static IEnumerable<ulong> IRange(ulong start, ulong max)
        {
            return IRange(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<ulong> IRange(ulong max)
        {
            return IRange(0, max);
        }
        public static IEnumerable<int> IRange(int start, int max,int step)
        {
            if (step == 0)
            {
                throw new ArgumentException("cannot be zero", nameof(step));
            }
            if (max != start && max - start < 0 != step < 0)
            {
                throw new ArgumentException($"{nameof(max)} must be higher than {nameof(start)} or the {nameof(step)} must be negative");
            }
            var negstep = step < 0;
            for (var i = start; (!negstep && i <= max) || (negstep && i >= max); i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<int> IRange(int start, int max)
        {
            return IRange(start, max, start < max ? 1 : -1);
        }
        public static IEnumerable<int> IRange(int max)
        {
            return IRange(0, max);
        }
        public static IEnumerable<double> Count(double start, double step = 1)
        {
			for (var i = start; ; i += step)
			{
				yield return i;
			}
		}
        public static IEnumerable<ulong> Count(ulong start, ulong step = 1)
        {
            for (var i = start; ; i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<int> Count(int start = 0, int step = 1)
        {
            for (var i = start; ; i += step)
            {
                yield return i;
            }
        }
        public static IEnumerable<T> Count<T>(T start, T step)
        {
            var field = Fields.getField<T>();
            for (T i = start; ; i = field.add(i,step))
            {
                yield return i;
            }
        }
        public static IEnumerable<T> Count<T>(T start)
        {
            return Count(start, Fields.getField<T>().one);
        }
        public static IEnumerable<T> Count<T>()
        {
            return Count(Fields.getField<T>().zero);
        }
        public static IEnumerable<Position> Infinite()
        {
            yield return Position.First|Position.Middle;
            while (true)
            {
                yield return Position.Middle;
            }
        }
        public static void Repeat(this int repeatcount, Action action)
        {
            foreach (int i in Range(repeatcount))
            {
                action?.Invoke();
            }
        }
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> @this, int count)
        {
            foreach (int i in Range(count))
            {
                foreach (var t in @this)
                {
                    yield return t;
                }
            }
        }
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> @this, ulong count)
        {
            foreach (int i in Range(count))
            {
                foreach (var t in @this)
                {
                    yield return t;
                }
            }
        }
        public static IEnumerable<T1> Detach<T1, T2>(this IEnumerable<Tuple<T1, T2>> @this, Guard<T2> informer1)
        {
            foreach (var t in @this)
            {
                informer1.value = t.Item2;
                yield return t.Item1;
            }
        }
        public static IEnumerable<T1> Detach<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> @this, Guard<T2> informer1, Guard<T3> informer2)
        {
            foreach (var t in @this)
            {
                informer1.value = t.Item2;
                informer2.value = t.Item3;
                yield return t.Item1;
            }
        }
        public static IEnumerable<Tuple<T1,T2>> Detach<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> @this, Guard<T3> informer1)
        {
            foreach (var t in @this)
            {
                informer1.value = t.Item3;
                yield return Tuple.Create(t.Item1,t.Item2);
            }
        }
        public static IEnumerable<T1> Detach<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> @this, Guard<T2> informer1, Guard<T3> informer2, Guard<T4> informer3)
        {
            foreach (var t in @this)
            {
                informer1.value = t.Item2;
                informer2.value = t.Item3;
                informer3.value = t.Item4;
                yield return t.Item1;
            }
        }
        public static IEnumerable<Tuple<T1,T2>> Detach<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> @this, Guard<T3> informer2, Guard<T4> informer3)
        {
            foreach (var t in @this)
            {
                informer2.value = t.Item3;
                informer3.value = t.Item4;
                yield return Tuple.Create(t.Item1, t.Item2);
            }
        }
        public static IEnumerable<Tuple<T1, T2, T3>> Detach<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> @this, Guard<T4> informer3)
        {
            foreach (var t in @this)
            {
                informer3.value = t.Item4;
                yield return Tuple.Create(t.Item1, t.Item2, t.Item3);
            }
        }
        public static IEnumerable<Tuple<T1, T2>> AttachEnumerable<T1, T2>(this IEnumerable<T1> @this, Func<IEnumerable<T1>, IEnumerable<T2>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentException();
            }
            return @this.Zip(selector(@this));
        }
        public static IEnumerable<Tuple<T1, T2>> Attach<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> selector)
        {
            return @this.Zip(@this.Select(selector));
        }
        public static IEnumerable<Tuple<T1, T2>> Attach<T1, T2>(this IEnumerable<Tuple<T1>> @this, Func<T1, T2> selector)
        {
            return @this.Select(a => Tuple.Create(a.Item1, selector(a.Item1)));
        }
        public static IEnumerable<Tuple<T1, T2, T3>> Attach<T1, T2, T3>(this IEnumerable<Tuple<T1,T2>> @this, Func<T1, T2, T3> selector)
        {
            return @this.Select(a => Tuple.Create(a.Item1, a.Item2, selector(a.Item1, a.Item2)));
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4>> Attach<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3>> @this, Func<T1, T2, T3, T4> selector)
        {
            return @this.Select(a => Tuple.Create(a.Item1, a.Item2, a.Item3, selector(a.Item1, a.Item2, a.Item3)));
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5>> Attach<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4>> @this, Func<T1, T2, T3, T4, T5> selector)
        {
            return @this.Select(a => Tuple.Create(a.Item1, a.Item2, a.Item3, a.Item4, selector(a.Item1, a.Item2, a.Item3, a.Item4)));
        }
        public static IEnumerable<IEnumerable<T>> Zip<T>(IEnumerable<IEnumerable<T>> @this)
        {
            var tor = @this.SelectToArray(a => a.GetEnumerator());
            while (tor.All(a => a.MoveNext()))
            {
                yield return tor.Select(a => a.Current);
            }
        }
        public static IEnumerable<IEnumerable> Zip(IEnumerable<IEnumerable> @this)
        {
            var tor = @this.SelectToArray(a => a.GetEnumerator());
            while (tor.All(a=>a.MoveNext()))
            {
                yield return tor.Select(a=>a.Current);
            }
        }
        public static IEnumerable<Tuple<T1>> Zip<T1>(this IEnumerable<T1> a)
        {
            return Zip(new IEnumerable[] { a }).Select(x => x.ToTuple<T1>());
        }
        public static IEnumerable<Tuple<T1,T2>> Zip<T1, T2>(this IEnumerable<T1> a, IEnumerable<T2> b)
        {
            return Zip(new IEnumerable[] {a, b}).Select(x=>x.ToTuple<T1,T2>());
        }
        public static IEnumerable<Tuple<T1, T2,T3>> Zip<T1, T2,T3>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c)
        {
            return Zip(new IEnumerable[] { a, b, c }).Select(x => x.ToTuple<T1, T2, T3>());
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4>> Zip<T1, T2, T3,T4>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c, IEnumerable<T4> d)
        {
            return Zip(new IEnumerable[] { a, b, c, d }).Select(x => x.ToTuple<T1, T2, T3,T4>());
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4,T5>> Zip<T1, T2, T3, T4, T5>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c, IEnumerable<T4> d, IEnumerable<T5> e)
        {
            return Zip(new IEnumerable[] { a, b, c, d,e }).Select(x => x.ToTuple<T1, T2, T3, T4, T5>());
        }
        public static IEnumerable<IEnumerable> ZipUnbound(IEnumerable<IEnumerable> @this,params object[] defvals)
        {
            IEnumerator[] tor = @this.SelectToArray(a => a.GetEnumerator());
            while (true)
            {
                bool cont = false;
                for(int i = 0; i < tor.Length; i++)
                {
                    if (tor[i] == null)
                        continue;
                    if (tor[i].MoveNext())
                        cont = true;
                    else
                        tor[i] = null;
                }
                if (!cont)
                    yield break;
                yield return tor.CountBind().Select(a => a.Item1 == null? (defvals.Length > a.Item2 ? defvals[a.Item2] : null) : a.Item1.Current);
            }
        }
        public static IEnumerable<IEnumerable<T>> ZipUnbound<T>(IEnumerable<IEnumerable<T>> @this, params T[] defvals)
        {
            IEnumerator<T>[] tor = @this.SelectToArray(a => a.GetEnumerator());
            while (true)
            {
                bool cont = false;
                for (int i = 0; i < tor.Length; i++)
                {
                    if (tor[i] == null)
                        continue;
                    if (tor[i].MoveNext())
                        cont = true;
                    else
                        tor[i] = null;
                }
                if (!cont)
                    yield break;
                yield return tor.CountBind().Select(a => a.Item1 == null ? (defvals.Length > a.Item2 ? defvals[a.Item2] : default(T)) : a.Item1.Current);
            }
        }
        public static IEnumerable<Tuple<T1>> ZipUnbound<T1>(this IEnumerable<T1> a, T1 defa = default(T1))
        {
            return ZipUnbound(new IEnumerable[] {a}, defa).Select(x => x.ToTuple<T1>());
        }
        public static IEnumerable<Tuple<T1, T2>> ZipUnbound<T1,T2>(this IEnumerable<T1> a, IEnumerable<T2> b, T1 defa = default(T1), T2 defb = default(T2))
        {
            return ZipUnbound(new IEnumerable[] { a ,b}, defa, defb).Select(x => x.ToTuple<T1, T2>());
        }
        public static IEnumerable<Tuple<T1, T2, T3>> ZipUnbound<T1, T2,T3>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c, T1 defa = default(T1), T2 defb = default(T2), T3 defc = default(T3))
        {
            return ZipUnbound(new IEnumerable[] { a, b ,c}, defa, defb,defc).Select(x => x.ToTuple<T1, T2, T3>());
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4>> ZipUnbound<T1, T2, T3,T4>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c,IEnumerable<T4> d, T1 defa = default(T1), T2 defb = default(T2), T3 defc = default(T3), T4 defd = default(T4))
        {
            return ZipUnbound(new IEnumerable[] { a, b, c ,d}, defa,defb,defc,defd).Select(x => x.ToTuple<T1, T2, T3,T4>());
        }
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5>> ZipUnbound<T1, T2, T3, T4, T5>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c, IEnumerable<T4> d, IEnumerable<T5> e, T1 defa = default(T1), T2 defb = default(T2), T3 defc = default(T3), T4 defd = default(T4), T5 defe = default(T5))
        {
            return ZipUnbound(new IEnumerable[] { a, b, c ,d,e}, defa, defb, defc, defd,defe).Select(x => x.ToTuple<T1, T2, T3, T4, T5>());
        }
        public static IEnumerable<T> Enumerate<T>(this T b)
        {
            yield return b;
        }
		public static IEnumerable<T> Concat<T>(this IEnumerable<IEnumerable<T>> a)
		{
			foreach (IEnumerable<T> i in a)
			{
			    foreach (T t in i)
			    {
			        yield return t;
			    }
			}
		}
        public static IEnumerable<T> Choose<T>(this IEnumerable<IEnumerable<T>> @this, Func<T[], int> chooser)
        {
            var numerators = new List<IEnumerator<T>>(@this.SelectToArray(a=>a.GetEnumerator()));
            numerators.RemoveAll(a => !a.MoveNext());
            while (numerators.Any())
            {
                int index = chooser(numerators.SelectToArray(a => a.Current));
                yield return numerators[index].Current;
                if (!numerators[index].MoveNext())
                    numerators.RemoveAt(index);
            }
        }
        public static IEnumerable<T> Choose<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            return Choose(@this, other, Comparer<T>.Default);
        }
        public static IEnumerable<T> Choose<T>(this IEnumerable<T> @this, IEnumerable<T> other, IComparer<T> chooser)
        {
            return new IEnumerable<T>[] {@this,other}.Choose(a=>a.Length > 1 ? (chooser.Compare(a[0],a[1]) < 0 ? 0 : 1) : 0);
        }
        public static IEnumerable<T> Switch<T>(this IEnumerable<IEnumerable<T>> @this)
	    {
		    IEnumerable<IEnumerator<T>> numerators = @this.SelectToArray(a => a.GetEnumerator());
		    bool returned = true;
		    while (returned)
		    {
			    returned = false;
			    foreach (IEnumerator<T> e in numerators)
			    {
				    if (e.MoveNext())
				    {
					    returned = true;
					    yield return e.Current;
				    }
			    }
		    }
	    }
        public static IEnumerable<T> SwitchUnbound<T>(this IEnumerable<IEnumerable<T>> @this, T def = default(T))
        {
            var numerators = @this.SelectToArray(a => a.GetEnumerator());
            var buffer = new List<T>(numerators.Length);
            while (numerators.Any(a=>a!=null))
            {
                for (int i = 0; i < numerators.Length; i++)
                {
                    IEnumerator<T> e = numerators[i];
                    if (e != null && e.MoveNext())
                        buffer.Add(e.Current);
                    else
                    {
                        numerators[i] = null;
                        buffer.Add(def);
                    }
                }
                if (numerators.Any(a => a != null))
                {
                    foreach (T t in buffer)
                    {
                        yield return t;
                    }
                    buffer.Clear();
                }
            }
        }
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> @this)
	    {
		    while (true)
		    {
			    foreach (T t in @this)
			    {
				    yield return t;
			    }
		    }
	    }
		public static IEnumerable<T> Where<T>(this IEnumerable<T> @this, params T[] toinclude)
		{
			return @this.Where(toinclude.Contains);
		}
		public static IEnumerable<T> Except<T>(this IEnumerable<T> @this, params T[] toexclude)
		{
			return @this.Where(a=>!toexclude.Contains(a));
		}
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> @this)
        {
            return OrderBy(@this, Comparer<T>.Default);
        }
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> @this, IComparer<T> comp)
        {
            return @this.OrderBy(a => a, comp);
        }
        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> @this)
        {
            return OrderByDescending(@this, Comparer<T>.Default);
        }
        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> @this, IComparer<T> comp)
        {
            return @this.OrderByDescending(a => a, comp);
        }
        [Flags]public enum CartesianType { AllPairs = 0, NoSymmatry = 1, NoReflexive = 2 }
        public static IEnumerable<Tuple<T, T>> Join<T>(this IEnumerable<T> a, CartesianType t = CartesianType.AllPairs)
        {
            foreach (var v0 in a.CountBind())
            {
                var iEnumerable = a.CountBind();
                if (t.HasFlag(CartesianType.NoSymmatry))
                    iEnumerable = iEnumerable.Take(v0.Item2+1);
                foreach (var v1 in iEnumerable)
                {
                    if (t.HasFlag(CartesianType.NoReflexive) && v0.Item2 == v1.Item2)
                        continue;
                    yield return new Tuple<T, T>(v0.Item1, v1.Item1);
                }
            }
        }
        public static IEnumerable<Tuple<T1, T2>> Join<T1, T2>(this IEnumerable<T1> a, IEnumerable<T2> b)
        {
            foreach (T1 v0 in a)
            {
                foreach (T2 v1 in b)
                {
                    yield return new Tuple<T1, T2>(v0, v1);
                }
            }
        }
        public static IEnumerable<Tuple<T1, T2, T3>> Join<T1, T2, T3>(this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c)
        {
            foreach (T1 v0 in a)
            {
                foreach (T2 v1 in b)
                {
                    foreach (T3 v2 in c)
                    {
                        yield return Tuple.Create(v0, v1,v2);
                    }
                }
            }
        }
	    public static IEnumerable<T[]> Join<T>(this IEnumerable<T> @this, int cartesLength, CartesianType t = CartesianType.AllPairs)
	    {
            var tors = @this.Enumerate().Repeat(cartesLength).SelectToArray(a => a.CountBind().GetEnumerator());
            //initialization
	        if (t.HasFlag(CartesianType.NoReflexive))
	        {
	            foreach (var enumerator in tors.CountBind())
	            {
	                foreach (var i in Range(tors.Length-enumerator.Item2))
	                {
	                    if (!enumerator.Item1.MoveNext())
	                        yield break;
	                }
	            }
	        }
            else {
                if (tors.Any(a => !a.MoveNext()))
                    yield break;
            }
            //yield initial
            yield return tors.SelectToArray(a => a.Current.Item1);
            while (true)
            {
                int nexttorind = 0;
                while (true)
                {
                    if (!tors.isWithinBounds(nexttorind))
                        yield break;
                    if (!tors[nexttorind].MoveNext())
                    {
                        tors[nexttorind] = @this.CountBind().GetEnumerator();
                        tors[nexttorind].MoveNext();
                        nexttorind++;
                    }
                    else
                    {
                        if (t.HasFlag(CartesianType.NoSymmatry) && nexttorind > 0)
                        {
                            bool retry = false;
                            foreach (var i in Range(0, nexttorind))
                            {
                                foreach (int i1 in Range(tors[nexttorind].Current.Item2 + (t.HasFlag(CartesianType.NoReflexive) ? i + 1 : 0)))
                                {
                                    if (!tors[nexttorind - i - 1].MoveNext())
                                    {
                                        retry = true;
                                        break;
                                    }
                                }
                                if (retry)
                                    break;
                            }
                            if (retry)
                            {
                                foreach (int i in IRange(nexttorind))
                                {
                                    tors[i] = @this.CountBind().GetEnumerator();
                                    tors[i].MoveNext();
                                }
                                nexttorind++;
                                continue;
                            }
                        }
                        break;
                    }
                }
                if (t.HasFlag(CartesianType.NoReflexive) && !t.HasFlag(CartesianType.NoSymmatry) &&
                        tors.Join(CartesianType.NoReflexive | CartesianType.NoSymmatry).Any(
                            a => a.Item1.Current.Item2 == a.Item2.Current.Item2))
                {
                    continue;
                }
                yield return tors.SelectToArray(a => a.Current.Item1);
            }
        }
        public static IEnumerable<T[]> Join<T>(this IList<IEnumerable<T>> @this)
        {
            var tors = @this.SelectToArray(a => a.GetEnumerator());
            //initialization
            if (tors.Any(a=>!a.MoveNext()))
                yield break;
            //yield initial
            yield return tors.SelectToArray(a => a.Current);
            while (true)
            {
                int nexttorind = 0;
                while (true)
                {
                    if (!tors.isWithinBounds(nexttorind))
                        yield break;
                    if (!tors[nexttorind].MoveNext())
                    {
                        tors[nexttorind] = @this[nexttorind].GetEnumerator();
                        tors[nexttorind].MoveNext();
                        nexttorind++;
                    }
                    else
                    {
                        break;
                    }
                }
                yield return tors.SelectToArray(a => a.Current);
            }
        }
        public static IEnumerable<IEnumerable<T>> SubSets<T>(this IEnumerable<T> @this)
        {
            return Count().Select(@this.SubSets).TakeWhile(a=>a.Any()).Concat();
        }
        public static IEnumerable<IEnumerable<int>> MultiSubSets(this IEnumerable<int> @this, bool inclusive = true)
        {
            return @this.SelectToArray(a=>(inclusive ? IRange(a) : Range(a)).ToArray()).Join();
        }
        public static IEnumerable<IEnumerable<T>> MultiSubSets<T>(this IEnumerable<T> @this, bool inclusive = true)
        {
            return @this.SelectToArray(a => (inclusive ? IRange(a) : Range(a)).ToArray()).Join();
        }
        public static IEnumerable<IEnumerable<Tuple<T,int>>> MultiSubSets<T>(this IEnumerable<Tuple<T,int>> @this, bool inclusive = true)
        {
            var items = @this.SelectToArray(a => a.Item1);
            return @this.Select(a=>a.Item2).MultiSubSets(inclusive).Select(x=>items.Zip(x));
        }
        public static IEnumerable<IEnumerable<Tuple<T, G>>> MultiSubSets<T,G>(this IEnumerable<Tuple<T, G>> @this, bool inclusive = true)
        {
            var items = @this.SelectToArray(a => a.Item1);
            return @this.Select(a => a.Item2).MultiSubSets(inclusive).Select(x => items.Zip(x));
        }
        public static IEnumerable<IEnumerable<T>> SubSets<T>(this IEnumerable<T> @this, int setSize)
        {
            return @this.Join(setSize, CartesianType.NoReflexive | CartesianType.NoSymmatry);
        }
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> @this)
        {
            return @this.Join(@this.Count(), CartesianType.NoReflexive);
        }
        public static IEnumerable<Tuple<T, int>> CountBind<T>(this IEnumerable<T> a, int start = 0)
	    {
		    return a.Zip(Count(start));
	    }
        public static IEnumerable<Tuple<T, C>> CountBind<T,C>(this IEnumerable<T> a, C start)
        {
            return a.Zip(Count(start));
        }
        [Flags]public enum Position { First = 1, Middle = 2, Last = 4, None=0, Only = First|Middle|Last}
        public static IEnumerable<Tuple<T, Position>> PositionBind<T>(this IEnumerable<T> @this)
        {
            bool first = true;
            var num = @this.GetEnumerator();
            bool last = !num.MoveNext();
            while (!last)
            {
                var v = num.Current;
                last = !num.MoveNext();
                Position ret = Position.Middle;
                if (first)
                {
                    ret |= Position.First;
                    first = false;
                }
                if (last)
                {
                    ret |= Position.Last;
                }
                yield return Tuple.Create(v, ret);
            }
        } 
        public static IEnumerable<Tuple<T, int, int>> CoordinateBind<T>(this T[,] @this)
        {
            foreach (int row in Range(@this.GetLength(0)))
            {
                foreach (int col in Range(@this.GetLength(1)))
                {
                    yield return new Tuple<T, int, int>(@this[row, col], row, col);
                }
            }
        }
        public static IEnumerable<Tuple<T, int, int,int>> CoordinateBind<T>(this T[,,] @this)
        {
            foreach (int c0 in Range(@this.GetLength(0)))
            {
                foreach (int c1 in Range(@this.GetLength(1)))
                {
                    foreach (int c2 in Range(@this.GetLength(2)))
                    {
                        yield return new Tuple<T, int, int,int>(@this[c0, c1,c2], c0,c1,c2);
                    }
                }
            }
        }
        public static IEnumerable<Tuple<object, int[]>> CoordinateBind(this Array @this)
        {
            return @this.getSize().SelectToArray(Range).Join().Select(a => Tuple.Create(@this.GetValue(a),a));
        }
        public static IEnumerable<Tuple<T, int[]>> CoordinateBind<T>(this Array @this)
        {
            return @this.getSize().SelectToArray(Range).Join().Select(a => Tuple.Create((T)@this.GetValue(a), a));
        }
        public static IEnumerable<Tuple<T, int, int>> CoordinateBind<T>(this IEnumerable<IEnumerable<T>> @this)
        {
            foreach (var t1 in @this.CountBind())
            {
                foreach (var t0 in t1.Item1.CountBind())
                {
                    yield return Tuple.Create(t0.Item1,t1.Item2,t0.Item2);
                }
            }
        }
        public static IEnumerable<Tuple<T, int, int, int>> CoordinateBind<T>(this IEnumerable<IEnumerable<IEnumerable<T>>> @this)
        {
            foreach (var t2 in @this.CountBind())
            {
                foreach (var t1 in t2.Item1.CountBind())
                {
                    foreach (var t0 in t1.Item1.CountBind())
                    {
                        yield return Tuple.Create(t0.Item1, t2.Item2, t1.Item2, t0.Item2);
                    }
                }
            }
        }
        public static IEnumerable<T[]> Group<T>(this IEnumerable<T> @this, int grouplength, T defval = default(T))
	    {
			var en = @this.GetEnumerator();
			T[] ret = new T[grouplength];
            bool end = false, empty = true;
			while (true)
			{
				foreach (int i in Range(ret.Length))
				{
				    if (!en.MoveNext())
				    {
				        end = true;
				        break;
				    }
					ret[i] = en.Current;
				    empty = false;
				}
			    if (end)
			        break;
				yield return ret;
                ret.Fill(defval);
			    empty = true;
			}
            if (!empty)
                yield return ret;
	    }
        public static IEnumerable<Tuple<T>> Group1<T>(this IEnumerable<T> a, T defval = default(T))
        {
            return a.Group(1, defval).Select(x => x.ToTuple1());
        }
        public static IEnumerable<Tuple<T, T>> Group2<T>(this IEnumerable<T> a, T defval = default(T))
	    {
		    return a.Group(2,defval).Select(x => x.ToTuple2());
	    }
		public static IEnumerable<Tuple<T, T, T>> Group3<T>(this IEnumerable<T> a, T defval = default(T))
		{
			return a.Group(3, defval).Select(x => x.ToTuple3());
		}
		public static IEnumerable<Tuple<T, T, T, T>> Group4<T>(this IEnumerable<T> a, T defval = default(T))
		{
			return a.Group(4, defval).Select(x => x.ToTuple4());
		}
		public static IEnumerable<Tuple<T, T, T, T, T>> Group5<T>(this IEnumerable<T> a, T defval = default(T))
		{
			return a.Group(5, defval).Select(x => x.ToTuple5());
		}
        public static IEnumerable<T[]> Trail<T>(this IEnumerable<T> @this, int trailLength, bool wrap = false)
        {
            while (true)
            {
                var buffer = new LinkedList<T>();
                if (wrap)
                {
                    @this = (@this).Concat(@this.Take(trailLength-1));
                    wrap = false;
                    continue;
                }
                foreach (T t in @this)
                {
                    buffer.AddLast(t);
                    while (buffer.Count > trailLength)
                    {
                        buffer.RemoveFirst();
                    }
                    if (buffer.Count == trailLength)
                    {
                        yield return buffer.ToArray();
                    }
                }
                break;
            }
        }
        public static IEnumerable<Tuple<T>> Trail1<T>(this IEnumerable<T> @this, bool wrap = false)
        {
            return @this.Trail(1, wrap).Select(a => a.ToTuple1());
        }
        public static IEnumerable<Tuple<T, T>> Trail2<T>(this IEnumerable<T> @this, bool wrap = false)
	    {
		    return @this.Trail(2,wrap).Select(a => a.ToTuple2());
	    }
		public static IEnumerable<Tuple<T, T, T>> Trail3<T>(this IEnumerable<T> @this, bool wrap = false)
		{
			return @this.Trail(3, wrap).Select(a => a.ToTuple3());
		}
		public static IEnumerable<Tuple<T, T, T, T>> Trail4<T>(this IEnumerable<T> @this, bool wrap = false)
		{
			return @this.Trail(4, wrap).Select(a => a.ToTuple4());
		}
		public static IEnumerable<Tuple<T, T, T, T, T>> Trail5<T>(this IEnumerable<T> @this, bool wrap = false)
		{
			return @this.Trail(5, wrap).Select(a => a.ToTuple5());
		}
		public static ILookup<T,T> ToLookup<T>(this IEnumerable<T> @this, IEqualityComparer<T> matcher)
		{
		    return @this.ToLookup(a => a, matcher);
		}
        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> @this, int start = 0, int count = -1, int step = 1)
        {
            var temp = @this.Skip(start).Step(step);
            return count >= 0 ? temp.Take(count) : temp;
        }
        public static IEnumerable<T> MutateOnEnumerations<T>(this IEnumerable<T> @this, Action<T> mutation)
        {
            foreach (T t in @this)
            {
                t.Mutate(mutation);
                yield return t;
            }
        }
        public static void Do<T>(this IEnumerable<T> @this, Action<T> action = null)
        {
            foreach (T t in @this)
            {
                action?.Invoke(t);
            }
        }
        public static IEnumerable<T> EnumerationHook<T>(this IEnumerable<T> @this,Action preNumeration = null, Action postNumeration = null, Action<T> preYield = null, Action<T> postYield = null)
        {
            preNumeration?.Invoke();
            foreach (var t in @this)
            {
                preYield?.Invoke(t);
                yield return t;
                postYield?.Invoke(t);
            }
            postNumeration?.Invoke();
        }
        public static IEnumerable<T> EnumerationHook<T,G>(this IEnumerable<T> @this, Func<G> preNumeration, Action<G> postNumeration = null, Func<T,G,G> preYield = null, Func<T,G,G> postYield = null)
        {
            var val = preNumeration.Invoke();
            foreach (var t in @this)
            {
                if (preYield != null)
                    val = preYield.Invoke(t,val);
                yield return t;
                if (postYield != null)
                    val = postYield.Invoke(t,val);
            }
            postNumeration?.Invoke(val);
        }
        [Flags] public enum LoopControl { None=0, Skip=2, Break=3, After = 4, SkipAfter = Skip|After , BreakAfter = Break|After}
        public static IEnumerable<T> EnumerationHook<T, G>(this IEnumerable<T> @this, Func<G> preNumeration, Action<G> postNumeration = null, Func<T, G, Tuple<G, LoopControl>> preYield = null, Func<T, G, Tuple<G, LoopControl>> postYield = null)
        {
            var val = preNumeration.Invoke();
            var preControl = LoopControl.None;
            foreach (var t in @this)
            {
                if (preControl.HasFlag(LoopControl.After))
                    preControl &= ~LoopControl.After;
                else
                    preControl = LoopControl.None;
                if (preYield != null)
                {
                    var o = preYield.Invoke(t, val);
                    val = o.Item1;
                    var curControl = o.Item2;
                    if (o.Item2.HasFlag(LoopControl.After))
                        curControl = LoopControl.None;
                    var control = curControl | preControl;
                    preControl = o.Item2;
                    if (control.HasFlag(LoopControl.Break))
                        break;
                    if (control.HasFlag(LoopControl.Skip))
                        continue;
                }
                yield return t;
                if (postYield != null)
                {
                    var o = preYield.Invoke(t, val);
                    val = o.Item1;
                    var curControl = o.Item2;
                    if (o.Item2.HasFlag(LoopControl.After))
                        curControl = LoopControl.None;
                    var control = curControl | preControl;
                    preControl = o.Item2;
                    if (control.HasFlag(LoopControl.Break))
                        break;
                }
            }
            postNumeration?.Invoke(val);
        }
        public static IEnumerable<T> Enum<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }
        public static IEnumerable<T> EnumFlags<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type with [Flags] attribute");
            }
            return System.Enum.GetValues(typeof(T)).Cast<int>().Where(a=>a.CountSetBits() == 1).Cast<T>();
        }
        public static IEnumerable<T> EnumFlags<T>(this T filter) where T : struct, IConvertible
        {
            var f = (Enum)(dynamic)filter;
            return EnumFlags<T>().Cast<Enum>().Where(a => (f.HasFlag(a))).Cast<T>();
        }
        public static IEnumerable<R> YieldAggregate<T, R>(this IEnumerable<T> @this, Func<T, R, R> aggregator,R seed = default(R))
        {
            foreach (T t in @this)
            {
                seed = aggregator(t, seed);
                yield return seed;
            }
        }
        public static IEnumerable<T> YieldAggregate<T>(Func<T, T> aggregator, T seed = default(T))
        {
            while (true)
            {
                yield return seed;
                seed = aggregator(seed);
            }
        }
        public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> @this)
        {
            return @this.ToDictionary(a => a.Key, a => a.Value);
        }
        public static IDictionary<K, V> ToDictionary<K, V>(this IEnumerable<Tuple<K, V>> @this)
        {
            return @this.ToDictionary(a => a.Item1, a => a.Item2);
        }
        public static IEnumerable<T> Generate<T>(Func<Tuple<T,LoopControl>> gen)
        {
            bool skipfrombefore = false;
            while (true)
            {
                var val = gen?.Invoke();
                if (val.Item2 == LoopControl.Skip || skipfrombefore)
                {
                    skipfrombefore = false;
                    continue;
                }
                if (val.Item2 == LoopControl.Break)
                    break;
                yield return val.Item1;
                if (val.Item2 == LoopControl.BreakAfter)
                    break;
                if (val.Item2 == LoopControl.SkipAfter)
                    skipfrombefore = true;
            }
        }
        public static IEnumerable<T> Generate<T>(Func<T> gen)
        {
            while (true)
            {
                yield return gen();
            }
        }
        public static EnumerableCache<T> Cache<T>(this IEnumerable<T> @this)
        {
            return new EnumerableCache<T>(@this);
        }
        public static IEnumerable<T> Step<T>(this IEnumerable<T> @this, int step = 2, int start  = 0)
        {
            int c = start;
            foreach (var t in @this)
            {
                if (c == 0)
                    yield return t;
                c++;
                if (c == step)
                    c = 0;
            }
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> @this, IEqualityComparer<T> splitter)
        {
            var splitpoints = @this.Trail2().CountBind(1).Where(a => !splitter.Equals(a.Item1.Item1, a.Item1.Item2)).Select(a => a.Item2);
            splitpoints = 0.Enumerate().Concat(splitpoints).Concat(@this.Count().Enumerate());
            var splitranges = splitpoints.Trail2().Select(a => Tuple.Create(a.Item1, a.Item2 - a.Item1));
            return splitranges.Select(a => @this.SubEnumerable(a.Item1, a.Item2));
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> @this, T divider, bool includedivider = false)
        {
            return Split(@this, a=>a.Equals(divider), includedivider);
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> @this, Func<T,bool> dividerdetector, bool includedivider = false)
        {
            var splitpoints = @this.CountBind().Where(a => dividerdetector(a.Item1)).Select(a => a.Item2);
            splitpoints = (includedivider ? 0 : -1).Enumerate().Concat(splitpoints).Concat((@this.Count() + (includedivider ? 0 : 1)).Enumerate() );
            var splitranges = splitpoints.Trail2().Select(a => Tuple.Create(a.Item1 + (includedivider ? 0 : 1), a.Item2 - a.Item1 + (includedivider ? 0 : -1)));
            return splitranges.Select(a => @this.SubEnumerable(a.Item1, a.Item2));
        }
        public static IEnumerable<T> PartialSums<T>(this IEnumerable<T> @this)
        {
            var f = Fields.getField<T>();
            return @this.YieldAggregate(f.add, f.zero);
        }
        public static IEnumerable<T> PartialProduct<T>(this IEnumerable<T> @this)
        {
            var f = Fields.getField<T>();
            return @this.YieldAggregate(f.multiply, f.one);
        }
        public static IEnumerable<T> SortedUnique<T>(this IEnumerable<T> @this, IEqualityComparer<T> comp = null)
        {
            comp = comp ?? EqualityComparer<T>.Default;
            T last = default(T);
            bool any = false;
            foreach (T t in @this)
            {
                if (!any || !comp.Equals(last,t))
                {
                    yield return t;
                    last = t;
                    any = true;
                }
            }
        }
        public static IEnumerable<Tuple<T,T>> SharedPrefix<T>(this IEnumerable<T> @this, IEnumerable<T> other, IEqualityComparer<T> comp = null)
        {
            comp = comp ?? EqualityComparer<T>.Default;
            return @this.Zip(other).TakeWhile(a=>comp.Equals(a.Item1,a.Item2));
        }
    }
    public static class Hooking
    {
        public static IEnumerable<T> HookComp<T>(this IEnumerable<T> @this, IGuard<T> min = null, IGuard<T> max = null)
        {
            return HookComp(@this, Comparer<T>.Default, min, max);
        }
        public static IEnumerable<T> HookComp<T>(this IEnumerable<T> @this, IComparer<T> comp, IGuard<T> min=null, IGuard<T> max=null)
        {
            bool any = false;
            foreach (T t in @this)
            {
                if (min!=null && (!any || comp.Compare(min.value, t) > 0))
                    min.value = t;
                if (max != null && (!any || comp.Compare(max.value, t) < 0))
                    max.value = t;
                yield return t;
                any = true;
            }
        }
        public static IEnumerable<T> HookCond<T>(this IEnumerable<T> @this, Func<T,bool> cond = null , IGuard<bool> any = null, IGuard<bool> all = null,IGuard<int> count = null)
        {
            any.CondSet(false);
            all.CondSet(true);
            count.CondSet(0);
            foreach (var t in @this)
            {
                bool c = cond?.Invoke(t) ?? true;
                if (c)
                {
                    any.CondSet(true);
                    count.CondMutate(a=>a+1);
                }
                else
                    all.CondSet(false);
                yield return t;
            }
        }
        public static IEnumerable<T> HookSelect<T>(this IEnumerable<T> @this, IGuard<T> sum = null, IGuard<T> product = null, IGuard<T> max = null, IGuard<T> min = null, IGuard<T> last = null, IGuard<T> first = null)
        {
            return HookSelect(@this, a=>a, sum, product, max, min, last, first);
        }
        public static IEnumerable<T> HookSelect<T, G>(this IEnumerable<T> @this, Func<T, G> map, IGuard<G> sum = null, IGuard<G> product = null, IGuard<G> max = null, IGuard<G> min = null, IGuard<G> last = null, IGuard<G> first = null)
        {
            Field<G> f;
            if (sum == null && product == null && min == null && max == null)
                f = null;
            else
                f = Fields.getField<G>();
            sum.CondSet(f.zero);
            product.CondSet(f.one);
            bool any = false;
            foreach (T t in @this)
            {
                var v = map(t);
                if (first != null && !any)
                    first.value = v;
                if (min != null && (!any || f.Compare(v, min.value) < 0))
                    min.value = v;
                if (max != null && (!any || f.Compare(v, max.value) > 0))
                    max.value = v;
                sum.CondMutate(a=>f.add(a, v));
                product.CondMutate(a=>f.multiply(a, v));
                last.CondSet(v);
                any = true;
                yield return t;
            }
        }
        public static IEnumerable<T> HookAggregate<T>(this IEnumerable<T> @this, Func<T, T, T> func, IGuard<T> aggregate)
        {
            return @this.HookAggregate<T, T>(func, aggregate);
        }
        public static IEnumerable<T> HookAggregate<T>(this IEnumerable<T> @this, Func<T, T, T> func, T seed, IGuard<T> aggregate)
        {
            return @this.HookAggregate<T, T>(func, seed, aggregate);
        }
        public static IEnumerable<T> HookAggregate<T, G>(this IEnumerable<T> @this, Func<T, G, G> func, G seed, IGuard<G> aggregate)
        {
            aggregate.value = seed;
            return @this.HookAggregate(func, aggregate);
        }
        public static IEnumerable<T> HookAggregate<T, G>(this IEnumerable<T> @this, Func<T, G, G> func, IGuard<G> aggregate)
        {
            foreach (var t in @this)
            {
                aggregate.value = func(t, aggregate.value);
                yield return t;
            }
        }
    }
}
