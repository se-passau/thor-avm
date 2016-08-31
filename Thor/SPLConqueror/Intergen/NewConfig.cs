using RDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intergen
{
    public partial class NewConfig : Form
    {

        private REngine engine;
        private int numberOfFeatures;
        private DistType currentType;
        private Double min;
        private Double max;
        private Double mean;
        private Double sd;

        public NewConfig()
        {
            InitializeComponent();
            engine = REngine.GetInstance();

            numberOfFeatures = 100;
            min = 0;
            max = 20;
            mean = 10;
            sd = 0;
            minBox.Text = min.ToString();
            maxBox.Text = max.ToString();
            meanBox.Text = mean.ToString();
            sdBox.Text = sd.ToString();
            textBox3.Text = numberOfFeatures.ToString();
            
        }

        public NewConfig (GeneratorConfiguration gConf)
        {
            InitializeComponent();
            engine = REngine.GetInstance();     
        }

        private void NewConfig_Load(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Open_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                numberOfFeatures = Convert.ToInt32(textBox3.Text);
                RedrawDistribution();
            }
           
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        { 
            
           
          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int _item = comboBox1.SelectedIndex;
            
           
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

            if (comboBox1.Text.Equals("Normal"))
            {
                currentType = DistType.Normal;
              
            }
            else if (comboBox1.Text.Equals("LogNormal"))
            {
                sd = 0.5;
                sdBox.Text = sd.ToString();
                max = 100;
                maxBox.Text = max.ToString();
                currentType = DistType.LogNormal;
                Console.WriteLine("Creating LogNormal");

            }
            RedrawDistribution();

        }

       


        private void RedrawDistribution() {
            NumericVector data;
            string TempImagePath = @"C:\Users\Tom\Desktop\test.png";
            engine.Evaluate(string.Format("png('{0}', {1}, {2})", TempImagePath.Replace('\\', '/'), pictureBox1.Width, this.pictureBox1.Height));
            Distribution d = new Distribution(currentType, min, max, mean, sd, numberOfFeatures);
            data = engine.CreateNumericVector(d.Values);
            engine.SetSymbol("data", data);
            engine.Evaluate("plot(density(data))");
            engine.Evaluate("dev.off()");
            this.pictureBox1.ImageLocation = TempImagePath;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            numberOfFeatures = Convert.ToInt32(textBox3.Text);
            RedrawDistribution();
        }

        private void minBox_Leave(object sender, EventArgs e)
        {
            min = Convert.ToDouble(minBox.Text);
            Console.WriteLine(min);
            RedrawDistribution();
        }

        private void minBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                min = Convert.ToDouble(minBox.Text);
                RedrawDistribution();
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            RedrawDistribution();
        }

        private void maxBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void maxBox_Leave(object sender, EventArgs e)
        {
            max = Convert.ToDouble(maxBox.Text);
            Console.WriteLine(max);
            RedrawDistribution();
        }

        private void meanBox_Leave(object sender, EventArgs e)
        {
            mean = Convert.ToDouble(meanBox.Text);
            RedrawDistribution();
        }

        private void sdBox_Leave(object sender, EventArgs e)
        {
            sd = Convert.ToDouble(sdBox.Text);
            RedrawDistribution();
        }
    }
}
