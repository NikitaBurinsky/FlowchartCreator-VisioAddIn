using System;
using System.Collections.Generic;
using System.IO;

namespace FlowchartGenerator
{
	internal class Logger
	{ 
		StreamWriter FileStream;
		static List<Logger> ExistLoggers = new List<Logger>();
		public static void ShutDownLogs()
		{
			foreach(Logger loger in ExistLoggers)
			{
				loger.FileStream.Close();
			}
		}
        public Logger(string LogObjectName, string LogFolder = null) // Lenovo
        {
			if (LogFolder == null)
				LogFolder = $@"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}\FlowchartCreatorAddIn" + @"\FG_LOGS";

			LogFolder = LogFolder + @"\" + LogObjectName + "_LOG.txt";
			FileStream = new StreamWriter(LogFolder, false);
			if(FileStream == null)
			{
				throw new Exception($"Cannot create log file in {LogFolder}!!!");
			}
			ExistLoggers.Add(this);
			this.Write($"{DateTime.Now} : {LogObjectName} : Construct : Success");
		}
		public void Write(string message, int index = 0)
		{
			FileStream.WriteLine(message);
			FileStream.Flush();
		}


	}
}
