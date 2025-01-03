using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Media3D;
using Visio = Microsoft.Office.Interop.Visio;

namespace FlowchartGenerator
{
	internal class StyleserSystem : SubSystem
	{
		private static Logger LOG = new Logger("Styleser_System");
		public bool Style_Arrows()
		{
			DiagramData stats = Diagram.Stats;
			foreach (Visio.Shape con in stats.GetAllSpawnedConnectors())
			{
				con.Cells["LineWeight"].Formula = "2 pt";
				con.Cells["EndArrow"].Formula = "4";
				con.Cells["CompoundType"].Formula = "3";
				con.Cells["Rounding"].Formula = "0.01";
				con.Cells["EndArrowSize"].Formula = "1";
				con.Cells["LockHeight"].Formula = "0";
				con.Cells["LockCalcWH"].Formula = "0";
				con.Cells["LockHeight"].FormulaForce = "FALSE";
				con.Cells["LockRotate"].FormulaForce = "FALSE";
				con.Cells["LockVtxEdit"].FormulaForce = "FALSE";
			}
			if(stats.GetAllConnectorsWithTag(DiagramData.ConTag.ToElsePoint) != null)
			foreach(DiagramData.ConnectorsStatsData conData in stats.GetAllConnectorsWithTag(DiagramData.ConTag.ToElsePoint))
			{
				conData.shape.Cells["EndArrow"].Formula = "0";
			}


			return true;
		}
		public bool Style_Shapes()
		{
			DiagramData stats = Diagram.Stats;
#if DEBUG
			foreach (CmdNode e in Diagram.Stats.GetAllSpawnedNodes())
			{
              //  e.GetShape().SetShapeText(e.GetText() + $"\n[{e.GetLocation().X} : {e.GetLocation().Y}]");
			}
#endif
			foreach (CmdNode node in Diagram.Stats.GetAllSpawnedNodes())
			{
				node.GetShape().SetCellParameter("LineWeight", "2 pt");
			}

			if(stats.GetAllSpawnedNodesByCMD(CMD.END_LOOP) != null)
			foreach (CmdNode LoopEnd in stats.GetAllSpawnedNodesByCMD(CMD.END_LOOP))
			{
				LoopEnd.GetShape().SetCellParameter("FlipY", "TRUE");
				LoopEnd.GetShape().SetCellParameter("Width", "25 mm");
				LoopEnd.GetShape().SetCellParameter("Height", "15 mm");
			}

            if (stats.GetAllSpawnedNodesByCMD(CMD.BREAK) != null)
                foreach (CmdNode LoopEnd in stats.GetAllSpawnedNodesByCMD(CMD.BREAK))
                {
                    LoopEnd.GetShape().SetCellParameter("Width", "10 mm");
                    LoopEnd.GetShape().SetCellParameter("Height", "0.01 mm");
                    LoopEnd.GetShape().SetCellParameter("Geometry1.NoShow", "TRUE");
                    LoopEnd.GetShape().SetCellParameter("TxtPinY", "=Height*0.5+0.083");
                }

            if (stats.GetAllSpawnedNodesByCMD(CMD.LOOP) != null)
                foreach (CmdNode LoopStart in stats.GetAllSpawnedNodesByCMD(CMD.LOOP))
			{
				LoopStart.GetShape().SetCellParameter("Width", "25 mm");
				LoopStart.GetShape().SetCellParameter("Height", "15 mm");
			}
            if (stats.GetAllSpawnedNodesByCMD(CMD.ELSE) != null)
                foreach (CmdNode LoopStart in stats.GetAllSpawnedNodesByCMD(CMD.ELSE))
                {
					LoopStart.GetShape().SetShapeText("else");
                }

            return true;
		}
	}
}
