﻿namespace FlowchartGenerator
{
	public class SettingsSystem
	{
		public int FigureTextMaxSize { get; set; } = 30; //-1 значит что не обрезаем
		public int MaxCombinedNodesOneType { get; set; } = int.MaxValue;
	}
}
