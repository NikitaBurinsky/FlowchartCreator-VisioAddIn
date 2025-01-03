using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowchartGenerator.AreaHandlers
{
	internal class BaseLoop_Handler : AreaHandler
	{
		public BaseLoop_Handler(List<Command> commands) : base(commands) { }

		public override bool HandleArea(int ZoneRootIndex, out int EOZ )
		{
			EOZ = -2;
			if (Commands[ZoneRootIndex].type != CMD.LOOP && Commands[ZoneRootIndex].type != CMD.DO_LOOP)
			{
				throw new Exception("ZoneRootIndex.type not LOOP");
			}
			//Start Loop
			CmdNode StartLoopNode = CreateCmdNode(Commands[ZoneRootIndex], new Vector2D(0,0));
			AreaRoot = StartLoopNode;
			Vector2D CurNodeLoc = new Vector2D(0, -1);
			List<From_Connection> ToEndNode = null;
			//Loop area
			if (Commands[ZoneRootIndex + 2].type != CMD.EOZ)
			{
				BaseArea_Handler LoopArea = new BaseArea_Handler(Commands);
				CreateInternalArea(LoopArea, ZoneRootIndex, out EOZ);
				LoopArea.SetZoneLocationByRootLocation(CurNodeLoc);
				Diagram.ConnectCmdShapesBase(new From_Connection(AreaRoot, ConType.Bottom), LoopArea.AreaRoot);
				CurNodeLoc.Y -= GetHeight() - 1;
				ToEndNode = LoopArea.OutputNodes;
			}
			else
			{
				EOZ = ZoneRootIndex + 2;
				ToEndNode = new List<From_Connection>() { StartLoopNode.CreateFromCon(ConType.Bottom) };
			}
			//End area
			string EndLoopText = "End Loop";
			if (Commands[EOZ].text.Contains("while"))
				EndLoopText = Commands[EOZ].text;
			CmdNode EndLoop = CreateCmdNode(EndLoopText, CMD.END_LOOP, CurNodeLoc);
			Diagram.ConnectCmdShapesBase(ToEndNode, EndLoop);
			OutputNodes.Add(new From_Connection(EndLoop, ConType.Bottom));
			
			RECalculateAreaSizeForce();
			return true;
		}



	}
}
