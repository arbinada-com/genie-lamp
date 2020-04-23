using System;

using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoGenerator : MetaInfoBaseTableOwned
	{
		public MetaInfoGenerator()
		{
		}

		public string MinValue { get; set; }
		public string MaxValue { get; set; }
		public string Increment { get; set; }
		public GeneratorType Type { get; set; }
	}
}

