using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using CCDefault.Annotations;

namespace Edge.Net
{
    public static class WebGuard
    {
	    [CanBeNull]
	    public static string getPageTitle(string url)
        {
            Exception proxy;
            return getPageTitle(url, out proxy);
        }
	    [CanBeNull]
	    public static string getPageTitle(string url,[CanBeNull] out Exception error)
        {
            try
            {
                WebClient x = new WebClient();
                string source = x.DownloadString(url);
                string ret = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
				error = null;
                return ret;
            }
            catch (Exception ex)
            {
				error = ex;
	            return null;
            }
        }
	    public static XmlDocument LoadXmlDocumentFromUrl(string url, [CanBeNull] out Exception error)
	    {
		    try
		    {
				XmlDocument ret = new XmlDocument();
			    ret.Load(url);
			    error = null;
			    return ret;
		    }
		    catch (Exception e)
		    {
			    error = e;
				return null;
		    }
	    }
    }
}
