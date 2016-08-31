using System;
using System.IO;
using System.Net.Configuration;


namespace InteracGenerator
{
    public class DistStore
    {
        private Distribution _selectedTd;
        public Distribution SelectedTargetDistribution
        {
            get { return _selectedTd; }
            set { _selectedTd = value; RIntegrator.PlotValues(_selectedTd, _model.Setting.FeatureAdjust); }
        }

        private Distribution _selectedFd;
        public Distribution SelectedFeatureDistribution
        {
            get { return _selectedFd; }
            set
            {
                _selectedFd = value;
                RIntegrator.PlotValues(_selectedFd, _model.Setting.FeatureAdjust);
            }
        }

        private Distribution _selectedId;
        public Distribution SelectedInteractionDistribution
        {
            get { return _selectedId; }
            set
            {
                _selectedId = value;
                RIntegrator.PlotValues(_selectedId, _model.Setting.FeatureAdjust);
            }
        }


        public Distribution[] ScaledInteractionDistributions { get; set; }
        public Distribution[] ScaledFeatureDistributions { get; set; }

        private Distribution _featureToStrap;
        public Distribution FeatureToStrap
        {
            get { return _featureToStrap;  }
            set {
                _featureToStrap = value;
                RIntegrator.PlotValues(_featureToStrap, _model.Setting.FeatureAdjust);
            }
        }

        private Distribution _interacToStrap;
        public Distribution InteracToStrap
        {
            get {return _interacToStrap;}
            set { _interacToStrap = value; RIntegrator.PlotValues(_interacToStrap, _model.Setting.FeatureAdjust); }
        }

        private readonly InterGen _model;

        public DistStore(InterGen model)
        {
            _model = model;
        }

        public void WriteDistributionToFile(Distribution.DistributionType type)
        {
            string fileName = "";
            Distribution toWrite = null;
            if (type == Distribution.DistributionType.Feature)
            {
                fileName = "scaledFeatures.txt";
                toWrite = SelectedFeatureDistribution;
            }
            if (type == Distribution.DistributionType.Interaction)
            {
                fileName = "scaledInteractions.txt";
                toWrite = SelectedInteractionDistribution;
            }
            if (type == Distribution.DistributionType.Variant)
            {
                fileName = "scaledVariants.txt";
                toWrite = SelectedTargetDistribution;
            }
            if (toWrite == null) return;
            File.WriteAllText(fileName, string.Empty);

            using (var sw = File.AppendText(fileName))
            {
                sw.WriteLine(toWrite.DisplayName);
                sw.WriteLine(toWrite.Name);
                sw.WriteLine(toWrite.DistType);
                for (var i = 0; i < toWrite.Values.Length - 1; i++)
                {
                    sw.Write(toWrite.Values[i] + ", ");
                }
                sw.WriteLine(toWrite.Values[toWrite.Values.Length - 1]);
            }
        }

        public Distribution LoadDistribution(string fileName)
        {
            if (!File.Exists(fileName)) return null;
            var file = File.ReadAllLines(fileName);
            if (file.Length == 0) return null;
            var displname = file[0];
            var name = file[1];
            var type = file[2];
            var distType =Distribution.DistributionType.Feature;

            if (type.Equals("Interaction")) distType = Distribution.DistributionType.Interaction;
            if (type.Equals("Variant")) distType = Distribution.DistributionType.Variant;
            var vals = file[3].Split(',');
            var values = new double[vals.Length];
            for (var i = 0; i < vals.Length; i++)
            {
                values[i] = Convert.ToDouble(vals[i]);
            }
            var selectedDistribution = new Distribution(values)
            {
                Name = name,
                DisplayName = displname,
                DistType = distType
            };

            if (distType == Distribution.DistributionType.Feature)
                if (values.Length == _model.Setting.NumberOfFeatures)
                {
                    SelectedFeatureDistribution = selectedDistribution;
                }
            if (distType == Distribution.DistributionType.Interaction)
            {
                if (values.Length == _model.Setting.NumberOfInteractions)
                {
                    SelectedInteractionDistribution = selectedDistribution;
                }
            }
            else
            {
                SelectedTargetDistribution = selectedDistribution;
            }
           
            return selectedDistribution;
        }
    }


   

}
