using System;
using System.Collections.Generic;
using System.Text;

using GNU.Gettext;

namespace GenieLamp.ModelEditor.Services
{
    class Services
    {
    }

    public class L
    {
        public static GettextResourceManager Catalog
        {
            get { return new GettextResourceManager(); }
        }
    }

}
