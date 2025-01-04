using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowchartGenerator.AreaHandlers
{
	internal class SwitchCase_Handler : AreaHandler
	{
		public SwitchCase_Handler(List<Command> commands) :
			base(commands) { AreaType = CMD.SWITCH; }

		public override bool HandleArea(int ZoneRootIndex, out int EOZ)
		{
			if (Commands[ZoneRootIndex].type != CMD.SWITCH)
			{
				throw new Exception("ZoneRootIndex is not SOZ");
			}

			Vector2D CurNodeLoc = new Vector2D(0, 0);
			CmdNode SwitchNode = CreateCmdNode(Commands[ZoneRootIndex], CurNodeLoc);
			AreaRoot = SwitchNode;
			int EOZIndex;
			List<int> CasesFound = FindCasesAndEOZ(ZoneRootIndex, out EOZIndex);
			if(CasesFound.Count == 0)
			{
				EOZ = ZoneRootIndex + 2;
				return false;
			}
			List<AreaHandler> CasesAreas = new List<AreaHandler> { };

			CurNodeLoc.Y -= 1;

			int CaseEoZ = FindEOZ(CasesFound[0]);
			foreach (int CaseIndex in CasesFound)
			{
				BaseArea_Handler CurAreaHandler = new BaseArea_Handler(Commands);
				CreateInternalArea(CurAreaHandler, CaseIndex, out CaseEoZ);
				Diagram.ConnectCmdShapesBase(new From_Connection(SwitchNode, ConType.Bottom), CurAreaHandler.AreaRoot);
				CurAreaHandler.SetZoneLocationByRootLocation(CurNodeLoc);
				CurNodeLoc.X += CurAreaHandler.GetWidth();
				OutputNodes.AddRange(CurAreaHandler.OutputNodes);
			}
			RECalculateAreaSizeForce();
			float deltaXCaseLoc = (GetWidth() - 1) / -2;
			CurNodeLoc = SwitchNode.GetLocation();
			AddZoneLocationOffset(new Vector2D(deltaXCaseLoc, 0));
			SwitchNode.SetLocation(CurNodeLoc);
			EOZ = EOZIndex;
			return true;
		}

		private List<int> FindCasesAndEOZ(int ZoneRootIndex, out int EOZIndex)
		{
			List<int> CaseIndexes =  new List<int>();
			int CInd = ZoneRootIndex + 2;
			for (int OpenedGates = 1; OpenedGates > 0; ++CInd)//Change to Switch (TEST)
			{
				if (Commands[CInd].type == CMD.SOZ)
					++OpenedGates;
				else if (Commands[CInd].type == CMD.EOZ)
					--OpenedGates;//Возможно дело в ретурне
				else if (Commands[CInd].type == CMD.CASE || Commands[CInd].type == CMD.DEFAULT_SWITCH)
					CaseIndexes.Add(CInd);
			}

			EOZIndex = CInd;
			return CaseIndexes;
		}
	}
}
