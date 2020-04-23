using System;
using System.Text;

namespace GenieLamp.Utils.AppVersion
{
	class MainClass
	{
		public static int Main(string[] args)
		{
			int result = 1;
			CmdLineParams cmdParams = new CmdLineParams();
			if (cmdParams.Accepted)
			{
				try
				{
					switch(cmdParams.Mode)
					{
					case CmdLineParams.CommandMode.ModeExtract:
						VersionExtractor ve = new VersionExtractor(cmdParams);
						ve.Run();
						Console.WriteLine(ve.Version);
						break;
					}
					result = 0;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					result = 1;
				}
			}
			else
			{
				cmdParams.ShowUsage();
			}
			return result;
		}
	}

	public class AppVersionException  : ApplicationException
	{
		public AppVersionException(string msg)
			: base(msg)
		{ }

		public AppVersionException(string format, params object[] args)
			: base(String.Format(format, args))
		{ }
	}
}
