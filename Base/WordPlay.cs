using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Funnels;

namespace Edge.WordsPlay
{
    public static class WordPlay
    {
		[Flags]
        public enum LetterFilter {Lowercase=1, Uppercase=2, Numbers = 4, All = -1}
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
        public static string[] truesplit(this string a, string divisor = ",", bool removeempties = true, string empties = "")
        {
            return truesplit(a, new string[] { divisor }, removeempties, empties);
        }
        public static string[] truesplit(this string a, string[] divisors , bool removeempties = true, string empties = "")
        {
            string tempa = a;
            List<string> ret = new List<string>();
            while (tempa != "")
            {
                int divindex = -1, divlength = 0;
                foreach (string t1 in divisors)
                {
                    int t = tempa.IndexOf(t1, StringComparison.Ordinal);
                    if ((t < divindex || divindex == -1) && t != -1)
                    {
                        divindex = t;
                        divlength = t1.Length;
                    }
                }
                if (divindex == -1)
                {
                    ret.Add(tempa);
                    break;
                }
                ret.Add(tempa.Substring(0, divindex));
                tempa = tempa.Substring(divindex + divlength) ;
            }
            if (removeempties)
            {
                ret.RemoveAll(s => s.Equals(empties));
            }
            return ret.ToArray();
        }
        public static string truejoin(this string[] coll, string divider = " ", bool divideatstart = false, bool divideatend = false)
        {
	        return coll.ToPrintable(divider, divideatstart ? divider : "", divideatend ? divider : "");
        }
        public static bool isletter(this char x, LetterFilter filter = LetterFilter.Lowercase | LetterFilter.Uppercase)
        {
            return filter.HasFlag(LetterFilter.All)
                   || (x <= 'z' && x >= 'a' && filter.HasFlag(LetterFilter.Lowercase)
                   || (x <= 'Z' && x >= 'A' && filter.HasFlag(LetterFilter.Uppercase))
                   || (x <= '9' && x >= '0' && filter.HasFlag(LetterFilter.Numbers)));
        }
        public static bool isdigit(this char x)
        {
            return x >= '0' && x <= '9';
        }
        public static bool ispalindrome(string x)
        {
            for (int i = 0; i < x.Length/2; i++)
            {
                if (x[i] != x[x.Length - i])
                    return false;
            }
            return true;
        }
        public static string convertToString (this IEnumerable<char> x)
        {
			return new string(x.ToArray());
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
        public static bool isvalidemail(this string tocheck)
        {
            return Regex.Match(tocheck,@"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$").Success;
        }
        public static int countappearances(this string tosearch, string tofind)
        {
            return tosearch.getappearances(tofind).Length;
        }
        public static int[] getappearances(this string tosearch, string tofind)
        {
            List<int> ret = new List<int>();
            string t = tosearch;
            int ti = t.IndexOf(tofind, StringComparison.Ordinal);
            while (ti != -1)
            {
                ret.Add(ti);
                ti = t.IndexOf(tofind, ti + 1, StringComparison.Ordinal);
            }
            return ret.ToArray();
        }
        public static string stretchtoFill(this string tostretch, int newsize, char filler = ' ', bool addToRight = true)
        {
            if (tostretch.Length >= newsize)
                return tostretch;
	        if (addToRight)
		        return tostretch + new string(filler, newsize - tostretch.Length);
	        return new string(filler, newsize - tostretch.Length) + tostretch;
        }
        public static string Reverse(this string x)
        {
            char[] ret = new char[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                ret[i] = x[x.Length - i - 1];
            }
            return ret.convertToString();
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
        public static void SplitbyIndex(this string @this, int charindex, out string first, out string second)
        {
            first = @this.Substring(0, charindex);
            second = @this.Substring(charindex);
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
	    public static class ParseExtentions
	    {
		    public static T[] ParseCollection<T>(this string @this, Func<string, T> proccesor,string[] divisors, bool removeEmpties = true, string empties = "")
		    {
			    string[] split = @this.truesplit(divisors, removeEmpties, empties);
			    return split.SelectToArray(proccesor);
		    }
	    }
    }
}