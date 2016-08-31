using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using InteracGenerator;
using IntergenDesktop.Forms;

namespace IntergenDesktop.UserControls
{
    public partial class VariantGenerationControl : UserControl, IStateModelLoader
    {
        private readonly InterGen _model;
        private readonly Button _nextButton;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nextButton"></param>
        public VariantGenerationControl(InterGen model, Button nextButton)
        {
            InitializeComponent();
            _nextButton = nextButton;
            _model = model;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = true;

            RandomModulo.Enabled = false;
            RandomSeconds.Enabled = false;
            RandomTreshold.Enabled = false;
            FWSeconds.Enabled = false;
            PWSeconds.Enabled = false;
            NFWSeconds.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
        }

 


        #region GenerationSettings

        /// <summary>
        /// Seconds for the Random Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomSeconds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var rndSeconds = Convert.ToInt32(RandomSeconds.Text);
                if (rndSeconds < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.RndSeconds = rndSeconds;
                label44.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label44.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Maximum number of random configs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomTreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var thresh = Convert.ToInt32(RandomTreshold.Text);
                if (thresh < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.RndTreshold = thresh;
                label46.ForeColor = Color.Black;

            }
            catch (Exception)
            {
                label46.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Random modulo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomModulo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var modulo = Convert.ToInt32(RandomModulo.Text);
                if (modulo < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.RndModulo = modulo;
                label47.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label47.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Time for Feature Wise Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FWSeconds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var fwSeconds = Convert.ToInt32(FWSeconds.Text);
                if (fwSeconds < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.FwSeconds = fwSeconds;
                label45.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label45.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Time for Negative Feature Wise Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NFWSeconds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var nfwSeconds = Convert.ToInt32(NFWSeconds.Text);
                if (nfwSeconds < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.NfwSeconds = nfwSeconds;
                label48.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label48.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Time for Pairwise Generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PWSeconds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var pwSeconds = Convert.ToInt32(PWSeconds.Text);
                if (pwSeconds < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.PwSeconds = pwSeconds;
                label49.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label49.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Amount of configs for each Size of Configurations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PseudoRND1Configs_TextChanged(object sender, EventArgs e)
        {

            try
            {
                var random1ConfigSize = Convert.ToInt32(PseudoRND1Configs.Text);
                if (random1ConfigSize < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.PseudoRndSize = random1ConfigSize;
                var configs = random1ConfigSize * _model.Setting.NumberOfFeatures;
                label51.Text = $"Configs at most: {configs}";
                label51.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label51.ForeColor = Color.Red;
            }
        }

        #endregion

        #region Checkboxes
        /// <summary>
        /// Random Checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.UseRnd = checkBox1.Checked;
            RandomModulo.Enabled = checkBox1.Checked;
            RandomSeconds.Enabled = checkBox1.Checked;
            RandomTreshold.Enabled = checkBox1.Checked;
        }

        /// <summary>
        /// Feature Wise 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            var check = checkBox2.Checked;
            _model.Setting.UseFw = check;
            FWSeconds.Enabled = check;
        }

        /// <summary>
        /// Negative Feature Wise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            var check = checkBox3.Checked;
            _model.Setting.UseNfw = check;
            NFWSeconds.Enabled = check;
        }

        /// <summary>
        /// Pair Wise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            var check = checkBox4.Checked;
            _model.Setting.UsePw = check;
            PWSeconds.Enabled = check;
        }

        /// <summary>
        /// Pseudo Random 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            var check = checkBox5.Checked;
            _model.Setting.UsePseudoRnd = check;
            PseudoRND1Configs.Enabled = check;
        }

        #endregion

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.LinearRandom = checkBox6.Checked;
            textBox1.Enabled = checkBox6.Checked;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var randLinearSize = Convert.ToInt32(textBox1.Text);
                if (randLinearSize < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                _model.Setting.LinearRndSize = randLinearSize;
                var numf = _model.Setting.NumberOfFeatures;
                var size = 0;
                var slope = randLinearSize/(float) numf;
                for (var i = 0; i < _model.Setting.NumberOfFeatures; i++)
                {
                    if (i < numf / 2.0f)
                    {
                        size += (int) (2 * slope * i + randLinearSize);
                    }
                    else
                    {
                        size +=(int) (-2 * slope * i) + 3* randLinearSize;
                    }
                }

                label1.Text = $"Configs at most: {size}";
                label1.ForeColor = Color.Black;
            }
            catch (Exception)
            {
                label1.ForeColor = Color.Red;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.QuadraticRandom = checkBox7.Checked;
            textBox2.Enabled = checkBox7.Checked;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var randQuadraticSize = Convert.ToDouble(textBox2.Text);
                if (randQuadraticSize < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                if (randQuadraticSize > 50)
                {
                    throw new WarningException("Must be between 0 and 50");
                }

                var size = 0;
                var nf = _model.Setting.NumberOfFeatures;

                for (var i = 0; i < nf; i++)
                {

                    var sl = randQuadraticSize * (i - nf / 2.0f);

                    var qu = -Math.Pow(sl, 2);
                    var yi = Math.Pow((nf/2.0f) * randQuadraticSize, 2);
                    size += (int)(qu + yi);
                }

                _model.Setting.QuadraticRandomScale = randQuadraticSize;
                label2.Text = $"Configs at most: {size}";
                label2.ForeColor = Color.Black;
            }
            catch (Exception excp)
            {
                label2.ForeColor = Color.Red;
                label2.Text = excp.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_model.Setting.PseudoRndSize != 0)
            {
                RIntegrator.PlotPseudoRandom(_model.Setting.PseudoRndSize, _model.Setting.NumberOfFeatures);
                var form = new VariantGenerationFunction();
                form.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_model.Setting.LinearRndSize != 0)
            {
                RIntegrator.PlotPseudoLinear(_model.Setting.LinearRndSize, _model.Setting.NumberOfFeatures);
                var form = new VariantGenerationFunction();
                form.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_model.Setting.QuadraticRandomScale != 0)
            {
                RIntegrator.PlotQuadraticRandom(_model.Setting.QuadraticRandomScale, _model.Setting.NumberOfFeatures);
                var form = new VariantGenerationFunction();
                form.Show();
            }
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var timeout = Convert.ToDouble(textBox3.Text);
                if (timeout <= 0)
                {
                    throw new WarningException("Timeout must be > 0");
                }
                _model.Setting.SolverTimeout = timeout;
                label3.ForeColor = Color.Black;
                label3.Text = @"Solver timeout in seconds";
            }
            catch (Exception ex)
            {
                label3.ForeColor = Color.Red;
                label3.Text = ex.Message;
            }
        }

        public void LoadSettings()
        {
            if (_model.Setting.LinearRandom)
            {
                checkBox6.Checked = true;
                textBox1.Enabled = true;
                textBox1.Text = _model.Setting.LinearRndSize.ToString();
                if (_model.Setting.LinearRndSize > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.UsePseudoRnd)
            {
                checkBox5.Checked = true;
                PseudoRND1Configs.Enabled = true;
                PseudoRND1Configs.Text = _model.Setting.PseudoRndSize.ToString();
                if (_model.Setting.PseudoRndSize > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.UsePw)
            {
                checkBox4.Checked = true;
                PWSeconds.Enabled = true;
                PWSeconds.Text = _model.Setting.PwSeconds.ToString();
                if (_model.Setting.PwSeconds > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.UseFw)
            {
                checkBox2.Checked = true;
                FWSeconds.Enabled = true;
                FWSeconds.Text = _model.Setting.FwSeconds.ToString();
                if (_model.Setting.FwSeconds > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.UseNfw)
            {
                checkBox3.Checked = true;
                NFWSeconds.Enabled = true;
                NFWSeconds.Text = _model.Setting.NfwSeconds.ToString();
                if (_model.Setting.NfwSeconds > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.UseRnd)
            {
                checkBox1.Checked = true;
                tableLayoutPanel12.Enabled = true;
                RandomSeconds.Text = _model.Setting.RndSeconds.ToString();
                RandomModulo.Text = _model.Setting.RndModulo.ToString();
                RandomTreshold.Text = _model.Setting.RndTreshold.ToString();
                if (_model.Setting.RndSeconds > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.QuadraticRandom)
            {
                checkBox7.Checked = true;
                textBox2.Enabled = true;
                textBox2.Text = _model.Setting.QuadraticRandomScale.ToString(CultureInfo.InvariantCulture);
                if (_model.Setting.QuadraticRandomScale > 0) _nextButton.Enabled = true;
            }
            if (_model.Setting.SolverTimeout > 0)
            {
                textBox3.Text = _model.Setting.SolverTimeout.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}