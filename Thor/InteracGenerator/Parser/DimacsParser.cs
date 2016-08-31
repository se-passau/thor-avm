using SPLConqueror_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace InteracGenerator.Parser
{
    internal class DimacsParser
    {
        public string FileName;
        public VariabilityModel model;
        public enum Mode {  Start, Comments, P, Clauses }
       
       // public List<BinaryOption> toRemove; 
        public Dictionary<BinaryOption, BinaryOption> implies;
        public int Count;
        public List<string> FeatureList; 

        //private Dictionary<int, string> numberToName;
        public DimacsParser(string fileName)
        {
          
            model = new VariabilityModel("Generated");
            FileName = fileName;
            implies = new Dictionary<BinaryOption, BinaryOption>();
            //toRemove = new List<BinaryOption>();
            FeatureList = new List<string>();

            var lines = File.ReadAllLines(FileName);
            Count = 1;
            foreach (var t in lines.Where(t => t.StartsWith("c ")))
            {
                Count++;
                var splits = t.Split(null);
                var name = splits[2];
                //Console.WriteLine(name);
                FeatureList.Add(name);
                //number = Convert.ToInt32(splits[1].Contains("$") ? splits[1].Replace("$", "") : splits[1]);
            }
        }


        public VariabilityModel Parse()
        {
            var lines = File.ReadAllLines(FileName);
            foreach (var t in lines)
            {
                if (t.StartsWith("c "))
                {
                   
                    ParseCommentLine(t);
                }
                else if (t.StartsWith("p cnf"))
                {
                    continue;
                }
                else
                {
                   
                    ParseClauseLine(t);
                }
            }
            model.saveXML("test.xml");
            return model;
        }



        private void ParseCommentLine(string line) {
            var splits = line.Split(null);
            var name = splits[2];
         
            model.addConfigurationOption(new BinaryOption(model, name) {Optional = true});
        }

        private void ParseClauseLine(string line)
        {

            var literals = line.Split(null);
            if (literals.Length == 1) return;
            if (literals.Length == 2)
            {
                //TODO
            }
            else
            {
                var binopt = ""; 
                for (var i = 0; i < literals.Length - 2; i++)
                {
                    var value = Convert.ToInt32(literals[i]);
                    var val = Math.Abs(value);

                    var name =FeatureList[val - 1];
                    if (value < 0)
                    {
                        binopt = binopt + "-" + name + " | ";
                    }
                    else {
                        binopt = binopt + name + " | ";
                    }
                    
                }
                var lastval = Convert.ToInt32(literals[literals.Length - 2]);
                if (lastval < 0)
                {
                    binopt = binopt + "-" + FeatureList[Math.Abs(lastval) - 1];
                }
                else {
                    binopt = binopt + FeatureList[Math.Abs(lastval) - 1];
                }
                //Console.WriteLine(binopt);
                model.BooleanConstraints.Add(binopt);
            }
           

            //line = line.Substring(0, line.Length - 1);
            //line = line.Replace(" ", " | ");
            //Console.WriteLine(line);

            /*

            var clause = new CNFClause(line);

            if (clause.Mandatory)
            {
               // Console.WriteLine("Mandatory " + clause.vars[0]);
                var opt = model.BinaryOptions[clause.vars[0]];
                opt.Optional = false;
                return;
            }

            if (clause.ToRemove)
            {
                //toRemove.Add(model.BinaryOptions[Math.Abs(clause.vars[0])]);
                return;
            }

            if (clause.Imply)
            {
                //Console.WriteLine($" {clause.vars[0]} implies {clause.vars[1]}");
                int fi;
                int se;
                if (clause.vars[0] < 0)   //first implies second
                {
                    fi = Math.Abs(clause.vars[0]);
                    se = Math.Abs(clause.vars[1]);
                }
                else   //second implies first
                {
                    fi = Math.Abs(clause.vars[1]);
                    se = Math.Abs(clause.vars[0]);
                }
                var first = model.BinaryOptions[fi].Implied_Options;
                var subimply = new List<ConfigurationOption> {model.BinaryOptions[se]};
                first.Add(subimply);
                return;
            }

            if (clause.Exclude)
            {
                //Console.WriteLine($"Exclude {clause.vars[0]} {clause.vars[1]}");
                var fi = Math.Abs(clause.vars[0]); 
                var se = Math.Abs(clause.vars[1]);
                var first = model.BinaryOptions[fi].Excluded_Options;
                var subexcl = new List<ConfigurationOption> { model.BinaryOptions[se] };
                first.Add(subexcl);

                return;
                //other way too???
            }

            if (clause.AllPositive)
            {
                //Console.WriteLine(clause);
                for (var i = 0; i < clause.vars.Length; i++)
                {
                    var implicant = model.BinaryOptions[Math.Abs(clause.vars[i])];
                    var implied = new List<ConfigurationOption>();
                    for (var j = 0; j < clause.vars.Length; j++)
                    {
                        if (i != j)
                        {
                            implied.Add(model.BinaryOptions[Math.Abs(clause.vars[j])]);
                        }
                    }
                    implicant.Implied_Options.Add(implied);
                }
                return;
            }


            if (clause.AllNegative)
            {
                //Console.WriteLine(clause);
                for (var i = 0; i < clause.vars.Length; i++)
                {
                    var ecluder = model.BinaryOptions[Math.Abs(clause.vars[i])];
                    List<ConfigurationOption> exluded = new List<ConfigurationOption>();
                    for (var j = 0; j < clause.vars.Length; j++)
                    {
                        if (i != j)
                        {
                            exluded.Add(model.BinaryOptions[Math.Abs(clause.vars[j])]);
                        }
                    }
                    ecluder.Excluded_Options.Add(exluded);
                }
                return;
            }

            
            if (clause.OnePositive)
            {
                //Console.WriteLine(line);
            }
            if (clause.OneNegative)
            {
                Console.WriteLine(line);
            } */
        }
    }
}
