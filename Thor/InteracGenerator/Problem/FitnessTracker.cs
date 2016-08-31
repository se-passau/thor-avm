using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InteracGenerator.Problem
{
    public struct MinMaxFitness
    {
        public int Step;
        public double FeatMin;
        public double FeatMax;
        public double InterMin;
        public double InterMax;
        public double VarMin;
        public double VarMax;
    }

    internal struct Fitness : IComparable<Fitness>
    {
        public int Eval;
        public double FeatVal;
        public double InteracVal;
        public double Varval;


        public int CompareTo(Fitness other)
        {
            return Eval.CompareTo(other.Eval);
        }
    }

    public static class FitnessTracker
    {
        public static int Counter;
        public static long MultiplicationTime { get; set; }
        public static long FitnessTime { get; set; }
        private static List<Fitness> _fitnessValues;
        private static object _locker;
        private static List<double> FeatMin;
        private static List<double> FeatMax;
        private static List<double> InteracMin;
        private static List<double> InteracMax;
        private static List<double> VarMin;
        private static List<double> VarMax;
        private static int _finishedCounter = 0;
        public static double FeatDiff;
        public static double InteracDiff;
        public static double VariantDiff;
        public static double VariantPerc;
        public static double FeatPerc;
        public static double InteracPerc;
        public static InterGen Model;
        public static void Init(InterGen model)
        {
            Model = model;
            _fitnessValues = new List<Fitness>();
            FeatMax = new List<double>();
            FeatMin = new List<double>();
            InteracMax = new List<double>();
            InteracMin = new List<double>();
            VarMin = new List<double>();
            VarMax = new List<double>();
            _locker = new object();
        }

        public static void Reset()
        {
            _fitnessValues = new List<Fitness>();
            MultiplicationTime = 0;
            FitnessTime = 0;
            FeatMax = new List<double>();
            FeatMin = new List<double>();
            InteracMax = new List<double>();
            InteracMin = new List<double>();
            VarMin = new List<double>();
            VarMax = new List<double>();

        }


        public static bool AddFitn(MinMaxFitness fitn)
        {
            FeatMin.Add(fitn.FeatMin);
            FeatMax.Add(fitn.FeatMax);
            InteracMax.Add(fitn.InterMax);
            InteracMin.Add(fitn.InterMin);
            VarMax.Add(fitn.VarMax);
            VarMin.Add(fitn.VarMin);

            
            double NewFeatDiff = 0;
            double NewInteracDiff = 0;
            double NewVarDiff = 0;
            var featFinished = false;
            var interacFinished = false;
            var variantFinished = false;

            if (Model.Setting.InteracFitness)
            {
                NewInteracDiff = fitn.InterMax - fitn.InterMin;
                InteracPerc = InteracDiff/NewInteracDiff;
                if (Math.Abs(InteracPerc - 1) < Model.Setting.StopEarlyLevel / 100)
                {
                    interacFinished = true;
                }
            }
            else
            {
                interacFinished = true;
            }
            if (Model.Setting.FeatureFitness)
            {
                NewFeatDiff = fitn.FeatMax - fitn.FeatMin;
                if (NewFeatDiff != 0)
                {
                    FeatPerc = FeatDiff/NewFeatDiff;

                    if (Math.Abs(FeatPerc - 1) < Model.Setting.StopEarlyLevel/100)
                    {
                        featFinished = true;
                    }
                }
                else
                {
                    featFinished = true;
                }
            }
            else
            {
                featFinished = true;
            }
            if (Model.Setting.VariantFitness)
            {
                NewVarDiff = fitn.VarMax - fitn.VarMin;
                VariantPerc = VariantDiff/NewVarDiff;
                if (Math.Abs(VariantPerc - 1) < Model.Setting.StopEarlyLevel / 100)
                {
                    variantFinished = true;
                }
            }
            else
            {
                variantFinished = true;
            }

            VariantDiff = NewVarDiff;
            InteracDiff = NewInteracDiff;
            FeatDiff = NewFeatDiff;
            if (Model.Setting.DrawFitness && Counter % (Model.Setting.PlotStepSize / Model.Setting.PopulationSize) == 0)
            {
                if (Model.Setting.FeatureFitness)RIntegrator.PlotFitnSeries(FeatMax, FeatMin, "featFit.png");
                if (Model.Setting.InteracFitness) RIntegrator.PlotFitnSeries(InteracMax, InteracMin, "interacFit.png");
                if (Model.Setting.VariantFitness) RIntegrator.PlotFitnSeries(VarMax, VarMin, "variantFit.png");
                
            }
            Counter++;
            if (!Model.Setting.StopEarly) return false;
            if (featFinished && interacFinished && variantFinished)
            {
                _finishedCounter++;
                //Console.WriteLine("{0}, {1}", FeatPerc, VariantPerc);
                if (_finishedCounter == 3)
                {
                    //Console.WriteLine("Done");
                    return true;
                }
                
            }
            else
            {
                _finishedCounter = 0;
            }
            return false;

        }


        public static void Add(int eval, double featVal, double interacVal, double variantVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = eval, FeatVal = featVal, InteracVal = interacVal, Varval = variantVal });
            }
        }

        internal static void AddInterac(int foundAtEval, double interacVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = foundAtEval, InteracVal = interacVal });
            }
        }

        public static void WriteFiles(string logFile, string fileName = "Fitn")
        {

           
            if (logFile.Equals("\\"))
            {
                fileName = Environment.CurrentDirectory + Path.DirectorySeparatorChar + fileName;
            }
            else
            {
                fileName = logFile + Path.DirectorySeparatorChar + fileName;
            }
            if (!File.Exists(fileName)) Directory.CreateDirectory(fileName);

            _fitnessValues.Sort();
            List<double> feats = new List<double>();
            List<double> interacs = new List<double>();
            List<double> vars = new List<double>();
            foreach (var f in _fitnessValues)
            {
                feats.Add(f.FeatVal);
                if (f.InteracVal != 0) interacs.Add(f.InteracVal);
                if (f.Varval != 0) vars.Add(f.Varval);
            }
            
            File.Delete(fileName + "FeatFitn.png");
            File.Delete(fileName + "InteracFitn.png");
            File.Delete(fileName + "VariantFitn.png");
            File.WriteAllLines(fileName + "Feat.txt", feats.Select(d => d.ToString()).ToArray());
            if (interacs.Count > 0) File.WriteAllLines(fileName + "Interac.txt", interacs.Select(d => d.ToString()).ToArray());
            if (vars.Count > 0) File.WriteAllLines(fileName + "Variant.txt", vars.Select(d => d.ToString()).ToArray());
        }

        internal static void AddFeatInterac(int eval, double featVal, double interacVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = eval, FeatVal = featVal, InteracVal = interacVal });
            }
        }

        internal static void AddFeat(int eval, double featVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = eval, FeatVal = featVal });
            }
        }

        internal static void AddVar(int eval, double varVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = eval, Varval = varVal });
            }
        }

        internal static void AddFeatVar(int eval, double featVal, double varVal)
        {
            lock (_locker)
            {
                _fitnessValues.Add(new Fitness { Eval = eval, FeatVal = featVal, Varval = varVal });
            }
        }

        public static void AddCalcTime(long getFitnessTime)
        {
            lock (_locker)
            {
                FitnessTime += getFitnessTime;
            }
        }

        public static void AddMultiplicationTime(long elapsedMilliseconds)
        {
            lock (_locker)
            {
                MultiplicationTime += elapsedMilliseconds;
            }
        }
    }
}