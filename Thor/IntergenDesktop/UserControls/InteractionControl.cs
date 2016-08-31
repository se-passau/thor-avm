using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InteracGenerator;

namespace IntergenDesktop.UserControls
{
    public partial class InteractionControl : UserControl
    {

        private readonly InterGen _model;
        private bool _afterBootstrap;
        private readonly Button _nextButton;

        public InteractionControl(InterGen model, Button nextButton)
        {
            InitializeComponent();
            _nextButton = nextButton;
            _model = model;

            SelectedInteractionProperty.Items.Add("binarySize");
            SelectedInteractionProperty.Items.Add("performance");
            SelectedInteractionProperty.Items.Add("mainmemory");
            SelectedInteractionProperty.Items.Add("Random functions");
            SelectedInteractionProperty.SelectedIndex = 0;
            BootstrapInteractions.Enabled = false;
            _nextButton.Enabled = false;
            button2.Enabled = false;
            var interac = new Binding("Text", _model.Setting, "NumberOfInteractions");
            interac.Format += (sender, args) => {
                args.Value = "Number of Interactions: " + args.Value;
            };
            NumberOfInteractions.DataBindings.Add(interac);

            //pvalue 1 binding
            var pVal1 = new Binding("Text", _model, "IPVal1");
            pVal1.Format += (sender, e) => {
                e.Value = @"Test-Value - " + _model.FeatureTestMethod + ": " + e.Value;
            };
            pValue1Label.DataBindings.Add(pVal1);

            //Data Binding for the second p-Value 
            var pVal2 = new Binding("Text", _model, "IPVal2");
            pVal2.Format += (sender, e) => {
                e.Value = @"Test-Value - " + _model.FeatureTestMethod + ": " + e.Value;
            };
            pValue2Label.DataBindings.Add(pVal2);

            SelectFirst.Enabled = false;
            SelectSecond.Enabled = false;
        }


        /// <summary>
        /// Selects the non-functional property for the interactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedInteractionProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = (string)SelectedInteractionProperty.SelectedItem;

            _model.LoadFeaturesForProperty(selection, Distribution.DistributionType.Interaction);
            SelectedInteractionValues.ResetText();
            SelectedInteractionValues.Items.Clear();
            BootstrapInteractions.Enabled = selection != "Random functions";
            foreach (var d in _model.AvailableDistributions.Where(d => d.DistType == Distribution.DistributionType.Interaction))
            {
                SelectedInteractionValues.Items.Add(d);
            }
        }


        /// <summary>
        /// selects the real-world interaction distribution for a property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedInteractionValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            BootstrapInteractions.Enabled = true;

            if (CheckForRandomFunction(SelectedInteractionValues.SelectedItem, Distribution.DistributionType.Interaction))
            {
                pictureBox2.ImageLocation = _model.DStore.ScaledInteractionDistributions[0].ImagePath;
                BootstrapInteractions.Text = @"Create another";
                pictureBox4.ImageLocation = null;
                return;
            }
            BootstrapInteractions.Text = @"Bootstrap Values";
            var dist = (Distribution)SelectedInteractionValues.SelectedItem;
            _model.DStore.InteracToStrap = dist;
            pictureBox2.ImageLocation = dist.ImagePath;
            pictureBox4.ImageLocation = null;

           
            SelectSecond.Enabled = false;
            SelectFirst.Enabled = false;
          
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
        /// Bootstrap the interactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BootstrapInteractions_Click(object sender, EventArgs e)
        {
            //Should not happen at this point! was catched by previous flow
            if (_model.Setting.NumberOfInteractions == 0)
            {
                NumberOfInteractions.Text = @"Select Number of Interactions first";
                return;
            }
            Distribution[] results;
            if (SelectedInteractionValues.SelectedItem == null)
                results = _model.ScaleDistribution(_model.DStore.SelectedInteractionDistribution, _model.Setting.NumberOfInteractions, 2);
            else
            {
                switch (((Distribution)SelectedInteractionValues.SelectedItem).DisplayName)
                {
                    case "Normal":
                        results = _model.CreateNormalDist(2, Distribution.DistributionType.Interaction);
                        pValue1Label.Text = @"No test yet";
                        pValue2Label.Text = @"No test yet";
                        break;
                    case "Uniform":
                        results = _model.CreateUnifDist(2, Distribution.DistributionType.Interaction);
                        pValue1Label.Text = @"No test yet";
                        pValue2Label.Text = @"No test yet";
                        break;
                    default:
                        results = _model.ScaleDistribution(_model.DStore.InteracToStrap, _model.Setting.NumberOfInteractions, 2);
                        break;
                }
            }
            //var results = _model.ScaleDistribution(_model.DStore.SelectedInteractionDistribution, _model.Setting.NumberOfInteractions, 2);

            pictureBox2.ImageLocation = results[0].ImagePath;
            pictureBox4.ImageLocation = results[1].ImagePath;

            SelectSecond.Enabled = true;
            SelectFirst.Text = @"Select";
            SelectSecond.Text = @"Select";
            SelectFirst.Enabled = true;
            _nextButton.Enabled = false;
            button2.Enabled = false;
            _afterBootstrap = true;
        }

        /// <summary>
        /// Selects the first interaction distribution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFirst_Click(object sender, EventArgs e)
        {
            if (!_afterBootstrap) return;

            _model.Setting.SelectedInteraction = 0;
            _model.DStore.SelectedInteractionDistribution = _model.DStore.ScaledInteractionDistributions[0];
            SelectFirst.Enabled = false;
            SelectFirst.Text = @"Selected";
            SelectSecond.Enabled = true;
            SelectSecond.Text = @"Select";
            _nextButton.Enabled = true;
            label1.Text = @"Current Min: " + _model.DStore.SelectedInteractionDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedInteractionDistribution.Values.Max();
            button2.Enabled = true;
        }

        /// <summary>
        /// Selects the second interaction distribution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSecond_Click(object sender, EventArgs e)
        {
            
            if (!_afterBootstrap) return;

            _model.Setting.SelectedInteraction = 1;
            _model.DStore.SelectedInteractionDistribution = _model.DStore.ScaledInteractionDistributions[1];
            SelectFirst.Enabled = true;
            SelectSecond.Enabled = false;
            SelectSecond.Text = @"Selected";
            SelectFirst.Text = @"Select";
            _nextButton.Enabled = true;
            label1.Text = @"Current Min: " + _model.DStore.SelectedInteractionDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedInteractionDistribution.Values.Max();
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _model.DStore.WriteDistributionToFile(Distribution.DistributionType.Interaction);
        }

        private void button5_Click(object sender, EventArgs e)
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
                        if (double.TryParse(srVal, out val))
                            doublelist.Add(val);
                    }
                }
                var featdist = new Distribution(doublelist.ToArray())
                {
                    Name = "interacFromFile",
                    DisplayName = "interac",
                    DistType = Distribution.DistributionType.Interaction
                };

                _model.DStore.SelectedInteractionDistribution = featdist;
                pictureBox2.ImageLocation = Environment.CurrentDirectory + @"/iDist.png";
                if (featdist.Values.Length != _model.Setting.NumberOfInteractions)
                {
                    _nextButton.Enabled = false;
                    BootstrapInteractions.Enabled = true;
                }
                else
                {
                    _nextButton.Enabled = true;
                    BootstrapInteractions.Enabled = false;
                }
                SelectFirst.Text = @"Selected";
            }
            catch (IOException)
            {
            }
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
          
            try
            {
                _model.Setting.InteractionScaleMin = Convert.ToDouble(textBox1.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        
    }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
            try
            {
                _model.Setting.InteractionScaleMax = Convert.ToDouble(textBox2.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "scaledInteractions.txt"))
            {
                MessageBox.Show(@"scaledInteractions.txt - File does not exist");
                return;
            }
            var file = File.ReadAllLines(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "scaledInteractions.txt");
            if (file.Length == 0)
            {
                MessageBox.Show(@"scaledInteractions.txt - File is empty");
                return;
            }
            var dist =_model.DStore.LoadDistribution("scaledInteractions.txt");
            if (dist.Values.Length != _model.Setting.NumberOfInteractions)
            {
                MessageBox.Show(@"scaledInteractions.txt - Wrong amount of Interactions");
                return;
            }
            pictureBox2.LoadAsync(Environment.CurrentDirectory + @"/fDist.png");
            pictureBox4.ImageLocation = null;
            _nextButton.Enabled = true;
            label1.Text = @"Current Min: " + _model.DStore.SelectedInteractionDistribution.Values.Min();
            label3.Text = @"Current Max: " + _model.DStore.SelectedInteractionDistribution.Values.Max();
        }

        private void tableLayoutPanel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
