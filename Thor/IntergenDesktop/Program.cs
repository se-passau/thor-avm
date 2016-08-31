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

            InterGen g = new InterGen();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mainframe(g));
        }
    }
}
