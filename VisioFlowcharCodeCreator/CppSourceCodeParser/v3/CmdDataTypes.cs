using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum CMD : int
{
	IF = 0,
	ELSE, ELSEIF, SWITCH, CASE,
	DEFAULT_SWITCH, DO_LOOP, LOOP, END_LOOP,
	SOZ, EOZ, SUBPROCESS, PROCESS,
	INPUT, OUTPUT, StartFunc, RETURN, BREAK, NONE, IGNORE
};

public class Command
{
	public Command(String txt, CMD tp)
	{
		text = txt;
		type = tp;
	}
	public String text;
	public CMD type;
}
