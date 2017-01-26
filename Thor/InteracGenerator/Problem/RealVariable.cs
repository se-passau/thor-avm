using System;
using JMetalCSharp.Core;
using JMetalCSharp.Utils;

namespace InteracGenerator.Problem
{
    internal class RealVariable : Variable
    {

        /// <summary>
        /// Get or Set the value of the <code>Real</code> encodings.variable.
        /// </summary>
        public new double Value { get; set; }

        /// <summary>
        /// Get or Set the lower bound of the <code>Real</code> encodings.variable.
        /// </summary>
        public new double LowerBound { get; set; }

        /// <summary>
        /// Get or Set the upper bound of the <code>Real</code> encodings.variable.
        /// </summary>
        public new double UpperBound { get; set; }

        public RealVariable()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowerBound">Lower limit for the encodings.variable</param>
        /// <param name="upperBound">Upper limit for the encodings.variable</param>
        public RealVariable(double lowerBound, double upperBound)
        {
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
            this.Value = JMetalRandom.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lowerBound">Lower limit for the encodings.variable</param>
        /// <param name="upperBound">Upper limit for the encodings.variable</param>
        /// <param name="value">Value of the variable</param>
        public RealVariable(double lowerBound, double upperBound, double value)
        {
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
            this.Value = value;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="variable">The encodings.variable to copy.</param>
        public RealVariable(Variable variable)
        {
            this.LowerBound = ((RealVariable)variable).LowerBound;
            this.UpperBound = ((RealVariable)variable).UpperBound;
            this.Value = ((RealVariable)variable).Value;
        }

        /// <summary>
        /// Returns a exact copy of the <code>Real</code> encodings.variable
        /// </summary>
        /// <returns>the copy</returns>
        public override Variable DeepCopy()
        {
            Variable result;
            try
            {
                result = new RealVariable(this);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(this.GetType().FullName + ".DeepCopy()", ex);
                Console.WriteLine("Error in " + this.GetType().FullName + ".DeepCopy()");
                result = null;
            }

            return result;
        }

        // <summary>
        /// Returns a string representing the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
