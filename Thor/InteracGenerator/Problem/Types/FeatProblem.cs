using System.Linq;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Problem.Types
{
    class FeatProblem : IntergenProblem
    {
       
        private int _counter;
        public FeatProblem(InterGen model) : base(model)
        {
            ProblemName = "FeatProblem";
            NumberOfObjectives = 1;
        }

        public override void Evaluate(Solution solution)
        {
            _counter++;
            IntergenSolution s = (IntergenSolution)solution;
            s.FoundAtEval = _counter;

            var doubleVal = new double[Model.Setting.NumberOfFeatures];
            var values = new XReal(solution);
            for (var i = 0; i < Model.Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }         

            //the distribution from the NSGA2
            var nsgaFv = new Distribution(doubleVal);

            var fc = new FitnessCalculator(Model, _counter);
            var fitnessValues = fc.Calculate(null, null, nsgaFv, FeatureTarget, null, null);
            solution.Objective[0] = fitnessValues.FeatureVal;

            FitnessTracker.AddFeat(s.FoundAtEval, fitnessValues.FeatureVal);
        }
    }
}
