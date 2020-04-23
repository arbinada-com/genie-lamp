using System;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Core.Metamodel
{
	class AttributeMigration : IAttributeMigration
	{
		private string name;
		private Attribute source;
		private Attribute owner;

		#region Constructors
		public AttributeMigration(Attribute owner, Attribute source)
		{
			this.owner = owner;
			if (source == null)
				throw new GlException("Cannot migrate attribute: source is null. Target entity: {0}", owner);
			if (owner.IsDeclared || owner.IsPrimaryId)
				this.name = owner.Name;
			else
				this.name = String.Format("{0}{1}", owner.Name, source.Name);
			this.source = source;
		}
		#endregion

		public Attribute Owner
		{
			get { return this.owner; }
		}

		public Entity RelatedEntity
		{
			get { return this.source.Entity; }
		}

		public override string ToString()
		{
			return string.Format("(Migration.Name: {0})", Name);
		}

		public Attribute Source
		{
			get { return this.source; }
		}

		#region IMigratedAttribute implementation
		IEntity IAttributeMigration.RelatedEntity
		{
			get { return this.RelatedEntity; }
		}

		public string Name
		{
			get { return this.name; }
			internal set { name = value; }
		}

		IAttribute IAttributeMigration.Source
		{
			get { return this.Source; }
		}
		#endregion
	}
}

