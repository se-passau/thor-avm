using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace InteracGenerator.InteracWeaving
{
    internal abstract class AbstractWeaver<T> : IInteracWeaver<T>
    {
        protected double[] FeatureValues;
        protected double[] InteractionValues;
        protected InterGen Model;
        protected BackgroundWorker Worker;
        protected Random Rand;
        protected List<T> FeatureList;
        protected List<List<T>> FoundInteractions; 

        protected AbstractWeaver(InterGen model)
        {
            Rand = new Random();
            Model = model;
        }

        public List<List<T>> GetInteractions()
        {
            if (FoundInteractions == null)
            {
                Console.WriteLine("null");
            }
            return FoundInteractions;
        }

        public void WeaveInteractions(List<T> features, Distribution featDist, Distribution interacDist, BackgroundWorker worker)
        {
            FeatureValues = featDist.Values;
            InteractionValues = interacDist.Values;
            Worker = worker;
            FeatureList = features;

            SetUpWeaver();
            if (Model.Setting.LoadedInteractions)
            {
                FoundInteractions = LoadInteractions();
                Console.WriteLine(FoundInteractions.Count);
            }
            else
            {
                AddAttributesToModel();
                AddInteractions(Model.Setting.InteractionOrderPercent, worker);
                //Model.WriteFoundDimacsInteractions();
            }
        }

        public abstract List<List<T>> LoadInteractions(string fileName = "foundInteractions.txt"); 

        public abstract bool CheckInteractionSat(List<T> tempConfig);

        public abstract void SetUpWeaver();

        public List<T> SelectRandomInteraction(int order)
        {
            var rands = new List<int>(order);
            var options = new T[order];
            var tempConfig = new List<T>();
            for (var i = 0; i < order; i++)
            {
                var randomVal = Rand.Next(1, FeatureValues.Length);
                if (rands.Contains(randomVal)) //we already have this random feature, try another
                {
                    i--; continue;
                }
                rands.Add(randomVal);  //add our found random val

                options[i] = FeatureList[rands[i]];


                //add to random config if we dont have it already
                if (tempConfig.Contains(options[i])) continue;
                tempConfig.Add(options[i]);
                //_featuresInInteraction.Add(options[i]);
                //TODO check if we can unite FeatureInInteaction with TempConfig
            }
            return tempConfig;
        }

        public abstract void AddAttributesToModel();

        public abstract void AddInteractionToModel(int index, List<T> tempConfig);

        public void AddInteractions(IReadOnlyList<double> orderP, BackgroundWorker worker)
        {
            
            Model.Tries = 0;

            //Order of Zero ==  Interaction of two features!
            var currentOrder = 0;
            var index = 0;

            for (var i = 0; i < InteractionValues.Length; i++)
            {
                //creates the next interaction of the same order
                if (index < orderP[currentOrder] * InteractionValues.Length * 0.01)
                {
                    index++;
                }
                else //creates interactions of the next higher order
                {
                    currentOrder++;
                    index = 1;
                }
                var tempConfig = SelectRandomInteraction(currentOrder + 2);

                //we already have found this exact same interaction  //TODO check if comparator is working correct
                if (AlreadyFoundInteraction(tempConfig))
                {
                    Model.Tries++;
                    if (Model.Tries > 5000)
                    {
                        Console.WriteLine("I cant find anymore new Interactions,  decide how to handle this case!");
                        throw new NotImplementedException();
                    }
                    i--;
                    index--;
                    continue;
                }

                //our random feature selection for the interaction is not SAT
                if (!CheckInteractionSat(tempConfig))
                {
                    Model.Tries++;
                    worker.ReportProgress((int)Model.ProgressPercent, Model.Tries);
                    i--;
                    index--;
                    continue;
                }

                //we found a new interaction
                Model.Tries = 0;
                AddInteractionToModel(i, tempConfig);
                FoundInteractions.Add(tempConfig);
                Model.Progress = i + 1;
                worker.ReportProgress((int)Model.ProgressPercent, i);

            }
        }


        /// <summary>
        /// Checks if this Interactions is already in the Found Interactions.
        /// </summary>
        /// <param name="newConfig"></param>
        /// <returns></returns>
        private bool AlreadyFoundInteraction(ICollection<T> newConfig)
        {
            foreach (var foundInter in FoundInteractions)
            {
                var isSame = true;
                foreach (var interacFeat in foundInter)
                {
                    //the Interaction to test does not contain a Feature from the current already found interaction
                    if (!newConfig.Contains(interacFeat))
                    {
                        isSame = false;
                        break;
                    }
                }
                if (isSame)
                {
                    return true;
                }
            }
            return false;
        }

      
        #region helper

        /// <summary>
        /// Make the interaction values similar to the interaction values of the selected feature non-functional property
        /// E.g     Feature distribution        =  binarySize: BDB
        ///         Interaction distribution    =  performance: SQLite
        /// Then scales the interaction distribution, to the values of the Interactions: BinarySize: BDB
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="interaction"></param>
        /// <returns></returns>
        protected Distribution ScaleInteractionValuesToFeature(Distribution feature, Distribution interaction)
        {

            //elevator has no interaction distribution, cant scale
            if (feature.DisplayName == "Elevator")
            {
                return interaction;
            }

            //different SPL selected, scale the values accordingly
            if (feature.DisplayName != interaction.DisplayName)
            {
                var interactionOfFeature = Model.DStore.SelectedFeatureDistribution;//Model.LoadSingleDistribution(feature.SelectedNFProperty.Name, Model.SelectedFeatureDistribution.DisplayName);
                return InteractionToScale(interaction, interactionOfFeature);

            }
            //same name but maybe from different nfp
            if (feature.SelectedNfProperty.Name == interaction.SelectedNfProperty.Name)
            {
                //same nfp: No need to scale, is already at correct values
                return interaction;
            }

            var interactionOfOtherNfp = Model.LoadSingleDistribution(feature.SelectedNfProperty.Name, Model.DStore.SelectedFeatureDistribution.DisplayName);
            return InteractionToScale(interaction, interactionOfOtherNfp); 
            
        }


        /// <summary>
        /// Scale the distribution toScale, to the Interval  Min,Max
        /// </summary>
        /// <param name="toScale"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Distribution InteractionToScale(Distribution toScale, double min, double max)
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
        /// Scales one value 
        /// </summary>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double ScaleFromTo(double fromMin, double fromMax, double toMin, double toMax, double value)
        {
            var div = (toMax - toMin) * (value - fromMin);
            var quot = fromMax - fromMin;
            return div / quot + toMin;
        }

        #endregion
    }
}
