using System.Windows.Media.Media3D;
using Visio = Microsoft.Office.Interop.Visio;


namespace FlowchartGenerator
{ 
	public class Shape : SubSystem
	{
		private static Logger LOG = new Logger("Shape class");
		public Shape(Vector2D CreateLocation, CMD Type, string Text = "")
		{
			Visio.Master master = ShapeMaster.GetMasterShapeByCMD(Type);
            if (master == null)
            {
				return;
            }
            shape = Page.Drop(master, 0, 0);
			SetLocation(CreateLocation);

			if (Text != null)
			if (Settings.FigureTextMaxSize > -1)
			{
				if (Text.Length > Settings.FigureTextMaxSize)
				{
					Text = Text.Substring(0, Settings.FigureTextMaxSize) + "...";
				}
			}
			shape.Text = Text /*+ $" : {shape.ID} : {shape.Name}"*/;
		}


		public Vector2D GetLocation() { return Location; }
		public bool SetLocation(Vector2D newLocation)
		{
			Visio.Cell CoordX = shape.Cells["PinX"];
			Visio.Cell CoordY = shape.Cells["PinY"];
			if (CoordX == null || CoordY == null)
			{
				LOG.Write("Null coords sent in function SetLocation", -1);
				return false;
			}
			Location = newLocation;
			newLocation = Diagram.GetVisioCoordinatesByIndLoc(newLocation);
			CoordX.Formula = newLocation.X.ToString() + " mm";
			CoordY.Formula = newLocation.Y.ToString() + " mm";
			return true;
		}
		public string GetShapeText() { return shape.Text; }
		public void SetShapeText(string text) { shape.Text = text; }
		public void SetWidth(float width) { shape.Cells["Width"].Formula = $"{width} mm"; }
		public void SetHeight(float height) { shape.Cells["Height"].Formula = $"{height} mm"; }

		public void SetSize(Vector2D newSize)
		{
            shape.Cells["Height"].Formula = $"{newSize.X} mm";
            shape.Cells["Width"].Formula = $"{newSize.Y} mm";
        }



        public static Visio.Shape ConnectSideToTop(From_Connection From, Shape To, Visio.Master conMaster)
		{
            Visio.Shape Con = null;
            Con = Page.Drop(conMaster, 0, 0);
            string FromName = From.FromNode.GetShape().shape.Name;
            string ToName = To.shape.Name;
			string FromIF_ConStartSide = $" + ('{FromName}'!Width * 0.5)";
			if (From.FromConnectionType == ConType.Left)
				FromIF_ConStartSide = $" - ('{FromName}'!Width * 0.5)";

			Con.Cells["LockHeight"].FormulaForce = "FALSE";
            Con.Cells["LockRotate"].FormulaForce = "FALSE";
            Con.Cells["LockVtxEdit"].FormulaForce = "FALSE";
			Con.Cells["FlipY"].FormulaForce = "TRUE";


            Con.Cells["BeginX"].FormulaForce = $"=('{FromName}'!PinX " + FromIF_ConStartSide + ")";
            Con.Cells["BeginY"].FormulaForce = $"='{FromName}'!PinY";
            Con.Cells["EndX"].FormulaForce = $"='{ToName}'!PinX";
			Con.Cells["EndY"].FormulaForce = $"=('{ToName}'!PinY + ('{ToName}'!Height * 0.5))";
            return Con;
        }
		public static Visio.Shape ConnectBottomToTop(From_Connection From, Shape To, Visio.Master conMaster) 
		{
            Visio.Shape Con = null;

            Con = Page.Drop(conMaster, 0, 0);
            string FromName = From.FromNode.GetShape().shape.Name;
            string ToName = To.shape.Name;

            Con.Cells["LockHeight"].FormulaForce = "FALSE";
            Con.Cells["LockRotate"].FormulaForce = "FALSE";
            Con.Cells["LockVtxEdit"].FormulaForce = "FALSE";


            Con.Cells["BeginX"].FormulaForce = $"='{FromName}'!PinX";
            Con.Cells["BeginY"].FormulaForce = $"=('{FromName}'!PinY - ('{FromName}'!Height / 2))";
            Con.Cells["EndX"].FormulaForce = $"='{ToName}'!PinX";
            Con.Cells["EndY"].FormulaForce = $"=('{ToName}'!PinY + ('{ToName}'!Height * 0.5))";

			return Con;

        }
        public void SetCellParameter(string CellName, string Formula)
		{
			shape.Cells[CellName].FormulaForce = Formula;
		}

		private Visio.Shape shape;
		Vector2D Location;
		Vector2D VisioCoordinates;
	}

}
