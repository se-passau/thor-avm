using Accord.Controls;
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
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace Intergen
{
    public partial class TestWindow : Form
    {
        private REngine engine;
        private NumericVector data;
        private double[] values;
        public TestWindow(REngine e, NumericVector data, double[] values)
        {
            InitializeComponent();
            this.engine = e;
            this.data = data;
            this.values = values;
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            engine.Evaluate("library('ks')");
            engine.Evaluate("library(ggplot2)");

            //engine.Evaluate("library(sm)");
            for (int i = 0; i < values.Length; i++) {

                Random r = new Random(3);
                //r.NextDouble();
                values[i] += 1000 * r.NextDouble();
            }
            data = engine.CreateNumericVector(values);
            engine.SetSymbol("group1", data);

            engine.Evaluate("fhat <- kde(group1, h = hpi(group1))");
            engine.Evaluate("X <- rkde(fhat = fhat, n = length(group1))");
            //engine.Evaluate("p1 <- plot(density(group1))");
           // engine.Evaluate("p2 <- plot(density(X))");
           
            string TempImagePath = @"C:\Users\Tom\Desktop\test.png";
            engine.Evaluate(string.Format("png('{0}', {1}, {2})", TempImagePath.Replace('\\', '/'), pictureBox1.Width, this.pictureBox1.Height));
            engine.Evaluate("par(mfrow=c(1,2))");
            engine.Evaluate("plot(density(group1))");
            engine.Evaluate("plot(density(X))");

            engine.Evaluate("dev.off()");
            this.pictureBox1.ImageLocation = TempImagePath;
            //engine.Evaluate("hist(group1)");

            //engine.Evaluate("d <- density(group1)");
            //engine.Evaluate("sm.density(group1, PANEL =\"TRUE\")");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
