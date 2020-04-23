using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Genies.DbSchemaImport
{
	public class MetaInfoColumn : MetaInfoBaseTableOwned, IAttributeTypeDefinitionImported
	{
		public MetaInfoColumn()
		{
			this.Length = 0;
			this.PersistentLength = 0;
			this.Precision = 0;
			this.CollectionType = CollectionType.None;
		}

		public IScalarType Type { get; internal set; }
		public string PersistentTypeName { get; internal set; }
		public int PersistentLength { get; internal set; }
		public string CompletePersistentTypeName
		{
			get
			{
				if (PersistentLength > 0)
				{
					return String.Format("{0}({1}{2})",
					                     PersistentTypeName,
					                     PersistentLength,
					                     HasPrecision ? "," + Precision.ToString() : "");
				}
				return PersistentTypeName;
			}
		}


		public bool Nullable
		{
			get { return !this.Required; }
			set { this.Required = !value; }
		}

		#region IAttributeTypeDefinitionImported implementation
		public bool HasLength { get { return Length > 0; } }

		public int Length { get; set; }

		public bool HasPrecision { get { return Precision > 0; } }

		public int Precision { get; set; }

		public bool HasFixed { get { return true; } }

		public bool Fixed { get; set; }

		public bool HasRequired { get { return true; } }

		public bool Required { get; set; }

		public bool HasDefault { get { return true; } }

		public string Default { get; set; }

		public CollectionType CollectionType { get; set; }
		#endregion
	}
}

