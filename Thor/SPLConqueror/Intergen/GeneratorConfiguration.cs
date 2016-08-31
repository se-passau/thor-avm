using JMetalCSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intergen
{
    public class GeneratorConfiguration
    {

        public string Name { get; set; }

        public string PopulationSize { get; set; }

        public Algorithm Algorithm { get; set; }

        public Problem Problem { get; set; }



        public GeneratorConfiguration()
        {
        }

        public void Load(String fileName)
        { 
            String[] file = System.IO.File.ReadAllLines(fileName);

            for (int i = 0; i < file.Length; i++) {
                Console.WriteLine(file);
            }
        }

        public void Save(String fileName)
        {
            string[] config = new string[10];
            System.IO.File.WriteAllLines(fileName, config);
        }
    }
}
