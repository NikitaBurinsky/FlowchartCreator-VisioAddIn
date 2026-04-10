using System;

namespace CMDParser
{
	[Serializable]
	public class FlowchartUserException : Exception
	{
		public FlowchartUserException(string message) : base(message) { }

		public FlowchartUserException(string message, Exception innerException) : base(message, innerException) { }
	}
}
