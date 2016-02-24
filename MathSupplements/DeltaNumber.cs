using System;
using Edge.Fielding;
using Edge.Random;
using Edge.Statistics;

namespace Edge.DeltaNumber
{
    public class DeltaNumber<T> : IStatistic<T>
    {
        private class DeltaNumberField : Field<DeltaNumber<T>>
        {
            private readonly Field<T> _int;
            public DeltaNumberField(Field<T> i)
            {
                this._int = i;
            }
            public override bool Invertible => false;
            public override bool ModduloAble => false;
            public override bool Negatable => false;
            public override DeltaNumber<T> Negate(DeltaNumber<T> x)
            {
                return -x;
            }
            public override bool Parsable => false;
            public override GenerationType GenType => GenerationType.None;
            public override DeltaNumber<T> abs(DeltaNumber<T> x)
            {
                throw new NotSupportedException();
            }
            public override DeltaNumber<T> add(DeltaNumber<T> a, DeltaNumber<T> b)
            {
                return a + b;
            }
            public override DeltaNumber<T> fromFraction(double a)
            {
                return new DeltaNumber<T>(_int.fromFraction(a), _int.zero);
            }
            public override DeltaNumber<T> fromInt(int x)
            {
                return new DeltaNumber<T>(_int.fromInt(x), _int.zero);
            }
            public override DeltaNumber<T> fromInt(ulong x)
            {
                return new DeltaNumber<T>(_int.fromInt(x), _int.zero);
            }
            public override DeltaNumber<T> multiply(DeltaNumber<T> a, DeltaNumber<T> b)
            {
                return a * b;
            }
            public override DeltaNumber<T> subtract(DeltaNumber<T> a, DeltaNumber<T> b)
            {
                return a - b;
            }
            public override DeltaNumber<T> one => new DeltaNumber<T>(_int.one);
            public override DeltaNumber<T> zero => new DeltaNumber<T>(_int.zero);
            public override DeltaNumber<T> naturalbase => new DeltaNumber<T>(_int.naturalbase);
            public override DeltaNumber<T> negativeone => new DeltaNumber<T>(_int.negativeone);
        }
        static DeltaNumber()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            Fields.setField(new DeltaNumberField(Fields.getField<T>()));
        } 
        private readonly FieldWrapper<T> _center;
        private readonly FieldWrapper<T> _delta;
        public DeltaNumber(T center) : this(center, new FieldWrapper<T>(0)) { }
        public DeltaNumber(T center, T delta)
        {
            this._center = center;
            this._delta = delta.ToFieldWrapper().abs();
        }
        public static DeltaNumber<T> fromBounds(T lower, T upper)
        {
            var cent = (lower.ToFieldWrapper() + upper)/2;
            var delt = upper - cent;
            return new DeltaNumber<T>(cent, delt);
        }
        public T delta
        {
            get
            {
                return _delta;
            }
        }
        public T center
        {
            get
            {
                return _center;
            }
        }
        public bool iswithin(T val)
        {
            return (val - _center).abs() <= delta;
        }
        public T lowerBound => _center - delta;
        public T upperbound => _center + delta;

        public static DeltaNumber<T> operator +(DeltaNumber<T> a, DeltaNumber<T> b)
        {
            return new DeltaNumber<T>(a.center+b._center, a.delta+b._delta);
        }
        public static DeltaNumber<T> operator -(DeltaNumber<T> a)
        {
            return new DeltaNumber<T>(-a._center, a.delta);
        }
        public static DeltaNumber<T> operator -(DeltaNumber<T> a, DeltaNumber<T> b)
        {
            return new DeltaNumber<T>(a.center - b._center, a.delta - b._delta);
        }
        public static DeltaNumber<T> operator *(DeltaNumber<T> a, DeltaNumber<T> b)
        {
            var c = (a._center * b._center + a._delta * b._delta);
            var d = (a._center * b._delta + a._delta * b._center);
            return new DeltaNumber<T>(c,d);
        }
        public override T RandomMember(RandomGenerator g)
        {
            return g.FromField(lowerBound,upperbound);
        }
        public override double CumulitiveDistributionFor(T x)
        {
            var f = x.ToFieldWrapper();
            if (f < lowerBound)
                return 0;
            if (f >= upperbound)
                return 1;
            return (double)((f - lowerBound) / (2 * _delta));
        }
        public override T Variance()
        {
            return (_delta)/6;
        }
        public override T expectedValue()
        {
            return center;
        }
    }
}
