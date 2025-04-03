using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDParser
{
	public struct CmdParseOptions
	{
		public CmdParseOptions(int CombinedOneType, Dictionary<string, CMD> knownSubprocesses_) {
			MaxCombinedNodesOneType = CombinedOneType;
			if (knownSubprocesses_.Count < 1)
			{
				throw new Exception("Known subprocesses list is empty. It may be because of reading CommandsFile error : 856");
			}
			knownSubrocesses = knownSubprocesses_;
		}
		public Dictionary<string, CMD> knownSubrocesses;
		public int MaxCombinedNodesOneType { get; set; }
	}
}
