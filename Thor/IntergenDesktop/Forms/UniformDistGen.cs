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

namespace IntergenDesktop
{
    public partial class UniformDistGen : Form
    {
        private InterGen model;
        private Distribution.DistributionType Type;

        public UniformDistGen(InterGen model, Distribution.DistributionType Type)
        {
            this.model = model;
            this.Type = Type;
            InitializeComponent();
            if (!double.IsNaN(model.Setting.UnifMin))
            {
                textBox1.Text = model.Setting.UnifMin.ToString();
            }
            if (!double.IsNaN(model.Setting.UnifMax))
            {
                textBox2.Text = model.Setting.UnifMax.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (model.Setting.UnifMax <= model.Setting.UnifMin) {
                label1.Text = "Max is smaller than Min";
                return;
            }

            if (double.IsNaN(model.Setting.UnifMin) && double.IsNaN(model.Setting.UnifMax))
            {
                label1.Text = "One or more values were not set!";
                return;
            }

            model.CreateUnifDist(2, Type);
            Close();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var min = Convert.ToDouble(textBox1.Text);

                label2.ForeColor = Color.Black;
                model.Setting.UnifMin = min;
                label2.Text = "Min: Ok";
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
                var max = Convert.ToDouble(textBox2.Text);

                label3.ForeColor = Color.Black;
                model.Setting.UnifMax = max;
                label3.Text = "Max: Ok";
            }
            catch (Exception ex)
            {
                label3.ForeColor = Color.Red;
                label3.Text = ex.Message;
            }
        }
    }
}
