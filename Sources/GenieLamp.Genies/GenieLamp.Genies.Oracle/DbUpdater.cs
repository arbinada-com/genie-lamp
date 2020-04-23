using System;
using System.Diagnostics;

using GenieLamp.Core.Exceptions;

namespace GenieLamp.Genies.Oracle
{
	public class DbUpdater
	{
		private OracleGenie genie;
		private string scriptFileName;

		public DbUpdater(OracleGenie genie)
		{
			this.genie = genie;
		}

		public string ScriptFileName
		{
			get { return scriptFileName; }
			internal set
			{
				scriptFileName = value;
				genie.Config.Macro.SetMacro("%FILE_NAME%", scriptFileName);
				genie.Config.Macro.SetMacro("%SCRIPT_NAME%", scriptFileName);
			}
		}

		public void Run()
		{
			string isql = genie.Config.Macro.Subst(genie.Config.Params.ValueByName("UpdateDatabase.Utility", "sqlplus"));
			string arguments = genie.Config.Macro.Subst(genie.Config.Params.ValueByName("UpdateDatabase.Arguments", "%FILE_NAME%"));
			genie.Model.Lamp.Logger.TraceLine("\nRunning database update: {0} {1}", isql, arguments);

			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = isql;
			p.StartInfo.Arguments = arguments;
			p.Start();
			p.StandardInput.WriteLine("/");
			p.StandardInput.WriteLine("exit");
			p.WaitForExit(60000);
			if (p.HasExited)
			{
				if (p.ExitCode != 0)
				{
					throw new GlException("Database may was not updated. ExitCode: {0}\n{1}\n{2}", 
					                      p.ExitCode, 
					                      p.StandardOutput.ReadToEnd(),
					                      p.StandardError.ReadToEnd());
				}
			}
			else
			{
				p.Close();
				p.Kill();
				throw new GlException("Database update interrupted by timeout");
			}

		}
	}
}

