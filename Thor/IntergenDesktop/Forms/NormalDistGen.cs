using InteracGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntergenDesktop
{
    public partial class NormalDistGen : Form
    {

        private InterGen Model;
        private Distribution.DistributionType Type;

        public NormalDistGen(InterGen Model, Distribution.DistributionType type)
        {
            InitializeComponent();
            this.Model = Model;
            this.Type = type;

            if (!double.IsNaN(Model.Setting.Mean)) {
                textBox1.Text = Model.Setting.Mean.ToString();
            }
            if (!double.IsNaN(Model.Setting.StandardDeviation))
            {
                textBox2.Text = Model.Setting.StandardDeviation.ToString();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double mean = 0;
            try
            {
                mean = Convert.ToDouble(textBox1.Text);
                
                label2.ForeColor = Color.Black;
                Model.Setting.Mean = mean;
                label2.Text = @"Mean: Ok";
            }
            catch (Exception ex)
            {
                label2.ForeColor = Color.Red;
                label2.Text = ex.Message;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var sd = Convert.ToDouble(textBox2.Text);
                if (sd < 0)
                {
                    throw new WarningException("Must be greater 0");
                }
                label3.ForeColor = Color.Black;
                Model.Setting.StandardDeviation = sd;
                label3.Text = @"Standard Deviation: Ok";
            }
            catch (Exception ex)
            {
                label3.ForeColor = Color.Red;
                label3.Text = ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (double.IsNaN(Model.Setting.Mean) && double.IsNaN(Model.Setting.StandardDeviation))
            {
                label1.Text = @"One or more values were not set!";
            }
            else
            {
                Model.CreateNormalDist(2, Type);
                Close();
            }
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
