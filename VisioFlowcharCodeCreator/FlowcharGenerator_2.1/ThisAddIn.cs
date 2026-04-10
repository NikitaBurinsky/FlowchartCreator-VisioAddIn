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
			Visio.Document ActiveDocument = this.Application.Documents.Add("");
			Visio.Page ActivePage = this.Application.ActivePage;
			ActiveDocument = this.Application.ActiveDocument;
#if !DEBUG
			try
			{
#endif
				if (!File.Exists(textBufferPath))
				{
					FileStream fs = File.Create(textBufferPath);
					fs.Dispose();
				}
				FG_Core FlowchartGenerator = new FG_Core();
				FlowchartGenerator.InitialiseSystems(this.Application, ActivePage, textBufferPath);
				FG.MENU.AddInMenuForm.EMenuResult menuResult = StartMenuForm(FlowchartGenerator, textBufferPath);
				if (menuResult != FG.MENU.AddInMenuForm.EMenuResult.Exit)
				{
					string text = new StreamReader(textBufferPath).ReadToEnd();
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
				ExceptionShape(ex.Message + " : " + ex.Source + " : " + ex.StackTrace + " : " + ex.TargetSite, ActivePage);
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
