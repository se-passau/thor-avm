using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InteracGenerator;

namespace IntergenDesktop.Forms
{
    public partial class DynamicBins : Form
    {
        private readonly DynamicHist _dynHist;
        private readonly InterGen _model;

        public DynamicBins(DynamicHist dynamicHist, InterGen model)
        {
            InitializeComponent();
            _dynHist = dynamicHist;
            _model = model;
            checkBox1.Checked = true;
            if (UsesStatic(_dynHist)) tableLayoutPanel3.Enabled = false;
            else tableLayoutPanel2.Enabled = false;

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _dynHist.UseScott = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel2.Enabled = radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel3.Enabled = radioButton2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _dynHist.UseSquareRoot = true;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _dynHist.UseSturges = true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _dynHist.UseCustomStatic = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var stepSize = Convert.ToInt32(textBox1.Text);
                if (stepSize < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                if (stepSize*_model.Setting.PopulationSize > _model.Setting.MaxEvaluations)
                {
                    throw new WarningException("steeping to big");
                }

                textBox1.ForeColor = Color.Black;
               _dynHist.CustomStaticSize = stepSize;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                textBox1.ForeColor = Color.Red;
            }
        }


        private static bool UsesStatic(DynamicHist hist)
        {
            return hist.UseSquareRoot || hist.UseCustomStatic || hist.UseScott || hist.UseSturges;
        }

        private void button1_Click(object sender, EventArgs e)
        {
             _dynHist.CalcLinear();
            pictureBox1.ImageLocation = @"dynamicHist.png";
        }

        private void startStep_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var start = Convert.ToInt32(startStep.Text);
                if (start < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                if (start > _model.Setting.MaxEvaluations)
               /* {
                    throw new WarningException("start to big");
                } */

                textBox1.ForeColor = Color.Black;
                _dynHist.StartEvolution = start;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                startStep.ForeColor = Color.Red;
            }
        }

        private void endStep_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var end = Convert.ToInt32(endStep.Text);
                if (end < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                /*if (end > _model.MaxEvaluations)
                {
                    throw new WarningException("start to big");
                } */

                endStep.ForeColor = Color.Black;
                _dynHist.EndEvolution = end;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                endStep.ForeColor = Color.Red;
            }
        }

        private void minSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var minBin = Convert.ToInt32(minSize.Text);
                if (minBin < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                minSize.ForeColor = Color.Black;
                _dynHist.StartBins = minBin;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                minSize.ForeColor = Color.Red;
            }
        }

        private void maxSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var maxBin = Convert.ToInt32(maxSize.Text);
                if (maxBin < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                maxSize.ForeColor = Color.Black;
                _dynHist.EndBins = maxBin;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                maxSize.ForeColor = Color.Red;
            }
        }

        private void stepping_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var step = Convert.ToInt32(stepping.Text);
                if (step < 0)
                {
                    throw new WarningException("Must be greater 0");
                }

                stepping.ForeColor = Color.Black;
                _dynHist.Stepping = step;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                stepping.ForeColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
