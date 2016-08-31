using System;
using System.Windows.Forms;
using InteracGenerator;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using InteracGenerator.Problem;

namespace IntergenDesktop.UserControls
{
    public partial class Evolution : UserControl
    {
        private readonly InterGen _model;
        private int _ticks;
        private int _currMax;
        private bool _doneGenerating;
        private readonly Button _nextButton;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nextButton"></param>
        public Evolution(InterGen model, Button nextButton)
        {
            InitializeComponent();


            _nextButton = nextButton;
            _model = model;
            checkBox1.Checked = true;
            model.Setting.DrawDensity = true;
            model.Setting.PlotStepSize = 500;
            numericUpDown2.Increment = _model.Setting.PopulationSize;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker3.WorkerReportsProgress = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            numericUpDown1.Value = new decimal(0.5);
           
            _nextButton.Enabled = false;
        }

        #region Interactions
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {



            var mdl = e.Argument as InterGen;
            mdl.Setting.WriteSetting();
            var bw = sender as BackgroundWorker;
            mdl?.CreateInteractions(e, bw);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label7.Text = $"Verarbeitet: {e.UserState}";
            if (e.ProgressPercentage > 100)
                progressBar1.Value = 100;
            else
                progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label7.Text = _model.Setting.NumberOfInteractions == 0 ? @"Done - No interactions selected" : @"Done";

            label7.ForeColor = Color.Green;
            progressBar1.Value = 100;
            //TODO Check for success
            timer1.Start();
            backgroundWorker2.RunWorkerAsync(_model);
        }
        #endregion

        #region Generate Variants
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var mdl = e.Argument as InterGen;
            var bw = sender as BackgroundWorker;
            mdl?.CreateVariants(bw, e);
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {

                _doneGenerating = true;
            }
            progressBar2.Value = e.ProgressPercentage;

            if ((int)e.UserState > _currMax)
            {
                _currMax = (int)e.UserState;
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label4.Text = $"Done: {_currMax} Variants";
            label4.ForeColor = Color.Green;
            timer1.Stop();
            progressBar2.Value = 100;
            backgroundWorker3.RunWorkerAsync(_model);


        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            _ticks++;
            if (_doneGenerating)
            {
                label4.Text = $"Calculating Feature/Variant Matrix: {_ticks}s";
                return;
            }
            label4.Text = $"Generating Variants: {_ticks}s";
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            var mdl = e.Argument as InterGen;
            var bw = sender as BackgroundWorker;
            mdl?.StartEvolution(bw, e);
        }

        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            progressBar3.Value = e.ProgressPercentage;
            var curDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            if (_model.Setting.DrawDensity || _model.Setting.DrawHistogram)
            {
                if (!_model.Setting.NoVariantCalculation)
                {
                    pictureBox1.ImageLocation = curDir + @"variantComp.png";
                    pictureBox1.LoadAsync();
                }

                pictureBox2.ImageLocation = curDir + @"featuresComp.png";
                pictureBox2.LoadAsync();

                if (_model.Setting.NumberOfInteractions > 0)
                {
                    pictureBox3.ImageLocation = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                                @"interacComp.png";
                    pictureBox3.LoadAsync();
                }
            }
            else
            {
                pictureBox1.ImageLocation = null;
                pictureBox2.ImageLocation = null;
                pictureBox3.ImageLocation = null;
            }

            var state = (UserProgress)e.UserState;
            if (state == null) return;
            if (_model.Setting.ChiAndCmv || _model.Setting.EuclAndCmv)
            {
                label5.Text =
                    $"V: {state.VariantP} F: {state.FeatureP} F-CmV: {state.FeatureCmV} V-CmV: {state.VariantCmV}";
            }
            else
            {
                label10.Text = state.InteracP == 0 ? "No Interactions Fitness" : $"Interac: {state.InteracP}";
                label6.Text = state.FeatureP == 0 ? "No Feature Fitness" : $"Feature: {state.FeatureP}";
                label5.Text = state.VariantP == 0 ? "No Variant Fitness" : $"Variant: {state.VariantP}";
            }
            label9.Text = $"{state.EvolutionStep} / {_model.Setting.MaxEvaluations}";
            if (_model.Setting.DrawFitness)
            {
                if (_model.Setting.FeatureFitness)
                {
                    pictureBox4.ImageLocation = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                                @"featFit.png";
                    label13.Text = $"PercentChange: {(FitnessTracker.FeatPerc - 1)*100}%";
                }
                if (_model.Setting.VariantFitness)
                {
                    pictureBox6.ImageLocation = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                                @"variantFit.png";
                    label12.Text = $"PercentChange: {(FitnessTracker.VariantPerc - 1)*100}%";
                }
                if (_model.Setting.InteracFitness)
                {
                    pictureBox5.ImageLocation = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +
                                                @"interacFit.png";
                    label14.Text = $"PercentChange: {(FitnessTracker.InteracPerc - 1)*100}%";
                }
            }
            else
            {
                pictureBox4.ImageLocation = null;
                pictureBox6.ImageLocation = null;
                pictureBox5.ImageLocation = null;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label9.Text = @"Done";
            label9.ForeColor = Color.Green;
            _nextButton.Enabled = true;
            button1.Enabled = true;
            button1.Text = @"Restart Evolution";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.DrawDensity = checkBox1.Checked;
            checkBox2.Checked = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.DrawHistogram = checkBox2.Checked;
            checkBox1.Checked = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _model.Setting.FeatureAdjust = (double)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(_model);
            button1.Enabled = false;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            _model.Setting.PlotStepSize = (int)numericUpDown2.Value;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.DrawFitness = checkBox3.Checked;
        }
    }
}
