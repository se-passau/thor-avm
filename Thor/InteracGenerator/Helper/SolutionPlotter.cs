using Accord.Math;
using JMetalCSharp.Core;
using JMetalCSharp.Utils.Wrapper;

namespace InteracGenerator.Helper
{
    public static class SolutionPlotter
    {

        private static int _counter;
        private static InterGen _model;

      

        public static void SetModel(InterGen mdl)
        {
            _model = mdl;
        }

      

        public static void Plot(Solution solution)
        {
            if (!_model.Setting.DrawDensity && !_model.Setting.DrawHistogram) return;
            _counter++;

            if (_counter%(_model.Setting.PlotStepSize/_model.Setting.PopulationSize) != 0) return;

            var doubleVal = new double[_model.Setting.NumberOfFeatures];
            var interacVal = new double[_model.Setting.NumberOfInteractions];
            var values = new XReal(solution);
            for (var i = 0; i < _model.Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }
            if (_model.Setting.NumberOfInteractions > 0)
            {
                for (var i = 0; i < _model.Setting.NumberOfInteractions; i++)
                {
                    interacVal[i] = values.GetValue(i + _model.Setting.NumberOfFeatures);
                }
            }

            if (_model.Setting.DrawDensity) RIntegrator.PlotFeatureTarget(doubleVal, _model.DStore.SelectedFeatureDistribution.Values,
                _model.Setting.FeatureAdjust);
            if (_model.Setting.DrawHistogram)RIntegrator.FeatureHistAndDens(doubleVal, _model.DStore.SelectedFeatureDistribution.Values);

            if (_model.Setting.NumberOfInteractions > 0)
            {

                if (_model.Setting.DrawDensity) RIntegrator.PlotInteracTarget(interacVal, _model.DStore.SelectedInteractionDistribution.Values);
                if (_model.Setting.DrawHistogram) RIntegrator.InteracHist(interacVal, _model.DStore.SelectedInteractionDistribution.Values);
                if (!_model.Setting.NoVariantCalculation)
                {
                    //interactions and variants
                    var variantValuesWithoutInteraction = _model.FeatureMatrix.Dot(doubleVal);
                    var interacVals = _model.InteractionMatrix.Dot(interacVal);
                    var variantResults = variantValuesWithoutInteraction.Add(interacVals);
                    if (_model.ScaledVariantTarget != null)
                    {
                        FMScaling.InteractionToScale(_model.ScaledVariantTarget, variantResults.Min(),
                            variantResults.Max());
                        if (_model.Setting.DrawDensity)
                            RIntegrator.PlotVariantTarget(variantResults, _model.ScaledVariantTarget.Values);
                        if (_model.Setting.DrawHistogram)
                            RIntegrator.VariantComparisonHisto(variantResults, _model.ScaledVariantTarget.Values);
                    }
                    else
                    {
                        if (_model.Setting.DrawDensity)
                            RIntegrator.PlotVariantTarget(variantResults);
                    }

                }
                else
                {
                    //interactions and no variants
                }
            }
            else
            {
                if (!_model.Setting.NoVariantCalculation)
                {
                    //no interactions and variants
                    var variantResults = _model.FeatureMatrix.Dot(doubleVal);
                    //FMScaling.InteractionToScale(_model.ScaledVariantTarget, variantResults.Min(),
                    //    variantResults.Max());
                    if (_model.Setting.DrawDensity) RIntegrator.PlotVariantTarget(variantResults);
                    if (_model.Setting.DrawHistogram)
                        RIntegrator.VariantComparisonHisto(variantResults, _model.ScaledVariantTarget.Values);
                }
            }

        }

    }
}