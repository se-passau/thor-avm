using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MachineLearning.Solver;
using SPLConqueror_Core;
using Microsoft.SolverFoundation.Services;
using System.ComponentModel.Composition;
using Microsoft.SolverFoundation.Solvers;


namespace MicrosoftSolverFoundation
{
    [Export(typeof(MachineLearning.Solver.ICheckConfigSAT))]
    [ExportMetadata("SolverType", "MSSolverFoundation")]
    public class CheckConfigurationSAT : ICheckConfigSAT
	{
        /// <summary>
        /// Checks whether the boolean selection is valid w.r.t. the variability model. Does not check for numeric options' correctness.
        /// </summary>
        /// <param name="config">The list of binary options that are SELECTED (only selected options must occur in the list).</param>
        /// <param name="vm">The variability model that represents the context of the configuration.</param>
        /// <param name="exact">Checks also the number of selected options such that it returns only true if exactly the given configuration is valid 
        /// (e.g., if we need to select more features to get a valid config, it returns false if exact is set to true).
        /// <returns>True if it is a valid selection w.r.t. the VM, false otherwise</returns>
        public bool checkConfigurationSAT(List<BinaryOption> config, VariabilityModel vm, bool exact)
		{
            
            List<CspTerm> variables = new List<CspTerm>();
            Dictionary<BinaryOption, CspTerm> elemToTerm = new Dictionary<BinaryOption, CspTerm>();
            Dictionary<CspTerm, BinaryOption> termToElem = new Dictionary<CspTerm, BinaryOption>();
            ConstraintSystem S = CSPsolver.getConstraintSystem(out variables, out elemToTerm, out termToElem, vm);
            
            //Feature Selection
            foreach (BinaryOption binayOpt in elemToTerm.Keys)
            {
                CspTerm term = elemToTerm[binayOpt];
                if (config.Contains(binayOpt))
                {
                    S.AddConstraints(S.Implies(S.True, term));
                }
                else
                {

                    if (exact) S.AddConstraints(S.Implies(S.True, S.Not(term)));
                }
            }
        
            ConstraintSolverSolution sol = S.Solve();
            if (sol.HasFoundSolution)
			{
				int count = 0;
                foreach (CspTerm cT in variables)
                {
                    if (sol.GetIntegerValue(cT) == 1)
                        count++;
                }
                //Needs testing TODO
				if (count != config.Count && exact == true)
				{
					return false;
				}
				return true;
			}
			else
				return false;
		}


        public bool checkDimacsSAT(List<string> tempConfig, string[] lines)
        {
            var exact = false;
            List<CspTerm> variables = new List<CspTerm>();
            Dictionary<string, CspTerm> elemToTerm = new Dictionary<string, CspTerm>();
            Dictionary<CspTerm, string> termToElem = new Dictionary<CspTerm, string>();

           
            var sys = CSPsolver.getDimacsCSystem(out variables, out elemToTerm, out termToElem, lines);

            foreach (var k in elemToTerm.Keys)
            {
                if (tempConfig.Contains(k))
                {
                    sys.Implies(sys.True, elemToTerm[k]);
                }
            }
               

            var sol = sys.Solve();
            return (sol.HasFoundSolution);

        }



        /// <summary>
        /// Checks whether the boolean selection of a configuration is valid w.r.t. the variability model. Does not check for numeric options' correctness.
        /// </summary>
        /// <param name="c">The configuration that needs to be checked.</param>
        /// <param name="vm">The variability model that represents the context of the configuration.</param>
        /// <returns>True if it is a valid selection w.r.t. the VM, false otherwise</returns>
        public bool checkConfigurationSAT(Configuration c, VariabilityModel vm)
		{
            List<CspTerm> variables = new List<CspTerm>();
            Dictionary<BinaryOption, CspTerm> elemToTerm = new Dictionary<BinaryOption, CspTerm>();
            Dictionary<CspTerm, BinaryOption> termToElem = new Dictionary<CspTerm, BinaryOption>();
            ConstraintSystem S = CSPsolver.getConstraintSystem(out variables, out elemToTerm, out termToElem, vm);

            //Feature Selection
            foreach (BinaryOption binayOpt in elemToTerm.Keys)
            {
                CspTerm term = elemToTerm[binayOpt];
                if (c.getBinaryOptions(BinaryOption.BinaryValue.Selected).Contains(binayOpt))
                {
                    S.AddConstraints(S.Implies(S.True, term));
                }
                else
                {
                    S.AddConstraints(S.Implies(S.True, S.Not(term)));
                }
            }

            ConstraintSolverSolution sol = S.Solve();
            if (sol.HasFoundSolution)
            {
                int count = 0;
                foreach (CspTerm cT in variables)
                {
                    if (sol.GetIntegerValue(cT) == 1)
                        count++;
                }
                //-1??? Needs testing TODO
                if (count - 1 != c.getBinaryOptions(BinaryOption.BinaryValue.Selected).Count)
                {
                    return false;
                }
                return true;
            }
            else
                return false;
		}
	}
}
