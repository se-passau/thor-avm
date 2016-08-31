using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BettyConqueror;
using Accord.Statistics;
//using SPLConqueror_Core;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System.Drawing;
using Accord.Statistics.Visualizations;
using Accord.Controls;
using RDotNet;
using JMetalCSharp.Metaheuristics.NSGAII;
using JMetalCSharp.Core;
using JMetalCSharp.Problems.Fonseca;
using JMetalCSharp.Problems.ZDT;
using JMetalCSharp.QualityIndicator;
using JMetalCSharp.Operators.Crossover;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using JMetalCSharp.Utils;

namespace Intergen
{
    static class Program
    {

        static REngine engine;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new LoadConfig());

           // VariabilityModel model = new VariabilityModel("Model1");
          //  InfluenceModel infModel = new InfluenceModel(model, new NFProperty("Test"));

          //  BettyFileParser bfp = new BettyFileParser(model, infModel);

            String[] file = System.IO.File.ReadAllLines(@"C:\Users\Tom\Desktop\Masterarbeit\SPLConqueror\SPLConqueror\BettyConqueror\FeatureModel0.afm");

          //  bfp.readConfiguration(file);
            //ILArray<float> A = ILMath.tosingle()
            //ILArray<float> A = ILMath.tosingle(ILMath.rand(3, 100));


            // RDotNet.NativeLibrary.UnmanagedDll.SetDllDirectory(@"C:\Program Files\R\R-3.2.0\bin\i386\R.dll");


            engine = REngine.GetInstance();

            NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            engine.SetSymbol("group1", group1);

            // e.Evaluate("hist(group1)");
            // e.Evaluate("hist(group1)");

            Problem problem;
            Operator crossover; // Crossover operator
            Operator mutation; // Mutation operator
            Operator selection; // Selection operator

            var logger = Logger.Log;

           // var appenders = logger.Logger.Repository.GetAppenders();
           // var fileAppender = appenders[0] as log4net.Appender.FileAppender;
           // fileAppender.File = "NSGAII.log";
            //.ActivateOptions();

            Dictionary<string, object> parameters; // Operator parameters

            QualityIndicator indicators; // Object to get quality indicators
            problem = new GeneratorProblem("Real", 30);
            

            NSGAII algorithm = new NSGAII(problem);
            indicators = null;

            algorithm.SetInputParameter("populationSize", 200);
            algorithm.SetInputParameter("maxEvaluations", 25000);

            // Mutation and Crossover for Real codification 
            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            parameters.Add("distributionIndex", 20.0);
            crossover = CrossoverFactory.GetCrossoverOperator("SBXCrossover", parameters);

            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 1.0 / problem.NumberOfVariables);
            parameters.Add("distributionIndex", 20.0);
            mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

            // Selection Operator 
            parameters = null;
            selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

            // Add the operators to the algorithm
            algorithm.AddOperator("crossover", crossover);
            algorithm.AddOperator("mutation", mutation);
            algorithm.AddOperator("selection", selection);

            // Add the indicator object to the algorithm
            algorithm.SetInputParameter("indicators", indicators);

            // Execute the Algorithm
            long initTime = Environment.TickCount;
            //SolutionSet population = algorithm.Execute();
            long estimatedTime = Environment.TickCount - initTime;
            logger.Info("Variables values have been writen to file VAR");
//population.PrintVariablesToFile("VAR");
            logger.Info("Objectives values have been writen to file FUN");
         //   population.PrintObjectivesToFile("FUN");
            Console.WriteLine("Time: " + estimatedTime);
            Console.ReadLine();
            //Application.Run(new Form1(e, group1, new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99, 1.3, 1.7, 9.4, 11.1  }));
        }
    }
}
