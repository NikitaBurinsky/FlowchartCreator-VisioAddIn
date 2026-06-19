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
			text = SplitStatementsByNewlines(text);
			text = AutoWrapControlBlocks(text);
			List<string> lines = SliceToLines();

			return lines;
		}

		private string AutoWrapControlBlocks(string code)
		{
			string[] lines = code.Split(new[] { "\n" }, StringSplitOptions.None);
			List<string> result = new List<string>();

			// Regex matching standard control structure boundaries (if, else if, else, while, for)
			Regex controlRegex = new Regex(@"^\s*(if\s*\(|while\s*\(|for\s*\(|else\s+if\s*\(|else\s*$|else\s*\{?\s*$)", RegexOptions.Compiled);

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string trimmed = line.Trim();

				if (controlRegex.IsMatch(trimmed) && !trimmed.EndsWith("{"))
				{
					int openParenCount = 0;
					int closeParenIndex = -1;
					bool isPureElse = trimmed.StartsWith("else") && !trimmed.Contains("if");

					if (isPureElse)
					{
						string afterElse = trimmed.Substring(4).Trim();
						if (afterElse.Length > 0 && afterElse.EndsWith(";"))
						{
							line = line.Replace(afterElse, "{ " + afterElse + " }");
							result.Add(line);
							continue;
						}
					}
					else
					{
						int firstParen = line.IndexOf('(');
						if (firstParen != -1)
						{
							for (int j = firstParen; j < line.Length; j++)
							{
								if (line[j] == '(') openParenCount++;
								else if (line[j] == ')')
								{
									openParenCount--;
									if (openParenCount == 0)
									{
										closeParenIndex = j;
										break;
									}
								}
							}
						}

						if (closeParenIndex != -1 && closeParenIndex < line.Length - 1)
						{
							string afterParen = line.Substring(closeParenIndex + 1).Trim();
							if (afterParen.Length > 0 && afterParen.EndsWith(";"))
							{
								string header = line.Substring(0, closeParenIndex + 1);
								line = header + " { " + afterParen + " }";
								result.Add(line);
								continue;
							}
						}
					}

					int nextIndex = -1;
					for (int k = i + 1; k < lines.Length; k++)
					{
						if (!string.IsNullOrWhiteSpace(lines[k]))
						{
							nextIndex = k;
							break;
						}
					}

					if (nextIndex != -1 && lines[nextIndex].Trim().StartsWith("{"))
					{
						result.Add(line);
						continue;
					}

					int endStatementIndex = -1;
					for (int k = i + 1; k < lines.Length; k++)
					{
						string nextLineTrimmed = lines[k].Trim();
						if (string.IsNullOrWhiteSpace(nextLineTrimmed)) continue;

						if (nextLineTrimmed.EndsWith(";") || nextLineTrimmed.EndsWith("}"))
						{
							endStatementIndex = k;
							break;
						}
					}

					if (endStatementIndex != -1)
					{
						lines[i] = lines[i] + " {";
						lines[endStatementIndex] = lines[endStatementIndex] + " }";
					}
				}

				result.Add(lines[i]);
			}

			return string.Join("\n", result);
		}

		private string SplitStatementsByNewlines(string code)
		{
			StringBuilder sb = new StringBuilder();
			bool inString = false;
			bool inChar = false;
			int parenDepth = 0;

			for (int i = 0; i < code.Length; i++)
			{
				char c = code[i];

				// Handle escaping inside literals
				if (c == '\\' && i + 1 < code.Length && (inString || inChar))
				{
					sb.Append(c);
					sb.Append(code[i + 1]);
					i++;
					continue;
				}

				if (c == '"' && !inChar)
				{
					inString = !inString;
				}
				else if (c == '\'' && !inString)
				{
					inChar = !inChar;
				}
				else if (c == '(' && !inString && !inChar)
				{
					parenDepth++;
				}
				else if (c == ')' && !inString && !inChar)
				{
					parenDepth = Math.Max(0, parenDepth - 1);
				}

				sb.Append(c);

				// Insert a newline after semicolons outside string/char literals and loop declarations (parentheses)
				if (c == ';' && !inString && !inChar && parenDepth == 0)
				{
					bool hasNewlineAhead = false;
					for (int j = i + 1; j < code.Length; j++)
					{
						if (code[j] == '\n')
						{
							hasNewlineAhead = true;
							break;
						}
						if (code[j] != ' ' && code[j] != '\r' && code[j] != '\t')
						{
							break;
						}
					}
					if (!hasNewlineAhead)
					{
						sb.Append('\n');
					}
				}
			}
			return sb.ToString();
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
			text = text.Replace("\t", " ");
			text = text.Replace("\r", string.Empty);
			text = text.Replace("\f", " ");
			text = text.Replace("\v", " ");
			return text;
		}
	}
}
