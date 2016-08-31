using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InteracGenerator.FitnessCalculation;

namespace InteracGenerator.Problem
{
    internal struct Ranks
    {
        public double[] First;
        public double[] Second;
    }

    public class CramerVonMises : IFitnessTest
    {
        private readonly double[] First;
        private readonly double[] Second;

        private double[] FirstRank;
        private double[] SecondRank;


        public CramerVonMises() { }

        public CramerVonMises(double[] first, double[] second)
        {
            First = new double[first.Length];
            Second = new double[second.Length];
            Array.Copy(first, First, first.Length);
            FirstRank = new double[First.Length];
            Array.Copy(second, Second, second.Length);
            SecondRank = new double[Second.Length];
        }


       
        public double Calculate(Distribution firstD, Distribution secondD)
        {
            double[] first = new double[firstD.Values.Length];
            double[] second = new double[secondD.Values.Length];
            Array.Copy(firstD.Values, first, first.Length);
            
            Array.Copy(secondD.Values, second, second.Length);

            double n = first.Length;
            double m = second.Length;
            double U = 0;
            double u1 = 0;
            double u2 = 0;

            Array.Sort(first);
            Array.Sort(second);

            var ranks = ComputeRanks(first, second);


            var tasks = new Task[2];

            tasks[0] = new Task(() => {
                for (int i = 0; i < n; i++)
                {
                    u1 += Math.Pow(ranks.First[i] - (i + 1), 2);
                }
            });
            tasks[1] = new Task(() => {

                for (int i = 0; i < m; i++)
                {
                    u2 += Math.Pow(ranks.Second[i] - (i + 1), 2);
                }
            });
            tasks[0].Start();
            tasks[1].Start();

            Task.WaitAll(tasks);

            U = n * u1 + m * u2;
            var result = U / (n * m * (n + m)) - (4 * m * n - 1) / (6.0 * (m + n));
            //Console.WriteLine(result);
            return result;
        }


        /*public double Calculate()
        {
            int N = First.Length;
            int M = Second.Length;
            double U = 0;
            double u1 = 0;
            double u2 = 0;
           
            Array.Sort(First);
            Array.Sort(Second);

            //ComputeRanks();


            var tasks = new Task[2];

            tasks[0] = new Task(()=> {
                for (int i = 0; i < N; i++)
                {
                    u1 += Math.Pow(FirstRank[i] - (i + 1), 2);
                }
            });
            tasks[1] = new Task(() => {

                for (int i = 0; i < M; i++)
                {
                    u2 += Math.Pow(SecondRank[i] - (i + 1), 2);
                }
            });
            tasks[0].Start();
            tasks[1].Start();
           
            Task.WaitAll(tasks);
    
            U = N * u1 + M * u2;
            var result = U / (N * M * (N + M))  -   (4*M*N -1)  / (6.0 * (M+ N));
            //Console.WriteLine(result);
            return result;
        } */

      

        private static Ranks ComputeRanks(IReadOnlyList<double> first, IReadOnlyList<double> second)
        {

            var combined = new double[first.Count + second.Count];
            var firstRank = new double[first.Count];
            var secondRank = new double[second.Count];
            int firstIndex = 0;
            int secondIndex = 0;
            bool secondDone = false;
            bool firstDone = false;

            for (int i = 0; i < combined.Length; i++)
            {

                if (firstDone)
                {
                    combined[i] = second[secondIndex];
                    secondRank[secondIndex] = i + 1;
                    if (secondIndex < second.Count - 1) secondIndex++;
                    continue;
                }

                if (secondDone) {
                    combined[i] = first[firstIndex];
                    firstRank[firstIndex] = i + 1;
                    if (firstIndex < first.Count - 1) firstIndex++;
                    continue;
                }


                if (first[firstIndex] < second[secondIndex])
                {
                    combined[i] = first[firstIndex];
                    firstRank[firstIndex] = i + 1;
                    if (firstIndex < first.Count - 1)
                    {
                        firstIndex++;
                    }
                    else
                    {
                        firstDone = true;
                    }
                }
                else if (first[firstIndex] == second[secondIndex])
                {
                    //midrank calculation  see english wiki page
                    combined[i] = first[firstIndex];
                    combined[i + 1] = second[secondIndex];

                    firstRank[firstIndex] = i + 1.5;
                    secondRank[secondIndex] = i + 1.5;


                    if (firstIndex < first.Count - 1)
                    {
                        firstIndex++;
                    }
                    else
                    {
                        firstDone = true;
                    }
                    if (secondIndex < second.Count - 1)
                    {
                        secondIndex++;
                    }
                    else
                    {
                        secondDone = true;
                    }
                    i++;
                }
                else
                {
                    combined[i] = second[secondIndex];
                    secondRank[secondIndex] = i + 1;
                    if (secondIndex < second.Count - 1)
                    {
                        secondIndex++;
                    }
                    else
                    {
                        secondDone = true;
                    }
                }
            }
            
            return new Ranks {First  = firstRank, Second = secondRank};
        }
    }
}
