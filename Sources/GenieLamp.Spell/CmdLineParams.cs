using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Spell
{
	public class CmdLineParams : IGenieLampSpellConfig
	{
		private string fileName;
		private WarningLevel minWarningLevel = WarningLevel.Medium;
		private bool accepted = false;

		public bool Accepted {
			get { return accepted; }
		}

		public CmdLineParams()
		{
			Parse();
		}

		enum ExpectedLexeme
		{
			Option,
			FileName,
			WarningLevel
		}

		public void Parse()
		{
			ExpectedLexeme expectedLexeme = ExpectedLexeme.Option;
			accepted = false;
			for (int i = 1; i < Environment.GetCommandLineArgs().GetLength(0); i++) {
				string arg = Environment.GetCommandLineArgs()[i];
            
				if (expectedLexeme == ExpectedLexeme.Option) {
					accepted = false;
					if (arg.Equals("-i"))
						expectedLexeme = ExpectedLexeme.FileName;
					else if (arg.Equals("-wl"))
						expectedLexeme = ExpectedLexeme.WarningLevel;
					else
						Console.WriteLine("Unknown option: {0}", arg);
				}
				else {
					accepted = true;
					switch (expectedLexeme) {
					case ExpectedLexeme.FileName:
						fileName = Environment.ExpandEnvironmentVariables(arg);
						break;
					case ExpectedLexeme.WarningLevel:
						int level;
						bool parsed = int.TryParse(arg, out level);
						if (!parsed || level < (int)WarningLevel.Low || level > (int)WarningLevel.High) {
							accepted = false;
							Console.WriteLine("Invalid warning level value: {0}", arg);
							return;
						}
						else
							minWarningLevel = (WarningLevel)level;
						break;
					default:
						accepted = false;
						break;
					}
					expectedLexeme = ExpectedLexeme.Option;
				}
			}
		}

		public void ShowUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine(String.Format("\t{0}[.exe] <options>", 
				Path.GetFileNameWithoutExtension(Path.GetFileName(Environment.GetCommandLineArgs()[0]))));
			Console.WriteLine("Options:");
			Console.WriteLine("\t-i <file name> - input project file");
			Console.WriteLine("\t-wl <0|1|2> - warning level output (0 - all, 2 - most important only");
		}

		public static string GetTitle()
		{
			return String.Format("Genie Lamp MDD Core {0}\nSpeller {1}",
				System.Reflection.Assembly.GetAssembly(typeof(GenieLamp.Core.GenieLamp)).GetName().Version.ToString(),
				System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
		}
		
		#region IGenieLampSpellConfig implementation
		public string FileName {
			get { return fileName; }
		}

		public WarningLevel MinWarningLevel {
			get { return this.minWarningLevel;	}
		}
		#endregion
	}
}

