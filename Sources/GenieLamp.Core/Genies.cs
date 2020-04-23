using System;
using System.Reflection;
using System.Collections.Generic;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core
{
	class Genies : IGenies
	{
        private GenieLamp lamp = null;
        private List<IGenie> genies = new List<IGenie>();

		#region Constructors
		public Genies(GenieLamp lamp, XmlNodeList nodes)
		{
            this.lamp = lamp;
			
			if (nodes == null || nodes.Count == 0)
				throw new GlException("No genies to spell...");

            lamp.Logger.TraceLine("Initializing genies");
            foreach (XmlNode genieNode in nodes)
            {
				GenieConfig genieConfig = new GenieConfig(lamp, genieNode);
                System.Type t = LoadPlugin(genieConfig.AssemblyName, genieConfig.TypeName);
				lamp.Logger.Trace(String.Format("{0}...", t.Name));
                try
                {
                    IGenie genie = Activator.CreateInstance(t) as IGenie;
					if (genie == null)
		                throw new GlException("Genie '{0}' doesn't implement {1}. Assembly: {2}. Type: {3}",
						                      genieConfig.Name,
						                      typeof(IGenie).Name,
						                      genieConfig.AssemblyName,
						                      genieConfig.TypeName);
                    genie.Init(genieConfig);
                    lamp.Logger.TraceLine(String.Format("OK ({0})", genie.Name));
                    genies.Add(genie);
					// Load assistants
					genieConfig.Assistants.Clear();
					foreach(GenieAssistantConfig assistantConfig in genieConfig.AssistantConfigs)
					{
		                System.Type t2 = LoadPlugin(assistantConfig.AssemblyName, assistantConfig.TypeName);
						lamp.Logger.Trace(String.Format("  Assistant: {0}...", t2.Name));
	                    IGenieAssistant genieAssistant = Activator.CreateInstance(t2) as IGenieAssistant;
						if (genieAssistant == null)
			                throw new GlException("Genie assistant '{0}' doesn't implement {1}. Assembly: {2}. Type: {3}",
							                      assistantConfig.Name,
							                      typeof(IGenieAssistant).Name,
							                      assistantConfig.AssemblyName,
							                      assistantConfig.TypeName);
	                    genieAssistant.Init(assistantConfig, genie);
						genieConfig.Assistants.Add(genieAssistant);
	                    lamp.Logger.TraceLine(String.Format("OK ({0})", assistantConfig.Name));
					}
                }
                catch (Exception)
                {
                    lamp.Logger.TraceLine("FAILED");
                    throw;
                }
            }
            lamp.Logger.TraceLine("All genies initialized OK");
		}
		#endregion

		private System.Type LoadPlugin(string assemblyName, string entryTypeName)
		{
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                throw new GlException("Assembly was not loaded: {0}", assemblyName);
            System.Type t = assembly.GetType(entryTypeName);
            if (t == null)
                throw new GlException("Assembly {0} does not contain type {1}",
				                      assemblyName,
				                      entryTypeName);
			return t;
		}

		public GenieLamp Lamp
		{
			get { return lamp; }
		}

        #region IGenies implementation
        public IGenie this[string name]
        {
            get
			{
				foreach(IGenie g in genies)
				{
					if (g.Name.Equals(name))
						return g;
				}
				throw new GlException("Genie {0} not found", name);
			}
        }
        #endregion


		#region IEnumerable[IGenie] implementation
		IEnumerator<IGenie> IEnumerable<IGenie>.GetEnumerator()
		{
			return genies.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return genies.GetEnumerator();
		}
		#endregion
	}


}

