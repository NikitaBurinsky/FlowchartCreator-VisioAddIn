using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;


public enum CMD : int
{
    IF = 0,
    ELSE, ELSEIF, SWITCH, CASE,
    DEFAULT_SWITCH, DO_LOOP, LOOP, END_LOOP,
    SOZ, EOZ, SUBPROCESS, PROCESS,
    INPUT, OUTPUT, StartFunc, RETURN, BREAK, NONE, IGNORE
};

public class Command
{
    public Command(String txt, CMD tp)
    {
        text = txt;
        type = tp;
    }
    public String text;
    public CMD type;
}

public class CommandsReader
{
	//User Interface
	public CommandsReader(FlowchartGenerator.SettingsSystem settings, string commandPath)
	{
		_xlsxPath = commandPath;
		cmd_scaner = new CMD_CommandScanner(commandPath);
		commands = new List<Command>();
		this.settings = settings;
	}
	public List<Command> GetCommandsFromFile(string filepath)
	{
		if (filepath == null)
			throw new ArgumentNullException("FilePath is NULL");
		else
			sreader = new StreamReader(filepath, true);
		ReadCommandsTextFile();
		if (commands.Count < 1)
		{
			Console.WriteLine("\nERROR IN : ReadCommandsTextFile()");
			return null;
		}
		AddZonesToSingleCommands();
		CombineCommandsOneType();
		return commands;
	}
	//Backdoors
	private StreamReader sreader;
	private List<Command> commands;
	private CMD_CommandScanner cmd_scaner;
	string _xlsxPath = null;
	FlowchartGenerator.SettingsSystem settings;
	public bool ReadCommandsTextFile()
	{
		StringBuilder CommandsText = new StringBuilder();

		if (sreader == null)
			return false;

		String line = sreader.ReadLine();
		while (line != null)
		{
			if (line.Length > 0)
				CommandsText.Append(line + '\n');
			line = sreader.ReadLine();
		}

		if (CommandsText.Length < 1)
			return false;


		PrepareCommandsText(CommandsText);
		ConvertTextToCommands(CommandsText);

		return true;
	}
	public bool PrepareCommandsText(StringBuilder Text)
	{
		//Clear from \t
		Text.Replace("\t", string.Empty);

		//Clear from \*...*\
		int S_Comment = FindSubstrAfter(0, "/*", Text);
		int E_Comment;
		while (S_Comment != -1)
		{
			E_Comment = FindSubstrAfter(S_Comment + 2, "*/", Text);
			E_Comment += 2;
			Text.Remove(S_Comment, E_Comment - S_Comment + 3);

			S_Comment = FindSubstrAfter(S_Comment, "/*", Text);
		}

		//Clear from //...'\n'
		S_Comment = FindSubstrAfter(0, "//", Text);
		while (S_Comment != -1)
		{
			E_Comment = FindSubstrAfter(S_Comment, "\n", Text);
			Text.Remove(S_Comment - 1, E_Comment - S_Comment);

			S_Comment = FindSubstrAfter(S_Comment, "//", Text);
		}

		return true;
	}
	private int FindSubstrAfter(int after, string substr, StringBuilder text)
	{
		for (int t = after; t < text.Length - substr.Length; ++t)
		{
			bool IsWrong = false;
			int ss = 0;
			if (text[t] == substr[ss])
			{
				++ss;
				int t_temp = ++t;
				for (; ss < substr.Length && ss < text.Length; ++ss, ++t_temp)
				{
					if (substr[ss] != text[t_temp])
					{
						IsWrong = true;
						break;
					}
				}
				if (IsWrong == false)
				{
					return t;
				}
			}
		}
		return -1;
	}

	private bool ConvertTextToCommands(StringBuilder CommandsText)
	{
		List<String> Sliced = new List<String>();
		String Text = CommandsText.ToString();
		List<int> LineEnds = new List<int>();
		string slicedLine;
		int n = 0;
		int sn = 0;
		while (n < Text.Length)
		{
			if (Text[n] == '\n')
			{
				slicedLine = Text.Substring(sn, n - sn);
				slicedLine = ClearSpacesEnd(slicedLine);
				slicedLine = ClearSpacesStart(slicedLine);
				if (slicedLine.EndsWith("{"))
				{
					Sliced.Add(slicedLine.Substring(0, slicedLine.Length - 1));
					Sliced.Add("{");
				}
				else if (slicedLine.EndsWith("}"))
				{
					Sliced.Add(slicedLine.Substring(0, slicedLine.Length - 1));
					Sliced.Add("}");
				}
				else
					Sliced.Add(slicedLine);
				sn = ++n;
			}
			else
				++n;
		}
		slicedLine = Text.Substring(sn, n - sn);
		slicedLine = ClearSpacesEnd(slicedLine);
		slicedLine = ClearSpacesStart(slicedLine);
		if (slicedLine.EndsWith("{"))
		{
			Sliced.Add(slicedLine.Substring(0, slicedLine.Length - 2));
			Sliced.Add("{");
		}
		else if (slicedLine.EndsWith("}"))
		{
			Sliced.Add(slicedLine.Substring(0, slicedLine.Length - 2));
			Sliced.Add("}");
		}
		else
			Sliced.Add(slicedLine);

		for (int i = 0; i < Sliced.Count; ++i)
		{
			CreateAndAddCommandInList(Sliced[i]);
		}

		int end = commands.Count - 1;
		if (commands[0].type == CMD.SUBPROCESS)
		{ commands[0].type = CMD.StartFunc; }
		if (commands[end - 1].type != CMD.RETURN)
		{
			CreateAndAddCommandInList("return;", end);
		}

		return true;
	}

	private bool AddZonesToSingleCommands()
	{
		int lastEOZ = -1;
		for (int i = commands.Count - 1; i > 0; --i)
		{
			if (commands[i].type <= CMD.LOOP && commands[i].text.EndsWith(";"))
			{
				commands.Insert(i + 1, new Command("", CMD.EOZ));
				commands.Insert(i, new Command("", CMD.SOZ));
				continue;
			}

			if (commands[i].type == CMD.EOZ)
				lastEOZ = i;

			if (IsOpenZone(commands[i - 1]))
				if (commands[i].type != CMD.SOZ)
				{
					if (!IsOpenZone(commands[i]))
					{
						commands.Insert(i + 1, new Command("", CMD.EOZ));
						lastEOZ = i + 2;
						commands.Insert(i, new Command("", CMD.SOZ));
					}
					else
					{
						commands.Insert(i, new Command("", CMD.SOZ));
						commands.Insert(++lastEOZ, new Command("", CMD.EOZ));
					}
				}
		}
		return true;
	}


	private bool IsOpenZone(Command cmd) { return cmd.type <= CMD.LOOP; }
	private void CombineCommandsOneType()
	{
		if (settings.MaxCombinedNodesOneType == 1)
			return;

		int alsoAdded = 0;
		for (int i = 0; i < commands.Count - 1;)
		{
			if (commands[i].type == commands[i + 1].type && commands[i].type != CMD.SOZ && commands[i].type != CMD.EOZ)
			{
				if (++alsoAdded > settings.MaxCombinedNodesOneType)//TEMP
				{
					alsoAdded = 0;
					++i;
				}

				Command x = commands[i];
				x.text = commands[i].text + "\n" + commands[i + 1].text;
				commands[i] = x;
				commands.RemoveAt(i + 1);
				continue;
			}
			alsoAdded = 0;
			++i;
		}
	}
	//public enum CMD : int { IF = 0, ELSE, SWITCH, CASE, DEFAULT_SWITCH, LOOP, END_LOOP, SOZ, EOZ, SUBPROCESS, PROCESS, IO, StartEnd, BREAK, NONE };

	private CMD GetCommandType(String line)
	{
		return cmd_scaner.ScanCommand(line);
	}

	private string ClearSpacesEnd(String line)
	{
		int i = line.Length - 1;
		while (i >= 0 && line[i] == ' ')
			--i;
		if (i < line.Length - 1 && i > 0)
		{
			++i;
			return line.Remove(i, line.Length - i);
		}
		return line;
	}
	private string ClearSpacesStart(String line)
	{
		int i = 0;
		while (i < line.Length && line[i] == ' ')
			++i;
		if (i > 0 && i < line.Length)
			return line.Remove(0, i);
		return line;
	}
	private bool CreateAndAddCommandInList(String line, int indexInList = -1)
	{
		if (line == "" || line == "\n" || line == null)
			return false;
		if (line.Replace(" ", string.Empty) == "")
			return false;

		CMD type = cmd_scaner.ScanCommand(line);

		if (type == CMD.NONE || type == CMD.IGNORE)
			return false;

		if (indexInList == -1)
		{
			indexInList = commands.Count;
			commands.Add(new Command(line, type));
		}
		else
			commands.Insert(indexInList, new Command(line, type));
		PostProccCmd(indexInList);
		return true;
	}
	private void PostProccCmd(int lastInd)
	{
		Command cmd = commands[lastInd];
		if (IsOpenZone(cmd))
		{
			int Op = cmd.text.IndexOf("{"), End = cmd.text.IndexOf("}");
			if (Op != -1 && End != -1)
			{
				string line = cmd.text;
				cmd.text = line.Substring(0, Op);
				commands.Add(new Command("{", CMD.SOZ));
				CreateAndAddCommandInList(line.Substring(Op + 1, End - Op - 1));
				commands.Add(new Command("}", CMD.EOZ));
			}
			//mb add no zone loops "while(getchar());" TEST
		}

	}

}

