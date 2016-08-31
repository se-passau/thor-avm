using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InteracGenerator;
using JMetalCSharp.Problems.LZ09;

namespace IntergenDesktop.UserControls
{

    
    public partial class SolutionVis : UserControl
    {

        private readonly InterGen _model;

        private double _w1;
        private double _w2;
        private double _w3;
        private double _sum;

        public SolutionVis(InterGen model)
        {
            InitializeComponent();
            _model = model;
            label9.Text = _model.Setting.LogFolder + Path.DirectorySeparatorChar + @"{feat, interac, variant}Solution.txt";
            _model.Setting.DrawAngle = 55;
            //comboBox1.DisplayMember = "DisplayName";

            //var parp = new ParetoPic(_model.BestSolutions);
            //parp.Dock = DockStyle.Fill;
            //tableLayoutPanel2.Controls.Add(parp, 0, 2);
            //tableLayoutPanel2.SetColumnSpan(parp, 2);

            //textBox1.DataBindings.Add("Text", this, "_w1");
           
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            /*for (int i = 0; i < _model.BestSolutions.Count; i++)
            {
                var sp = new SolutionPic(_model.BestSolutions[i], _model, i);

                flowLayoutPanel1.Controls.Add(sp);
            }*/
            
        }


        private void SolutionVis_Load(object sender, EventArgs e)
        {
            label1.Text = "Done";

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _w1 = Convert.ToDouble(textBox1.Text);
                label2.Text = @"Select";
            }
            catch (FormatException ex)
            {
                label2.Text = ex.Message;
            }
            button3.Enabled = Math.Abs(_w1 + _w2 + _w3 - 100) < 0.005;
            button5.Enabled = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _w2 = Convert.ToDouble(textBox2.Text);
                label2.Text = @"Select";
            }
            catch (FormatException ex)
            {
                label2.Text = ex.Message;
            }
            button5.Enabled = false;
            button3.Enabled = Math.Abs(_w1 + _w2 + _w3 - 100) < 0.005;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _w3 = Convert.ToDouble(textBox3.Text);
                label2.Text = @"Select";
            }
            catch (FormatException ex)
            {
                label2.Text = ex.Message;
            }
            button3.Enabled = Math.Abs(_w1 + _w2 + _w3 - 100) < 0.005;
            button5.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sum = _w1 + _w2 + _w3;
            if (sum != 100)
            {
                var scale = 100/sum;
                _w1 = _w1*scale;
                _w2 = _w2*scale;
                _w3 = _w3*scale;
            }

            textBox1.Text = $"{_w1}";
            textBox2.Text = $"{_w2}";
            textBox3.Text = $"{_w3}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var sum = _w1 + _w2 + _w3;
          
            if (Math.Abs(sum - 100) < 0.000001)
            {
                _model.ShowParetoSolution(_w1, _w2, _w3);
                pictureBox1.ImageLocation = @"solutionBest.png";
                button5.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _model.Draw3DPareto();
            pictureBox2.ImageLocation = @"3dplot.png";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _model.Setting.DrawAngle = (int)numericUpDown1.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "FeatFitn.png");
            File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "InteracFitn.png");
            File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "VariantFitn.png");
            RIntegrator.DrawFitnessEvolution(_model.Setting.LogFolder, _model.Setting.FeatureFitness, _model.Setting.InteracFitness, _model.Setting.VariantFitness);
            if (_model.Setting.FeatureFitness) pictureBox3.ImageLocation = @"FeatFitn.png";
            if (_model.Setting.InteracFitness) pictureBox4.ImageLocation = @"InteracFitn.png";
            if (_model.Setting.VariantFitness) pictureBox5.ImageLocation = @"VariantFitn.png";
            button4.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            _model.WriteResult();
            //_model.BestSolution.WriteSolution(_model.Setting.LogFolder);
        }
    }
}
