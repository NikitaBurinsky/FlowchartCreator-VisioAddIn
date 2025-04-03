using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json;
using System.IO;
namespace CMDParser.ReadKnown
{
	public static class KnownFunctionsDictionaryReader
	{
		static public Dictionary<string, CMD> DeserializeKnownFunctions(string jsonPath)
		{
			if (!File.Exists(jsonPath))
				throw new Exception($"Commands file is not exists in {jsonPath} : 964");
			string text = new StreamReader(jsonPath).ReadToEnd();
			var deserialized = JsonConvert.DeserializeObject<Dictionary<string, CMD>>(text);
			if (deserialized == null)
				throw new Exception("Deserialization failed");
			return deserialized;
		}
	}
}
