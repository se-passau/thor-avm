using System;
using Accord.Math;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Problem.Types
{
    internal class FeatVariantProblem : IntergenProblem
    {

        public FeatVariantProblem(InterGen model) : base(model)
        {
            ProblemName = "FeatAndVariantProblem";
            NumberOfObjectives = 2;
        }

        public override void Evaluate(Solution solution)
        {
            var s = (IntergenSolution)solution;

            var doubleVal = new double[Model.Setting.NumberOfFeatures];
           
            var values = new XReal(solution);
            for (var i = 0; i < Model.Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }


            //the distribution from the NSGA2
            var nsgaFv = new Distribution(doubleVal);
            var variantResults = FeatureMatrix.Dot(doubleVal);
           
          
            var variantResult = new Distribution(variantResults);
           
            var localScaledVariants = new double[VariantTarget.Values.Length];
            Array.Copy(VariantTarget.Values, localScaledVariants, VariantTarget.Values.Length);
            var localVariantTarget = new Distribution(localScaledVariants);
            localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantResults.Min(),
                        variantResults.Max());


            var fc = new FitnessCalculator(Model, s.FoundAtEval);
            var fitnessValues = fc.Calculate(variantResult, localVariantTarget, nsgaFv, FeatureTarget, null, null);
            solution.Objective[0] = fitnessValues.FeatureVal;
            solution.Objective[1] = fitnessValues.VariantVal;

            FitnessTracker.AddFeatVar(s.FoundAtEval, fitnessValues.FeatureVal, fitnessValues.VariantVal);
        }
    }
}
