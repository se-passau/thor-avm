using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.Variable;

namespace InteracGenerator.Problem
{
    public class IntergenSolution : Solution
    {



        public int FoundAtEval { get; set; }

        public IntergenSolution(IntergenProblem p) : base(p) {
            this.problem = problem;
            this.Type = problem.SolutionType;
            numberOfObjectives = problem.NumberOfObjectives;
            this.Objective = new double[numberOfObjectives];

            this.Fitness = 0.0;
            this.KDistance = 0.0;
            this.CrowdingDistance = 0.0;
            this.DistanceToSolutionSet = double.PositiveInfinity;
            if (p.Model.Setting.UseInitialFv)
            {
                var fdist = p.Model.DStore.SelectedFeatureDistribution;

                for (int i = 0; i < fdist.Values.Length; i++)
                {
                    Variable[i] = new Real(problem.LowerLimit[0], problem.UpperLimit[0], fdist.Values[i]);
                }
                if (p.Model.Setting.NumberOfInteractions <= 0) return;
                var idist = p.Model.DStore.SelectedInteractionDistribution;
                for (int i = 0; i < idist.Values.Length; i++)
                {
                    Variable[i + fdist.Values.Length] = new Real(problem.LowerLimit[fdist.Values.Length + 1], problem.UpperLimit[fdist.Values.Length + 1], idist.Values[i]);
                }
                //this.Variable = Type.CreateInitialVariable();
               
            }
            else
            {
                this.Variable = Type.CreateVariables();
            }
        }

        public IntergenSolution(IntergenProblem problem, bool initialValues) : base(problem)
        {
           
        }



        public IntergenSolution(Solution solution) : base(solution) { }

    }

}
