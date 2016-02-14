using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.RandomGen;
using Edge.Statistics;
using Edge.SystemExtensions;

namespace Edge.Dice
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
        public override GenerationType GenType => GenerationType.None;
        public override IDie<G> abs(IDie<G> x)
        {
            return new AbsDie<G>(x);
        }
        public override IDie<G> add(IDie<G> a, IDie<G> b)
        {
            return a + b;
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
            return a * b;
        }
        public override IDie<G> subtract(IDie<G> a, IDie<G> b)
        {
            return a - b;
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
        public RollAttribute attributes { get; }
    }
    public abstract class IDie<T> : IStatistic<T>
    {
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
            return PossibleOutcomes().Select(a => func(a).ToFieldWrapper() * Fields.getField<T>().fromFraction(ProbabilityFor(a))).getSum();
        }
        public static IDie<T> operator +(IDie<T> a, IDie<T> b)
        {
            return new SumDice<T>(a, b);
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
        public virtual double ProbabilityForMin
        {
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
            var r = Minimum.ToFieldWrapper() + Maximum - (Fields.getField<T>().shape.HasFlag(FieldShape.Discrete) ? 1 : 0);
            return (r) / 2;
        }
        public override T Variance()
        {
            return ((Maximum.ToFieldWrapper() - Minimum + 1).pow(2) - 1) / 12;
        }
        public override T Maximum { get; }
        public override T Minimum { get; }
        public override T RandomMember(RandomGenerator g)
        {
            return g.FromField(Minimum, Maximum);
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
                return (x).iswithin(Minimum, Maximum) ? f.toDouble(f.Invert(Maximum.ToFieldWrapper() - Minimum)) ?? 0 : 0;
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
            return _dice.Select(a => a.expectedValue(func)).getSum();
        }
        public override T expectedValue()
        {
            return _dice.Select(a => a.expectedValue()).getSum();
        }
        public override T Maximum
        {
            get
            {
                return _dice.Select(a => a.Maximum).getSum();
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
            return _dice.Select(a => a.Variance(func)).getSum();
        }
        public override T Variance()
        {
            return _dice.Select(a => a.Variance()).getSum();
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
        public DropDice(IDie<T> i, int factor, int drophighestcount = 0, int droplowestcount = 0)
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
                return this._int.Maximum.ToFieldWrapper() * (_factor - _drophighestcount - _droplowestcount);
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
            return Loops.Range(_factor).Select(a => _int.RandomMember(g)).OrderBy().Skip(_droplowestcount).Take(_factor - _drophighestcount).getSum();
        }
        public override double ProbabilityForMax
        {
            get
            {
                return _int.ProbabilityForMax.pow(_factor - _droplowestcount);
            }
        }
        public override double ProbabilityForMin
        {
            get
            {
                return _int.ProbabilityForMin.pow(_factor - _drophighestcount);
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
            return _int.expectedValue(func) + func(_int.Maximum).ToFieldWrapper() * (_int.ProbabilityForMax / (1 - _int.ProbabilityForMax));
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
            return _dice.Select(a => a.expectedValue(func)).getProduct();
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
            return _dice.SelectToArray(a => a.PossibleOutcomes()).Join().Select(a => a.getProduct()).Distinct();
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
            return _int.PossibleOutcomes().Select(a => a.ToFieldWrapper().abs().val).Distinct();
        }
        public override T Variance(Func<T, T> func)
        {
            return _int.Variance(a => func(a.ToFieldWrapper().abs()));
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
