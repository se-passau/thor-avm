
using System;
using System.Windows.Forms;
using IntergenDesktop.UserControls;
using InteracGenerator;

namespace IntergenDesktop.Forms
{
    public partial class Mainframe : Form
    {

        private enum State {Start, FeatureModel, Features, Interaction, Variant, Generate, EvolutionSettings, Evolution, Results }

        private Features _featureForm;
        private FeatureModel _featureModelForm;
        private InteractionControl _interacForm;
        private TargetVariant _variantForm;
        private EvolutionSettings _evoSetting;
        private Evolution _evolution;
        private SolutionVis _solution;
        private VariantGenerationControl _varGen;

        private State _current;
        private UserControl _currentControl;
        private readonly InterGen _model;
        public Button Next;
        public Mainframe(InterGen model)
        {
            InitializeComponent();

            _model = model;
            _current = State.Start;
            Next = button2;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = @"THOR";
            _currentControl = new Start(_model, this) {Dock = DockStyle.Fill};
            //_currentControl = new FeatureModel(_model, button2) {Dock = DockStyle.Fill};
            tableLayoutPanel1.Controls.Add(_currentControl, 0, 1);

            label1.Text = @"Generator";
            button1.Visible = false;
            button2.Visible = false;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Maximized;
            }
            button1.Enabled = false;
            Activate();
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            _current = NextState();
            LoadState(_current);
        }


        private void LoadState(State curState)
        {
            tableLayoutPanel1.Controls.Remove(_currentControl);

            switch (curState)
            {
                case State.Start:
                    break;
                case State.FeatureModel:
                    if (_featureModelForm == null)
                    {
                        _featureModelForm = new FeatureModel(_model, button2);
                    }
                   
                    button2.Visible = true;
                    _currentControl = _featureModelForm;
                    label1.Text = @"Feature Model Settings";
                    button1.Enabled = false;
                    break;
                case State.Features:
                    button1.Visible = true;
                    if (_featureForm == null) _featureForm = new Features(_model, button2);
                    _currentControl = _featureForm;
                    button1.Enabled = true;
                    label1.Text = @"Feature Distribution";
                    break;
                case State.Interaction:
                    if (_interacForm ==null)_interacForm = new InteractionControl(_model, button2);
                    _currentControl = _interacForm;
                    label1.Text = @"Interaction Distribution";
                    break;
                case State.Variant:
                    if (_variantForm == null) _variantForm = new TargetVariant(_model, button2);
                    _currentControl = _variantForm;
                    label1.Text = @"Variant Distribution";
                    break;
                case State.Generate:
                    if (_varGen == null) _varGen = new VariantGenerationControl(_model, button2);
                    _currentControl = _varGen;
                    label1.Text = @"Variant Generation Settings";
                    break;
                case State.EvolutionSettings:
                    if (_evoSetting == null) _evoSetting = new EvolutionSettings(_model, button2);
                    _currentControl = _evoSetting;
                    label1.Text = @"Evolutionary Algorithm Settings";
                    break;
                case State.Evolution:
                    if (_evolution == null) _evolution = new Evolution(_model, button2);
                    _currentControl = _evolution;
                    label1.Text = @"Evolutionary Algorithm";
                    button2.Enabled = true;
                    break;
                case State.Results:
                    if (_solution == null) _solution = new SolutionVis(_model);
                    button2.Enabled = false;
                    _currentControl = _solution;
                    label1.Text = @"Solutions";
                    break;
                default:
                    return;

                   
            }
            var sml = _currentControl as IStateModelLoader;
            sml?.LoadSettings();
            _currentControl.Dock = DockStyle.Fill;
            _currentControl.Padding = new Padding(10);
            tableLayoutPanel1.Controls.Add(_currentControl, 0, 1);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private State NextState()
        {
            switch (_current)
            {
                case State.Start:
                    return State.FeatureModel;
                case State.FeatureModel:
                    return State.Features;
                case State.Features:

                    if (_model.Setting.NumberOfInteractions == 0)
                    {
                        return _model.Setting.NoVariantCalculation ? State.EvolutionSettings : State.Variant;
                    }
                    return State.Interaction;
                case State.Interaction:
                    return _model.Setting.NoVariantCalculation ? State.EvolutionSettings : State.Variant;
                case State.Variant:
                    return State.Generate;
                case State.Generate:
                    return State.EvolutionSettings;
                case State.EvolutionSettings:
                    return State.Evolution;
                case State.Evolution:
                    return State.Results;
                case State.Results:
                    return State.Results;
                default:
                    return State.Features;
            }
        }

        public void LoadedModel()
        {
            _current = NextState();
            LoadState(_current);
        }


        //featmodel - feat - interac - variant - vargenerate - evosetting - evolution - results
        private State PreviousState()
        {
            
            switch (_current)
            {
                case State.FeatureModel:
                    return State.FeatureModel;
                case State.Features:
                    return State.FeatureModel;
                case State.Interaction:
                    return State.Features;
                case State.Variant:
                    return _model.Setting.NumberOfInteractions == 0 ? State.Features : State.Interaction;
                case State.Generate:
                    if (_model.Setting.NumberOfInteractions == 0)
                    {
                        return _model.Setting.NoVariantCalculation ? State.Features : State.Variant;
                    }
                    return _model.Setting.NoVariantCalculation ? State.Interaction : State.Variant;
                case State.EvolutionSettings:
                    return _model.Setting.NoVariantCalculation
                        ? _model.Setting.NumberOfInteractions > 0 ? State.Interaction : State.Features
                        : State.Generate;
                case State.Evolution:
                    return State.EvolutionSettings;
                case State.Results:
                    return State.Evolution;
                default:
                    return State.Features;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _current = PreviousState();
            LoadState(_current);
            
        }

       
    }
}
