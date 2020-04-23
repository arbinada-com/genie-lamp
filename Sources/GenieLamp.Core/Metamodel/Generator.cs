using System;
using System.Xml;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class Generator : MetaObjectSchemed, IGenerator
	{
		private Persistence persistence;
		private GeneratorType type = GeneratorType.Sequence;
		private Entity entity;
		private int startWith = 1;
		private int increment = 1;
		private bool hasMinValue = false;
		private decimal minValue = Int32.MinValue;
		private bool hasMaxValue = false;
		private decimal maxValue = Int32.MaxValue;
		
		#region Constructors
		public Generator(Model model, XmlNode node)
			: base(model, node)
		{
			Init(null, node);
		}
			
		public Generator(Model model, Entity owner, XmlNode node)
			: base(owner, node)
		{
			Init(owner, node);
		}
		
		internal Generator(Entity owner)
			: base(owner, owner.Schema, Generator.DefaultName(owner))
		{
			Init(owner, null);
		}

		private void Init(Entity entity, XmlNode node)
		{
			this.entity = entity;
			if (node != null)
			{
				type = Parse(Utils.Xml.GetAttrValue(node, "type"));
				startWith = Utils.Xml.GetAttrValue(node, "startWith", startWith);
				increment = Utils.Xml.GetAttrValue(node, "startWith", increment);
				hasMaxValue = Utils.Xml.IsAttrExists(node, "maxValue");
				maxValue = Utils.Xml.GetAttrValue(node, "maxValue", maxValue);
				persistence = new Persistence(this, node);
			}
			else
			{
				persistence = new Persistence(this, entity.Persistence);
			}

			SetMinMax();
		}
		#endregion

		private void SetMinMax()
		{
			if (Increment > 0 && !HasMinValue)
				minValue = StartWith;
			if (Increment < 0 && !HasMaxValue)
				maxValue = StartWith;
		}

		public static string DefaultName(Entity entity)
		{
			return entity.Name;
		}

		public static string ToTypeName(GeneratorType type)
		{
			return Enum.GetName(typeof(GeneratorType), type).ToLower();
		}

		public static GeneratorType Parse(string genTypeName)
		{
			if (genTypeName.Equals("sequence", StringComparison.InvariantCultureIgnoreCase))
				return GeneratorType.Sequence;
			else if (genTypeName.Equals("guid", StringComparison.InvariantCultureIgnoreCase))
				return GeneratorType.Guid;
			else if (genTypeName.Equals("guid.hex", StringComparison.InvariantCultureIgnoreCase))
				return GeneratorType.GuidHex;
			throw new GlException("Unknown generator type: {0}", genTypeName);
		}
		
		public Persistence Persistence
		{
			get { return persistence; }
		}
		
		public void Update()
		{
			string persistentSchemaName = Model.DefaultPersistentSchema;
			if (entity != null)
			{
				persistentSchemaName = entity.Persistence.Schema;
			}
			// Naming
			string nameTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName("Generator.Template", Persistence.Name).Value;
			Persistence.UpdateNames(persistentSchemaName, Macro.Subst(nameTemplate));
		}

		#region IIdGenerator implementation
		public GeneratorType Type
		{
			get { return type; }
			internal set { type = value; }
		}

		IPersistence IPersistent.Persistence
		{
			get { return persistence; }
		}

		public int StartWith
		{
			get { return this.startWith; }
			internal set { startWith = value; }
		}

		public int Increment
		{
			get { return this.increment; }
			internal set
			{
				increment = value;
				SetMinMax();
			}
		}

		public bool HasMinValue
		{
			get { return this.hasMinValue; }
			internal set { hasMinValue = value; }
		}
		
		public decimal MinValue
		{
			get { return this.minValue; }
			internal set { minValue = value; }
		}

		public bool HasMaxValue
		{
			get { return this.hasMaxValue; }
			internal set { hasMaxValue = value; }
		}
		
		public decimal MaxValue
		{
			get { return this.maxValue; }
			internal set { maxValue = value; }
		}
		#endregion




	}
}

