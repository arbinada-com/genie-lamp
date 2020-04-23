using System;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Utils;
using GenieLamp.Core.Metamodel.Physical;

namespace GenieLamp.Core.Metamodel
{
	class PrimaryId : EntityConstraint, IPrimaryId, IConsistency
	{
		private Generator generator;
		private Index index;

		#region Constructors
		public PrimaryId(Entity owner, XmlNode constraintNode) 
			: base(owner, constraintNode)
		{ 
			QualName qn = new QualName(constraintNode, "generatorSchema", "generator", owner.Schema);
			generator = Model.Generators.GetByName(qn.FullName);
			Init();
		}
		
		public PrimaryId(Entity owner, Attribute attribute) 
			: base(owner, attribute.Name)
		{
			Attributes.Add(attribute);
			if (!attribute.GeneratorName.IsEmpty)
				generator = Model.Generators.GetByName(attribute.GeneratorName.FullName, true, this);
			Init();
		}
		
		public PrimaryId(Entity owner, PrimaryId source)
			: base(owner, source.Name)
		{ 
			foreach(Attribute a in source.Attributes) {
				Attribute target = owner.Attributes.GetByName(a.FullName);
				if (target == null)
					throw new GlException("Cannot migrate primary id. Attribute \"{0}\" not found. Entity: {1}", 
					                      a.FullName, owner.FullName);
				Attributes.Add(target);
			}
			Init();
		}

		private void Init()
		{
			this.index = Model.PhysicalModel.Indexes.CreateIndex(this.Entity);
		}
		#endregion

		public void Update()
		{
			// Naming
			string constraintTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName("PrimaryKey.ConstraintTemplate", Persistence.Name).Value;
			Persistence.UpdateNames(Const.EmptyName, Macro.Subst(constraintTemplate));
			// Apply rule for non composite id only
			if (Attributes.Count == 1) {
				foreach(Attribute a in Attributes) {
					string attributeTemplate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName(
						"PrimaryKey.ColumnTemplate", a.Persistence.Name).Value;
					a.Persistence.UpdateNames(Const.EmptyName, Macro.Subst(attributeTemplate));
					a.Persistence.LockName();
				}
			}
			index.InitBy(this);
			index.Generate = Model.Lamp.Config.Layers.PersistenceConfig.NamingConvention.Params.ParamByName(
						"PrimaryKey.CreateIndex", "false").AsBool;
		}
		

		public Generator Generator
		{
			get { return this.generator; }
			internal set { this.generator = value; }
		}
		
		public Physical.Index Index
		{
			get { return this.index; }
		}

		public Metamodel.Attribute Attribute
		{
			get { return Attributes.Count > 0 ? Attributes.GetByIndex(0) :  null; }
		}

		#region IPrimaryIdConstraint implementation
		IGenerator IPrimaryId.Generator {
			get { return generator; }
		}

		public bool HasGenerator {
			get { return generator != null; }
		}

		Physical.IIndex IPrimaryId.Index
		{
			get { return this.index; }
		}
		#endregion
		
		#region IConsistency implementation
		public override void Check()
		{
			base.Check();
			if (Attributes.Count == 0)
				throw new GlException("Primary identifier \"{0}\" has no attributes. {1}", FullName, Entity);

			if (Attributes.Count > 1 && HasGenerator)
				throw new GlException("Primary identifier \"{0}\" is composite and cannot use generator {1}. {2}", 
				                      FullName, Generator.Name, Entity);
		}

		public bool IsAutoincrement
		{
			get
			{
				if (Attributes.Count == 1)
				{
					Attribute a = this.Attributes.GetByIndex(0);
					if (a.IsAutoincrement && a.Type is ScalarType && (a.Type as ScalarType).IsIntegerValueType)
						return true;
				}
				return false;
			}
		}
		#endregion








	}
}

