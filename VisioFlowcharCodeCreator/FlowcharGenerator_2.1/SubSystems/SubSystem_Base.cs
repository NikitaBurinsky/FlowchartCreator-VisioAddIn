using Visio = Microsoft.Office.Interop.Visio;


namespace FlowchartGenerator
{
    abstract public class SubSystem
    {
        public static DiagramField Diagram;
        public static FG_Core Core;
		public static SettingsSystem Settings;
        public static Visio.Page Page;
        public static Visio.Application Application;

        public static bool InitClass(DiagramField d, Visio.Page page, Visio.Application application, FG_Core C)
        {
            if (d == null || page == null || application == null)
                return false;
            Core = C;
            Diagram = d;
			Settings = C.FGSettings;
            Application = application;
            Page = page;
            return true;
        }
    }

}
