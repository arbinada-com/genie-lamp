using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Utils
{
	public static class Xml
	{
		public class XmlDocHelper : IDocHelper
		{
			private XmlDocument document;

			public XmlNode CurrentNode { get; set; }

			#region Constructors
			private XmlDocHelper()
			{
			}
	        
			public XmlDocHelper(XmlDocument doc)
			{
				document = doc;
			}
	
			public XmlDocHelper(string rootName)
			{
				document = new XmlDocument();
				document.InsertBefore(
	                document.CreateXmlDeclaration("1.0", "utf-8", String.Empty),
	                document.DocumentElement);
				CurrentNode = document.CreateElement(rootName);
				document.AppendChild(CurrentNode);
			}
	
			public XmlDocHelper(XmlDocument doc, XmlNode node) :
	            this(doc)
			{
				CurrentNode = node;
			}
			#endregion
	
			#region IDocHelper implementation
			public XmlAttribute AddAttribute(string name, string value)
			{
				return Utils.Xml.AddAttrib(document, CurrentNode, name, value);
			}
	
			public XmlDocument Document
			{
				get { return document; }
			}

			public XmlNode CreateElement(string name)
			{
				CurrentNode = this.Document.CreateElement(name);
				return CurrentNode;
			}

			public XmlNode CreateComment(string format, params object[] args)
			{
				return CreateComment(String.Format(format, args));
			}

			public XmlNode CreateComment(string comment)
			{
				XmlNode commentNode = Document.CreateComment(comment);
				if (CurrentNode != null)
					CurrentNode.AppendChild(commentNode);
				else
					Document.DocumentElement.AppendChild(commentNode);
				return commentNode;
			}

			public void SetCurrentToRoot()
			{
				CurrentNode = Document.DocumentElement;
			}

			public XmlNode AddFirstChild(XmlNode newChildNode)
			{
				if (CurrentNode == null)
					CurrentNode = Document.DocumentElement;
				if (Document.DocumentElement == null)
					throw new GlException("Xml document must have document element node");
				if (CurrentNode.FirstChild == null)
					return CurrentNode.AppendChild(newChildNode);
				else
					return CurrentNode.InsertBefore(newChildNode, CurrentNode.FirstChild);
			}
			#endregion
		}
		
		
		public static XmlAttribute AddAttrib(XmlDocument doc, XmlNode node, string name, string value)
		{
			XmlAttribute attr = doc.CreateAttribute(name);
			attr.Value = value;
			node.Attributes.Append(attr);
			return attr;
		}

		public static bool IsAttrExists(XmlNode node, string attributeName)
		{
			return (node.Attributes[attributeName] != null);
		}
		
		public static string GetAttrValue(XmlNode node, string attributeName)
		{
			XmlNode attr = node.Attributes[attributeName];
			if (attr == null)
			{
				throw new GlException("Attribute \"{0}\" is not found. Node \"{1}\"", attributeName, node.Name);
			}
			return attr.Value;
		}
		
		public static string GetAttrValue(XmlNode node, string attributeName, string defaultValue)
		{
			XmlNode attr = node.Attributes[attributeName];
			return attr == null ? defaultValue : attr.Value;
		}
		
		public static int GetAttrValue(XmlNode node, string attributeName, int defaultValue)
		{
			XmlNode attr = node.Attributes[attributeName];
			return attr == null ? defaultValue : int.Parse(attr.Value);
		}
		
		public static decimal GetAttrValue(XmlNode node, string attributeName, decimal defaultValue)
		{
			XmlNode attr = node.Attributes[attributeName];
			return attr == null ? defaultValue : decimal.Parse(attr.Value);
		}

		public static bool GetAttrValue(XmlNode node, string attributeName, bool defaultValue)
		{
			XmlNode attr = node.Attributes[attributeName];
			return attr == null ? defaultValue : bool.Parse(attr.Value);
		}


		public static string StripNamespaces(XmlNode node)
		{
			return StripNamespaces(node.InnerXml);
		}

		public static string StripNamespaces(string xml)
		{
			string pattern = @"[ ]+xmlns=\""[^""]+\""";
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(
				pattern,
				System.Text.RegularExpressions.RegexOptions.Multiline |
				System.Text.RegularExpressions.RegexOptions.IgnoreCase |
				System.Text.RegularExpressions.RegexOptions.CultureInvariant);
			return regex.Replace(xml, "");
		}

		/// <summary>
		/// Returns xmlnode text taken from iner Text or CData node
		/// </summary>
		/// <returns>
		/// The node text.
		/// </returns>
		/// <param name='node'>
		/// Node.
		/// </param>
		public static string GetNodeText(XmlNode node)
		{
			string text = node.InnerText;
			foreach(XmlNode childNode in node.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Text ||
				    childNode.NodeType == XmlNodeType.CDATA)
				{
					text = childNode.InnerText;
					break;
				}
			}
			return text;
		}
	}
	
	public static class Strings
	{
		public static string Replicate(string source, int count)
		{
			if (count <= 0)
				return String.Empty;
			if (count == 1)
				return source;
			StringBuilder sb = new StringBuilder(count * source.Length);
			for (int i = 0; i < count; i++)
				sb.Append(source);
			return sb.ToString();			
		}
	}


	public static class Text
	{
		public static string ReadFromResource(Assembly assembly, string ressourceId)
		{
			string assemblyName = assembly.GetName().Name;
			if (!ressourceId.StartsWith(assemblyName))
				ressourceId = String.Format("{0}.{1}", assemblyName, ressourceId);
			using (Stream stream = assembly.GetManifestResourceStream(ressourceId))
			{
		        using (StreamReader reader = new StreamReader(stream))
				{
	            	return reader.ReadToEnd();
				}
			}
		}
	}
	
	public static class Sys
	{
		public static string GetCallStack()
		{
			StringBuilder sb = new StringBuilder();
			StackTrace stackTrace = new StackTrace();
			StackFrame[] stackFrames = stackTrace.GetFrames();
			foreach (StackFrame stackFrame in stackFrames)
			{
				sb.Append(stackFrame.ToString() /*.GetMethod().Name*/);
			}
			return sb.ToString();
		}

		public static string ExpandEnvironmentVariables(string source)
		{
			/*
			 * Workaroud of bug #5169
			 * https://bugzilla.xamarin.com/show_bug.cgi?id=5169
			 * ExpandEnvironmentVariables() add % when 2 or more variables are in the same string and no substitution was applied.
			 */
			const char EnvVarSeparator = '%';
			StringBuilder target = new StringBuilder();
			StringBuilder variable = new StringBuilder();
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i].Equals(EnvVarSeparator))
				{
					variable.Append(source[i]);
					if (variable.Length > 1)
					{
						target.Append(System.Environment.ExpandEnvironmentVariables(variable.ToString()));
						variable.Clear();
					}
				}
				else
				{
					if (variable.Length > 0)
					{
						variable.Append(source[i]);
					}
					else
					{
						target.Append(source[i]);
					}
				}
			}

			target.Append(variable);
			return target.ToString();
		}
	}
		
}

