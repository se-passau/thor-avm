using System;
using System.Collections.Generic;
using Accord.Math;
using JMetalCSharp.Core;

namespace InteracGenerator.Pareto
{
    class ParetoSolver
    {

        private SolutionSet _solutions;
        private int _solution;
        //private int _objectives;


        private double[] _f1Scaled;
        private double[] _f2Scaled;
        private double[] _f3Scaled;

        public ParetoSolver(SolutionSet solutions)
        {
            _solutions = solutions;
            var size = _solutions.GetObjectives().GetUpperBound(0) + 1;
            var ocount = _solutions.GetObjectives()[0].Length;
            var objectives = _solutions.GetObjectives();

            var f1 = new double[size];
            var f2 = new double[size];
            var f3 = new double[] { };
            if (ocount == 3) f3 = new double[size];

            for (var i = 0; i < size; i++)
            {
                f1[i] = objectives[i][0];
                if (ocount > 1) f2[i] = objectives[i][1];
                if (ocount > 2) f3[i] = objectives[i][2];
            }

            var f1min = f1.Min();
            var f1max = f1.Max();
            var f2min = f2.Min();
            var f2max = f2.Max();

            double f3min = 0;
            double f3max = 0;

            if (ocount == 3)
            {
                f3min = f3.Min();
                f3max = f3.Max();
            }

            _f1Scaled = new double[size];
            _f2Scaled = new double[size];
            _f3Scaled = new double[size];
            for (var i = 0; i < size; i++)
            {
                _f1Scaled[i] = FMScaling.ScaleFromTo(f1min, f1max, 0, 1, f1[i]);
                if (ocount > 1)_f2Scaled[i] = FMScaling.ScaleFromTo(f2min, f2max, 0, 1, f2[i]);
                if (ocount == 3) _f3Scaled[i] = FMScaling.ScaleFromTo(f3min, f3max, 0, 1, f3[i]);
            }
        }



        public Solution GetBest(double w1, double w2, double w3)
        {
            var size = _solutions.Size();
            _solution = -1;
            var min = double.MaxValue;
            double wsum = -1;
            for (var i = 0; i < size; i++)
            {

                wsum = w1*_f1Scaled[i] + w2*_f2Scaled[i] + w3*_f3Scaled[i];
                if (!(wsum < min)) continue;
                min = wsum;
                _solution = i;
            }
            //Console.WriteLine("Best solution: " + _solution + "  wsum= "  + wsum);
            return _solutions.Get(_solution);
        }


        public int GetNumber()
        {
            return _solution;
        }
    }
}
