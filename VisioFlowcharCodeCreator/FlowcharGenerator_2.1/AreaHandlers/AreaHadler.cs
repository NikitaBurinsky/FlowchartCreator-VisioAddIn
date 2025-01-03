using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowchartGenerator.AreaHandlers
{
	struct AreaSize
	{
		public float Right;
		public float Left;
		public float Top;
		public float Bottom;
	}
	//public API
	/*
	HandleArea - Обрабатывает зону
	AddZoneLocationOffset - Сдвигает все фигуры в зоне на дельту координат
	SetZoneLocationByRootLocation - Сдвигает зону так, что корневая нода оказывается в координатах
	GetWidth - Возвращает ширину зоны (В координатных индексах)
	GetHeigth - Возвращает высоту зоны (В координатных индексах)
	  
	//protected API
	FindEOZ - Ищет конец зоны
	RECalculateAreaSizeForce - Пересчитывает размер зоны
	UseSubAreaHandler - Запускает HandleArea сабзоны и добвляет ее в InternalAreas
	IsOpenZone - Проверяет, является ли команда - открывающей зону (If, switch и т.д.)

	//static public
	GetAreaHandler - Возвращает обьект хендлера зоны для типа зоны, переданногов в аргумента. В необработтанном случае вернет BaseAreaHandler
	 */

	/*
	 * Процесс создания саб-зоны:
	 *  1 - Создать обьект Хендлера
	    2 - Вызвать handler.HandleArea();
	    3 - Расположить созданную зону в нужном месте
		4 - Забрать из зоны OutputNodes	
	 *  5 - Законнектить верх и низ зоны
	 */
	abstract internal class AreaHandler : SubSystem
	{
		//Тип зоны
		protected CMD AreaType { get; set; }
		//Первая созданная, корневая нода в зоне
		public CmdNode AreaRoot;

		//Все ноды зоны, за исключением нод внутренних зон
		protected List<CmdNode> AreaNodes = new List<CmdNode>();

		//Крайние точки зоны
		public AreaSize Size = new AreaSize { Right = int.MinValue, Left = int.MaxValue, Bottom = int.MaxValue, Top = int.MinValue };

		//Ноды, из которых далее пойдет выполнение, к которым подключается нода после зоны
		public List<From_Connection> OutputNodes = new List<From_Connection>();

		//Внутренние зоны
		protected List<AreaHandler> InternalAreas = new List<AreaHandler>();

		//Список команд
		protected List<Command> Commands; 
		public AreaHandler(List<Command> cmds)
		{
			Commands = cmds;
		}
		
		//МЕТОДЫ
		abstract public bool HandleArea(int ZoneRootIndex, out int EOZ);
		virtual public void AddZoneLocationOffset(Vector2D Offset)
		{
		foreach(CmdNode node in AreaNodes)
			{
				node.AddMove(Offset.X, Offset.Y);
			}
		foreach(AreaHandler area in InternalAreas)
			{
				area.AddZoneLocationOffset(Offset);
			}
		}
		virtual public void SetZoneLocationByRootLocation(Vector2D RootNewLocation)
		{
		Vector2D DeltaLocation;
		DeltaLocation.X = RootNewLocation.X - AreaRoot.GetLocation().X;
		DeltaLocation.Y = RootNewLocation.Y - AreaRoot.GetLocation().Y;
		AddZoneLocationOffset(DeltaLocation);
		}

		//Protected methods
		protected bool IsOpenZone(Command cmd) {return cmd.type <= CMD.LOOP;}
		
		//AREA AND NODES CREATORS
        protected void CreateInternalArea(AreaHandler subArea, int SubRootIndex, out int EOZ)
        {
            subArea.HandleArea(SubRootIndex, out EOZ);
            InternalAreas.Add(subArea);
        }
        protected CmdNode CreateCmdNode(Command command, Vector2D loc, bool IsOutputNode = false)
		{
			CmdNode node = Diagram.CreateCmdNode(command, loc);
			AreaNodes.Add(node);
			return node;
		}
        protected CmdNode CreateCmdNode(String text, CMD type, Vector2D loc, bool IsOutputNode = false)
        {
            CmdNode node = Diagram.CreateCmdNode(text, type, loc);
            AreaNodes.Add(node);
            return node;
        }

        protected virtual int FindEOZ(int ZoneRootIndex)
		{
			int CInd = ZoneRootIndex + 2;
			for (int OpenedGates = 1; OpenedGates > 0; ++CInd)
			{
				if (Commands[CInd].type == CMD.SOZ)
					++OpenedGates;
				else if (Commands[CInd].type == CMD.EOZ)
					--OpenedGates;
			}
			return CInd - 1;
		}
		protected void RECalculateAreaSizeForce()
		{
			if (AreaRoot == null)
			{
				Size.Left = int.MaxValue;
				Size.Right = int.MinValue;
				Size.Top = int.MinValue;
				Size.Bottom = int.MaxValue;
			}
			else
			{
				Size.Left = AreaRoot.GetLocation().X;
				Size.Right = AreaRoot.GetLocation().X;
				Size.Top = AreaRoot.GetLocation().Y;
				Size.Bottom = AreaRoot.GetLocation().Y;
			}
				foreach (CmdNode node in AreaNodes)
			{
				Size.Left = System.Math.Min(Size.Left, node.GetLocation().X);
				Size.Right = System.Math.Max(Size.Right, node.GetLocation().X);
				Size.Top = System.Math.Max(Size.Top, node.GetLocation().Y);
				Size.Bottom = System.Math.Min(Size.Bottom, node.GetLocation().Y);
			}
			foreach (AreaHandler area in InternalAreas)
			{
				area.RECalculateAreaSizeForce();
				AreaSize curSize = area.Size;
				Size.Left = System.Math.Min(Size.Left, curSize.Left);
				Size.Right = System.Math.Max(Size.Right, curSize.Right);
				Size.Top = System.Math.Max(Size.Top, curSize.Top);
				Size.Bottom = System.Math.Min(Size.Bottom, curSize.Bottom);
			}
		}

		//STATIC
		public static AreaHandler GetAreaHandler(int AreaRootIndex, List<Command> Commands)
		{
			CMD type = Commands[AreaRootIndex].type;
			switch(type)
			{
				case CMD.SWITCH: return new SwitchCase_Handler(Commands);
				case CMD.IF: return new IfElse_Handler(Commands);
				case CMD.LOOP: return new BaseLoop_Handler(Commands);
				case CMD.DO_LOOP: return new BaseLoop_Handler(Commands);

				default: return new BaseArea_Handler(Commands);
			}
		}


		//Размер зоны, геттеры 
		public float GetWidth() 
		{
			if (Size.Left == int.MaxValue || Size.Right == int.MinValue || Size.Top == int.MinValue || Size.Bottom == int.MaxValue)
				RECalculateAreaSizeForce();
			return Size.Right - Size.Left + 1;
		}
        public float GetHeight()
        {
			if (Size.Left == int.MaxValue || Size.Right == int.MinValue || Size.Top == int.MinValue || Size.Bottom == int.MaxValue)
				RECalculateAreaSizeForce();
			return Size.Top - Size.Bottom + 1;
        }


			
	}

}
