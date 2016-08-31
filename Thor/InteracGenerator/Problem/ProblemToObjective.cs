using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteracGenerator.Problem
{

    

    static class ObjectiveMapping
    {
        public static List<bool> Feat = new List<bool> { true, false, false };
        public static List<bool> Interac = new List<bool> {false, true, false};
        public static List<bool> Variant = new List<bool> {false, false, true};
        public static List<bool> FeatInterac = new List<bool> {true, true, false};
        public static List<bool> FeatVariant = new List<bool> {true, false, true};
        public static List<bool> InteracVariant = new List<bool> {false, true, true};
        public static List<bool> Complete = new List<bool> {true, true, true};

        public static List<bool> GetList(InterGen.ProbType type)
        {
            switch (type)
            {
                case InterGen.ProbType.Feat:
                    return Feat;
                case InterGen.ProbType.Interac:
                    return Interac;
                case InterGen.ProbType.InteracVariant:
                    return InteracVariant;
                case InterGen.ProbType.Complete:
                    return Complete;
                case InterGen.ProbType.Variant:
                    return Variant;
                case InterGen.ProbType.FeatInterac:
                    return FeatInterac;
                case InterGen.ProbType.FeatVariant:
                    return FeatVariant;
            }
            return null;
        }
    }
}
