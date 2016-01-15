using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Edge.Comparison;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.SpecialNumerics;
using Edge.RandomGen;
using Edge.SystemExtensions;
using Edge.WordsPlay;

namespace Edge.Arrays
{
    // ReSharper disable once InconsistentNaming
    public static class arrayExtensions
    {
        public static bool SumDefinition<T, G>(this IDictionary<T, G> @this, T key, G val)
        {
            return AggregateDefinition(@this, key, val, Fields.getField<G>().add);
        }
        public static bool ProductDefinition<T, G>(this IDictionary<T, G> @this, T key, G val)
        {
            return AggregateDefinition(@this, key, val, Fields.getField<G>().multiply);
        }
        public static bool AggregateDefinition<T, G>(this IDictionary<T, G> @this, T key, G val, Func<G, G, G> aggfunc)
        {
            bool ret = @this.ContainsKey(key);
            @this[key] = ret ? aggfunc(val, @this[key]) : val;
            return ret;
        }
        public static bool EnsureDefinition<T, G>(this IDictionary<T, G> @this, T key, G defaultval = default(G))
        {
            if (@this.ContainsKey(key))
                return true;
            @this[key] = defaultval;
            return false;
        }
        //maximum is exclusive
        public static int binSearch(Func<int, int> searcher, int min, int max)
        {
            while (min < max)
            {
                int i = (min + max) / 2;
                int res = searcher(i);
                if (res == 0)
                    return i;
                if (i == min)
                {
                    break;
                }
                if (res > 0)
                    min = i;
                else
                    max = i;
            }
            return -1;
        }
        /// <summary>
        /// returns the last index at which searcher returns positive
        /// </summary>
        /// <param name="searcher"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int binSearch(Func<int, bool> searcher, int min, int max)
        {
            int rangemax = max;
            while (min < max)
            {
                int i = (min + max) / 2;
                var res = searcher(i);
                if (res == false)
                    max = i;
                else
                {
                    if (i + 1 >= rangemax || !searcher(i + 1))
                        return i;
                    min = i + 1;
                }
            }
            return -1;
        }
        /// <summary>
        /// returns the last index at which searcher returns positive
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortedarr"></param>
        /// <param name="searcher"></param>
        /// <returns></returns>
        public static int binSearch<T>(this IList<T> sortedarr, Func<T, bool> searcher)
        {
            return binSearch(i => searcher(sortedarr[i]), 0, sortedarr.Count);
        }
        /// <summary>
        /// searches an array with a binary search
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="sortedarr">the <b>sorted</b> array</param>
        /// <param name="searcher">a negative result means the target is before this element, a positive result means after, and zero means this is the target</param>
        /// <returns>the index of the sought element</returns>
        public static int binSearch<T>(this IList<T> sortedarr, Func<T, int> searcher)
        {
            return binSearch(i => searcher(sortedarr[i]), 0, sortedarr.Count);
        }
        /// <summary>
        /// searches an array with a binary search
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="sortedarr">the <b>sorted</b> array</param>
        /// <param name="tofind">the target to find</param>
        /// <param name="comp">the comparer the array was sorted by</param>
        /// <returns>the index of the sought element</returns>
        public static int binSearch<T>(this IList<T> sortedarr, T tofind, IComparer<T> comp)
        {
            return binSearch(sortedarr, a => comp.Compare(tofind, a));
        }
        /// <summary>
        /// searches an array with a binary search
        /// </summary>
        /// <typeparam name="T" />
        /// <param name="sortedarr">the <b>sorted</b> array</param>
        /// <param name="tofind">the target to find</param>
        /// <returns>the index of the sought element</returns>
        public static int binSearch<T>(this IList<T> sortedarr, T tofind)
        {
            return binSearch(sortedarr, tofind, Comparer<T>.Default);
        }
        /// <summary>
        /// fill an array with values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tofill">array to fill</param>
        /// <param name="v">elements to fill the array with, they will repeat through the array</param>
        public static void Fill<T>(this IList<T> tofill, params T[] v)
        {
            tofill.Fill(v, 0);
        }
        public static void Fill<T>(this IList<T> tofill, T[] v, int start, int count = int.MaxValue)
        {
            if (v.Length == 0)
                throw new ArgumentException("cannot be empty", nameof(v));
            Fill(tofill, i => (v[i % v.Length]), start, count);
        }
        public static void Fill<T>(this IList<T> tofill, Func<int, T> filler, int start = 0, int count = int.MaxValue)
        {
            for (int i = 0; i < tofill.Count - start && i < count; i++)
            {
                if (i + start < 0)
                    continue;
                tofill[i + start] = filler(i + start);
            }
        }
        public static void Fill<T>(this IList<T> tofill, Func<T> filler, int start = 0, int count = int.MaxValue)
        {
            tofill.Fill(a => filler(), start, count);
        }
        public static T[] Fill<T>(int length, T[] filler, int start, int count = int.MaxValue)
        {
            T[] ret = new T[length];
            ret.Fill(filler, start, count);
            return ret;
        }
        public static T[] Fill<T>(int length, params T[] filler)
        {
            T[] ret = new T[length];
            ret.Fill(filler);
            return ret;
        }
        public static T[] Fill<T>(int length, Func<int, T> filler, int start = 0, int count = int.MaxValue)
        {
            T[] ret = new T[length];
            ret.Fill(filler, start, count);
            return ret;
        }
        public static T[] Fill<T>(int length, Func<T> filler, int start = 0, int count = int.MaxValue)
        {
            T[] ret = new T[length];
            ret.Fill(a => filler(), start, count);
            return ret;
        }
        /// <summary>
        /// translates an IEnumerable to a converted array
        /// </summary>
        /// <typeparam name="T" />
        /// <typeparam name="T1" />
        /// <param name="filler1">the <b>finite</b> enumerable to convert</param>
        /// <param name="function">the converter for the array</param>
        /// <returns>an array converted with the function</returns>
        public static T[] SelectToArray<T, T1>(this IEnumerable<T1> filler1, Func<T1, T> function)
        {
            return filler1.Select(function).ToArray();
        }
        private static int RecommendSize<T>(this IEnumerable<T> @this)
        {
            var collection = @this as ICollection<T>;
            return collection?.Count ?? 0;
        }
        public static T[] ToArray<T>(this IEnumerable<T> @this, Action<int> reporter)
        {
            return ToArray(@this, @this.RecommendSize(), (arg1, i) => reporter(i));
        }
        public static T[] ToArray<T>(this IEnumerable<T> @this, Action<T, int> reporter)
        {
            return ToArray(@this, @this.RecommendSize(), reporter);
        }
        public static T[] ToArray<T>(this IEnumerable<T> @this, int capacity)
        {
            return ToArray(@this, capacity, (Action<T, int>)null);
        }
        public static T[] ToArray<T>(this IEnumerable<T> @this, int capacity, Action<int> reporter)
        {
            return ToArray(@this, capacity, (arg1, i) => reporter(i));
        }
        public static T[] ToArray<T>(this IEnumerable<T> @this, int capacity, Action<T, int> reporter)
        {
            T[] ret = new T[capacity <= 0 ? 1 : capacity];
            int i = 0;
            foreach (T t in @this)
            {
                if (ret.Length <= i)
                    Array.Resize(ref ret, ret.Length * 2);
                ret[i] = t;
                reporter?.Invoke(t, i);
                i++;
            }
            Array.Resize(ref ret, i);
            return ret;
        }
        public static bool isSymmetrical<T>(this IList<T> @this)
        {
            return isSymmetrical(@this, EqualityComparer<T>.Default);
        }
        public static bool isSymmetrical<T>(this IList<T> @this, IEqualityComparer<T> c)
        {
            foreach (int i in Loops.Range(@this.Count / 2))
            {
                if (!c.Equals(@this[i], @this[(-i - 1).TrueMod(@this.Count)]))
                    return false;
            }
            return true;
        }
        public static IDictionary<T, ulong> ToOccurances<T>(this IEnumerable<T> arr)
        {
            return ToOccurances(arr, EqualityComparer<T>.Default);
        }
        public static IDictionary<T, ulong> ToOccurances<T>(this IEnumerable<T> arr, IEqualityComparer<T> c)
        {
            Dictionary<T, ulong> oc = new Dictionary<T, ulong>(c);
            foreach (T v in arr)
            {
                if (oc.ContainsKey(v))
                    oc[v]++;
                else
                    oc[v] = 1;
            }
            return oc;
        }
        /// <summary>
        /// converts a dictionary to an array where each key appears a number of times as its value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d">the dictionary</param>
        /// <returns>an array formed from the dictionary</returns>
        public static IEnumerable<T> FromOccurances<T>(this IEnumerable<KeyValuePair<T, ulong>> d)
        {
            foreach (KeyValuePair<T, ulong> pair in d)
            {
                foreach (int i in Loops.Range(pair.Value))
                    yield return pair.Key;
            }
        }
        public static bool ContainsAll<T>(this IEnumerable<T> @this, IEnumerable<T> vals)
        {
            return vals.All(@this.Contains);
        }
        public static bool ContainsAny<T>(this IEnumerable<T> @this, IEnumerable<T> vals)
        {
            return vals.Any(@this.Contains);
        }
        public static void WriteTo<T>(this IEnumerable<T> @this, TextWriter w, string seperator = ", ", string opening = "[", string closing = "]")
        {
            w.Write(opening);
            bool first = true;
            foreach (T temp in @this)
            {
                if (!first)
                    w.Write(seperator);
                first = false;
                w.Write(temp);
            }
            w.Write(closing);
        }
        public static string ToPrintable<T>(this IEnumerable<T> a, string seperator = ", ", string opening = "[", string closing = "]")
        {
            StringWriter w = new StringWriter();
            a.WriteTo(w, seperator, opening, closing);
            return w.ToString();
        }
        public static string ToPrintable<K,V>(this IDictionary<K,V> a, string definitionSeperator = ":", string seperator = ", ", string opening = "{", string closing = "}")
        {
            return ToPrintable(a, x=>x.ToString(), x=>x.ToString(), definitionSeperator, seperator, opening, closing);
        }
        public static string ToPrintable<K,V>(this IDictionary<K,V> a,Func<K,string> kPrinter, Func<V,string> vPrinter, string definitionSeperator = ":", string seperator = ", ", string opening = "{", string closing = "}")
        {
            return a.Select(x => kPrinter(x.Key) + definitionSeperator + vPrinter(x.Value)).ToPrintable(seperator,opening, closing);
        }
        /// <summary>
        /// returns number of members in array that are being searched for
        /// </summary>
        public static int Count<T>(this IEnumerable<T> tosearch, IEnumerable<T> valuestofind)
        {
            return tosearch.Count(valuestofind.Contains);
        }
        public static void Swap<T>(this IList<T> toswap, int index1, int index2)
        {
            if (index1 != index2)
            {
                T temp = toswap[index1];
                toswap[index1] = toswap[index2];
                toswap[index2] = temp;
            }
        }
        public static T[] Sort<T>(this T[] tosort)
        {
            return Sort(tosort, Comparer<T>.Default);
        }
        public static T[] Sort<T>(this T[] tosort, IComparer<T> comparer)
        {
            T[] ret = tosort.Copy();
            Array.Sort(ret, comparer);
            return ret;
        }
        public static T getMedian<T>(this IEnumerable<T> tosearch, IComparer<T> comparer, out int index)
        {
            if (!tosearch.Any())
                throw new ArgumentException("cannot be empty", nameof(tosearch));
            Tuple<T, int>[] arr =
                tosearch.CountBind().ToArray().Sort(new FunctionComparer<Tuple<T, int>>((a, b) => comparer.Compare(a.Item1, b.Item1)));
            var ret = arr[arr.Length / 2];
            index = ret.Item2;
            return ret.Item1;
        }
        public static T getMedian<T>(this IEnumerable<T> tosearch, IComparer<T> comparer)
        {
            int prox;
            return tosearch.getMedian(comparer, out prox);
        }
        public static T getMedian<T>(this IEnumerable<T> tosearch, out int index) where T : IComparable<T>
        {
            return tosearch.getMedian(Comparer<T>.Default, out index);
        }
        public static T getMedian<T>(this IEnumerable<T> tosearch) where T : IComparable<T>
        {
            int prox;
            return tosearch.getMedian(Comparer<T>.Default, out prox);
        }
        public static T getMin<T>(this IEnumerable<T> tosearch, IComparer<T> compare, out int index)
        {
            if (!tosearch.Any())
                throw new ArgumentException("cannot be empty", nameof(tosearch));
            T ret = tosearch.FirstOrDefault();
            index = 0;
            int i = 0;
            foreach (T var in tosearch)
            {
                if (compare.Compare(var, ret) < 0)
                {
                    ret = var;
                    index = i;
                }
                i++;
            }
            return ret;
        }
        public static T getMin<T>(this IEnumerable<T> tosearch, IComparer<T> compare)
        {
            int prox;
            return tosearch.getMin(compare, out prox);
        }
        public static T getMin<T>(this IEnumerable<T> tosearch, out int index)
        {
            return tosearch.getMin(Comparer<T>.Default, out index);
        }
        public static T getMin<T>(this IEnumerable<T> tosearch)
        {
            return tosearch.getMin(Comparer<T>.Default);
        }
        public static T getMax<T>(this IEnumerable<T> tosearch, IComparer<T> compare, out int index)
        {
            return tosearch.getMin(compare.Reverse(), out index);
        }
        public static T getMax<T>(this IEnumerable<T> tosearch, IComparer<T> compare)
        {
            int prox;
            return tosearch.getMax(compare, out prox);
        }
        public static T getMax<T>(this IEnumerable<T> tosearch, out int index)
        {
            return tosearch.getMax(Comparer<T>.Default, out index);
        }
        public static T getMax<T>(this IEnumerable<T> tosearch)
        {
            return tosearch.getMax(Comparer<T>.Default);
        }
        public static T getAverage<T>(this IEnumerable<T> tosearch)
        {
            Field<T> f = Fields.getField<T>();
            return tosearch.getSum().ToFieldWrapper() / tosearch.Count();
        }
        public static T getGeometricAverage<T>(this IEnumerable<T> tosearch)
        {
            var f = Fields.getField<T>();
            return f.pow(tosearch.getProduct(), f.Invert(f.fromInt(tosearch.Count())));
        }
        public static T getMode<T>(this IEnumerable<T> tosearch, out int index)
        {
            if (!tosearch.Any())
                throw new ArgumentException("cannot be empty", nameof(tosearch));
            var oc = tosearch.CountBind().ToOccurances(new EqualityFunctionComparer<Tuple<T, int>>(a => a.Item1));
            KeyValuePair<Tuple<T, int>, ulong> max = oc.getMax(new FunctionComparer<KeyValuePair<Tuple<T, int>, ulong>>(a => a.Value));
            index = max.Key.Item2;
            return max.Key.Item1;
        }
        public static T getMode<T>(this IEnumerable<T> tosearch)
        {
            int prox;
            return tosearch.getMode(out prox);
        }
        public static T getSum<T>(this IEnumerable<T> toAdd)
        {
            Field<T> f = Fields.getField<T>();
            return toAdd.getSum(f.add);
        }
        public static T getSum<T>(this IEnumerable<T> toAdd, Func<T, T, T> adder)
        {
            Field<T> f = Fields.getField<T>();
            return toAdd.Aggregate(f.zero, adder);
        }
        public static T getProduct<T>(this IEnumerable<T> toAdd)
        {
            return getProduct(toAdd, Fields.getField<T>().multiply);
        }
        public static T getProduct<T>(this IEnumerable<T> toAdd, Func<T, T, T> multiplier)
        {
            Field<T> f = Fields.getField<T>();
            return toAdd.Aggregate(f.one, multiplier);
        }
        public static T[] Shuffle<T>(this IList<T> arr)
        {
            return Shuffle(arr, new GlobalRandomGenerator());
        }
        public static T[] Shuffle<T>(this IList<T> arr, RandomGenerator gen)
        {
            T[] x = arr.ToArray(arr.Count);
            for (int i = 0; i < x.Length; i++)
            {
                int j = gen.Int(i, x.Length);
                x.Swap(i, j);
            }
            return x;
        }
        public static bool AllEqual<T>(this IEnumerable<T> @this)
        {
            return AllEqual(@this, EqualityComparer<T>.Default);
        }
        public static bool AllEqual<T>(this IEnumerable<T> @this, IEqualityComparer<T> comp)
        {
            return @this.All(a => comp.Equals(a, @this.First()));
        }
        public static Tuple<T2, T1> FlipTuple<T1, T2>(this Tuple<T1, T2> @this)
        {
            return Tuple.Create(@this.Item2, @this.Item1);
        } 
        public static Tuple<T1> ToTuple<T1>(this object[] @this)
        {
            return new Tuple<T1>((T1)@this[0]);
        }
        public static Tuple<T1, T2> ToTuple<T1, T2>(this object[] @this)
        {
            return new Tuple<T1, T2>((T1)@this[0], (T2)@this[1]);
        }
        public static Tuple<T1, T2, T3> ToTuple<T1, T2, T3>(this object[] @this)
        {
            return new Tuple<T1, T2, T3>((T1)@this[0], (T2)@this[1], (T3)@this[2]);
        }
        public static Tuple<T1, T2, T3, T4> ToTuple<T1, T2, T3, T4>(this object[] @this)
        {
            return new Tuple<T1, T2, T3, T4>((T1)@this[0], (T2)@this[1], (T3)@this[2], (T4)@this[3]);
        }
        public static Tuple<T1, T2, T3, T4, T5> ToTuple<T1, T2, T3, T4, T5>(this object[] @this)
        {
            return new Tuple<T1, T2, T3, T4, T5>((T1)@this[0], (T2)@this[1], (T3)@this[2], (T4)@this[3], (T5)@this[4]);
        }
        public static Tuple<T> ToTuple1<T>(this T[] @this)
        {
            return Tuple.Create(@this[0]);
        }
        public static Tuple<T, T> ToTuple2<T>(this T[] @this)
        {
            return Tuple.Create(@this[0], @this[1]);
        }
        public static Tuple<T, T, T> ToTuple3<T>(this T[] @this)
        {
            return Tuple.Create(@this[0], @this[1], @this[2]);
        }
        public static Tuple<T, T, T, T> ToTuple4<T>(this T[] @this)
        {
            return Tuple.Create(@this[0], @this[1], @this[2], @this[3]);
        }
        public static Tuple<T, T, T, T, T> ToTuple5<T>(this T[] @this)
        {
            return Tuple.Create(@this[0], @this[1], @this[2], @this[3], @this[4]);
        }
        public static Tuple<T1> ToTuple<T1>(this IEnumerable @this)
        {
            return @this.toObjArray().ToTuple<T1>();
        }
        public static Tuple<T1, T2> ToTuple<T1, T2>(this IEnumerable @this)
        {
            return @this.toObjArray().ToTuple<T1, T2>();
        }
        public static Tuple<T1, T2, T3> ToTuple<T1, T2, T3>(this IEnumerable @this)
        {
            return @this.toObjArray().ToTuple<T1, T2, T3>();
        }
        public static Tuple<T1, T2, T3, T4> ToTuple<T1, T2, T3, T4>(this IEnumerable @this)
        {
            return @this.toObjArray().ToTuple<T1, T2, T3, T4>();
        }
        public static Tuple<T1, T2, T3, T4, T5> ToTuple<T1, T2, T3, T4, T5>(this IEnumerable @this)
        {
            return @this.toObjArray().ToTuple<T1, T2, T3, T4, T5>();
        }
        public static object[] toObjArray(this IEnumerable @this)
        {
            return @this.Cast<object>().ToArray();
        }
    }
    public sealed class RotatorArray<T> : IList<T>
    {
        private readonly T[] _items;
        private RollerNum<int> _mIndex;
        public int Index
        {
            get
            {
                return _mIndex.value;
            }
        }
        public int IndexOf(T item)
        {
            return ((IList)_items).IndexOf(item);
        }
        void IList<T>.Insert(int index, T item)
        {
            ((IList)_items).Insert(index, item);
        }
        void IList<T>.RemoveAt(int index)
        {
            ((IList)_items).RemoveAt(index);
        }
        public T this[int i]
        {
            get
            {
                return _items[(_mIndex + i).value];
            }
            set
            {
                _items[(_mIndex + i).value] = value;
            }
        }
        public void Rotate(int val = 1)
        {
            _mIndex += val;
        }
        public static RotatorArray<T> operator ++(RotatorArray<T> a)
        {
            a.Rotate();
            return a;
        }
        public static RotatorArray<T> operator --(RotatorArray<T> a)
        {
            a.Rotate(-1);
            return a;
        }
        public RotatorArray(params T[] switchvalues)
        {
            if (switchvalues?.Length == 0)
                throw new ArgumentException("cannot be empty",nameof(switchvalues));
            this._items = switchvalues.Copy();
            this._mIndex = new RollerNum<int>(0, _items.Length, 0);
        }
        public RotatorArray(int length)
        {
            this._items = new T[length];
            this._mIndex = new RollerNum<int>(0, _items.Length, 0);
        }
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                yield return this[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        void ICollection<T>.Add(T item)
        {
            ((IList)_items).Add(item);
        }
        void ICollection<T>.Clear()
        {
            ((IList)_items).Clear();
        }
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }
        bool ICollection<T>.Remove(T item)
        {
            return ((IList<T>)_items).Remove(item);
        }
        public int Count
        {
            get
            {
                return _items.Count();
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return _items.IsReadOnly;
            }
        }
    }
    public class SparseArray<T>
    {
        private struct Coordinate : IComparable<Coordinate>
        {
            private readonly int[] _cors;
            public Coordinate(params int[] cors)
            {
                this._cors = cors;
            }
            public override int GetHashCode()
            {
                return _cors.GetHashCode() ^ _cors.Length;
            }
            public override bool Equals(object obj)
            {
                Coordinate? c = obj as Coordinate?;
                return c != null && this._cors.SequenceEqual(c.Value._cors);
            }
            public override string ToString()
            {
                return _cors.ToPrintable();
            }
            public int CompareTo(Coordinate other)
            {
                if (other._cors.Length != _cors.Length)
                    return _cors.Length.CompareTo(other._cors.Length);
                int ret = 0;
                var cors = _cors.Zip(other._cors).GetEnumerator();
                while (ret == 0 && cors.MoveNext())
                    ret = cors.Current.Item1.CompareTo(cors.Current.Item2);
                return ret;
            }
        }
        private readonly IDictionary<Coordinate, T> _values;
        private readonly int[] _dim;
        public SparseArray(int dims, T def = default(T))
        {
            if (dims <= 0)
                throw new ArgumentException("must be positive", nameof(dims));
            this._dim = new int[dims];
            this.defaultValue = def;
            this._values = new SortedDictionary<Coordinate, T>();
        }
        public T defaultValue { get; }
        public T this[params int[] query]
        {
            get
            {
                Coordinate co = new Coordinate(query);
                return _values.ContainsKey(co) ? _values[co] : defaultValue;
            }
            set
            {
                Coordinate co = new Coordinate(query);
                if (query.Length != _dim.Length)
                    throw new ArgumentException("incorrect number of arguments for " + _dim.Length + " rank array");
                for (int i = 0; i < query.Length; i++)
                {
                    if (query[i] > _dim[i])
                        _dim[i] = query[i] + 1;
                }
                this._values[co] = value;
            }
        }
        public int GetLength(int i)
        {
            return _dim[i];
        }
    }
    namespace Arr2D
    {
        public static class Array2D
        {
            public static T[,] Fill<T>(int rows, int cols, T tofill = default(T))
            {
                Func<int, int, T> tf = (n, m) => tofill;
                return Fill(rows, cols, tf);
            }
            public static T[,] Fill<T>(int rows, int cols, Func<int, int, T> tofill)
            {
                T[,] ret = new T[rows, cols];
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = tofill(i, j);
                }
                return ret;
            }
            public static IEnumerable<int> getSize(this Array mat)
            {
                return Loops.Range(mat.Rank).Select(mat.GetLength);
            }
            public static T[,] to2DArr<T>(this T[] a, int dim0Length)
            {
                if (a.Length % dim0Length != 0)
                    throw new Exception("array length must divide row length evenly");
                int dim2Length = a.Length / dim0Length;
                T[,] ret = new T[dim0Length, dim2Length];
                for (int i = 0; i < dim0Length; i++)
                {
                    for (int j = 0; j < dim2Length; j++)
                        ret[i, j] = a[i * ret.GetLength(1) + j];
                }
                return ret;
            }
            public static bool isWithinBounds(this Array arr, params int[] ind)
            {
                if (arr.Rank != ind.Length)
                    throw new ArgumentException("mismatch on indices");
                return arr.getSize().Zip(ind).All(a => a.Item2.iswithinPartialExclusive(0, a.Item1));
            }
            public static string ToTablePrintable<T>(this T[,] arr, string openerfirst = "/", string openermid = "|", string openerlast = @"\",
                                                string closerfirst = @"\", string closermid = "|", string closerlast = "/", string divider = " ")
            {
                int s = arr.Cast<T>().Select(a=>a.ToString().Length).Concat(0.Enumerate()).Max();
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    string opener = openermid;
                    if (i == 0)
                        opener = openerfirst;
                    if (i == arr.GetLength(0) - 1)
                        opener = openerlast;
                    ret.Append(opener);
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        if (j > 0)
                            ret.Append(divider);
                        ret.Append(arr[i, j].ToString().PadLeft(s));
                    }
                    string closer = closermid;
                    if (i == 0)
                        closer = closerfirst;
                    if (i == arr.GetLength(0) - 1)
                        closer = closerlast;
                    ret.Append(closer + Environment.NewLine);
                }
                return ret.ToString();
            }
            public static T[,] Concat<T>(int dimen, params T[][,] a)
            {
                switch (dimen)
                {
                    case 0:
                        {
                            if (!a.AllEqual(new EqualityFunctionComparer<T[,]>(x => x.GetLength(1))) || a.Length == 0)
                                throw new ArgumentException("the arrays must be non-empty and of compatible sizes");
                            T[,] ret = new T[a.Sum(x => x.GetLength(0)), a[0].GetLength(1)];
                            int row = 0;
                            foreach (T[,] m in a)
                            {
                                foreach (int i in Loops.Range(m.GetLength(0)))
                                {
                                    foreach (int j in Loops.Range(m.GetLength(1)))
                                        ret[row, j] = m[i, j];
                                    row++;
                                }
                            }
                            return ret;
                        }
                    case 1:
                        {
                            if (!a.AllEqual(new EqualityFunctionComparer<T[,]>(x => x.GetLength(0))) || a.Length == 0)
                                throw new ArgumentException("the arrays must be non-empty and of compatible sizes");
                            T[,] ret = new T[a[0].GetLength(0), a.Sum(x => x.GetLength(1))];
                            int col = 0;
                            foreach (T[,] m in a)
                            {
                                foreach (int i in Loops.Range(m.GetLength(1)))
                                {
                                    foreach (int j in Loops.Range(m.GetLength(0)))
                                        ret[j, col] = m[j, i];
                                    col++;
                                }
                            }
                            return ret;
                        }
                }
                throw new ArgumentException($"{nameof(dimen)} must be either 1 or 0");
            }
            public static T[,] Concat<T>(this T[,] @this, T[,] other, int dimen)
            {
                return Concat(dimen, @this, other);
            }
        }
        public class SymmetricMatrix<T>
        {
            private readonly T[] _data;
            public int Size { get; }
            public bool Reflexive { get; }
            private readonly Func<int, int, int> _func;
            public SymmetricMatrix(int size, bool reflexive = true)
            {
                this.Size = size;
                if (Size < 0)
                    throw new ArgumentException("must be non-negative",nameof(size));
                Reflexive = reflexive;
                if (reflexive)
                {
                    _data = new T[this.Size * (this.Size + 1) / 2];
                    this._func = (r, c) => (int)(r * (this.Size - (r + 1) / 2.0) + c);
                }
                else
                {
                    _data = new T[this.Size * (this.Size - 1) / 2];
                    this._func = (r, c) =>
                    {
                        if (r == c)
                            throw new IndexOutOfRangeException("this matrix is non-reflexive");
                        return (int)(r * (this.Size - (r + 3) / 2.0) + c - 1);
                    };
                }
            }
            private int getindex(int r, int c)
            {
                if (r >= this.Size || c >= this.Size)
                    throw new IndexOutOfRangeException();
                return r > c ? this._func(c, r) : this._func(r, c);
            }
            public T this[int row, int col]
            {
                get
                {
                    if (row >= Size || col >= Size)
                        throw new IndexOutOfRangeException();
                    return _data[this.getindex(row, col)];
                }
                set
                {
                    if (row >= Size || col >= Size)
                        throw new IndexOutOfRangeException();
                    _data[this.getindex(row, col)] = value;
                }
            }
            public int GetLength(int i)
            {
                switch (i)
                {
                    case 1:
                    case 0:
                        return this.Size;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            public T[,] toArr()
            {
                T[,] ret = new T[this.Size, this.Size];
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = ((i == j && !this.Reflexive) ? default(T) : this[i, j]);
                }
                return ret;
            }
        }
    }
}