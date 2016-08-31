using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InteracGenerator;

namespace IntergenDesktop.UserControls
{
    public partial class ParetoPic : UserControl
    {


        public ParetoPic(List<SolutionContainer> solutions)
        {
            InitializeComponent();


            CreateRString(solutions);

        }


        private void CreateRString(List<SolutionContainer> sols)
        {

            var minFeatTestVal = double.MaxValue;
            var maxFeatTestVal = double.MinValue;
            var minVarTestVal = double.MaxValue;
            var maxVarTestVal = double.MinValue;

            var featureTests = "c(";
            var variantTests = "c(";

            for (var i = 0; i < sols.Count - 1; i++)
            {
                if (sols[i].FeatureTVal < minFeatTestVal)
                {
                    minFeatTestVal = sols[i].FeatureTVal;
                }
                if (sols[i].FeatureTVal > maxFeatTestVal)
                {
                    maxFeatTestVal = sols[i].FeatureTVal;
                }
                if (sols[i].VariantTVal < minVarTestVal)
                {
                    minVarTestVal = sols[i].VariantTVal;
                }
                if (sols[i].VariantTVal > maxVarTestVal)
                {
                    maxVarTestVal = sols[i].VariantTVal;
                }

                featureTests = string.Concat(featureTests, sols[i].FeatureTVal, ", ");
                variantTests = string.Concat(variantTests, sols[i].VariantTVal, ", ");
            }

            featureTests = string.Concat(featureTests, sols[sols.Count-1].FeatureTVal, ")");
            variantTests = string.Concat(variantTests, sols[sols.Count-1].VariantTVal, ")");


            RIntegrator.PlotPareto(featureTests, variantTests);

            

        }

        protected override void OnLoad(EventArgs e)
        {
            pictureBox1.ImageLocation = "test.png";
        }
    }
}
