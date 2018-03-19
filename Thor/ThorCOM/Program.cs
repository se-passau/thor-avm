using InteracGenerator;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ThorCOM.Parser;

namespace ThorCOM
{
    class Program
    {
        //C:\ThorProjekt\Test.txt

        private Thor _model;
        static void Main(string[] args)
        {
            try
            {
                string path;
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Console.WriteLine(" _____  _   _  _____ ______");
                Console.WriteLine("|_   _|| | | ||  _  || ___ \\ ");
                Console.WriteLine("  | |  | |_| || | | || |_/ /");
                Console.WriteLine("  | |  |  _  || | | ||    / ");
                Console.WriteLine("  | |  | | | |\\ \\_/ /| |\\ \\ ");
                Console.WriteLine("  \\_/  \\_| |_/ \\___/ \\_| \\_|");
                Thor model = new Thor();
                Commander commander = new Commander(model);
                if (args.Length == 0)
                {
                    Console.WriteLine("Please Enter a valid path: ");
                    path = Console.ReadLine();
                    commander.ReadFile(path);
                    commander.StartEvolution();
                }
                else
                {
                    commander.ReadFile(args[0]);
                    commander.StartEvolution();
                }

            }
            catch (TypeInitializationException)
            {
                Console.WriteLine(@"Please Install R");
            }

        }





        /*
         * Start(Loading), 
         * FeatureModel, 
         * Features, 
         * Interaction, 
         * Variant, 
         * Generate, 
         * EvolutionSettings, 
         * Evolution, 
         * Results 
         */
    }
}
