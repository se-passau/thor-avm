using Accord.Statistics.Visualizations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InteracGenerator.Problem
{

    internal struct BinPairCollection
    {
        public HistogramBinCollection First;
        public HistogramBinCollection Second;
    }


    internal class BinnedDistance
    {
        private readonly HistogramBinCollection _firstBins;
        private readonly HistogramBinCollection _secondBins;


        public BinnedDistance()
        {
        }

        public BinnedDistance(Distribution first, Distribution second, int numberOfBins)
        {
            var firstHist = new Histogram();
            firstHist.Compute(first.Values, numberOfBins);
            _firstBins = firstHist.Bins;

            var secondHist = new Histogram();
            secondHist.Compute(second.Values, numberOfBins);
            _secondBins = secondHist.Bins;
        }

        public BinnedDistance(Distribution first, Distribution second, DynamicHist dynamicHist, int currentEvolution)
        {
            var firstHisto = new Histogram();
            var secondHisto = new Histogram();

            if (dynamicHist.StartBins != 0 && dynamicHist.EndBins != 0)
            {
                var size = dynamicHist.GetBinSize(currentEvolution);
                firstHisto.Compute(first.Values, size);
                secondHisto.Compute(second.Values,size);
                _firstBins = firstHisto.Bins;
                _secondBins = secondHisto.Bins;
                return;
            }

            if (dynamicHist.UseCustomStatic)
            {
                firstHisto.Compute(first.Values, dynamicHist.CustomStaticSize);
                secondHisto.Compute(second.Values, dynamicHist.CustomStaticSize);
                _firstBins = firstHisto.Bins;
                _secondBins = secondHisto.Bins;
                return;
            }

            if (dynamicHist.UseScott)
            {
                firstHisto.AutoAdjustmentRule = BinAdjustmentRule.Scott;
                secondHisto.AutoAdjustmentRule = BinAdjustmentRule.Scott;
            }
            if (dynamicHist.UseSturges)
            {
                firstHisto.AutoAdjustmentRule = BinAdjustmentRule.Sturges;
                secondHisto.AutoAdjustmentRule = BinAdjustmentRule.Sturges;
            }
            firstHisto.Compute(first.Values);
            secondHisto.Compute(second.Values);
            _firstBins = firstHisto.Bins;
            _secondBins = secondHisto.Bins;
        }



        public BinPairCollection GetBinCollection(double[] first, double[] second, DynamicHist dynamicHist,
            int currentEvolution)
        {
            var firstHisto = new Histogram();
            var secondHisto = new Histogram();

            if (dynamicHist.StartBins != 0 && dynamicHist.EndBins != 0)
            {
                var size = dynamicHist.GetBinSize(currentEvolution);
                firstHisto.Compute(first, size);
                secondHisto.Compute(second, size);
                //_firstBins = firstHisto.Bins;
                //_secondBins = secondHisto.Bins;
                return new BinPairCollection {First = firstHisto.Bins, Second = secondHisto.Bins};
            }

            if (dynamicHist.UseCustomStatic)
            {
                firstHisto.Compute(first, dynamicHist.CustomStaticSize);
                secondHisto.Compute(second, dynamicHist.CustomStaticSize);
                //_firstBins = firstHisto.Bins;
                //_secondBins = secondHisto.Bins;
                return new BinPairCollection { First = firstHisto.Bins, Second = secondHisto.Bins};
            }

            if (dynamicHist.UseScott)
            {
                firstHisto.AutoAdjustmentRule = BinAdjustmentRule.Scott;
                secondHisto.AutoAdjustmentRule = BinAdjustmentRule.Scott;
            }
            if (dynamicHist.UseSturges)
            {
                firstHisto.AutoAdjustmentRule = BinAdjustmentRule.Sturges;
                secondHisto.AutoAdjustmentRule = BinAdjustmentRule.Sturges;
            }
            firstHisto.Compute(first);
            secondHisto.Compute(second);
            return new BinPairCollection { First = firstHisto.Bins, Second = secondHisto.Bins };
            //_firstBins = firstHisto.Bins;
            //_secondBins = secondHisto.Bins;
        }



        private static double[] NormalizeData(IEnumerable<double> data, double min, double max)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data
                .Select(d => (d - dataMin) / range)
                .Select(n => (1 - n) * min + n * max)
                .ToArray();
        }

        public double EuclidianDist() {
            var result = _firstBins.Select((t, i) => Math.Pow(t.Value - _secondBins[i].Value, 2)).Sum();
            return Math.Sqrt(result);
        }

        public double ChiSquaredDist() {

            var result = 0.0;

            for (var i = 0; i < _firstBins.Count; i++) {
                var quot = Math.Pow(_firstBins[i].Value - _secondBins[i].Value, 2);
                double div = _firstBins[i].Value + _secondBins[i].Value;

                //dont add distance if dividend is zero
                if (div != 0)
                {
                    result += quot/div;
                }
            }
            return result / 2.0;
        }

        /// <summary>
        /// Chi squared without fields, only local parameters, introduced for testing purposes on parallel execution
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double ChiSquaredDistance(HistogramBinCollection x, HistogramBinCollection y)
        {
            Debug.WriteLineIf(x.Count == y.Count, "Error: Chi Squared Distance Comparison of Bins of different Length");
            var result = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                var quot = Math.Pow(x[i].Value - y[i].Value, 2);
                double div = x[i].Value + y[i].Value;
                result += quot / div;
            }
            return result / 2.0;
        }

        /// <summary>
        /// Euclidean distance with local parameters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double EuclidianDist(HistogramBinCollection x, HistogramBinCollection y)
        {
            //Square root of Sum of  (x_i - y_i)^2
            var result = x.Select((t, i) => Math.Pow(t.Value - y[i].Value, 2)).Sum();
            return Math.Sqrt(result);
        }
    }
}
