using System;
using System.IO;
using System.Text;
using System.Xml;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.Assistants
{
	public class SimpleDocAssistant : IGenieAssistant
	{
		private StringBuilder doc = new StringBuilder();
		
		#region Constructors
		public SimpleDocAssistant()
		{ }
		#endregion
		
		public IGenieAssistantConfig Config { get; private set; }
		public IGenie Master { get; private set; }
		public string OutFile { get; private set; }

		#region IGenieAssistant implementation
		public void Init(IGenieAssistantConfig config, IGenie master)
		{
			this.Config = config;
			this.Master = master;
			this.OutFile = config.Params.ParamByName("outFile", true).Value;
		}

		public void HandleEvent(string eventName, IMetaObject sender, string text, params object[] args)
		{
			switch(eventName.ToLower())
			{
			case "create":
				WriteMetaobjectDoc(sender, text);
				break;
			case "finished":
		        using (StreamWriter sw = new StreamWriter(OutFile))
		        {
		            sw.Write(doc.ToString());
		        }
				break;
			default:
				break;
			}
		}
		#endregion
		
		private void WriteMetaobjectDoc(IMetaObject metaObject, string text)
		{
			if (metaObject.HasDoc)
				doc.AppendLine(metaObject.Doc.Text);
		}
	}
}

