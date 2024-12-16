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
			List<Visio.Shape> Arrows = GetArrowsShapes();
			foreach (Visio.Shape arrow in Arrows)
			{
				arrow.Cells["LineWeight"].Formula = "2 pt";
				arrow.Cells["EndArrow"].Formula = "4";
				arrow.Cells["CompoundType"].Formula = "3";
				arrow.Cells["Rounding"].Formula = "0.01";
				arrow.Cells["EndArrowSize"].Formula = "1";
				arrow.Cells["LockHeight"].Formula = "0";
				arrow.Cells["LockCalcWH"].Formula = "0";
				arrow.Cells["LockHeight"].FormulaForce = "FALSE";
				arrow.Cells["LockRotate"].FormulaForce = "FALSE";
				arrow.Cells["LockVtxEdit"].FormulaForce = "FALSE";
			}
			return true;
		}
		public bool Style_Shapes()
		{
#if DEBUG
			foreach(CmdNode e in Diagram.SpawnedShapes)
			{
              //  e.GetShape().SetShapeText(e.GetText() + $"\n[{e.GetLocation().X} : {e.GetLocation().Y}]");
			}
#endif
			foreach (CmdNode node in Diagram.SpawnedShapes)
			{
				node.GetShape().SetCellParameter("LineWeight", "2 pt");
			}

			if(GetFigureShapesByType(CMD.END_LOOP) != null)
			foreach (CmdNode LoopEnd in GetFigureShapesByType(CMD.END_LOOP))
			{
				LoopEnd.GetShape().SetCellParameter("FlipY", "TRUE");
				LoopEnd.GetShape().SetCellParameter("Width", "25 mm");
				LoopEnd.GetShape().SetCellParameter("Height", "15 mm");
			}

            if (GetFigureShapesByType(CMD.BREAK) != null)
                foreach (CmdNode LoopEnd in GetFigureShapesByType(CMD.BREAK))
                {
                    LoopEnd.GetShape().SetCellParameter("Width", "10 mm");
                    LoopEnd.GetShape().SetCellParameter("Height", "0.01 mm");
                    LoopEnd.GetShape().SetCellParameter("Geometry1.NoShow", "TRUE");
                    LoopEnd.GetShape().SetCellParameter("TxtPinY", "=Height*0.5+0.083");
                }

            if (GetFigureShapesByType(CMD.LOOP) != null)
                foreach (CmdNode LoopStart in GetFigureShapesByType(CMD.LOOP))
			{
				LoopStart.GetShape().SetCellParameter("Width", "25 mm");
				LoopStart.GetShape().SetCellParameter("Height", "15 mm");
			}
            if (GetFigureShapesByType(CMD.ELSE) != null)
                foreach (CmdNode LoopStart in GetFigureShapesByType(CMD.ELSE))
                {
                   //LoopStart.GetShape().SetCellParameter("Width", "10 mm");
                   //LoopStart.GetShape().SetCellParameter("Height", "5 mm");
					LoopStart.GetShape().SetShapeText("else");
                }

            return true;
		}

		//Private
		private List<Visio.Shape> GetArrowsShapes()
		{
			return Diagram.SpawnedArrows;
		}

		private List<CmdNode> GetFigureShapesByType(CMD type)
		{
			if(Diagram.TypedShapesRefs.ContainsKey(type))
			return Diagram.TypedShapesRefs[type];
			else
				return null;
		}
		private List<Visio.Shape> GetArrowShapesByType(int conHash)
		{
			if (Diagram.SpawnedConnectorsByType.ContainsKey(conHash))
				return Diagram.SpawnedConnectorsByType[conHash];
			else
				return null;
		}




	}
}
