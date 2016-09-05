using System.Collections.Generic;
using System.ComponentModel;

namespace InteracGenerator.VariantGenerators
{
    internal interface IVariantGenerator<T>
    {

        List<List<T>>  GenerateVariants(List<HeuristicOption> options, BackgroundWorker worker);


    }
}
