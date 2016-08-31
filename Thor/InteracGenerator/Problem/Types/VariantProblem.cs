using System;
using Accord.Math;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Problem.Types
{
    public class VariantProblem : IntergenProblem
    {

        public VariantProblem(InterGen model) : base(model)
        {
            ProblemName = "VariantIntergenProblem";
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

           
            var localScaledVariants = new double[VariantTarget.Values.Length];
            Array.Copy(VariantTarget.Values, localScaledVariants, VariantTarget.Values.Length);
            var localVariantTarget = new Distribution(localScaledVariants);
            localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantResults.Min(),
                        variantResults.Max());

            var fc = new FitnessCalculator(Model, s.FoundAtEval);
            var fitnessValues = fc.Calculate(variantResult, localVariantTarget, nsgaFv, FeatureTarget, interacDist, InteractionTarget);

            solution.Objective[0] = fitnessValues.VariantVal;

            FitnessTracker.AddVar(s.FoundAtEval, fitnessValues.VariantVal);
        }
    }
}