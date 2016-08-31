using System.Collections.Generic;
using System.ComponentModel;

namespace InteracGenerator.InteracWeaving
{
    internal interface IInteracWeaver<T>
    {
        List<List<T>> GetInteractions();

        void WeaveInteractions(List<T> features, Distribution featDist, Distribution interacDist, BackgroundWorker worker);

        bool CheckInteractionSat(List<T> tempConfig);

        List<T> SelectRandomInteraction(int order);

        void SetUpWeaver();

        void AddAttributesToModel();

        void AddInteractionToModel(int index, List<T> tempConfig);

        void AddInteractions(IReadOnlyList<double> orderP, BackgroundWorker worker);
    }
}
