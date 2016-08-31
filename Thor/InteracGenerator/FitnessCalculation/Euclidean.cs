using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Visualizations;
using InteracGenerator.Problem;

namespace InteracGenerator.FitnessCalculation
{
    public class Euclidean : IFitnessTest
    {
        private readonly InterGen _model;
        private readonly int _counter;

        public Euclidean(InterGen model, int counter)
        {
            _model = model;
            _counter = counter;
        }

        public double Calculate(Distribution first, Distribution second)
        {
            var bd = new BinnedDistance();
            switch (first.DistType)
            {
                case Distribution.DistributionType.Feature:
                    {
                        var coll = bd.GetBinCollection(first.Values, second.Values, _model.FeaturesDynamicHist, _counter);
                        return BinnedEuclidean(coll.First, coll.Second);
                    }
                case Distribution.DistributionType.Interaction:
                    {
                        var coll = bd.GetBinCollection(first.Values, second.Values, _model.InteracDynamicHist, _counter);
                        return BinnedEuclidean(coll.First, coll.Second);
                    }
                case Distribution.DistributionType.Variant:
                    {
                        var coll = bd.GetBinCollection(first.Values, second.Values, _model.VariantDynamicHist, _counter);
                        return BinnedEuclidean(coll.First, coll.Second);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static double BinnedEuclidean(HistogramBinCollection x, HistogramBinCollection y)
        {
            var result = x.Select((t, i) => Math.Pow(t.Value - y[i].Value, 2)).Sum();
            return Math.Sqrt(result);
        }
    }
}
