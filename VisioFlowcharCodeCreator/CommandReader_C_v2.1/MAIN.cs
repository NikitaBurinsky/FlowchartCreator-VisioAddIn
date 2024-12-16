using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandReader_C_v2._1
{
	internal class MAIN
	{
		public static void Main(string[] args)
		{
			CommandsReader c = new CommandsReader();
			List<Command> lc = c.GetCommandsFromFile(null);
			Console.WriteLine("\n\n---------------------\n\n");

			for (int i = 0; i < lc.Count; ++i)
			{
				Console.WriteLine($"~{lc[i].type.ToString()}~ : " + lc[i].text);
			}



		}

	}
}
