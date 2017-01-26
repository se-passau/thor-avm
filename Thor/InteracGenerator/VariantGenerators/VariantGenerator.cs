using System.Collections.Generic;
using System.ComponentModel;
using MachineLearning.Sampling.Heuristics;
using SPLConqueror_Core;

namespace InteracGenerator.VariantGenerators
{
    

    internal class VariantGenerator : AbstractVariantGenerator<BinaryOption>
    {

        private readonly VariabilityModel _vm;

        public VariantGenerator(VariabilityModel vm)
        {
            _vm = vm;
        }

        public override List<List<BinaryOption>> QuadraticRandom(HeuristicOption opt, BackgroundWorker worker)
        {
            if (!opt.HasTreshold && !opt.HasScale) return null;
            var vg = new MicrosoftSolverFoundation.VariantGenerator();
           
            return vg.GenerateRQuadratic(_vm, opt.Treshold, opt.Scale, opt.SolverTimeout, worker);
        }

        public override List<List<BinaryOption>> LinearRandom(HeuristicOption opt, BackgroundWorker worker)
        {
            if (!opt.HasTreshold) return null;
            var vg1 = new MachineLearning.Solver.VariantGenerator(null);
            return vg1.GenerateRLinear(_vm, opt.Treshold, opt.SolverTimeout, worker);
            //var vg = new MicrosoftSolverFoundation.VariantGenerator();
            //return vg.GenerateRLinear(_vm, opt.Treshold, worker);
        }

        public override List<List<BinaryOption>> FixedRandom(HeuristicOption opt, BackgroundWorker worker)
        {
            if (!opt.HasTreshold) return null;
            var vg = new MicrosoftSolverFoundation.VariantGenerator();
            return vg.generateR1(_vm, opt.Treshold, opt.SolverTimeout, worker);
        }

        public override List<List<BinaryOption>> FeatureWise(HeuristicOption opt, BackgroundWorker worker)
        {
            return !opt.HasTimeLimit ? null : new FeatureWise().generateFeatureWiseUntilSeconds(_vm, opt.TimeLimitSeconds);
        }

        public override List<List<BinaryOption>> NegFeatureWise(HeuristicOption opt, BackgroundWorker worker)
        {
            return !opt.HasTimeLimit ? null : new NegFeatureWise().generateNegativeFWUntilSeconds(_vm, opt.TimeLimitSeconds);
        }

        public override List<List<BinaryOption>> PairWise(HeuristicOption opt, BackgroundWorker worker)
        {
            return !opt.HasTimeLimit ? null : new PairWise().generatePairWiseVariantsUntilSeconds(_vm, opt.TimeLimitSeconds);
        }

        public override List<List<BinaryOption>> Random(HeuristicOption opt, BackgroundWorker worker)
        {
            if (!opt.HasTreshold) return null;
            if (!opt.HasTimeLimit) return null;
            if (opt.Modulo == 0) return null;
            var vg = new MicrosoftSolverFoundation.VariantGenerator();
            return vg.generateRandomVariantsUntilSeconds(_vm, opt.TimeLimitSeconds, opt.Treshold, opt.Modulo);
        }
    }
}
