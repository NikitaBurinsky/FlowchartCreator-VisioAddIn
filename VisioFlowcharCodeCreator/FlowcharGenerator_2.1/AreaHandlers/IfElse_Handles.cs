using System.Collections.Generic;
using Visio = Microsoft.Office.Interop.Visio;



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
			CmdNode IfNode = CreateCmdNode(Commands[ZoneRootIndex], CurNodeLoc);
			CmdNode LastNoZoneElseNode = IfNode;
            AreaRoot = IfNode;

			BaseArea_Handler IFAreaHandler = new BaseArea_Handler(Commands);
			CreateInternalArea(IFAreaHandler, ZoneRootIndex, out EOZ);
            CurNodeLoc.Plus(IFAreaHandler.GetWidth()/-2, -1);
            IFAreaHandler.SetZoneLocationByRootLocation(CurNodeLoc);
            OutputNodes.AddRange(IFAreaHandler.OutputNodes);
			Diagram.ConnectCmdShapesBase(IfNode.CreateFromCon(ConType.Left, "Да"), IFAreaHandler.AreaRoot);
			CurNodeLoc.Plus(IFAreaHandler.GetWidth(), 1);

			int IsHaveElse;
			List<int> ElseIfBranches = FindEOZ_And_ElseBranches(ZoneRootIndex, out IsHaveElse, out EOZ);
			if (ElseIfBranches.Count == 0 && IsHaveElse == -1 && AreaType != CMD.ELSEIF)
			{
				CmdNode ElseDot = Create_ELSE_Point(LastNoZoneElseNode);
			}
			else
			{
				Handle_ELSEIF_Cases_IfExist(ElseIfBranches, ref CurNodeLoc, ref LastNoZoneElseNode);
				Handle_ELSE_Case_ifExist(IsHaveElse, ref CurNodeLoc, ref EOZ, ref LastNoZoneElseNode);
			}
			RECalculateAreaSizeForce();
			return true;
		}


		private CmdNode Create_ELSE_Point(CmdNode LastNoZoneElseNode)
		{
			Vector2D elseLoc = LastNoZoneElseNode.GetLocation() + new Vector2D(1, 0);
            CmdNode ElseDot = CreateCmdNode(null, CMD.ELSE, elseLoc);
			ElseDot.GetShape().SetSize(new Vector2D(0.01f, 0.01f));
			Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.CreateFromCon(ConType.Right, "Else"), ElseDot, DiagramData.ConTag.ToElsePoint);
			OutputNodes.Add(new From_Connection(ElseDot, ConType.Bottom));
			return ElseDot;
        }
        private void Handle_ELSEIF_Cases_IfExist(List<int> ElseIfBranches, ref Vector2D CurNodeLoc, ref CmdNode LastNoZoneElseNode)
        {
            foreach (int ElseIfIndex in ElseIfBranches)
            {
                CurNodeLoc.Y -= 1;
				int CurElseIfEOZ = FindEOZ(ElseIfIndex);

                IfElse_Handler CurElseIfHandler = new IfElse_Handler(Commands, CMD.ELSEIF);
				ElseIfHandlers.Add(CurElseIfHandler);
                CreateInternalArea(CurElseIfHandler, ElseIfIndex, out CurElseIfEOZ);
                CurElseIfHandler.SetZoneLocationByRootLocation(CurNodeLoc + new Vector2D(CurElseIfHandler.GetWidth()/2, 0));
                CurNodeLoc.X += CurElseIfHandler.GetWidth();
				Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.CreateFromCon(ConType.Right, "Нет"), CurElseIfHandler.AreaRoot);
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
                CreateInternalArea(ElseHandler, ElseIndex, out ElseEOZ);
                ElseHandler.SetZoneLocationByRootLocation(CurNodeLoc + new Vector2D((ElseHandler.GetWidth() / 2), 0));
                CurNodeLoc.Plus(ElseHandler.GetWidth(), 0); 
                OutputNodes.AddRange(ElseHandler.OutputNodes);//!!!
				Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.CreateFromCon(ConType.Right, "Else"), ElseHandler.AreaRoot);
                return true;
            }
			else
			{
				CmdNode ElseDot = Diagram.CreateCmdNode(null, CMD.ELSE, LastNoZoneElseNode.GetLocation() + new Vector2D(0.5f, 0));
				ElseDot.GetShape().SetCellParameter("Width", "0.01 mm");
				ElseDot.GetShape().SetCellParameter("Height", "0.01 mm");
				Diagram.ConnectCmdShapesBase(LastNoZoneElseNode.CreateFromCon(ConType.Right), ElseDot);
				AreaNodes.Add(ElseDot);
				OutputNodes.Add(new From_Connection(ElseDot, ConType.Bottom));
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
