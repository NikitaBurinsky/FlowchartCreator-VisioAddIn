using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;

namespace FlowchartGenerator
{
	[ComVisible(true)]
	public class FlowchartRibbon : Office.IRibbonExtensibility
	{
		private Office.IRibbonUI ribbon;
		private ThisAddIn addIn;

		public FlowchartRibbon(ThisAddIn addInInstance)
		{
			this.addIn = addInInstance;
		}

		public string GetCustomUI(string ribbonID)
		{
			return GetResourceText("FlowcharGenerator_2._1.FlowchartRibbon.xml");
		}

		public void OnRibbonLoad(Office.IRibbonUI ribbonUI)
		{
			this.ribbon = ribbonUI;
		}

		public void OnGenerateClick(Office.IRibbonControl control)
		{
			try
			{
				addIn.RunGenerator();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error running Flowchart Generator:\n" + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static string GetResourceText(string resourceName)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			string[] names = asm.GetManifestResourceNames();
			foreach (string name in names)
			{
				if (string.Compare(resourceName, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(name)))
					{
						if (resourceReader != null)
						{
							return resourceReader.ReadToEnd();
						}
					}
				}
			}
			return null;
		}
	}
}
