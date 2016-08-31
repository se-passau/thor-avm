using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intergen
{
    public enum DistType {  Normal, LogNormal, LeftSkew }

    class Distribution
    {

        public int Size { get; set; }

        public Double[] Values { get; set;  }

        public Distribution() { }

        public Distribution(DistType type)
        {
        }

        public Distribution(DistType type, double min, double max, double mean, double sd, int size) {
            Size = size;

            REngine engine = REngine.GetInstance();

            if (type == DistType.Normal) {
                Console.WriteLine("Creating normal");
                
                var res = engine.Evaluate( String.Format("x<-seq({0},{1},length={2})", min, max, size));
                // resArr = res.AsNumeric().ToArray<double>();

                var result = engine.Evaluate(String.Format("y <- rnorm (x, mean={0}, sd={1})", mean, sd));
                Values =  result.AsNumeric().ToArray<double>();
            }

            if (type == DistType.LogNormal) {
                var res = engine.Evaluate(String.Format("x<-seq({0},{1},length={2})", min, max, size));

                var result = engine.Evaluate(String.Format("y <- dlnorm(x, meanlog={0}, sd={1})", mean, sd));
                Values = result.AsNumeric().ToArray<double>();
            }
        }
    }
}
