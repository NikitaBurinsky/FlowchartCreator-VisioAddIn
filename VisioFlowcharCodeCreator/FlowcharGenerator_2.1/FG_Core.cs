using System;
using System.Collections.Generic;
using System.IO;
using Visio = Microsoft.Office.Interop.Visio;

namespace FlowchartGenerator
{
	public class FG_Core
	{
		static Logger LOG = new Logger("Core");
		//COMPONENTS
		public SettingsSystem FGSettings = new SettingsSystem();

		private DiagramField Diagram;
		private StyleserSystem Styliser = new StyleserSystem();
		private List<Command> Commands { get; set; }

		//MEMBERS

		Visio.Application Application;
		Visio.Page ActivePage;


		public bool InitialiseSystems(Visio.Application app, Visio.Page page, string textBufferPath)
		{
			LOG.Write("Initialise Systems : Start");
			ActivePage = page;
			Application = app;
			SubSystem.Settings = FGSettings;
			Diagram = new DiagramField(Application, ActivePage, new Vector2D(40, 25));
			ShapeMaster.Startup(Application);
			SubSystem.InitClass(Diagram, page, app, this);
			LOG.Write("Initialise Systems : Finish");
			return true;
		}

		public int GenerateDiagram(int StartLocation_X, int StartLocation_Y, List<Command> CommandS)
		{
				LOG.Write("Generate diagram : Start");
				Commands = CommandS;	
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
				Diagram.ConnectCmdShapesBase(StartNode.CreateFromCon(ConType.Bottom), StartArea.AreaRoot);

				LOG.Write("Stylise : Start");
				Styliser.Style_Arrows();
				Styliser.Style_Shapes();
				LOG.Write("Stylise : Success");
			return 1;
		}
	}
}
