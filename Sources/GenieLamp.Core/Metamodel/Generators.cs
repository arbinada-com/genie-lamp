using System;
using System.Xml;

namespace GenieLamp.Core.Metamodel
{
	class Generators : MetaObjectNamedCollection<IGenerator, Generator>, IGenerators
	{
		public Generators(Model model)
			: base(model)
		{
		}

		public void AddList(XmlNodeList generatorsList)
		{
			this.AddList(null, generatorsList);
		}

		/// <summary>
		/// Adds generator metaobjects from xml definitions list.
		/// If list is null or empty do nothing.
		/// </summary>
		/// <param name='entity'>
		/// Entity.
		/// </param>
		/// <param name='generatorsList'>
		/// Generators list.
		/// </param>
		public void AddList(Entity entity, XmlNodeList generatorsList)
		{
			if (generatorsList == null)
				return;
			foreach(XmlNode generatorNode in generatorsList) {
				Generator gen = 
					entity == null ? 
						new Generator(Model, generatorNode) : 
						new Generator(Model, entity, generatorNode);
				this.Add(gen);
			}
		}
		
		public void Update()
		{
			foreach(Generator gen in this)
				gen.Update();
		}
	}
}

