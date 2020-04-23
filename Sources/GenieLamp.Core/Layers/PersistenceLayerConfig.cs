using System;
using System.Xml;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Layers
{
	class PersistenceLayerConfig : LayerConfig, IPersistenceLayerConfig
	{
		public const string LayerName = "Persistence";

		#region Constructors
		internal PersistenceLayerConfig(GenieLampConfig owner)
			: base(owner, LayerName)
		{
			Init();
		}

		public PersistenceLayerConfig(GenieLampConfig owner, XmlNode node)
			: base(owner, node)
		{
			Init();
			ParamSimple booleanValues = this.Params.GetByName("BooleanValues", false);
			if (booleanValues != null)
			{
				switch(booleanValues.Value.ToLower())
				{
				case "native":
					break;
				case "truefalse":
					this.BooleanValuePersistence = BooleanValuePersistence.TrueFalse;
					break;
				case "yesno":
					this.BooleanValuePersistence = BooleanValuePersistence.YesNo;
					break;
				}
			}
		}

		void Init()
		{
			this.BooleanValuePersistence = BooleanValuePersistence.Native;
			InitKeywords();
		}

		private void InitKeywords()
		{
			this.Keywords = new LayerKeywords(
				new string[]
			{
				"ALL",
				"DELETE",
				"FROM",
				"GROUP",
				"HAVING",
				"FOREIGN",
				"INDEX",
				"INSERT",
				"JOIN",
				"KEY",
				"MERGE",
				"NULL",
				"ORDER",
				"REFERENCES",
				"SELECT",
				"TRUNCATE",
				"UNION",
				"UPDATE",
				"WHERE"
			});
		}
		#endregion

		#region IPersistenceLayerConfig implementation
		public BooleanValuePersistence BooleanValuePersistence { get; internal set; }
		#endregion
	}
}

