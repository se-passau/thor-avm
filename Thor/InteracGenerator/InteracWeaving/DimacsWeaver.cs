using System.Collections.Generic;
using System.IO;

namespace InteracGenerator.InteracWeaving
{
    internal class DimacsWeaver : AbstractWeaver<string>
    {
        private string[] _lines;

        public DimacsWeaver(InterGen model) : base(model)
        {
        }

        public override bool CheckInteractionSat(List<string> tempConfig)
        {
            var t = new MicrosoftSolverFoundation.CheckConfigurationSAT();
            //var solver = new CheckConfigSAT(null);
            return t.checkDimacsSAT(tempConfig, _lines);
        }

        public void SetFile(string[] fileContent)
        {
            _lines = fileContent;
        }

        public override void SetUpWeaver()
        {
            FoundInteractions = new List<List<string>>();
           
        }

        public override List<List<string>> LoadInteractions(string fileName = "foundInteractions.txt")
        {
            var interactions = new List<List<string>>();
            var lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                var split = lines[i].Split('*');
                var interac = new List<string>();
                for (int j = 0; j < split.Length - 1; j++)
                {
                    interac.Add(split[j]);
                }
                interactions.Add(interac);
            }
            return interactions;
        }


        public override void AddAttributesToModel()
        {

            //empty the file content
            File.WriteAllText(@"ivmodel.txt", string.Empty);

            var influence = "";
            var toAssign = new List<double>(FeatureValues);
            foreach (var opt in FeatureList)
            {
                var next = Rand.Next(0, toAssign.Count);
                var value = toAssign[next];
                toAssign.RemoveAt(next);  //do not assign this attribute value again
                //_influenceModel.BinaryOptionsInfluence.Add(opt, new InfluenceFunction(opt.Name + " * " + value));
                influence = string.Concat(influence, $"{opt} * {value}\n");
            }

           // influence = influence.Remove(influence.Length - 1, 1);

            using (var sw = File.AppendText("ivmodel.txt"))
            {
                sw.Write(influence);
            }
        }

        public override void AddInteractionToModel(int index, List<string> tempConfig)
        {
            var val = InteractionValues[index];
            using (var sw = File.AppendText("ivmodel.txt"))
            {
                foreach (var t in tempConfig)
                {
                    sw.Write(t + " * ");
                }
                sw.WriteLine(val);
            }
        }
    }
}
