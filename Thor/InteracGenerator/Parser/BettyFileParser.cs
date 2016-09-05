using System;
using System.Collections.Generic;
using System.Diagnostics;
using SPLConqueror_Core;

namespace InteracGenerator.Parser
{
    public enum ModeOfOperation { Start, R, A, C }
    public enum Constraints {  Requires, Excludes }


    public class BettyFileParser
    {
        private readonly VariabilityModel _varModel;
        private readonly InfluenceModel _inflModel;

        public BettyFileParser()
        {
            _varModel = new VariabilityModel("generated");
            _inflModel = new InfluenceModel(_varModel, new NFProperty("nfp"));
        }


        public VariabilityModel ReadConfiguration(string[] bettyFile) { 

            var current = ModeOfOperation.Start;
            for (var i = 0; i < bettyFile.Length; i++) {

                //% selects mode
                if (bettyFile[i].StartsWith("%")) {
                    current = SelectModeOfOperation(bettyFile, i);
                    continue;  //this line has nothing left
                }

                switch (current)
                {
                    case ModeOfOperation.R:
                        
                        var line = bettyFile[i].Replace("\'", "");
                        var tokens = line.Split(null);   //whitespace split
                        ParseRelationLine(tokens);
                        break;
                    case ModeOfOperation.A:
                        ParseAttributeLine(bettyFile[i]);
                        break;
                    case ModeOfOperation.C:
                        ParseConstraintLine(bettyFile[i]);
                        break;
                    
                }
            }
            //model.saveXML();
            return _varModel;
        }

        public void ParseAttributeLine(string line)
        {
            
            var tokens = line.Split('.');
            var value = line.Split(',');
            if (!value[0].Contains("Integer")) return;
            var nfpVal = Convert.ToInt32(value[1]);
            var influence = new InfluenceFunction(tokens[0] + " * " + nfpVal);



            //BETTY: 
            //F1.Atribute1: Integer[0 to 100],57,0;
            //F1.Atribute0: Integer[0 to 100],4,0;
            //Betty can have multiple Attributes on one Feature
            //Need new InfluenceModel for second Attribute???
            //TODO
            if (!_inflModel.BinaryOptionsInfluence.ContainsKey(_varModel.getBinaryOption(tokens[0])))
            {
                _inflModel.BinaryOptionsInfluence.Add(_varModel.getBinaryOption(tokens[0]), influence);

            }
            //Console.WriteLine(influence.ToString());
        }

        public void ParseConstraintLine(string line)
        {
            if (line.Length <= 1) return;

            var tokens = line.Split(null);
            var feature1 = tokens[0];
            var keyword = tokens[1];
            var feature2 = tokens[2].Remove(tokens[2].Length - 1, 1);
            var f1 = _varModel.getBinaryOption(feature1);
            var f2 = _varModel.getBinaryOption(feature2);
                
            if (keyword.Equals(Constraints.Excludes.ToString()))
            {

                List<List<ConfigurationOption>> excl = f1.Excluded_Options;
                List<ConfigurationOption> subexcl = new List<ConfigurationOption> {f2};
                excl.Add(subexcl);
                f1.Excluded_Options = excl;
                //Console.WriteLine("Added exclude " + f1 + " >-< " + f2);
            }
            else if (keyword.Equals(Constraints.Requires.ToString()))
            {
                List<List<ConfigurationOption>> imply = f1.Implied_Options;
                List<ConfigurationOption> subimply = new List<ConfigurationOption>();
                subimply.Add(f2);
                imply.Add(subimply);
                f1.Implied_Options = imply;
                //Console.WriteLine("Added imply " + f1 + " -> " + f2);
            }
            else
            {

            }
            //Console.WriteLine(feature1 + keyword + feature2);
        }

        public void ParseRelationLine(string[] tokens)
        {
            BinaryOption parentOption = null;
            bool childOf = false;

            for (int j = 0; j < tokens.Length; j++)
            {
                //next binOps are Children
                if (tokens[j].Equals(":"))
                {
                    childOf = true;
                    continue;  //next token
                }
                if (tokens[j].Equals(";"))
                {
                    Debug.Assert(j == tokens.Length - 1);
                    continue; //last token, get next line
                }
                if (tokens[j].Equals(""))
                {
                    continue; //divider token, continue with next token
                }
                else
                {
                    if (!childOf)
                    {
                        parentOption = 
                            tokens[j] == "root" ? 
                            _varModel.Root : 
                            _varModel.getBinaryOption(tokens[j]);

                    }
                    else
                    {
                        ParseRelation(tokens[j], parentOption, childOf);
                    }
                  
                }
            }
        }

        public void ParseRelation(string binOpName, BinaryOption parentOption, bool childOf)
        {
            BinaryOption toAdd = null;
            if (binOpName.Contains(";")) binOpName = binOpName.Replace(";", "");
            if (binOpName.Contains("[") && binOpName.Contains("]"))
            {
                //binOpName = binOpName.Remove(0, 1);
                //binOpName = binOpName.Remove(binOpName.Length - 1, 1);
                Console.WriteLine("Cardinality Group");
                //Console.WriteLine("Added : " + binOpName + " optional with parent " + parentOption);
                //_varModel.addConfigurationOption(toAdd = new BinaryOption(_varModel, binOpName) { Optional = true });
            }
            else
            {
                //Console.WriteLine("Added : " + binOpName + " with parent " + parentOption);
                _varModel.addConfigurationOption(toAdd = new BinaryOption(_varModel, binOpName));
            }

            if (childOf)
            {
                toAdd.Parent = parentOption;
            }
            else
            {
                //Console.WriteLine("Setting Parent: " + toAdd + " " +  tokens[j]);
                //parentOption = toAdd;
            }
        }

        public ModeOfOperation SelectModeOfOperation(string[] file, int index) {
            if (file[index].Contains("Relationships"))
            {
                Console.WriteLine("\tParsing Relationships");
                return ModeOfOperation.R;
            }
            if (file[index].Contains("Attributes"))
            {
                Console.WriteLine("\tParsing Attributes");
                return ModeOfOperation.A;

            }
            if (file[index].Contains("Constraints"))
            {
                Console.WriteLine("\tParsing Constraints");
                return ModeOfOperation.C;
            }
            {
                return ModeOfOperation.Start;
            }
        }

    }
}
