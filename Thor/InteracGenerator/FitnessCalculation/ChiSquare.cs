using System;
using Accord.Statistics.Visualizations;
using InteracGenerator.Problem;

namespace InteracGenerator.FitnessCalculation
{
    public class ChiSquare : IFitnessTest
    {
        private readonly InterGen _model;
        private readonly int _counter;

        public ChiSquare(InterGen model, int counter)
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
                if (double.IsNaN(quot))
                {
                    Console.WriteLine("Wat");
                }
                double div = x[i].Value + y[i].Value;

                if (double.IsNaN(div))
                {
                    Console.WriteLine("Wat");
                }
                if (div != 0) result += quot/div;
                if (double.IsNaN(result))
                {
                    Console.WriteLine(quot);
                    Console.WriteLine(div);
                }
            }
            if (double.IsNaN(result))
            {
                Console.WriteLine("Why");
            }
            return result/2.0;
        }
    }
}
