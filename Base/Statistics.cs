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
        public class DieField<G> : Field<IDie<G>>
        {
            private readonly Field<G> _int;
            public DieField(Field<G> i)
            {
                this._int = i;
            }
            public override bool Invertible => false;
            public override bool ModduloAble => false;
            public override bool Negatable => false;
            public override IDie<G> Negate(IDie<G> x)
            {
                return -x;
            }
            public override bool Parsable => false;
            public override RandomGenType RandGen => RandomGenType.None;
            public override IDie<G> abs(IDie<G> x)
            {
                return new AbsDie<G>(x);
            }
            public override IDie<G> add(IDie<G> a, IDie<G> b)
            {
                return a+b;
            }
            public override IDie<G> fromFraction(double a)
            {
                return new ConstDie<G>(_int.fromFraction(a));
            }
            public override IDie<G> fromInt(int x)
            {
                return new ConstDie<G>(_int.fromInt(x));
            }
            public override IDie<G> fromInt(ulong x)
            {
                return new ConstDie<G>(_int.fromInt(x));
            }
            public override IDie<G> multiply(IDie<G> a, IDie<G> b)
            {
                return a*b;
            }
            public override IDie<G> subtract(IDie<G> a, IDie<G> b)
            {
                return a-b;
            }
            public override IDie<G> one => new ConstDie<G>(_int.one);
            public override IDie<G> zero => new ConstDie<G>(_int.zero);
            public override IDie<G> naturalbase => new ConstDie<G>(_int.naturalbase);
            public override IDie<G> negativeone => new ConstDie<G>(_int.negativeone);
        }
        public enum RollAttribute
        {
            Regular = 0,
            Maximum = 2,
            Minimum = 1,
            NoneError = -1
        };
        public class DieRoll<T>
        {
            public DieRoll(T value, RollAttribute attributes)
            {
                this.attributes = attributes;
                this.value = value;
            }
            public T value { get; }
            public RollAttribute attributes {get;}
        }
        public abstract class IDie<T> : IStatistic<T> {
            public abstract T Maximum { get; }
            public abstract T Minimum { get; }
            public DieRoll<T> RandomRoll()
            {
                return RandomRoll(new GlobalRandomGenerator());
            }
            public DieRoll<T> RandomRoll(RandomGenerator g)
            {
                var res = this.RandomMember(g);
                var att = RollAttribute.Regular;
                if (Maximum.Equals(res) && res.ToFieldWrapper() > Minimum)
                    att = RollAttribute.Maximum;
                if (Minimum.Equals(res) && res.ToFieldWrapper() < Maximum)
                    att = RollAttribute.Minimum;
                return new DieRoll<T>(res, att);
            }
            public override T expectedValue(Func<T, T> func)
            {
                return PossibleOutcomes().Select(a => func(a).ToFieldWrapper()*Fields.getField<T>().fromFraction(ProbabilityFor(a))).getSum();
            }
            public static IDie<T> operator +(IDie<T> a, IDie<T> b)
            {
                return new SumDice<T>(a,b);
            }
            public static IDie<T> operator +(IDie<T> a, T b)
            {
                return new SumDice<T>(a, new ConstDie<T>(b));
            }
            public static IDie<T> operator -(IDie<T> a, IDie<T> b)
            {
                return new SumDice<T>(a, -b);
            }
            public static IDie<T> operator -(IDie<T> a, T b)
            {
                return new SumDice<T>(a, new ConstDie<T>(-b.ToFieldWrapper()));
            }
            public static IDie<T> operator *(IDie<T> a, IDie<T> b)
            {
                return new ProdDice<T>(a, b);
            }
            public static IDie<T> operator *(IDie<T> a, T b)
            {
                return new ProdDice<T>(a, new ConstDie<T>(b));
            }
            public static IDie<T> operator -(IDie<T> a)
            {
                return a * Fields.getField<T>().negativeone;
            }
            public virtual double ProbabilityForMax
            {
                get
                {
                    return base.ProbabilityFor(this.Maximum);
                }
            }
            public virtual double ProbabilityForMin {
                get
                {
                    return base.ProbabilityFor(this.Minimum);
                }
            }
        }
        public class ConstDie<T> : IDie<T>
        {
            public ConstDie(T val)
            {
                this.Val = val;
            }
            public T Val { get; }
            public override T expectedValue(Func<T, T> func)
            {
                return func(Val);
            }
            public override T Variance(Func<T, T> func) => Fields.getField<T>().zero;
            public override T Maximum
            {
                get
                {
                    return Val;
                }
            }
            public override T Minimum
            {
                get
                {
                    return Val;
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                return Val;
            }
            public override double ProbabilityForMax { get; } = 1;
            public override double ProbabilityForMin { get; } = 1;
            public override double ProbabilityFor(T x)
            {
                return x.Equals(Val).Indicator();
            }
            public override IEnumerable<T> PossibleOutcomes()
            {
                return Val.Enumerate();
            }
        }
        public class Die<T> : IDie<T>
        {
            public Die(T minimum, T maximum)
            {
                this.Maximum = maximum;
                this.Minimum = minimum;
            }
            public override T expectedValue()
            {
                return (Minimum.ToFieldWrapper() + Maximum) / 2;
            }
            public override T Variance()
            {
                return ((Maximum.ToFieldWrapper()-Minimum+1).pow(2) - 1)/12;
            }
            public override T Maximum { get; }
            public override T Minimum { get; }
            public override T RandomMember(RandomGenerator g)
            {
                return g.FromField(Minimum,Maximum);
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
            public override double ProbabilityFor(T x)
            {
                var f = Fields.getField<T>();
                if (f.shape.HasFlag(FieldShape.Discrete))
                    return (x).iswithin(Minimum, Maximum) ? f.toDouble(f.Invert(Maximum.ToFieldWrapper() - Minimum))??0 : 0;
                return 0;
            }
            public override double CumulitiveDistributionFor(T v)
            {
                var x = v.ToFieldWrapper();
                return x < Minimum ? 0 : (x >= Maximum ? 1 : (double)((x - Minimum) / (Maximum.ToFieldWrapper() - Minimum)));
            }
            public override IEnumerable<T> PossibleOutcomes()
            {
                var f = Fields.getField<T>();
                if (f.shape.HasFlag(FieldShape.Discrete))
                    return Loops.Range(Minimum, Maximum);
                return base.PossibleOutcomes();
            }
        }
        public class SumDice<T> : IDie<T>
        {
            private readonly IEnumerable<IDie<T>> _dice;
            public SumDice(params IDie<T>[] dice)
            {
                this._dice = dice.ToArray();
            }
            public override T expectedValue(Func<T, T> func)
            {
                return _dice.Select(a=>a.expectedValue(func)).getSum();
            }
            public override T expectedValue()
            {
                return _dice.Select(a=>a.expectedValue()).getSum();
            }
            public override T Maximum
            {
                get
                {
                   return _dice.Select(a=>a.Maximum).getSum();
                }
            }
            public override T Minimum
            {
                get
                {
                    return _dice.Select(a => a.Minimum).getSum();
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                return _dice.Select(a => a.RandomMember(g)).getSum();
            }
            public override T Variance(Func<T, T> func)
            {
                return _dice.Select(a=>a.Variance(func)).getSum();
            }
            public override T Variance()
            {
                return _dice.Select(a => a.Variance()).getSum();
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
            public override IEnumerable<T> PossibleOutcomes()
            {
                return _dice.SelectToArray(a => a.PossibleOutcomes()).Join().Select(a => a.getSum()).Distinct();
            }
        }
        public class DropDice<T> : IDie<T>
        {
            protected readonly IDie<T> _int;
            protected readonly int _factor;
            private readonly int _drophighestcount;
            private readonly int _droplowestcount;
            public DropDice(IDie<T> i, int factor, int drophighestcount=0, int droplowestcount=0)
            {
                if (factor < (drophighestcount + droplowestcount) && (factor > 0 && droplowestcount > 0 && drophighestcount > 0))
                    throw new ArgumentException("factor must be larger than drophighest and droplowest combined ,and they all have to be positive");
                _int = i;
                _factor = factor;
                _drophighestcount = drophighestcount;
                _droplowestcount = droplowestcount;
            }
            public override T Maximum
            {
                get
                {
                    return this._int.Maximum.ToFieldWrapper() * (_factor-_drophighestcount - _droplowestcount);
                }
            }
            public override T Minimum
            {
                get
                {
                    return this._int.Minimum.ToFieldWrapper() * (_factor - _drophighestcount - _droplowestcount);
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                return Loops.Range(_factor).Select(a=>_int.RandomMember(g)).OrderBy().Skip(_droplowestcount).Take(_factor-_drophighestcount).getSum();
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _int.ProbabilityForMax.pow(_factor-_droplowestcount);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _int.ProbabilityForMin.pow(_factor-_drophighestcount);
                }
            }
        }
        public class ExplodingDice<T> : IDie<T>
        {
            public ExplodingDice(IDie<T> i)
            {
                this._int = i;
            }
            private IDie<T> _int { get; }
            public override T expectedValue()
            {
                return _int.expectedValue() + _int.Maximum.ToFieldWrapper() * (_int.ProbabilityForMax / (1 - _int.ProbabilityForMax));
            }
            public override T expectedValue(Func<T, T> func)
            {
                return _int.expectedValue(func) + func(_int.Maximum).ToFieldWrapper() * (_int.ProbabilityForMax/(1-_int.ProbabilityForMax));
            }
            public override T Maximum
            {
                get
                {
                    throw new NotSupportedException("exploding dice have no maximum");
                }
            }
            public override T Minimum
            {
                get
                {
                    return _int.Minimum;
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                var ret = Fields.getField<T>().zero.ToFieldWrapper();
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
        public class ProdDice<T> : IDie<T>
        {
            private readonly IEnumerable<IDie<T>> _dice;
            public ProdDice(params IDie<T>[] dice)
            {
                _dice = dice;
            }
            public override T expectedValue(Func<T, T> func)
            {
                return _dice.Select(a=>a.expectedValue(func)).getProduct();
            }
            public override T expectedValue()
            {
                return _dice.Select(a => a.expectedValue()).getProduct();
            }
            public override T Maximum
            {
                get
                {
                    return _dice.Select(a => a.Maximum).getProduct();
                }
            }
            public override T Minimum
            {
                get
                {
                    return _dice.Select(a => a.Minimum).getProduct();
                }
            }
            public override double ProbabilityForMax
            {
                get
                {
                    return _dice.Select(a => a.ProbabilityForMax).getProduct((a, b) => a * b);
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    return _dice.Select(a => a.ProbabilityForMin).getProduct((a, b) => a * b);
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                return _dice.Select(a => a.RandomMember(g)).getProduct();
            }
            public override IEnumerable<T> PossibleOutcomes()
            {
                return _dice.SelectToArray(a=>a.PossibleOutcomes()).Join().Select(a=>a.getProduct()).Distinct();
            }
            public override T Variance(Func<T, T> func)
            {
                var esq = _dice.Select(a => a.expectedValue(func).ToFieldWrapper().pow(2)).Cache();
                return _dice.Zip(esq).Select(a => a.Item1.Variance(func) + a.Item2).getProduct((a, b) => a * b) - esq.getProduct((a, b) => a * b);
            }
            public override T Variance()
            {
                var esq = _dice.Select(a => a.expectedValue().ToFieldWrapper().pow(2)).Cache();
                return _dice.Zip(esq).Select(a => a.Item1.Variance() + a.Item2).getProduct((a, b) => a * b) - esq.getProduct((a, b) => a * b);
            }
        }
        public class AbsDie<T> : IDie<T>
        {
            private readonly IDie<T> _int;
            private readonly sbyte _minormaxhigher;
            public AbsDie(IDie<T> i)
            {
                _int = i;
                _minormaxhigher = (i.Minimum.ToFieldWrapper().abs() == i.Maximum.ToFieldWrapper().abs()) ? (sbyte)0 : (i.Maximum.ToFieldWrapper().abs() > i.Minimum.ToFieldWrapper().abs() ? (sbyte)1 : (sbyte)-1);
            }
            public override T Maximum
            {
                get
                {
                    return _minormaxhigher >= 0 ? _int.Maximum : _int.Minimum;
                }
            }
            public override T Minimum
            {
                get
                {
                    return Fields.getField<T>().zero.iswithin(_int.Minimum, _int.Maximum) ? Fields.getField<T>().zero : (_minormaxhigher < 0 ? _int.Maximum : _int.Minimum);
                }
            }
            public override double ProbabilityForMax
            {
                get
                {
                    if (_minormaxhigher == 0)
                        return _int.ProbabilityForMax + _int.ProbabilityForMin;
                    return _minormaxhigher > 0 ? _int.ProbabilityForMax : _int.ProbabilityForMin;
                }
            }
            public override double ProbabilityForMin
            {
                get
                {
                    if (Fields.getField<T>().zero.iswithin(_int.Minimum, _int.Maximum))
                        return _int.ProbabilityFor(Fields.getField<T>().zero);
                    return _minormaxhigher < 0 ? _int.ProbabilityForMax : _int.ProbabilityForMin;
                }
            }
            public override T RandomMember(RandomGenerator g)
            {
                return _int.RandomMember(g).ToFieldWrapper().abs();
            }
            public override double CumulitiveDistributionFor(T x)
            {
                return _int.CumulitiveDistributionFor(x) - _int.CumulitiveDistributionFor(-x.ToFieldWrapper()) + _int.ProbabilityFor(x);
            }
            public override IEnumerable<T> PossibleOutcomes()
            {
                return _int.PossibleOutcomes().Select(a=>a.ToFieldWrapper().abs().val).Distinct();
            }
            public override T Variance(Func<T, T> func)
            {
                return _int.Variance(a=>func(a.ToFieldWrapper().abs()));
            }
            public override T expectedValue(Func<T, T> func)
            {
                return _int.expectedValue(a => func(a.ToFieldWrapper().abs()));
            }
            public override double ProbabilityFor(T x)
            {
                return _int.ProbabilityFor(x) + _int.ProbabilityFor(-x.ToFieldWrapper());
            }
        }
    }
}
