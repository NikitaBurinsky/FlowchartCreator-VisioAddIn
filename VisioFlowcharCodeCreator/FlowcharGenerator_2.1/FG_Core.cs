using System;
using System.Collections.Generic;
using Visio = Microsoft.Office.Interop.Visio;

namespace FlowchartGenerator
{
	public class FG_Core
	{
		static Logger LOG = new Logger("Core");
		//COMPONENTS
		DiagramField Diagram;
		StyleserSystem Styliser = new StyleserSystem();
		public SettingsSystem FGSettings = new SettingsSystem();
		CommandsReaderImplementation CMDReader;

		//MEMBERS
		public List<Command> Commands { get; set; }
		private Dictionary<CMD, Func<int, List<CmdNode>, AreaReturn>> CommandsHandlers = new Dictionary<CMD, Func<int, List<CmdNode>, AreaReturn>>();

		private void InitialiseHandlersFunctions()
		{
			CommandsHandlers.Add(CMD.LOOP , this.Handle_Loop);
			CommandsHandlers.Add(CMD.SWITCH , this.Handle_SwitchCase);
			CommandsHandlers.Add(CMD.IF , this.Handle_IfElse);
        }

		public bool InitialiseSystems(Visio.Application app, Visio.Page page, string textBufferPath, string XlsxCommandsSheet)
		{
			ActivePage = page;
			Application = app;
			SubSystem.Settings = FGSettings;
			CMDReader = new CommandsReaderImplementation(XlsxCommandsSheet);
			Diagram = new DiagramField(Application, ActivePage, new Vector2D(40, 25));
			ShapeMaster.Startup(Application);
			SubSystem.InitClass(Diagram, page, app, this);

			InitialiseHandlersFunctions();

			return true;
		}


		public int GenerateDiagram(int StartLocation_X, int StartLocation_Y, string cmdtextpath)
		{
				LOG.Write("Generate diagram : Start");
				LOG.Write("GetCommandsFromBuffer : Start");
				Commands = CMDReader.GetCommandsFromFile(cmdtextpath);
				LOG.Write($"GetCommandsFromBuffer : Commands found : {Commands.Count}");
				if (Commands[0].type != CMD.StartFunc)
					return -1;
				Vector2D CurLoc = new Vector2D(StartLocation_X, StartLocation_Y);
				int i;
				CmdNode StartNode = Diagram.CreateCmdNode(Commands[0], CurLoc);
				AreaHandlers.BaseArea_Handler StartArea = new AreaHandlers.BaseArea_Handler(Commands);
				LOG.Write("Handle function area : Start");
				StartArea.HandleArea(0, out i);
				LOG.Write("Handle function area : Success");
				--CurLoc.Y;
				StartArea.SetZoneLocationByRootLocation(CurLoc);
				Diagram.ConnectCmdShapesBase(StartNode.GetConnection(ConType.Bottom), StartArea.AreaRoot);

				LOG.Write("Stylise : Start");
				Styliser.Style_Arrows();
				Styliser.Style_Shapes();
				LOG.Write("Stylise : Success");
			return 1;
		}
		private AreaReturn Create_Area(int ZoneRootCommandIndex, List<CmdNode> From)
		{
			/*
			* Функция проходит по всем командам, до закрывающего ее EOZ, вызывая соответвующие функции обработки в зависимости от типа команды
			* Ввод : Команда, открывающая зону, идущая перед SOZ
			* Вывод : Индекс последнего EOZ
			*/
			return new AreaReturn();
		
		}

		private CmdNode Create_BasicCommand(int CommandIndex, List<From_Connection> From)
		{
			/*
			 * Функция, создающая фигуру одиночной команды и коннектирущая ее к фигурам From
			 */

			if (From.Count == 0)
				return null;

                Command cmd = Commands[CommandIndex];
				Vector2D newLoc = Diagram.FindNextLogicalLocation(From);

                CmdNode cmdNode = Diagram.CreateCmdNode(cmd.text, cmd.type, newLoc);
				Diagram.ConnectCmdShapesBase(From, cmdNode);

			return cmdNode;
		}

		//Handler functions
		private bool IsOpenZone(Command cmd) { return cmd.type <= CMD.LOOP; }

		private AreaReturn Handle_SwitchCase(int SwitchIndex, List<CmdNode> From)
		{
			return new AreaReturn();
        }

        private AreaReturn Handle_Loop(int LoopIndex, List<CmdNode> From)
		{
			
			if (Commands[LoopIndex].type != CMD.LOOP)
				throw new Exception("Error : Incorrect LoopIndex");

			//CmdNode StartLoop = field.CreateAndConnectNode(Commands[LoopIndex], From);
			//AreaReturn InLoopArea = Create_Area(LoopIndex, StartLoop.GetConnection());

			//CmdNode EndLoop = field.CreateAndConnectNode("End Loop", CMD.END_LOOP, InLoopArea.AreaEnds);
			//InLoopArea.AreaEnds = EndLoop.GetConnection();
			//return InLoopArea;
			return new AreaReturn();
		}

		private AreaReturn Handle_IfElse(int IfIndex, List<CmdNode> From)
		{
			return new AreaReturn();

		}

		private int CountCommandsInZone(int SOZ, int EOZ, CMD type)
		{
			if (Commands[SOZ].type != CMD.SOZ || Commands[EOZ].type != CMD.EOZ || SOZ < EOZ)
				return -1;

			int CountType = 0;
			for (int i = SOZ; i < EOZ; ++i)
				if (Commands[i].type == type)
					++CountType;

			return CountType;
		}
		private int FindEndOfZone(int SOZIndex)
		{
			int OpenedGates = 1;
			++SOZIndex;
			while(OpenedGates > 0)
			{
				if (Commands[SOZIndex].type == CMD.SOZ)
					++OpenedGates;
				else if (Commands[SOZIndex].type == CMD.EOZ)
					--OpenedGates;
				++SOZIndex;
			}
			if (Commands[--SOZIndex].type != CMD.EOZ)
				return -6;
			return SOZIndex;
		}
		private void MergeAreaEndList(List<CmdNode> Result, List<CmdNode> FromTaken) 
		{
			for(int i = 0; i < FromTaken.Count; ++i) 
			{
			Result.Add(FromTaken[i]);
			}
			FromTaken.Clear();
		}

		Visio.Application Application;
		Visio.Page ActivePage;
	}
}
