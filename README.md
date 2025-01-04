# FlowchartCreator-VisioAddIn
The plugin is designed to create flowcharts based on code in C\C++ languages.

The generator scans the entered code, converting each line into one of the following tokens:

    IF
    ELSE, ELSEIF, SWITCH, CASE,
    DEFAULT_SWITCH, DO_LOOP (implement start of DoWhile loop), LOOP (start of base loops, as for, while),     
    SUBPROCESS (functions witchout special tag), PROCESS (base operations as assigning, comparison and so on),
    INPUT (as scanf, getchar), OUTPUT (as printf, cout), StartFunc, RETURN, BREAK, IGNORE (ignoring commands,  
    that are not supposed to be in flowchart)

 Commands perceived by the program are stored and entered into Commands.xlsx , which can be opened via a button in the form of a plugin. In a standard situation, if it is impossible to determine the type of command, it will be assigned a SUBPROCESS or PROCESS.

To add a specific type of command, you need to add it to the corresponding line in Commands.xlsx and fill in the cells Type, StartsWith, Cmd Name and (optionally) Contains and NotContains (Characters and parts of the string contained in the command entry)


![image](https://github.com/user-attachments/assets/a1887193-23c9-422a-9c62-af9e33bad9f6)

![image](https://github.com/user-attachments/assets/4cb4f16a-2e6f-4250-a722-9adf41b763fa)
