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
    public partial class LoadConfig : Form
    {
        public LoadConfig()
        {
            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
          
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void newConfigEvent(object sender, EventArgs e)
        {
            NewConfig frm = new NewConfig();
            frm.Location = this.Location;
            frm.Width = this.Width;
            frm.Height = this.Height;
            frm.StartPosition = FormStartPosition.Manual;
            frm.FormClosing += delegate { this.Show(); };
            frm.Show();
            this.Hide();
        }

        private void loadConfigEvent(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "C:\\";
          
            openFileDialog1.Filter = "GeneratorConfigs |*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Benutzer möchte nun öffnen: " + openFileDialog1.FileName);
                GeneratorConfiguration genConf = new GeneratorConfiguration();
                genConf.Load(openFileDialog1.FileName);

                NewConfig frm = new NewConfig(genConf);
                frm.Location = Location;
                frm.Width = Width;
                frm.Height = Height;
                frm.StartPosition = StartPosition;
                frm.FormClosing += delegate { this.Show(); };
                frm.Show();
                this.Hide();
            }
        }
    }
}
