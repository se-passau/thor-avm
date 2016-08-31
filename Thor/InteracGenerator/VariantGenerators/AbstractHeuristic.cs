using System.Collections.Generic;
using System.ComponentModel;

namespace InteracGenerator.VariantGenerators
{
    internal abstract class AbstractHeuristic<T> : IHeuristic<T>
    {
        public abstract List<List<T>> QuadraticRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> LinearRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> FixedRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> FeatureWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> NegFeatureWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> PairWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> Random(HeuristicOption opt, BackgroundWorker worker);
    }
}
