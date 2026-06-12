using System;

namespace FlowchartGenerator.UX_MENU_Forms
{
	public static class StaticCodeValidator
	{
		public static string ValidateBraces(string code)
		{
			if (string.IsNullOrEmpty(code)) return null;

			int openCurlies = 0;
			int closeCurlies = 0;
			int openParens = 0;
			int closeParens = 0;

			bool inString = false;
			bool inChar = false;
			bool inSingleLineComment = false;
			bool inMultiLineComment = false;

			for (int i = 0; i < code.Length; i++)
			{
				char c = code[i];

				// Handle comments
				if (inSingleLineComment)
				{
					if (c == '\n') inSingleLineComment = false;
					continue;
				}
				if (inMultiLineComment)
				{
					if (c == '/' && i > 0 && code[i - 1] == '*') inMultiLineComment = false;
					continue;
				}

				if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
				{
					inSingleLineComment = true;
					i++;
					continue;
				}
				if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
				{
					inMultiLineComment = true;
					i++;
					continue;
				}

				// Handle strings/chars
				if (inString)
				{
					if (c == '"' && code[i - 1] != '\\') inString = false;
					continue;
				}
				if (inChar)
				{
					if (c == '\'' && code[i - 1] != '\\') inChar = false;
					continue;
				}

				if (c == '"')
				{
					inString = true;
					continue;
				}
				if (c == '\'')
				{
					inChar = true;
					continue;
				}

				// Count braces
				if (c == '{') openCurlies++;
				else if (c == '}') closeCurlies++;
				else if (c == '(') openParens++;
				else if (c == ')') closeParens++;
			}

			if (openCurlies != closeCurlies)
			{
				return $"Несовпадение фигурных скобок {{}}:\nОткрывающих: {openCurlies}, Закрывающих: {closeCurlies}.";
			}

			if (openParens != closeParens)
			{
				return $"Несовпадение круглых скобок ():\nОткрывающих: {openParens}, Закрывающих: {closeParens}.";
			}

			return null;
		}
	}
}
