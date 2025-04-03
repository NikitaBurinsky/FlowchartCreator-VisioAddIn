# FlowchartCreator-VisioAddIn
The plugin is designed to create flowcharts based on code in C\C++ languages.

The generator scans the entered code, converting each line into one of the following tokens:

    IF
    ELSE, ELSEIF, SWITCH, CASE,
    DEFAULT_SWITCH, DO_LOOP (implement start of DoWhile loop), LOOP (start of base loops, as for, while),     
    SUBPROCESS (functions witchout special tag), PROCESS (base operations as assigning, comparison and so on),
    INPUT (as scanf, getchar), OUTPUT (as printf, cout), StartFunc, RETURN, BREAK, IGNORE (ignoring commands,  
    that are not supposed to be in flowchart)

 Commands perceived by the program are stored and entered into Commands.json , which can be opened via a button in the form of a plugin. In a standard situation, if it is impossible to determine the type of command, it will be assigned a SUBPROCESS or PROCESS.

To add a specific type of command, you need to add it to the corresponding line in Commands.xlsx and fill in the cells Type, StartsWith, Cmd Name and (optionally) Contains and NotContains (Characters and parts of the string contained in the command entry)


![image](https://github.com/user-attachments/assets/692d85c8-7b39-4ead-9db0-0e562fa87387)

![image](https://github.com/user-attachments/assets/82ae3fe9-6fb4-49e1-b120-567fb97f5dc8)


