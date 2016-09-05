using SPLConqueror_Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InteracGenerator.Parser
{
    internal class SplotParser
    {
        public enum Mode { Start, MetaData, MetaEnd, FeatureTree, FeatureEnd, Constraint, ConstraintEnd, Done }
        private int previousLevel;
        public VariabilityModel model;
        private string FileName;
        private BinaryOption lastAddition;
        private bool exlusiveMode;
        private Dictionary<int, BinaryOption> ParentOnLevel;
        private Dictionary<int, List<BinaryOption>> ExcludesOnLevel;

        public SplotParser(string fileName) {
            model = new VariabilityModel("Generated");
            FileName = fileName;
        }

        public VariabilityModel Parse() { 
            Mode current = Mode.Start;
            lastAddition = model.Root;
            ParentOnLevel = new Dictionary<int, BinaryOption>();
            ExcludesOnLevel = new Dictionary<int, List<BinaryOption>>();
            //ParentOnLevel.Add(1, model.Root);

            string[] lines = File.ReadAllLines(FileName);

            foreach (var t in lines)
            {
                if (t.Contains("<feature_tree>"))
                {
                    current = Mode.FeatureTree;
                    continue;
                }
                if (t.Contains("</feature_tree>"))
                {
                    current = Mode.FeatureEnd;
                    continue;
                }
                if (t.Contains("<constraints>"))
                {
                    current = Mode.Constraint;
                    continue;
                }
                if (t.Contains("</constraints>"))
                {
                    current = Mode.ConstraintEnd;
                    continue;
                }
                if (t.Contains("<meta>"))
                {
                    current = Mode.MetaData;
                    continue;
                }
                if (t.Contains("</meta>"))
                {
                    current = Mode.MetaEnd;
                    continue;
                }
                else {
                }

                ParseLine(t, current);
            }

            model.saveXML("model.xml");
            return model;
        }

        private void ParseLine(string line, Mode current)
        {
            switch (current)
            {
                case Mode.Start:
                    break;
                case Mode.FeatureTree:
                    ParseOneFeatureTreeLine(line);
                    break;
                case Mode.FeatureEnd:
                    break;
                case Mode.Constraint:
                    ParseOneConstraintLine(line);
                    break;
                case Mode.ConstraintEnd:
                    break;
                default:
                    break;
            }
        }

        #region FeatureTreeParsing

        private void ParseOneFeatureTreeLine(string line) {
            int lineLevel = CheckLevel(line);

            if (line.StartsWith(":r"))
            {
                //root
            }
            else if (line.StartsWith("\t"))
            {
                line = line.TrimStart();
                if (previousLevel < lineLevel)
                {
                    //save the parent on this linelevel
                    ParentOnLevel.Add(lineLevel, lastAddition);
                }
                else if (previousLevel == lineLevel)
                {
                    //TODO Exists a case where there is something to handle
                }
                else
                {

                    //Next Feature has a lower lineLevel -> It is higher up in the FM
                    for (int i = lineLevel + 1; i <= previousLevel; i++)
                    {
                        //next Features with higher lineLevel have new parents
                        ParentOnLevel.Remove(i);

                        //we can finish the exclusive option group now, now more features will come
                        if (ExcludesOnLevel.ContainsKey(i))
                        {
                            CompleteExcludeGroup(ExcludesOnLevel[i]);
                            ExcludesOnLevel.Remove(i);
                        }
                    }
                }

                if (line.StartsWith(":o"))
                {
                    HandleNewOption(line, lineLevel, /* optional */ true);

                }
                else if (line.StartsWith(":m"))
                {
                    HandleNewOption(line, lineLevel, /* optional */ false);
                }
                else if (line.StartsWith(":g"))
                {
                    //TODO IS THIS ENOUGH   WHAT ABOUT [1,3] when 5 features are grouped
                    exlusiveMode = line.Contains("[1,1]");
                }

                // :o   :m  :g  already filtered out ->  these are grouped features
                else if (line.StartsWith(":"))
                {
                    HandleNewGroupedFeature(line, lineLevel);
                }
                else
                {
                    Console.WriteLine("Something went wrong @" + line);
                }
            }
            previousLevel = lineLevel;
        }


        private void HandleNewGroupedFeature(string line, int lineLevel)
        {
            var name = line.Split('(')[0];
            name = name.Remove(0, 2);

            var id = (line.Split('(')[1]);
            id = id.Remove(id.Length - 1, 1);

            BinaryOption opt = new BinaryOption(model, id);
            opt.OutputString = name;
            opt.Optional = true;
            opt.Parent = ParentOnLevel[lineLevel - 1];
            model.addConfigurationOption(opt);
            
            lastAddition = opt;

            HandleGroupedFeatureExcludes(lineLevel, opt);
        }

        private void HandleGroupedFeatureExcludes(int lineLevel, BinaryOption opt) {

            if (exlusiveMode)
            {
                opt.Optional = false;
                if (ExcludesOnLevel.ContainsKey(lineLevel))
                {
                    ExcludesOnLevel[lineLevel].Add(opt);
                }
                else
                {
                    List<BinaryOption> excludes = new List<BinaryOption>();
                    excludes.Add(opt);
                    ExcludesOnLevel.Add(lineLevel, excludes);
                }
            }
        }

        private void HandleNewOption(string line, int lineLevel, bool optional)
        {
            var name = line.Split('(')[0];
            name = name.Remove(0, 3);

            var id = (line.Split('(')[1]);
            id = id.Remove(id.Length - 1, 1);

            BinaryOption opt = new BinaryOption(model, id);
            opt.OutputString = name;
            opt.Optional = optional;
            opt.Parent = ParentOnLevel[lineLevel];
            model.addConfigurationOption(opt);
            lastAddition = opt;
        }

        private void CompleteExcludeGroup(List<BinaryOption> ExcludingOptions) {
            foreach (BinaryOption opt in ExcludingOptions) {

                
                foreach (BinaryOption excludedOption in ExcludingOptions) {
                    List<ConfigurationOption> excludes = new List<ConfigurationOption>();

                    if (opt != excludedOption) {
                        excludes.Add(excludedOption);
                        opt.Excluded_Options.Add(excludes);
                    }
                }
            }
        }


        private static int CheckLevel(string line)
        {
            return line.Count(c => c == '\t');
        }

        #endregion

        #region ConstraintParsing

        private void ParseOneConstraintLine(String line)
        {
            var split = line.Split(':');
            var constraintName = split[0];
            var expression = split[1];

            //Console.WriteLine(expression);
            ParseExpression(expression);
        }

        private void ParseExpression(string expression)
        {

            var exp = expression.Replace("~", "!");
            exp = exp.Replace(" or ", " | ");
            exp = exp.Replace(" and ", " & ");

            /*string[] separator = new string[] { "or" };
            var clauses = expression.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            string SPLConqerorResult = "";
            foreach (string literal in clauses)
            {
                var replacedLiteral = "";
                if (literal.Contains('~'))
                {
                    //NEGATION
                    replacedLiteral = literal.Replace("~", "!");
                }
                else {
                    replacedLiteral = literal;
                }
                SPLConqerorResult += replacedLiteral;
            }
            */
            model.BooleanConstraints.Add(exp);
        }
        #endregion

    }
}
