using System;

namespace GenieLamp.Core.Test
{
	public class SpellConfigMock : IGenieLampSpellConfig
	{
		public SpellConfigMock()
		{
		}

		#region IGenieLampSpellConfig implementation
		public string FileName { get; set; }

		public WarningLevel MinWarningLevel { get; set; }
		#endregion
	}
}

