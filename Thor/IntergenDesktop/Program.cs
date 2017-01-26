using InteracGenerator;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using IntergenDesktop.Forms;


namespace IntergenDesktop
{
    static class Program
    {
        /// <summary>
        /// This is where the magic happens.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Thor g = new Thor();
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Mainframe(g));
            }
            catch (TypeInitializationException)
            {
                Application.Run(new ErrorFrame("You need to Install R first!"));
                Console.WriteLine(@"Please Install R");
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Error in library("))
                {
                    Application.Run(new ErrorFrame("Install R Package: " + e.Message.Substring(e.Message.IndexOf('\''))));
                    return;
                }
                Application.Run(new ErrorFrame("Unknown Error: " + e.Message));
            }
        }
    }
}
