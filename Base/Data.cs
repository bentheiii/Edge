using System;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using Edge.Looping;
using Edge.SystemExtensions;
using LiteDB;

namespace Edge.Data
{
	public static class XPathMarksman
	{
		public static XmlNodeList SelectNodes(this XmlNode @this, string query, out Exception error, params string[] namespaces)
		{
			error = null;
			XmlNamespaceManager xnm = new XmlNamespaceManager(@this.OwnerDocument.NameTable);
			foreach (var ns in namespaces.Group2())
			{
				xnm.AddNamespace(ns.Item1,ns.Item2);
			}
			try
			{
				return @this.SelectNodes(query, xnm);
			}
			catch (Exception er)
			{
				error = er;
			}
			return null;
		}
		public static XmlNode SelectSingleNode(this XmlNode @this, string query, out Exception error, params string[] namespaces)
		{
			error = null;
			XmlNamespaceManager xnm = new XmlNamespaceManager(@this.OwnerDocument.NameTable);
			foreach (var ns in namespaces.Group2())
			{
				xnm.AddNamespace(ns.Item1, ns.Item2);
			}
			try
			{
				return @this.SelectSingleNode(query, xnm);
			}
			catch (Exception er)
			{
				error = er;
			}
			return null;
		}
	    public static XmlDocument getDocFromString(string innertext)
	    {
	        XmlDocument ret = new XmlDocument();
	        ret.InnerText = innertext;
	        return ret;
	    }
	}
    public static class EntityManager
    {
        public static void Update<T>(this LiteCollection<T> @this, int id, Action<T> mutation) where T : new()
        {
            @this.Update(@this.FindById(id),mutation);
        }
        public static void Update<T>(this LiteCollection<T> @this, T e, Action<T> mutation) where T : new()
        {
            @this.Update(e.Mutate(mutation));
        }
    }
}
