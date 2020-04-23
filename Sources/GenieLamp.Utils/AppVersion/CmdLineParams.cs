using System;
using System.IO;

namespace GenieLamp.Utils.AppVersion
{
	public class CmdLineParams
	{
		public enum CommandMode
		{
			Unknown,
			ModeExtract
		}

		#region Constructors
		public CmdLineParams()
		{
			this.Accepted = false;
			this.Mode = CommandMode.Unknown;
			Parse();
		}
		#endregion

		public bool Accepted { get; private set; }
		public string AssemblyName { get; private set; }
		public CommandMode Mode { get; private set; }

		enum ExpectedLexeme
		{
			None,
			AssemblyName,
		}

		public void Parse()
		{
			if (Environment.GetCommandLineArgs().GetLength(0) > 1)
			{
				switch (Environment.GetCommandLineArgs()[1].ToLower())
				{
				case "extract":
					Mode = CommandMode.ModeExtract;
					break;
				default:
					Console.WriteLine("Command was not specified");
					Console.WriteLine("Assembly/exe file name is not specified");
					return;
				}
			}
			else
				return;


			ExpectedLexeme expectedLexeme = ExpectedLexeme.None;
			Accepted = true;
			for (int i = 2; i < Environment.GetCommandLineArgs().GetLength(0); i++)
			{
				string arg = Environment.GetCommandLineArgs()[i];

				if (expectedLexeme == ExpectedLexeme.None)
				{
					if (arg.Equals("--assembly", StringComparison.InvariantCultureIgnoreCase))
						expectedLexeme = ExpectedLexeme.AssemblyName;
					else
					{
						Accepted = false;
						Console.WriteLine("Unknown option: {0}", arg);
					}
				}
				else
				{
					switch (expectedLexeme)
					{
					case ExpectedLexeme.AssemblyName:
						AssemblyName = Path.GetFullPath(Environment.ExpandEnvironmentVariables(arg));
						break;
					default:
						Accepted = false;
						break;
					}
					expectedLexeme = ExpectedLexeme.None;
				}
			}
		}

		public void ShowUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine(String.Format("\t{0}[.exe] <command> [options...]",
				Path.GetFileNameWithoutExtension(Path.GetFileName(Environment.GetCommandLineArgs()[0]))));
			Console.WriteLine();
			Console.WriteLine("Commands:");
			Console.WriteLine("\textract - extract and write out assembly version");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("\t--assembly <file name> - input assembly/exe file");
		}

		public static string GetTitle()
		{
			return String.Format("AppVersion (from Genie Lamp utils) {0}.{1}.{2}",
				System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Major,
			    System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Minor,
			    System.Reflection.Assembly.GetEntryAssembly().GetName().Version.Build);
		}
	}
}

