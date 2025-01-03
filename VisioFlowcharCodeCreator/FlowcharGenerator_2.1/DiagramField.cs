using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Visio = Microsoft.Office.Interop.Visio;

namespace FlowchartGenerator
{
	public class DiagramField
	{
		//Stats
		public Dictionary<CMD, List<CmdNode>> TypedShapesRefs = new Dictionary<CMD, List<CmdNode>>();
		public List<CmdNode> SpawnedShapes = new List<CmdNode>();
		public List<Visio.Shape> SpawnedArrows = new List<Visio.Shape>();
		public Dictionary<int, List<Visio.Shape>> SpawnedConnectorsByType = new Dictionary<int, List<Visio.Shape>>();

		//Members
		static Logger LOG = new Logger("Field");
		private static Visio.Application Application;
		private static Visio.Page Page;

		//Values
		private Vector2D BaseGridStep;

		//Constructor
		public DiagramField(Visio.Application app, Visio.Page page, Vector2D step)
		{
			Application = app;
			Page = page;
			BaseGridStep = step;
		}

		//Methods
		public CmdNode CreateAndConnectNode(Command cmd, List<From_Connection> From)
		{
			CmdNode node = CreateCmdNode(cmd.text, cmd.type, FindNextLogicalLocation(From));
			ConnectCmdShapesBase(From, node);
			return node;
		}
		public CmdNode CreateAndConnectNode(string text, CMD type, List<From_Connection> From)
		{
			CmdNode node = CreateCmdNode(text, type, FindNextLogicalLocation(From));
			ConnectCmdShapesBase(From, node);

			return node;
		}
		public CmdNode CreateCmdNode(Command cmd, Vector2D loc)
		{
			int id = SpawnedShapes.Count;
			CmdNode node = new CmdNode(cmd.text, cmd.type, loc, id);
			if (TypedShapesRefs.ContainsKey(cmd.type))
			{
				TypedShapesRefs[cmd.type].Add(node);
			}
			else
			{
				TypedShapesRefs.Add(cmd.type, new List<CmdNode>());
				TypedShapesRefs[cmd.type].Add(node);
			}
			SpawnedShapes.Add(node);

			return node;
		}
		public CmdNode CreateCmdNode(string text, CMD type, Vector2D loc)
		{
			int id = SpawnedShapes.Count;
			CmdNode node = new CmdNode(text, type, loc, id);

			if (TypedShapesRefs.ContainsKey(type))
			{
				TypedShapesRefs[type].Add(node);
			}
			else
			{
				TypedShapesRefs.Add(type, new List<CmdNode>());
				TypedShapesRefs[type].Add(node);
			}
			SpawnedShapes.Add(node);

			return node;
		}
		public void ConnectCmdShapesBase(List<From_Connection> From, CmdNode To)
		{
			if (From == null || To == null) return;
			for (int i = 0; i < From.Count; ++i)
				ConnectCmdShapesBase(From[i], To);
		}
		public Visio.Shape ConnectCmdShapesBase(From_Connection From, CmdNode To)
		{
			if (From.FromNode == null || To == null) return null;
			if (From.FromNode.GetCMDType == CMD.RETURN) return null;

			bool isFromHigher = false;
			if (From.FromNode.GetLocation().Y >= To.GetLocation().Y)
				isFromHigher = true;

			Visio.Master MCon = ShapeMaster.GetConnectorMasterByConTypes(From.FromConnectionType, ConType.Top, isFromHigher);
			Visio.Shape CreatedConnector;
			if (From.FromConnectionType == ConType.Left || From.FromConnectionType == ConType.Right)
			{
				CreatedConnector = Shape.ConnectSideToTop(From, To.GetShape(), MCon);
			}
			else
			{
				CreatedConnector = Shape.ConnectBottomToTop(From, To.GetShape(), MCon);
			}

			if (CreatedConnector == null)
			{
				LOG.Write($"Failed connector creation between : {From.FromNode.GetText()} -> {To.GetText()}");
				return null;
			}
			CreatedConnector.Text = From.Text;
			int conHash = ShapeMaster.GetConnectionHash(From.FromConnectionType, ConType.Top, isFromHigher);
			if (SpawnedConnectorsByType.ContainsKey(conHash))
				SpawnedConnectorsByType[conHash].Add(CreatedConnector);
			else
				SpawnedConnectorsByType.Add(conHash, new List<Visio.Shape>() { CreatedConnector });

			SpawnedArrows.Add(CreatedConnector);
			return CreatedConnector;
		}
		public Vector2D FindNextLogicalLocation(List<From_Connection> From)
		{
			if (From.Count == 0)
				throw new Exception("FG - Error : Fault in calculating next location - Zero connection roots");
			Vector2D newLoc = new Vector2D(0, From[0].FromNode.GetLocation().Y);
			for (int i = 0; i < From.Count; ++i)
			{
				newLoc.X += From[i].FromNode.GetLocation().X;
				if (newLoc.Y > From[i].FromNode.GetLocation().Y)
				{
					newLoc.Y = From[i].FromNode.GetLocation().Y;
				}
			}
			--newLoc.Y;
			newLoc.X /= From.Count;
			return newLoc;
		}
		public Vector2D GetVisioCoordinatesByIndLoc(Vector2D loc)
		{
			loc.Y *= BaseGridStep.Y;
			loc.X *= BaseGridStep.X;
			return loc;
		}
	}
}
