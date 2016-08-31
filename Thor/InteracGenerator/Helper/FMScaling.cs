using SPLConqueror_Core;
using System;
using System.Collections.Generic;
using MachineLearning.Solver;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InteracGenerator
{
    public class FMScaling
    {

        private VariabilityModel _varModel;
        private InfluenceModel _influenceModel;

        private readonly InterGen _model;

        public double[] FeatureValues;
        public double[] InteractionValues;

        private List<BinaryOption> _tempConfig;
        private List<BinaryOption> _featuresInInteraction;
        private readonly Random _rand;
        private List<string> DimacsFeatureList;
        private List<List<string>> DimacsFound;
        private List<List<BinaryOption>> _interactionsFound;


        public FMScaling()
        {
        }

        public FMScaling(InterGen model)
        {
            _model = model;
            _rand = new Random();
        }

        #region Main

        public List<List<BinaryOption>> GetAddedInteractions() {
            return _interactionsFound;
        }

        public List<List<string>> GetAddedInteraction()
        {
            return DimacsFound;
        }

        /// <summary>
        /// Creates attributed variability model with interactions,  reports progress to GUI BackgroundWorker <param>worker</param>
        /// </summary>
        /// <param name="varModel"></param>
        /// <param name="features"></param>
        /// <param name="interactions"></param>
        /// <param name="worker"></param>
        public void CreateFromVarModel(VariabilityModel varModel, Distribution features, Distribution interactions, BackgroundWorker worker)
        {
            _varModel = varModel;

            var prop = new NFProperty("NFP");
            GlobalState.currentNFP = prop;

            _influenceModel = new InfluenceModel(varModel, prop);
            GlobalState.infModel = _influenceModel;
            GlobalState.varModel = varModel;

            FeatureValues = features.Values;
            interactions = ScaleInteractionValuesToFeature(features, interactions);
            InteractionValues = interactions.Values.OrderBy(x => _rand.Next()).ToArray();
            
            AddAttributesToModel();
            AddInteractions(_model.Setting.InteractionOrderPercent, worker);
           
            _influenceModel.printModelAsFunction("ivmodel.txt");
        }


        public void CreateFromDimacs(List<string> featuresList, string[] lines, Distribution features, Distribution interactions,
            BackgroundWorker worker)
        {
            FeatureValues = features.Values;
            interactions = ScaleInteractionValuesToFeature(features, interactions);
            InteractionValues = interactions.Values.OrderBy(x => _rand.Next()).ToArray();
            var influence = "";
            DimacsFeatureList = featuresList;
            DimacsFound = new List<List<string>>();
            var toAssign = new List<double>(FeatureValues);
            foreach (var opt in featuresList)
            {
                var next = _rand.Next(0, toAssign.Count);
                var value = toAssign[next];
                toAssign.RemoveAt(next);  //do not assign this attribute value again
                //_influenceModel.BinaryOptionsInfluence.Add(opt, new InfluenceFunction(opt.Name + " * " + value));
                influence = string.Concat(influence, $"{opt} * {value}\n");
            }

            //empty the file content
            File.WriteAllText(@"dimacsInfluenceModel.txt", string.Empty);

            influence = influence.Remove(influence.Length - 1, 1);
           
            using (var sw = File.AppendText("dimacsInfluenceModel.txt"))
            {
                sw.WriteLine(influence);
            }

            DimacsAddInteractions(_model.Setting.InteractionOrderPercent, lines, worker);
        }



        /// <summary>
        /// Writes random values from the distribution to features to the influence model 
        /// </summary>
        private void AddAttributesToModel()
        {
            var toAssign = new List<double>(FeatureValues);

            foreach (var opt in _varModel.BinaryOptions)
            {
                var next = _rand.Next(0, toAssign.Count);
                var value = toAssign[next];
                toAssign.RemoveAt(next);  //do not assign this attribute value again
                _influenceModel.BinaryOptionsInfluence.Add(opt, new InfluenceFunction(opt.Name + " * " + value));
            }
        }


        /// <summary>
        /// Checks a temporary configuration for SAT
        /// </summary>
        /// <returns></returns>
        private bool CheckConfigForSat()
        {
            var t = new MicrosoftSolverFoundation.CheckConfigurationSAT();
            var solver = new CheckConfigSAT(null);  //TODO is this call necessary for the path information??

            return t.checkConfigurationSAT(_tempConfig, _varModel, false);
        }
        #endregion

        #region Interactions


        private void DimacsAddInteractionToModel(IEnumerable<string> tempConfig, int index)
        {
            var val = InteractionValues[index];
            using (var sw = File.AppendText("dimacsInfluenceModel.txt"))
            {
                foreach (var t in tempConfig)
                {
                    sw.Write(t + " * ");
                }
                sw.WriteLine(val);
            }
        }

        /// <summary>
        /// Writes one interaction to the influence Model
        /// </summary>
        /// <param name="index"></param>
        private void AddInteractionToModel(int index)
        {
            //function e.g.  f1 * f2 * f3 * 1000;
            var function = _featuresInInteraction.Aggregate("", (current, op) => current + op.Name + " * ");
            function = function + InteractionValues[index];

            var interF = new InfluenceFunction(function);
            var inter = new Interaction(interF);  //TODO Need to set the BinaryOption for the Interaction?
            _influenceModel.InteractionInfluence.Add(inter, interF);
        }

        /// <summary>
        /// Selects Random Features and adds them to a temporary configuration
        /// </summary>
        /// <param name="order"></param>
        private void CreateRandomInteraction(int order)
        {
            var rands = new List<int>(order);
            var options = new BinaryOption[order];
            for (var i = 0; i < order; i++)
            {
                var randomVal = _rand.Next(0, FeatureValues.Length);
                if (rands.Contains(randomVal)) //we already have this random feature, try another
                {
                    i--; continue;
                }
                rands.Add(randomVal);  //add our found random val

                options[i] = _varModel.BinaryOptions[rands[i]];
                

                //add to random config if we dont have it already
                if (_tempConfig.Contains(options[i])) continue;
                _tempConfig.Add(options[i]);
                _featuresInInteraction.Add(options[i]);
                //TODO check if we can unite FeatureInInteaction with TempConfig
            }

        }

        private List<string> RandomDimacsInteraction(int order)
        {
            var rands = new List<int>(order);
            var options = new string[order];
            var tempConfig = new List<string>();
            for (var i = 0; i < order; i++)
            {
                var randomVal = _rand.Next(0, FeatureValues.Length);
                if (rands.Contains(randomVal)) //we already have this random feature, try another
                {
                    i--; continue;
                }
                rands.Add(randomVal);  //add our found random val

                options[i] = DimacsFeatureList[rands[i]];


                //add to random config if we dont have it already
                if (tempConfig.Contains(options[i])) continue;
                tempConfig.Add(options[i]);
                //_featuresInInteraction.Add(options[i]);
                //TODO check if we can unite FeatureInInteaction with TempConfig
            }
            return tempConfig;
        }


        private void DimacsAddInteractions(IReadOnlyList<double> orderPercentages, string[] lines, BackgroundWorker worker)
        {
            var currentOrder = 0;
            var index = 0;
            for (var i = 0; i < InteractionValues.Length; i++)
            {
                if (index < orderPercentages[currentOrder] * InteractionValues.Length * 0.01)
                {
                    index++;
                }
                else  //creates interactions of the next higher order
                {
                    currentOrder++;
                    index = 1;
                }

                var tempConfig = RandomDimacsInteraction(currentOrder + 2);
                if (!DimacsFound.Contains(tempConfig))
                {
                    //do nothing here? TODO
                    //DimacsFound.Add(tempConfig);
                }
                else
                {
                    index--;
                    i--;
                    _model.Tries++;
                    continue;
                }

                if (CheckDimacsSat(tempConfig, lines))
                {
                    _model.Tries = 0;
                    DimacsAddInteractionToModel(tempConfig, i);
                    DimacsFound.Add(tempConfig);
                    _model.Progress = i + 1;
                    worker.ReportProgress((int) _model.ProgressPercent, _model.Tries);
                }
                else
                {
                    Console.WriteLine("not sat");
                    _model.Tries++;
                    i--;
                    index--;
                    worker.ReportProgress((int)_model.ProgressPercent, _model.Tries);
                }
            }
        }

        private bool CheckDimacsSat(List<string> tempConfig, string[] lines)
        {
            var t = new MicrosoftSolverFoundation.CheckConfigurationSAT();
            //var solver = new CheckConfigSAT(null);
            return t.checkDimacsSAT(tempConfig, lines);
           
        }


        /// <summary>
        /// Searches for random interactions, adds them if SAT to influence model
        /// </summary>
        /// <param name="orderPercentages"></param>
        /// <param name="worker"></param>
        private void AddInteractions(IReadOnlyList<double> orderPercentages, BackgroundWorker worker)
        {
            _interactionsFound = new List<List<BinaryOption>>();
            _model.Tries = 0;

            //Order of Zero ==  Interaction of two features!
            var currentOrder = 0;
            var index = 0;

            for (var i = 0; i < InteractionValues.Length; i++)
            {
                //creates the next interaction of the same order
                if (index < orderPercentages[currentOrder] * InteractionValues.Length * 0.01)
                {
                    index++;
                }
                else  //creates interactions of the next higher order
                {
                    currentOrder++;
                    index = 1;
                }

                _tempConfig = new List<BinaryOption>();
                _featuresInInteraction = new List<BinaryOption>();

                //Add +2, since first order interaction contain 2 features, but currentOrder starts with 0  //TODO improve
                CreateRandomInteraction(currentOrder + 2);

                //we already have found this exact same interaction  //TODO check if comparator is working correct
                if (_interactionsFound.Contains(_featuresInInteraction))
                {
                    _model.Tries++;
                    i--;
                    index--;
                    continue;
                }

                //our random features for the interaction are not SAT
                if (!CheckConfigForSat())
                {
                    _model.Tries++;
                    worker.ReportProgress((int)_model.ProgressPercent, _model.Tries);
                    i--;
                    index--;
                    continue;
                }

                //we found a new interaction
                _model.Tries = 0;
                AddInteractionToModel(i);
                _interactionsFound.Add(_featuresInInteraction);
                _model.Progress = i + 1;
                worker.ReportProgress((int)_model.ProgressPercent, i);

            }
        }

        /// <summary>
        /// Scale the distribution toScale, to the Interval  Min,Max
        /// </summary>
        /// <param name="toScale"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Distribution InteractionToScale(Distribution toScale, double min, double max)
        {

            var fromMin = toScale.Values.Min();
            var fromMax = toScale.Values.Max();

            for (var i = 0; i < toScale.Values.Length; i++)
            {
                toScale.Values[i] = ScaleFromTo(fromMin, fromMax, min, max, toScale.Values[i]);
            }
            return toScale;
        }

        /// <summary>
        /// Scales the distribution <param>toScale</param>  to the Interaval of <param>from</param>
        /// </summary>
        /// <param name="toScale"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public Distribution InteractionToScale(Distribution toScale, Distribution from)
        {
            if (from != null)
            {
                var fromMax = toScale.Values.Max();
                var fromMin = toScale.Values.Min();
                var toMax = from.Values.Max();
                var toMin = from.Values.Min();

                for (var i = 0; i < toScale.Values.Length; i++)
                {
                    toScale.Values[i] = ScaleFromTo(fromMin, fromMax, toMin, toMax, toScale.Values[i]);
                }
                return toScale;

            }
            Debug.WriteLine("Could not scale interaction values to featurevalues");
            return null;
        }

        /// <summary>
        /// Make the interaction values similar to the interaction values of the selected feature non-functional property
        /// E.g     Feature distribution        =  binarySize: BDB
        ///         Interaction distribution    =  performance: SQLite
        /// Then scales the interaction distribution, to the values of the Interactions: BinarySize: BDB
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="interaction"></param>
        /// <returns></returns>
        private Distribution ScaleInteractionValuesToFeature(Distribution feature, Distribution interaction)
        {

            //elevator has no interaction distribution, cant scale
            if (feature.DisplayName == "Elevator")
            {
                return interaction;
            }

            //different SPL selected, scale the values accordingly
            if (feature.DisplayName != interaction.DisplayName)
            {
                var interactionOfFeature = _model.DStore.SelectedFeatureDistribution;//Model.LoadSingleDistribution(feature.SelectedNFProperty.Name, Model.SelectedFeatureDistribution.DisplayName);
                return InteractionToScale(interaction, interactionOfFeature);

            }
            //same name but maybe from different nfp
            if (feature.SelectedNfProperty.Name == interaction.SelectedNfProperty.Name)
            {
                //same nfp: No need to scale, is already at correct values
                return interaction;
            }

            var interactionOfOtherNfp = _model.LoadSingleDistribution(feature.SelectedNfProperty.Name, _model.DStore.SelectedFeatureDistribution.DisplayName);
            return InteractionToScale(interaction, interactionOfOtherNfp);
        }

        #endregion

        /// <summary>
        /// Scales one value 
        /// </summary>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ScaleFromTo(double fromMin, double fromMax, double toMin, double toMax, double value)
        {
            if (fromMax == fromMin) return fromMin;
            var div = (toMax - toMin) * (value - fromMin);
            var quot = fromMax - fromMin;
            return div / quot + toMin;
        }
    }
}
