using System.ComponentModel;
using InteracGenerator.Problem;
using JMetalCSharp.Core;

namespace InteracGenerator.Helper
{
    public static class ProgressReporter
    {

        private static InterGen _model;

        public static void Init(InterGen model)
        {
            _model = model;
        }



        public static void ReportSolution(int evaluation, Solution sol, BackgroundWorker worker)
        {
            var progress = new UserProgress
            {
                EvolutionStep = evaluation
            };


            var list = ObjectiveMapping.GetList(_model.ProblemType);

            var objindex = 0;
            if (list[0])
            {
                progress.FeatureP = sol.Objective[objindex];
                objindex++;
            }
            if (list[1])
            {
                progress.InteracP = sol.Objective[objindex];
                objindex++;
            }
            if (list[2])
            {
                progress.VariantP = sol.Objective[objindex];
            }

            worker.ReportProgress((int) 100.0 * evaluation / _model.Setting.MaxEvaluations, progress);
        }

    }
}
