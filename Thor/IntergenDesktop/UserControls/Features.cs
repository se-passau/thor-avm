using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using InteracGenerator;

namespace IntergenDesktop.UserControls
{
    public partial class Features : UserControl, IStateModelLoader
    {
        private readonly InterGen _model;
        private bool AfterBootstrap { get; set; }
        private readonly Button _nextButton;
        public bool OwnValues { get; set; }

        /// <summary>
        /// Fills the available NFP-properties, and adds Bindings to Labels
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nextButton"></param>
        public Features(InterGen model, Button nextButton)
        {
            _model = model;
            _nextButton = nextButton;
            InitializeComponent();
          
            BootstrapClick.Enabled = false;

            //Add selectable non-functional properties
            FeatureNFPBox.Items.Add("binarySize");
            FeatureNFPBox.Items.Add("performance");
            FeatureNFPBox.Items.Add("mainmemory");
            FeatureNFPBox.Items.Add("Random functions");
            FeatureNFPBox.SelectedIndex = 0;

            //NumberOfFeatureLabel is binded to model.NumberOfFeatures
            var feature = new Binding("Text", _model.Setting, "NumberOfFeatures");
            feature.Format += NumberFeaturesFormat;
            NumberOfFeaturesLabel.DataBindings.Add(feature);
            _model.Setting.FeatureAdjust = 0.5;
            //Data Binding for the P-Values of the selected Tests
            var pVal1 = new Binding("Text", _model, "FPVal1");
            pVal1.Format += (sender, e) => {
                var val = e.Value;
                e.Value = @"Test-Value - " + _model.FeatureTestMethod + ": " + val;
            };

            //Data Binding for the second p-Value 
            var pVal2 = new Binding("Text", _model, "FPVal2");
            pVal2.Format += (sender, e) => {
                var val = e.Value;
                e.Value = @"Test-Value - " + _model.FeatureTestMethod + ": " + val;
            };

            FirstPLabel.DataBindings.Add(pVal1);
            SecondPLabel.DataBindings.Add(pVal2);
            nextButton.Enabled = false;
            button1.Enabled = false;
        }

        /// <summary>
        /// Formatting for the Feature Label Binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void NumberFeaturesFormat(object sender, ConvertEventArgs e)
        {
            var value = e.Value;
            e.Value = @"Anzahl der Features: " + value;
        }

        /// <summary>
        /// Changed the selected real-world system distribution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RealFeatureValuesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BootstrapClick.Enabled = true;
            OwnValues = false;
            label1.Text = @"Current Min: ";
            label3.Text = @"Current Max: ";
            if (CheckForRandomFunction(RealFeatureValuesBox.SelectedItem, Distribution.DistributionType.Feature))
            {
                BootstrapClick.Text = @"Create another";
                pictureBox1.ImageLocation = _model.DStore.ScaledFeatureDistributions[0].ImagePath;
                pictureBox3.ImageLocation = null;
                
                return;
            }
            BootstrapClick.Text = @"Bootstrap Values";
            var dist = (Distribution)RealFeatureValuesBox.SelectedItem;
            //_model.DStore.SelectedFeatureDistribution = dist;
            _model.DStore.FeatureToStrap = dist;
            pictureBox1.ImageLocation = Environment.CurrentDirectory +  @"/fDist.png";
            pictureBox3.ImageLocation = null;
           
        }


        /// <summary>
        /// Checks if a probability function was selected
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckForRandomFunction(object selected, Distribution.DistributionType type)
        {
            switch (((Distribution)selected).DisplayName)
            {
                case "Normal":
                    var normal = new NormalDistGen(_model, type);
                    normal.ShowDialog();
                    return true;
                case "Uniform":
                    var unif = new UniformDistGen(_model, type);
                    unif.ShowDialog();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Selects the desired Non-functional property value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeatureNFPBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = (string)FeatureNFPBox.SelectedItem;
            BootstrapClick.Enabled = selection != "Random functions";
            _model.LoadFeaturesForProperty(selection, Distribution.DistributionType.Feature);
            RealFeatureValuesBox.ResetText();
            RealFeatureValuesBox.Items.Clear();

            foreach (var d in _model.AvailableDistributions.Where(d => d.DistType == Distribution.DistributionType.Feature))
            {
                RealFeatureValuesBox.Items.Add(d);
            }
            BootstrapClick.Enabled = false;
            SelectSecondDist.Enabled = false;
            SelectFirstDist.Enabled = false;
        }

        /// <summary>
        /// Click on Bootstrap:  bootstrap selected distribution, show two solutions at once
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootstrapClick_Click(object sender, EventArgs e)
        {
            AfterBootstrap = true;
            _nextButton.Enabled = false;
            button1.Enabled = false;
            if (_model.Setting.NumberOfFeatures == 0)
            {
                NumberOfFeaturesLabel.Text = @"Select Number of Features first";
                return;
            }
            Distribution[] results;
            if (OwnValues)
            {
                results = _model.ScaleDistribution(_model.DStore.SelectedFeatureDistribution, _model.Setting.NumberOfFeatures, 2);

                pictureBox1.ImageLocation = results[0].ImagePath;
                pictureBox3.ImageLocation = results[1].ImagePath;

                _model.Setting.SelectedFeature = 0;

                SelectFirstDist.Enabled = true;
                SelectSecondDist.Enabled = true;
                return;
            }
            switch (((Distribution)RealFeatureValuesBox.SelectedItem).DisplayName)
            {
                case "Normal":
                    results = _model.CreateNormalDist(2, Distribution.DistributionType.Feature);
                    FirstPLabel.Text = @"No test yet";
                    SecondPLabel.Text = @"No test yet";
                    break;
                case "Uniform":
                    results = _model.CreateUnifDist(2, Distribution.DistributionType.Feature);
                    FirstPLabel.Text = @"No test yet";
                    SecondPLabel.Text = @"No test yet";
                    break;
                default:
                    NumberOfFeaturesLabel.Text = @"Scaling to " + _model.Setting.NumberOfFeatures;
                    results = _model.ScaleDistribution(_model.DStore.FeatureToStrap, _model.Setting.NumberOfFeatures, 2);
                    break;
            }
           
            pictureBox1.ImageLocation = results[0].ImagePath;
            pictureBox3.ImageLocation = results[1].ImagePath;

            //_model.Setting.SelectedFeature = 0;

            SelectFirstDist.Enabled = true;
            SelectFirstDist.Text = @"Select";
            SelectSecondDist.Text = @"Select";
            SelectSecondDist.Enabled = true;
        }

        
        /// <summary>
        /// Selected the first Distribution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFirst_Click(object sender, EventArgs e)
        {
            if (!AfterBootstrap) return;

            _model.Setting.SelectedFeature = 0;
            _model.DStore.SelectedFeatureDistribution = _model.DStore.ScaledFeatureDistributions[0];
            label1.Text = @"Current Min: " + _model.DStore.SelectedFeatureDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedFeatureDistribution.Values.Max();
            SelectSecondDist.Enabled = true;
            SelectSecondDist.Text = @"Select";
            pictureBox3.Enabled = true;
            pictureBox1.Enabled = false;
            SelectFirstDist.Enabled = false;
            SelectFirstDist.Text = @"Selected";
            _nextButton.Enabled = true;
            button1.Enabled = true;

        }

        /// <summary>
        /// Selected the second Distribution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSecond_Click(object sender, EventArgs e)
        {
            if (!AfterBootstrap) return;

            _model.Setting.SelectedFeature = 1;
            _model.DStore.SelectedFeatureDistribution = _model.DStore.ScaledFeatureDistributions[1];
            label1.Text = @"Current Min: " + _model.DStore.SelectedFeatureDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedFeatureDistribution.Values.Max();
            SelectSecondDist.Enabled = false;
            SelectFirstDist.Enabled = true;
            SelectFirstDist.Text = @"Select";
            pictureBox3.Enabled = false;
            pictureBox1.Enabled = true;
            SelectSecondDist.Text = @"Selected";
            _nextButton.Enabled = true;
            button1.Enabled = true;
        }

        /* <summary>
        /// Go the the next step:  ->  Interactions or Target Variants
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextStep_Click(object sender, EventArgs e)
        {
            //do not show interactions when u didnt select them
            if (_model.NumberOfInteractions != 0)
            {
                var interac = new Interaction(_model);
                Close();
                interac.Show();
            }
            else
            {
                var target = new TargetVariant(_model);
                Close();
                target.Show();
            }
        } */


        private void button1_Click(object sender, EventArgs e)
        {
            _model.DStore.WriteDistributionToFile(Distribution.DistributionType.Feature);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "scaledFeatures.txt"))
            {
                MessageBox.Show(@"scaledFeatures.txt - File does not exist");
                return;
            }
            var file = File.ReadAllLines(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "scaledFeatures.txt");
            if (file.Length == 0)
            {
                MessageBox.Show(@"scaledFeatures.txt - File is empty");
                return;
            }
            _model.DStore.LoadDistribution("scaledFeatures.txt");
            pictureBox1.LoadAsync(Environment.CurrentDirectory + @"/fDist.png");
            pictureBox3.ImageLocation = null;
            _nextButton.Enabled = true;
            label1.Text = @"Current Min: " + _model.DStore.SelectedFeatureDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedFeatureDistribution.Values.Max();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseInitialFv = checkBox1.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _model.Setting.FeatureAdjust = (double)numericUpDown1.Value;
            var results =_model.ScaleDistribution(_model.DStore.SelectedFeatureDistribution, _model.Setting.NumberOfFeatures, 2);
            pictureBox1.ImageLocation = results[0].ImagePath;
            pictureBox3.ImageLocation = results[1].ImagePath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            try
            {
                var text = File.ReadAllText(openFileDialog1.FileName);
                var doublelist = new List<double>();

                var srValues = text.Split(null);
                foreach (var srVal in srValues)
                {
                    if (!string.IsNullOrWhiteSpace(srVal))
                    {
                        double val = 0;
                        if(double.TryParse(srVal, out val))
                            doublelist.Add(val);
                    }
                }
                var featdist = new Distribution(doublelist.ToArray())
                {
                    Name = "featFromFile",
                    DisplayName = "feat",
                    DistType = Distribution.DistributionType.Feature
                };

                _model.DStore.SelectedFeatureDistribution = featdist;
                pictureBox1.ImageLocation = Environment.CurrentDirectory + @"/fDist.png";
                SelectSecondDist.Text = @"Selected";
                if (featdist.Values.Length != _model.Setting.NumberOfFeatures)
                {
                    _nextButton.Enabled = false;
                    BootstrapClick.Enabled = true;
                }
                else
                {
                    _nextButton.Enabled = true;
                    BootstrapClick.Enabled = false;
                }

                OwnValues = true;
            }
            catch (IOException)
            {
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _model.Setting.FeatureScaleMin = Convert.ToDouble(textBox1.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _model.Setting.FeatureScaleMax = Convert.ToDouble(textBox2.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public void LoadSettings()
        {
            Console.WriteLine("Loading Settings");
            textBox2.Text = _model.Setting.FeatureScaleMax.ToString(CultureInfo.InvariantCulture);
        }
    }
}
