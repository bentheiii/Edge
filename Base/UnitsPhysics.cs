using System;
using Edge.Funnels;
using Edge.Units.Distance;
using Edge.WordsPlay.Parsing;

namespace Edge.Units
{
    namespace Area
    {
        public class Area : Measurement
        {
            public static readonly UnitofMeasurment<Area> MeterSq, KilometerSq, CentimeterSq, MillimeterSq, InchSq, FootSq, MileSq, YardSq, Acre;
            public static readonly MeasureSystem<Area> Metric, Imperial;
            private static readonly Lazy<Funnel<string, Area>> DefaultParsers =
            new Lazy<Funnel<string, Area>>(() => new Funnel<string, Area>(
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(m|meters?)^2$", m => new Area(double.Parse(m.Groups[1].Value), MeterSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(km|kilometers?)^2$", m => new Area(double.Parse(m.Groups[1].Value), KilometerSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(cm|centimeters?)^2$", m => new Area(double.Parse(m.Groups[1].Value), CentimeterSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(mm|millimeters?)^2$", m => new Area(double.Parse(m.Groups[1].Value), MillimeterSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(ft|feet|foot)^2$", m => new Area(double.Parse(m.Groups[1].Value), FootSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(in|inch(es)?)^2$", m => new Area(double.Parse(m.Groups[1].Value), InchSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(mi|miles?)^2$", m => new Area(double.Parse(m.Groups[1].Value), MileSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(yd|yards?)^2$", m => new Area(double.Parse(m.Groups[1].Value), YardSq)),
                new Parser<Area>(@"^(\d+(\.\d+)?) ?(ac|acres?)$", m => new Area(double.Parse(m.Groups[1].Value), Acre))
            ));
            public static UnitofMeasurment<Area> ArbitraryUnit => MeterSq;
            public static Area Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Area()
            {
                MeterSq = Length.Meter.pow<Area>();
                KilometerSq = Length.Kilometer.pow<Area>();
                CentimeterSq = Length.Centimeter.pow<Area>();
                MillimeterSq = Length.Millimeter.pow<Area>();
                InchSq = Length.Inch.pow<Area>();
                FootSq = Length.Foot.pow<Area>();
                MileSq = Length.Mile.pow<Area>();
                YardSq = Length.Yard.pow<Area>();
                Acre = new UnitofMeasurment<Area>("ac", 4840, YardSq);
                Metric = new MeasureSystem<Area>(", ", new UnitofMeasurment<Area>[] { MeterSq, KilometerSq, CentimeterSq, MillimeterSq });
                Imperial = new MeasureSystem<Area>(" ", new UnitofMeasurment<Area>[] { InchSq, FootSq, MileSq, YardSq });
            }
            public override double Arbitrary { get; }
            public static Area FromCircle(Length radius)
            {
                return radius * radius * Math.PI;
            }
            public static Area FromTriangle(Length side1, Angle.Angle midAngle, Length side2)
            {
                return side1 * side2 * (midAngle.sin() / 2);
            }
            public static Area FromTriangle(Angle.Angle angle1, Length midside, Angle.Angle angle2)
            {
                return (midside * midside) * angle1.sin() * angle2.sin() / (2 * (angle1 + angle2).sin());
            }
            public static Area FromRightTriangle(Length hypotenuse, Angle.Angle midAngle)
            {
                return hypotenuse * hypotenuse * (midAngle.sin() * midAngle.cos() / 2);
            }
            public static Area operator -(Area a)
            {
                return (-1.0 * a);
            }
            public static Area operator *(Area a, double b)
            {
                return new Area(a.InUnits(MeterSq) * b, MeterSq);
            }
            public static Area operator *(double b, Area a)
            {
                return a * b;
            }
            public static Area operator /(Area a, double b)
            {
                return a * (1 / b);
            }
            public static Area operator +(Area a, Area b)
            {
                return new Area(a.InUnits(MeterSq) + b.InUnits(MeterSq), MeterSq);
            }
            public static Area operator -(Area a, Area b)
            {
                return a + (-b);
            }
            public static double operator /(Area a, Area b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Length operator /(Area a, Length b)
            {
                return new Length(a.InUnits(MeterSq) / b.InUnits(Length.Meter), Length.Meter);
            }
            public static Volume.Volume operator *(Area a, Length b)
            {
                return new Volume.Volume(a.InUnits(MeterSq) * b.InUnits(Length.Meter), Volume.Volume.MeterCb);
            }
            public static Volume.Volume operator *(Length a, Area b)
            {
                return b * a;
            }
            public Area(double a, UnitofMeasurment<Area> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Metric.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Area)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Area)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
            public Length Sqrt()
            {
                return new Length(Math.Sqrt(this.InUnits(MeterSq)), Length.Meter);
            }
        }
    }
    namespace Volume
    {
        public class Volume : Measurement
        {
            public static readonly UnitofMeasurment<Volume> MeterCb, KilometerCb, CentimeterCb, MillimeterCb, Litre, Millilitre, Pint, Gallon;
            public static readonly MeasureSystem<Volume> Metric, Imperial, LitreMeasureSystem;
            private static readonly Lazy<Funnel<string, Volume>> DefaultParsers =
            new Lazy<Funnel<string, Volume>>(() => new Funnel<string, Volume>(
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(m|meters?)^3$", m => new Volume(double.Parse(m.Groups[1].Value), MeterCb)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(km|kilometers?)^3$", m => new Volume(double.Parse(m.Groups[1].Value), KilometerCb)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(cm|centimeters?)^3$", m => new Volume(double.Parse(m.Groups[1].Value), CentimeterCb)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(mm|millimeters?)^3$", m => new Volume(double.Parse(m.Groups[1].Value), MillimeterCb)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(L|Liters?|Liters?)$", m => new Volume(double.Parse(m.Groups[1].Value), Litre)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?milli(l|liters?|liters?)$", m => new Volume(double.Parse(m.Groups[1].Value), Millilitre)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(pt|Pints?)$", m => new Volume(double.Parse(m.Groups[1].Value), Pint)),
                new Parser<Volume>(@"^(\d+(\.\d+)?) ?(g|G|Gallons?)$", m => new Volume(double.Parse(m.Groups[1].Value), Gallon))
            ));
            public static UnitofMeasurment<Volume> ArbitraryUnit => MeterCb;
            public static Volume Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Volume()
            {
                MeterCb = Length.Meter.pow<Volume>(3);
                KilometerCb = Length.Kilometer.pow<Volume>(3);
                CentimeterCb = Length.Centimeter.pow<Volume>(3);
                MillimeterCb = Length.Millimeter.pow<Volume>(3);
                Litre = new UnitofMeasurment<Volume>("L", 1000, CentimeterCb);
                Millilitre = new UnitofMeasurment<Volume>("mL", 1, CentimeterCb);
                Pint = new UnitofMeasurment<Volume>("pt", 568, Millilitre);
                Gallon = new UnitofMeasurment<Volume>(" gallon", 4.54609, Litre);
                LitreMeasureSystem = new MeasureSystem<Volume>(", ", new UnitofMeasurment<Volume>[] { Litre, Millilitre });
                Metric = new MeasureSystem<Volume>(", ", new UnitofMeasurment<Volume>[] { MeterCb, KilometerCb, CentimeterCb, MillimeterCb });
                Imperial = new MeasureSystem<Volume>(" ", new UnitofMeasurment<Volume>[] { Pint, Gallon });
            }
            public override double Arbitrary { get; }
            public static Volume operator -(Volume a)
            {
                return (-1.0 * a);
            }
            public static Volume operator *(Volume a, double b)
            {
                return new Volume(a.InUnits(MeterCb) * b, MeterCb);
            }
            public static Volume operator *(double b, Volume a)
            {
                return a * b;
            }
            public static Volume operator /(Volume a, double b)
            {
                return a * (1 / b);
            }
            public static Volume operator +(Volume a, Volume b)
            {
                return new Volume(a.InUnits(MeterCb) + b.InUnits(MeterCb), MeterCb);
            }
            public static Volume operator -(Volume a, Volume b)
            {
                return a + (-b);
            }
            public static double operator /(Volume a, Volume b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Area.Area operator /(Volume a, Length b)
            {
                return new Area.Area(a.InUnits(MeterCb) / b.InUnits(Length.Meter), Area.Area.MeterSq);
            }
            public static Length operator /(Volume a, Area.Area b)
            {
                return new Length(a.InUnits(MeterCb) / b.InUnits(Area.Area.MeterSq), Length.Meter);
            }
            public Volume(double a, UnitofMeasurment<Volume> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override string ToString()
            {
                return Metric.ToString(this.Arbitrary);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Volume)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Volume)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
            public Length Cbrt()
            {
                return new Length(Math.Pow(this.InUnits(MeterCb), 1.0 / 3), Length.Meter);
            }
        }
    }
    namespace Density
    {
        public class Density : Measurement
        {
            public static class Quantities
            {
                public static readonly Density WaterDensity = new Density(1000, KilogramsPerMeterCb), Air = new Density(1.2, KilogramsPerMeterCb);
            }
            public static readonly UnitofMeasurment<Density> KilogramsPerMeterCb, TonnesPerMeterCb;
            private static readonly Lazy<Funnel<string, Density>> DefaultParsers =
                new Lazy<Funnel<string, Density>>(() => new Funnel<string, Density>(
                    new Parser<Density>(@"^(\d+(\.\d+)?) ?((Kg|k|K|kg)(/|per)(m^3))$",
                        m => new Density(double.Parse(m.Groups[1].Value), KilogramsPerMeterCb)),
                    new Parser<Density>(@"^(\d+(\.\d+)?) ?((T|Tonnes)(/|per)(m^3))$",
                        m => new Density(double.Parse(m.Groups[1].Value), TonnesPerMeterCb))));
            public static Density Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Density()
            {
                KilogramsPerMeterCb = new UnitofMeasurment<Density>("Kg/m^3", 1);
                TonnesPerMeterCb = new UnitofMeasurment<Density>("Kg/m^3", 0.001);
            }
            public override double Arbitrary { get; }
            public static Density operator -(Density a)
            {
                return (-1.0 * a);
            }
            public static Density operator *(Density a, double b)
            {
                return new Density(a.InUnits(KilogramsPerMeterCb) * b, KilogramsPerMeterCb);
            }
            public static Density operator *(double b, Density a)
            {
                return a * b;
            }
            public static Density operator /(Density a, double b)
            {
                return a * (1 / b);
            }
            public static Density operator +(Density a, Density b)
            {
                return new Density(a.InUnits(KilogramsPerMeterCb) + b.InUnits(KilogramsPerMeterCb), KilogramsPerMeterCb);
            }
            public static Density operator -(Density a, Density b)
            {
                return a + (-b);
            }
            public static double operator /(Density a, Density b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Mass.Mass operator /(Volume.Volume b, Density a)
            {
                return new Mass.Mass(b.InUnits(Volume.Volume.MeterCb) / a.InUnits(KilogramsPerMeterCb), Mass.Mass.Kilogram);
            }
            public static Volume.Volume operator *(Density a, Mass.Mass b)
            {
                return new Volume.Volume(a.InUnits(KilogramsPerMeterCb) * b.InUnits(Mass.Mass.Kilogram), Volume.Volume.MeterCb);
            }
            public static Volume.Volume operator *(Mass.Mass b, Density a)
            {
                return a * b;
            }
            public static Acceleration.Acceleration operator /(Density a, TimeSpan b)
            {
                return new Acceleration.Acceleration(a.InUnits(KilogramsPerMeterCb) / b.TotalSeconds, Acceleration.Acceleration.MetersPerSecondSquared);
            }
            public Density(double a, UnitofMeasurment<Density> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Density)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Density)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Speed
    {
        public class Speed : Measurement
        {
            public static class Quantities
            {
                public static readonly Speed SpeedingBullet = new Speed(381, MetersPerSecond);
                public static readonly Speed SpeedingCar = new Speed(90, KilometersPerHour);
                public static readonly Speed HumanRunning = new Speed(15, MilesPerHour);
                public static readonly Speed RecordHumanRunning = new Speed(44.7, KilometersPerHour);
                public static readonly Speed HumanWalking = new Speed(5, KilometersPerHour);
                /// <summary>
                /// in 0% humidity, 20 Celsius
                /// </summary>
                public static readonly Speed Sound = new Speed(343.2, MetersPerSecond);
                public static readonly Speed LightSpeed = new Speed(1, SpeedofLight);
            }
            public static readonly UnitofMeasurment<Speed> KilometersPerHour, MetersPerSecond, MilesPerHour, SpeedofLight, FeetPerSecond, Knot;
            private static readonly Lazy<Funnel<string, Speed>> DefaultParsers =
            new Lazy<Funnel<string, Speed>>(() => new Funnel<string, Speed>(
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?(mph|m/h)$", m => new Speed(double.Parse(m.Groups[1].Value), MilesPerHour)),
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?(mps|m/s)$", m => new Speed(double.Parse(m.Groups[1].Value), MetersPerSecond)),
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?(kmph|kph|km\\h|k\\h)$", m => new Speed(double.Parse(m.Groups[1].Value), KilometersPerHour)),
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?(c)$", m => new Speed(double.Parse(m.Groups[1].Value), SpeedofLight)),
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?(fps|f/s)$", m => new Speed(double.Parse(m.Groups[1].Value), FeetPerSecond)),
                new Parser<Speed>(@"^(\d+(\.\d+)?) ?((K|k)nots?)$", m => new Speed(double.Parse(m.Groups[1].Value), Knot))
            ));
            public static Speed Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Speed()
            {
                KilometersPerHour = new UnitofMeasurment<Speed>("Km/h", 1079252848.8);
                MetersPerSecond = new UnitofMeasurment<Speed>("m/s", 299792458.0);
                MilesPerHour = new UnitofMeasurment<Speed>("Mph", 670616629.384);
                SpeedofLight = new UnitofMeasurment<Speed>("c", 1);
                FeetPerSecond = new UnitofMeasurment<Speed>("feet/second", 9.83571056430446194225E8);
                Knot = new UnitofMeasurment<Speed>("knots", 5.827499183585313174946E8);
            }
            public override double Arbitrary { get; }
            public static Speed operator -(Speed a)
            {
                return (-1.0 * a);
            }
            public static Speed operator *(Speed a, double b)
            {
                return new Speed(a.InUnits(SpeedofLight) * b, SpeedofLight);
            }
            public static Speed operator *(double b, Speed a)
            {
                return a * b;
            }
            public static Speed operator /(Speed a, double b)
            {
                return a * (1 / b);
            }
            public static Speed operator +(Speed a, Speed b)
            {
                return new Speed(a.InUnits(SpeedofLight) + b.InUnits(SpeedofLight), SpeedofLight);
            }
            public static Speed operator -(Speed a, Speed b)
            {
                return a + (-b);
            }
            public static Momentum.Momentum operator *(Speed a, Mass.Mass b)
            {
                return new Momentum.Momentum(a.InUnits(MetersPerSecond) * b.InUnits(Mass.Mass.Kilogram), Momentum.Momentum.NewtonSecond);
            }
            public static Momentum.Momentum operator *(Mass.Mass b, Speed a)
            {
                return new Momentum.Momentum(a.InUnits(MetersPerSecond) * b.InUnits(Mass.Mass.Kilogram), Momentum.Momentum.NewtonSecond);
            }
            public static double operator /(Speed a, Speed b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Length operator *(Speed a, TimeSpan b)
            {
                return new Length(a.InUnits(MetersPerSecond) * b.TotalSeconds, Length.Meter);
            }
            public static Length operator *(TimeSpan b, Speed a)
            {
                return a * b;
            }
            public static TimeSpan operator /(Length a, Speed b)
            {
                return TimeSpan.FromSeconds(a.InUnits(Length.Meter) / b.InUnits(MetersPerSecond));
            }
            public static Acceleration.Acceleration operator /(Speed a, TimeSpan b)
            {
                return new Acceleration.Acceleration(a.InUnits(MetersPerSecond) / b.TotalSeconds, Acceleration.Acceleration.MetersPerSecondSquared);
            }
            public Speed(double a, UnitofMeasurment<Speed> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Speed)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Speed)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
            public RelativisticSpeed toRelativistic()
            {
                return new RelativisticSpeed(Arbitrary, SpeedofLight);
            }
        }
        public class RelativisticSpeed : Speed
        {
            public RelativisticSpeed(double a, UnitofMeasurment<Speed> u) : base(a, u) { }
            public static Speed operator -(RelativisticSpeed a)
            {
                return (-1.0 * a);
            }
            public static Speed operator +(RelativisticSpeed a, RelativisticSpeed b)
            {
                return new Speed((a.InUnits(SpeedofLight) + b.InUnits(SpeedofLight)) / (1 + a.Arbitrary * b.Arbitrary), SpeedofLight);
            }
            public static Speed operator -(RelativisticSpeed a, RelativisticSpeed b)
            {
                return a + (-b);
            }
            public static double operator /(RelativisticSpeed a, RelativisticSpeed b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
        }
    }
    namespace RotationalSpeed
    {
        public class RotationalSpeed : Measurement
        {
            public static class Quantities
            {}
            public static readonly UnitofMeasurment<RotationalSpeed> DegreesPerSecond, TurnsPerSecond, RadiansPerSecond, RoundsPerMinute;
            private static readonly Lazy<Funnel<string, RotationalSpeed>> DefaultParsers =
            new Lazy<Funnel<string, RotationalSpeed>>(() => new Funnel<string, RotationalSpeed>(
                new Parser<RotationalSpeed>(@"^(\d+(\.\d+)?) ?(d\s|dPs)$", m => new RotationalSpeed(double.Parse(m.Groups[1].Value), DegreesPerSecond)),
                new Parser<RotationalSpeed>(@"^(\d+(\.\d+)?) ?(tpS|t\s)$", m => new RotationalSpeed(double.Parse(m.Groups[1].Value), TurnsPerSecond)),
                new Parser<RotationalSpeed>(@"^(\d+(\.\d+)?) ?(rPs|r\s)$", m => new RotationalSpeed(double.Parse(m.Groups[1].Value), RadiansPerSecond)),
                new Parser<RotationalSpeed>(@"^(\d+(\.\d+)?) ?(rpm)$", m => new RotationalSpeed(double.Parse(m.Groups[1].Value), RoundsPerMinute))
            ));
            public static RotationalSpeed Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static RotationalSpeed()
            {
                TurnsPerSecond = new UnitofMeasurment<RotationalSpeed>("tps",1);
                DegreesPerSecond = new UnitofMeasurment<RotationalSpeed>("dps",360);
                RadiansPerSecond = new UnitofMeasurment<RotationalSpeed>("rps",2*Math.PI);
                RoundsPerMinute = new UnitofMeasurment<RotationalSpeed>("rpm",1.0/60);
            }
            public override double Arbitrary { get; }
            public static RotationalSpeed operator -(RotationalSpeed a)
            {
                return (-1.0 * a);
            }
            public static RotationalSpeed operator *(RotationalSpeed a, double b)
            {
                return new RotationalSpeed(a.InUnits(TurnsPerSecond) * b, TurnsPerSecond);
            }
            public static RotationalSpeed operator *(double b, RotationalSpeed a)
            {
                return a * b;
            }
            public static RotationalSpeed operator /(RotationalSpeed a, double b)
            {
                return a * (1 / b);
            }
            public static RotationalSpeed operator +(RotationalSpeed a, RotationalSpeed b)
            {
                return new RotationalSpeed(a.InUnits(TurnsPerSecond) + b.InUnits(TurnsPerSecond), TurnsPerSecond);
            }
            public static RotationalSpeed operator -(RotationalSpeed a, RotationalSpeed b)
            {
                return a + (-b);
            }
            public static double operator /(RotationalSpeed a, RotationalSpeed b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Angle.Angle operator *(RotationalSpeed a, TimeSpan b)
            {
                return new Angle.Angle(a.InUnits(DegreesPerSecond) * b.TotalSeconds, Angle.Angle.Degree);
            }
            public static Angle.Angle operator *(TimeSpan b, RotationalSpeed a)
            {
                return a * b;
            }
            public static TimeSpan operator /(Angle.Angle a, RotationalSpeed b)
            {
                return TimeSpan.FromSeconds(a.InUnits(Angle.Angle.Degree) / b.InUnits(DegreesPerSecond));
            }
            public RotationalSpeed(double a, UnitofMeasurment<RotationalSpeed> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((RotationalSpeed)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((RotationalSpeed)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Acceleration
    {
        public class Acceleration : Measurement
        {
            public static class Quantities
            {
                public static readonly Acceleration Gravity = new Acceleration(9.80665, MetersPerSecondSquared);
            }
            public static readonly UnitofMeasurment<Acceleration> MetersPerSecondSquared, FeetPerSecondSquared, Gal;
            private static readonly Lazy<Funnel<string, Acceleration>> DefaultParsers =
            new Lazy<Funnel<string, Acceleration>>(() => new Funnel<string, Acceleration>(
                new Parser<Acceleration>(@"^(\d+(\.\d+)?) ?(mps^2|m/s^2|ms^-2)$", m => new Acceleration(double.Parse(m.Groups[1].Value), MetersPerSecondSquared)),
                new Parser<Acceleration>(@"^(\d+(\.\d+)?) ?(gal|cmps^2|cm/s^2|cms^-2)$", m => new Acceleration(double.Parse(m.Groups[1].Value), Gal)),
                new Parser<Acceleration>(@"^(\d+(\.\d+)?) ?(fps^2|f/s^2)$", m => new Acceleration(double.Parse(m.Groups[1].Value), FeetPerSecondSquared))
            ));
            public static Acceleration Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Acceleration()
            {
                MetersPerSecondSquared = new UnitofMeasurment<Acceleration>("m/s^2", 1);
                Gal = new UnitofMeasurment<Acceleration>("gal", 100);
                FeetPerSecondSquared = new UnitofMeasurment<Acceleration>("f/s^2", 30.48);
            }
            public override double Arbitrary { get; }
            public static Acceleration operator -(Acceleration a)
            {
                return (-1.0 * a);
            }
            public static Acceleration operator *(Acceleration a, double b)
            {
                return new Acceleration(a.InUnits(MetersPerSecondSquared) * b, MetersPerSecondSquared);
            }
            public static Acceleration operator *(double b, Acceleration a)
            {
                return a * b;
            }
            public static Acceleration operator /(Acceleration a, double b)
            {
                return a * (1 / b);
            }
            public static Acceleration operator +(Acceleration a, Acceleration b)
            {
                return new Acceleration(a.InUnits(MetersPerSecondSquared) + b.InUnits(MetersPerSecondSquared), MetersPerSecondSquared);
            }
            public static Acceleration operator -(Acceleration a, Acceleration b)
            {
                return a + (-b);
            }
            public static double operator /(Acceleration a, Acceleration b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Speed.Speed operator *(Acceleration a, TimeSpan b)
            {
                return new Speed.Speed(a.InUnits(MetersPerSecondSquared) * b.TotalSeconds, Speed.Speed.MetersPerSecond);
            }
            public static Speed.Speed operator *(TimeSpan b, Acceleration a)
            {
                return a * b;
            }
            public static Force.Force operator *(Acceleration a, Mass.Mass b)
            {
                return new Force.Force(a.InUnits(MetersPerSecondSquared) * b.InUnits(Mass.Mass.Kilogram), Force.Force.Newton);
            }
            public Acceleration(double a, UnitofMeasurment<Acceleration> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Acceleration)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Acceleration)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Force
    {
        public class Force : Measurement
        {
            public static readonly UnitofMeasurment<Force> Newton, Dyne, Kilopond, PoundForce, Poundal;
            private static readonly Lazy<Funnel<string, Force>> DefaultParsers =
            new Lazy<Funnel<string, Force>>(() => new Funnel<string, Force>(
                new Parser<Force>(@"^(\d+(\.\d+)?) ?(N|(N|n)ewtons?)$", m => new Force(double.Parse(m.Groups[1].Value), Newton)),
                new Parser<Force>(@"^(\d+(\.\d+)?) ?(Dyn|(D|d)ynes?)$", m => new Force(double.Parse(m.Groups[1].Value), Dyne)),
                new Parser<Force>(@"^(\d+(\.\d+)?) ?(kgf|kp)$", m => new Force(double.Parse(m.Groups[1].Value), Kilopond)),
                new Parser<Force>(@"^(\d+(\.\d+)?) ?(lbf?)$", m => new Force(double.Parse(m.Groups[1].Value), PoundForce)),
                new Parser<Force>(@"^(\d+(\.\d+)?) ?(pdl)$", m => new Force(double.Parse(m.Groups[1].Value), Poundal))
            ));
            public static Force Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Force()
            {
                Newton = new UnitofMeasurment<Force>("N", 1);
                Dyne = new UnitofMeasurment<Force>("Dyn", 1E5);
                Kilopond = new UnitofMeasurment<Force>("Kp", 0.10197);
                PoundForce = new UnitofMeasurment<Force>("lbf", 0.22481);
                Poundal = new UnitofMeasurment<Force>("pdl", 7.2330);
            }
            public override double Arbitrary { get; }
            public static Force operator -(Force a)
            {
                return (-1.0 * a);
            }
            public static Force operator *(Force a, double b)
            {
                return new Force(a.InUnits(Newton) * b, Newton);
            }
            public static Force operator *(double b, Force a)
            {
                return a * b;
            }
            public static Force operator /(Force a, double b)
            {
                return a * (1 / b);
            }
            public static Force operator +(Force a, Force b)
            {
                return new Force(a.InUnits(Newton) + b.InUnits(Newton), Newton);
            }
            public static Force operator -(Force a, Force b)
            {
                return a + (-b);
            }
            public static double operator /(Force a, Force b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static Acceleration.Acceleration operator /(Force a, Mass.Mass b)
            {
                return new Acceleration.Acceleration(a.InUnits(Newton) / b.InUnits(Mass.Mass.Kilogram), Acceleration.Acceleration.MetersPerSecondSquared);
            }
            public Force(double a, UnitofMeasurment<Force> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Force)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Force)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
    namespace Momentum
    {
        public class Momentum : Measurement
        {
            public static readonly UnitofMeasurment<Momentum> NewtonSecond;
            private static readonly Lazy<Funnel<string, Momentum>> DefaultParsers =
            new Lazy<Funnel<string, Momentum>>(() => new Funnel<string, Momentum>(
                new Parser<Momentum>(@"^(\d+(\.\d+)?) ?(Ns)$", m => new Momentum(double.Parse(m.Groups[1].Value), NewtonSecond))
            ));
            public static Momentum Parse(string s)
            {
                return DefaultParsers.Value.Process(s);
            }
            static Momentum()
            {
                NewtonSecond = new UnitofMeasurment<Momentum>("N*s", 1);
            }
            public override double Arbitrary { get; }
            public static Momentum operator -(Momentum a)
            {
                return (-1.0 * a);
            }
            public static Momentum operator *(Momentum a, double b)
            {
                return new Momentum(a.InUnits(NewtonSecond) * b, NewtonSecond);
            }
            public static Momentum operator *(double b, Momentum a)
            {
                return a * b;
            }
            public static Momentum operator /(Momentum a, double b)
            {
                return a * (1 / b);
            }
            public static Momentum operator +(Momentum a, Momentum b)
            {
                return new Momentum(a.InUnits(NewtonSecond) + b.InUnits(NewtonSecond), NewtonSecond);
            }
            public static Momentum operator -(Momentum a, Momentum b)
            {
                return a + (-b);
            }
            public static double operator /(Momentum a, Momentum b)
            {
                return a.Arbitrary / b.Arbitrary;
            }
            public static TimeSpan operator /(Momentum a, Mass.Mass b)
            {
                return TimeSpan.FromSeconds(a.InUnits(NewtonSecond) / b.InUnits(Mass.Mass.Kilogram));
            }
            public static Mass.Mass operator /(Momentum a, Speed.Speed b)
            {
                return new Mass.Mass(a.InUnits(NewtonSecond) / b.InUnits(Speed.Speed.MetersPerSecond), Mass.Mass.Kilogram);
            }
            public static Force.Force operator /(Momentum a, TimeSpan b)
            {
                return new Force.Force(a.InUnits(NewtonSecond) / b.TotalSeconds, Force.Force.Newton);
            }
            public Momentum(double a, UnitofMeasurment<Momentum> u)
            {
                this.Arbitrary = u.toArbitrary(a);
            }
            public override int CompareTo(object a)
            {
                return (int)(this.Arbitrary - ((Momentum)a).Arbitrary);
            }
            public override bool Equals(object obj)
            {
                return ((Momentum)obj).Arbitrary == this.Arbitrary;
            }
            public override int GetHashCode()
            {
                return this.Arbitrary.GetHashCode();
            }
        }
    }
}
