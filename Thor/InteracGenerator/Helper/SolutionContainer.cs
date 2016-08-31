using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace InteracGenerator
{
    public class SolutionContainer
    {
        [JsonIgnore]
        public string DisplayName { get; set; }

        public int FoundAtEval { get; set; }
        public string TestName { get; set; }
        public double FeatureTVal { get; set; }
        public double VariantTVal { get; set; }
        public double InteracTVal { get; set; }

        public double AdditionalFeatureCmv { get; set; }
        public double AdditionalVariantCmv { get; set; }

        public Distribution Features { get; set; }

        [JsonIgnore]
        public Distribution TargetFeatures { get; set; }

        public Distribution Interaction { get; set; }
        public Distribution Variant { get; set; }
        public Distribution TargetVariant { get; set; }
        //public Distribution TargetInteraction { get; set; }


        public long CalcTime { get; set; }
        public long FitnessTime { get; set; }
        public long ScaleTime { get; set; }

        

        public void Write(string fileName)
        {
            var todump = JsonConvert.SerializeObject(this);
            File.WriteAllText(fileName, todump);
        }

        public void WriteSolution(string folder)
        {


            folder = folder + Path.DirectorySeparatorChar;
            File.Delete(folder + "featSolution.txt");
            File.Delete(folder + "interacSolution.txt");
            File.Delete(folder + "variantSolution.txt");
            if (Features.Values.Length > 0) File.WriteAllLines(folder + "featSolution.txt", Features.Values.Select(d => d.ToString()).ToArray());
            if (Interaction.Values != null && Interaction.Values.Length > 0) File.WriteAllLines(folder + "interacSolution.txt", Interaction.Values.Select(d => d.ToString()).ToArray());
            if (Variant.Values != null && Variant.Values.Length > 0) File.WriteAllLines(folder + "variantSolution.txt", Variant.Values.Select(d => d.ToString()).ToArray());
        }
    }
}
