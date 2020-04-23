using System;
using System.IO;
using System.Reflection;

namespace GenieLamp.Utils.AppVersion
{
	public class VersionExtractor
	{
		private CmdLineParams cmdParams;

		public VersionExtractor(CmdLineParams cmdParams)
		{
			this.cmdParams = cmdParams;
			this.Version = String.Empty;
		}

		public string Version { get; private set; }
		public int Major { get; private set; }
		public int Minor { get; private set; }
		public int Build { get; private set; }
		public int Revision { get; private set; }

		public void Run()
		{
			if (!File.Exists(cmdParams.AssemblyName))
				throw new AppVersionException("Assembly not found: {0}", cmdParams.AssemblyName);

            Assembly assembly = Assembly.LoadFile(cmdParams.AssemblyName);
            if (assembly == null)
                throw new AppVersionException("Assembly was not loaded: {0}", cmdParams.AssemblyName);

			this.Version = assembly.GetName().Version.ToString();
			this.Major = assembly.GetName().Version.Major;
			this.Minor = assembly.GetName().Version.Minor;
			this.Build = assembly.GetName().Version.Build;
			this.Revision = assembly.GetName().Version.Revision;
		}
	}
}

