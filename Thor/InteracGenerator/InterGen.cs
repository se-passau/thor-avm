using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using InteracGenerator.InteractionProblem;
using InteracGenerator.Parser;
using InteracGenerator.Problem;
using JMetalCSharp.Core;
using JMetalCSharp.Operators.Mutation;
using JMetalCSharp.Operators.Selection;
using SPLConqueror_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Accord.Math;
using InteracGenerator.Helper;
using InteracGenerator.InteracWeaving;
using InteracGenerator.Pareto;
using InteracGenerator.Problem.NSGA;
using InteracGenerator.Problem.Types;
using InteracGenerator.VariantGenerators;
using JMetalCSharp.Utils.Wrapper;
using Newtonsoft.Json;

namespace InteracGenerator
{
    public class InterGen : INotifyPropertyChanged
    {

        public List<Distribution> AvailableDistributions;

        public Dictionary<int, SolutionContainer> History;
        public List<SolutionContainer> BestSolutions;
        public SolutionContainer BestSolution;
        public enum ProbType
        {
            Feat, Interac, Variant, FeatInterac, FeatVariant, InteracVariant, Complete, NotImplemented,
        }

        public int RequiredEvaluations { get; set; }

        public Distribution[] CreateUnifDist(int amount, Distribution.DistributionType type)
        {
            switch (type)
            {
                case Distribution.DistributionType.Feature:
                    DStore.ScaledFeatureDistributions = new Distribution[amount];
                    break;
                case Distribution.DistributionType.Interaction:
                    DStore.ScaledInteractionDistributions = new Distribution[amount];
                    break;
                case Distribution.DistributionType.Variant:
                    UniformContinuousDistribution n = new UniformContinuousDistribution(Setting.UnifMin, Setting.UnifMax);
                    var d = new Distribution(n.Generate(500))
                    {
                        Name = "Uniform",
                        DisplayName = "Uniform",
                        DistType = type,
                        SelectedNfProperty = new NFProperty("Random function")
                    };
                    DStore.SelectedTargetDistribution = d;
                    return null;
                default:
                    Console.WriteLine("error no such dist");
                    break;
            }
            for (var i = 0; i < amount; i++)
            {
                UniformContinuousDistribution n = new UniformContinuousDistribution(Setting.UnifMin, Setting.UnifMax);
                double[] values = null;
                if (type == Distribution.DistributionType.Feature)
                {
                    values = n.Generate(Setting.NumberOfFeatures);
                }
                else if (type == Distribution.DistributionType.Interaction)
                {
                    values = n.Generate(Setting.NumberOfInteractions);
                }
                Distribution d = new Distribution(values)
                {
                    Name = "Uniform" + i,
                    ImagePath = "Uniform" + i + ".png",
                    DistType = type,
                    DisplayName = "Uniform",
                    SelectedNfProperty = new NFProperty("Random function")
                };
                RIntegrator.PlotRandomDist(d, Setting.FeatureAdjust);

                if (type == Distribution.DistributionType.Feature)
                {
                    DStore.ScaledFeatureDistributions[i] = d;

                }
                else if (type == Distribution.DistributionType.Interaction)
                {
                    DStore.ScaledInteractionDistributions[i] = d;
                }
            }

            switch (type)
            {
                case Distribution.DistributionType.Feature:
                    return DStore.ScaledFeatureDistributions;
                case Distribution.DistributionType.Interaction:
                    return DStore.ScaledInteractionDistributions;
                default:
                    //TODO
                    return null;
            }
        }

        public void ShowParetoSolution(double w1, double w2 = 0, double w3 = 0)
        {
            if (_population != null)
            {
                var ps = new ParetoSolver(_population);
                var sol = ps.GetBest(w1, w2, w3);
                var index = ps.GetNumber();
                CalculateSolution((IntergenSolution)sol, index, true);

            }
        }

        public void Draw3DPareto()
        {
            if (_population != null)
            {
                var size = _population.GetObjectives().Length;
                var ocount = _population.GetObjectives()[0].Length;
                var objectives = _population.GetObjectives();

                var f1 = new double[size];
                var f2 = new double[size];
                var f3 = new double[] { };
                if (ocount == 3) f3 = new double[size];

                for (var i = 0; i < size; i++)
                {
                    f1[i] = objectives[i][0];
                    if (ocount > 1) f2[i] = objectives[i][1];
                    if (ocount > 2) f3[i] = objectives[i][2];
                }
                var f1min = f1.Min();
                var f1max = f1.Max();
                var f2min = f2.Min();
                var f2max = f2.Max();

                double f3min = 0;
                double f3max = 0;

                if (ocount == 3)
                {
                    f3min = f3.Min();
                    f3max = f3.Max();
                }

                var _f1Scaled = new double[50];
                var _f2Scaled = new double[50];
                var _f3Scaled = new double[50];

                for (var i = 0; i < size; i++)
                {
                    _f1Scaled[i] = FMScaling.ScaleFromTo(f1min, f1max, 0, 1, f1[i]);
                    if (ocount > 1) _f2Scaled[i] = FMScaling.ScaleFromTo(f2min, f2max, 0, 1, f2[i]);
                    if (ocount > 2) _f3Scaled[i] = FMScaling.ScaleFromTo(f3min, f3max, 0, 1, f3[i]);
                }
                if (ocount > 1) RIntegrator.Draw3dPareto(_f1Scaled, _f2Scaled, _f3Scaled, Setting.DrawAngle);
            }
        }

        public void CalculateSolution(IntergenSolution sol, int index, bool draw)
        {
            var featVals = GetFeatureValues(sol);
            var interacVals = GetInteractionValues(sol);

            var variant = FeatureMatrix.Dot(featVals);
            if (ProbHasInterac(ProblemType))
            {
                var interacVars = InteractionMatrix.Dot(interacVals);
                variant = variant.Add(interacVars);
            }
            Distribution localVariantTarget = null;
            if (ScaledVariantTarget != null)
            {

                var localScaledVariants = new double[ScaledVariantTarget.Values.Length];
                Array.Copy(ScaledVariantTarget.Values, localScaledVariants, ScaledVariantTarget.Values.Length);
                localVariantTarget = new Distribution(localScaledVariants);
                localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variant.Min(),
                    variant.Max());
            }
            BestSolution = new SolutionContainer
            {
                Features = new Distribution(featVals),
                Interaction = new Distribution(interacVals),
                Variant = new Distribution(variant)
            };


            if (Setting.NumberOfInteractions > 0) { 

            //if (ProbHasInterac(ProblemType))
            //{
                RIntegrator.PlotSolution(featVals, DStore.SelectedFeatureDistribution.Values, interacVals,
                    DStore.SelectedInteractionDistribution.Values, variant, localVariantTarget, index);
            }
            else
            {
                if (Setting.VariantFitness)
                {
                    RIntegrator.PlotSolution(featVals, DStore.SelectedFeatureDistribution.Values, variant,
                        localVariantTarget, index);
                }
                else
                {
                    RIntegrator.PlotSolution(featVals, DStore.SelectedFeatureDistribution.Values, variant,
                       null, index);
                }
            }

        }


        /*public string[] DrawSolution(SolutionContainer sc, int index)
        {
            var imagepaths = new string[2];
            //imagepaths[1] = RIntegrator.PlotVariantTarget(sc.Variant.Values, sc.TargetVariant.Values, index);
            //imagepaths[0] = RIntegrator.PlotFeatureTarget(sc.Features.Values, sc.TargetFeatures.Values, index);
            return imagepaths;
        } */

        public DynamicHist FeaturesDynamicHist { get; set; }
        public DynamicHist VariantDynamicHist { get; set; }

        public Distribution[] CreateNormalDist(int amount, Distribution.DistributionType type)
        {
            if (double.IsNaN(Setting.Mean) || double.IsNaN(Setting.StandardDeviation)) return null;


            switch (type)
            {
                case Distribution.DistributionType.Variant:
                    UnivariateContinuousDistribution n = new NormalDistribution(Setting.Mean, Setting.StandardDeviation);
                    Distribution d = new Distribution(n.Generate(500));
                    d.Name = "NormalTarget";
                    d.DistType = type;
                    d.DisplayName = "Normal";
                    d.SelectedNfProperty = new NFProperty("Random function");
                    DStore.SelectedTargetDistribution = d;
                    return null;
                case Distribution.DistributionType.Feature:
                    DStore.ScaledFeatureDistributions = new Distribution[amount];
                    break;
                case Distribution.DistributionType.Interaction:
                    DStore.ScaledInteractionDistributions = new Distribution[amount];
                    break;
                default:
                    Console.WriteLine("Error: No such distribution type");
                    break;
            }

            for (int i = 0; i < amount; i++)
            {
                
                var n = new NormalDistribution(Setting.Mean, Setting.StandardDeviation);
               
                double[] values = null;
                switch (type)
                {
                    case Distribution.DistributionType.Feature:
                        if(Setting.FeatureScaleMin == 0 && Setting.FeatureScaleMax == 0)
                            values = n.Generate(Setting.NumberOfFeatures);
                        else
                            values = RIntegrator.GenerateNormalDistribution(Setting.FeatureScaleMin, Setting.FeatureScaleMax, Setting.NumberOfFeatures);
                        
                        break;
                    case Distribution.DistributionType.Interaction:
                        if (Setting.FeatureScaleMin == 0 && Setting.FeatureScaleMax == 0)
                            values = n.Generate(Setting.NumberOfInteractions);
                        else
                            values = RIntegrator.GenerateNormalDistribution(Setting.FeatureScaleMin, Setting.FeatureScaleMax, Setting.NumberOfInteractions);
                        
                        break;
                }
                var d = new Distribution(values)
                {
                    Name = "Normal" + i,
                    ImagePath =  "Normal" + i + ".png",
                    DistType = type,
                    DisplayName = "Normal",
                    SelectedNfProperty = new NFProperty("Random function")
                };
                RIntegrator.PlotRandomDist(d, Setting.FeatureAdjust);
                switch (type)
                {
                    case Distribution.DistributionType.Feature:
                        DStore.ScaledFeatureDistributions[i] = d;
                        break;
                    case Distribution.DistributionType.Interaction:
                        DStore.ScaledInteractionDistributions[i] = d;
                        break;
                }
            }
            switch (type)
            {
                case Distribution.DistributionType.Feature:
                    return DStore.ScaledFeatureDistributions;
                case Distribution.DistributionType.Interaction:
                    return DStore.ScaledInteractionDistributions;
                default:
                    return null;
            }
        }

        public SolutionSet Solutions;
        private double[] _featureValues;

        public double[] FeatureValues
        {
            get { return _featureValues; }
            set
            {
                RIntegrator.SetFeatureValues(FeatureName, value);
                _featureValues = value;
            }
        }

        public string FeatureName { get; set; }
        public string InteractionName { get; set; }

        public double[] ScaledFeatureValues { get; set; }

        private int _progress;

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                ProgressPercent = _progress / (double)Setting.NumberOfInteractions * 100.0f;
            }
        }

        private double _pp;

        public double ProgressPercent
        {
            get { return _pp; }
            set
            {
                _pp = value;
                NotifyPropertyChanged();
            }
        }


        private double[] _interactionValues;
        public double[] InteractionValues
        {
            get { return _interactionValues; }
            set
            {
                RIntegrator.SetFeatureValues(InteractionName, value);
                _interactionValues = value;
            }
        }

        public double[] ScaledInteractionValues { get; set; }

        private double _fp1;
        private double _fp2;
        private double _ip1;
        private double _ip2;

        public double FPVal1
        {
            get { return _fp1; }
            set
            {
                _fp1 = value;
                NotifyPropertyChanged();
            }
        }

        public double FPVal2
        {
            get { return _fp2; }
            set
            {
                _fp2 = value;
                NotifyPropertyChanged();
            }
        }

        public double IPVal1
        {
            get { return _ip1; }
            set
            {
                _ip1 = value;
                NotifyPropertyChanged();
            }
        }

        public double IPVal2
        {
            get { return _ip2; }
            set
            {
                _ip2 = value;
                NotifyPropertyChanged();
            }
        }

        private string currentBestImage;

        public string CurrentBestImage
        {
            get { return currentBestImage; }
            set
            {
                currentBestImage = value;
                NotifyPropertyChanged();
            }
        }


        private int _tries;
        public int Tries
        {
            get { return _tries; }
            set
            {
                _tries = value;
                NotifyPropertyChanged();
            }
        }

        public Settings Setting;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private List<List<BinaryOption>> FoundInteractions;

        public VariabilityModel Vm;
        public string FeatureTestMethod;
        private double[] interactionValuesPerVariant;

        public DistStore DStore;

        public InterGen()
        {
            Setting = new Settings();
            DStore = new DistStore(this);
            AvailableDistributions = new List<Distribution>();
            History = new Dictionary<int, SolutionContainer>();
            BestSolutions = new List<SolutionContainer>();
            FeatureTestMethod = "KS-Test";
            RIntegrator.Init(this);
            SolutionPlotter.SetModel(this);
            ProgressReporter.Init(this);
            FitnessTracker.Init(this);
            FeaturesDynamicHist = new DynamicHist(this) { UseSquareRoot = true };
            VariantDynamicHist = new DynamicHist(this) { UseSquareRoot = true };
            InteracDynamicHist = new DynamicHist(this) { UseSquareRoot = true };
        }

        public Distribution[] ScaleDistribution(Distribution dist, int size, int amount = 2)
        {
            if (dist.DistType == Distribution.DistributionType.Feature)
            {
                DStore.ScaledFeatureDistributions = new Distribution[amount];
            }
            else
            {
                DStore.ScaledInteractionDistributions = new Distribution[amount];

            }
            for (var i = 0; i < amount; i++)
            {
                var result = RIntegrator.BootStrapValues(dist, size);
                result.ImagePath = RIntegrator.PlotOrigAndFitted(dist, result, i, Setting.FeatureAdjust);

                if (dist.DistType == Distribution.DistributionType.Feature)
                {
                    if (i == 0) FPVal1 = KSmirnoffTest(dist, result);
                    if (i == 1) FPVal2 = KSmirnoffTest(dist, result);
                    result.DisplayName = dist.DisplayName;
                    DStore.ScaledFeatureDistributions[i] = result;

                }
                else
                {
                    if (i == 0) IPVal1 = KSmirnoffTest(dist, result);
                    if (i == 1) IPVal2 = KSmirnoffTest(dist, result);
                    result.DisplayName = dist.DisplayName;
                    DStore.ScaledInteractionDistributions[i] = result;
                }
            }

            return dist.DistType == Distribution.DistributionType.Feature ? DStore.ScaledFeatureDistributions : DStore.ScaledInteractionDistributions;
        }

        public double KSmirnoffTest(Distribution first, Distribution second)
        {
            if (first.Values.Length > 0 && second.Values.Length > 0)
            {
                TwoSampleKolmogorovSmirnovTest test = new TwoSampleKolmogorovSmirnovTest(first.Values, second.Values);
                return test.PValue;
            }
            return 0;
        }

        public double KSmirnoffTest(double[] first, double[] second)
        {
            TwoSampleKolmogorovSmirnovTest test = new TwoSampleKolmogorovSmirnovTest(first, second);
            return test.PValue;
        }

        /// <summary>
        /// Tries to parse an input variability model
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFM(string fileName)
        {

            Setting.FileName = fileName;
            if (fileName.EndsWith(".dimacs") || File.ReadAllLines(fileName)[0].StartsWith("c "))
            {
                //Vm = new DimacsParser(fileName).Parse();
                var parser = new DimacsParser(fileName);
                Setting.NumberOfFeatures = parser.Count;
                Setting.DimacsFeatureList = parser.FeatureList;
                //Setting.Dimacs = true;

                Vm = parser.Parse();
            }
            else if (fileName.EndsWith(".afm"))
            {
                Vm = new BettyFileParser().ReadConfiguration(File.ReadAllLines(fileName));
                Setting.NumberOfFeatures = Vm.BinaryOptions.Count;
            }
            else if (fileName.EndsWith(".xml") && File.ReadAllLines(fileName)[0].StartsWith("<vm name="))
            {
                Vm = VariabilityModel.loadFromXML(fileName);
                Setting.NumberOfFeatures = Vm.BinaryOptions.Count;
            }
            else
            {
                Vm = new SplotParser(fileName).Parse();
                Setting.NumberOfFeatures = Vm.BinaryOptions.Count;
            }
        }

        public Distribution ScaledVariantTarget { get; set; }
        public byte[,] FeatureMatrix { get; set; }
        public byte[,] InteractionMatrix { get; set; }

        public List<List<string>> FoundDimacsInteractions { get; set; }
        public ProbType ProblemType { get; set; }
        public ProbType GetProbType()
        {

            var interac = Setting.NumberOfInteractions > 0;

            if (Setting.FeatureFitness && !interac && !Setting.VariantFitness) return ProbType.Feat;
            if (Setting.FeatureFitness && interac && !Setting.VariantFitness) return ProbType.FeatInterac;
            if (Setting.FeatureFitness && interac && Setting.VariantFitness) return ProbType.Complete;
            if (!Setting.FeatureFitness && !Setting.InteracFitness && Setting.VariantFitness) return ProbType.Variant;
            if (Setting.FeatureFitness && !interac && Setting.VariantFitness) return ProbType.FeatVariant;
            if (!Setting.FeatureFitness && interac && !Setting.VariantFitness) return ProbType.Interac;
            return ProbType.NotImplemented;
        }

        public bool ProbHasInterac(ProbType type)
        {
            return type == ProbType.Interac || type == ProbType.Complete || type == ProbType.FeatInterac ||
                   type == ProbType.InteracVariant;
        }

        public bool ProbHasVariant(ProbType type)
        {
            
            return type == ProbType.Complete || type == ProbType.FeatVariant || type == ProbType.InteracVariant ||
                    type == ProbType.Variant || !Setting.NoVariantCalculation;
        }

        /// <summary>
        /// Adds interactions to the influence model.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="worker"></param>
        public void CreateInteractions(DoWorkEventArgs e, BackgroundWorker worker)
        {

            if (Setting.NumberOfInteractions == 0) return;

            var features = DStore.SelectedFeatureDistribution;
            var interactions = DStore.SelectedInteractionDistribution;


            var weaver = new BinaryOptionWeaver(this);
            weaver.SetVariabilityModel(Vm);
            weaver.WeaveInteractions(Vm.BinaryOptions, features, interactions, worker);
            FoundInteractions = weaver.GetInteractions();



            /*var fms = new FMScaling(this);

            if (Setting.Dimacs)
            {
                fms.CreateFromDimacs(Setting.DimacsFeatureList, File.ReadAllLines(FileName) , features, interactions, worker);
                FoundDimacsInteractions = fms.GetAddedInteraction();
            }
            else
            {
                fms.CreateFromVarModel(Vm, features, interactions, worker);
            }
          
            FoundInteractions = fms.GetAddedInteractions();
            
            */

        }


        public void WriteFoundDimacsInteractions(string fileName = "foundInteractions.txt")
        {

            File.WriteAllText(fileName, String.Empty);

            using (var sw = File.AppendText(fileName))
            {
                for (int i = 0; i < FoundDimacsInteractions.Count; i++)
                {
                    var list = FoundDimacsInteractions[i];
                    for (int j = 0; j < FoundDimacsInteractions[i].Count - 1; j++)
                    {
                        sw.Write(list[j] + " * ");
                    }
                    sw.WriteLine(list[list.Count - 1] + " * " + DStore.SelectedInteractionDistribution.Values[i]);
                }
            }
        }

        public void LoadSetting(string fileName = "setting.json")
        {
            var file = File.ReadAllText(fileName);
            var sett = JsonConvert.DeserializeObject<Settings>(file);
            Setting = sett;
        }

        public void CreateVariants(BackgroundWorker worker, DoWorkEventArgs e)
        {

            var heuristiclist = CreateHeuristicSettings();


            var generator = new VariantGenerator(Vm);
            var gen = new Stopwatch();
            gen.Start();
            var generatedVariants = generator.GenerateVariants(heuristiclist, worker); //Generate(worker);
            gen.Stop();

            Console.WriteLine("Variants: " + generatedVariants.Count + " Time: " + gen.ElapsedMilliseconds);
            var variantCalculator = new VariantAttributeCalculator();

            var mat = new Stopwatch(); mat.Start();
            FeatureMatrix = variantCalculator.CalculateVariantMatrix(generatedVariants, Vm.BinaryOptions);
            mat.Stop();
            Console.WriteLine("Matrix Calculation: " + mat.ElapsedMilliseconds);
            if (Setting.NumberOfInteractions != 0)
            {

                InteractionMatrix = variantCalculator.GetInteractionMatrix(generatedVariants, FoundInteractions);
                //interactionValuesPerVariant = variantCalculator.CalculateInteractionSums(generatedVariants,
                //    FoundInteractions, DStore.SelectedInteractionDistribution.Values);
            }
           
        }

        public IntergenProblem GetProblem(ProbType type)
        {
            IntergenProblem p = null;
            if (Setting.FeatureScaleMin != 0 || Setting.FeatureScaleMax != 0)
            {
                DStore.SelectedFeatureDistribution = FMScaling.InteractionToScale(DStore.SelectedFeatureDistribution, Setting.FeatureScaleMin,
                     Setting.FeatureScaleMax);
            }
            if (Setting.InteractionScaleMin != 0 || Setting.InteractionScaleMax != 0)
            {
                DStore.SelectedInteractionDistribution = FMScaling.InteractionToScale(DStore.SelectedInteractionDistribution, Setting.InteractionScaleMin,
                Setting.InteractionScaleMax);
            }
            switch (type)
            {
                case ProbType.FeatInterac:
                    p = new FeatAndInteractionProblem(this);
                    p.ProblemType = ProbType.FeatInterac;

                    break;
                case ProbType.Feat:
                    p = new FeatProblem(this);
                    p.ProblemType = ProbType.Feat;
                    break;
                case ProbType.Variant:
                    p = new VariantProblem(this);
                    p.InteractionMatrix = InteractionMatrix;
                    p.InteractionTarget = DStore.SelectedInteractionDistribution;
                    p.ProblemType = ProbType.Variant;
                    break;
                case ProbType.Interac:
                    p = new Problem.Types.InteractionProblem(this);
                    p.ProblemType = ProbType.Interac;
                    break;
                case ProbType.Complete:
                    p = new CompleteIntergenProblem(this);
                    p.ProblemType = ProbType.Complete;
                    break;
                case ProbType.FeatVariant:
                    p = new FeatVariantProblem(this);
                    p.ProblemType = ProbType.FeatVariant;
                    break;
                default:

                    break;
            }

            Setting.FeatTarget = DStore.SelectedFeatureDistribution;
            Setting.FeatureMatrix = FeatureMatrix;
            p.FeatureMatrix = FeatureMatrix;
            p.FeatureTarget = DStore.SelectedFeatureDistribution;
            p.UseInitialFv = Setting.UseInitialFv;

            if (ProbHasInterac(type))
            {
                if (Setting.InteractionScaleMin != 0 || Setting.InteractionScaleMax != 0)
                {
                    DStore.SelectedInteractionDistribution = FMScaling.InteractionToScale(DStore.SelectedInteractionDistribution, Setting.InteractionScaleMin,
                    Setting.InteractionScaleMax);
                }
                Setting.InteracTarget = DStore.SelectedInteractionDistribution;
                Setting.InteracMatrix = InteractionMatrix;
                p.InteractionTarget = DStore.SelectedInteractionDistribution;
                p.InteractionMatrix = InteractionMatrix;
            }
            if (ProbHasVariant(type))
            {

                ScaledVariantTarget = RIntegrator.BootStrapValues(DStore.SelectedTargetDistribution, FeatureMatrix.GetUpperBound(0) + 1);
                Setting.VariantTarget = ScaledVariantTarget;
                p.VariantTarget = ScaledVariantTarget;
            }
            return p;

            //p.resetCounter();

        }


        public double[] GetFeatureValues(IntergenSolution sol)
        {
            var doubleVal = new double[Setting.NumberOfFeatures];
            var values = new XReal(sol);
            for (var i = 0; i < Setting.NumberOfFeatures; i++)
            {
                doubleVal[i] = values.GetValue(i);
            }
            return doubleVal;
        }

        public double[] GetInteractionValues(IntergenSolution sol)
        {
            if (Setting.NumberOfInteractions == 0) return null;
            var interacVal = new double[Setting.NumberOfInteractions];
            var values = new XReal(sol);
            for (var i = 0; i < Setting.NumberOfInteractions; i++)
            {
                interacVal[i] = values.GetValue(i + Setting.NumberOfFeatures);
            }
            return interacVal;
        }


        public void StartEvolution(BackgroundWorker worker, DoWorkEventArgs e)
        {
            History.Clear();
            FitnessTracker.Reset();

            //Setting.LogFolder = Path.GetDirectoryName(Setting.LogFolder) + Path.DirectorySeparatorChar;

            ProblemType = GetProbType();
            var p = GetProblem(ProblemType);

            Dictionary<string, object> parameters;

            Algorithm algorithm = null; // The algorithm to use
            Operator crossover; // Crossover operator
            Operator mutation; // Mutation operator
            Operator selection;


            if (Setting.Parallel)
            {
                //Console.WriteLine("use parallel eval");
                algorithm = new PNSGAII(p, worker, new MyMultiThreadedEvaluator());
            }
            else
            {
                algorithm = new NSGA2(p, worker);
            }

            //algorithm = new JMetalCSharp.Metaheuristics.SPEA2.SPEA2(p);
            // Algorithm parameters
            algorithm.SetInputParameter("populationSize", Setting.PopulationSize);
            algorithm.SetInputParameter("maxEvaluations", Setting.MaxEvaluations);

            // Mutation and Crossover for Real codification 
            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 0.9);
            parameters.Add("distributionIndex", 10.0);
            crossover = IntergenCrossoverFactory.GetCrossoverOperator("IntergenCrossover", parameters);

            parameters = new Dictionary<string, object>();
            parameters.Add("probability", 1.0 / p.NumberOfVariables);
            parameters.Add("distributionIndex", 20.0);
            mutation = MutationFactory.GetMutationOperator("PolynomialMutation", parameters);

            // Selection Operator 
            parameters = null;
            selection = SelectionFactory.GetSelectionOperator("BinaryTournament2", parameters);

            // Add the operators to the algorithm
            algorithm.AddOperator("crossover", crossover);
            algorithm.AddOperator("mutation", mutation);
            algorithm.AddOperator("selection", selection);
            var evotime = new Stopwatch();
            evotime.Start();

            var pr = Process.GetCurrentProcess();
            var startUserProcessorTm = pr.UserProcessorTime.TotalMilliseconds;


            _population = algorithm.Execute();
            evotime.Stop();

            var endUserProcessorTm = pr.UserProcessorTime.TotalMilliseconds;
            var cputime = endUserProcessorTm - startUserProcessorTm;


            //Console.WriteLine("Fitnesstime: " + FitnessTracker.FitnessTime);
            //Console.WriteLine("Multiplication: " + FitnessTracker.MultiplicationTime);
            //Console.WriteLine("Evolutiontime: " + evotime.ElapsedMilliseconds + " CpuTime: " + cputime);
            //Solutions = population;
            //var best = population.IndexBest(new FitnessComparator());
            //Console.WriteLine("Best Solution: " + best);
            //using (var sw = File.AppendText(Setting.LogFolder + "solution" + ".txt"))
            // {
            FitnessTracker.WriteFiles(Setting.LogFolder, fileName: "Fitn");
            if (Setting.Logging)
            {

                WriteFiles(_population, ProblemType);
                WriteTargets();
                //WriteMatrix();
            }
            RequiredEvaluations = (int) algorithm.GetOutputParameter("evaluations");
           
            var ps = new ParetoSolver(_population);
            var best = ps.GetBest(0.25, 0.25, 0.5);

            /*if (!Setting.Parallel)
                {
                    //var sc = History[sol.FoundAtEval];
                    var sc =
                        JsonConvert.DeserializeObject<SolutionContainer>(File.ReadAllText(Setting.LogFolder + "evalstep" +
                                                                         sol.FoundAtEval + ".json"));
                    sc.DisplayName = "EvolutionStep: " + sol.FoundAtEval;
                    BestSolutions.Add(sc);
                }
                else
                {
                    //Console.WriteLine(sol.FoundAtEval);
                    foreach (var t in parallelHistory)
                    {
                        lock (locker)
                        {
                            if (t.FoundAtEval != sol.FoundAtEval) continue;
                            Console.WriteLine("Hit " + t.FoundAtEval);
                            BestSolutions.Add(t);
                        }
                    }
                }
                // sw.Write(sol.FoundAtEval + ": ");
                // for (int i = 0; i < sol.Variable.Length - 1; i++) {
                //     sw.Write(sol.Variable[i] + ", ");
                // }
                // sw.WriteLine(sol.Variable[sol.Variable.Length - 1]);
            }
            //}

            //WriteBestSolution(population.Get(best));
            //if (Setting.LogFolder != null)
            //{
              //  WriteSolutions(Path.GetDirectoryName(Setting.LogFolder));
            //} */
           // Console.WriteLine("Done");
        }



        public void WriteMatrix()
        {
            var fileName = Path.GetDirectoryName(Setting.LogFolder) + "featMatrix.txt";
            var bound0 = FeatureMatrix.GetUpperBound(0);
            var bound1 = FeatureMatrix.GetUpperBound(1);
            File.WriteAllText(fileName, string.Empty);
            for (var i = 0; i <= bound0; i++)
            {
                for (var j = 0; j <= bound1; j++)
                {
                    File.AppendAllText(fileName, FeatureMatrix[i, j].ToString());
                }
                File.AppendAllText(fileName, Environment.NewLine);
            }

            if (Setting.NumberOfInteractions == 0) return;

            fileName = Path.GetDirectoryName(Setting.LogFolder) + "interacMatrix.txt";
            bound0 = InteractionMatrix.GetUpperBound(0);
            bound1 = InteractionMatrix.GetUpperBound(1);
            File.WriteAllText(fileName, string.Empty);
            for (var i = 0; i <= bound0; i++)
            {
                for (var j = 0; j <= bound1; j++)
                {
                    File.AppendAllText(fileName, InteractionMatrix[i, j].ToString());
                }
                File.AppendAllText(fileName, Environment.NewLine);
            }
        }

        public void WriteSolution(IntergenSolution sol, int index, ProbType type)
        {

            var sc = new SolutionContainer();

            var doubleVal = GetFeatureValues(sol);
            var interacVal = GetInteractionValues(sol);

            File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "FeatSolution" + index + ".txt", doubleVal.Select(d => d.ToString()).ToArray());
            sc.Features = new Distribution(doubleVal) { DisplayName = "FeatSolution" + index, DistType = Distribution.DistributionType.Feature, Name = "FeatSol" };
            sc.FeatureTVal = sol.Objective[0];
            sc.FoundAtEval = sol.FoundAtEval;
            sc.TestName = "CmV";



            if (type == ProbType.Feat)
            {
                var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantSolution" + index + ".txt", variantValuesWithoutInteraction.Select(d => d.ToString()).ToArray());
                sc.Variant = new Distribution(variantValuesWithoutInteraction) { DisplayName = "VariantSolution" + index, DistType = Distribution.DistributionType.Variant };
                sc.Write(Setting.LogFolder + Path.DirectorySeparatorChar + "solution" + index + ".json");
                return;
            }

            if (type == ProbType.Variant)
            {
                sc.FeatureTVal = 0;
                sc.VariantTVal = sol.Objective[0];

                if (Setting.NumberOfInteractions > 0)
                {

                    var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                    var interacVals = InteractionMatrix.Dot(interacVal);
                    File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "InteracSolution" + index + ".txt",
                        interacVal.Select(d => d.ToString()).ToArray());
                    var variantResults = variantValuesWithoutInteraction.Add(interacVals);
                    sc.Interaction = new Distribution(interacVal)
                    {
                        DisplayName = "InteracSolution" + index,
                        DistType = Distribution.DistributionType.Interaction
                    };
                    sc.Variant = new Distribution(variantResults)
                    {
                        DisplayName = "VariantSolution" + index,
                        DistType = Distribution.DistributionType.Variant
                    };

                    var fms = new FMScaling(this);
                    var localScaledVariants = new double[ScaledVariantTarget.Values.Length];
                    Array.Copy(ScaledVariantTarget.Values, localScaledVariants, ScaledVariantTarget.Values.Length);
                    var localVariantTarget = new Distribution(localScaledVariants);
                    localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantValuesWithoutInteraction.Min(),
                        variantValuesWithoutInteraction.Max());
                    File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantTarget" + index + ".txt",
                        localVariantTarget.Values.Select(d => d.ToString()).ToArray());
                    sc.TargetVariant = localVariantTarget;

                    File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantSolution" + index + ".txt",
                        variantResults.Select(d => d.ToString()).ToArray());
                    WriteInfluenceModels(index, sc.Features, sc.Interaction, Setting.LogFolder);
                }
                else
                {
                    var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                    var variantResult = new Distribution(variantValuesWithoutInteraction);

                    var fms = new FMScaling(this);
                    var localScaledVariants = new double[ScaledVariantTarget.Values.Length];
                    Array.Copy(ScaledVariantTarget.Values, localScaledVariants, ScaledVariantTarget.Values.Length);
                    var localVariantTarget = new Distribution(localScaledVariants);
                    localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantValuesWithoutInteraction.Min(),
                        variantValuesWithoutInteraction.Max());
                    File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantTarget" + index + ".txt",
                        localVariantTarget.Values.Select(d => d.ToString()).ToArray());
                    sc.TargetVariant = localVariantTarget;
                    sc.Variant = variantResult;
                    variantResult.DisplayName = "VariantSolution" + index;
                    variantResult.DistType = Distribution.DistributionType.Variant;
                    File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantSolution" + index + ".txt",
                    variantResult.Values.Select(d => d.ToString()).ToArray());
                }
                sc.Write(Setting.LogFolder + Path.DirectorySeparatorChar + "solution" + index + ".json");
                return;
            }

            if (ProbHasInterac(type))
            {
                interacVal = GetInteractionValues(sol);
                File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "InteracSolution" + index + ".txt", interacVal.Select(d => d.ToString()).ToArray());
                sc.InteracTVal = sol.Objective[1];

                var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                var interacVals = InteractionMatrix.Dot(interacVal);
                var variantResults = variantValuesWithoutInteraction.Add(interacVals);
                sc.Interaction = new Distribution(interacVal) { DisplayName = "InteracSolution" + index, DistType = Distribution.DistributionType.Interaction };
                sc.Variant = new Distribution(variantResults) { DisplayName = "VariantSolution" + index, DistType = Distribution.DistributionType.Variant };
                File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantSolution" + index + ".txt", variantResults.Select(d => d.ToString()).ToArray());
                WriteInfluenceModels(index, sc.Features, sc.Interaction, Setting.LogFolder);
            }
            if (ProbHasVariant(type))
            {
                var variantValuesWithoutInteraction = FeatureMatrix.Dot(doubleVal);
                var interacVals = InteractionMatrix.Dot(interacVal);
                var variantResults = variantValuesWithoutInteraction.Add(interacVals);
                var variantResult = new Distribution(variantResults);

                var fms = new FMScaling(this);
                var localScaledVariants = new double[ScaledVariantTarget.Values.Length];
                Array.Copy(ScaledVariantTarget.Values, localScaledVariants, ScaledVariantTarget.Values.Length);
                var localVariantTarget = new Distribution(localScaledVariants);
                localVariantTarget = FMScaling.InteractionToScale(localVariantTarget, variantResults.Min(),
                    variantResults.Max());
                File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantTarget" + index + ".txt",
                    localVariantTarget.Values.Select(d => d.ToString()).ToArray());
                sc.TargetVariant = localVariantTarget;
                sc.Variant = variantResult;
                variantResult.DisplayName = "VariantSolution" + index;
                variantResult.DistType = Distribution.DistributionType.Variant;
                File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "VariantSolution" + index + ".txt",
                variantResult.Values.Select(d => d.ToString()).ToArray());
                sc.VariantTVal = sol.Objective[2];
            }
            sc.Write(Setting.LogFolder + Path.DirectorySeparatorChar + "solution" + index + ".json");
            sc = null;
        }



        public void WriteFiles(SolutionSet population, ProbType type)
        {
            //Setting.WriteSetting(Path.GetDirectoryName(Setting.LogFolder) + Path.DirectorySeparatorChar + "setting.json");
            for (var i = 0; i < population.Size(); i++)
            {
                var sol = (IntergenSolution)population.Get(i);
                WriteSolution(sol, i, type);
            }
        }

        public void WriteTargets()
        {
            File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "TargetFeatureValues.txt", DStore.SelectedFeatureDistribution.Values.Select(d => d.ToString()).ToArray());
            if (Setting.NumberOfInteractions > 0) File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "TargetInteractionValues.txt", DStore.SelectedInteractionDistribution.Values.Select(d => d.ToString()).ToArray());
            if (!Setting.NoVariantCalculation && ScaledVariantTarget != null) File.WriteAllLines(Setting.LogFolder + Path.DirectorySeparatorChar + "TargetVariantValues.txt", ScaledVariantTarget.Values.Select(d => d.ToString()).ToArray());
        }





        /* public void WriteSolutions(string targetDirectory)
         {
             var counter = 0;

             foreach (var solution in BestSolutions)
             {
                 var result = JsonConvert.SerializeObject(solution, Formatting.Indented);
                 File.WriteAllText(targetDirectory + Path.DirectorySeparatorChar + "solution" + counter + ".json", result);

                 counter++;
             }
         } */

        private List<HeuristicOption> CreateHeuristicSettings()
        {
            var heuristiclist = new List<HeuristicOption>();
            if (Setting.UseFw) heuristiclist.Add(new HeuristicOption { Method = VariantGenerator.Method.FeatureWise, TimeLimitSeconds = Setting.FwSeconds });
            if (Setting.UsePw) heuristiclist.Add(new HeuristicOption { Method = VariantGenerator.Method.Pairwise, TimeLimitSeconds = Setting.PwSeconds });
            if (Setting.UseNfw) heuristiclist.Add(new HeuristicOption { Method = VariantGenerator.Method.NegativeFeatureWise, TimeLimitSeconds = Setting.NfwSeconds });
            if (Setting.UseRnd)
                heuristiclist.Add(new HeuristicOption
                {
                    Method = VariantGenerator.Method.Random,
                    TimeLimitSeconds = Setting.RndSeconds,
                    Treshold = Setting.RndTreshold,
                    Modulo = Setting.RndModulo
                });
            if (Setting.UsePseudoRnd)
                heuristiclist.Add(new HeuristicOption
                {
                    Method = VariantGenerator.Method.PseudoRandom,
                    Treshold = Setting.PseudoRndSize
                });
            if (Setting.LinearRandom)
                heuristiclist.Add(new HeuristicOption
                {
                    Method = VariantGenerator.Method.LinearRandom,
                    Treshold = Setting.LinearRndSize,
                });
            if (Setting.QuadraticRandom)
            {
                heuristiclist.Add(new HeuristicOption
                {
                    Method = VariantGenerator.Method.QuadraticRandom,
                    Scale = Setting.QuadraticRandomScale,
                    Treshold = 1,
                });
            }
            return heuristiclist;
        }

        private void WriteInfluenceModels(int index, Distribution featVal, Distribution interacVal, string targetDirectory)
        {

            var sb = new StringBuilder();
            {
                for (var i = 0; i < featVal.Values.Length; i++)
                {
                    if (Setting.Dimacs)
                    {
                        sb.AppendLine(Setting.DimacsFeatureList[i] + ", " + featVal.Values[i]);
                    }
                    else
                    {
                        sb.AppendLine(Vm.BinaryOptions[i] + ", " + featVal.Values[i]);
                    }
                }
            }
            File.WriteAllText(targetDirectory + Path.DirectorySeparatorChar + "ivmF" + index + ".txt", sb.ToString());

            if (Setting.NumberOfInteractions == 0) return;
            {

                sb = new StringBuilder();
                foreach (var opt in Vm.BinaryOptions)
                {
                    sb.Append(opt + ", ");
                }
                sb.AppendLine("0");

                for (var i = 0; i < FoundInteractions.Count; i++)
                {
                    var interac = FoundInteractions[i];
                    foreach (var opt in Vm.BinaryOptions)
                    {
                        sb.Append(interac.Contains(opt) ? "1, " : "0, ");
                    }
                    sb.AppendLine(interacVal.Values[i].ToString());
                }
                File.WriteAllText(targetDirectory + Path.DirectorySeparatorChar + "ivmI" + index + ".txt", sb.ToString());

                /*foreach (var t in Setting.DimacsFeatureList)
                {
                    sb.Append(t + ", ");
                }
                sb.AppendLine("0");
                for (var i = 0; i < FoundDimacsInteractions.Count; i++)
                {
                    var interac = FoundDimacsInteractions[i];
                    foreach (var feat in Setting.DimacsFeatureList)
                    {
                        sb.Append(interac.Contains(feat) ? "1, " : "0, ");
                    }
                    sb.AppendLine(DStore.SelectedInteractionDistribution.Values[i].ToString());
                }
                File.WriteAllText(targetDirectory + Path.DirectorySeparatorChar + "ivmI" + index + ".txt", sb.ToString());


                sb = new StringBuilder();
                for (var i = 0; i < bestSolution.Features.Values.Length; i++)
                {
                    sb.AppendLine(Setting.DimacsFeatureList[i] + " * " + bestSolution.Features.Values[i]);
                }
                for (var i = 0; i < FoundDimacsInteractions.Count; i++)
                {
                    var list = FoundDimacsInteractions[i];
                    for (var j = 0; j < FoundDimacsInteractions[i].Count - 1; j++)
                    {
                        sb.Append(list[j] + " * ");
                    }
                    sb.AppendLine(list[list.Count - 1] + " * " + DStore.SelectedInteractionDistribution.Values[i]);
                }
                File.WriteAllText(targetDirectory + Path.DirectorySeparatorChar + "influenceModel" + index + ".txt",
                    sb.ToString()); */
            }
        }

        private void LoadRandomFunctions(string Property, Distribution.DistributionType type)
        {
            Distribution normal = new Distribution("normal", Property, type);
            normal.DisplayName = "Normal";
            Distribution uniform = new Distribution("uniform", Property, type);
            uniform.DisplayName = "Uniform";

            if (!AvailableDistributions.Contains(normal)) AvailableDistributions.Add(normal);
            if (!AvailableDistributions.Contains(uniform)) AvailableDistributions.Add(uniform);
        }

        public void LoadFeaturesForProperty(string selectedFeatureProperty, Distribution.DistributionType type)
        {
            AvailableDistributions.Clear();
            Console.WriteLine("LoadProperty");

            if (selectedFeatureProperty == "Random functions")
            {
                LoadRandomFunctions(selectedFeatureProperty, type);
                return;
            }


            if (selectedFeatureProperty == "") selectedFeatureProperty = "binarySize";
            var files = Directory.GetFiles(Environment.CurrentDirectory + @"\FeatureValues\" + selectedFeatureProperty + @"\");

            foreach (string file in files)
            {
                var split = file.Split(Path.DirectorySeparatorChar);
                var name = split[split.Length - 1];
                //Console.WriteLine(selectedFeatureProperty + " " + name);
                Distribution newDist = null;
                if (file.EndsWith("_fv.txt"))
                {
                    newDist = new Distribution(name, selectedFeatureProperty, Distribution.DistributionType.Feature);
                    newDist.ReadFile(false /* feature */);

                }
                if (file.EndsWith("_iv.txt"))
                {
                    newDist = new Distribution(name, selectedFeatureProperty, Distribution.DistributionType.Interaction);
                    newDist.ReadFile(false /* feature */ );
                }

                newDist.DisplayName = name.Split('_')[0];

                if (!AvailableDistributions.Contains(newDist)) AvailableDistributions.Add(newDist);
                //Distribution newDist = new Distribution(selectedFeatureProperty, )
            }
        }

        public void LoadTargetDistributionForProperty(string selectedFeatureProperty)
        {
            AvailableDistributions.Clear();

            if (selectedFeatureProperty == "Random functions")
            {
                LoadRandomFunctions(selectedFeatureProperty, Distribution.DistributionType.Variant);
                return;
            }

            if (selectedFeatureProperty == "") selectedFeatureProperty = "binarySize";
            var files = Directory.GetFiles(Environment.CurrentDirectory + @"\FeatureValues\" + selectedFeatureProperty + @"\variants\");
            foreach (string file in files)
            {
                var split = file.Split(Path.DirectorySeparatorChar);
                var name = split[split.Length - 1];

                Distribution newDist = new Distribution(name, selectedFeatureProperty, Distribution.DistributionType.Variant);
                newDist.ReadFile(true /* variant */);
                newDist.DisplayName = name.Split('.')[0];
                if (!AvailableDistributions.Contains(newDist)) AvailableDistributions.Add(newDist);
            }
        }

        public Distribution LoadSingleDistribution(string NFP, string Name)
        {
            string filename = Environment.CurrentDirectory + @"\FeatureValues\" + NFP + @"\" + Name;
            //LoadFeaturesForProperty(NFP);
            return AvailableDistributions.Find(x => x.DisplayName == Name && x.DistType == Distribution.DistributionType.Interaction);
        }



        public List<SolutionContainer> parallelHistory = new List<SolutionContainer>();
        readonly object locker = new Object();
        internal DynamicHist InteracDynamicHist;
        private SolutionSet _population;

        public void AddSolutionToHistory(SolutionContainer c)
        {
            if (Setting.Parallel)
            {
                lock (locker)
                {
                    parallelHistory.Add(c);
                }
            }
            else
            {
                History.Add(c.FoundAtEval, c);
            }
        }

        public void WriteResult()
        {
            if (File.Exists(Setting.LogFolder + Path.DirectorySeparatorChar + "interactionSolution.txt")) File.Delete(Setting.LogFolder + Path.DirectorySeparatorChar + "interactionSolution.txt");
            if (File.Exists(Setting.LogFolder + Path.DirectorySeparatorChar + "variantSolution.txt")) File.Delete(Setting.LogFolder + Path.DirectorySeparatorChar + "variantSolution.txt");
            if (File.Exists(Setting.LogFolder + Path.DirectorySeparatorChar + "featSolution.txt")) File.Delete(Setting.LogFolder + Path.DirectorySeparatorChar + "featSolution.txt");
            var sf = new StringBuilder();
           
            for (var i = 0; i < Vm.BinaryOptions.Count; i++)
            {
                sf.AppendLine(Vm.BinaryOptions[i] + ": " + BestSolution.Features.Values[i]);
            }
            if (Setting.NumberOfInteractions > 0)
            {
                var si = new StringBuilder();
                for (var i = 0; i < FoundInteractions.Count; i++)
                {
                    var interac = FoundInteractions[i];
                    for (var j = 0; j < interac.Count - 1; j++)
                    {
                        si.Append(interac[j] + "#");
                    }
                    si.Append(interac[interac.Count - 1]);
                    si.AppendLine(": " + BestSolution.Interaction.Values[i]);
                }
                File.WriteAllText(Setting.LogFolder + Path.DirectorySeparatorChar + "interactionSolution.txt", si.ToString());
                
            }
            var sv = new StringBuilder();
            for (int i = 0; i <=FeatureMatrix.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= FeatureMatrix.GetUpperBound(1); j++)
                {
                    if (FeatureMatrix[i, j] == 1)
                    {
                        sv.Append(Vm.BinaryOptions[j] + ",");
                    }
                    
                }
                sv = sv.Remove(sv.Length - 1, 1);
                sv.AppendLine(": " + BestSolution.Variant.Values[i]);
            }
            File.WriteAllText(Setting.LogFolder + Path.DirectorySeparatorChar + "variantSolution.txt", sv.ToString());
            File.WriteAllText(Setting.LogFolder + Path.DirectorySeparatorChar + "featSolution.txt", sf.ToString());

        }
    }
}
