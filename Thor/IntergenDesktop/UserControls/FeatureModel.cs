using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using InteracGenerator;


namespace IntergenDesktop.UserControls
{
    public partial class FeatureModel : UserControl, IStateModelLoader
    {
        private double _sum;
        private readonly InterGen _model;
        private bool _dontRaiseEvent;
        private List<double> orderValues;
        private List<TextBox> interactionBoxes;
        private int _orderCount = 1;
        private List<Label> interactionErrorLabel;
        private Button _nextButton;

        public FeatureModel(InterGen model, Button nextButton)
        {
            InitializeComponent();
            _model = model;

            NumberOfFeatures.DataBindings.Add("Text", _model.Setting, "NumberOfFeatures");
            NumberOfFeatures.ReadOnly = true;
            model.Setting.NumberOfInteractions = 0;
            interactionBoxes = new List<TextBox>();
            interactionErrorLabel = new List<Label>();

            orderValues = new List<double>();
            _nextButton = nextButton;
            _nextButton.Enabled = false;
            flowLayoutPanel2.Enabled = false;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddNewInteractionOrder();

            _model.Setting.NoVariantCalculation = true;
            _model.Setting.FeatureFitness = true;

        }

        private void NumberOfFeatures_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var features = Convert.ToInt32(NumberOfFeatures.Text);
                if (features < 0)
                {
                    throw new WarningException("Must be positive");
                }

                _model.Setting.NumberOfFeatures = features;

                label4.ForeColor = Color.Black;
                label2.ForeColor = Color.Black;
                label2.Text = @"Ok";
            }
            catch (Exception ex)
            {
                if (_model.Setting.NumberOfInteractions == 0)
                {
                    label4.Text = "";
                }
                label2.ForeColor = Color.Red;
                label2.Text = ex.Message;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (_dontRaiseEvent) return;
            try
            {
                var interactions = Convert.ToDouble(textBox2.Text);
                if (interactions < 0)
                {
                    throw new WarningException("Must be positive");
                }

                label4.ForeColor = Color.Black;
                label4.Text = @"Ok";

                _model.Setting.NumberOfInteractions = (int)interactions;

                if (_model.Setting.NumberOfInteractions == 0)
                {
                    flowLayoutPanel2.BackColor = Color.FromArgb(15, Color.Green);
                    //button15.Enabled = true;
                    _nextButton.Enabled = true;
                    button14.Enabled = false;
                    button1.Enabled = false;
                    checkBox2.Enabled = false;
                    flowLayoutPanel2.Enabled = false;
                    checkBox2.Checked = false;
                }
                else
                {
                    button14.Enabled = true;
                    flowLayoutPanel2.Enabled = true;
                    checkBox2.Enabled = true;
                    button1.Enabled = true;
                    if (Math.Abs(_sum - 100) > 0.0000001)
                    {
                        flowLayoutPanel2.BackColor = Color.FromArgb(15, Color.Red);
                        _nextButton.Enabled = false;
                    }
                }

                _dontRaiseEvent = true;
                textBox11.Text = $"{(_model.Setting.NumberOfInteractions * 100.0f) / _model.Setting.NumberOfFeatures}";
                _dontRaiseEvent = false;
            }

            catch (FormatException ex)
            {
                _dontRaiseEvent = true;
                textBox2.Text = _model.Setting.NumberOfInteractions.ToString();
                _dontRaiseEvent = false;
            }

            catch (Exception ex)
            {
                label4.ForeColor = Color.Red;
                label4.Text = ex.Message;
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (_dontRaiseEvent)
            {
                return;
            }

            try
            {
                var percent = Convert.ToDouble(textBox11.Text);
                if (percent < 0)
                {
                    throw new WarningException("Percentage must be greater zero");
                }
                _dontRaiseEvent = true;
                textBox2.Text = $"{(int)(_model.Setting.NumberOfFeatures * percent / 100)}";
                _model.Setting.NumberOfInteractions = (int) (_model.Setting.NumberOfFeatures*percent/100);
                _dontRaiseEvent = false;
                if ((int)(_model.Setting.NumberOfFeatures * percent / 100) != 0)
                {
                    label4.ForeColor = Color.Black;
                    flowLayoutPanel2.Enabled = true;
                    label4.Text = @"Ok";
                    if (Math.Abs(_sum - 100) > 0.0000001)
                    {
                        flowLayoutPanel2.BackColor = Color.FromArgb(15, Color.Red);
                        _nextButton.Enabled = false;
                    }
                }
                else
                {


                    textBox2.Text = @"0";
                    flowLayoutPanel2.BackColor = Color.FromArgb(15, Color.Green);
                    flowLayoutPanel2.Enabled = false;
                    _nextButton.Enabled = true;
                    label4.Text = @"Ok";
                }
            }

            catch (Exception ex)
            {
                label4.ForeColor = Color.Red;
                label4.Text = ex.Message;
            }
        }

        private void AddNewInteractionOrder()
        {

            var flp = new FlowLayoutPanel
            {
                Height = 40,
                Width = flowLayoutPanel2.Width,
                Dock = DockStyle.Top
            };
            var info = new Label { Text = @"Interaction Order: " + _orderCount, Width = 180 };

            flp.Controls.Add(info);
            var txtBox = new TextBox
            {
                Name = _orderCount.ToString(),
                Text = 0.ToString(),
                Width = 100,
            };
            txtBox.TextChanged += TxtBox_TextChanged;
            flp.Controls.Add(txtBox);

            var errorLabel = new Label
            {
                Text = @"%",
                Dock = DockStyle.Fill,
                Width = 200,
            };
            flp.Controls.Add(errorLabel);

            interactionErrorLabel.Add(errorLabel);
            interactionBoxes.Add(txtBox);
            orderValues.Add(0);

            flowLayoutPanel2.Controls.Add(flp);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            _orderCount++;
            AddNewInteractionOrder();
        }


        private void TxtBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox?.Name == null) return;

            var order = Convert.ToInt32(textBox.Name);
            try
            {
                var val = Convert.ToDouble(textBox.Text);

                //percentage must be between 0 and 100
                if (val < 0 || val > 100)
                {
                    interactionErrorLabel[order - 1].ForeColor = Color.Red;
                    interactionErrorLabel[order - 1].Text = @"% Must be between 0 and 100";
                    return;
                }
                interactionErrorLabel[order - 1].Text = @"% Ok";
                interactionErrorLabel[order - 1].ForeColor = Color.Black;
                orderValues[order - 1] = val;

                _sum = 0;
                _sum = orderValues.Sum();

                label11.Text = _sum.ToString(CultureInfo.CurrentCulture);

                //percentages do not add to 100, and we have interactions, --> cant go to next step
                if (_sum != 100 && _model.Setting.NumberOfInteractions > 0)
                {
                    flowLayoutPanel2.BackColor = Color.FromArgb(25, Color.Red);
                    _nextButton.Enabled = false;
                }
                else
                {
                    //percentages add to 100 or no interactions
                    flowLayoutPanel2.BackColor = Color.FromArgb(25, Color.Green);
                    _nextButton.Enabled = true;
                    _model.Setting.InteractionOrderPercent = orderValues;
                    //button15.Enabled = true;
                }
            }

            catch (Exception exc)
            {
                interactionErrorLabel[order - 1].ForeColor = Color.Red;
                interactionErrorLabel[order - 1].Text = exc.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _sum = orderValues.Sum();


            //Sum is zero
            if (Math.Abs(_sum) < 0.000001)
            {
                var size = orderValues.Count;
                for (var i = 0; i < orderValues.Count; i++)
                {
                    interactionBoxes[i].Text = (100.0 / size).ToString(CultureInfo.CurrentCulture);
                }
            }

            //Sum is not adding up to 100
            else if (Math.Abs(_sum - 100.0) > 0.0000001)
            {
                var scale = 100 / _sum;
                for (var i = 0; i < orderValues.Count; i++)
                {
                    interactionBoxes[i].Text = (orderValues[i] * scale).ToString(CultureInfo.CurrentCulture);
                }
            }

            flowLayoutPanel2.BackColor = Color.FromArgb(15, Color.Green);
            _nextButton.Enabled = true;
            _model.Setting.InteractionOrderPercent = orderValues;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.NoVariantCalculation = !checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.InteracFitness = checkBox2.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.FeatureFitness = checkBox4.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            _model.Setting.VariantFitness = checkBox3.Checked;
            if (checkBox3.Checked)
            {
                checkBox1.Checked = true;
            }
        }

        public void LoadSettings()
        {
            if (flowLayoutPanel2.BackColor == Color.FromArgb(15, Color.Green))
            {
                _nextButton.Enabled = true;
            }
        }
    }
}
