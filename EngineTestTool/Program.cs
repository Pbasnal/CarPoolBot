using Bot.Worker;
using System;
using System.Windows.Forms;

namespace EngineTestTool
{
    static class Program
    {
        public static Gmap Map;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadHandlers();
            HandlerInitializer.CreateAllHandlers();
            Map = new Gmap();
            try
            {
                Application.Run(Map);
            }
            catch
            { 
            }
        }

        static void LoadHandlers()
        { 
            //new RequestOwnerToAcceptPoolersHandler();
        }
    }
}
