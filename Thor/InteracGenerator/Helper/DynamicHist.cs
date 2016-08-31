
namespace InteracGenerator
{



    public class DynamicHist
    {

        public bool IsForFeature { get; set; }

        private bool _useSquareRoot;
        public bool UseSquareRoot {
            get { return _useSquareRoot; }
            set {
                if (value)
                {
                    UseScott = false;
                    UseSturges = false;
                    UseCustomStatic = false;
                }
                _useSquareRoot = value;
            }
        }

        private bool _useScott;
        public bool UseScott {
            get { return _useScott; }
            set
            {
                if (value)
                {
                    UseSturges = false;
                    UseSquareRoot = false;
                    UseCustomStatic = false;
                }
                _useScott = value;
            }
        }

        private bool _useSturges;

        public bool UseSturges {
            get { return _useSturges; }
            set
            {
                if (value)
                {
                    UseScott = false;
                    UseSquareRoot = false;
                    UseCustomStatic = false;
                }
                _useSturges = value;
            }
        }

        private bool _useCustom ;
        private double _slope;
        private double _yinterc;
        private readonly InterGen _model;

        public bool UseCustomStatic {
            get { return _useCustom;  }
            set
            {
                if (value)
                {
                    UseSquareRoot = false;
                    UseSturges = false;
                    UseScott = false;
                }
                _useCustom = value;
            }
        }

        public int CustomStaticSize { get; set; }

        public int StartEvolution { get; set; }
        public int EndEvolution { get; set; }
        public int StartBins { get; set; }
        public int EndBins { get; set; }
        public int Stepping { get; set; }

        public DynamicHist(InterGen model)
        {
            _model = model;
        }

        public DynamicHist(int startEvo, int endEvo, int startS, int endS, int step)
        {
            StartEvolution = startEvo;
            EndEvolution = endEvo;
            StartBins = startS;
            EndBins = endS;
            Stepping = step;
        }

        public int GetBinSize(int currentEvolution)
        {
           
            if (currentEvolution <= StartEvolution) return StartBins;
            if (currentEvolution > EndEvolution) return EndBins;
            var size  = (int) (_slope*currentEvolution + _yinterc);
           
            return size;
        }

        public void CalcLinear()
        {
            _slope = (EndBins - StartBins)/((double)EndEvolution - StartEvolution);
            _yinterc = EndBins - (_slope*EndEvolution);
           
            //Console.WriteLine("Function: y = " + slope + " x + "  + yinterc);

            RIntegrator.DrawDynamicBin(StartBins, StartEvolution, EndBins, EndEvolution, _model.Setting.MaxEvaluations, _slope, _yinterc);
            //indi.f < -function(x){ (10) * (x <= 10) + (x) * ((10 < x) & (x < 50)) + (50) * (x >= 50)}
            // plot(x, indi.f(x))
            //plot(x, indi.f(x), ylim = c(0, 60)

           
        }
    }
}
