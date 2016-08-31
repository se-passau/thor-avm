using System.Collections.Generic;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Utils;

namespace InteracGenerator.Problem.IBEA
{
    class IBEA : Algorithm
    {
        private JMetalCSharp.Core.Problem problem;


        public IBEA(IntergenProblem problem) : base(problem)
        {
            this.problem = problem;
        }

        public override SolutionSet Execute()
        {
            int evaluations;
            int populationSize = -1;
            int archiveSize = 100;

            SolutionSet solutionSet, offSpringSolutionSet;

            Utils.GetIntValueFromParameter(this.InputParameters, "populationSize", ref populationSize);

            solutionSet = new SolutionSet();
            var archive = new List<Solution>(archiveSize);

            evaluations = 0;

            Solution newSolution;

            for (var i = 0; i < populationSize; i++)
            {
                //newSolution;
            }

            return solutionSet;

        }
    }
}
