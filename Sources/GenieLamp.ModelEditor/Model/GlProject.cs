using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GenieLamp.Core;
using GenieLamp.ModelEditor.Services;

namespace GenieLamp.ModelEditor.Model
{
    class GlProject
    {
        private static GlProject instance = null;
        public static GlProject Instance
        {
            get { return instance; }
        }

        public IGenieLamp Lamp { get; private set; }
        public SpellConfig SpellConfig { get; private set; }

        public static void OpenProject(string fileName)
        {
            if (Instance == null)
            {
                instance = new GlProject();
            }

            Instance.SpellConfig = new SpellConfig()
            {
                FileName = fileName
            };

            Instance.Lamp = GenieLamp.Core.GenieLamp.CreateGenieLamp(Instance.SpellConfig, Logger.Instance);
        }
    }
}
