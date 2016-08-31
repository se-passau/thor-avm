using System;
using System.Linq;
using System.Windows.Forms;
using InteracGenerator;

namespace IntergenDesktop.UserControls
{
    public partial class TargetVariant : UserControl
    {
        private readonly Button _nextButton;
        private readonly InterGen _model ;
        public TargetVariant(InterGen model, Button nextButton)
        {
            InitializeComponent();
            _model = model;
            _nextButton = nextButton;
            _nextButton.Enabled = false;
            TargetPropertyBox.Items.Add("binarySize");
            TargetPropertyBox.Items.Add("performance");
            TargetPropertyBox.Items.Add("mainmemory");
            TargetPropertyBox.Items.Add("Random functions");
            TargetPropertyBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Select target variant non-functional property 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetPropertyBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            
            pictureBox5.ImageLocation = null;

            _model.LoadTargetDistributionForProperty((string)TargetPropertyBox.SelectedItem);
            TargetValueBox.ResetText();
            TargetValueBox.Items.Clear();

            foreach (var d in _model.AvailableDistributions.Where(d => d.DistType == Distribution.DistributionType.Variant))
            {
                TargetValueBox.Items.Add(d);
            }
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
        /// Selects the variant target distribution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetValueBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dist = (Distribution) TargetValueBox.SelectedItem;

            if (CheckForRandomFunction(TargetValueBox.SelectedItem, Distribution.DistributionType.Variant))
            {
                pictureBox5.ImageLocation = _model.DStore.SelectedTargetDistribution.ImagePath;
                _nextButton.Enabled = true;
                return;
            }
            _model.DStore.SelectedTargetDistribution = dist;
            pictureBox5.ImageLocation = dist.ImagePath;

            _nextButton.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _model.DStore.WriteDistributionToFile(Distribution.DistributionType.Variant);
        }
    }
}
