using System;
using System.Diagnostics;
using System.Threading;
using Accord.Statistics.Testing;
using InteracGenerator.Problem;

namespace InteracGenerator.FitnessCalculation
{
    internal class FitnessCalculator
    {
        private readonly int _counter;
        private readonly InterGen _model;
        private long _fitnessTime;

        public FitnessCalculator(InterGen model, int counter)
        {
            _model = model;
            _counter = counter;   
        }

        public long GetFitnessTime()
        {
            return _fitnessTime;
        }

        public FitnessValues Calculate(Distribution variant, Distribution varTarget, Distribution feat, Distribution featTarget, Distribution interac, Distribution interacTarget)
        {
            var test = GetTest();
            double featVal = -1;
            double interacVal = -1;
            double varVal = -1;


            var testTime = new Stopwatch();
            testTime.Start();

            if (_model.Setting.FeatureFitness)
            {
                featVal = test.Calculate(feat, featTarget);
            }
            if (_model.Setting.InteracFitness)
            {
                interacVal = test.Calculate(interac, interacTarget);
            }
            if (_model.Setting.VariantFitness)
            {
                varVal = test.Calculate(variant, varTarget);
            }

            testTime.Stop();
            _fitnessTime += testTime.ElapsedMilliseconds;

            return new FitnessValues { FeatureVal = featVal, InteracVal = interacVal, VariantVal = varVal };

            /*if (_model.Setting.NoVariantCalculation)
            {
                if (_model.Setting.NumberOfInteractions > 0)
                {
                    return CalcFeatAndInteracs(Feat, FeatTarget, Interac, InteracTarget);
                }
                return CalcFeatOnly(Feat, FeatTarget);
            }

            if (_model.Setting.NumberOfInteractions > 0)
            {
                return CalcAll(Feat, FeatTarget, Interac, InteracTarget, Variant, VarTarget);
            }

            return CalcFeatAndVariant(Variant, VarTarget, Feat, FeatTarget); */

        }

        private IFitnessTest GetTest()
        {
            if (_model.Setting.UseCmv) return new CramerVonMises();
            if (_model.Setting.UseChiSquared) return new ChiSquare(_model, _counter);
            if (_model.Setting.UseEuclidean) return new Euclidean(_model, _counter);
            if (_model.Setting.UseKs) return new KsTest();
            return null;
        }

    }
}
