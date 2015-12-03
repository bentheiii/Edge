using System;
using System.Collections;
using System.Collections.Generic;
using Edge.Fielding;
using Edge.WordsPlay;

namespace Edge.Params
{
    public class ParamsProtocol
    {
        public const string DEFAULTDIVISOR = "&&_&&";
        public const string DEFAULTPARAMSYNSEPERATOR = "=_=";
        internal string Divisor;
        internal string Seperator;
        public ParamsProtocol(string divisor = DEFAULTDIVISOR, string seperator = DEFAULTPARAMSYNSEPERATOR)
        {
            this.Divisor = divisor;
            this.Seperator = seperator;
        }
        public override bool Equals(object obj)
        {
            ParamsProtocol paramsProtocol = obj as ParamsProtocol;
            if (paramsProtocol != null)
            {
                ParamsProtocol p = paramsProtocol;
                return p.Divisor.Equals(this.Divisor) && p.Seperator.Equals(this.Seperator);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.Divisor.GetHashCode() ^ this.Seperator.GetHashCode();
        }
    }
    public class Recoder : IEnumerable
    {
        private readonly Dictionary<string, string> _parameters;
        private readonly ParamsProtocol _mProtocol;
        public Recoder(ParamsProtocol p):this(p.Divisor,p.Seperator){}
        public Recoder(string divisor = ParamsProtocol.DEFAULTDIVISOR, string paramsynseperator = ParamsProtocol.DEFAULTPARAMSYNSEPERATOR)
        {
            this._mProtocol = new ParamsProtocol();
            this._mProtocol.Divisor = divisor;
            this._mProtocol.Seperator = paramsynseperator;
            this._parameters = new Dictionary<string, string>();
            this._mProtocol = new ParamsProtocol {Divisor = divisor, Seperator = paramsynseperator};
        }
        public string this[string i]
        {
            get
            {
                return !this._parameters.ContainsKey(i) ? null : this._parameters[i];
            }
            set
            {
                this._parameters.Add(i,value);
            }
        }
        public string recode()
        {
            string ret = "";
            foreach (KeyValuePair<string, string> pair in this._parameters)
            {
                ret = ret + this._mProtocol.Divisor + pair.Key + this._mProtocol.Seperator + pair.Value;
            }
            ret = ret.Substring(this._mProtocol.Divisor.Length);
            return ret;
        }
        public void Add(string i, string j)
        {
            this[i] = j;
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) this._parameters).GetEnumerator();
        }
    }
    /// <summary>
    /// <![CDATA[args default divisor is &&_&& DO NOT USE THIS AS PARAMATER NAME OR VALUE]]>
    /// <para>
    /// <![CDATA[format is : &&_&&param1name=param1value&&_&&param2name=param2value&&_&&param3name=param3value...]]>
    /// </para>
    /// <para>
    /// <![CDATA[do not use parameter syntax separator (default =_=) in parameter name, is okay in parameter value]]> 
    /// </para>
    /// </summary>
    public class Decoder
    {
	    private readonly IDictionary<string,string> _parameters;
        public Decoder()
        {
            _parameters = new Dictionary<string, string>();
            protocol = new ParamsProtocol();
        }
        public Decoder(string args, ParamsProtocol p):this(args,p.Divisor,p.Seperator){}
        public Decoder(string args, string divisor = ParamsProtocol.DEFAULTDIVISOR, string paramsynseperator = ParamsProtocol.DEFAULTPARAMSYNSEPERATOR)
        {
            this._parameters = new Dictionary<string, string>();
            this.protocol = new ParamsProtocol {Divisor = divisor, Seperator = paramsynseperator};
            string[] splitparams = args.truesplit(divisor);
            this.sortparams(splitparams);
        }
        private void sortparams(IEnumerable<string> splitargs)
        {
            foreach (string s in splitargs)
            {
                int sepindex = s.IndexOf(this.protocol.Seperator, StringComparison.Ordinal);
                if (sepindex < 1)
                {
                    continue;
                }
                string paramname = s.Substring(0, sepindex);
                string paramvalue = s.Substring(sepindex + this.protocol.Seperator.Length);
                this._parameters[paramname] = paramvalue;
            }
        }
        public string this[string identifier]
        {
            get
            {
                return !this._parameters.ContainsKey(identifier) ? null : this._parameters[identifier];
            }
        }
        public string recode()
        {
            string ret = "";
            foreach (KeyValuePair<string, string> pair in this._parameters)
            {
                ret = ret + this.protocol.Divisor + pair.Key + this.protocol.Seperator + pair.Value;
            }
            ret = ret.Substring(this.protocol.Divisor.Length);
            return ret;
        }
        public ParamsProtocol protocol { get; }
	    public ISet<string> getMissingParameters(params string[] parametersToLookFor)
	    {
		    ISet<string> ret = new HashSet<string>();
		    foreach (string s in parametersToLookFor)
		    {
			    if (this[s] == null)
					ret.Add(s);
		    }
		    return ret;
	    }
        public string Extract(string parameter, string defaultvalue)
        {
            return Extract(parameter, defaultvalue, a=>a);
        }
        public T Extract<T>(string parameter, T defaultvalue)
        {
            return Extract(parameter, defaultvalue, Fields.getField<T>().Parse);
        }
        public T Extract<T>(string parameter, T defaultvalue, Func<string,T> converter)
        {
            return this[parameter] == null ? defaultvalue : converter(this[parameter]);
        }
    }
}
