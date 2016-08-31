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
    public partial class SolutionPic : UserControl
    {

        private SolutionContainer _solCon;
        private InterGen _model;
        private int _index ;
        private string[] picArray;

        public SolutionPic(SolutionContainer solutionContainer, InterGen model,  int index)
        {
            InitializeComponent();
            _solCon = solutionContainer;
            _model = model;
            _index = index;

            //pictureBox1.ImageLocation
            //picArray = _model.DrawSolution(solutionContainer, index);
       
          

            label1.Text = solutionContainer.DisplayName;
            featLabel.Text = $"Test: {solutionContainer.FeatureTVal}";
            varLabel.Text = $"Test: {solutionContainer.VariantTVal}";
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            pictureBox1.ImageLocation = picArray[0];
            pictureBox2.ImageLocation = picArray[1];
        }
    }
}
