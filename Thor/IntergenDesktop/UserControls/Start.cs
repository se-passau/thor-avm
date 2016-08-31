using System;
using System.IO;
using System.Windows.Forms;
using InteracGenerator;
using IntergenDesktop.Forms;

namespace IntergenDesktop.UserControls
{
    public partial class Start : UserControl
    {
        private readonly InterGen _model;
        private readonly Mainframe _frame;

        public Start(InterGen model, Mainframe frame)
        {
            InitializeComponent();
            _model = model;
            _frame = frame;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = @"Select Feature Model File";
            var result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    //var text = File.ReadAllText(openFileDialog1.FileName);


                    //var loaded = new NewGen(Model, true /* from file */);
                    _model.LoadFM(openFileDialog1.FileName);
                    _frame.LoadedModel();
                    
                }
                catch (IOException exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }
    }
}
