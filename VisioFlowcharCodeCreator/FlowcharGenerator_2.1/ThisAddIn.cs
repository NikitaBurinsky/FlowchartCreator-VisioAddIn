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
				ExceptionShape(ex.Message + " : " + ex.Source + " : " + ex.StackTrace + " : " + ex.TargetSite, activePage);
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

		private void ExceptionShape(string message, Visio.Page ActivePage)
		{
			Visio.Document BasicShapes_Stencil = Application.Documents.OpenEx("Basic Shapes.vss", (short)Microsoft.Office.Interop.Visio.VisOpenSaveArgs.visOpenDocked);
			Visio.Shape exepc = ActivePage.Drop(BasicShapes_Stencil.Masters.get_ItemU("Snip Same Side Corner Rectangle"), 0, 0);
			const string ErrMessage = "AddIn disabled.\nTo start AddIn again turn on it in \"Options/AddIn/COM/Go\"";
			MessageBox.Show("Generating fatal error\n" + ErrMessage, null,
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			exepc.Text = ErrMessage + "\n\n" + message;
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
