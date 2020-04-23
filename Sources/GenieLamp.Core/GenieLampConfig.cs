using System;
using System.Xml;

using GenieLamp.Core.Utils;
using GenieLamp.Core.Layers;
using GenieLamp.Core.Patterns;

namespace GenieLamp.Core
{
	class GenieLampConfig : IGenieLampConfig
	{
		private GenieLamp lamp;
		private LayersConfig layers;
		private Patterns.Patterns patterns;
		private MacroExpander macro;

		#region Constructrs
		public GenieLampConfig(GenieLamp lamp, XmlNode node)
		{
			this.lamp = lamp;
			macro = new MacroExpander(lamp.Macro);
			layers = new LayersConfig(this, node);
			patterns = new Patterns.Patterns(this, node);
		}
		#endregion

		public void Update()
		{
			patterns.Update();
		}
		
		public GenieLamp Lamp
		{
			get { return lamp; }
		}
		
		public LayersConfig Layers
		{
			get { return layers; }
		}

		public Patterns.Patterns Patterns
		{
			get { return this.patterns; }
		}

		public MacroExpander Macro
		{
			get { return macro; }
		}

		#region IGenieLampConfig implementation
		ILayersConfig IGenieLampConfig.Layers
		{
			get { return layers; }
		}

		Patterns.IPatterns IGenieLampConfig.Patterns
		{
			get { return this.patterns; }
		}
		#endregion
	}
}

