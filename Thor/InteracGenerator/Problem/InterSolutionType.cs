using JMetalCSharp.Core;

namespace InteracGenerator.Problem
{
    abstract class InterSolutionType
    {
        public JMetalCSharp.Core.Problem Problem { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">The problem to solve</param>
        public InterSolutionType(JMetalCSharp.Core.Problem problem)
        {
            Problem = problem;
        }

        /// <summary>
        /// Abstract method to create the variables of the solution
        /// </summary>
        /// <returns></returns>
        public abstract Variable[] CreateVariables();


        public abstract Variable[] CreateInitialVariable();
        /// <summary>
        /// Copies the decision variables
        /// </summary>
        /// <param name="vars"></param>
        /// <returns>An array of variables</returns>
        public virtual Variable[] CopyVariables(Variable[] vars)
        {
            Variable[] variables;

            variables = new Variable[vars.Length];

            for (int var = 0; var < vars.Length; var++)
            {
                variables[var] = vars[var].DeepCopy();
            }

            return variables;
        }
    }
}
