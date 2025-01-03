using System;
using System.Collections.Generic;

namespace FlowchartGenerator
{
	public partial class CmdNode : SubSystem// PRIVATE
	{
		static Logger LOG = new Logger("CmdNode class");
		private Shape shape;
		private CMD Type;
		private int ID;
		public List<CmdNode> ConnectedFrom = new List<CmdNode>();
		public List<CmdNode> ConnectedTo = new List<CmdNode>();
		public CmdNode(String text, CMD type, Vector2D Location)
		{
			shape = new FlowchartGenerator.Shape(Location, type, text);
			Type = type;
		}
		public From_Connection CreateFromCon(ConType conType, string text = null) { return new From_Connection(this, conType, text); }
		public Vector2D GetLocation() { return shape.GetLocation(); }
		public void SetLocation(Vector2D newLoc) { shape.SetLocation(newLoc); }
		public string GetText() { return shape.GetShapeText(); }
		public Shape GetShape() { return shape; }

		public void AddMove(float x, float y)
		{
			Vector2D loc = shape.GetLocation();
			loc.X += x;
			loc.Y += y;
			shape.SetLocation(loc);
		}

		public CMD GetCMDType
		{ get { return Type; } }
		//Static functions
	}

}
