using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;
using FG = FlowchartGenerator;
namespace FlowchartGenerator
{
	public partial class ThisAddIn
	{
		FG.MENU.AddInMenuForm ux;
		CMDParser.CppCommandsParser CmdParser = new CMDParser.CppCommandsParser();

		static string localappdata = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		string textBufferPath = $@"{localappdata}\FlowchartCreatorAddIn" + "\\CommandsLineTextBuffer.txt"; //temp file
		string KnownFunctionsJsonPath = $@"{localappdata}\FlowchartCreatorAddIn" + "\\Commands.json";
		private void ThisAddIn_Startup(object sender, System.EventArgs e)
		{
			try
			{
				if (!File.Exists(textBufferPath))
				{
					string targetDir = Path.GetDirectoryName(textBufferPath);
					if (!Directory.Exists(targetDir))
					{
						Directory.CreateDirectory(targetDir);
					}
					FileStream fs = File.Create(textBufferPath);
					fs.Dispose();
				}
				string jsonDir = Path.GetDirectoryName(KnownFunctionsJsonPath);
				if (!Directory.Exists(jsonDir))
				{
					Directory.CreateDirectory(jsonDir);
				}
				if (!File.Exists(KnownFunctionsJsonPath))
				{
					string sourceJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Commands.json");
					if (File.Exists(sourceJson))
					{
						File.Copy(sourceJson, KnownFunctionsJsonPath);
					}
					else
					{
						string defaultJson = @"{
  ""printf"": ""OUTPUT"",
  ""printf_s"": ""OUTPUT"",
  ""cout"": ""OUTPUT"",
  ""fprintf"": ""OUTPUT"",
  ""fprintf_s"": ""OUTPUT"",
  ""putchar"": ""OUTPUT"",
  ""putch"": ""OUTPUT"",
  ""scanf_s"": ""INPUT"",
  ""scanf"": ""INPUT"",
  ""getchar"": ""INPUT"",
  ""getch"": ""INPUT"",
  ""cin"": ""INPUT"",
  ""fin"": ""INPUT""
}";
						File.WriteAllText(KnownFunctionsJsonPath, defaultJson);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error initializing Flowchart Generator Add-In:\n" + ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
		{
			return new FlowchartRibbon(this);
		}

		public void RunGenerator()
		{
			Visio.Document newDocument = this.Application.Documents.Add("");
			Visio.Page activePage = this.Application.ActivePage;

#if !DEBUG
			try
			{
#endif
				FG_Core FlowchartGenerator = new FG_Core();
				FlowchartGenerator.InitialiseSystems(this.Application, activePage, textBufferPath);
				FG.MENU.AddInMenuForm.EMenuResult menuResult = StartMenuForm(FlowchartGenerator, textBufferPath);
				if (menuResult != FG.MENU.AddInMenuForm.EMenuResult.Exit)
				{
					string text = File.ReadAllText(textBufferPath);
					CMDParser.CmdParseOptions parseOptions = new CMDParser.CmdParseOptions(FlowchartGenerator.FGSettings.MaxCombinedNodesOneType,
						CMDParser.ReadKnown.KnownFunctionsDictionaryReader.DeserializeKnownFunctions(KnownFunctionsJsonPath));
					List<Command> commands = CmdParser.ParseAndTokenizeSourceCode(text, parseOptions);
					FlowchartGenerator.GenerateDiagram(5, 8, commands);
				}
				Logger.ShutDownLogs();
#if !DebugVersion && !DEBUG
			}
			catch (Exception ex)
			{
				ShowErrorDialog(ex, activePage);
			}
#endif
		}
		//Shutdown
		private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
		{
			if (ux != null && !ux.IsDisposed)
			{
				ux.Close();
			}
			Logger.ShutDownLogs();
		}

		//MENU
		private FG.MENU.AddInMenuForm.EMenuResult StartMenuForm(FG_Core FGenerator, string filepath)
		{
			using (ux = new FG.MENU.AddInMenuForm(filepath, KnownFunctionsJsonPath, FGenerator.FGSettings))
			{
				ux.ShowDialog();
				return ux.MenuResult;
			}
		}

		private void ShowErrorDialog(Exception ex, Visio.Page activePage)
		{
			string userFriendlyMessage = "При анализе кода или построении схемы произошла ошибка. Проверьте правильность синтаксиса C-кода и соответствие открывающих и закрывающих скобок.";
			if (ex.Message.Contains("не совпадает") || ex.Message.Contains("SOZ") || ex.Message.Contains("EOZ"))
			{
				userFriendlyMessage = "Ошибка синтаксического анализа: Не совпадает количество открывающих '{' и закрывающих '}' фигурных скобок.";
			}
			else if (ex.Message.Contains("Start of function was not defined"))
			{
				userFriendlyMessage = "Ошибка структуры кода: Начало функции не определено. Убедитесь, что код начинается с сигнатуры функции и открывающей фигурной скобки.";
			}

			string technicalDetails = $"Исключение: {ex.GetType().FullName}\n" +
			                           $"Сообщение: {ex.Message}\n" +
			                           $"Источник: {ex.Source}\n" +
			                           $"Метод: {ex.TargetSite}\n\n" +
			                           $"Стек вызовов:\n{ex.StackTrace}";

			using (var errorForm = new FlowchartGenerator.UX_MENU_Forms.ErrorForm(userFriendlyMessage, technicalDetails))
			{
				errorForm.ShowDialog();
			}
		}

		#region Код, автоматически созданный VSTO

		/// <summary>
		/// Метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(ThisAddIn_Startup);
			this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
		}

		#endregion
	}
}
