using InteracGenerator.InteractionProblem;
using JMetalCSharp.Core;
using JMetalCSharp.Utils.Wrapper;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;


namespace InteracGenerator

{
    public class RIntegrator
    {
        public static string ImagePath = "";
        private static readonly REngine Engine = REngine.GetInstance();

        static readonly HashSet<string> StoredValues = new HashSet<string>();

        private const string LegendNoFill = " + guides(color = guide_legend(override.aes=list(fill= NA)))";
        private const string PanelGrid = " + theme(panel.grid.minor = element_line(colour = \"grey\", size = 0.1), panel.grid.major = element_line(colour = \"darkgrey\", size = 0.2)) + theme(legend.position='top')";
        private const string DensityPercent =
            " + scale_y_continuous(labels = percent_format(), name = 'Density', expand = c(0, 0))" +
            " + scale_x_continuous(labels=comma, name = \"Attribute values\", expand = c(0, 0))";

        private static InterGen _model;

        public static void SetFeatureValues(string name, double[] values)
        {
            Console.WriteLine("Setting Values: " + name);
            var data = Engine.CreateNumericVector(values);
            Engine.SetSymbol(name, data);
            StoredValues.Add(name);
        }

        public static void Init(InterGen model)
        {
            _model = model;
            Engine.AutoPrint = false;
            Engine.Evaluate("suppressMessages(require('ks', quietly=TRUE))");
            //Engine.Evaluate("install.packages('ggplot2')");
            Engine.Evaluate("suppressMessages(library(ks))");
            Engine.Evaluate("require('ggplot2', quietly=TRUE)");
            Engine.Evaluate("suppressMessages(library(ggplot2))");
            Engine.Evaluate("require('scales', quietly=TRUE)");
            Engine.Evaluate("suppressMessages(library(scales))");
            Engine.Evaluate("require(gridExtra)");
            Engine.Evaluate("require(scatterplot3d)");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        public static Distribution BootStrapValues(Distribution dist, int size) {
            

            if (!StoredValues.Contains(dist.Name)) {
                Console.WriteLine("Error");
            }
            var result = new double[size];
           
            Engine.Evaluate(string.Format("fhat <- kde({0} , h = hpi({0}));", dist.Name));
            Engine.Evaluate(dist.Name + "Fit <-rkde (fhat = fhat, n = " + size + ")");

            
            var answer = Engine.Evaluate(dist.Name + "Fit");
            if (answer.IsVector())
            {
                var v = answer.AsVector();

                var i = 0;
                foreach (double val in v)
                {
                    result[i] = val;
                    i++;
                }
            }
            var newDist = new Distribution(result)
            {
                Name = dist.Name,
                DisplayName = dist.DisplayName, 
                DistType = dist.DistType,
                Scaled = true
            };
            return newDist;
        }


        public static void PlotRandomDist(Distribution dist, double adjust)
        {
            //Console.WriteLine("Loading ggplot2");
            //Engine.Evaluate("require('ggplot2')");
            var data = Engine.CreateNumericVector(dist.Values);
            Engine.SetSymbol(dist.Name, data);
            //StoredValues.Add(dist.Name);
           
            PlotValues(dist.Name, dist.ImagePath, adjust, dist.DistType);
        }


        public static void PlotValues(Distribution dist, double adjust) {
            //Console.WriteLine("Loading ggplot2");
            //Engine.Evaluate("require('ggplot2')");
            var data = Engine.CreateNumericVector(dist.Values);
            Engine.SetSymbol(dist.Name, data);
            StoredValues.Add(dist.Name);
            switch (dist.DistType)
            {
                case Distribution.DistributionType.Feature:
                    dist.ImagePath = "fDist.png";
                    break;
                case Distribution.DistributionType.Interaction:
                    dist.ImagePath = "iDist.png";
                    break;
                default:
                    dist.ImagePath = "vDist.png";
                    break;
            }
            PlotValues(dist.Name, dist.ImagePath, adjust, dist.DistType);
        }

        /*public static void PlotValues(double[] dist, string name) {
            NumericVector data = Engine.CreateNumericVector(dist);
            StoredValues.Add(name);
            Engine.SetSymbol(name, data);
            //PlotValues(name);
        } */

        public static void PlotValues(SolutionSet currentBestSolutions, IntergenProblem problem) {

            /*if (currentBestSolutions.Size() < 2) { return; }

            var dir = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            var project =  dir.Replace("\\", "/");
           
            string test = "source('" + project + "/PlotMulti.R')";


           var result = Engine.Evaluate(test);

            string imagePath = "CurrentBest";
           
            //Engine.Evaluate($"png('{ImagePath.Replace('\\', '/') + imagePath + ".png"}', {900}, {600})");
            //engine.AutoPrint = true;
            var combined = "";
            var varCombined = "";
            for (var i = 0; i < currentBestSolutions.Size(); i++)
            {
                string name = "CurrentBests" + i;
                Solution sol = currentBestSolutions.Get(i);
                XReal wrap = new XReal(sol);
                double[] values = new double[wrap.Size()];

                for (int j = 0; j < wrap.Size(); j++)
                {
                    values[j] = wrap.GetValue(j);
                }
                var withoutInterac = problem.FeatureMatrix.Multiply(values);
                var variantValues = problem.InteractionValuesPerConfig != null ? withoutInterac.Add(problem.InteractionValuesPerConfig) : withoutInterac;


                FMScaling fms = new FMScaling(problem.GetModel());

                Distribution targetV = fms.InteractionToScale(problem.VariantTarget, variantValues.Min(), variantValues.Max());
                NumericVector variantData = Engine.CreateNumericVector(targetV.Values);
                StoredValues.Add("targetBest" + i);
                Engine.SetSymbol("targetBest" + i, variantData);
                if (i < currentBestSolutions.Size() - 1)
                {
                    varCombined += "targetBest" + i + ", ";
                }
                else
                {
                    varCombined += "targetBest" + i;
                }

                //PlotValues(values, "test" + i);

                NumericVector data = Engine.CreateNumericVector(values);
                StoredValues.Add(name);
                Engine.SetSymbol(name, data);

                if (i < currentBestSolutions.Size() - 1)
                {
                    combined += "CurrentBests" + i + ", ";
                }
                else {
                    combined += "CurrentBests" + i;
                }
                //engine.Evaluate("d" + i + " <- density(" + name + ")");
                //

                //TODO CREATE LIST FOR plot multi dens 
            }

            Engine.Evaluate("best(list(" + combined + "))");
           // Engine.Evaluate("dev.off()");

            //Engine.Evaluate(string.Format("png('{0}', {1}, {2})", ImagePath.Replace('\\', '/') + "VariantBest.png", 900, 600));
            //Engine.Evaluate("test(list(" + varCombined + "))");
            //Engine.Evaluate("dev.off()");

            //Console.WriteLine("Plotted Pareto"); */
        }





        internal static string PlotVariantTarget(double[] first, double[] second, int index)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Variants", data1);
            Engine.SetSymbol("second", data2);

            string imagePath = $"variantComp{index}.png";

            // Engine.Evaluate("popul <- data.frame(first)");

            //Engine.Evaluate("vTarget <- data.frame('second')");
            Engine.Evaluate(
                "ggplot() + geom_density(aes( x = Variants), adjust=0.5, alpha=0.4, fill='red')  + geom_density(aes( x = second), adjust=0.5) + labs(title='Calculated Variants vs Variants Target') + scale_x_continuous(labels = comma) + scale_y_continuous(labels = comma)");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");

            return imagePath;
        }

        internal static string PlotFeatureTarget(double[] first, double[] second, int index)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Features", data1);
            Engine.SetSymbol("secondF", data2);

            string imagePath = $"featuresComp{index}.png";

            Engine.Evaluate(
                "ggplot() + geom_density(aes( x = Features), adjust=0.5, alpha=0.4, fill='green')  + geom_density(aes( x = secondF), adjust=0.5) + labs(title='Feature Target vs Feature Population')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");

            return imagePath;
        }


        /*public static void PlotValues(Solution currentBestSolution) {
           // Console.WriteLine(CurrentBestSolution.Variable);
            var wrap = new XReal(currentBestSolution);
            var values = new double[wrap.Size()];

            for (var i = 0; i < wrap.Size(); i++)
            {
                values[i] = wrap.GetValue(i);
            }
            PlotValues(values, "CurrentBest"); 
          
        }*/

        public static void PlotValues(string name, string imagepath, double adjust, Distribution.DistributionType type)
        {
            if (!StoredValues.Contains(name))
            {
                //throw new ArgumentNullException("Values not found: " + name);
            }
            var color = "red";
            if (type == Distribution.DistributionType.Variant) color = "blue";
            if (type == Distribution.DistributionType.Feature) color = "green";
            Engine.Evaluate($"featureDist1 <- data.frame({name})");
            Engine.Evaluate($"plot <- ggplot(data=featureDist1, aes({name})) + geom_density(fill='{color}', alpha=0.3, adjust=0.5)" + LegendNoFill + PanelGrid + DensityPercent);
            Engine.Evaluate($"ggsave(plot=plot, file='{imagepath}', width=7, height=5)");
        }

        internal static double[] GenerateNormalDistribution(double min, double max, int numberOfFeatures)
        {
            try {
                /*    Engine.Evaluate($"rnorm.between <- function(n, minimum = 0, maximum = 1)");
                    Engine.Evaluate("{");
                    Engine.Evaluate($"x <- rnorm(n)");
                    Engine.Evaluate($"max_x <- max(x)");
                    Engine.Evaluate($"min_x <- min(x)");
                    Engine.Evaluate($"x <- x - min_x");
                    Engine.Evaluate($"x <- x / (max_x - min_x)");
                    Engine.Evaluate($"x <- x * (maximum - minimum)");
                    Engine.Evaluate($"x <- x + minimum");
                    Engine.Evaluate($"return (x)");
                    Engine.Evaluate("}");
                    */
                //var func = Engine.Evaluate("function (n, min, max) {x <- rnorm(n, mean = 0, sd = 1); x <- x - -3; x <- x / (6); x <- x * (min - max); y <- x + min; return (y) }").AsFunction();
                var func = Engine.Evaluate(@"function (n, min, max) {
                    x <- rnorm(n, mean = 0, sd = 1)
                    x <- x - -3
                    x <- x / (6)
                    x <- x * (max - min)
                    x <- x + min
                    return (x) 
                }").AsFunction();
                var param1 = Engine.CreateInteger(numberOfFeatures);
                var param2 = Engine.CreateNumeric(min);
                var param3 = Engine.CreateNumeric(max);
                return func.Invoke(new SymbolicExpression[] { param1, param2, param3 } ).AsNumeric().ToArray();
                //return Engine.Evaluate($"result <- rnorm.between({numberOfFeatures}, {min}, {max})").AsNumeric().ToArray();
            }
            catch(Exception e)
            { Console.WriteLine(e.ToString()); return new double[] { 0 }; }
        }

        public static void PlotMultiple(string name) {
            Engine.Evaluate("plot(density(" + name + "))");
        }

        public static string PlotOrigAndFitted(Distribution first, Distribution scaled, int imageNumber, double adjust)
        {

            NumericVector data1 = Engine.CreateNumericVector(first.Values);
            NumericVector data2 = Engine.CreateNumericVector(scaled.Values);
            Engine.SetSymbol("Scaled", data1);
            Engine.SetSymbol("Original", data2);

            var color = "red";
            if (first.DistType == Distribution.DistributionType.Feature) color = "green";
            if (first.DistType == Distribution.DistributionType.Variant) color = "blue";
            string imagePath = first.Name + "_" + imageNumber + ".png";
            //Engine.Evaluate("origDF <- data.frame(" + first.Name + ")");
            //Engine.Evaluate("names(origDF) <- c('DistOrig')");
            //Engine.Evaluate("newDF <- data.frame(" + scaled.Name + ")");
            //Engine.Evaluate("names(newDF) <- c(\"DistNew\")");

            //Engine.Evaluate("ggplot() + geom_density(aes(x = DistNew, y =..scaled..), alpha = 0.5, fill = \"blue\", adjust = 0.2, data = newDF) + scale_y_continuous(labels = percent_format(), name = \"Density\", expand = c(0, 0)) + scale_x_continuous(name = \"Attribute values\", expand = c(0, 0)) + theme(panel.grid.minor = element_line(colour = \"grey\", size = 0.1), panel.grid.major = element_line(colour = \"darkgrey\", size = 0.2)) + ggtitle(\"Generated distribution\") + geom_density(aes(x = DistOrig, y =..scaled..), alpha = 0.5, fill = \"red\", adjust = 1 / 5, data = origDF)");
            Engine.Evaluate(
                $"ggplot() + geom_density(aes(x = Original, colour='Scaled'), adjust = 0.5, alpha = 0.3, fill = '{color}')" +
                $" + geom_density(aes(x = Scaled, colour='Target'), adjust = 0.5) + labs(title = 'Scaled and Original')" +
                $" + scale_colour_manual(values = c('Target' = 'black', 'Scaled' = '{color}'), name = 'Densities')" + 
                LegendNoFill + DensityPercent + PanelGrid);
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
            return imagePath;

          
        }

        public static string PlotFeatureTarget(double[] first, double[] second, double adjust)
        {

            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Features", data1);
            Engine.SetSymbol("secondF", data2);

            var imagePath = "featuresComp.png";
            //Console.WriteLine("The current culture is {0}",
              //          System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            Engine.Evaluate(
               $"ggplot() + geom_density(aes( x = Features, colour='Population'), adjust=0.5, alpha=0.4, fill='green') " +
               $" + geom_density(aes( x = secondF, colour='Target'), adjust=0.5)" +
               $" + labs(title='Feature Target vs Feature Population')" +
               $" + scale_colour_manual(values = c('Target' = 'black', 'Population' = 'green'), name = 'Densities')" + LegendNoFill + DensityPercent + PanelGrid);
            try
            {
                Engine.Evaluate($"unlink('{imagePath}')");
                Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
               
            }
            catch (EvaluationException exc)
            {
                Console.WriteLine("With unlink: " + exc.Message);
                //Thread.Sleep(10000);
            }

            return imagePath;
        }

    

        public static string FeatureComparisonHist(double[] first, double[] second)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Features", data1);
            Engine.SetSymbol("secondF", data2);

            string imagePath = "featuresComp.png";

            Engine.Evaluate(
                "ggplot() + geom_histogram(aes( x = Features), alpha=0.5, fill='green')  + geom_histogram(aes( x = secondF), fill='black', alpha=0.2) + labs(title='Feature Target vs Feature Population')");
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
            return imagePath;
        }

        internal static void PlotVariantTarget(double[] values)
        {
            NumericVector data1 = Engine.CreateNumericVector(values);
            Engine.SetSymbol("Variants", data1);
            string imagePath = "variantComp.png";

            // Engine.Evaluate("popul <- data.frame(first)");

            //Engine.Evaluate("vTarget <- data.frame('second')");
            Engine.Evaluate(
                "ggplot() + geom_density(aes( x = Variants), adjust=1.2, alpha=0.4, fill='blue') + labs(title = 'Calculated Variants')" +
                " + scale_colour_manual(values = c('Target' = 'black', 'Population' = 'blue'), name = 'Densities')" + DensityPercent + PanelGrid);
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
        }

        public static string VariantComparisonHisto(double[] first, double[] second)
        {
            var data1 = Engine.CreateNumericVector(first);
            var data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Variants", data1);
            Engine.SetSymbol("second", data2);
            var imagePath = "variantComp.png";
            Engine.Evaluate(
               "ggplot() + geom_histogram(aes( x = Variants), alpha=0.6, fill='red', colour='black', binwidth=1) " + /* " + geom_histogram(aes( x = second), fill='white', colour='black', alpha=0.2)*/ " + labs(title='Calculated Variants vs Variants Target')");
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
            return ImagePath;
        }

        public static string PlotInteracTarget(double[] first, double[] second)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("population", data1);
            Engine.SetSymbol("second", data2);

            const string imagePath = "interacComp.png";

            // Engine.Evaluate("popul <- data.frame(first)");

            //Engine.Evaluate("vTarget <- data.frame('second')");
           Engine.Evaluate(
                "ggplot() + geom_density(aes( x = population, colour='Population'), adjust=0.5, alpha=0.4, fill='red') " +
                " + geom_density(aes( x = second, colour='Target'), adjust=0.5) + labs(title='Interactions vs Target Interactions')" +
                " + scale_colour_manual(values = c('Target' = 'black', 'Population' = 'red'), name = 'Densities') " + DensityPercent + LegendNoFill + PanelGrid);
            try
            {
                Engine.Evaluate($"unlink('{imagePath}')");
                Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
            }
            catch (EvaluationException exc)
            {
                Console.WriteLine(exc.Message);
            }

            return imagePath;
        }



        public static string PlotVariantTarget(double[] first, double[] second)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Variant", data1);
            Engine.SetSymbol("second", data2);

            string imagePath = "variantComp.png";

           // Engine.Evaluate("popul <- data.frame(first)");
        
            //Engine.Evaluate("vTarget <- data.frame('second')");
            Engine.Evaluate(
                "ggplot() + geom_density(aes( x = Variant, colour='Population'), adjust=0.5, alpha=0.4, fill='blue') " +
                " + geom_density(aes( x = second, colour='Target'), adjust=0.5)" +
                " + labs(title='Calculated Variants vs Variants Target')" +
                " + scale_colour_manual(values = c('Target' = 'black', 'Population' = 'blue'), name = 'Densities')" + DensityPercent + LegendNoFill + PanelGrid);
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");

            return imagePath;
        }

        public static string DrawDynamicBin(int StartBins, int StartEvolution, int EndBins, int EndEvolution,
            int MaxEvolutions, double slope, double yinterc)
        {
            var stringfunction =
                string.Format(
                    "indi.f <- function(x) {{ ({0}) * (x <= {1}) + ({2} * x + {3}) * (({1} < x) & (x < {4})) + ({5}) * (x >= {4})}}",
                    StartBins, StartEvolution, slope, yinterc, EndEvolution, EndBins);

            var ggplot = "p <- ggplot(data = data.frame(x = 0), mapping = aes(x = x))";
            var plot =
                $"p + stat_function(fun = indi.f) + xlim(0, {MaxEvolutions}) + ylim(0, {EndBins}) + xlab('Evolutionsschritt') + ylab('Bin Count')";


            Engine.Evaluate(stringfunction);
            Engine.Evaluate(ggplot);
            Engine.Evaluate(plot);
            Engine.Evaluate("ggsave(file='dynamicHist.png', width=5, height=5)");
            return "dynamicHist.png";
        }

        public static string FeatureHistAndDens(double[] first, double[] second)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Features", data1);
            Engine.SetSymbol("secondF", data2);

            string imagePath = "featuresComp.png";

            Engine.Evaluate(
                "ggplot() + geom_histogram(aes( x = Features), alpha=0.5, fill='green')  + geom_histogram(aes( x = secondF),fill='white', colour='black', alpha=0.2) + labs(title='Feature Target vs Feature Population')");
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
            return imagePath;
        }

        public static string VariantHistAndDens(double[] first, double[] second)
        {
            NumericVector data1 = Engine.CreateNumericVector(first);
            NumericVector data2 = Engine.CreateNumericVector(second);
            Engine.SetSymbol("Variants", data1);
            Engine.SetSymbol("second", data2);

            string imagePath = "variantComp.png";

            // Engine.Evaluate("popul <- data.frame(first)");

            //Engine.Evaluate("vTarget <- data.frame('second')");
            Engine.Evaluate(
                "ggplot() + geom_histogram(aes( x = Variants), alpha=0.5, fill='red') " + /*"+ geom_histogram(aes( x = second), fill='black', alpha=0.2)*/ "+ labs(title='Calculated Variants vs Variants Target')");
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");

            return imagePath;
        }

        public static void PlotPareto(string featureTests, string variantTests)
        {
            Engine.Evaluate("feat <- " + featureTests);
            Engine.Evaluate("varian <- " + variantTests);

            Engine.Evaluate("paretoFrame <- data.frame(feat, varian)");

            Engine.Evaluate("ggplot(paretoFrame, (aes(x = feat, y = varian))) + geom_point(shape=1) + geom_smooth()");
            Engine.Evaluate("ggsave(file='test.png', width=6, height=5)");
        }

        public static void PlotPseudoRandom(int pseudoRndSize, int features)
        {
            var function =
                $"indi.f <- function(x) {{ (0) * (x <= 0) + ({pseudoRndSize}) * (x > 0) + (-{pseudoRndSize}) * (x > {features}) }}";
            Engine.Evaluate(function);
            Engine.Evaluate("p <- ggplot(data = data.frame(x = 0), mapping = aes(x=x))");
            Engine.Evaluate($"p + stat_function(fun = indi.f) + xlim(-5, {features + 5}) + ylim(0, {pseudoRndSize + 5}) + xlab('Configuration Size') + ylab('Number of Configurations')");
            Engine.Evaluate("ggsave(file='VariantGenerationFunction.png', width=15, height=5)");
        }


        public static void PlotPseudoLinear(int linearRndSize, int features)
        {
            var halfSize = features + linearRndSize;
            float slope = linearRndSize/(float) features;
            var function =
                $"indi.f <- function(x) {{ (0) * (x <= 0) + (({2 *slope} * x) + {linearRndSize}) * (0 < x & x <= {features / 2}) + ((-{2*slope} * x) + {3*linearRndSize}) * (x > {features/2} & x <= {features}) + (0) + (x < {features}) }}";
            Engine.Evaluate(function);
            Engine.Evaluate("p <- ggplot(data = data.frame(x = 0), mapping = aes(x=x))");
            Engine.Evaluate($"p + stat_function(fun = indi.f) + xlim(-5, {features + 5}) + ylim(0, { 5 + 2.0* linearRndSize}) + xlab('Configuration Size') + ylab('Number of Configurations')");
            Engine.Evaluate("ggsave(file='VariantGenerationFunction.png', width=15, height=5)");
        }

        public static void PlotQuadraticRandom(double quadraticRandomScale, int numberOfFeatures)
        {
            var function =
                $"indi.f <- function(x) {{ (0) * (x <= 0) + ({quadraticRandomScale} * -((x-{numberOfFeatures/2})^2) + {quadraticRandomScale} * {Math.Pow(numberOfFeatures/2, 2)})  * (0 < x & x <= {numberOfFeatures}) + (0) * (x > {numberOfFeatures}) }}";
            Engine.Evaluate(function);
            Engine.Evaluate("p <- ggplot(data = data.frame(x = 0), mapping = aes(x=x))");
            Engine.Evaluate($"p + stat_function(fun = indi.f) + xlim(-5, {numberOfFeatures + 5}) + ylim(-5, {quadraticRandomScale} * {Math.Pow(numberOfFeatures/2, 2)} + 5) + xlab('Configuration Size') + ylab('Number of Configurations')");
            Engine.Evaluate("ggsave(file='VariantGenerationFunction.png', width=15, height=5)");
        }

        public static void PlotSolution(double[] featVals, double[] values, double[] interacVars, double[] interacTar, double[] complete, Distribution varTar, int index)
        {
            NumericVector featValues = Engine.CreateNumericVector(featVals);
            NumericVector featTarget = Engine.CreateNumericVector(values);
            Engine.SetSymbol("featValues", featValues);
            Engine.SetSymbol("featTarget", featTarget);

            Engine.Evaluate(
                "featplot <- ggplot() + geom_density(aes( x = featValues), adjust=0.5, alpha=0.4, fill='green')  + geom_density(aes( x = featTarget), adjust=0.5) + labs(title='Calculated Features vs Feature Target')");


            NumericVector interacValues = Engine.CreateNumericVector(interacVars);
            NumericVector interacTarget = Engine.CreateNumericVector(interacTar);
            Engine.SetSymbol("interacValues", interacValues);
            Engine.SetSymbol("interacTarget", interacTarget);

            Engine.Evaluate(
                "interacPlot <- ggplot() + geom_density(aes( x = interacValues), adjust=0.5, alpha=0.4, fill='red')  + geom_density(aes( x = interacTarget), adjust=0.5) + labs(title='Calculated Interaction vs Interaction Target')");


            NumericVector varValues = Engine.CreateNumericVector(complete);
            Engine.SetSymbol("varValues", varValues);
            if (varTar != null)
            {
                NumericVector varTarget = Engine.CreateNumericVector(varTar.Values);
                Engine.SetSymbol("varTarget", varTarget);
                Engine.Evaluate(
               "varPlot <- ggplot() + geom_density(aes( x = varValues), adjust=0.5, alpha=0.4, fill='blue')  + geom_density(aes( x = varTarget), adjust=0.5) + labs(title='Calculated Variants vs Variant Target')");
            }
            else
            {
                Engine.Evaluate(
              "varPlot <- ggplot() + geom_density(aes( x = varValues), adjust=0.5, alpha=0.4, fill='red')  + labs(title='Calculated Variants')");
            }

          
            Engine.Evaluate("png(file='solutionBest.png', width=2024, height=550)");
            Engine.Evaluate($"grid.arrange(featplot, interacPlot, varPlot, ncol = 3, top='Solution {index}')");
            Engine.Evaluate("dev.off()");
        }

        public static void PlotSolution(double[] featVals, double[] values, double[] complete, Distribution varTar, int index)
        {
            NumericVector featValues = Engine.CreateNumericVector(featVals);
            NumericVector featTarget = Engine.CreateNumericVector(values);
            Engine.SetSymbol("featValues", featValues);
            Engine.SetSymbol("featTarget", featTarget);

            Engine.Evaluate(
                "featplot <- ggplot() + geom_density(aes( x = featValues), adjust=0.5, alpha=0.4, fill='green')  + geom_density(aes( x = featTarget), adjust=0.5) + labs(title='Calculated Features vs Feature Target')" + DensityPercent + LegendNoFill + PanelGrid);


            NumericVector varValues = Engine.CreateNumericVector(complete);
            Engine.SetSymbol("varValues", varValues);
            if (varTar != null)
            {
                NumericVector varTarget = Engine.CreateNumericVector(varTar.Values);
                Engine.SetSymbol("varTarget", varTarget);
                Engine.Evaluate(
               "varPlot <- ggplot() + geom_density(aes( x = varValues), adjust=0.5, alpha=0.4, fill='blue')  + geom_density(aes( x = varTarget), adjust=0.5) + labs(title='Calculated Variants vs Variant Target')");
            }
            else
            {
                Engine.Evaluate(
              "varPlot <- ggplot() + geom_density(aes( x = varValues), adjust=1.0, alpha=0.4, fill='blue')  + labs(title='Calculated Variants')" + DensityPercent + LegendNoFill + PanelGrid);
            }


            Engine.Evaluate("png(file='solutionBest.png', width=1524, height=550)");
            Engine.Evaluate($"grid.arrange(featplot, varPlot, ncol = 2, top='Solution {index}')");
            Engine.Evaluate("dev.off()");
        }

        public static void Draw3dPareto(double[] featFit, double[] interacFit, double[] variantFit, int angle=55)
        {
            NumericVector featFitn = Engine.CreateNumericVector(featFit);
            NumericVector interacFitn = Engine.CreateNumericVector(interacFit);
            NumericVector varFitn = Engine.CreateNumericVector(variantFit);
            Engine.SetSymbol("featfitn", featFitn);
            Engine.SetSymbol("interacfitn", interacFitn);
            Engine.SetSymbol("varfitn", varFitn);

            Engine.Evaluate("png(file='3dplot.png', width=1024, height=600)");
            Engine.Evaluate($"s3d <- scatterplot3d(featfitn, varfitn, interacfitn, highlight.3d=TRUE, col.axis='blue', angle={angle}, type='h',box=FALSE, lwd='1', pch=4)");
            Engine.Evaluate("s3d.coords <- s3d$xyz.convert(featfitn,varfitn,interacfitn)");
            Engine.Evaluate("text(s3d.coords$x, s3d.coords$y, labels=which(s3d.coords$x & s3d.coords$y)-1, pos=3,cex=0.9)");
            Engine.Evaluate("dev.off()");
        }

        public static void DrawFitnessEvolution(string logFolder, bool featFit, bool interacFit, bool varFit)
        {

            var project = logFolder.Replace("\\", "/");
            var dir = Environment.CurrentDirectory + Path.DirectorySeparatorChar;
            var homeDir = dir.Replace("\\", "/");
            if (project.Equals("/"))
            {
                project = homeDir;
            }

            if (featFit)
            {
                Engine.Evaluate($"feat <- read.table(paste('{project}', 'FitnFeat.txt', sep = '/'))");
                Engine.Evaluate("feat$evolution_step <- seq.int(nrow(feat))");
                Engine.Evaluate("colnames(feat)[1] <- 'CmV_Fitnessvalue'");
                Engine.Evaluate(
                    "ggplot(feat) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle('CmV Fitnessvalue of Features')");
                Engine.Evaluate($"ggsave(filename = paste('{homeDir}', 'FeatFitn.png', sep = '/'))");
            }

            if (interacFit)
            {
                Engine.Evaluate($"interac <- read.table(paste('{project}', 'FitnInterac.txt', sep = '/'))");
                Engine.Evaluate("interac$evolution_step <- seq.int(nrow(interac))");
                Engine.Evaluate("colnames(interac)[1] <- 'CmV_Fitnessvalue'");
                Engine.Evaluate(
                    "ggplot(interac) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle('CmV Fitnessvalue of Interactions')");
                Engine.Evaluate($"ggsave(filename = paste('{homeDir}', 'InteracFitn.png', sep = '/'))");
            }
            if (varFit)
            {
                Engine.Evaluate($"var <- read.table(paste('{project}', 'FitnVariant.txt', sep = '/'))");
                Engine.Evaluate("var$evolution_step <- seq.int(nrow(var))");
                Engine.Evaluate("colnames(var)[1] <- 'CmV_Fitnessvalue'");
                Engine.Evaluate(
                    "ggplot(var) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle('CmV Fitnessvalue of Variants')");
                Engine.Evaluate($"ggsave(filename = paste('{homeDir}', 'VariantFitn.png', sep = '/'))");
            }
            /*  interacvalgraph < -function(x) {
                  interac < -read.table(paste(x, "FitnInterac.txt", sep = "/"))
                  interac$evolution_step < -seq.int(nrow(interac))
              colnames(interac)[1] < -"CmV_Fitnessvalue"
              ggplot(interac) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle("CmV Fitnessvalue of Interactions")
              ggsave(filename = paste(x, "InteracFitn.png", sep = "/"))
              }

              variantvalgraph < -function(x) {
                  interac < -read.table(paste(x, "FitnVariant.txt", sep = "/"))
                  interac$evolution_step < -seq.int(nrow(interac))
              colnames(interac)[1] < -"CmV_Fitnessvalue"
              ggplot(interac) + geom_point(aes(x = evolution_step, y = CmV_Fitnessvalue), size = 0.3, alpha = 0.3) + ggtitle("CmV Fitnessvalue of Variants")
              ggsave(filename = paste(x, "VariantFitn.png", sep = "/"))
              })
               */
            // string test = "source('" + project + "/SolutionPlot.R')";
            //Engine.Evaluate(test);
            //Engine.Evaluate($"featvalgraph('{project}')");
            //Engine.Evaluate($"interacvalgraph('{project}')");
            //Engine.Evaluate($"variantvalgraph('{project}')");
        }

        public static void InteracHist(double[] interacVal, double[] target)
        {
            NumericVector data1 = Engine.CreateNumericVector(interacVal);
            NumericVector data2 = Engine.CreateNumericVector(target);
            Engine.SetSymbol("Interactions", data1);
            Engine.SetSymbol("secondF", data2);

            var imagePath = "interacComp.png";

            Engine.Evaluate(
                "ggplot() + geom_histogram(aes( x = Interactions), alpha=0.5, fill='red')  + geom_histogram(aes( x = secondF), fill='white', colour='black', alpha=0.2)  + labs(title='Interaction Target vs Interaction Population')");
            Engine.Evaluate($"unlink('{imagePath}')");
            Engine.Evaluate($"ggsave(file='{imagePath}', width=7, height=5)");
           
        }

        public static void PlotFitnSeries(List<double> featMax, List<double> featMin, string fileName)
        {
            try
            {
                var data1 = Engine.CreateNumericVector(featMax);
                var data2 = Engine.CreateNumericVector(featMin);
                Engine.SetSymbol("data1", data1);
                Engine.SetSymbol("data2", data2);
                Engine.Evaluate("data <- data.frame(max = data1, min = data2)");

                Engine.Evaluate(
                    $"ggplot(data) + geom_line(aes(x= 50* c(1:nrow(data)), y=max)) + geom_line(aes(x=50*c(1:nrow(data)), y=min)) + scale_x_continuous(limits = c(0, {_model.Setting.MaxEvaluations}))");

                Engine.Evaluate($"unlink('{fileName}')");
                Engine.Evaluate($"ggsave(file='{fileName}', width=7, height=5)");
            }
            catch (EvaluationException e)
            {
                Console.WriteLine(e.Message);
                //Thread.Sleep(1000);
            }

        }
    }
}
