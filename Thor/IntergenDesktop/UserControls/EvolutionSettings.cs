using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InteracGenerator;
using IntergenDesktop.Forms;

namespace IntergenDesktop.UserControls
{
    public partial class EvolutionSettings : UserControl, IStateModelLoader
    {

        private readonly InterGen _model;
        private readonly Button _nextButton;

        public EvolutionSettings(InterGen model, Button nextButton)
        {
            InitializeComponent();
            _model = model;
            _nextButton = nextButton;
            _nextButton.Enabled = false;
            _model.Setting.Parallel = true;
            textBox1.Text = @"5000";
            _model.Setting.LogFolder = Environment.CurrentDirectory;
            label52.Text = Environment.CurrentDirectory;
            textBox2.Text = @"50";
            model.Setting.StopEarlyLevel = 1;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseKs = radioButton2.Checked;
            _nextButton.Enabled = true;
            button1.Enabled = !radioButton2.Checked;
            button2.Enabled = !radioButton2.Checked;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseChiSquared = radioButton4.Checked;
            _nextButton.Enabled = true;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.ScaleToGlobalMinMax = checkBox12.Checked;
            _nextButton.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseCmv = radioButton1.Checked;
            _nextButton.Enabled = true;
            button1.Enabled = !radioButton1.Checked;
            button2.Enabled = !radioButton1.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseEuclidean = radioButton3.Checked;
            _nextButton.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var maxEvaluations = Convert.ToInt32(textBox1.Text);
                if (maxEvaluations <= 0)
                {
                    throw new WarningException("Must be > 0");
                }
                _model.Setting.MaxEvaluations = maxEvaluations;
                label1.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                Console.WriteLine(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.Parallel = checkBox1.Checked;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.Logging = checkBox6.Checked;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

           
            _model.Setting.LogFolder = folderBrowserDialog1.SelectedPath;
            label52.Text = _model.Setting.LogFolder;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var featureBins = new DynamicBins(_model.FeaturesDynamicHist, _model);
            featureBins.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var varBins = new DynamicBins(_model.VariantDynamicHist, _model);
            varBins.ShowDialog();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.EuclAndCmv = radioButton5.Checked;
            _nextButton.Enabled = true;
            
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.ChiAndCmv = radioButton6.Checked;
            _nextButton.Enabled = true;
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var popSize = Convert.ToInt32(textBox2.Text);
                if (popSize <= 0)
                {
                    throw new WarningException("Must be > 0");
                }
                _model.Setting.PopulationSize = popSize;
                label1.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                Console.WriteLine(ex.Message);
            }
        }

        private void folderBrowserDialog2_HelpRequest(object sender, EventArgs e)
        {

        }

        public void LoadSettings()
        {
            if (_model.Setting.Logging)
            {
                label52.Text = _model.Setting.LogFolder;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.StopEarly = checkBox2.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var val = (double)numericUpDown1.Value;
            _model.Setting.StopEarlyLevel = val;
        }
    }
}
