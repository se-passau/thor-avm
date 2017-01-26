using System;
using Accord.Statistics.Visualizations;
using InteracGenerator.Problem;

namespace InteracGenerator.FitnessCalculation
{
    public class ChiSquare : IFitnessTest
    {
        private readonly Thor _model;
        private readonly int _counter;

        public ChiSquare(Thor model, int counter)
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
                    return BinnedChi(coll.First, coll.Second);
                }
                case Distribution.DistributionType.Interaction:
                {
                    var coll = bd.GetBinCollection(first.Values, second.Values, _model.InteracDynamicHist, _counter);
                    return BinnedChi(coll.First, coll.Second);
                }
                case Distribution.DistributionType.Variant:
                {
                    var coll = bd.GetBinCollection(first.Values, second.Values, _model.VariantDynamicHist, _counter);
                    return BinnedChi(coll.First, coll.Second);
                }
                default:
                    throw new NotImplementedException();
            }

            return 0;
            //return BinnedChi();
        }


        public static double BinnedChi(HistogramBinCollection x, HistogramBinCollection y)
        {
            var result = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                var quot = Math.Pow(x[i].Value - y[i].Value, 2);
                double div = x[i].Value + y[i].Value;

                if (div != 0) result += quot/div;
               
            }
            return result/2.0;
        }
    }
}
