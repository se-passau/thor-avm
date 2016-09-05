using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using SPLConqueror_Core;

namespace InteracGenerator.VariantGenerators
{
    internal class VariantAttributeCalculator
    {

        public byte[,] CalculateVariantMatrix(List<List<BinaryOption>> randomConfigs, List<BinaryOption> features)
        {

            var featuresInConfigMatrix = new byte[randomConfigs.Count, features.Count];
            var bound0 = featuresInConfigMatrix.GetUpperBound(0);
            var bound1 = featuresInConfigMatrix.GetUpperBound(1);


            Parallel.For(0, bound0 + 1, i =>
            {
                for (var j = 0; j <= bound1; j++)
                {
                    if (randomConfigs[i].Contains(features[j]))
                    {
                        featuresInConfigMatrix[i, j] = 1;
                    }
                }
            });

            /*for (var i = 0; i <= bound0; i++)
            {
                if (i == bound0)
                {
                    Console.WriteLine(randomConfigs[i].Count);
                }
                for (var j = 0; j <= bound1; j++)
                {
                    if (randomConfigs[i] == null) continue;
                    if (randomConfigs[i].Contains(features[j]))
                    {
                        featuresInConfigMatrix[i, j] = 1;
                    }
                    /*
                    TODO else case necessary?
                    else
                    {
                        featuresInConfigMatrix[i, j] = 0;
                    } 
                }
            } */
            return featuresInConfigMatrix;
        }

        public double[,] CalculateVariantMatrix(List<List<string>> randConfigs, List<string> features)
        {
            var featuresInConfigMatrix = new double[randConfigs.Count, features.Count];
            var bound0 = featuresInConfigMatrix.GetUpperBound(0);
            var bound1 = featuresInConfigMatrix.GetUpperBound(1);
            for (var i = 0; i <= bound0; i++)
            {
                for (var j = 0; j <= bound1; j++)
                {
                    if (randConfigs[i] == null) continue;
                    if (randConfigs[i].Contains(features[j]))
                    {
                        featuresInConfigMatrix[i, j] = 1;
                    }
                }
            }
            return featuresInConfigMatrix;
        }

        public double[] CalculateInteractionSums(List<List<string>> randomConfigs, List<List<string>> interactionList,
            double[] interactionValues)
        {
            var configHasInteraction = new double[randomConfigs.Count, interactionList.Count];

            for (var i = 0; i < randomConfigs.Count; i++)
            {
                for (var j = 0; j < interactionList.Count; j++)
                {
                    var interactionsF = interactionList[j];
                    if (randomConfigs[i] == null) continue;
                    var inInteraction = interactionsF.All(opt => randomConfigs[i].Contains(opt));
                    if (inInteraction)
                    {
                        configHasInteraction[i, j] = 1;
                    }
                }
            }
            Console.WriteLine("Has interactions? " + configHasInteraction.Max());
            return configHasInteraction.Dot(interactionValues);
        }

        public
            byte[,] GetInteractionMatrix(List<List<BinaryOption>> randomConfigs, List<List<BinaryOption>> interactionList)
        {
            //var configHasInteraction = new double[randomConfigs.Count, interactionList.Count];
            var parallel = new byte[randomConfigs.Count, interactionList.Count];
            /*for (var i = 0; i < randomConfigs.Count; i++)
            {
                for (var j = 0; j < interactionList.Count; j++)
                {

                    var interactionsF = interactionList[j];

                    //true when random config contains all binary options of the interaction
                    if (randomConfigs[i] == null) continue;
                    var inInteraction = interactionsF.All(opt => randomConfigs[i].Contains(opt));
                    if (inInteraction)
                    {
                        configHasInteraction[i, j] = 1.0;
                    }
                }
            } */

            Parallel.For(0, randomConfigs.Count, i =>
            {
                for (var j = 0; j < interactionList.Count; j++)
                {
                    var interactionF = interactionList[j];
                    var inInteraction = interactionF.All(opt => randomConfigs[i].Contains(opt));
                    if (inInteraction)
                    {
                        parallel[i, j] = 1;
                    }
                }
            });
            return parallel;
        }

        public
            double[] CalculateInteractionSums(List<List<BinaryOption>> randomConfigs, List<List<BinaryOption>> interactionList, double[] interactionValues)
        {
            var configHasInteraction = new double[randomConfigs.Count, interactionList.Count];

            for (var i = 0; i < randomConfigs.Count; i++)
            {
                for (var j = 0; j <  interactionList.Count; j++)
                {

                    var interactionsF = interactionList[j];

                    //true when random config contains all binary options of the interaction
                    if (randomConfigs[i] == null) continue;
                    var inInteraction = interactionsF.All(opt => randomConfigs[i].Contains(opt));
                    if (inInteraction)
                    {
                        configHasInteraction[i, j] = 1;
                    }
                }
            }
            
            return  configHasInteraction.Dot(interactionValues);
        }      
    }
}
