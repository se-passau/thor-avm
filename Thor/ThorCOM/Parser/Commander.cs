using InteracGenerator;
using InteracGenerator.Problem;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace ThorCOM.Parser
{
    class Commander
    {
        private readonly Thor _model;

        public const string COMMAND_OUTPUT_PATH = "output";
        public const string COMMAND_PATH_FEATURE_MODEL = "model";
        public const string COMMAND_PATH_FEATURE_DISTRIBUTION = "feature_distribution";
        public const string COMMAND_PATH_INTERACTION_DISTRIBUTION = "interaction_distribution";
        public const string COMMAND_PATH_VARIANT_DISTRIBUTION = "variant_distribution";

        public const string COMMAND_FEATUREMODEL_NUMBER_OF_INTERACTIONS = "interaction_count";
        public const string COMMAND_FEATUREMODEL_INTERACTION = "interaction_degrees";
        public const string COMMAND_FEATUREMODEL_CALCULATE_FEATURE_FITNESS = "feature_fitness";
        public const string COMMAND_FEATUREMODEL_CALCULATE_INTERACTION_FITNESS = "interaction_fitness";
        public const string COMMAND_FEATUREMODEL_CALCULATE_VARIANT_FITNESS = "variant_fitness";
        public const string COMMAND_FEATUREMODEL_CALCULATE_VARIANTS = "variant";

        public const string COMMAND_NON_FUNCT_PROPERTY_BINARY_SIZE = "binarysize";
        public const string COMMAND_NON_FUNCT_PROPERTY_PERFORMANCE = "performance";
        public const string COMMAND_NON_FUNCT_PROPERTY_MAINMEMORY = "mainmemory";
        public const string COMMAND_NON_FUNCT_PROPERTY_RANDOM_FUNCTION = "randomfunction";
        public const string COMMAND_NORMAL_RANDOM_FUNCTION = "normal_distribution";
        public const string COMMAND_UNIFORM_RANDOM_FUNCTION = "uniform_distribution";

        public const string COMMAND_FEATURES_SCALE_MIN = "feature_scale_min";
        public const string COMMAND_FEATURES_SCALE_MAX = "feature_scale_max";
        public const string COMMAND_INTERACTIONS_SCALE_MIN = "interaction_scale_min";
        public const string COMMAND_INTERACTIONS_SCALE_MAX = "interaction_scale_max";
        public const string COMMAND_FEATURES_INITIAL_FW = "initial_fw";

        public const string COMMAND_EVOLUTION_LOGGING = "logging";
        public const string COMMAND_EVOLUTION_LOG_FOLDER = "log_folder";
        public const string COMMAND_EVOLUTION_TEST = "";
        public const string COMMAND_EVOLUTION_FIT_CRAMER_VON_MISES = "cramer_von_mises";
        public const string COMMAND_EVOLUTION_FIT_KOLMOGOROV_SMIRNOV = "kolmogorov_smirnov";
        public const string COMMAND_EVOLUTION_FIT_EUCLIDEAN_DISTANCE = "euclidean_distance";
        public const string COMMAND_EVOLUTION_FIT_CHI_SQUARED_DISTANCE = "chi_squared";
        public const string COMMAND_EVOLUTION_FIT_EUCLIDEAN_CMV = "euclidean_cmv";
        public const string COMMAND_EVOLUTION_FIT_CHI_SQUARED_CMV = "chisquared_cmv";
        public const string COMMAND_EVOLUTION_SCALE_MIN_MAX_VARIANT_DISTANCES = "scale_variant_distance";

        public const string COMMAND_EVOLUTION_ALGORITHM_MAX_EVALUATION = "max_evaluation";
        public const string COMMAND_EVOLUTION_ALGORITHM_POPULATION_SIZE = "population_size";

        public const string COMMAND_EVOLUTION_ALGORITHM_EARLY_STOP = "early_stop";
        public const string COMMAND_EVOLUTION_ALGORITHM_EARLY_STOP_LEVEL = "early_stop_level";
        public const string COMMAND_EVOLUTION_ALGORITHM_PARALLEL_NSGA_2 = "parallel_nsga";

        public const string COMMAND_EVOLUTION_DRAW_DENSITY_VALUE = "density_value";
        public const string COMMAND_EVOLUTION_DRAW_DENSITY = "density";
        public const string COMMAND_EVOLUTION_DRAW_PLOT_STEPSIZE = "plot_stepsize";
        public const string COMMAND_EVOLUTION_DRAW_FITNESS_EVOLUTION = "fitness_evolution";

        public const string COMMAND_VARIANT_USE_RANDOM = "random";
        public const string COMMAND_VARIANT_USE_FEATURE_WISE = "featurewise";
        public const string COMMAND_VARIANT_USE_NEGATIVE_FEATURE_WISE = "nfeaturewise";
        public const string COMMAND_VARIANT_USE_PAIR_WISE = "pairwise";
        public const string COMMAND_VARIANT_USE_PSEUDO_RANDOM = "pseudo_random";
        public const string COMMAND_VARIANT_USE_RANDOM_LINEAR_VALUE = "random_linear";
        public const string COMMAND_VARIANT_USE_RANDOM_QUADRATIC_VALUE = "random_quadratic";
        public const string COMMAND_VARIANT_USE_ALL_VARIANT = "all_variant";

        public const string COMMAND_VARIANT_TRESHOLD = "treshold";
        public const string COMMAND_VARIANT_MODULO = "modulo";
        public const string COMMAND_VARIANT_NUMBER_PER_CONFIGURATION_SIZE = "number_config_size";
        public const string COMMAND_VARIANT_LINEAR_CONFIG_START_SIZE = "linear_config_start_size";
        public const string COMMAND_VARIANT_QUAD_CONFIG_START_SIZE = "quadratic_config_start_size";
        public const string COMMAND_VARIANT_RANDOM_VARIANT_GENERATION_SECONDS = "random_variant_seconds";
        public const string COMMAND_VARIANT_FW_GENERATION_SECONDS = "featurewise_seconds";
        public const string COMMAND_VARIANT_NEGATIV_FW_GENERATION_SECONDS = "nfeaturewise_seconds";
        public const string COMMAND_VARIANT_PAIR_WISE_GENERATION_SECONDS = "pairwise_seconds";
        public const string COMMAND_VARIANT_SOLVER_TIMEOUT_SECONDS = "solver_timeout_seconds";

        public const string COMMAND_SOLUTION_WEIGHT_FEATURE = "weight_feature";
        public const string COMMAND_SOLUTION_WEIGHT_INTERACTION = "weight_interaction";
        public const string COMMAND_SOLUTION_WEIGHT_VARIANT = "weight_variant";
        public const string COMMAND_SOLUTION_WEIGHT_DRAW_ANGLE = "draw_angle";

        public const string COMMAND_WEIGHT_FEATURE = "weight_feature";
        public const string COMMAND_WEIGHT_INTERACTION = "weight_interaction";
        public const string COMMAND_WEIGHT_VARIANT = "weight_variant";

        //TODO Show, Save as png to output folder
        public const string COMMAND_SHOW_PSEUDORANDOM = "pseudo_random";
        public const string COMMAND_SHOW_LINEAR = "linear";
        public const string COMMAND_SHOW_QUADRATIC = "quadratic";
        public const string COMMAND_SHOW_SOLUTION = "solution";

        public const string COMMAND_SHOW_FEATURE = "feature";
        public const string COMMAND_SHOW_INTERACTION = "interaction";
        public const string COMMAND_SHOW_VARIANT = "variant";

        public const string COMMAND_SHOW_PARETO_SOLUTION = "pareto_solution";
        public const string COMMAND_SHOW_PARETO_3D = "pareto_3d";

        public List<double> interactionValues = new List<double>();
        private double[] weightValues = new double[3];
        private double[] feature_random_function_values;
        private double[] interaction_random_function_values;
        private int seconds;
        private string output_path;
        private bool draw_pareto_solution;
        private bool draw_pareto_3d;
        private bool write_finished;
        private string featurepath;
        private string interactionpath;
        private string variantpath;
        private BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        private BackgroundWorker backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
        private BackgroundWorker backgroundWorker3 = new System.ComponentModel.BackgroundWorker();

        //Interac to Strap Feature to Strap
        public Commander(Thor model)
        {
            _model = model;

            //Initialize default values
            //Feature Model
            _model.Setting.NumberOfInteractions = 0;
            _model.Setting.NoVariantCalculation = false;
            _model.Setting.FeatureFitness = true;
            _model.Setting.UseInitialFv = true;
            _model.Setting.SelectedFeature = 0;
            _model.Setting.SelectedInteraction = 0;

            //Variant
            _model.Setting.PseudoRndSize = 50;
            _model.Setting.UsePseudoRnd = true;
            _model.Setting.SolverTimeout = 3;

            //Evolution
            _model.Setting.Parallel = true;
            _model.Setting.Logging = true;
            _model.Setting.LogFolder = Environment.CurrentDirectory;
            _model.Setting.StopEarlyLevel = 1;
            //_model.Setting.DrawDensity = true;
            _model.Setting.PlotStepSize = 500;
            _model.Setting.MaxEvaluations = 5000;
            _model.Setting.PopulationSize = 50;

            //Solution            
            _model.Setting.DrawAngle = 55;

            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker3.WorkerReportsProgress = true;

        }

        public void ReadFile(string path)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    String[] tokens = line.Split(new char[0]);
                    if (tokens[0].StartsWith("#"))
                    {
                        break;
                    }
                    foreach (string token in tokens)
                    {
                        token.ToLower();
                        //Console.WriteLine("Argument: " + token);
                    }

                    //Parser _parser = new Parser();
                    //_parser.Parse(tokens);
                    PerformCommand(tokens);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void PerformCommand(string[] argument)
        {
            //Console.WriteLine("Evaluate Arguments: " + parser.Arguments.Count);
            try
            {
                //string[] argument = parser.Arguments[0];
                switch (argument[0])
                {
                    //[ARGUMENT]
                    case "load":
                        argument = argument.Skip(1).ToArray();
                        switch (argument[0])
                        {
                            case COMMAND_PATH_FEATURE_MODEL:
                                _model.LoadFM(argument[1]);
                                Console.WriteLine("Feature Count: " + _model.Setting.NumberOfFeatures);
                                break;
                            case COMMAND_PATH_FEATURE_DISTRIBUTION:
                                switch (argument[1])
                                {
                                    case COMMAND_NORMAL_RANDOM_FUNCTION:
                                    case COMMAND_UNIFORM_RANDOM_FUNCTION:
                                        feature_random_function_values = new double[2];
                                        feature_random_function_values[0] = Convert.ToDouble(argument[2]);
                                        feature_random_function_values[1] = Convert.ToDouble(argument[3]);
                                        Console.WriteLine("Feature Random Function");
                                        break;
                                    default:                                        
                                        Console.WriteLine("Feature Path: " + featurepath);
                                        break;
                                }
                                featurepath = argument[1];
                                break;
                            case COMMAND_PATH_INTERACTION_DISTRIBUTION:
                                switch (argument[1])
                                {
                                    case COMMAND_NORMAL_RANDOM_FUNCTION:
                                    case COMMAND_UNIFORM_RANDOM_FUNCTION:
                                        interaction_random_function_values = new double[2];
                                        interaction_random_function_values[0] = Convert.ToDouble(argument[2]);
                                        interaction_random_function_values[1] = Convert.ToDouble(argument[3]);
                                        Console.WriteLine("Interaction Random Function");
                                        break;
                                    default:
                                        Console.WriteLine("Interaction Path: " + featurepath);
                                        break;
                                }
                                interactionpath = argument[1];                                
                                break;
                            case COMMAND_PATH_VARIANT_DISTRIBUTION:
                                variantpath = argument[1];
                                Console.WriteLine("Variant Path: " + variantpath);
                                break;
                        }
                        break;
                    case "set":
                        // SET [ARGUMENT]
                        argument = argument.Skip(1).ToArray();
                        switch (argument[0])
                        {
                            //PATH
                            case COMMAND_OUTPUT_PATH:
                                output_path = argument[1];
                                break;

                            //FEATURE
                            case COMMAND_FEATURES_SCALE_MIN:
                                _model.Setting.FeatureScaleMin = Convert.ToDouble(argument[1]);
                                break;
                            case COMMAND_FEATURES_SCALE_MAX:
                                _model.Setting.FeatureScaleMax = Convert.ToDouble(argument[1]);
                                break;

                            //INTERACTION
                            case COMMAND_FEATUREMODEL_NUMBER_OF_INTERACTIONS:
                                _model.Setting.NumberOfInteractions = Convert.ToInt32(argument[1]);
                                break;
                            case COMMAND_FEATUREMODEL_INTERACTION:
                                //_SET_ INTERACTION 0.2 0.3 0.4
                                argument = argument.Skip(1).ToArray();
                                //_SET_ _INTERACTION 0.2 0.3 0.4
                                for (int i = 0; i < argument.Count(); ++i) { interactionValues.Add(Convert.ToDouble(argument[i]));}
                                _model.Setting.InteractionOrderPercent = Scaleto100(interactionValues);
                                break;
                            case COMMAND_INTERACTIONS_SCALE_MIN:
                                _model.Setting.InteractionScaleMin = Convert.ToDouble(argument[1]);
                                break;
                            case COMMAND_INTERACTIONS_SCALE_MAX:
                                _model.Setting.InteractionScaleMax = Convert.ToDouble(argument[1]);
                                break;
                            ///interactionValues.Add(a);
                            //FEATURE MODEL
                            case COMMAND_FEATUREMODEL_CALCULATE_FEATURE_FITNESS:
                                _model.Setting.FeatureFitness = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_FEATUREMODEL_CALCULATE_INTERACTION_FITNESS:
                                _model.Setting.InteracFitness = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_FEATUREMODEL_CALCULATE_VARIANT_FITNESS:
                                _model.Setting.VariantFitness = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_FEATUREMODEL_CALCULATE_VARIANTS:
                                _model.Setting.NoVariantCalculation = !Convert.ToBoolean(argument[1]);
                                break;


                            //EVOLUTION SETTING
                            case COMMAND_EVOLUTION_LOGGING:
                                _model.Setting.Logging = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_LOG_FOLDER:
                                _model.Setting.LogFolder = argument[1];
                                break;
                            case COMMAND_EVOLUTION_FIT_CRAMER_VON_MISES:
                                _model.Setting.UseCmv = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_FIT_KOLMOGOROV_SMIRNOV:
                                _model.Setting.UseKs = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_FIT_EUCLIDEAN_DISTANCE:
                                _model.Setting.UseEuclidean = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_FIT_CHI_SQUARED_DISTANCE:
                                _model.Setting.UseChiSquared = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_FIT_EUCLIDEAN_CMV:
                                _model.Setting.EuclAndCmv = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_FIT_CHI_SQUARED_CMV:
                                _model.Setting.ChiAndCmv = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_SCALE_MIN_MAX_VARIANT_DISTANCES:
                                _model.Setting.ScaleToGlobalMinMax = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_ALGORITHM_MAX_EVALUATION:
                                {
                                    int i = Convert.ToInt32(argument[1]);
                                    if (i < 0) { throw new WarningException("Must be > 0"); }
                                    _model.Setting.MaxEvaluations = i;
                                    break;
                                }
                            case COMMAND_EVOLUTION_ALGORITHM_POPULATION_SIZE:
                                {
                                    int i = Convert.ToInt32(argument[1]);
                                    if (i < 0) { throw new WarningException("Must be > 0"); }
                                    _model.Setting.PopulationSize = i;
                                    break;
                                }
                            case COMMAND_EVOLUTION_ALGORITHM_EARLY_STOP:
                                _model.Setting.StopEarly = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_EVOLUTION_ALGORITHM_EARLY_STOP_LEVEL:
                                {
                                    int i = Convert.ToInt32(argument[1]);
                                    if (i < 0) { throw new WarningException("Must be > 0"); }
                                    _model.Setting.StopEarlyLevel = i;
                                    break;
                                }
                            case COMMAND_EVOLUTION_ALGORITHM_PARALLEL_NSGA_2:
                                _model.Setting.Parallel = Convert.ToBoolean(argument[1]);
                                break;

                            //VARIANT GENERATION CONTROL
                            case COMMAND_VARIANT_USE_RANDOM:
                                _model.Setting.UseRnd = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_FEATURE_WISE:
                                _model.Setting.UseFw = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_NEGATIVE_FEATURE_WISE:
                                _model.Setting.UseNfw = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_PAIR_WISE:
                                _model.Setting.UsePw = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_PSEUDO_RANDOM:
                                _model.Setting.UsePseudoRnd = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_RANDOM_LINEAR_VALUE:
                                _model.Setting.LinearRandom = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_RANDOM_QUADRATIC_VALUE:
                                _model.Setting.QuadraticRandom = Convert.ToBoolean(argument[1]);
                                break;
                            case COMMAND_VARIANT_USE_ALL_VARIANT:
                                _model.Setting.UseAllVariant = Convert.ToBoolean(argument[1]);
                                break;

                            //VARIANT VALUE CONTROL
                            case COMMAND_VARIANT_RANDOM_VARIANT_GENERATION_SECONDS:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UseRnd = true;
                                    _model.Setting.RndSeconds = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_TRESHOLD:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UseRnd = true;
                                    _model.Setting.RndTreshold = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_MODULO:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UseRnd = true;
                                    _model.Setting.RndModulo = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_FW_GENERATION_SECONDS:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UseFw = true;
                                    _model.Setting.FwSeconds = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_NEGATIV_FW_GENERATION_SECONDS:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UseNfw = true;
                                    _model.Setting.NfwSeconds = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_PAIR_WISE_GENERATION_SECONDS:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UsePw = true;
                                    _model.Setting.PwSeconds = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_NUMBER_PER_CONFIGURATION_SIZE:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.UsePseudoRnd = true;
                                    _model.Setting.PseudoRndSize = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_LINEAR_CONFIG_START_SIZE:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else
                                {
                                    _model.Setting.LinearRandom = true;
                                    _model.Setting.LinearRndSize = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_QUAD_CONFIG_START_SIZE:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0 || seconds > 50) { break; }
                                else
                                {
                                    _model.Setting.QuadraticRandom = true;
                                    _model.Setting.QuadraticRandomScale = seconds;
                                }
                                break;
                            case COMMAND_VARIANT_SOLVER_TIMEOUT_SECONDS:
                                seconds = Convert.ToInt32(argument[1]);
                                if (seconds < 0) { break; }
                                else { _model.Setting.SolverTimeout = seconds; }
                                break;

                            //SOLUTION
                            case COMMAND_WEIGHT_FEATURE:
                                weightValues[0] = Convert.ToDouble(argument[1]);
                                break;
                            case COMMAND_WEIGHT_INTERACTION:
                                weightValues[1] = Convert.ToDouble(argument[1]);
                                break;
                            case COMMAND_WEIGHT_VARIANT:
                                weightValues[2] = Convert.ToDouble(argument[1]);
                                break;

                        }
                        break;
                    case "show":
                        argument = argument.Skip(1).ToArray();
                        switch (argument[0])
                        {
                            case COMMAND_SHOW_FEATURE:
                                break;
                            case COMMAND_SHOW_INTERACTION:
                                break;
                            case COMMAND_SHOW_LINEAR:
                                break;
                            case COMMAND_SHOW_PSEUDORANDOM:
                                break;
                            case COMMAND_SHOW_QUADRATIC:
                                break;
                            case COMMAND_SHOW_SOLUTION:
                                break;
                            case COMMAND_SHOW_VARIANT:
                                break;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error!");
            }

        }

        Distribution CreateDistFromFile(string path, string type)
        {
            var text = File.ReadAllText(path);
            var doublelist = new List<double>();
            text.Replace("[", string.Empty).Replace("]", string.Empty).Replace(",", string.Empty);

            var srValues = text.Split(null);
            
            foreach (var srVal in srValues)
            {
                if (!string.IsNullOrWhiteSpace(srVal))
                {
                    double val = 0;
                    if (double.TryParse(srVal, out val))
                        doublelist.Add(val);
                }
            }
            var featdist = new Distribution(doublelist.ToArray()) { };
            switch (type)
            {
                case "feature":
                    {
                        featdist.Name = "featFromFile";
                        featdist.DisplayName = "feat";
                        featdist.DistType = Distribution.DistributionType.Feature;
                        break;
                    }
                case "interaction":
                    {
                        featdist.Name = "interacFromFile";
                        featdist.DisplayName = "interac";
                        featdist.DistType = Distribution.DistributionType.Interaction;
                        break;
                    }
                case "variant":
                    {
                        featdist.Name = "variantFromFile";
                        featdist.DisplayName = "variant";
                        featdist.DistType = Distribution.DistributionType.Variant;
                        break;
                    }
            } 
            return featdist;
        }

        List<double> Scaleto100(List<double> values)
        {
            double _sum = values.Sum();
            if (Math.Abs(_sum) < 0.00000001)
            {
                var size = values.Count;
                for (var i = 0; i < values.Count; ++i)
                {
                    values[i] = 100 / size;
                    Console.WriteLine("Interaction Order "+i+": "+ values[i].ToString());
                }
            }
            else if (Math.Abs(_sum - 100.0) > 0.00000001)
            {
                var scale = 100 / _sum;
                for (var i = 0; i < values.Count; ++i)
                {
                    values[i] = values[i] * scale;
                    Console.WriteLine("Interaction Order"+i+": "+ values[i].ToString());
                }
            }
            return values;
        }
        double[] Scaleto100(double[] values)
        {
            double _sum = values.Sum();
            if (Math.Abs(_sum) < 0.00000001)
            {
                var size = values.Count();
                for (var i = 0; i < values.Count(); ++i)
                {
                    values[i] = 100 / size;
                    
                }
            }
            else if (Math.Abs(_sum - 100.0) > 0.00000001)
            {
                var scale = 100 / _sum;
                for (var i = 0; i < values.Count(); ++i)
                {
                    values[i] = values[i] * scale;
                    
                }
            }
            return values;
        }
        public void StartEvolution()
        {
            //Load and Scale Feature
            try {
                switch (featurepath) {
                    case COMMAND_NORMAL_RANDOM_FUNCTION:
                        _model.Setting.Mean = feature_random_function_values[0];
                        _model.Setting.StandardDeviation = feature_random_function_values[1];
                        _model.CreateNormalDist(1, Distribution.DistributionType.Feature);
                        break;
                    case COMMAND_UNIFORM_RANDOM_FUNCTION:
                        _model.Setting.UnifMin = feature_random_function_values[0];
                        _model.Setting.UnifMax = feature_random_function_values[1];
                        _model.CreateUnifDist(1, Distribution.DistributionType.Feature);
                        break;
                    default:
                        try
                        {
                            _model.DStore.SelectedFeatureDistribution = CreateDistFromFile(featurepath, "feature");
                        }
                        catch (Exception e) { Console.WriteLine(e); }
                        if (_model.Setting.SelectedFeature == 0) { _model.CreateNormalDist(2, Distribution.DistributionType.Feature); }
                        else { _model.CreateUnifDist(2, Distribution.DistributionType.Feature); }
                        Console.WriteLine("Selected Feature");
                        _model.BestDistribution(_model.DStore.SelectedFeatureDistribution, _model.Setting.NumberOfFeatures, 100, 2);                        
                        break;
                }
                _model.DStore.SelectedFeatureDistribution = _model.DStore.ScaledFeatureDistributions[_model.Setting.SelectedFeature];
            }
            catch (Exception e) { Console.WriteLine("Feature Scale failed"); }

            //Load and Scale Interaction
            try
            {
                switch (interactionpath)
                {
                    case COMMAND_NORMAL_RANDOM_FUNCTION:
                        _model.Setting.Mean = interaction_random_function_values[0];
                        _model.Setting.StandardDeviation = interaction_random_function_values[1];
                        _model.CreateNormalDist(1, Distribution.DistributionType.Interaction);
                        break;
                    case COMMAND_UNIFORM_RANDOM_FUNCTION:
                        _model.Setting.UnifMin = interaction_random_function_values[0];
                        _model.Setting.UnifMax = interaction_random_function_values[1];
                        _model.CreateUnifDist(1, Distribution.DistributionType.Interaction);
                        break;
                    default:
                        _model.DStore.SelectedInteractionDistribution = CreateDistFromFile(interactionpath, "interaction");
                        if (_model.Setting.SelectedInteraction == 0) { _model.CreateNormalDist(2, Distribution.DistributionType.Interaction); }
                        else { _model.CreateUnifDist(2, Distribution.DistributionType.Interaction); }
                        Console.WriteLine("Selected Interaction");
                        _model.BestDistribution(_model.DStore.SelectedInteractionDistribution, _model.Setting.NumberOfInteractions, 100, 2);
                        break;
                }
                _model.DStore.SelectedInteractionDistribution = _model.DStore.ScaledInteractionDistributions[_model.Setting.SelectedInteraction];
            }
            catch (Exception e) { Console.WriteLine("Interaction Scale failed"); }
            //Load Variant
            _model.DStore.SelectedTargetDistribution = CreateDistFromFile(variantpath, "variant");
            Console.WriteLine("Selected Variant");

            //Evolution
            Console.WriteLine("Start Evolution");
            try
            {
                //System.Windows.Forms.Application.DoEvents();
                backgroundWorker1.RunWorkerAsync(_model);
            }
            catch (Exception e) { Console.WriteLine("Evolution failed"); }

            while (!write_finished)
            {
                //Wait half a second.
                System.Threading.Thread.Sleep(500);
            }
        }

        public void WriteResult()
        {
            //After Evolution
            //weightValues = Scaleto100(weightValues);
            var sum = weightValues[0]+ weightValues[1]+ weightValues[2];
            if (sum != 100)
            {
                var scale = 100 / sum;
                weightValues[0] = weightValues[0] * scale;
                weightValues[1] = weightValues[1] * scale;
                weightValues[2] = weightValues[2] * scale;
            }
            try
            {
                _model.ShowParetoSolution(weightValues[0], weightValues[1], weightValues[2]);
            }
            catch (Exception e) { Console.WriteLine(e); }
                //if (draw_pareto_3d) { _model.Draw3DPareto(); }
            //Write Results to path
            _model.WriteResult(output_path);
            Console.WriteLine("Done.");
            write_finished = true;
        }

        #region Interactions
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var mdl = e.Argument as Thor;
            mdl.Setting.WriteSetting();
            var bw = sender as BackgroundWorker;
            mdl?.CreateInteractions(e, bw);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker2.RunWorkerAsync(_model);
        }
        #endregion

        #region Generate Variants
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var mdl = e.Argument as Thor;
            var bw = sender as BackgroundWorker;
            mdl?.CreateVariants(bw, e);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker3.RunWorkerAsync(_model);
        }
        #endregion

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            var mdl = e.Argument as Thor;
            var bw = sender as BackgroundWorker;
            try { mdl?.StartEvolution(bw, e); }
            catch (Exception r) { Console.WriteLine("Error: Can't Start Evolution"); Console.WriteLine(r); }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WriteResult();
        }


    }
}
