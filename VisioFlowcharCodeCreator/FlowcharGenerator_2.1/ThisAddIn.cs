using System;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;
using System.IO;
using System.Threading;
using FG = FlowchartGenerator;


namespace FlowchartGenerator
{

	public partial class ThisAddIn
	{
		static bool IsExit = false;
		FG.MENU.AddInMenuForm ux;

        static string localappdata = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		string textBufferPath = $@"{localappdata}\FlowchartCreatorAddIn" + "\\CommandsLineTextBuffer.txt"; //temp file
        string cmdXlsxSheetPath = $@"{localappdata}\FlowchartCreatorAddIn" + "\\Commands.xlsx"; //temp file
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
                FlowchartGenerator.InitialiseSystems(this.Application, ActivePage, textBufferPath, cmdXlsxSheetPath);
                StartMenuForm(FlowchartGenerator, textBufferPath);
                if (!IsExit)
                {
                    FlowchartGenerator.GenerateDiagram(5, 8, textBufferPath);
                }
                Logger.ShutDownLogs();
#if !DebugVersion && !DEBUG
		}
            catch(Exception ex) 
            {
                ExceptionShape(ex.Message + " : " + ex.Source + " : " + ex.StackTrace + " : " + ex.TargetSite, ActivePage);
            }
#endif
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
			ux = new FG.MENU.AddInMenuForm(filepath, cmdXlsxSheetPath, FGenerator.FGSettings);
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
            exepc.Text = message;  
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
