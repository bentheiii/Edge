using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.RandomGen;
using Edge.SystemExtensions;

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
            foreach (T key in keys)
            {
                this[key] = generator(key);
            }
        }
        public DiscreteStatistic(ulong sampleSize, Func<T> generator)
        {
            foreach (int i in Loops.Range(sampleSize))
            {
                Add(generator());
            }
        }
        public DiscreteStatistic(bool generatefieldmembers = true)
        {
            if (!generatefieldmembers)
                _expected = null;
        } 
        private readonly IDictionary<T, ulong> _occurances = new Dictionary<T, ulong>();
        [CanBeNull] private FieldWrapper<T> _expected = new FieldWrapper<T>(0);
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
                if (_expected != null)
                    _expected -= x.ToFieldWrapper() * _occurances[x];
                this.SampleSize -= _occurances[x];
                if (value == 0)
                    _occurances.Remove(x);
                else
                {
                    _occurances[x] = value;
                    this.SampleSize += _occurances[x];
                    if (_expected != null)
                        _expected += x.ToFieldWrapper() * _occurances[x];
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
            if (_expected != null)
                _expected += x.ToFieldWrapper() * val;
            this.SampleSize += val;
        }
        public bool Remove(T item, ulong value)
        {
            if (this[item] < value)
                return false;
            if (this[item] == value)
            {
                if (_expected != null)
                    _expected -= item.ToFieldWrapper() * _occurances[item];
                this.SampleSize -= _occurances[item];
                _occurances.Remove(item);
            }
            else
            {
                if (_expected != null)
                    _expected += item.ToFieldWrapper() * value;
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
            if (_expected != null)
                return _expected /SampleSize;
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
            if (_expected != null)
                _expected = new FieldWrapper<T>(0);
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
            if (_expected != null)
                _expected = this.expectedValue(a => a);
            SampleSize = _occurances.Values.getSum();
        }
        public override IEnumerable<T> PossibleOutcomes()
        {
            return Keys;
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
    namespace Dice
    {
        public enum RollAttribute
        {
            Regular = 0,
            Maximum = 2,
            Minimum = 1,
            NoneError = -1
        };
        public class DieRoll
        {
            public DieRoll(int value, RollAttribute attributes)
            {
                this.attributes = attributes;
                this.value = value;
            }
            public int value { get; }
            public RollAttribute attributes {get;}
        }
        public abstract class IDie : IStatistic<double> {
            public abstract int Maximum { get; }
            public abstract int Minimum { get; }
            public DieRoll RandomRoll()
            {
                return RandomRoll(new GlobalRandomGenerator());
            }
            public DieRoll RandomRoll(RandomGenerator g)
            {
                var res = this.RandomInt(g);
                var att = RollAttribute.Regular;
                if (Maximum == res && res > Minimum)
                    att = RollAttribute.Maximum;
                if (Minimum == res && res < Maximum)
                    att = RollAttribute.Minimum;
                return new DieRoll(res, att);
            }
            public override double expectedValue(Func<double, double> func)
            {
                return Loops.IRange(Minimum, Maximum).Sum(a => func(a) * ProbabilityFor(a));
            }
            public static IDie operator +(IDie a, IDie b)
            {
                return new DiceCollection(a,b);
            }
            public static IDie operator +(IDie a, int b)
            {
                return new DiceCollection(a, new ConstDie(b));
            }
            public abstract double ProbabilityForMax { get; }
            public abstract double ProbabilityForMin { get; }
            public virtual double ProbabilityFor(int x)
            {
                return base.ProbabilityFor(x);
            }
            public int RandomInt()
            {
                return RandomInt(new GlobalRandomGenerator());
            }
            public abstract int RandomInt(RandomGenerator g);
            public override sealed double ProbabilityFor(double x)
            {
                return x.whole() && x.iswithin(Minimum, Maximum) ? this.ProbabilityFor((int) x) : 0;
            }
            public override sealed double RandomMember(RandomGenerator g)
            {
                return this.RandomInt(g);
            }
            public virtual double CumulitiveDistributionFor(int x)
            {
                return base.CumulitiveDistributionFor(x);
            }
            public override sealed double CumulitiveDistributionFor(double x)
            {
                return CumulitiveDistributionFor((int)x);
            }
            
        }
        public static class Dice
        {
            public static readonly IDie Coin = new Die(1, 0), D2 = Coin + 1, D3 = new Die(3), D4 = new Die(4),
                                        D6 = new Die(6), D8 = new Die(8), D10 = new Die(10), D12 = new Die(12),
                                        D20 = new Die(20), D100 = new Die(100);
        }
        public class ConstDie : IDie
        {
            public ConstDie(int val)
            {
                this.Val = val;
            }
            public int Val { get; }
            public override double expectedValue(Func<double, double> func)
            {
                return func(Val);
            }
            public override double Variance(Func<double, double> func) => 0;
            public override int Maximum
            {
                get
                {
                    return Val;
                }
            }
            public override int Minimum
            {
                get
                {
                    return Val;
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return Val;
            }
            public override double ProbabilityForMax { get; } = 1;
            public override double ProbabilityForMin { get; } = 1;
            public override double ProbabilityFor(int x)
            {
                return x == this.Maximum ? 1 : 0;
            }
            public override IEnumerable<double> PossibleOutcomes()
            {
                return Val.Enumerate().Cast<double>();
            }
        }
        public class Die : IDie
        {
            public Die(int maximum, int minimum = 1)
            {
                this.Maximum = maximum;
                this.Minimum = minimum;
            }
            public override double expectedValue()
            {
                return ((double)Minimum + Maximum) / 2;
            }
            public override double Variance()
            {
                return ((Maximum-Minimum+1).pow(2) -1.0)/12;
            }
            public override int Maximum { get; }
            public override int Minimum { get; }
            public override int RandomInt(RandomGenerator g)
            {
                return g.Int(Minimum, Maximum, true);
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return ProbabilityFor(Maximum);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return ProbabilityFor(Minimum);
                }
            }
            public override double ProbabilityFor(int x)
            {
                return x.iswithin(Minimum, Maximum) ? 1.0 / (Maximum - Minimum + 1.0) : 0;
            }
            public override double CumulitiveDistributionFor(int x)
            {
                return x < Minimum ? 0 : (x >= Maximum ? 1 : (x - Minimum + 1.0) / (Maximum - Minimum + 1.0));
            }
            public override IEnumerable<double> PossibleOutcomes()
            {
                return Loops.Range(Minimum, Maximum).Cast<double>();
            }
        }
        public class DiceCollection : IDie
        {
            private readonly IEnumerable<IDie> _dice;
            public DiceCollection(params IDie[] dice)
            {
                this._dice = dice.ToArray();
            }
            public override double expectedValue(Func<double, double> func)
            {
                return _dice.Sum(a=>a.expectedValue(func));
            }
            public override double expectedValue()
            {
                return _dice.Select(a=>a.expectedValue()).Sum();
            }
            public override int Maximum
            {
                get
                {
                   return _dice.Sum(a=>a.Maximum);
                }
            }
            public override int Minimum
            {
                get
                {
                    return _dice.Sum(a => a.Minimum);
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return _dice.Sum(a => a.RandomInt(g));
            }
            public override double Variance(Func<double, double> func)
            {
                return _dice.Sum(a=>a.Variance(func));
            }
            public override double Variance()
            {
                return _dice.Sum(a => a.Variance());
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _dice.Select(a => a.ProbabilityForMax).getProduct((a,b) => a*b);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _dice.Select(a => a.ProbabilityForMin).getProduct((a, b) => a * b);
                }
            }
        }
        public class FactorDie : IDie
        {
            private readonly IDie _int;
            private readonly int _factor;
            public FactorDie(IDie i, int factor)
            {
                this._int = i;
                this._factor = factor;
            }
            public override double expectedValue(Func<double, double> func)
            {
                return this._int.expectedValue(func) * _factor;
            }
            public override double expectedValue()
            {
                return this._int.expectedValue() * _factor;
            }
            public override int Maximum
            {
                get
                {
                    return this._int.Maximum * _factor;
                }
            }
            public override int Minimum
            {
                get
                {
                    return this._int.Minimum * _factor;
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return _int.RandomInt(g)*_factor;
            }
            public override double Variance(Func<double, double> func)
            {
                return _int.Variance(func) * _factor * _factor;
            }
            public override double Variance()
            {
                return _int.Variance() * _factor * _factor;
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _int.ProbabilityForMax;
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin;
                }
            }
            public override double ProbabilityFor(int x)
            {
                return x % _factor != 0 ? 0 : _int.ProbabilityFor(x / _factor);
            }
            public override IEnumerable<double> PossibleOutcomes()
            {
                return _int.PossibleOutcomes().Select(a=>a*_factor);
            }
            public override double CumulitiveDistributionFor(int x)
            {
                return _int.CumulitiveDistributionFor(x / _factor);
            }
        }
        public class MultiDie : IDie
        {
            protected readonly IDie _int;
            protected readonly int _factor;
            public MultiDie(IDie i, int factor)
            {
                this._int = i;
                this._factor = factor;
            }
            public override double expectedValue(Func<double, double> func)
            {
                return this._int.expectedValue(func) * _factor;
            }
            public override double expectedValue()
            {
                return this._int.expectedValue() * _factor;
            }
            public override int Maximum
            {
                get
                {
                    return this._int.Maximum * _factor;
                }
            }
            public override int Minimum
            {
                get
                {
                    return this._int.Minimum*_factor;
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return Loops.Range(_factor).Sum(a=>_int.RandomInt(g));
            }
            public override double Variance(Func<double, double> func)
            {
                return _int.Variance(func) * _factor * _factor;
            }
            public override double Variance()
            {
                return _int.Variance() * _factor * _factor;
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _int.ProbabilityForMax.pow(_factor);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin.pow(_factor);
                }
            }
        }
        public class DropLowestDice : IDie
        {
            protected readonly IDie _int;
            protected readonly int _factor;
            private readonly int _droplowest;
            public DropLowestDice(int droplowest, int factor, IDie i)
            {
                this._droplowest = droplowest;
                this._factor = factor;
                this._int = i;
            }
            public override int Maximum
            {
                get
                {
                    return this._int.Maximum * (_factor-_droplowest);
                }
            }
            public override int Minimum
            {
                get
                {
                    return this._int.Minimum * (_factor - _droplowest);
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return Loops.Range(_factor).SelectToArray(a=>_int.RandomInt(g)).Sort().Skip(_droplowest).Sum();
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _int.ProbabilityForMax.pow(_factor-_droplowest);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin.pow(_factor);
                }
            }
        }
        public class DropHighestDice : IDie
        {
            protected readonly IDie _int;
            protected readonly int _factor;
            private readonly int _drophighest;
            public DropHighestDice(int droplowest, int factor, IDie i)
            {
                this._drophighest = droplowest;
                this._factor = factor;
                this._int = i;
            }
            public override int Maximum
            {
                get
                {
                    return this._int.Maximum * (_factor - _drophighest);
                }
            }
            public override int Minimum
            {
                get
                {
                    return this._int.Minimum * (_factor - _drophighest);
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                return Loops.Range(_factor).SelectToArray(a => _int.RandomInt(g)).Sort().Take(_factor- _drophighest).Sum();
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin.pow(_factor-_drophighest);
                }
            }
            public override double expectedValue(Func<double, double> func)
            {
                return Loops.IRange(Minimum, Maximum).Sum(a => func(a) / ProbabilityFor(a));
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _int.ProbabilityForMax.pow(_factor);
                }
            }
        }
        public class ExplodingDice : IDie
        {
            public ExplodingDice(IDie i)
            {
                this._int = i;
            }
            private IDie _int { get; }
            public override double expectedValue()
            {
                return _int.expectedValue() + _int.Maximum * (_int.ProbabilityForMax / (1 - _int.ProbabilityForMax));
            }
            public override double expectedValue(Func<double, double> func)
            {
                return _int.expectedValue(func) + func(_int.Maximum) * (_int.ProbabilityForMax/(1-_int.ProbabilityForMax));
            }
            public override int Maximum
            {
                get
                {
                    throw new NotSupportedException("exploding dice have no maximum");
                }
            }
            public override int Minimum
            {
                get
                {
                    return _int.Minimum;
                }
            }
            public override int RandomInt(RandomGenerator g)
            {
                int ret = 0;
                while (true)
                {
                    var roll = _int.RandomRoll(g);
                    ret += roll.value;
                    if (roll.attributes != RollAttribute.Maximum)
                        return ret;
                }
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return 0;
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin;
                }
            }
        }
    }
}
