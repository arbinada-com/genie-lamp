using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;

namespace GenieLamp.Spell
{
	class Logger : ILogger
	{

		private static Logger instance = null;

		public static Logger Instance
		{
			get
			{
				if (instance == null)
					instance = new Logger();
				return instance;
			}
		}
		
		private bool lastLF = false;
		private StreamWriter fileStreamLog;
		private string fileName;
		private bool echo = true;
		private int warningCount = 0;
		private int errorCount = 0;
		private WarningLevel minWarningLevel = WarningLevel.Medium;
		private ConsoleColor defaultFGColor = Console.ForegroundColor;
		
		#region Constructors
		private Logger()
		{
		}
		#endregion
		
		public string FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				if (fileStreamLog != null)
					fileStreamLog.Close();
				FileStream fs = new FileStream(fileName, FileMode.Create);
				fileStreamLog = new StreamWriter(fs);
			}
		}
		
		public void Flush()
		{
			if (fileStreamLog != null)
				fileStreamLog.Flush();
		}
		
		public void ConsoleWarningColors()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
		}
		
		public void ConsoleErrorColors()
		{
			Console.ForegroundColor = ConsoleColor.Red;
		}
		
		public void ConsoleResetColors()
		{
			Console.ForegroundColor = defaultFGColor;
		}

		#region ILogger implementation
		public int WarningCount
		{
			get { return warningCount; }
		}

		public WarningLevel MinWarningLevel
		{
			get { return this.minWarningLevel; }
			set
			{ 
				this.minWarningLevel = value; 
				TraceLine("Using minimal warn level: {0} ({1})", (int)minWarningLevel, minWarningLevel);
			}
		}
		
		public int ErrorCount
		{
			get { return errorCount; }
		}
		
		public void Trace(string message)
		{
			if (fileStreamLog != null)
				fileStreamLog.Write(String.Format("{0:yyyy-MM-ddTHH:mm:ss.fff}: {1}", DateTime.Now, message));
			if (echo)
				Console.Write(message);
			lastLF = false;
		}

		public void Trace(string message, params object[] args)
		{
			Trace(String.Format(message, args));
		}

		public void TraceLine(string message)
		{
			Trace(message + Environment.NewLine);
			lastLF = true;
		}
		
		public void TraceLine(string message, params object[] args)
		{
			TraceLine(String.Format(message, args));
		}

		public void Debug(string message)
		{
#if DEBUG
			bool currEcho = Echo;
			Echo = true;
			Trace(message);
			Echo = currEcho;
#endif
		}

		public void Debug(string message, params object[] args)
		{
			Debug(String.Format(message, args));
		}

		public void DebugLine(string message)
		{
			Debug(message + Environment.NewLine);
		}

		public void DebugLine(string message, params object[] args)
		{
			DebugLine(String.Format(message, args));
		}

		public void ProgressStep()
		{
			Console.Write(".");
			lastLF = false;
		}

		public bool Echo
		{
			get { return echo; }
			set { echo = value; }
		}
		
		public void Error(string format, params object[] args)
		{
			Error(String.Format(format, args));
		}
		
		public void Error(string message)
		{
			errorCount++;
			ConsoleErrorColors();
			try
			{
				TraceLine(message);
			}
			finally
			{
				ConsoleResetColors();
			}
		}

		public void Warning(WarningLevel level, string message)
		{
			if (level < minWarningLevel)
				return;
			warningCount++;
			bool currEcho = Echo;
			try
			{
				Echo = true;
				ConsoleWarningColors();
				if (!lastLF)
					TraceLine(String.Empty);
				TraceLine(String.Format("Warning: {0}", message));
			}
			finally
			{
				Echo = currEcho;
				ConsoleResetColors();
			}
		}

		public void Warning(WarningLevel level, string message, params object[] args)
		{
			Warning(level, String.Format(message, args));
		}
		#endregion
	}
}

