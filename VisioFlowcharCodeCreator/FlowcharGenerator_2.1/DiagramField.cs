using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Visio = Microsoft.Office.Interop.Visio;

namespace FlowchartGenerator
{
	public class DiagramData
	{
		public enum ConTag : Int16 { ToElsePoint , None};
		public struct ConnectorsStatsData
		{
			public From_Connection From;
			public CmdNode To;
			public Visio.Shape shape;
			public string Tag;
		}
		//Stats
		private Dictionary<CMD, List<CmdNode>> SpawnedNodesByType = new Dictionary<CMD, List<CmdNode>>();
		private List<CmdNode> SpawnedNodes = new List<CmdNode>();
		private List<Visio.Shape> SpawnedConnectors = new List<Visio.Shape>();
		private Dictionary<ConTag, List<ConnectorsStatsData>> SpawnedConnectorsByTag = new Dictionary<ConTag, List<ConnectorsStatsData>>();

		//Methods
		public List<CmdNode> GetAllSpawnedNodes() { return SpawnedNodes; }
		public List<Visio.Shape> GetAllSpawnedConnectors() { return SpawnedConnectors; }
		public void AddConnectorToStats(Visio.Shape connector, From_Connection From, CmdNode To, ConTag Tag = ConTag.None)
		{
			ConnectorsStatsData conData = new ConnectorsStatsData { shape = connector, To = To, From = From};
			if (SpawnedConnectorsByTag.ContainsKey(Tag))
				SpawnedConnectorsByTag[Tag].Add(conData);
			else
				SpawnedConnectorsByTag.Add(Tag, new List<ConnectorsStatsData>() { conData });
			SpawnedConnectors.Add(connector);
		}
		public void AddCmdNodeToStats(CmdNode node)
		{
			if (SpawnedNodesByType.ContainsKey(node.GetCMDType))
			{
				SpawnedNodesByType[node.GetCMDType].Add(node);
			}
			else
			{
				SpawnedNodesByType.Add(node.GetCMDType, new List<CmdNode>());
				SpawnedNodesByType[node.GetCMDType].Add(node);
			}
			SpawnedNodes.Add(node);
		}

		/*
		 * Getters
		 */
		public List<ConnectorsStatsData> GetAllConnectorsWithTag(ConTag tag)
		{
			if(SpawnedConnectorsByTag.ContainsKey(tag))
				return SpawnedConnectorsByTag[tag];
			return null;
		}
		public List<CmdNode> GetAllSpawnedNodesByCMD(CMD type)
		{
			if (SpawnedNodesByType.ContainsKey(type))
				return SpawnedNodesByType[type];
			return null;
		}
	}

	public class DiagramField
	{

		//Members
		static Logger LOG = new Logger("Field");
		private static Visio.Application Application;
		private static Visio.Page Page;
		public DiagramData Stats { get; } = new DiagramData();

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
			CmdNode node = new CmdNode(cmd.text, cmd.type, loc);
			Stats.AddCmdNodeToStats(node);
			return node;
		}
		public CmdNode CreateCmdNode(string text, CMD type, Vector2D loc)
		{
			CmdNode node = new CmdNode(text, type, loc);
			Stats.AddCmdNodeToStats(node);
			return node;
		}
		public void ConnectCmdShapesBase(List<From_Connection> From, CmdNode To, DiagramData.ConTag conTag = DiagramData.ConTag.None)
		{
			if (From == null || To == null) return;
			for (int i = 0; i < From.Count; ++i)
				ConnectCmdShapesBase(From[i], To, conTag);
		}
		public void ConnectCmdShapesBase(From_Connection From, CmdNode To, DiagramData.ConTag conTag = DiagramData.ConTag.None)
		{
			if (From.FromNode == null || To == null) return;
			if (From.FromNode.GetCMDType == CMD.RETURN) return;

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
				return;
			}
			CreatedConnector.Text = From.Text;
			Stats.AddConnectorToStats(CreatedConnector, From, To, conTag);
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
