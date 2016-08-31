using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPLConqueror_Core;

namespace MachineLearning.Solver
{
    public interface ICheckConfigSAT
    {
        /// <summary>
        /// Checks whether the boolean selection is valid w.r.t. the variability model. Does not check for numeric options' correctness.
        /// </summary>
        /// <param name="config">The list of binary options that are SELECTED (only selected options must occur in the list).</param>
        /// <param name="vm">The variability model that represents the context of the configuration.</param>
        /// <param name="exact">Checks also the number of selected options such that it returns only true if exactly the given configuration is valid 
        /// (e.g., if we need to select more features to get a valid config, it returns false if exact is set to true).
        /// <returns>True if it is a valid selection w.r.t. the VM, false otherwise</returns>
        bool checkConfigurationSAT(List<BinaryOption> config, VariabilityModel vm, bool exact);

        /// <summary>
        /// Checks whether the boolean selection of a configuration is valid w.r.t. the variability model. Does not check for numeric options' correctness.
        /// </summary>
        /// <param name="c">The configuration that needs to be checked.</param>
        /// <param name="vm">The variability model that represents the context of the configuration.</param>
        /// <returns>True if it is a valid selection w.r.t. the VM, false otherwise</returns>
        bool checkConfigurationSAT(Configuration c, VariabilityModel vm);

        bool checkDimacsSAT(List<string> tempConfig, string[] lines);

        //Not important
        //List<ConfigurationOption> determineSetOfInvalidFeatures(int nbOfFeatures, VariabilityModel vm, bool withDerivatives, List<ConfigurationOption> forbiddenFeatures, RuntimeProperty rp, NFPConstraint constraint);
    }
}
