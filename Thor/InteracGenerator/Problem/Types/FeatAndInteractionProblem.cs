using System;
using System.Linq;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Problem
{
    class FeatAndInteractionProblem : IntergenProblem
    {
        /*public Distribution FeatureTarget { get; set; }
        public Distribution InteractionTarget { get; set; }
        public double[,] FeatureMatrix { get; set; }
        public double[,] InteractionMatrix { get; set; }
        private int _counter = 0;
        private InterGen Model { get; set; } */

        public FeatAndInteractionProblem(InterGen model) : base(model)
        {
            Model = model;
            ProblemName = "FeatAndInteractionProblem";
            
            NumberOfObjectives = 2;
        }

        public override void Evaluate(Solution solution)
        {

            
            var s = (IntergenSolution)solution;
            
            var doubleVal = new double[Model.Setting.NumberOfFeatures];
            var interacVal = new double[Model.Setting.NumberOfInteractions];
            var values = new XReal(solution);
            for (var i = 0; i < Model.Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }

            for (var i = 0; i < Model.Setting.NumberOfInteractions; i++)
            {
                interacVal[i] = values.GetValue(i + Model.Setting.NumberOfFeatures);
            }

            //the distribution from the NSGA2
            var nsgaFv = new Distribution(doubleVal);
            var interacDist = new Distribution(interacVal);

            var fc = new FitnessCalculator(Model, s.FoundAtEval);
            var fitnessValues = fc.Calculate(null, null, nsgaFv, FeatureTarget, interacDist, InteractionTarget);
            var time = fc.GetFitnessTime();
            solution.Objective[0] = fitnessValues.FeatureVal;
            solution.Objective[1] = fitnessValues.InteracVal;

            FitnessTracker.AddFeatInterac(s.FoundAtEval, fitnessValues.FeatureVal, fitnessValues.InteracVal);

            /*var cont = new SolutionContainer
            {
      
                Features = nsgaFv,
                Interaction = interacDist,
                //TargetFeatures = FeatureTarget,
                FeatureTVal = fitnessValues.FeatureVal,
                InteracTVal = fitnessValues.InteracVal,
                VariantTVal = fitnessValues.VariantVal,
                FoundAtEval = _counter,
            }; 
            Model.AddSolutionToHistory(cont); */
            //_worker.ReportProgress((int) (_counter*100/(double) Model.Setting.MaxEvaluations));
        }
    }
}
