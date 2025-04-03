using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMDParser.Preprocess
{
	internal class CMDTextPre_Processing
	{
		String text;
		public List<string> PreProcessing(string Text)
		{
			text = Text;
			ClearComments();	
			ClearSpaceSymbols();
			List<string> lines = SliceToLines();

			return lines;
		}

		private List<string> SliceToLines()
		{
			List<string> lines = new List<string>();
			int startSubStr = 0;
			for (int endSubStr = 0; endSubStr < text.Length; ++endSubStr)
			{//TODO Optimiz
				if (text[endSubStr] == '\n')
				{
					if (endSubStr == startSubStr)
						continue;
					lines.Add(text.Substring(startSubStr, endSubStr - startSubStr));
					startSubStr = endSubStr + 1;
				}
				else if (text[endSubStr] == ';')
				{
					if (endSubStr == startSubStr)
						continue;
					lines.Add(text.Substring(startSubStr, endSubStr - startSubStr + 1));
					startSubStr = endSubStr + 1;
				}
			}
			if (startSubStr < text.Length)
				lines.Add(text.Substring(startSubStr, text.Length - startSubStr));
			for (int i = lines.Count - 1; i >= 0; --i)
				if (String.IsNullOrEmpty(lines[i]))
					lines.RemoveAt(i);
				else { lines[i] = lines[i].Replace("\n", string.Empty); }

			return lines;
		}
		
		private string ClearComments()
		{
			Regex MultiLineComments = new Regex(@"\/\*[\s\S]*?\*\/", RegexOptions.Multiline);
			Regex OneLinComments = new Regex(@"//.*?\n", RegexOptions.Multiline);
			string mcomment = MultiLineComments.Match(text).Value;
			string ocomment = OneLinComments.Match(text).Value;
			Console.WriteLine($"\n{mcomment}\n");
			Console.WriteLine($"\n{ocomment}\n");
			text = OneLinComments.Replace(text, string.Empty);
			text = MultiLineComments.Replace(text, string.Empty);
			return text;
		}
		private string ClearStartEndSpaces(string line)
		{
			int s = 0, e = line.Length - 1;
			while (s < line.Length && line[s++] == ' ');
			while (e >= 0 && line[e++] == ' ') ;
			line = line.Substring(s, e - s);
			return line;
		}

		private string ClearSpaceSymbols()
		{
			Regex SpaceSymbols = new Regex("[\t\r\f\v]");
			text = SpaceSymbols.Replace(text, string.Empty);
			return text;
		}
	}
}
