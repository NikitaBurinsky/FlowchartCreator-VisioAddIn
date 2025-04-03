using System;
using System.Collections.Generic;

namespace CMDParser.TLPostProcesser
{
	internal class CmdTokenListPostProcesser
	{ 
		public void UseCommandListPostProcess(List<Command> tokens, CmdParseOptions options)
		{
			SetFunctionStart(tokens);
			SetOrCreateFunctionEnd(tokens);
			GatesFormatter gatesFormatter = new GatesFormatter();

			CombineCommandsOneType(tokens, options.MaxCombinedNodesOneType);
			gatesFormatter.AddGatesToOpenZonesCommands(tokens);
			if (!IsGatesNumberRight(tokens))
				throw new Exception("Количество SOZ и EOZ после пост процесса токенизации не совпадает.");

		}
		private void ClearFromEmpty(List<Command> commands)
		{
			for (int i = commands.Count-1; i >= 0; ++i)
			{
				string clearedText = commands[i].text.Replace("\n", string.Empty);
				if (string.IsNullOrEmpty(clearedText) || string.IsNullOrWhiteSpace(clearedText))
					commands.RemoveAt(i);
			}
		}
		private void CombineCommandsOneType(List<Command> commands, int MaxNumOfCombiningTokens)
		{
			int CombinedCommands = 1;
			for (int i = 1; i < commands.Count;)
			{
				if (CombinedCommands < MaxNumOfCombiningTokens
					&&( commands[i].type == CMD.PROCESS 
					|| commands[i].type == CMD.OUTPUT 
					|| commands[i].type == CMD.INPUT 
					|| commands[i].type == CMD.SUBPROCESS))
				{
					if (commands[i].type == commands[i - 1].type)
					{
						commands[i - 1].text = commands[i - 1].text + "\n" +commands[i].text;
						commands.RemoveAt(i);
						++CombinedCommands;
						continue;
					}	
				}
				else
					CombinedCommands = 1;
					++i;
			}
		}

		private bool IsGatesNumberRight(List<Command> commands)
		{
			int OpenedGates = 0;
			foreach (Command command in commands)
			{
				if (command.type == CMD.SOZ)
					++OpenedGates;
				else if (command.type == CMD.EOZ)
					--OpenedGates;
			}
			return OpenedGates == 0;
		}

		private void SetFunctionStart(List<Command> commands)
		{
			if (commands[0].type != CMD.SUBPROCESS && commands[1].type != CMD.SOZ)
				throw new Exception("Start of function was not defined : 152");
			commands[0].type = CMD.StartFunc;
		}

		private void SetOrCreateFunctionEnd(List<Command> commands)
		{
			if (commands[commands.Count-2].type != CMD.RETURN)
			{
				commands.Insert(commands.Count - 1, new Command("return", CMD.RETURN));
			}
		}
		internal class GatesFormatter
		{
			static readonly List<CMD> openingZoneCommands = new List<CMD> {CMD.IF, CMD.ELSEIF
					, CMD.ELSE, CMD.LOOP
					, CMD.DO_LOOP, CMD.DEFAULT_SWITCH, CMD.StartFunc};

			public void AddGatesToOpenZonesCommands(List<Command> commands)
			{
				for(int i = commands.Count - 2; i >= 0; --i)
				{
					if (commands[i].type == CMD.SWITCH)
						HandleCasesInSwitch(i, commands);
					else if (IsOpeningZone(commands[i]) && commands[i+1].type != CMD.SOZ)
					{
						RecursiveAddZone(1, i, commands);
					}
				}
			}

			private void RecursiveAddZone(int prevEOZCount, int rootOZIndex, List<Command> commands)
			{
				int CurPotentialSOZ = rootOZIndex + 1;
				if (IsOpeningZone(commands[CurPotentialSOZ]))
				{
					RecursiveAddZone(prevEOZCount + 1, CurPotentialSOZ, commands);
					commands.Insert(CurPotentialSOZ, new Command("{", CMD.SOZ));
				}
				else
				{
					int allAddedEOZInd = CurPotentialSOZ + 1; 
					for(int i = 0; i < prevEOZCount ; ++i)
					{
						commands.Insert(allAddedEOZInd ,new Command("}", CMD.EOZ));
					}
					commands.Insert(CurPotentialSOZ, new Command("{", CMD.SOZ));
				}
			}
			private void HandleCasesInSwitch(int SwitchIndex, List<Command> commands)
			{
				int SwitchEOZ = findEOZforSOZ(SwitchIndex + 1, commands);
				for(int i = SwitchEOZ - 1; i > SwitchIndex ; --i)
				{
					if (commands[i].type == CMD.CASE || commands[i].type == CMD.DEFAULT_SWITCH)
						HandleOneCASE(i, SwitchEOZ, commands);
				}
			}
			private void HandleOneCASE(int CaseIndex, int EOZSwitch, List<Command> commands)
			{
				if (commands[CaseIndex+1].type != CMD.SOZ)
				{
					commands.Insert(CaseIndex + 1, new Command("{", CMD.SOZ));
					int i;
					for (i = CaseIndex; i < (EOZSwitch - 1) && commands[i + 1].type != CMD.CASE; ++i);
					commands.Insert(i + 1, new Command("}", CMD.EOZ));
				}
			}

			public int findEOZforSOZ(int SOZ, List<Command> commands)
			{
				int OpenedGates = 0, s;
				for(s = SOZ; OpenedGates > 0 && s < commands.Count; ++s)
				{
					if (commands[s].type == CMD.SOZ)
						++OpenedGates;
					if (commands[s].type == CMD.EOZ)
						--OpenedGates;
				}
				if (OpenedGates != 0)
					throw new Exception("EOZ не был найден : 1984");
				return s - 1 ;
			}
			public bool IsOpeningZone(Command command) => openingZoneCommands.Contains(command.type);
			public bool IsOpeningZone(CMD type) => openingZoneCommands.Contains(type);
		}


	}
}
