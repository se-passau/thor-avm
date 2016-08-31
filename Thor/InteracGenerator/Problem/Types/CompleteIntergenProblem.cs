using System;
using System.Diagnostics;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;
using Accord.Math;

namespace InteracGenerator.Problem.Types
{
    public class CompleteIntergenProblem : IntergenProblem
    {

        public CompleteIntergenProblem(InterGen model) : base(model)
        {
            ProblemName = "CompleteIntergenProblem";
            NumberOfObjectives = 3;
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

            var multiplicationTime = new Stopwatch(); multiplicationTime.Start();

            var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
            var interacVals = InteractionMatrix.Dot(interacVal);
            var variantResults = variantValuesWithoutInteraction.Add(interacVals);

            multiplicationTime.Stop();
            FitnessTracker.AddMultiplicationTime(multiplicationTime.ElapsedMilliseconds);
            var variantResult = new Distribution(variantResults);

           
            var localScaledVariants = new double[VariantTarget.Values.Length];
            Array.Copy(VariantTarget.Values, localScaledVariants, VariantTarget.Values.Length);
            var localVariantTarget = new Distribution(localScaledVariants);
            localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantResults.Min(),
                        variantResults.Max());
            

            var fc = new FitnessCalculator(Model, s.FoundAtEval);
            var fitnessValues = fc.Calculate(variantResult, localVariantTarget, nsgaFv, FeatureTarget, interacDist, InteractionTarget);
          
            solution.Objective[0] = fitnessValues.FeatureVal;
            solution.Objective[1] = fitnessValues.InteracVal;
            solution.Objective[2] = fitnessValues.VariantVal;
            FitnessTracker.AddCalcTime(fc.GetFitnessTime());
            FitnessTracker.Add(s.FoundAtEval, fitnessValues.FeatureVal, fitnessValues.InteracVal, fitnessValues.VariantVal);

        }
    }
}