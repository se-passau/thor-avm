using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using SPLConqueror_Core;


namespace InteracGenerator.VariantGenerators
{
    internal abstract class AbstractVariantGenerator<T> : IVariantGenerator<T>, IHeuristic<T>
    {

        public enum Method { FeatureWise, NegativeFeatureWise, Pairwise, Random, PseudoRandom, LinearRandom, QuadraticRandom }

        public List<List<T>> GenerateVariants(List<HeuristicOption> options, BackgroundWorker worker)
        {



            var runs = new Task[options.Count];
            var results = new List<List<T>>();

            for (var i = 0; i < options.Count; i++)
            {
                var h = options[i];
                runs[i] = Task.Factory.StartNew(() =>
                {
                    var result = GenerateAsync(h, worker);
                    results.AddRange(result);
                });
            }
            Task.WaitAll(runs);
            return results;
        }

        public List<List<T>> GenerateAsync(HeuristicOption option, BackgroundWorker worker)
        {
            switch (option.Method)
            {
                case VariantGenerator.Method.QuadraticRandom:
                    return QuadraticRandom(option, worker);
                case VariantGenerator.Method.FeatureWise:
                    return FeatureWise(option, worker);
                case VariantGenerator.Method.NegativeFeatureWise:
                    return NegFeatureWise(option, worker);
                case VariantGenerator.Method.Pairwise:
                    return PairWise(option, worker);
                case VariantGenerator.Method.LinearRandom:
                    return LinearRandom(option, worker);
                case VariantGenerator.Method.Random:
                    return Random(option, worker);
                case VariantGenerator.Method.PseudoRandom:
                    return FixedRandom(option, worker);
                default:
                    throw new NotImplementedException();

            }
        }

        public abstract List<List<T>> QuadraticRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> LinearRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> FixedRandom(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> FeatureWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> NegFeatureWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> PairWise(HeuristicOption opt, BackgroundWorker worker);
        public abstract List<List<T>> Random(HeuristicOption opt, BackgroundWorker worker);
    }
}
