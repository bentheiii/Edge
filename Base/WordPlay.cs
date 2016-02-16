using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Funnels;

namespace Edge.WordsPlay
{
    public static class WordPlay
    {
        public static string RegexDoubleNoSign = @"((\d+(\.\d+)?)((e|E)(\+|-)?\d+)?))";
        public static string RegexDouble = $@"((\+|-)?{RegexDoubleNoSign}";
        public static string ToString(this IFormattable @this, string format)
        {
            return @this.ToString(format, CultureInfo.CurrentCulture);
        }
        public static string Removespecific(this string x, string toremove)
        {
            int i = x.IndexOf(toremove, StringComparison.Ordinal);
            return i == -1 ? x : x.Remove(i, toremove.Length);
        }
        public static string Substringbetween(this string x, int start, int end)
        {
            if (start > end)
                throw new Exception("start must be before or at end");
            return x.Substring(start, end - start);
        }
        public static string Substringbetween(this string x, string start, string end, bool includestart = false)
        {
            int si = x.IndexOf(start, StringComparison.Ordinal);
            int ei = x.IndexOf(end, si, StringComparison.Ordinal);
            if (si == -1 || ei == -1)
                throw new Exception("both start and end must be in x");
            if (!includestart)
                si += start.Length;
            if (si > ei)
                throw new Exception("start must be before or at end");
            return x.Substringbetween(si, ei);
        }
        public static string Substringbetween(this string x, string start, bool includestart = false)
        {
            int si = x.IndexOf(start, StringComparison.Ordinal);
            if (si == -1)
                throw new Exception("start must be in x");
            if (!includestart)
                si += start.Length;
            return x.Substring(si);
        }
        public static string RemovefromEnd(this string x, int count)
        {
            if (count > x.Length)
                throw new Exception("count must be lower than string length");
            return count == 0 ? x : x.Remove(x.Length - count);
        }
        public static string convertToString(this IEnumerable<byte> x)
        {
            return new string(x.Select(a=>(char)a).ToArray());
        }
        public static string convertToString (this IEnumerable<char> x)
        {
			return new string(x.ToArray());
        }
        public static byte[] ToBytes(this string @this)
        {
            return @this.SelectToArray(a => (byte)a);
        }
        public static string pluralize(int c, string singular, string plural, bool includecount = false, bool pluralreplacesingle = false)
        {
            return pluralize((double)c, singular, plural, includecount, pluralreplacesingle);
        }
        public static string pluralize(double c, string singular, string plural, bool includecount = false, bool pluralreplacesingle = false)
        {
            string ret = "";
            if (includecount)
            {
                ret = c + " ";
            }
            if (c == 1 || !pluralreplacesingle)
            {
                ret += singular;
            }
            else
            {
                ret += plural;
            }
            return ret;
        }
        private static string ToRomanNumerals(int i, char ones, char fives, char tens)
        {
            switch (i)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    return new string(ones,i);
                case 4:
                    return ones + fives.ToString();
                case 5:
                case 6:
                case 7:
                case 8:
                    return fives + new string(ones, i-5);
                case 9:
                    return ones + tens.ToString();
                default:
                    return "";
            }
        }
        private static string ToRomanNumerals(double i, char twelfth, char half)
        {
            i %= 1;
            i *= 12;
            string ret = "";
            if (i >= 6)
            {
                ret = half.ToString();
                i -= 6;
            }
            ret += new string(twelfth,(int)i);
            return ret;
        }
        public static string ToRomanNumerals(this int i)
        {
            if (i == 0)
                return "N";
            if (i < 0)
                throw new Exception("Cannot get roman numerals of number less than zero");
            StringBuilder ret = new StringBuilder();
            ret.Append(new string('M', i / 1000));
            i %= 1000;
            ret.Append(ToRomanNumerals(i / 100, 'C', 'D', 'M'));
            i %= 100;
            ret.Append(ToRomanNumerals(i / 10, 'X', 'L', 'C'));
            i %= 10;
            ret.Append(ToRomanNumerals(i, 'I', 'V', 'X'));
            return ret.ToString();
        }
        public static string ToRomanNumerals(this double i)
        {
            return ((i>=1 || i <= (1/12.0)?((int) i).ToRomanNumerals():"") + ToRomanNumerals(i, '.', 'S'));
        }
        public static string[] SmartSplit(this string @this, string divisor, string opener, string closer)
        {
            if (!@this.Balanced(opener,closer,1))
                throw new ArgumentException("string is not balanced");
            ResizingArray<string> ret = new ResizingArray<string>();
            while (@this.StartsWith(opener))
            {
                ret.Add("");
                @this = @this.Substring(opener.Length);
            }
            int divindex = -2;
            int openerindex = -2;
            while (@this.Length!=0)
            {
                if (@this.StartsWith(opener))
                {
                    @this = @this.Substring(opener.Length);
                    int closerind = @this.IndexOf(closer);
                    ret.Add(@this.Substring(0,closerind));
                    @this = @this.Substring(closerind + closer.Length);
                    continue;
                }
                if (divindex <= -2)
                    divindex = @this.IndexOf(divisor);
                if (openerindex <= -2)
                    openerindex = @this.IndexOf(opener);
                if (divindex == -1 && openerindex == -1)
                {
                    ret.Add(@this);
                    break;
                }
                if (divindex == -1 || (openerindex != -1 && openerindex < divindex))
                {
                    ret.Add(@this.Substring(0, openerindex));
                    @this = @this.Substring(openerindex);
                    divindex -= openerindex;
                    openerindex = -2;
                }
                else
                {
                    ret.Add(@this.Substring(0, divindex));
                    @this = @this.Substring(divisor.Length + divindex);
                    openerindex -= (divindex + divisor.Length);
                    divindex = -2;
                }
            }
            return ret;
        }
    }
    namespace Parsing
    {
        public class Parser<T> : IProccesor<string, T>
        {
            private readonly string _query;
            private readonly Func<Match, T> _converter;
            public Parser(string q, Func<Match, T> c)
            {
                this._query = q;
                this._converter = c;
            }
            public bool tryParse(string s, [CanBeNull] out T u)
            {
                Match m = Regex.Match(s, this._query);
                u = m.Success ? this._converter(m) : default(T);
                return m.Success;
            }
            public Proccesor<string, T> toProcessor()
            {
                return tryParse;
            }
        }
    }
}