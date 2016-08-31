using SPLConqueror_Core;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace InteracGenerator
{
    public class Distribution
    {

        public enum DistributionType
        {
            Feature,
            Interaction,
            Variant
        }

        [JsonIgnore]
        public DistributionType DistType { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public string DisplayName { get; set; }

        [JsonIgnore]
        public NFProperty SelectedNfProperty;
        public double[] Values { get; set; }

        [JsonIgnore]
        public string ImagePath { get; set; }

        [JsonIgnore]
        public bool Scaled { get; set; }

        public Distribution(string name, string prop, DistributionType type) {
            Name = name;
            SelectedNfProperty = GlobalState.getOrCreateProperty(prop);
            DistType = type;
            Scaled = false;
        }

        [JsonConstructor]
        public Distribution(double[] values)
        {
            Values = values;
        }

        public void ReadFile(bool variant)
        {
            var path = "";
            if (variant) {
                path = Path.Combine(Environment.CurrentDirectory, @"FeatureValues\" + SelectedNfProperty + Path.DirectorySeparatorChar + "variants\\"+ Name);
            }
            else {
                path = Path.Combine(Environment.CurrentDirectory, @"FeatureValues\" + SelectedNfProperty + Path.DirectorySeparatorChar + Name);
            }
 
            string[] lines = File.ReadAllLines(path);
            if (lines.Length > 0) {

                //remove surrounding [ val1, val2, .. ]
                if (lines[0].Contains('['))
                {
                    lines[0] = lines[0].Remove(0, 1);
                    lines[0] = lines[0].Remove(lines[0].Length - 1, 1);
                }
                var split = lines[0].Split(',');
                Values = Array.ConvertAll(split, double.Parse);   
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
