using System;
using System.Collections.Generic;
using System.Linq;
using SPLConqueror_Core;

namespace InteracGenerator.InteracWeaving
{
    internal class BinaryOptionWeaver : AbstractWeaver<BinaryOption>
    {
        private VariabilityModel _vm;
        private InfluenceModel _influenceModel;

        public BinaryOptionWeaver(InterGen model) : base(model)
        {
        }

        public void SetVariabilityModel(VariabilityModel model)
        {
            _vm = model;
            var prop = new NFProperty("NFP");
            GlobalState.currentNFP = prop;

            _influenceModel = new InfluenceModel(_vm, prop);
            GlobalState.infModel = _influenceModel;
            GlobalState.varModel = _vm;
        }

        public override void SetUpWeaver()
        {
            FoundInteractions = new List<List<BinaryOption>>();
        }


        public override void AddAttributesToModel()
        {
            var toAssign = new List<double>();
            toAssign.AddRange(FeatureValues);
            
            foreach (var opt in FeatureList)
            {
                if (toAssign.Count == 0)
                    return;
                var next = Rand.Next(0, toAssign.Count);
                var value = toAssign[next];
                toAssign.RemoveAt(next);  //do not assign this attribute value again
                _influenceModel.BinaryOptionsInfluence.Add(opt, new InfluenceFunction(opt.Name + " * " + value));
            }
        }


        public override void AddInteractionToModel(int index, List<BinaryOption> tempConfig)
        {
            var function = tempConfig.Aggregate("", (current, op) => current + op.Name + " * ");
            function = function + InteractionValues[index];

            var interF = new InfluenceFunction(function);
            var inter = new Interaction(interF);  //TODO Need to set the BinaryOption for the Interaction?
            _influenceModel.InteractionInfluence.Add(inter, interF);
        }

        public override List<List<BinaryOption>> LoadInteractions(string fileName = "foundInteractions.txt")
        {
            throw new NotImplementedException();
        }

        public override bool CheckInteractionSat(List<BinaryOption> tempConfig)
        {
            var t = new MicrosoftSolverFoundation.CheckConfigurationSAT();
            //var solver = new CheckConfigSAT(null);  //TODO is this call necessary for the path information??

            return t.checkConfigurationSAT(tempConfig, _vm, false);
        }
    }
}
