using System;
using System.Collections.Generic;
using System.IO;

namespace FlowchartGenerator
{
	internal class Logger
	{ 
		string LogFilePath;
		static List<Logger> ExistLoggers = new List<Logger>();
		public static void ShutDownLogs()
		{
			ExistLoggers.Clear();
		}
        public Logger(string LogObjectName, string LogFolder = null) // Lenovo
        {
			if (LogFolder == null)
				LogFolder = $@"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}\FlowchartCreatorAddIn" + @"\FG_LOGS";

			LogFilePath = LogFolder + @"\" + LogObjectName + "_LOG.txt";
			Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
			if (!File.Exists(LogFilePath))
			{
				using (new StreamWriter(LogFilePath, false)) { }
			}
			ExistLoggers.Add(this);
			this.Write($"{DateTime.Now} : {LogObjectName} : Construct : Success");
		}
		public void Write(string message, int index = 0)
		{
			using (StreamWriter streamWriter = new StreamWriter(LogFilePath, true))
			{
				streamWriter.WriteLine(message);
			}
		}


	}
}
