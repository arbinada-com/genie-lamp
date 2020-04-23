using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GenieLamp.Core;

namespace GenieLamp.ModelEditor.Model
{
    class SpellConfig : IGenieLampSpellConfig
    {
        public SpellConfig()
        {
            this.MinWarningLevel = WarningLevel.High;
        }

        public string FileName { get; internal set; }

        public WarningLevel MinWarningLevel { get; internal set; }
    }
}
