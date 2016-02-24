using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;
using Edge.Random;

namespace Edge.Statistics
{
    public abstract class IStatistic<T>{
        public virtual T expectedValue(Func<T, T> func)
        {
            return new DiscreteStatistic<T>((int)2E4, this.RandomMember).expectedValue(func);
        }
        public virtual T Variance(Func<T, T> func)
        {
            var ex = expectedValue(func);
            return this.expectedValue(a => ((a).ToFieldWrapper()) ^ 2) - ex.ToFieldWrapper().pow(2);
        }
        public virtual T Variance()
        {
            return this.Variance(a => a);
        }
        public virtual T expectedValue()
        {
            return this.expectedValue(a => a);
        }
        public virtual IEnumerable<T> PossibleOutcomes()
        {
            return new DiscreteStatistic<T>((int)2E4, this.RandomMember).PossibleOutcomes();
        }
        public virtual double CumulitiveDistributionFor(T x)
        {
            return PossibleOutcomes().Where(a=>a.ToFieldWrapper() <= x).Sum(a=>ProbabilityFor(a));
        }
        public virtual double ProbabilityFor(T x)
        {
            return new DiscreteStatistic<T>((int)2E4,this.RandomMember).ProbabilityFor(x);
        }
        public T RandomMember()
        {
            return RandomMember(new GlobalRandomGenerator());
        }
        public abstract T RandomMember(RandomGenerator g);
    }
    public class DiscreteStatistic<T> : IStatistic<T>, ICollection<T>, IDictionary<T,ulong>
    {
        public DiscreteStatistic(ulong sampleSize, Func<T, double> generator, params T[] keys) : this(a=>(ulong)(generator(a)*sampleSize),keys)
        {}
        public DiscreteStatistic(Func<T,ulong> generator, params T[] keys)
        {
            _occurances = new Dictionary<T, ulong>();
            foreach (T key in keys)
            {
                this[key] = generator(key);
            }
        }
        public DiscreteStatistic(ulong sampleSize, Func<T> generator)
        {
            _occurances = new Dictionary<T, ulong>();
            foreach (int i in Loops.Range(sampleSize))
            {
                Add(generator());
            }
        }
        public DiscreteStatistic(bool generatefieldmembers = true, IEqualityComparer<T> comp = null)
        {
            comp = comp ?? EqualityComparer<T>.Default;
            _occurances = new Dictionary<T, ulong>(comp);
            if (!generatefieldmembers)
                _expectedsum = null;
        } 
        private readonly IDictionary<T, ulong> _occurances;
        [CanBeNull] private FieldWrapper<T> _expectedsum = new FieldWrapper<T>(0);
        public bool TryGetValue(T key, out ulong value)
        {
            return _occurances.TryGetValue(key, out value);
        }
        public ulong this[T x]
        {
            get
            {
                return _occurances[x];
            }
            set
            {
                if (_expectedsum != null)
                    _expectedsum -= x.ToFieldWrapper() * _occurances[x];
                this.SampleSize -= _occurances[x];
                if (value == 0)
                    _occurances.Remove(x);
                else
                {
                    _occurances[x] = value;
                    this.SampleSize += _occurances[x];
                    if (_expectedsum != null)
                        _expectedsum += x.ToFieldWrapper() * _occurances[x];
                }
            }
        }
        public ICollection<T> Keys
        {
            get
            {
                return _occurances.Keys;
            }
        }
        public ICollection<ulong> Values
        {
            get
            {
                return _occurances.Values;
            }
        }
        public bool ContainsKey(T key)
        {
            return _occurances.ContainsKey(key);
        }
        public void Add(T x, ulong val)
        {
            _occurances.AggregateDefinition(x, val, (i, i1) => i + i1);
            if (_expectedsum != null)
                _expectedsum += x.ToFieldWrapper() * val;
            this.SampleSize += val;
        }
        public bool Remove(T item, ulong value)
        {
            if (this[item] < value)
                return false;
            if (this[item] == value)
            {
                if (_expectedsum != null)
                    _expectedsum -= item.ToFieldWrapper() * _occurances[item];
                this.SampleSize -= _occurances[item];
                _occurances.Remove(item);
            }
            else
            {
                if (_expectedsum != null)
                    _expectedsum += item.ToFieldWrapper() * value;
                this.SampleSize -= value;
                _occurances[item] -= value;
            }
            return true;
        }
        public override T expectedValue(Func<T,T> func)
        {
            Field<T> field = Fields.getField<T>();
            T ret = field.zero;
            foreach (KeyValuePair<T, ulong> keyValuePair in _occurances)
            {
                ret = field.add(ret, field.multiply(func(keyValuePair.Key), field.fromInt(keyValuePair.Value)));
            }
            ret = field.divide(ret, field.fromInt(SampleSize));
            return ret;
        }
        public override T expectedValue()
        {
            if (_expectedsum != null)
                return _expectedsum /SampleSize;
            return base.expectedValue();
        }
        public ulong SampleSize { get; private set; } = 0;
        public override double ProbabilityFor(T x)
        {
            return !this.ContainsKey(x) ? 0 : this[x] / (double)SampleSize;
        }
        public override T RandomMember(RandomGenerator g)
        {
            if (SampleSize == 0)
                throw new Exception("statistic is empty");
            ulong val = g.ULong(SampleSize);
            foreach (KeyValuePair<T, ulong> keyValuePair in _occurances)
            {
                if (keyValuePair.Value > val)
                    return keyValuePair.Key;
                val -= keyValuePair.Value;
            }
            throw new Exception("this can't happen");
        }
        IEnumerator<KeyValuePair<T, ulong>> IEnumerable<KeyValuePair<T, ulong>>.GetEnumerator()
        {
            return _occurances.GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _occurances.Keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public void Add(T item)
        {
            Add(item,1);
        }
        public void Add(KeyValuePair<T, ulong> item)
        {
            _occurances.Add(item);
        }
        public void Clear()
        {
            _occurances.Clear();
            if (_expectedsum != null)
                _expectedsum = new FieldWrapper<T>(0);
            SampleSize = 0;
        }
        public bool Contains(KeyValuePair<T, ulong> item)
        {
            return _occurances.Contains(item);
        }
        public void CopyTo(KeyValuePair<T, ulong>[] array, int arrayIndex)
        {
            _occurances.CopyTo(array, arrayIndex);
        }
        public bool Remove(KeyValuePair<T, ulong> item)
        {
            return _occurances.Remove(item);
        }
        public int Count
        {
            get
            {
                return _occurances.Count;
            }
        }
        public bool Contains(T item)
        {
            return _occurances.Keys.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            _occurances.Keys.CopyTo(array,arrayIndex);
        }
        public bool Remove(T item)
        {
            return Remove(item,1);
        }
        int ICollection<T>.Count
        {
            get
            {
                return _occurances.Keys.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public void resize(ulong newSampleSize)
        {
            double factor = SampleSize / (double)newSampleSize;
            foreach (T key in _occurances.Keys.ToArray())
            {
                this[key] = (ulong)(this[key] / factor);
            }
            if (_expectedsum != null)
                _expectedsum = this.expectedValue(a => a);
            SampleSize = _occurances.Values.getSum();
        }
        public override IEnumerable<T> PossibleOutcomes()
        {
            return Keys;
        }
        public double Entropy()
        {
            return _occurances.Select(a => a.Value / (double)SampleSize).Select(a => -a * Math.Log(a, 2)).Sum();
        }
    }
    public class SmartDiscreteStatistic<T> : IStatistic<T>, ICollection<T>, IDictionary<T,ulong>
    {
        private readonly DiscreteStatistic<T> _int;
        public SmartDiscreteStatistic() : this(new T[] { }) { }
        public SmartDiscreteStatistic(params T[] vals) : this(false, vals) { }
        public SmartDiscreteStatistic(bool generatefieldmembers, params T[] vals)
        {
            _int = new DiscreteStatistic<T>(generatefieldmembers);
            foreach (T val in vals)
            {
                _int.Add(val);
            }
        }
        public T RandomMember(RandomGenerator g, ulong? resetval)
        {
            T ret = _int.RandomMember(g);
            if (resetval.HasValue)
            {
                foreach (T t in _int.Keys)
                {
                    _int[t]++;
                }
                _int[ret] = resetval.Value;
            }
            return ret;
        }
        public override T RandomMember(RandomGenerator g)
        {
            return RandomMember(g, 1);
        }
        public override double ProbabilityFor(T x)
        {
            return _int.ProbabilityFor(x);
        }
        public override T expectedValue(Func<T, T> func)
        {
            return _int.expectedValue(func);
        }
        public override T expectedValue()
        {
            return _int.expectedValue();
        }
        public override T Variance()
        {
            return _int.Variance();
        }
        public override T Variance(Func<T,T> func)
        {
            return _int.Variance(func);
        }
        IEnumerator<KeyValuePair<T, ulong>> IEnumerable<KeyValuePair<T, ulong>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T, ulong>>)_int).GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _int.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_int).GetEnumerator();
        }
        public void Add(T item)
        {
            _int.Add(item);
        }
        public void Add(KeyValuePair<T, ulong> item)
        {
            _int.Add(item);
        }
        public void Clear()
        {
            _int.Clear();
        }
        public bool Contains(KeyValuePair<T, ulong> item)
        {
            return _int.Contains(item);
        }
        public void CopyTo(KeyValuePair<T, ulong>[] array, int arrayIndex)
        {
            _int.CopyTo(array, arrayIndex);
        }
        public bool Remove(KeyValuePair<T, ulong> item)
        {
            return _int.Remove(item);
        }
        public bool Contains(T item)
        {
            return _int.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            _int.CopyTo(array, arrayIndex);
        }
        public bool ContainsKey(T key)
        {
            return _int.ContainsKey(key);
        }
        public void Add(T key, ulong value)
        {
            _int.Add(key, value);
        }
        public bool Remove(T item)
        {
            return _int.Remove(item);
        }
        public bool TryGetValue(T key, out ulong value)
        {
            return _int.TryGetValue(key, out value);
        }
        public ulong this[T key]
        {
            get
            {
                return _int[key];
            }
            set
            {
                _int[key] = value;
            }
        }
        public ICollection<T> Keys
        {
            get
            {
                return _int.Keys;
            }
        }
        public ICollection<ulong> Values
        {
            get
            {
                return _int.Values;
            }
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
        public override IEnumerable<T> PossibleOutcomes()
        {
            return _int.PossibleOutcomes();
        }
    }
    
}
