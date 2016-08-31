using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intergen
{
    class GeneratorProblem : Problem
    {

        public GeneratorProblem(string solutionType, int numberOfVariables) {
            SolutionType = new RealSolutionType(this);
        }

        public GeneratorProblem(string solutionType)
			: this(solutionType, 30)
		{

        }

        public GeneratorProblem(SolutionType type) {
            SolutionType = new RealSolutionType(this);
        }

        public override void Evaluate(Solution solution)
        {
            Console.WriteLine("Calling Evaluate");
            //throw new NotImplementedException();
        }
    }
}
