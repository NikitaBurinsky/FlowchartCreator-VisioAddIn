using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Visio = Microsoft.Office.Interop.Visio;
using FG = FlowchartGenerator;
namespace FlowchartGenerator
{
	public partial class ThisAddIn
	{
		static bool IsExit = false;
		FG.MENU.AddInMenuForm ux;
		CMDParser.CppCommandsParser CmdParser = new CMDParser.CppCommandsParser();

		static string localappdata = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		string appDataBaseDirectory = $@"{localappdata}\FlowchartCreatorAddIn";
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
				Directory.CreateDirectory(appDataBaseDirectory);
				if (!File.Exists(textBufferPath))
				{
					using (FileStream fs = File.Create(textBufferPath)) { }
				}
				EnsureCommandsJsonExists();
				FG_Core FlowchartGenerator = new FG_Core();
				FlowchartGenerator.InitialiseSystems(this.Application, ActivePage, textBufferPath);
				StartMenuForm(FlowchartGenerator, textBufferPath);
				string text;
				using (StreamReader streamReader = new StreamReader(textBufferPath))
				{
					text = streamReader.ReadToEnd();
				}
				CMDParser.CmdParseOptions parseOptions = new CMDParser.CmdParseOptions(FlowchartGenerator.FGSettings.MaxCombinedNodesOneType,
					CMDParser.ReadKnown.KnownFunctionsDictionaryReader.DeserializeKnownFunctions(KnownFunctionsJsonPath));
				List<Command> commands = CmdParser.ParseAndTokenizeSourceCode(text, parseOptions);
				if (!IsExit)
				{
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

		private void EnsureCommandsJsonExists()
		{
			if (File.Exists(KnownFunctionsJsonPath))
				return;

			const string defaultCommandsJson = "{}";
			using (StreamWriter streamWriter = new StreamWriter(KnownFunctionsJsonPath, false))
			{
				streamWriter.Write(defaultCommandsJson);
			}

			MessageBox.Show(
				$"Файл команд создан:\n{KnownFunctionsJsonPath}\n\n" +
				"Это сценарий первого запуска. Откройте файл и заполните команды в формате JSON, затем повторите генерацию.",
				"Flowchart Creator AddIn",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}
		//Shutdown
		private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
		{
			formClosed = true;
			ux.Close();
			Logger.ShutDownLogs();
		}

		//MENU
		static bool formClosed = false;
		private void StartMenuForm(FG_Core FGenerator, string filepath)
		{
			ux = new FG.MENU.AddInMenuForm(filepath, KnownFunctionsJsonPath, FGenerator.FGSettings);
			ux.Show();
			ux.FormClosed += Form_FormClosed;
			while (!formClosed)
			{
				System.Windows.Forms.Application.DoEvents();
				Thread.Sleep(10);
			}
		}
		private static void Form_FormClosed(object sender, FormClosedEventArgs e)
		{
			formClosed = true;
			FlowchartGenerator.MENU.AddInMenuForm form = (FlowchartGenerator.MENU.AddInMenuForm)sender;
			if (form.MenuResult == MENU.AddInMenuForm.EMenuResult.Exit)
				IsExit = true;
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
