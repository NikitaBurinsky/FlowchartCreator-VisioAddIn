using System.Collections.Generic;

namespace FlowchartGenerator.AreaHandlers
{
	internal class BaseArea_Handler : AreaHandler
	{
		public BaseArea_Handler(List<Command> commands) : base(commands) { AreaType = CMD.NONE; }

		public override bool HandleArea(int ZoneRootIndex, out int EOZ)
		{
			if (Commands[ZoneRootIndex + 1].type != CMD.SOZ)
			{
				EOZ = -1;
				throw new System.Exception("ZoneRootIndex + 1 != SOZ");
			}

			int Iter = ZoneRootIndex + 1;
			List<From_Connection> FromConnects = null;
			int EndOfZone = FindEOZ(ZoneRootIndex);
			Vector2D NextNodeLocation = new Vector2D(0, 0);
			AreaHandler areaHandler;
			Command curCommand;
			CmdNode cmdNode = null;

			while (Iter < EndOfZone)
			{
				curCommand = Commands[Iter];//debug
				int PrevIter = Iter;
				if (IsOpenZone(Commands[Iter]))
				{
					if (curCommand.type == CMD.IF ||
						curCommand.type == CMD.SWITCH ||
						curCommand.type == CMD.LOOP)
					{
						areaHandler = AreaHandler.GetAreaHandler(Iter, Commands);
						UseSubAreaHandler(areaHandler, Iter, out Iter);
						areaHandler.SetZoneLocationByRootLocation(NextNodeLocation);
						Diagram.ConnectCmdShapesBase(FromConnects, areaHandler.AreaRoot);
						if (AreaRoot == null)
						{
							AreaRoot = areaHandler.AreaRoot;
						}
						NextNodeLocation.X = Diagram.FindNextLogicalLocation(areaHandler.OutputNodes).X;
						NextNodeLocation.Y -= areaHandler.GetHeight() ;
						FromConnects = areaHandler.OutputNodes;
					}
				}
				else
				{
					if (Commands[Iter].type != CMD.SOZ && Commands[Iter].type != CMD.EOZ)
					{
						if (AreaRoot == null)
						{
							cmdNode = Diagram.CreateCmdNode(Commands[Iter].text, Commands[Iter].type, NextNodeLocation);
							AreaRoot = cmdNode;
							NextNodeLocation = AreaRoot.GetLocation();
							NextNodeLocation.Y -= 1;
							Diagram.ConnectCmdShapesBase(FromConnects, cmdNode);
							FromConnects = new List<From_Connection> { new From_Connection(cmdNode, ConType.Bottom) };
						}
						else
						{
							cmdNode = Diagram.CreateCmdNode(Commands[Iter].text, Commands[Iter].type, NextNodeLocation);
							NextNodeLocation.Y -= 1;
							Diagram.ConnectCmdShapesBase(FromConnects, cmdNode);
							FromConnects = new List<From_Connection> { new From_Connection(cmdNode, ConType.Bottom) };
						}
						AreaNodes.Add(cmdNode);
						if (cmdNode.GetCMDType == CMD.BREAK)
							OutputNodes.Add(new From_Connection(cmdNode, ConType.Bottom));
					}
				}
				++Iter;
			}
			RECalculateAreaSizeForce();
			OutputNodes = FromConnects;
			EOZ = EndOfZone;
			return true;

		}






	}
}
