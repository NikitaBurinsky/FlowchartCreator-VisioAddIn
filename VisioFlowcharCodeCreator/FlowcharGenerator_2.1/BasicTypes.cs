using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visio = Microsoft.Office.Interop.Visio;
using Office = Microsoft.Office.Core;
using OfficeOpenXml.FormulaParsing.Excel.Operators;

namespace FlowchartGenerator
{
    public enum ConType : int { Top, Bottom, Right, Left }

    public struct Vector2D
	{
        public float X;
        public float Y;

        public Vector2D(float x, float y)
		{
			X = x;
			Y = y;
		}
		public static Vector2D operator+(Vector2D v1, Vector2D v2)
		{
			v1.X = v1.X + v2.X;	
			v1.Y = v1.Y + v2.Y;
			return v1;
		}
        public static Vector2D operator-(Vector2D v1, Vector2D v2)
        {
            v1.X = v1.X - v2.X;
            v1.Y = v1.Y - v2.Y;
            return v1;
        }

        public void Plus(float x, float y) 
        {
            X += x;
            Y += y;
        }
	}


	public struct From_Connection
    {
        public CmdNode FromNode;
        public ConType FromConnectionType;
		public string Text;
        public From_Connection(CmdNode fromNode, ConType conType, string text = null)
        {
			Text = text;
            FromConnectionType = conType;
            FromNode = fromNode;
        }
    }
}
