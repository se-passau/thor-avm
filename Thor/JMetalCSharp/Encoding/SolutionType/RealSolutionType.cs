using System;
using System.IO;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace JMetalCSharp.Encoding.SolutionType
{
	/// <summary>
	/// Class representing a solution type composed of real variables
	/// </summary>
	public class RealSolutionType : Core.SolutionType
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="problem">Problem to solve</param>
		public RealSolutionType(Problem problem)
			: base(problem)
		{

		}

        public override Core.Variable[] CreateInitialVariable()
        {
            Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];
            var file = File.ReadAllLines("scaledFeatures.txt");
            var vals = file[3].Split(',');
            var values = new double[vals.Length];
            Console.WriteLine(vals.Length);
            for (var i = 0; i < vals.Length; i++)
            {
                values[i] = Convert.ToDouble(vals[i]);
                if (values[i] == double.NaN) {
                    Console.WriteLine("NAN");
                }
            }
            for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
            {
                variables[i] = new Real(Problem.LowerLimit[i], Problem.UpperLimit[i], values[i]);
            }

            return variables;
        }

        public override Core.Variable[] CreateVariables()
		{
			Core.Variable[] variables = new Core.Variable[Problem.NumberOfVariables];

			for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
			{
				variables[i] = new Real(Problem.LowerLimit[i], Problem.UpperLimit[i]);
			}

			return variables;
		}
	}
}
