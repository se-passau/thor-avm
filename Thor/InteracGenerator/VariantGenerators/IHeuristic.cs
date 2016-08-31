using System.Collections.Generic;
using System.ComponentModel;

namespace InteracGenerator.VariantGenerators
{
    internal interface IHeuristic<T>
    {

        List<List<T>> QuadraticRandom(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> LinearRandom(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> FixedRandom(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> FeatureWise(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> NegFeatureWise(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> PairWise(HeuristicOption opt, BackgroundWorker worker);

        List<List<T>> Random(HeuristicOption opt, BackgroundWorker worker);
    }
}
