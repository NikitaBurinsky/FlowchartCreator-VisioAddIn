using CMDParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace CMDParser
{
	public class CppCommandsParser
	{
		Preprocess.CMDTextPre_Processing TextPre_Processer = new Preprocess.CMDTextPre_Processing();
		Tokenizer.CmdTokenizer CmdTokenizer = new CmdTokenizer();
		TLPostProcesser.CmdTokenListPostProcesser postProcesser = new TLPostProcesser.CmdTokenListPostProcesser();
		public CppCommandsParser(){ }

		public List<Command> ParseAndTokenizeSourceCode(string text, CmdParseOptions parseOptions)
		{
			List<string> lines = TextPre_Processer.PreProcessing(text);
			List<Command> commands = CmdTokenizer.TokenizeLines(lines, parseOptions.knownSubrocesses);
			postProcesser.UseCommandListPostProcess(commands, parseOptions);
			return commands;
		}

	}
}
