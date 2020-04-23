using System;
using System.Diagnostics;
using System.IO;

using GenieLamp.Core;

namespace GenieLamp.Spell
{
	class MainClass
	{
		public static int Main (string[] args)
		{
            Console.WriteLine("{0}\n", CmdLineParams.GetTitle());
            Logger.Instance.FileName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]) + ".log";
            CmdLineParams cmdLineParams = new CmdLineParams();
			Logger.Instance.MinWarningLevel = cmdLineParams.MinWarningLevel;

            int errLevel = 1;
            try
            {
                if (cmdLineParams.Accepted)
                {
		            Logger.Instance.TraceLine("Application started");
					GenieLamp.Core.GenieLamp lamp = GenieLamp.Core.GenieLamp.CreateGenieLamp(cmdLineParams, Logger.Instance);
					try
					{
	                    lamp.Init();
	                    lamp.Spell();
			            Logger.Instance.TraceLine("Spell finished");
						if (Logger.Instance.WarningCount > 0) {
							Logger.Instance.ConsoleWarningColors();
				            Logger.Instance.TraceLine("There was(were) {0} warning(s) during the project validation", Logger.Instance.WarningCount);
							Logger.Instance.ConsoleResetColors();
						}
						else
				            Logger.Instance.TraceLine("No warnings");
	                    errLevel = 0;
					}
					catch(Exception e)
					{
						lamp.DumpModel();
	                    throw new Exception(e.Message, e);
					}
                }
                else
                {
                    cmdLineParams.ShowUsage();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e.Message);
#if DEBUG
				Logger.Instance.Echo = true;
#else
				Logger.Instance.Echo = false;
#endif
				Logger.Instance.TraceLine(e.ToString());
				Logger.Instance.Echo = false;
#if DEBUG
				Exception innerEx = e;
				while (innerEx != null) {
                	Logger.Instance.TraceLine(innerEx.StackTrace);
					innerEx = innerEx.InnerException;
				}
#endif
				Logger.Instance.Echo = true;
				Logger.Instance.ConsoleErrorColors();
                Logger.Instance.Error("Spell FAILED");
				Logger.Instance.ConsoleResetColors();
            }
			Logger.Instance.Flush();

			return errLevel;
		}
	}
}
