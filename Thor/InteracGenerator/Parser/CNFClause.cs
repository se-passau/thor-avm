using System;
using System.Linq;

namespace InteracGenerator.Parser
{
    class CNFClause
    {

        public int[] vars;


        public int[] Feat; 

        public bool Mandatory { get; set; }
        public bool ToRemove { get; set;  }
        public bool Imply { get; set; }
        public bool Exclude { get; set; }

        public bool AllNegative { get; set; }
        public bool AllPositive { get; set; }
        public bool OnePositive { get; set; }
        public bool OneNegative { get; set; }

        public CNFClause(string line)
        {
            var vrs = line.Split(null);
            vars = new int[vrs.Length -1 ];
            for (var i = 0; i < vars.Length; i++)
            {
                vars[i] = Convert.ToInt32(vrs[i]);
            }

            if (vars.Length == 1 && vars[0] > 0)
            {
                Mandatory = true;
                return;
            }

            if (vars.Length == 1 && vars[0] < 0)
            {
               ToRemove = true;
                return;
            }

            if (vars.Length == 2)
            {
                //both negative
                if (vars[0] < 0 && vars[1] < 0)
                {
                    
                    Exclude = true;
                    return;
                }
                //one negative one positive
                if (vars[0] < 0 && vars[1] > 0)
                {
                    Imply = true;
                    return;
                }

                //one negative one positive
                if (vars[0] > 0 && vars[1] < 0)
                {
                    Imply = true;
                    return;
                }

            }
            if (vars.Length > 2)
            {

                if (vars.Max() < 0) {
                    AllNegative = true;
                    return;
                }
               
                
                if (vars.Min() > 0) { AllPositive = true;
                    return;
                }

                if (vars[0] < 0 && vars[1] > 0)
                {
                    OneNegative = true;
                    return;
                }
                OnePositive = true;

            }

            
        }

        public override string ToString()
        {
            return vars.Aggregate("", (current, t) => current + " " + t + " ");
        }
    }
}
