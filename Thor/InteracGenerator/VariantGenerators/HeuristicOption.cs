namespace InteracGenerator.VariantGenerators
{
    /// <summary>
    /// Encapsulates variant generation options
    /// </summary>
    internal class HeuristicOption
    {
        public VariantGenerator.Method Method { get; set; }

        public bool HasTimeLimit;
        public bool HasTreshold;
        public bool HasScale;
        public int Modulo;

        private int _timeLimitSeconds;
        public int TimeLimitSeconds
        {
            get { return _timeLimitSeconds;}
            set { _timeLimitSeconds = value;
                if (value != 0) HasTimeLimit = true;
            }
        }

        private int _treshold;
        public int Treshold
        { 
            get { return _treshold;}
            set { _treshold = value;
                if (value != 0) HasTreshold = true;
            }
        }

        private double _scale;
        public double Scale
        {
            get { return _scale;}
            set { _scale = value;
                if (value != 0) HasScale = true;
            }
        }

        public int SolverTimeout = 3;

    }
}
