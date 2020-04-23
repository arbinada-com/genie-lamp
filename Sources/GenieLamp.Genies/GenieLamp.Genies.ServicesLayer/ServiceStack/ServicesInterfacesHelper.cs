using System;

using GenieLamp.Core.Metamodel;
using GenieLamp.Core;
using GenieLamp.Core.CodeWriters.CSharp;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class ServicesInterfacesHelper : HelperBase
	{
		public ServicesInterfacesHelper(ICodeWriterCSharp cw, IEnvironmentHelper environment)
			: base(cw, environment)
		{
		}

		public void WritePaginationProperties(IEntity entity)
		{
			WriteProperty("int?",
			              NamingHelperStack.ParamName_PageNumber);
			WriteProperty("int?",
			              NamingHelperStack.ParamName_PageSize);
			WriteProperty("string",
			              NamingHelperStack.ParamName_SortOrderProperty);
			WriteProperty("bool?",
			              NamingHelperStack.ParamName_SortOrderAsc);
		}


		public void WriteProperties(IAttributes attributes)
		{
			foreach(IAttribute a in attributes)
			{
				if (!a.Processed)
				{
					WriteProperty(a);
				}
			}
		}

		public void WriteProperty(IAttribute attribute)
		{
			WriteProperty(environment.ToTypeName(attribute, false, true),
			              attribute.Name);
			attribute.Processed = true;
		}

		public void WriteProperty(string typeName, string name)
		{
    		cw.WriteLine("[DataMember]");
			cw.SimpleProperty(AccessLevel.Public,
			                  VirtualisationLevel.None,
			                  typeName,
			                  name,
			                  true, true);
		}
	}
}

