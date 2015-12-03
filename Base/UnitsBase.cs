using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using CCDefault.Annotations;
using Edge.Credentials;
using Edge.Data;
using Edge.Funnels;
using Edge.Net;
using Edge.NumbersMagic;
using Edge.PermanentObject;
using Edge.SystemExtensions;
using Edge.Units.Distance;
using Edge.WordsPlay.Parsing;
using static Edge.WordsPlay.WordPlay;

namespace Edge.Units
{
    public class UnitofMeasurment<T> : MeasureSystem<T> where T : Measurement
    {
	    public string prefix { get; }
	    public string suffix { get; }
		//what is arbitrary(x)-arbitrary(x+1) in this unit
		public double factorfrombase { get; protected set; }
		//when arbitrary unit is zero, what is this unit?
		public double zerofrombase { get; protected set; }
	    public bool isbaseequivelant => this.factorfrombase == 1 && zerofrombase==0;
	    public UnitofMeasurment(double factor): this("", "", factor){}
        public UnitofMeasurment(string isuf, double factor): this("", isuf, factor)
        {
        }
        public UnitofMeasurment(string pref, string isuf, double factor) : this(pref,isuf,factor,0){}
	    public UnitofMeasurment(double factor, UnitofMeasurment<T> @base) : this("", factor, @base) {}
	    public UnitofMeasurment(string isuf, double factor, UnitofMeasurment<T> @base) : this("", isuf, factor, @base) {}
	    public UnitofMeasurment(string pref, string isuf, double factor, UnitofMeasurment<T> @base) : this(pref,isuf,factor*@base.factorfrombase) {}
		public UnitofMeasurment(double factor, double zerofrombase) : this("", "", factor,zerofrombase) { }
		public UnitofMeasurment(string isuf, double factor, double zerofrombase) : this("", isuf, factor,zerofrombase)
		{
		}
		public UnitofMeasurment(string pref, string isuf, double factor, double zerofrombase)
		{
			this.zerofrombase = zerofrombase;
			this.factorfrombase = factor;
			this.prefix = pref;
			this.suffix = isuf;
		}
		public double toArbitrary(double value)
        {
            return (value-zerofrombase)/ this.factorfrombase;
        }
        public double fromArbitrary(double value)
        {
            return (value * this.factorfrombase) + zerofrombase;
        }
        /// <summary>
        /// treats as a full base unit
        /// </summary>
        public override string ToString()
        {
            return this.ToString(1);
        }
        public override string ToString(double baseval)
        {
            return this.prefix + this.fromArbitrary(baseval) + this.suffix;
        }
        public bool islarger(UnitofMeasurment<T> tocompare)
        {
            return this.factorfrombase < tocompare.factorfrombase;
        }
        public virtual UnitofMeasurment<G> pow<G>(int p=2) where G : Measurement
        {
            return pow<G>(suffix+$"^{p}", p);
        }
        public virtual UnitofMeasurment<G> pow<G>(string newsuffix,int p=2) where G : Measurement
        {
            return pow<G>(prefix, newsuffix, p);
        }
        public virtual UnitofMeasurment<G> pow<G>(string newprefix,string newsuffix,int p=2) where G : Measurement
        {
            if (zerofrombase != 0.0)
                throw new Exception("can't pow a non-zero based measurement");
            return new UnitofMeasurment<G>(newprefix, newsuffix,factorfrombase.pow(p));
        }
        public override int GetHashCode()
        {
            return this.factorfrombase.GetHashCode() ^ this.prefix.GetHashCode() ^ this.suffix.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj.GetHashCode().Equals(this.GetHashCode());
        }
    }
	// ReSharper disable once InconsistentNaming
	public class RWUnitofMeasurment<T> : UnitofMeasurment<T> where T : Measurement
	{
		private readonly ICredentialValidator _credentials;

		public RWUnitofMeasurment(double factor, ICredentialValidator credentials) : this("", factor, credentials) { }
		public RWUnitofMeasurment(string isuf, double factor, ICredentialValidator credentials) : this("", isuf, factor, credentials) { }
		public RWUnitofMeasurment(string pref, string isuf, double factor, ICredentialValidator credentials)
			: base(pref, isuf, factor)
		{
			_credentials = credentials;
		}
		public RWUnitofMeasurment(double factor, UnitofMeasurment<T> @base, ICredentialValidator credentials) : this("", factor, @base, credentials) { }
		public RWUnitofMeasurment(string isuf, double factor, UnitofMeasurment<T> @base, ICredentialValidator credentials) :
									this("", isuf, factor, @base, credentials)
		{ }
		public RWUnitofMeasurment(string pref, string isuf, double factor, UnitofMeasurment<T> @base,
                                  ICredentialValidator credentials) : this(pref, isuf, factor * @base.factorfrombase, credentials)
		{ }
		public void setFactorFromBase(double newfactor, Credential credential)
		{
			_credentials.ThrowIfInvalid(credential);
            factorfrombase = newfactor;
		}
		public void setZerofrombase(double newfactor, Credential credential)
		{
			_credentials.ThrowIfInvalid(credential);
            zerofrombase = newfactor;
		}
	}
	public class MeasureSystem<T> where T : Measurement
    {
        public UnitofMeasurment<T>[] units { get; }
	    public string spacer { get; }
	    /// <summary>
        /// units are sorted from largest to smallest
        /// </summary>
        public void sortunits()
        {
            for (int k = 0; k < this.units.Length; k++)
            {
                int largestint = k;
                bool toswitch = false;
                for (int l = k + 1; l < this.units.Length; l++)
                {
                    if (this.units[l].islarger(this.units[largestint]))
                    {
                        toswitch = true;
                        largestint = l;
                    }
                }
                if (toswitch)
                {
                    UnitofMeasurment<T> t = this.units[k];
                    this.units[k] = this.units[largestint];
                    this.units[largestint] = t;
                }
            }
        }
        public MeasureSystem()
        {
            this.spacer = "";
            this.units = new UnitofMeasurment<T>[0];
        }
        public MeasureSystem(UnitofMeasurment<T> u)
        {
            this.spacer = "";
            this.units = new UnitofMeasurment<T>[] { u };
        }
        public MeasureSystem(string spacers, UnitofMeasurment<T>[] u)
        {
            this.spacer = spacers;
            this.units = u;
            this.sortunits();
        }
        public virtual string ToString(double basevalue)
        {
            foreach (var t in units)
            {
                if (t.fromArbitrary(basevalue) > 1)
                    return t.ToString(basevalue);
            }
            return units.Last().ToString(basevalue);
        }
        public override string ToString()
        {
            return this.ToString(1);
        }
        public override int GetHashCode()
        {
            return this.units.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj.GetHashCode().Equals(this.GetHashCode());
        }
    }
    public abstract class Measurement : IComparable {
        public abstract double Arbitrary { get; }
        public abstract int CompareTo(object obj);
    }
    public static class MeasurementExtentions
    {
        public static double InUnits<T>(this T @this, UnitofMeasurment<T> oum) where T : Measurement
        {
            return oum.fromArbitrary(@this.Arbitrary);
        }
        public static string ToString<T>(this T @this, MeasureSystem<T> s) where T : Measurement
        {
            return s.ToString(@this.Arbitrary);
        }
    }
    namespace Angle
    {
        public class Angle : Measurement
        {
            public static class Quantities
            {
                public readonly static Angle Empty = new Angle(0, Degree);
                public readonly static Angle Right = new Angle(90, Degree);
                public readonly static Angle Straight = new Angle(180, Degree);
            }
            static Angle()
            {
                Degree = new UnitofMeasurment<Angle>("°", 360);
                Minute = new UnitofMeasurment<Angle>("'", 21600);
                Second = new UnitofMeasurment<Angle>("''", 1296000);
                Radian = new UnitofMeasurment<Angle>(" rads", 2 * Math.PI);
                Milliradian = new UnitofMeasurment<Angle>(" millirads", 2000 * Math.PI);
                Turn = new UnitofMeasurment<Angle>(" turns", 1);
                Quadrant = new UnitofMeasurment<Angle>(" quads", 4);
                Gradian = new UnitofMeasurment<Angle>("g", 400);
                DegreeSystem = new MeasureSystem<Angle>(", ", new UnitofMeasurment<Angle>[] { Degree, Minute, Second });
                TurnSystem = new MeasureSystem<Angle>(", ", new UnitofMeasurment<Angle>[] { Turn, Quadrant });
                RadSystem = new MeasureSystem<Angle>(", ", new UnitofMeasurment<Angle>[] { Radian, Milliradian });
            }
            /// <summary>
            /// all values are in degrees
            /// values on non-specific angles are the lowest middle ground between the next two specific angles
            /// </summary>
            public enum AngleClassification { Acute = 45, Right = 90, Obtuse = 135, Straight = 180, Reflex = 270, FullorEmpty = 360, none = 0 };
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Angle)a).Arbitrary);
            }
            public static readonly UnitofMeasurment<Angle> Degree, Minute, Second, Radian, Milliradian, Turn, Quadrant, Gradian;
            public static readonly MeasureSystem<Angle> DegreeSystem, TurnSystem, RadSystem, GradSystem;
            private static readonly Lazy<Funnel<string, Angle>> DefaultParsers =
            new Lazy<Funnel<string, Angle>>(()=> new Funnel<string, Angle>(
                new Parser<Angle>(@"^(\d+(\.\d+)?) ?(turns?|t)$", m => new Angle(double.Parse(m.Groups[1].Value), Turn)),
                new Parser<Angle>(@"^(\d+(\.\d+)?) ?(°|degrees?|d)$", m => new Angle(double.Parse(m.Groups[1].Value), Degree)),
                new Parser<Angle>(@"^(\d+(\.\d+)?) ?(rad|㎭|radians?|c|r)$", m => new Angle(double.Parse(m.Groups[1].Value), Radian)),
                new Parser<Angle>(@"^(\d+(\.\d+)?) ?(grad|g|gradians?|gon)$", m => new Angle(double.Parse(m.Groups[1].Value), Gradian))
            ));
            public static Angle Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
			public static UnitofMeasurment<Angle> ArbitraryUnit => Turn;
			/// <summary>
			/// sign cannot be displayed in many formats
			/// </summary>
			public const char SIGN = '∠';
	        public AngleClassification classify()
            {
                double d = this.InUnits(Degree);
                if (d == 0)
                    return AngleClassification.FullorEmpty;
                if (d < 90 && d > 0)
                    return AngleClassification.Acute;
                if (d == 90)
                    return AngleClassification.Right;
                if (d < 180 && d > 90)
                    return AngleClassification.Obtuse;
                if (d == 180)
                    return AngleClassification.Straight;
                if (d < 180 && d > 360)
                    return AngleClassification.Reflex;
                return AngleClassification.none;
            }
            private readonly Lazy<double> _cos ;
            private readonly Lazy<double> _sin ;
            public double sin()
            {
                return _sin.Value;
            }
            public double tan()
            {
                return sin() / cos();
            }
            public double cos()
            {
                return _cos.Value;
            }
            protected Angle()
            {
                _cos = new Lazy<double>(() => Math.Cos(this.InUnits(Radian)));
                _sin = new Lazy<double>(() => Math.Sin(this.InUnits(Radian)));
            }
            public Angle(double a, UnitofMeasurment<Angle> u) : this()
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public Angle(int a, UnitofMeasurment<Angle> u)
                : this((double)a, u)
            {}
            private Angle(Angle c) : this(c.InUnits(Degree),Degree){}
            public Angle(Point from, Point to)
                : this(aTan((from.X - to.X) / (double)(from.Y - to.Y)))
            {
            }
            public static Angle aSin(double a)
            {
                return new Angle(Math.Asin(a), Radian);
            }
            public static Angle aTan(double a)
            {
                return new Angle(Math.Atan(a), Radian);
            }
            public static Angle aCos(double a)
            {
                return new Angle(Math.Acos(a), Radian);
            }
            public bool isright
            {
                get
                {
                    return (this.Arbitrary % 1 == 0.25);
                }
            }
            public bool isstraight
            {
                get
                {
                    return (this.Arbitrary % 1 == 0.5);
                }
            }
            /// <summary>
            /// the difference between full and Empty angles is virtually meaningless
            /// </summary>
            public bool isfull
            {
                get
                {
                    return (this.Arbitrary % 1 == 0);
                }
            }
            public bool isempty
            {
                get
                {
                    return (this.Arbitrary % 1 == 0);
                }
            }
            public override double Arbitrary { get; }
            public bool isVertical
            {
                get
                {
                    return (this.InUnits(Turn) - 0.25) % 0.5 == 0;
                }
            }
            public bool isHorizontal
            {
                get
                {
                    return (this.InUnits(Turn)) % 0.5 == 0;
                }
            }
			public static Angle operator -(Angle a)
			{
				return (-1.0 * a);
			}
			public static Angle operator *(Angle a, double b)
            {
                return new Angle(a.InUnits(Degree) * b, Degree);
            }
            public static Angle operator /(Angle a, double b)
            {
	            return a * (1 / b);
            }
			public static Angle operator *(double b,Angle a)
			{
				return a * b;
			}
			public static Angle operator +(Angle a, Angle b)
            {
                double c = a.InUnits(Degree) + b.InUnits(Degree);
                return new Angle(c, Degree);
            }
            public static Angle operator -(Angle a, Angle b)
            {
	            return a + (-b);
            }
            public static double operator /(Angle a, Angle b)
            {
	            return a.Arbitrary / b.Arbitrary;
            }
            public static RotationalSpeed.RotationalSpeed operator /(Angle a, TimeSpan b)
            {
                return new RotationalSpeed.RotationalSpeed(a.InUnits(Degree) / b.TotalSeconds, RotationalSpeed.RotationalSpeed.DegreesPerSecond);
            }
            public double Radians => this.InUnits(Radian);
            public override string ToString()
            {
                return DegreeSystem.ToString(this.Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Angle)obj).Arbitrary == this.Arbitrary;
            }
            public static Angle loadfromformat(string format)
            {
                return new Angle(double.Parse(format), Turn);
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode() ^ this.sin().GetHashCode();
            }
            public AbsoluteAngle toAbsoluteAngle()
            {
                return new AbsoluteAngle(this);
            }
        }
        public class AbsoluteAngle : Angle
        {
            public AbsoluteAngle(double a, UnitofMeasurment<Angle> uom) : base(uom.toArbitrary(a).TrueMod(1.0), ArbitraryUnit){}
            public AbsoluteAngle(int a, UnitofMeasurment<Angle> u)
                : this((double)a, u)
            { }
            public AbsoluteAngle(Angle c) : this(c.InUnits(Degree),Degree){ }
            public AbsoluteAngle(AbsoluteAngle c) : this(c.InUnits(Degree), Degree) { }
            public static AbsoluteAngle FromPoints(Point from, Point to)
            {
                if (from.Equals(to))
                    throw new ArgumentException("from and to cannot be equal");
                return to.Y == @from.Y ? new AbsoluteAngle(@from.X < to.X ? 0.25 : 0.75,Turn) : (aTan((@from.X - to.X) / (double)(@from.Y - to.Y)) + (to.X < from.X ? new AbsoluteAngle(0.5,Turn) : new AbsoluteAngle(0,Turn))).toAbsoluteAngle();
            }
            public static AbsoluteAngle FromPoints(double x0, double y0, double x1, double y1)
            {
                if (x0.Equals(x1) && y0.Equals(y1))
                    throw new ArgumentException("from and to cannot be equal");
                return x0 == x1 ? new AbsoluteAngle(y0 < y1 ? 0.25 : 0.75, Turn) : ((aTan((y0 - y1) / (x0 - x1)) + (x1 < x0 ? new AbsoluteAngle(0.5, Turn) : new AbsoluteAngle(0, Turn)))).toAbsoluteAngle();
            }
        }
    }
    namespace Distance
    {
        public class Length : Measurement
        {
            public class Quantities
            {
                public static readonly Length Equator = new Length(40075.04, Kilometer);
                public static readonly Length LunarDistance = new Length(384400, Kilometer);
                public static readonly Length GrainOfSand = new Length(0.5, Millimeter);
                /// <summary>
                /// protons and neutrons are the same size
                /// </summary>
                public static readonly Length Proton = new Length(0.000000000000001, Meter);
                /// <summary>
                /// up and down quarks only
                /// </summary>
                public static readonly Length Quark = new Length(0.000000000000001, Millimeter);
                public static readonly Length HumanHeight = new Length(1.7, Meter);
                public static readonly Length MatchStick = new Length(5, Centimeter);
                public static readonly Length Penny = new Length(1.9, Centimeter);
                public static readonly Length Amoeba = new Length(0.35, Millimeter);
                public static readonly Length HumanHairWidth = new Length(0.1, Millimeter);
                public static readonly Length Boeing = new Length(65, Meter);
                public static readonly Length EiffelTower = new Length(320, Meter);
                public static readonly Length EarthRadius = new Length(6367.4447, Kilometer);
                public static readonly Length SunRadius = new Length(695500, Kilometer);
            }
            public static readonly UnitofMeasurment<Length> Meter, Kilometer, Centimeter, Millimeter, Inch, Foot, Mile, Yard, Astronomicalunit, Lightsecond, Lightyear, Parsec;
            public static readonly MeasureSystem<Length> Metric, Imperial, Astronomical;
            private static readonly Lazy<Funnel<string,Length>> DefaultParsers =
            new Lazy<Funnel<string, Length>>(()=>new Funnel<string, Length>(
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(m|meters?)$", m => new Length(double.Parse(m.Groups[1].Value), Meter)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(km|kilometers?)$", m => new Length(double.Parse(m.Groups[1].Value), Kilometer)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(cm|centimeters?)$", m => new Length(double.Parse(m.Groups[1].Value), Centimeter)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(mm|millimeters?)$", m => new Length(double.Parse(m.Groups[1].Value), Millimeter)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(ft|feet|foot)$", m => new Length(double.Parse(m.Groups[1].Value), Foot)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(in|inch(es)?)$", m => new Length(double.Parse(m.Groups[1].Value), Inch)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(mi|miles?)$", m => new Length(double.Parse(m.Groups[1].Value), Mile)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(yd|yards?)$", m => new Length(double.Parse(m.Groups[1].Value), Yard)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(au|astronomical units?)$", m => new Length(double.Parse(m.Groups[1].Value), Astronomicalunit)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(ls|light seconds?)$", m => new Length(double.Parse(m.Groups[1].Value), Lightsecond)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(ly|light years?)$", m => new Length(double.Parse(m.Groups[1].Value), Lightyear)),
                new Parser<Length>(@"^(\d+(\.\d+)?) ?(pc|parsecs?)$", m => new Length(double.Parse(m.Groups[1].Value), Parsec))
            ));
			public static Length Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Length()
            {
                Meter = new UnitofMeasurment<Length>("m", 1);
                Kilometer = new UnitofMeasurment<Length>("km", 0.001);
                Centimeter = new UnitofMeasurment<Length>("cm", 100);
                Millimeter = new UnitofMeasurment<Length>("mm", 1000);
                Inch = new UnitofMeasurment<Length>("''", 39.372);
                Foot = new UnitofMeasurment<Length>("'", 3.281);
                Mile = new UnitofMeasurment<Length>("mi", 0.0006214);
                Yard = new UnitofMeasurment<Length>("yd", 1.094);
                Astronomicalunit = new UnitofMeasurment<Length>(" AU", 0.0000000000066845871);
                Lightsecond = new UnitofMeasurment<Length>(" LS", 0.000000003336);
                Lightyear = new UnitofMeasurment<Length>(" LY", 0.0000000000000001057);
                Parsec = new UnitofMeasurment<Length>(" Pc", 0.000000000000000032407793);
                Metric = new MeasureSystem<Length>(", ", new UnitofMeasurment<Length>[] { Meter, Kilometer, Centimeter, Millimeter });
                Imperial = new MeasureSystem<Length>(" ", new UnitofMeasurment<Length>[] { Inch, Foot, Mile, Yard });
                Astronomical = new MeasureSystem<Length>(", ", new UnitofMeasurment<Length>[] { Astronomicalunit, Lightsecond, Lightyear, Parsec });
            }
            public override double Arbitrary { get; }
            public static Length operator -(Length a)
			{
				return (-1.0 * a);
			}
			public static Length operator *(Length a, double b)
            {
                return new Length(a.InUnits(Meter) * b, Meter);
            }
			public static Length operator *(double b, Length a)
			{
				return a* b;
			}
			public static Area.Area operator *(Length b, Length a)
			{
				return new Area.Area(a.InUnits(Meter)*b.InUnits(Meter),Area.Area.MeterSq);
			}
			public static Length operator /(Length a, double b)
            {
	            return a * (1 / b);
            }
	        public static Length operator +(Length a, Length b)
            {
                return new Length(a.InUnits(Meter) + b.InUnits(Meter), Meter);
            }
            public static Length operator -(Length a, Length b)
            {
	            return a + (-b);
            }
            public static double operator /(Length a, Length b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Speed.Speed operator /(Length a, TimeSpan b)
            {
                return new Speed.Speed(a.InUnits(Meter)/b.TotalSeconds,Speed.Speed.MetersPerSecond);
            }
            public Length(double a, UnitofMeasurment<Length> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Metric.ToString(this.Arbitrary);
            }
            public string ToString(MeasureSystem<Length> s)
            {
                return s.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Length)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Length)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Time
    {
        public static class TimeExtentions
        {
            public enum TimeRoundPoint {Days, Hours, Minutes, Seconds, none}
            public static TimeSpan Divide(this TimeSpan t, double divisor)
            {
                return multiply(t,1.0/divisor);
            }
            public static double Divide(this TimeSpan t, TimeSpan divisor)
            {
                return (t.Ticks / (double)divisor.Ticks);
            }
            public static TimeSpan multiply(this TimeSpan t, double factor)
            {
                return new TimeSpan((long)(t.Ticks * factor));
            }
            public static string printable(this TimeSpan t, bool shorthand = false, bool avoidzeroes = true)
            {
                if (!shorthand)
                {
                    if (t.TotalMilliseconds < 300)
	                    return pluralize(avoidzeroes ? t.TotalMilliseconds.round(NumberMagic.RoundMethod.UntilNotZero) : t.TotalMilliseconds.round(), "millisecond", "s", true);
	                if (t.TotalSeconds < 60)
                            return pluralize(t.TotalSeconds.round(1), "second", "s", true);
                    if (t.TotalMinutes < 60)
                    {
                        string ret = pluralize(t.Minutes, "minute", "s", true);
                        if (t.Seconds > 0)
                            ret = ret + " and " + pluralize(t.Seconds, "second", "s", true);
                        return ret;
                    }
                    if (t.TotalHours < 24)
                    {
                        string ret = pluralize(t.Hours, "hour", "s", true);
                        if (t.Minutes > 0)
                            ret = ret + " and " + pluralize(t.Minutes, "minute", "s", true);
                        return ret;
                    }
                    if (t.TotalDays < 183)
                    {
                        string ret = pluralize(t.Days, "day", "s", true);
                        if (t.Hours > 0)
                            ret = ret + " and " + pluralize(t.Hours, "hour", "s", true);
                        return ret;
                    }
                    string v = pluralize((t.Days / 365), "year", "s", true);
                    if (t.Days % 365 > 0)
                        v = v + " and " + pluralize(t.Days % 365, "day", "s", true);
                    return v;
                }
                if (t.TotalMilliseconds < 300)
                {
                    if (avoidzeroes)
                        return t.TotalMilliseconds.round(NumberMagic.RoundMethod.UntilNotZero) + "ms";
                    return t.TotalMilliseconds.round() + "ms";
                }
                if (t.TotalSeconds < 60)
                    return t.TotalSeconds.round(1) + "s";
                if (t.TotalMinutes < 60)
                {
                    string ret = t.Minutes + "m";
                    if (t.Seconds > 0)
                        ret = ret + t.Seconds + "s";
                    return ret;
                }
                if (t.TotalHours < 24)
                {
                    string ret = t.Hours + "h";
                    if (t.Minutes > 0)
                        ret = ret + t.Minutes + "m";
                    return ret;
                }
                if (t.TotalDays < 183)
                {
                    string ret = t.Days + "d";
                    if (t.Hours > t.Days * 3)
                        ret = ret + t.Hours+"h";
                    return ret;
                }
                string u = (t.Days / 365).round() +  "y";
                if (t.Days % 365 > 0)
                    u = u +(t.Days % 365) +"d";
                return u;
            }
            public static TimeSpan Round(this TimeSpan t, TimeRoundPoint place = TimeRoundPoint.Seconds)
            {
                switch (place)
                {
                    case TimeRoundPoint.Days:
                        return new TimeSpan(t.Days, 0, 0, 0);
                    case TimeRoundPoint.Hours:
                        return new TimeSpan(t.Days, t.Hours, 0, 0);
                    case TimeRoundPoint.Minutes:
                        return new TimeSpan(t.Days, t.Hours, t.Minutes,0);
                    case TimeRoundPoint.Seconds:
                        return new TimeSpan(t.Days, t.Hours, t.Minutes, t.Seconds);
                    default:
                        return t;
                }
            }
        }
        public static class Quantities
        {
	        // ReSharper disable InconsistentNaming
            public readonly static TimeSpan Second, Minute, Millisecond, Hour, Day, Week, Month_28Days, Month_30Days, Month_31Days, Month_Average, Year, Decade;
	        // ReSharper restore InconsistentNaming
            static Quantities()
            {
                Second = new TimeSpan(0, 0, 1);
                Minute = new TimeSpan(0, 1, 0);
                Millisecond = new TimeSpan(0, 0, 0, 0, 1);
                Hour = new TimeSpan(1, 0, 0);
                Day = new TimeSpan(1, 0, 0, 0);
				Week = new TimeSpan(7,0,0);
                Month_28Days = new TimeSpan(28, 0, 0, 0);
                Month_30Days = new TimeSpan(30, 0, 0, 0);
                Month_31Days = new TimeSpan(31, 0, 0, 0);
                Month_Average = new TimeSpan(30, 10, 5, 0);
                Year = new TimeSpan(365, 0, 0, 0);
                Decade = new TimeSpan(3650, 0, 0, 0);
            }
        }
    }
	namespace Mass
    {
        public class Mass : Measurement
        {
            public static readonly UnitofMeasurment<Mass> Gram, Kilogram, Ton, Milligram, Microgram, Grain, Ounce, Pound, Stone, Earth, Solar;
            public static readonly MeasureSystem<Mass> Metric, Imperial, Astronomical;
            private static readonly Lazy<Funnel<string,Mass>> DefaultParsers =
            new Lazy<Funnel<string, Mass>>(()=>new Funnel<string, Mass>(
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(g|grams?)$", m => new Mass(double.Parse(m.Groups[1].Value), Gram)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(kg|kilograms?)$", m => new Mass(double.Parse(m.Groups[1].Value), Kilogram)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(t|ton(nes)?)$", m => new Mass(double.Parse(m.Groups[1].Value), Ton)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(mg|milligrams?)$", m => new Mass(double.Parse(m.Groups[1].Value), Milligram)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(µg|ug|mcg|micrograms?)$", m => new Mass(double.Parse(m.Groups[1].Value), Microgram)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(grains?)$", m => new Mass(double.Parse(m.Groups[1].Value), Grain)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(oz|ounces?)$", m => new Mass(double.Parse(m.Groups[1].Value), Ounce)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(lbm?|pounds?)$", m => new Mass(double.Parse(m.Groups[1].Value), Pound)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(st|stones?)$", m => new Mass(double.Parse(m.Groups[1].Value), Stone)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(M⊕|earth mass(es)?)$", m => new Mass(double.Parse(m.Groups[1].Value), Earth)),
                new Parser<Mass>(@"^(\d+(\.\d+)?) ?(M☉|solar mass(es)?)$", m => new Mass(double.Parse(m.Groups[1].Value), Solar))
            ));
            public static Mass Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
			public static UnitofMeasurment<Mass> ArbitraryUnit => Kilogram;
			static Mass()
            {
                Microgram = new UnitofMeasurment<Mass>("microgram", 1E9);
                Milligram = new UnitofMeasurment<Mass>("milligram", 1E6);
                Gram = new UnitofMeasurment<Mass>("gram", 1000);
                Kilogram = new UnitofMeasurment<Mass>("kg", 1);
                Ton = new UnitofMeasurment<Mass>("ton", 1E-3);
                Grain = new UnitofMeasurment<Mass>("grain", 15432.09877);
                Ounce = new UnitofMeasurment<Mass>("ounce", 35.27336861);
                Pound = new UnitofMeasurment<Mass>("lb", 2.204922622);
                Stone = new UnitofMeasurment<Mass>("stone", 0.1574730444);
                Earth = new UnitofMeasurment<Mass>("earth mass", 1.6744252E-25);
                Solar = new UnitofMeasurment<Mass>("solar unit", 5.029081E-31);
                Metric = new MeasureSystem<Mass>(", ", new UnitofMeasurment<Mass>[] { Gram, Kilogram, Ton, Milligram, Microgram });
                Imperial = new MeasureSystem<Mass>(" ", new UnitofMeasurment<Mass>[] { Grain, Ounce, Pound, Stone });
                Astronomical = new MeasureSystem<Mass>(", ", new UnitofMeasurment<Mass>[] { Earth, Solar });
            }
            public override double Arbitrary { get; }
            public static Mass operator -(Mass a)
			{
				return (-1.0 * a);
			}
			public static Mass operator *(Mass a, double b)
            {
                return new Mass(a.InUnits(Kilogram) * b, Kilogram);
            }
			public static Mass operator *(double b, Mass a)
			{
				return a * b;
			}
			public static Mass operator /(Mass a, double b)
			{
				return a * (1 / b);
			}
            public static Mass operator +(Mass a, Mass b)
            {
                return new Mass(a.InUnits(Kilogram) + b.InUnits(Kilogram), Kilogram);
            }
            public static Mass operator -(Mass a, Mass b)
            {
	            return a + (-b);
            }
            public static double operator /(Mass a, Mass b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Density.Density operator /(Volume.Volume a, Mass b)
            {
                return new Density.Density(a.InUnits(Volume.Volume.MeterCb)/b.InUnits(Kilogram),Density.Density.KilogramsPerMeterCb);
            }
            public Mass(double a, UnitofMeasurment<Mass> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Metric.ToString(this.Arbitrary);
            }
            public string ToString(MeasureSystem<Mass> s)
            {
                return s.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Mass)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Mass)obj).Arbitrary == this.Arbitrary;
            }
            public static Mass loadfromformat(string format)
            {
                return new Mass(double.Parse(format), Kilogram);
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode() ^ this.InUnits(Stone).GetHashCode();
            }
        }
    }
    namespace DigitalInfo
    {
        public class BinaryData : Measurement
        {
            public static readonly UnitofMeasurment<BinaryData> Bit, Byte, Kilobyte, Megabyte, Gigabyte, Terrabyte, Pettabyte, Exabyte, Zettabyte, Yottabyte;
            public static readonly MeasureSystem<BinaryData> BinaryByte;
			public static UnitofMeasurment<BinaryData> ArbitraryUnit => Megabyte;
			private static readonly Lazy<Funnel<string, BinaryData>> DefaultParsers =
            new Lazy<Funnel<string, BinaryData>>(()=> new Funnel<string, BinaryData>(
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(b|bits?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Bit)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(bytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Byte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(kb|kilobytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Kilobyte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(mb|megabytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Megabyte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(gb|gigabytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Gigabyte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(tb|terrabytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Terrabyte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(pb|pettabytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Pettabyte)),
                new Parser<BinaryData>(@"^(\d+(\.\d+)?) ?(eb|exabytes?)$", m => new BinaryData(double.Parse(m.Groups[1].Value), Exabyte))
            ));
            public static BinaryData Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
	        static BinaryData()
            {
                Bit = new UnitofMeasurment<BinaryData>("bit", 8388608);
                Byte = new UnitofMeasurment<BinaryData>("byte", 1048576);
                Kilobyte = new UnitofMeasurment<BinaryData>("KB", 1024);
                Megabyte = new UnitofMeasurment<BinaryData>("MB", 1);
                Gigabyte = new UnitofMeasurment<BinaryData>("GB", 1 / (double)1024);
                Terrabyte = new UnitofMeasurment<BinaryData>("TB", 1 / (double)1048576);
                Pettabyte = new UnitofMeasurment<BinaryData>("PB", 1 / (double)1073741824);
                Exabyte = new UnitofMeasurment<BinaryData>("EB", 1 / (double)1099511627776);
                Zettabyte = new UnitofMeasurment<BinaryData>("ZB", 1 / (double)1125899906842624);
                Yottabyte = new UnitofMeasurment<BinaryData>("YB", 1 / (double)1152921504606846976);
                BinaryByte = new MeasureSystem<BinaryData>(", ", new UnitofMeasurment<BinaryData>[]
                    {Bit, Byte, Kilobyte, Megabyte, Gigabyte, Terrabyte, Pettabyte, Exabyte, Zettabyte, Yottabyte});
            }
            public override double Arbitrary { get; }
            public static BinaryData operator -(BinaryData a)
			{
				return (-1.0 * a);
			}
			public static BinaryData operator *(BinaryData a, double b)
            {
                return new BinaryData(a.InUnits(Kilobyte) * b, Kilobyte);
            }
            public static BinaryData operator /(BinaryData a, double b)
            {
	            return a * (1.0 / b);
            }
			public static BinaryData operator *(double b, BinaryData a)
			{
				return a*b;
			}
			public static BinaryData operator +(BinaryData a, BinaryData b)
            {
                return new BinaryData(a.InUnits(Kilobyte) + b.InUnits(Kilobyte), Kilobyte);
            }
            public static BinaryData operator -(BinaryData a, BinaryData b)
            {
	            return a + (-b);
            }
            public static double operator /(BinaryData a, BinaryData b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public BinaryData(double a, UnitofMeasurment<BinaryData> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Megabyte.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((BinaryData)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((BinaryData)obj).Arbitrary == this.Arbitrary;
            }
            public static BinaryData loadfromformat(string format)
            {
                return new BinaryData(double.Parse(format), Megabyte);
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Currency
    {
        public class Money : Measurement
        {
            private static readonly CredentialValidator _editcredientials;
            public static readonly RWUnitofMeasurment<Money> Dollars, Cents, IsraeliNewSheckels, Agoras, Euros, CentEuros, Yen;
            public static readonly MeasureSystem<Money> UnitedStates, Israel, Europe, Japan;
            public readonly static TimeSpan DefaultUpdateTimeTolerance = new TimeSpan(12, 0, 0);
#pragma warning disable CC0033
            private readonly static SyncPermaObject<string> _ExchangeRatePerma = new SyncPermaObject<string>(Encoding.ASCII.GetString, Encoding.ASCII.GetBytes, "__Cobra_Units_Money_ExchangeRates", true, ".xml");
            private static bool _initialized;
            private static readonly Lazy<Funnel<string, Money>> DefaultParsers =
            new Lazy<Funnel<string, Money>>(() => new Funnel<string, Money>(
                new Parser<Money>(@"^(\d+(\.\d+)?) ?(dollars?|bucks?)$", m => new Money(double.Parse(m.Groups[1].Value), Dollars)),
                new Parser<Money>(@"^\$(\d+(\.\d+)?)$", m => new Money(double.Parse(m.Groups[1].Value), Dollars)),
                new Parser<Money>(@"^(\d+(\.\d+)?) ?(cents?)$", m => new Money(double.Parse(m.Groups[1].Value), Cents)),
                new Parser<Money>(@"^(\d+(\.\d+)?) ?(INS|ins|Israeli New Sheckels?|israeli new sheckel)$", m => new Money(double.Parse(m.Groups[1].Value), IsraeliNewSheckels)),
                new Parser<Money>(@"^₪(\d+(\.\d+)?))$", m => new Money(double.Parse(m.Groups[1].Value), IsraeliNewSheckels)),
                new Parser<Money>(@"^(\d+(\.\d+)?)) ?(euros?|Euros?)$", m => new Money(double.Parse(m.Groups[1].Value), Euros)),
                new Parser<Money>(@"^€(\d+(\.\d+)?))$", m => new Money(double.Parse(m.Groups[1].Value), Euros)),
                new Parser<Money>(@"^(\d+(\.\d+)?)) ?(yen)$", m => new Money(double.Parse(m.Groups[1].Value), Yen)),
                new Parser<Money>(@"^¥(\d+(\.\d+)?))$", m => new Money(double.Parse(m.Groups[1].Value), Yen))
            ));
            public static UnitofMeasurment<Money> ArbitraryUnit => Euros;
            public static Money Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Money()
            {
                _editcredientials = new CredentialValidator();
                Euros = new RWUnitofMeasurment<Money>("€", "", 1, _editcredientials);
                CentEuros = new RWUnitofMeasurment<Money>("euro cents", 100, _editcredientials);
                Dollars = new RWUnitofMeasurment<Money>("$", "", 1, _editcredientials);
                Cents = new RWUnitofMeasurment<Money>("¢", "", 1, _editcredientials);
                IsraeliNewSheckels = new RWUnitofMeasurment<Money>("₪", "", 1, _editcredientials);
                Agoras = new RWUnitofMeasurment<Money>("agoras", 1, _editcredientials);
                Yen = new RWUnitofMeasurment<Money>("¥", "", 1, _editcredientials);
                UnitedStates = new MeasureSystem<Money>(", ", new UnitofMeasurment<Money>[] { Dollars, Cents });
                Israel = new MeasureSystem<Money>(", ", new UnitofMeasurment<Money>[] { IsraeliNewSheckels, Agoras });
                Europe = new MeasureSystem<Money>(", ", new UnitofMeasurment<Money>[] { Euros, CentEuros });
                Japan = new MeasureSystem<Money>(Yen);
            }
            public static bool updateRates()
            {
                Exception error;
                return updateRates(out error);
            }
            public static bool updateRates([CanBeNull] out Exception error)
            {
                return updateRates(DefaultUpdateTimeTolerance, out error);
            }
            public static bool updateRates(TimeSpan updateTimeTolerance, [CanBeNull] out Exception error)
            {
                error = null;
                if (_ExchangeRatePerma.timeSinceUpdate() < updateTimeTolerance && _initialized)
                    return false;
                var doc = _ExchangeRatePerma.timeSinceUpdate() < updateTimeTolerance ? XPathMarksman.getDocFromString(_ExchangeRatePerma.value) : loadXml(out error);
                if (error != null)
                {
                    return false;
                }
                XmlElement root = doc.DocumentElement;
                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("gesmes", @"http://www.gesmes.org/xml/2002-08-01");
                nsm.AddNamespace("def", @"http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
                Dollars.setFactorFromBase(getrate(root, "USD", nsm), _editcredientials.valid);
                Cents.setFactorFromBase(Dollars.factorfrombase * 100, _editcredientials.valid);
                IsraeliNewSheckels.setFactorFromBase(getrate(root, "ILS", nsm), _editcredientials.valid);
                Agoras.setFactorFromBase(IsraeliNewSheckels.factorfrombase * 100, _editcredientials.valid);
                Yen.setFactorFromBase(getrate(root, "JPY", nsm), _editcredientials.valid);
                _initialized = true;
                return true;
            }
            private static XmlDocument loadXml(out Exception error)
            {
                XmlDocument doc = WebGuard.LoadXmlDocumentFromUrl(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml", out error);
                _ExchangeRatePerma.value = doc.InnerXml;
                return doc;
            }
            private static double getrate(XmlNode root, string identifier, XmlNamespaceManager xnsm)
            {
                return double.Parse(root.SelectSingleNode("//def:Cube[@currency=\"" + identifier + "\"]", xnsm).Attributes["rate"].InnerText);
            }
            public override double Arbitrary { get; }
            public static Money operator -(Money a)
            {
                return (-1.0 * a);
            }
            public static Money operator +(Money a, Money b)
            {
                return new Money(a.InUnits(Euros) + b.InUnits(Euros), Euros);
            }
            public static Money operator -(Money a, Money b)
            {
                return a + (-b);
            }
            public static Money operator *(Money a, double b)
            {
                return new Money(a.InUnits(Euros) * b, Euros);
            }
            public static Money operator *(double b, Money a)
            {
                return a * b;
            }
            public static Money operator /(Money a, double b)
            {
                return a * (1.0 / b);
            }
            public static double operator /(Money a, Money b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public Money(double a, UnitofMeasurment<Money> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Euros.ToString(this.InUnits(Euros));
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Money)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Money)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Temperature
    {
        public class Temperature : Measurement
        {
            public static class Quantities
            {
                public static readonly Temperature AbsoluteZero = new Temperature(0, Kelvin),
                                                   WaterFreeze = new Temperature(0, Celsius),
                                                   RoomTemperature = new Temperature(20, Celsius),
                                                   HumanTemperature = new Temperature(37, Celsius),
                                                   WaterBoil = new Temperature(100, Celsius)
                    ;
            }
            public static readonly UnitofMeasurment<Temperature> Kelvin, Celsius, Fahrenheit;
            public static UnitofMeasurment<Temperature> ArbitraryUnit => Kelvin;
            private static readonly Lazy<Funnel<string, Temperature>> DefaultParsers = new Lazy<Funnel<string, Temperature>>(
                () => new Funnel<string, Temperature>(
                new Parser<Temperature>(@"^(\d+(\.\d+)?) ?((K|k)(elvin)?)$", m => new Temperature(double.Parse(m.Groups[1].Value), Kelvin)),
                new Parser<Temperature>(@"^(\d+(\.\d+)?) ?((°)?(C|c)(elsius)?)$", m => new Temperature(double.Parse(m.Groups[1].Value), Celsius)),
                new Parser<Temperature>(@"^(\d+(\.\d+)?) ?((°)?(F|f)(ahrenheit)?)$", m => new Temperature(double.Parse(m.Groups[1].Value), Fahrenheit))
            ));
            public static Temperature Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Temperature()
            {
                Kelvin = new UnitofMeasurment<Temperature>("", "K", 1);
                Celsius = new UnitofMeasurment<Temperature>("", "°C", 1, -273.15);
                Fahrenheit = new UnitofMeasurment<Temperature>("", "°F", 9 / 5.0, -459.67);
            }
            public Temperature(double a, UnitofMeasurment<Temperature> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override double Arbitrary { get; }
            public static Temperature operator *(Temperature a, double b)
            {
                return new Temperature(a.InUnits(Kelvin) * b, Kelvin);
            }
            public static Temperature operator /(Temperature a, double b)
            {
                return a * (1.0 / b);
            }
            public static Temperature operator *(double b, Temperature a)
            {
                return a * b;
            }
            public override string ToString()
            {
                return Kelvin.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Temperature)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Temperature)obj).Arbitrary == this.Arbitrary;
            }
            public static Temperature loadfromformat(string format)
            {
                return new Temperature(double.Parse(format), Kelvin);
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Frequency
    {
        public class Frequency : Measurement
        {
            public static readonly UnitofMeasurment<Frequency> Hertz, MilliHertz, KiloHertz, MegaHertz, GigaHertz,TerraHertz;
            public static readonly MeasureSystem<Frequency> HertzSystem;
            public static UnitofMeasurment<Frequency> ArbitraryUnit => Hertz;
            private static readonly Lazy<Funnel<string, Frequency>> DefaultParsers =
            new Lazy<Funnel<string, Frequency>>(() => new Funnel<string, Frequency>(
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), Hertz)),
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(milli|m)(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), MilliHertz)),
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(K|kilo)(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), KiloHertz)),
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(mega|M)(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), MegaHertz)),
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(giga|G)(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), GigaHertz)),
                new Parser<Frequency>(@"^(\d+(\.\d+)?) ?(tera|T)(hz|Hz|h|H|Hertz|hertz)$", m => new Frequency(double.Parse(m.Groups[1].Value), TerraHertz))
            ));
            public static Frequency Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Frequency()
            {
                Hertz = new UnitofMeasurment<Frequency>("Hz",1);
                MilliHertz = new UnitofMeasurment<Frequency>("mHz",1E3);
                KiloHertz = new UnitofMeasurment<Frequency>("KHz", 1E-3);
                MegaHertz = new UnitofMeasurment<Frequency>("MHz", 1E-6);
                GigaHertz = new UnitofMeasurment<Frequency>("GHz", 1E-9);
                TerraHertz = new UnitofMeasurment<Frequency>("THz", 1E-12);
                HertzSystem = new MeasureSystem<Frequency>(",", new []{MilliHertz,Hertz,KiloHertz,MegaHertz,GigaHertz,TerraHertz});
            }
            public override double Arbitrary { get; }
            public static Frequency operator *(Frequency a, double b)
            {
                return new Frequency(a.InUnits(ArbitraryUnit) * b, ArbitraryUnit);
            }
            public static Frequency operator /(Frequency a, double b)
            {
                return a * (1.0 / b);
            }
            public static TimeSpan operator /(double b, Frequency a)
            {
                return TimeSpan.FromSeconds(b/ a.InUnits(Hertz));
            }
            public static Frequency operator *(double b, Frequency a)
            {
                return a * b;
            }
            public static double operator /(Frequency a, Frequency b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static double operator *(Frequency a, TimeSpan b)
            {
                return a.InUnits(Hertz)*b.TotalSeconds;
            }
            public static double operator *(TimeSpan a, Frequency b)
            {
                return b*a;
            }
            public static Speed.Speed operator *(Length a, Frequency b)
            {
                return a / (1 / b);
            }
            public static Force.Force operator *(Momentum.Momentum a, Frequency b)
            {
                return a / (1 / b);
            }
            public static Acceleration.Acceleration operator *(Speed.Speed a, Frequency b)
            {
                return a / (1 / b);
            }
            public Frequency(double a, UnitofMeasurment<Frequency> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return HertzSystem.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Frequency)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Frequency)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
        public static class FrequencyExtention
        {
            public static Frequency invert(this TimeSpan @this)
            {
                return new Frequency(1.0/@this.TotalSeconds, Frequency.Hertz);
            }
        }
    }
}