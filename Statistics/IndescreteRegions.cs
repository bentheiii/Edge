using System;
using System.Collections.Generic;
using System.Linq;
using Edge.Arrays;
using Edge.Fielding;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.Random;
using Edge.Statistics;

namespace Edge
{
    public abstract class IndescreteRegions<T> : IStatistic<T>
    {
        public static IndescreteRegions<T> Create(IEnumerable<T> probabilities, double start = 0, bool checkforduplicates = true)
        {
            double count = probabilities.Count();
            return Create(probabilities.Select(a => Tuple.Create(a, 1/count)), start, checkforduplicates);
        }
        public static IndescreteRegions<T> Create(IEnumerable<Tuple<T, int>> probabilities, double start = 0, bool checkforduplicates = true)
        {
            double sum = probabilities.Sum(a => a.Item2);
            return Create(probabilities.Select(a=>Tuple.Create(a.Item1,(double)a.Item2)), start, checkforduplicates);
        }
        public static IndescreteRegions<T> Create(IEnumerable<Tuple<T, double>> probabilities, double start = 0, bool checkforduplicates = true)
        {
            if (checkforduplicates && probabilities.Select(a=>a.Item1).Duplicates().Any())
                throw new ArgumentException("duplicate values in probabilities");
            if (!probabilities.Any())
                return null;
            if (!probabilities.CountAtLeast(2))
                return new ConstIndescreteRegion<T>(probabilities.First().Item1,start, start+probabilities.First().Item2);
            return new SplitIndescreteRegion<T>(probabilities,start);
        } 
        public abstract double min { get; }
        public abstract double max { get; }
        public abstract T this[double val] { get; }
        public sealed override T RandomMember(RandomGenerator g)
        {
            return this[g.Double(min, max)];
        }
    }
    class ConstIndescreteRegion<T> : IndescreteRegions<T>
    {
        private readonly T _val;
        public ConstIndescreteRegion(T val, double min, double max)
        {
            if (min >= max)
                throw new ArgumentException();
            this._val = val;
            this.min = min;
            this.max = max;
        }
        public override double min { get; }
        public override double max { get; }
        public override T this[double d]
        {
            get
            {
                if (!(d.iswithinPartialExclusive(min,max)))
                    throw new ArgumentOutOfRangeException();
                return this._val;
            }
        }
        public override double CumulitiveDistributionFor(T x)
        {
            return _val.ToFieldWrapper() >= x? 1 : 0;
        }
        public override IEnumerable<T> PossibleOutcomes()
        {
            yield return _val;
        }
        public override double ProbabilityFor(T x)
        {
            return _val.Equals(x) ? 1 : 0;
        }
        public override T Variance(Func<T, T> func)
        {
            return Fields.getField<T>().zero;
        }
        public override T expectedValue(Func<T, T> func)
        {
            return func(_val);
        }
        public override T expectedValue()
        {
            return _val;
        }
    }
    sealed class SplitIndescreteRegion<T> : IndescreteRegions<T>
    {
        public override double min { get; }
        private readonly IndescreteRegions<T>[] _children;
        private readonly double _splitpoint;
        private double _splitratio => (_splitpoint - min) / max;
        public SplitIndescreteRegion(IEnumerable<Tuple<T, double>> probabilities, double min)
        {
            this.min = min;
            this.max = min + probabilities.Sum(a => a.Item2);
            this._splitpoint = min;
            int firstcount = 0;
            var halfpoint = (max + min) / 2;
            var c = probabilities.Count();
            foreach (var probability in probabilities.TakeWhile((tuple, index) => (_splitpoint < halfpoint) && index+1 < c))
            {
                firstcount++;
                _splitpoint += probability.Item2;
            }
            _children = new IndescreteRegions<T>[2];
            _children[0] = Create(probabilities.Take(firstcount),min,false);
            _children[1] = Create(probabilities.Skip(firstcount),_splitpoint,false);
        }
        public override double max { get; }
        public override T this[double val]
        {
            get
            {
                return  _children[val > _splitpoint ? 1 : 0][val];
            }
        }
        public override T expectedValue(Func<T,T> f)
        {
            return _children[0].expectedValue(f).ToFieldWrapper()*_splitratio + _children[1].expectedValue(f).ToFieldWrapper()*(1- _splitratio);
        }
        public override double CumulitiveDistributionFor(T x)
        {
            return _children[0].CumulitiveDistributionFor(x) * _splitratio + _children[1].CumulitiveDistributionFor(x) * (1 - _splitratio);
        }
        public override IEnumerable<T> PossibleOutcomes()
        {
            return _children.SelectMany(a=>a.PossibleOutcomes());
        }
        public override double ProbabilityFor(T x)
        {
            return _children[0].ProbabilityFor(x) * _splitratio + _children[1].ProbabilityFor(x) * (1 - _splitratio);
        }
    }
}
