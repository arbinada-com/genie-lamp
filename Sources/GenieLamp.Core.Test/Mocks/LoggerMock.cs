using System;
using System.Text;

namespace GenieLamp.Core.Test
{
	public class LoggerMock : ILogger
	{
		public LoggerMock()
		{
			this.Lines = new StringBuilder();
		}

		internal StringBuilder Lines { get; private set; }

		#region ILogger implementation
		public void Trace(string message)
		{
			this.Lines.Append(message);
		}

		public void Trace(string message, params object[] args)
		{
		}

		public void TraceLine(string message)
		{
			this.Lines.AppendLine(message);
		}

		public void TraceLine(string message, params object[] args)
		{
			TraceLine(String.Format(message, args));
		}

		public void Debug(string message)
		{
			this.Lines.Append(message);
		}

		public void Debug(string message, params object[] args)
		{
			Debug(String.Format(message, args));
		}

		public void DebugLine(string message, params object[] args)
		{
			DebugLine(String.Format(message, args));
		}

		public void DebugLine(string message)
		{
			this.Lines.AppendLine(message);
		}

		public void Warning(WarningLevel level, string message)
		{
			this.Lines.AppendLine(message);
		}

		public void Warning(WarningLevel level, string message, params object[] args)
		{
			Warning(level, String.Format(message, args));
		}

		public void Error(string format, params object[] args)
		{
			Error(String.Format(format, args));
		}

		public void Error(string message)
		{
			this.Lines.AppendLine(message);
		}

		public void ProgressStep()
		{
		}

		public WarningLevel MinWarningLevel { get; set; }

		public int WarningCount { get; set; }

		public int ErrorCount { get; set; }

		public bool Echo { get; set; }
		#endregion
	}
}

