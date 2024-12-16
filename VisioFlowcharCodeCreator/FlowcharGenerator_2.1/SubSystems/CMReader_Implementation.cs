using System.Collections.Generic;

namespace FlowchartGenerator
{
    internal class CommandsReaderImplementation : SubSystem
    {
        CommandsReader CMDReader;
        static Logger LOG = new Logger("CommandsReader_I");
        public CommandsReaderImplementation(string commandsXlsxPath)
        {
            CMDReader = new CommandsReader(Settings, commandsXlsxPath);
        }
        public List<Command> GetCommandsFromFile(string fileName)
        {
            List<Command> commands = CMDReader.GetCommandsFromFile(fileName);
            for (int i = 0; i < commands.Count; ++i)
            {
                LOG.Write($"~{commands[i].type.ToString()}~ {commands[i].text}");
            }
            LOG.Write("Scan Commands From Text : Success");
            return commands;
        }
    }
}
