using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace InteracGenerator
{
    public class Settings : INotifyPropertyChanged
    {
        private int _numberOfFeatures;
        public int NumberOfFeatures
        {
            get { return _numberOfFeatures; }
            set
            {
                if (value == _numberOfFeatures) return;
                _numberOfFeatures = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public int SelectedFeature { get; set; }

        [JsonIgnore]
        public int SelectedInteraction { get; set; }

        public bool QuadraticRandom;
        public double QuadraticRandomScale;
        public bool Dimacs;
        public List<string> DimacsFeatureList;
        public string FileName;

        private int _numberOfInteractions;
        public bool LoadedInteractions;
        public double SolverTimeout;
        public bool UseInitialFv;
        public double FeatureAdjust = 0.3;

        public int NumberOfInteractions
        {
            get { return _numberOfInteractions; }
            set { _numberOfInteractions = value; NotifyPropertyChanged(); }
        }

        public string LogFolder { get; set; }
        public bool Logging { get; set; }
        public bool Parallel { get; set; }
        public double StandardDeviation { get; set; }
        public double Mean { get; set; }
        public double UnifMin { get; set; }
        public double UnifMax { get; set; }

        public List<double> InteractionOrderPercent { get; set; }

        public bool UseKs { get; set; }
        public bool UseCmv { get; set; }
        public bool UseChiSquared { get; set; }
        public bool UseEuclidean { get; set; }
        public bool EuclAndCmv { get; set; }
        public bool ChiAndCmv { get; set; }

        public int PwSeconds { get; set; }
        public int FwSeconds { get; set; }
        public int NfwSeconds { get; set; }
        public int RndSeconds { get; set; }
        public int RndTreshold { get; set; }
        public int RndModulo { get; set; }
        public int PseudoRndSize { get; set; }
        public int LinearRndSize { get; set; }

        public bool UseFw { get; set; }
        public bool UsePw { get; set; }
        public bool UseNfw { get; set; }
        public bool UseRnd { get; set; }
        public bool UsePseudoRnd { get; set; }
        public bool LinearRandom { get; set; }
        public bool DrawHistogram { get; set; }
        public bool DrawDensity { get; set; }

        public int MaxEvaluations { get; set; }
        public bool ScaleToGlobalMinMax { get; set; }
        public int PopulationSize { get; set; }
        public bool NoVariantCalculation { get; set; }
        public double InteracMarginValue { get; set; }

        public bool FeatureFitness { get; set; }
        public bool InteracFitness { get; set; }
        public bool VariantFitness { get; set; }

        public byte[,] InteracMatrix { get; set; }
        public byte[,] FeatureMatrix { get; set; }
        public Distribution FeatTarget { get; set; }
        public Distribution InteracTarget { get; set; }
        public Distribution VariantTarget { get; set; }
        public bool OnlyVariantFitness { get; set; }

        public double FeatureScaleMin { get; set; }
        public double FeatureScaleMax { get; set; }
        public double InteractionScaleMin { get; set; }
        public double InteractionScaleMax { get; set; }

        public int DrawAngle { get; set; }
        public int PlotStepSize { get; set; }
        public bool DrawFitness { get; set; }
        public bool StopEarly { get; set; }
        public double StopEarlyLevel { get; set; }

        public Settings()
        {
            InteractionOrderPercent = new List<double>();
            Mean = double.NaN;
            StandardDeviation = double.NaN;
            UnifMin = double.NaN;
            UnifMax = double.NaN;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void WriteSetting(string fileName = "setting.json")
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(fileName, json);
        }
    }
}
