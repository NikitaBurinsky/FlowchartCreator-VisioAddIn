using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FlowchartGenerator.AreaHandlers
{
	internal class IfElse_Handler : AreaHandler
	{
		public IfElse_Handler(List<Command> commands, CMD type = CMD.IF) : base(commands) { AreaType = type; }

		private List<AreaHandler> ElseIfHandlers = new List<AreaHandler>();

		public override bool HandleArea(int ZoneRootIndex, out int EOZ)
	{
			AreaType = Commands[ZoneRootIndex].type;
			Vector2D CurNodeLoc = new Vector2D(0, 0);
			CmdNode IfNode = Diagram.CreateCmdNode(Commands[ZoneRootIndex], CurNodeLoc);
			CmdNode LastNoZoneElseNode = IfNode;
            AreaRoot = IfNode;
			AreaNodes.Add(IfNode);

			BaseArea_Handler IFAreaHandler = new BaseArea_Handler(Commands);
			UseSubAreaHandler(IFAreaHandler, ZoneRootIndex, out EOZ);
			OutputNodes.AddRange(IFAreaHandler.OutputNodes);
			CurNodeLoc.Plus(IFAreaHandler.GetWidth()/-2, -1);
			IFAreaHandler.SetZoneLocationByRootLocation(CurNodeLoc);
			Diagram.ConnectCmdShapesBase(IfNode.GetConnection(ConType.Left, "Да"), IFAreaHandler.AreaRoot);
			CurNodeLoc.Plus(IFAreaHandler.GetWidth(), 1);

			int IsHaveElse;
			List<int> ElseIfBranches = FindEOZ_And_ElseBranches(ZoneRootIndex, out IsHaveElse, out EOZ);
			if (ElseIfBranches.Count == 0 && IsHaveElse == -1 && AreaType != CMD.ELSEIF)
			{
				OutputNodes.Add(new From_Connection(AreaRoot, ConType.Right));
				return true;
			}
			else
			{
				Handle_ELSEIF_Cases(ElseIfBranches, ref CurNodeLoc, ref LastNoZoneElseNode);
				Handle_ELSE_Case_ifExist(IsHaveElse, ref CurNodeLoc, ref EOZ, ref LastNoZoneElseNode);
			}
			RECalculateAreaSizeForce();
			return true;
		}
        private void Handle_ELSEIF_Cases(List<int> ElseIfBranches, ref Vector2D CurNodeLoc, ref CmdNode LastNoZoneElseNode)
        {
            foreach (int ElseIfIndex in ElseIfBranches)
            {
                CurNodeLoc.Y -= 1;
				int CurElseIfEOZ = FindEOZ(ElseIfIndex);

                IfElse_Handler CurElseIfHandler = new IfElse_Handler(Commands, CMD.ELSEIF);
				ElseIfHandlers.Add(CurElseIfHandler);
                UseSubAreaHandler(CurElseIfHandler, ElseIfIndex, out CurElseIfEOZ);
                CurElseIfHandler.SetZoneLocationByRootLocation(CurNodeLoc + new Vector2D(CurElseIfHandler.GetWidth()/2, 0));
                CurNodeLoc.X += CurElseIfHandler.GetWidth();
				Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.GetConnection(ConType.Right, "Нет"), CurElseIfHandler.AreaRoot);
                LastNoZoneElseNode = CurElseIfHandler.AreaRoot;
				OutputNodes.AddRange(CurElseIfHandler.OutputNodes);
            }
        }

        private bool Handle_ELSE_Case_ifExist(int ElseIndex, ref Vector2D CurNodeLoc, ref int ElseEOZ, ref CmdNode LastNoZoneElseNode)
        {
			if (AreaType == CMD.ELSEIF) 
				return true;

			if (ElseIndex != -1)
            {
				CurNodeLoc.Y -= 1;
				BaseArea_Handler ElseHandler = new BaseArea_Handler(Commands);
                UseSubAreaHandler(ElseHandler, ElseIndex, out ElseEOZ);
                ElseHandler.SetZoneLocationByRootLocation(CurNodeLoc + new Vector2D((ElseHandler.GetWidth() / 2), 0));
                CurNodeLoc.Plus(ElseHandler.GetWidth(), 0); 
                OutputNodes.AddRange(ElseHandler.OutputNodes);//!!!
				Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.GetConnection(ConType.Right, "Else"), ElseHandler.AreaRoot);
                return true;
            }
			else
			{
				OutputNodes.Add(new From_Connection(LastNoZoneElseNode, ConType.Right, "Else"));
			}

			return false;
        }

        private List<int> FindEOZ_And_ElseBranches(int ZoneRootIndex, out int IsHaveElse, out int EOZ)
		{
            List<int> ElseIfBranches;
            if (AreaType == CMD.ELSEIF)
            {
                ElseIfBranches = new List<int>();
                IsHaveElse = -1;
                EOZ = FindEOZ(ZoneRootIndex);
            }
            else
            {
                ElseIfBranches = FindDecisionCasesAndEOZ(ZoneRootIndex, out IsHaveElse, out EOZ);
            }
			return ElseIfBranches;
        }

		private List<int> FindDecisionCasesAndEOZ(int ZoneRootIndex, out int IsHaveElse, out int EOZ)
		{
			List<int> BranchesFound = new List<int>();
			int Iter = ZoneRootIndex;
			while (true)
			{
				Iter = base.FindEOZ(Iter);
				++Iter;
				if (Commands[Iter].type == CMD.ELSE)
				{
					IsHaveElse = Iter;
					EOZ = base.FindEOZ(Iter);
					return BranchesFound;
				}
				else if (Commands[Iter].type == CMD.ELSEIF)
				{
					BranchesFound.Add(Iter);
				}
				else
				{
					EOZ = Iter - 1;
					IsHaveElse = -1;
					return BranchesFound;
				}
			}
		}



	}
}
