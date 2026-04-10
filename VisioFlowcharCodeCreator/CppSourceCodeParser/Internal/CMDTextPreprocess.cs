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
			return text
				.Split('\n')
				.Select(line => line.Trim())
				.Where(line => !string.IsNullOrEmpty(line))
				.ToList();
		}
		
		private string ClearComments()
		{
			Regex multiLineComments = new Regex(@"/\*[\s\S]*?\*/", RegexOptions.Singleline);
			Regex oneLineComments = new Regex(@"//.*?(?=\r?\n|$)", RegexOptions.Multiline);
			text = oneLineComments.Replace(text, string.Empty);
			text = multiLineComments.Replace(text, string.Empty);
			return text;
		}

		private string ClearSpaceSymbols()
		{
			Regex SpaceSymbols = new Regex("[\t\r\f\v]");
			text = SpaceSymbols.Replace(text, string.Empty);
			return text;
		}
	}
}
