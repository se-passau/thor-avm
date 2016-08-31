using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Testing;

namespace InteracGenerator.FitnessCalculation
{
    class KsTest : IFitnessTest
    {
        public double Calculate(Distribution first, Distribution second)
        {
            var t = new TwoSampleKolmogorovSmirnovTest(first.Values, second.Values);
            return t.Statistic;
        }
    }
}
