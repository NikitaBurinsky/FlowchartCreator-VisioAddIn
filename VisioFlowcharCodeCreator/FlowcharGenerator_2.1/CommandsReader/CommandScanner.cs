using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;



public class CMD_CommandScanner
{
	public CMD_CommandScanner(string xpath)
	{
		ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		package = new ExcelPackage(xpath);
		worksheet = package.Workbook.Worksheets[0];

		CheckAndCalculateSheetSize();


	}

	//USER INTERFACE
	int CommandsInSheet = 0;
	int StartWithCollumnsNum = 0;
	int ContainsCollumnsNum = 0;
	int NotContainsCollumnsNum = 0;

	public CMD ScanCommand(string line)
	{
		int RowC = 2;
		if (worksheet.Cells["C1"].GetValue<String>() != "StartsWith")
		{
			Console.WriteLine("\nПИЗДОС В : ScanCommand (Excel)");
			return CMD.NONE;
		}
		for (; RowC < CommandsInSheet + 2; ++RowC)
		{
			if (CheckCommand(RowC, line))
				return GetCellCommandType(RowC);
		}


		if (line.Contains("("))
			return CMD.SUBPROCESS;
		else if (line.StartsWith("}"))
			return CMD.EOZ;
		else
			return CMD.PROCESS;
	}


	private bool CheckCommand(int RowInd, string command)
	{
		bool IsGood = false;
		IsGood = StringStartsWithAnyOfRange(RowInd, 3, command) &&
			StringContainsALLRange(RowInd, 3 + StartWithCollumnsNum, command) &&
			StringNOTContainsALLRange(RowInd, 3 + StartWithCollumnsNum + ContainsCollumnsNum, command);
		return IsGood;
	}

	private bool StringContainsALLRange(int R, int StartC, String line)
	{
		int tempC = StartC + StartWithCollumnsNum;
		for (; StartC < tempC; ++StartC)
		{
			string val = GetCellValue(R, StartC);
			if (val == null)
				continue;
			if (!line.Contains(val))
				return false;
		}
		return true;
	}
	private bool StringNOTContainsALLRange(int R, int C, String line)
	{
		int tempC = C + NotContainsCollumnsNum;

		for (; C < tempC; ++C)
		{
			string val = GetCellValue(R, C);
			if (val == null)
				continue;
			if (line.Contains(val))
				return false;
		}
		return true;
	}
	private bool StringStartsWithAnyOfRange(int R, int C, String line)
	{
		int tempC = C + ContainsCollumnsNum;
		for (; C < tempC; ++C)
		{
			string val = GetCellValue(R, C);
			if (val == null || val == "")
				return true;
			if (line.StartsWith(val))
				return true;
		}
		return false;
	}

	static Dictionary<string, CMD> StringCommandsTypes = new Dictionary<string, CMD>()
	{
		{ "IN", CMD.INPUT },
		{ "ELSEIF" , CMD.ELSEIF },
		{ "OUT", CMD.OUTPUT },
		{ "IF", CMD.IF },
		{ "ELSE", CMD.ELSE },
		{ "SWITCH", CMD.SWITCH },
		{ "CASE", CMD.CASE },
		{ "DEF_SWITCH", CMD.DEFAULT_SWITCH },
		{ "LOOP", CMD.LOOP },
		{ "SOZ", CMD.SOZ },
		{ "EOZ", CMD.EOZ },
		{ "BREAK", CMD.BREAK },
		{ "RETURN", CMD.RETURN },
		{ "IGNORE", CMD.IGNORE }

	};

	private CMD GetCellCommandType(int R)
	{
		string cmdl = worksheet.Cells[R, 1].GetValue<String>();
		if (cmdl == null || cmdl == "")
			return CMD.NONE;

		if (StringCommandsTypes.ContainsKey(cmdl))
			return StringCommandsTypes[cmdl];
		else
			return CMD.NONE;
	}

	private bool CheckAndCalculateSheetSize()
	{
		int R = 1;
		int C = 3;
		if (GetCellValue(1, 1) != "Type")
			return false;
		if (GetCellValue(1, 2) != "CMD Name")
			return false;
		while (GetCellValue(R, C) == "StartsWith")
		{
			++StartWithCollumnsNum;
			++C;
		}
		while (GetCellValue(R, C) == "Contains")
		{
			++ContainsCollumnsNum;
			++C;
		}
		while (GetCellValue(R, C) == "NotContains")
		{
			++NotContainsCollumnsNum;
			++C;
		}
		++R;
		while (GetCellValue(R, 1) != null && GetCellValue(R, 2) != "")
		{
			++CommandsInSheet;
			++R;
		}
		return true;

	}

	private string GetCellValue(int R, int C)
	{
		return worksheet.Cells[R, C].GetCellValue<String>();
	}

	//BACKDOORS
	private ExcelPackage package;
	private ExcelWorksheet worksheet;

}
