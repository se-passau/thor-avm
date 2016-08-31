using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Problem.Types
{
    public class InteractionProblem : IntergenProblem
    {
        public InteractionProblem(InterGen model) : base(model)
        {
            ProblemName = "InteractionProblem";
            NumberOfObjectives = 1;
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

            var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
            var interacVals = InteractionMatrix.Dot(interacVal);
            var variantResults = variantValuesWithoutInteraction.Add(interacVals);
            var variantResult = new Distribution(variantResults);




            var fc = new FitnessCalculator(Model, s.FoundAtEval);
            var fitnessValues = fc.Calculate(null, null, null, null, interacDist, InteractionTarget);

            solution.Objective[0] = fitnessValues.InteracVal;

            FitnessTracker.AddInterac(s.FoundAtEval, fitnessValues.InteracVal);
        }
    }
}
