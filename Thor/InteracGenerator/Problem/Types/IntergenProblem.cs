using System;
using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
//using RDotNet;
using Accord.Statistics.Testing;

using JMetalCSharp.Utils.Wrapper;
using System.IO;
using Accord.Math;
using System.ComponentModel;
using System.Diagnostics;
using InteracGenerator.FitnessCalculation;
using InteracGenerator.Problem;


namespace InteracGenerator.InteractionProblem
{
    public class IntergenProblem : JMetalCSharp.Core.Problem
    {
        //private Distribution target;
       
        public InterGen Model { get; set; }
    
        //Distribution Featurevalues;
        //Distribution interactions;
        //VariabilityModel vm;
        //InfluenceModel ivm;
        
        public double[,] calculated;

        public InterGen.ProbType ProblemType;
        public BackgroundWorker Worker;
        //public List<List<BinaryOption>> FoundInteractions { get; set; }
        //public double[] InteractionValuesPerConfig { get; set; }
        //private List<List<BinaryOption>> allConfigs;
        private int _counter = 0;
        //private int? parallelIndex = 0;

        public Distribution FeatureTarget { get; set; }
        public Distribution InteractionTarget { get; set; }
        public Distribution VariantTarget { get; set; }
        public Distribution ScaledVariantTarget { get; set; }

        public bool UseInitialFv { get; set; }

        public byte [,] FeatureMatrix { get; set; }
        public byte [,] InteractionMatrix { get; set; }

        public IntergenProblem() { }

        public IntergenProblem(InterGen model)
        {

         
            Model = model;
           
            NumberOfVariables = model.Setting.NumberOfFeatures + model.Setting.NumberOfInteractions;
            //ProblemName = "InteractionProblem";
            SolutionType = new RealSolutionType(this);
           
            UpperLimit = new double[NumberOfVariables];
            LowerLimit = new double[NumberOfVariables];

            var featMin = Model.DStore.SelectedFeatureDistribution.Values.Min();
            var featMax = Model.DStore.SelectedFeatureDistribution.Values.Max();
           

            const double featMargin = 0.05;
            double lowInterac = 0;
            double highInterac = 0;

            var lowFeat = featMin < 0 ? featMin * (1 + featMargin) : featMin * (1 - featMargin);
            var highFeat = featMax < 0 ? featMax * (1 - featMargin) : featMax * (1 + featMargin);

            if (model.Setting.NumberOfInteractions > 0)
            {
                const double interacMargin = 0.05;
                var interacMin = Model.DStore.SelectedInteractionDistribution.Values.Min();
                var interacMax = Model.DStore.SelectedInteractionDistribution.Values.Max();
                lowInterac = interacMin < 0 ? interacMin*(1 + interacMargin) : interacMin*(1 - interacMargin);
                highInterac = interacMax < 0 ? interacMax*(1 - interacMargin) : interacMax*(1 + interacMargin);
            }

            for (var index = 0; index < NumberOfVariables; index++)
            {
                if (index < model.Setting.NumberOfFeatures)
                {
                    LowerLimit[index] = lowFeat;
                    UpperLimit[index] = highFeat;
                }
                else
                {
                    LowerLimit[index] = lowInterac;
                    UpperLimit[index] = highInterac;
                }
            }
        }


      

        public InterGen GetModel()
        {
            return Model;
        }


       
        public void LoadScaledVariant()
        {

        }




        public void resetCounter()
        {
            _counter = 0;
        }

        double currentMin = double.MaxValue;
        double currentMax = double.MinValue;

        private long calcTime;
        private long scaleTime;
        private long fitnessTime;


        public override void Evaluate(Solution solution)
        {
#if DEBUG
            if (_counter == 0)
            {
                if (Model.Setting.Logging)
                {
                    LogArrayValues(FeatureTarget.Values, "targetFeatures");
                    if (Model.Setting.NumberOfInteractions > 0) LogArrayValues(InteractionTarget.Values, "targetInteraction");
                }
            }

            _counter++;
#endif
            Console.WriteLine(_counter);
            Distribution variantTarget = null;
            if (!Model.Setting.NoVariantCalculation)
            {

                var localScaledVariants = new double[ScaledVariantTarget.Values.Length];
                Array.Copy(ScaledVariantTarget.Values, localScaledVariants, ScaledVariantTarget.Values.Length);
                variantTarget = new Distribution(localScaledVariants);
            }
            var doubleVal = new double[Model.Setting.NumberOfFeatures];
            var interacVal = new double[Model.Setting.NumberOfInteractions];
            var values = new XReal(solution);
            for (var i = 0; i < Model.Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }

            for (var i = 0; i < Model.Setting.NumberOfInteractions; i++)
            {
                interacVal[i] = values.GetValue(i + Model.Setting.NumberOfFeatures);
            }

            //the distribution from the NSGA2
            var nsgaFv = new Distribution(doubleVal);
            var interacDist = new Distribution(interacVal);

            var watch = Stopwatch.StartNew();
            //calculate the variant values
            Distribution variantResult = null;

            if (_counter > 9500)
            {
                var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                
               
                
                if (Model.Setting.NumberOfInteractions > 0)
                {
                    var interacVals = InteractionMatrix.Dot(interacVal);
                    var variantResults = variantValuesWithoutInteraction.Add(interacVals);
                    variantResult = new Distribution(variantResults);
                }
                else
                {
                    variantResult = new Distribution(variantValuesWithoutInteraction);
                }
            }
            watch.Stop();
            calcTime += watch.ElapsedMilliseconds;

            //scale the variant target distribution to the size of the calculated variant distribution

            var scaleWatch = Stopwatch.StartNew();
            FMScaling fms = new FMScaling(Model);
            if (Model.Setting.ScaleToGlobalMinMax)
            {
                var change = false;
                if (currentMin > variantResult.Values.Min())
                {
                    currentMin = variantResult.Values.Min();
                    change = true;
                }
                if (currentMax < variantResult.Values.Max())
                {
                    currentMax = variantResult.Values.Max();
                    change = true;
                }
                if (change)
                {
                    ScaledVariantTarget = FMScaling.InteractionToScale(ScaledVariantTarget, currentMin, currentMax);
                }

            }
            else {
                if (!Model.Setting.NoVariantCalculation)
                {
                    variantTarget = FMScaling.InteractionToScale(variantTarget, variantResult.Values.Min(),
                        variantResult.Values.Max());
                }
            }

            scaleWatch.Stop();
            scaleTime += scaleWatch.ElapsedMilliseconds;

            IntergenSolution s = (IntergenSolution)solution;
            s.FoundAtEval = _counter;


            var testWatch = Stopwatch.StartNew();
            //calculate the fitness values for features and variants



            var fc = new FitnessCalculator(Model, _counter);
            var fitnessValues = fc.Calculate(variantResult, variantTarget, nsgaFv, FeatureTarget, interacDist, InteractionTarget);
            solution.Objective[0] = fitnessValues.FeatureVal;

            if (Model.Setting.NoVariantCalculation)
            {
                if (Model.Setting.NumberOfInteractions > 0)
                {
                    solution.Objective[1] = fitnessValues.InteracVal;
                }
            }
            else
            {
                //interacs and variants
                if (Model.Setting.NumberOfInteractions > 0)
                {
                    solution.Objective[1] = fitnessValues.InteracVal;
                    solution.Objective[2] = fitnessValues.VariantVal;
                }
                else
                {
                    solution.Objective[1] = fitnessValues.VariantVal;
                }
            }



            /* if (Model.Setting.UseKs)
            {
                PerfomKs(variantResult, variantTarget, nsgaFv, FeatureTarget, solution);
                
            }
            else if (Model.Setting.UseCmv)
            {
                PerformCmv(variantResult, variantTarget, nsgaFv, FeatureTarget, InteractionTarget, interacDist, solution);    
            }
            else if (Model.Setting.UseEuclidean) {
                var bd = new BinnedDistance(variantTarget, variantResult, Model.VariantDynamicHist, _counter);
                var bd2 = new BinnedDistance(nsgaFv, FeatureTarget, Model.FeaturesDynamicHist, _counter);
              
                solution.Objective[0] = bd2.EuclidianDist();
                solution.Objective[1] = bd.EuclidianDist();
                if (Model.Setting.NumberOfInteractions > 0)
                {
                    var interac = new BinnedDistance(interacDist, InteractionTarget, Model.InteracDynamicHist, _counter);
                    solution.Objective[2] = interac.EuclidianDist();
                }
            }
            else if (Model.Setting.UseChiSquared)
            {
                var bd = new BinnedDistance(variantTarget, variantResult, Model.VariantDynamicHist,
                    _counter);
                var bd2 = new BinnedDistance(nsgaFv, FeatureTarget, Model.FeaturesDynamicHist, _counter);
              

                solution.Objective[0] = bd2.ChiSquaredDist();
                solution.Objective[1] = bd.ChiSquaredDist();
                if (Model.Setting.NumberOfInteractions > 0)
                {
                    var interac = new BinnedDistance(interacDist, InteractionTarget, Model.InteracDynamicHist, _counter);
                    solution.Objective[2] = interac.ChiSquaredDist();
                }
            }
            else if (Model.Setting.EuclAndCmv)
            {
                PerformCmv(variantTarget, variantResult, nsgaFv, FeatureTarget, InteractionTarget, interacDist, solution);

                var variant = new BinnedDistance(variantTarget, variantResult, Model.VariantDynamicHist, _counter);
                var feat = new BinnedDistance(nsgaFv, FeatureTarget, Model.FeaturesDynamicHist, _counter);
                var interac = new BinnedDistance(interacDist, InteractionTarget, Model.InteracDynamicHist, _counter);

                solution.Objective[3] = feat.EuclidianDist();
                solution.Objective[4] = variant.EuclidianDist();
                solution.Objective[5] = interac.EuclidianDist();
            }
            else if (Model.Setting.ChiAndCmv)
            {
                PerformCmv(variantTarget, variantResult, nsgaFv, FeatureTarget, InteractionTarget, interacDist, solution);
               var variant = new BinnedDistance(variantTarget, variantResult, Model.VariantDynamicHist,_counter);
                var feat = new BinnedDistance(nsgaFv, FeatureTarget, Model.FeaturesDynamicHist, _counter);
                var interac = new BinnedDistance(interacDist, InteractionTarget, Model.InteracDynamicHist, _counter);
                solution.Objective[3] = feat.ChiSquaredDist();
                solution.Objective[4] = variant.ChiSquaredDist();
                solution.Objective[5] = interac.ChiSquaredDist();
            }
            */
            testWatch.Stop();
            fitnessTime += testWatch.ElapsedMilliseconds;

            //report our progress to the model for GUI
            if (_counter % 200 == 0)
            {

                if (!Model.Setting.Parallel /*|| (Model.Parallel && parallelIndex % 50 == 0) */)
                {

                    if (Model.Setting.DrawDensity && Model.Setting.DrawHistogram)
                    {
                        RIntegrator.FeatureHistAndDens(nsgaFv.Values, FeatureTarget.Values);
                        RIntegrator.VariantHistAndDens(variantResult.Values, variantTarget.Values);
                    }
                    else if (Model.Setting.DrawDensity)
                    {
                        RIntegrator.PlotFeatureTarget(nsgaFv.Values, FeatureTarget.Values, Model.Setting.FeatureAdjust);
                        if (Model.Setting.NoVariantCalculation)
                        {
                            if (_counter > 9500) RIntegrator.PlotVariantTarget(variantResult.Values);
                        }
                        else
                        {
                            RIntegrator.PlotVariantTarget(variantResult.Values, variantTarget.Values);
                        }
                        
                        if (Model.Setting.NumberOfInteractions > 0 ) RIntegrator.PlotInteracTarget(interacDist.Values, InteractionTarget.Values);

                    }
                    else if (Model.Setting.DrawHistogram)
                    {
                        RIntegrator.FeatureComparisonHist(nsgaFv.Values, FeatureTarget.Values);

                        if (Model.Setting.NoVariantCalculation)
                        {
                            RIntegrator.PlotVariantTarget(variantResult.Values);
                        }
                        else
                        {
                            RIntegrator.VariantComparisonHisto(variantResult.Values, variantTarget.Values);
                        }
                    }
                        
                    
                    else
                    {

                    }

                    //ReportProgress(solution);
                }
                ReportProgress(solution, fitnessValues);
                //Worker.ReportProgress((int)(counter * 100 / (double)Model.MaxEvaluations), new UserProgress { VariantP = solution.Objective[1], FeatureP = solution.Objective[0] });
            }

            if (_counter == Model.Setting.MaxEvaluations)
            {
                Console.WriteLine("FitnessTime: " + fitnessTime);
                Console.WriteLine("ScaleTime: " + scaleTime);
                Console.WriteLine("CalcTime: " + calcTime);
            }
            
            var cont = new SolutionContainer
            {
                //TargetVariant = variantTarget,
                Variant = variantResult,
                Features = nsgaFv,
                Interaction = interacDist,
                //TargetFeatures = FeatureTarget,
                FeatureTVal = fitnessValues.FeatureVal,
                InteracTVal = fitnessValues.InteracVal,
                VariantTVal = fitnessValues.VariantVal,
                
                CalcTime = watch.ElapsedMilliseconds,
                FitnessTime = testWatch.ElapsedMilliseconds,
                ScaleTime = scaleWatch.ElapsedMilliseconds,
                TestName = GetUsedTest(),
                FoundAtEval = _counter,
            };
            
        
            //if (_counter > 7000)
          //  {
            //    cont.Write(Model.Setting.LogFolder + "evalstep" + _counter + ".json");
           // }
           
            //cont = null;
        //Console.WriteLine(cont.FoundAtEval + "\t" +  cont.FeatureTVal + "\t" + cont.VariantTVal);

           

            if (Model.Setting.ChiAndCmv || Model.Setting.EuclAndCmv)
            {
                cont.AdditionalFeatureCmv = solution.Objective[4];
                cont.AdditionalVariantCmv = solution.Objective[5];
            }

            //if (!Model.Setting.Parallel)Model.History.Add(counter, cont);
            Model.AddSolutionToHistory(cont);

            if (!Model.Setting.Logging) return;
            var usedTest = GetUsedTest();

            //LogArrayValues(nsgaFV.Values, "Features");
            //LogArrayValues(variantResult.Values, "Variants");
            //LogArrayValues(ScaledVariantTarget.Values, "targetVariants");

            if (Model.Setting.NumberOfInteractions > 0) LogSingleValue(fitnessValues.InteracVal, "InteracFitn" + usedTest);
            if (!Model.Setting.NoVariantCalculation) LogSingleValue(fitnessValues.VariantVal, "VarFitn" + usedTest);
            LogSingleValue(fitnessValues.FeatureVal, "FeatFitn" + usedTest);
        }

        private void ReportProgress(Solution solution, FitnessValues values)
        {
            if (Model.Setting.ChiAndCmv || Model.Setting.EuclAndCmv)
            {
                Worker.ReportProgress(
              (int)(_counter * 100 / (double)Model.Setting.MaxEvaluations),
              new UserProgress
              {
                  FeatureCmV = solution.Objective[0],
                  VariantCmV = solution.Objective[1],
                  InteracCmV = solution.Objective[2],
                  FeatureP = solution.Objective[3],
                  VariantP = solution.Objective[4],
                  EvolutionStep = _counter,
              });
            }
            else
            {
                
                var p = new UserProgress
                {
                    FeatureP = values.FeatureVal,
                    EvolutionStep = _counter,
                };
                if (Model.Setting.NumberOfInteractions > 0) p.InteracP = values.InteracVal;
                if (!Model.Setting.NoVariantCalculation) p.VariantP = values.VariantVal;
                Worker.ReportProgress(
                    (int) (_counter*100/(double) Model.Setting.MaxEvaluations), p);
            }
        }

        /*private void PerformCmv(Distribution variantResult, Distribution scaledVariantTarget, Distribution nsgaFv, Distribution featureTarget, Distribution interacTarget, Distribution interacVals, Solution solution)
        {
            Task<double>[] tasks = {};
            if (Model.Setting.NoVariantCalculation)
            {
                tasks = new Task<double>[Model.Setting.NumberOfInteractions > 0 ? 2 : 1];
            }
            else
            {
                tasks = new Task<double>[Model.Setting.NumberOfInteractions > 0 ? 3 : 2];
            }
           
            
           
            tasks[0] = Task.Factory.StartNew(() =>
            {
                var cmv2 = new CramerVonMises(nsgaFv.Values, featureTarget.Values);
                return  Math.Abs(cmv2.Calculate());
            });

            if (!Model.Setting.NoVariantCalculation)
            {
                tasks[2] = Task.Factory.StartNew(() =>
                {
                    var cmv = new CramerVonMises(variantResult.Values, scaledVariantTarget.Values);
                    return Math.Abs(cmv.Calculate());
                });
            }
            if (Model.Setting.NumberOfInteractions > 0)
            {

                tasks[1] = Task.Factory.StartNew(() =>
                {
                    var cmv3 = new CramerVonMises(interacTarget.Values, interacVals.Values);
                    return  Math.Abs(cmv3.Calculate());
                });
               
            }

            Task.WaitAll(tasks);

            solution.Objective[0] = tasks[0].Result;
            solution.Objective[1] = tasks[1].Result;
            if (Model.Setting.NumberOfInteractions > 0) solution.Objective[2] = tasks[2].Result;
        } */

        private static void PerfomKs(Distribution variantResult, Distribution scaledVariantTarget, Distribution nsgaFv, Distribution featureTarget, Solution solution)
        {
            var variant = new TwoSampleKolmogorovSmirnovTest(variantResult.Values, scaledVariantTarget.Values);
            if (variant.PValue == 0d)
            {
                solution.Objective[0] = double.MaxValue;
            }
            else
            {
                solution.Objective[0] = -variant.PValue;
            }

            var feature = new TwoSampleKolmogorovSmirnovTest(nsgaFv.Values, featureTarget.Values);

            if (feature.PValue == 0d)
            {
                solution.Objective[1] = double.MaxValue;
            }
            else
            {
                solution.Objective[1] = -feature.PValue;
            }
        }

        private string GetUsedTest()
        {
            if (Model.Setting.UseCmv) return "CMV";
            if (Model.Setting.UseKs) return "KS";
            if (Model.Setting.UseChiSquared) return "Chi";
            if (Model.Setting.UseEuclidean) return "Euclid";
            if (Model.Setting.ChiAndCmv) return "ChiSquared+CmV";
            if (Model.Setting.EuclAndCmv) return "Euclidean+CmV";
            return null;
        }

        private void LogArrayValues(double[] vals, string fileName) {
            using (var sw = File.AppendText(Model.Setting.LogFolder + fileName + ".txt"))
            {
                for (var i = 0; i < vals.Length - 1; i++)
                {
                    sw.Write(vals[i] + " ");
                }
                sw.WriteLine(vals[vals.Length - 1]);
            }
        }

        private void LogSingleValue(double val, string fileName) {
            using (StreamWriter sw = File.AppendText(Model.Setting.LogFolder + fileName + ".txt"))
            {
                sw.WriteLine(val);
            }
        }
    }
}
