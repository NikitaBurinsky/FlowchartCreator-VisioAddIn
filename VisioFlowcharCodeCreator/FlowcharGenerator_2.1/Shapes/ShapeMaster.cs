using System;
using System.Collections.Generic;
using Visio = Microsoft.Office.Interop.Visio;


namespace FlowchartGenerator
{

	public static class ShapeMaster
	{
		private static Visio.Application Application;
		private static bool SHInitialised = false;

		private static Dictionary<CMD, Visio.Master> MShapes;
		private static Dictionary<int, Visio.Master> MConnections;
		private static Visio.Documents Docs;
		private static Visio.Document BasicFlowchartShapes_Stencil;
		private static Visio.Document Connectors_Stencil = null;
		private static Visio.Document BasicShapes_Stencil;
		public static void Startup(Visio.Application app)
		{
			if (SHInitialised) return;
			SHInitialised = true;
			Application = app;

			if (MConnections != null)
				return;
			MShapes = new Dictionary<CMD, Visio.Master>();
			MConnections = new Dictionary<int, Visio.Master>();
			Docs = Application.Documents;
			BasicFlowchartShapes_Stencil = Docs.OpenEx("Basic Flowchart Shapes.vss", (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visOpenDocked);
			BasicShapes_Stencil = Docs.OpenEx("Basic Shapes.vss", (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visOpenDocked);

			//SHAPES
			MShapes.Add(CMD.IF, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Decision"));
			MShapes.Add(CMD.ELSEIF, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Decision"));
			MShapes.Add(CMD.ELSE, BasicShapes_Stencil.Masters.get_ItemU("Circle"));
			MShapes.Add(CMD.SWITCH, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Decision"));

			MShapes.Add(CMD.LOOP, BasicShapes_Stencil.Masters.get_ItemU("Snip Same Side Corner Rectangle"));
			MShapes.Add(CMD.DO_LOOP, BasicShapes_Stencil.Masters.get_ItemU("Snip Same Side Corner Rectangle"));
			MShapes.Add(CMD.END_LOOP, BasicShapes_Stencil.Masters.get_ItemU("Snip Same Side Corner Rectangle"));//Rotate

			MShapes.Add(CMD.SUBPROCESS, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Subprocess"));
			MShapes.Add(CMD.PROCESS, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Process"));

			MShapes.Add(CMD.INPUT, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Data"));
			MShapes.Add(CMD.OUTPUT, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Data"));
			MShapes.Add(CMD.StartFunc, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Start/End"));
			MShapes.Add(CMD.RETURN, BasicFlowchartShapes_Stencil.Masters.get_ItemU("Start/End"));
			MShapes.Add(CMD.BREAK, BasicFlowchartShapes_Stencil.Masters.get_ItemU("On-page reference"));

			//CONNECTORS
			Connectors_Stencil = Docs.OpenEx("Connectors.vss", (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visOpenDocked);
			if (Connectors_Stencil == null)
				return;

			MConnections.Add(-1, Connectors_Stencil.Masters.get_ItemU("Dynamic connector"));
			MConnections.Add(GetConnectionHash(ConType.Bottom, ConType.Top, true), Connectors_Stencil.Masters.get_ItemU("Bottom to top 1"));
			MConnections.Add(GetConnectionHash(ConType.Bottom, ConType.Top, false), Connectors_Stencil.Masters.get_ItemU("Bottom to top 2"));
			MConnections.Add(GetConnectionHash(ConType.Right, ConType.Top, true), Connectors_Stencil.Masters.get_ItemU("Side to top/bottom"));
			MConnections.Add(GetConnectionHash(ConType.Left, ConType.Top, true), Connectors_Stencil.Masters.get_ItemU("Side to top/bottom"));
			MConnections.Add(GetConnectionHash(ConType.Right, ConType.Top, false), Connectors_Stencil.Masters.get_ItemU("Side to top"));
			MConnections.Add(GetConnectionHash(ConType.Left, ConType.Top, false), Connectors_Stencil.Masters.get_ItemU("Side to top"));
		}

		public static int GetConnectionHash(ConType FromT, ConType ToT, bool FromHigher)
		{
			int ConHash = (((int)FromT * 10) + (int)ToT) * 10 + System.Convert.ToInt32(FromHigher);
			return ConHash;
		}



		public static Visio.Master GetMasterShapeByCMD(CMD type)
		{
			if (MShapes.ContainsKey(type))
				return MShapes[type];
			else
				throw new Exception("Invalid Shape Type found");
		}

		public static Visio.Master GetConnectorMasterByConTypes(ConType FromT, ConType ToT, bool IsFromHigher)
		{
			//return MConnections[-1];//TEST
			int hash = GetConnectionHash(FromT, ToT, IsFromHigher);
			if (MConnections.ContainsKey(hash))
				return MConnections[hash];
			else
				throw new Exception("Invalid Connection Types found");
		}


	}
}
