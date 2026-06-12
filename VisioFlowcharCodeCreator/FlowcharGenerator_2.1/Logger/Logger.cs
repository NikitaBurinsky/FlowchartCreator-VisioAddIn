using System;
using System.IO;

namespace FlowchartGenerator
{
	internal class Logger
	{ 
		private string logFilePath;

		public static void ShutDownLogs()
		{
			// No-op since we don't keep streams open anymore
		}

		public Logger(string LogObjectName, string LogFolder = null)
		{
			if (LogFolder == null)
				LogFolder = $@"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}\FlowchartCreatorAddIn" + @"\FG_LOGS";

			try
			{
				if (!Directory.Exists(LogFolder))
				{
					Directory.CreateDirectory(LogFolder);
				}
				logFilePath = Path.Combine(LogFolder, LogObjectName + "_LOG.txt");
				
				// Overwrite / clear the log file on creation
				File.WriteAllText(logFilePath, $"{DateTime.Now} : {LogObjectName} : Construct : Success" + Environment.NewLine);
			}
			catch 
			{
				// Fail silently to prevent logger from crashing the add-in
			}
		}

		public void Write(string message, int index = 0)
		{
			if (string.IsNullOrEmpty(logFilePath)) return;

			try
			{
				File.AppendAllText(logFilePath, message + Environment.NewLine);
			}
			catch
			{
				// Fail silently
			}
		}
	}
}
