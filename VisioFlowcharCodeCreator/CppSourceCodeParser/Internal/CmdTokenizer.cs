using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CMDParser.Tokenizer
{
	internal class CmdTokenizer
	{

		Dictionary<CMD, List<Regex>> staticTokens = new Dictionary<CMD, List<Regex>>()
		{
			{ CMD.IF, new List<Regex>{ new Regex("\\bif\\s*\\((?:[^()]|\\((?:[^()]|\\([^()]*\\))*\\))*\\)") }},
			{ CMD.ELSEIF, new List<Regex>{new Regex("\\belse\\s+if\\s*\\(([^)]*)\\)") }},
			{ CMD.ELSE, new List<Regex>{new Regex("\\belse\\b(?!\\s*if\\b)") }},
			{ CMD.SWITCH, new List<Regex>{new Regex("\\bswitch\\s*\\(([^)]*)\\)") }},
			{ CMD.CASE, new List<Regex>{new Regex("\\bcase\\s+([^:]+):") }},
			{ CMD.DEFAULT_SWITCH, new List<Regex>{new Regex("\\bdefault\\s*:") }},
			{ CMD.BREAK, new List<Regex>{new Regex("\\bbreak\\s*;") }},
			{ CMD.LOOP, new List<Regex>
				{new Regex("\\bfor\\s*\\(([^;]*;[^;]*;[^)]*)\\)"), new Regex("\\bwhile\\s*\\(([^)]*)\\)"), new Regex("\\bfor\\s*\\(\\s*(?:auto|const\\s+auto\\s*&)\\s+(\\w+)\\s*:\\s*([^)]+)\\)")}},
			{ CMD.DO_LOOP, new List<Regex>{new Regex("\\bdo\\s*({[^}]*}|[^}]*)\\s*while\\s*\\(([^)]*)\\)") }},
			{ CMD.END_LOOP, new List<Regex>{new Regex("\\bwhile\\s*\\(([^)]*)\\)\\s*;") }},
			{ CMD.RETURN, new List<Regex>{new Regex("\\breturn\\s*([^;]*)\\s*;") }},
			{ CMD.SOZ, new List<Regex>{ new Regex("{")} },
			{ CMD.EOZ, new List<Regex>{ new Regex("}")} }
		};

		public List<Command> TokenizeLines(List<string> lines, Dictionary<string, CMD> knownSPTokensNames)
		{
			CheckAndHandleGates(lines);
			List<Command> result = new List<Command>();
			Command currentCommand = new Command("",CMD.NONE);
			for (int i = 0; i < lines.Count; ++i)
			{
				currentCommand = CheckForStaticKeyToken(lines[i]);
				if (currentCommand.type == CMD.NONE)
					currentCommand = ContainsKnownSubprocess(lines[i], knownSPTokensNames);
				if (currentCommand.type == CMD.NONE )
					currentCommand = new Command(lines[i], CMD.PROCESS);//TEMP
				result.Add(currentCommand);
			}
			if (!IsGatesNumberRight(result))
				throw new Exception("Ошибка токенизации, SOZ и EOZ не совпадают : 6362");
			return result;
		}

		private static Command ContainsKnownSubprocess(string codeLine, Dictionary<string, CMD> knownSubProcessNames)
		{
			var rgx_ComplexFunction = new Regex(@"
            (?:[\w:]+\s*::\s*)? 
            (\w+)
            \s*<\s*[\w<>\s,]+\s*>?
            \s*\( 
            [^)]*
            \)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
			var rgx_BasicFunction = new Regex(@"(?<func>\b\w+\b)\s*\([^)]*\)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
			var matches = rgx_BasicFunction.Matches(codeLine);
			if (matches.Count == 0)
			{
				matches = rgx_ComplexFunction.Matches(codeLine);
			}

			foreach (Match match in matches)
			{
				if (match.Groups.Count > 1)
				{
					string functionName = match.Groups[1].Value;
					if (knownSubProcessNames.ContainsKey(functionName))
					{
						return new Command(codeLine, knownSubProcessNames[functionName]); //TODO Разбор на ВВОД, ВЫВОД и др
					}
					return new Command(codeLine, CMD.SUBPROCESS);
				}
			}
			return new Command("", CMD.NONE);
		}

		private void CheckAndHandleGates(List<string> lines)
		{
			for (int i = 0; i < lines.Count; ++i)
			{
				if (lines[i].Length != 1)
				{
					if (lines[i].StartsWith("{") || lines[i].StartsWith("}"))
					{
						lines[i] = lines[i].Substring(1);
						lines.Insert(i, lines[i++].Substring(0));
					}
					if (lines[i].EndsWith("{") || lines[i].EndsWith("}"))
					{
						lines.Insert(i + 1, lines[i].Substring(lines[i].Length - 1));
						lines[i] = lines[i].Substring(0, lines[i++].Length - 1);
					}
				}
			}
			return;
		}

		private Command CheckForStaticKeyToken(string line)
		{
			foreach (KeyValuePair<CMD, List<Regex>> keyValue in staticTokens)
			{
				foreach (Regex regex in keyValue.Value)
				{
					if (regex.IsMatch(line))
					{
						return new Command(line, keyValue.Key);
					}
				}
			}
			return new Command("", CMD.NONE);
		}
		private bool IsGatesNumberRight(List<Command> commands)
		{
			int OpenedGates = 0, ClosedGates = 0;
			foreach (Command command in commands)
			{
				if (command.type == CMD.SOZ)
					++OpenedGates;
				else if (command.type == CMD.EOZ)
					++ClosedGates;
			}

			return OpenedGates == ClosedGates;
		}


	}
}
