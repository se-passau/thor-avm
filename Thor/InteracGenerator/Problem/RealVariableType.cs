using JMetalCSharp.Core;

namespace InteracGenerator.Problem
{
    class RealVariableType : SolutionType
    {

        public RealVariableType(JMetalCSharp.Core.Problem problem) : base(problem)
        {
        }


        public override Variable[] CreateInitialVariable()
        {
            
            Variable[] variables = new Variable[Problem.NumberOfVariables];

            for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
            {
                variables[i] = new RealVariable(Problem.LowerLimit[i], Problem.UpperLimit[i], 1);
            }

            return variables;
        }


        public override Variable[] CreateVariables()
        {
            Variable[] variables = new Variable[Problem.NumberOfVariables];

            for (int i = 0, li = Problem.NumberOfVariables; i < li; i++)
            {
                variables[i] = new RealVariable(Problem.LowerLimit[i], Problem.UpperLimit[i]);
            }

            return variables;
        }
    }
}
