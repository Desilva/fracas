using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Utilities.Statistical_Engine
{
    public class MathFunction
    {
        #region Static Variables ------------------------------------------------

        private static readonly double MACHINE_EPSILON = 1.0e-12;

        private readonly static double[] a = 
        {
          2.2352520354606839287e00,1.6102823106855587881e02,1.0676894854603709582e03,
          1.8154981253343561249e04,6.5682337918207449113e-2
        };

        private readonly static double[] b = 
        {
          4.7202581904688241870e01,9.7609855173777669322e02,1.0260932208618978205e04,
          4.5507789335026729956e04
        };

        private static readonly double[] c = 
        {
          3.9894151208813466764e-1,8.8831497943883759412e00,9.3506656132177855979e01,
          5.9727027639480026226e02,2.4945375852903726711e03,6.8481904505362823326e03,
          1.1602651437647350124e04,9.8427148383839780218e03,1.0765576773720192317e-8
        };

        private static readonly double[] d = 
        {
          2.2266688044328115691e01,2.3538790178262499861e02,1.5193775994075548050e03,
          6.4855582982667607550e03,1.8615571640885098091e04,3.4900952721145977266e04,
          3.8912003286093271411e04,1.9685429676859990727e04
        };

        private static readonly double half = 0.5e0;

        private static readonly double e = 2.7182818284590452353602874713527;

        private static readonly double[] p = 
        {
          2.1589853405795699e-1,1.274011611602473639e-1,2.2235277870649807e-2,
          1.421619193227893466e-3,2.9112874951168792e-5,2.307344176494017303e-2
        };

        private static readonly double one = 1.0e0;

        private static readonly double[] q = 
        {
          1.28426009614491121e00,4.68238212480865118e-1,6.59881378689285515e-2,
          3.78239633202758244e-3,7.29751555083966205e-5
        };

        private static readonly double sixten = 1.60e0;
        private static readonly double sqrpi = 3.9894228040143267794e-1;
        private static readonly double thrsh = 0.66291e0;
        private static readonly double root32 = 5.656854248e0;
        private static readonly double zero = 0.0e0;

        private static readonly double OneOverRoot2Pi = 1.0 / Math.Sqrt(2 * Math.PI);

        #endregion Static Variables


        #region Instance Variables ----------------------------------------------

        private double mean_;
        private double sigma_;
        private double oneOverSigma_;
        private double oneOverSigmaSqr_;
        private double betha_;
        private double nano_;
        private double etha_;
        // c_ is just oneOverSigma_*OneOverRoot2Pi
        private double c_;
        private double r_;

        #endregion Instance Variables


        public double Etha
        {
            get
            {
                return etha_;
            }
        }

        public double Betha
        {
            get
            {
                return betha_;
            }
        }

        #region Constructors ----------------------------------------------------

        /// <summary>
        /// Constructs a NormalDist instance with the given mean and variance.
        /// </summary>
        /// <param name="mean">The mean of the density.</param>
        /// <param name="var">The variance of the density. Must be positive.</param>
        /// <exception cref="Exception">Thrown if the variance is less 
        /// than or equal to zero.</exception>
        /// <remarks>The variance of the distribution is the standard deviation squared.</remarks>

        public MathFunction() { }
        #endregion Constructors


        #region Properties ------------------------------------------------------

        public void SetNormalDist(double mean, double var)
        {
            mean_ = mean;
            Variance = var;
        }

        public void SetExponentialDist(double mean)
        {
            mean_ = mean;
        }

        public void SetWeibulDist(double[] x)
        {
            BethaFunction(x);
            NanoFunction(x);
        }
        /// <summary>
        /// Gets and sets the mean of the density.
        /// </summary>
        public double Mean
        {
            get
            {
                return mean_;
            }

            set
            {
                mean_ = value;
            }
        }

        /// <summary>
        /// Gets and sets the variance of the density.
        /// </summary>
        /// <exception cref="Exception">Thrown if the variance is less 
        /// than or equal to zero.</exception>
        /// <remarks>The variance of the density is the standard deviation squared.</remarks>
        public double Variance
        {
            get
            {
                return sigma_;
            }

            set
            {
                if (value <= 0.0)
                {
                    string msg = string.Format("Expected variance > 0 in NormalDistribution. Found variance = {0}", value);
                    throw new Exception(msg);
                }
                sigma_ = Math.Sqrt(value);
                oneOverSigma_ = 1.0 / sigma_;
                oneOverSigmaSqr_ = oneOverSigma_ * oneOverSigma_;
                c_ = oneOverSigma_ * OneOverRoot2Pi;
            }
        }

        /// <summary>
        /// Gets the skewness, a measure of the degree of asymmetry of 
        /// this density. 
        /// </summary>
        /// <remarks>
        /// The skewness is the third central moment divided by the cube of 
        /// the standard deviation.
        /// </remarks>
        public double Skewness
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets the kurtosis, a measure of the degree of peakednesss of the
        /// density.
        /// </summary>
        /// <remarks>
        /// The kurtosis is the fourth centeral moment divided by the
        /// fouth power of the standard deviation, normalized so that
        /// the kurtosis if the normal density is zero. 
        /// </remarks>
        public double Kurtosis
        {
            get
            {
                return 0.0;
            }
        }

        #endregion Properties


        #region Member Functions ------------------------------------------------


        #region pdf
        public double PDF(double x)
        {
            double y = (x - mean_);
            double xMinusMuSqr = y * y;

            // c_ is a constant equal to one over sigma times one over square root of 2 PI
            return c_ * Math.Exp(-0.5 * xMinusMuSqr * oneOverSigmaSqr_);
        }

        public double PDFExp(double x)
        {

            // c_ is a constant equal to one over sigma times one over square root of 2 PI
            return 1 / mean_ * Math.Exp(-x / mean_);

        }

        public double PDFWeibul(double x)
        {
            return (betha_ / etha_) * Math.Pow((x / etha_), (betha_ - 1)) * Math.Exp(-Math.Pow((x / etha_), betha_));
        }

        public double PDFWeibulRank(double i, double n)
        {
            return (i - 0.3) / (n + 0.4);
        }

        #endregion pdf

        #region cdf
        public double CDF(double x)
        {
            int i;
            double del, temp, z, xden, xnum, y, xsq, min;
            double result, ccum;
            double arg = (x - mean_) / sigma_;

            min = Double.Epsilon;
            z = arg;
            y = Math.Abs(z);
            if (y <= thrsh)
            {
                //
                // Evaluate  anorm  for  |X| <= 0.66291
                //
                xsq = zero;
                if (y > MACHINE_EPSILON) xsq = z * z;
                xnum = a[4] * xsq;
                xden = xsq;
                for (i = 0; i < 3; i++)
                {
                    xnum = (xnum + a[i]) * xsq;
                    xden = (xden + b[i]) * xsq;
                }
                result = z * (xnum + a[3]) / (xden + b[3]);
                temp = result;
                result = half + temp;
                ccum = half - temp;
            }

            //
            // Evaluate  anorm  for 0.66291 <= |X| <= sqrt(32)
            //
            else if (y <= root32)
            {
                xnum = c[8] * y;
                xden = y;
                for (i = 0; i < 7; i++)
                {
                    xnum = (xnum + c[i]) * y;
                    xden = (xden + d[i]) * y;
                }
                result = (xnum + c[7]) / (xden + d[7]);
                xsq = Math.Floor(y * sixten) / sixten;
                del = (y - xsq) * (y + xsq);
                result = Math.Exp(-(xsq * xsq * half)) * Math.Exp(-(del * half)) * result;
                ccum = one - result;
                if (z > zero)
                {
                    temp = result;
                    result = ccum;
                    ccum = temp;
                }
            }

            //
            // Evaluate  anorm  for |X| > sqrt(32)
            //
            else
            {
                result = zero;
                xsq = one / (z * z);
                xnum = p[5] * xsq;
                xden = xsq;
                for (i = 0; i < 4; i++)
                {
                    xnum = (xnum + p[i]) * xsq;
                    xden = (xden + q[i]) * xsq;
                }
                result = xsq * (xnum + p[4]) / (xden + q[4]);
                result = (sqrpi - result) / y;
                xsq = Math.Floor(z * sixten) / sixten;
                del = (z - xsq) * (z + xsq);
                result = Math.Exp(-(xsq * xsq * half)) * Math.Exp(-(del * half)) * result;
                ccum = one - result;
                if (z > zero)
                {
                    temp = result;
                    result = ccum;
                    ccum = temp;
                }
            }

            if (result < min) result = 0.0e0;
            //
            // Fix up for negative argument, erf, etc.
            //
            if (ccum < min) ccum = 0.0e0;

            return result;
        }

        public double CDFExp(double x)
        {

            // c_ is a constant equal to one over sigma times one over square root of 2 PI
            return 1 - Math.Exp(-mean_ * x);
        }


        #endregion cdf


        //tipe :
        //  e = exponential
        //  n = normal
        //  w = weibul
        public double Reliability(string tipe, double x)
        {
            double result = 0.0;
            if (tipe.Equals("n"))
            {
                result = 1 - CDF(x);
            }
            else if ((tipe.Equals("e")))
            {
                result = Math.Exp(-mean_ * x);
            }
            return result;
        }

        public double FailureRate(string tipe, double x)
        {
            double result = 0.0;
            if (tipe.Equals("n"))
            {
                result = PDF(x) / Reliability(tipe, x);
            }
            else if ((tipe.Equals("e")))
            {
                result = 1 / mean_;
            }
            return result;
        }

        public double MTTF(string tipe, double failure_rate)
        {
            if (tipe.Equals("w"))
            {
                r_ = Config.GetGamma(String.Format("{0:0.00}", (1 + (1 / betha_))));
                return etha_ * r_;
            }
            else
            {
                return 1 / failure_rate;
            }
        }

        public double SumFunction(double[] data)
        {
            double sum = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }
            return sum;
        }

        public double MeanFunction(double[] data)
        {

            return SumFunction(data) / data.Length;
        }

        public double VarianceFunction(double[] data)
        {
            double sum = 0.0;
            double mean = MeanFunction(data);
            for (int i = 0; i < data.Length; i++)
            {
                sum += (data[i] - mean) * (data[i] - mean);
            }
            return sum / (data.Length - 1);
        }

        //belum
        public void BethaFunction(double[] x)
        {
            int n = x.Length;
            int i = 1;
            double result1 = 0;
            double result2 = 0;
            double result3 = 0;
            double result4 = 0;

            foreach (double t in x)
            {
                result1 += Math.Log(t, e) * Math.Log((-Math.Log(1 - PDFWeibulRank(i, n), e)), e);
                result2 += Math.Log(t, e);
                result3 += Math.Log(-Math.Log(1 - PDFWeibulRank(i, n), e), e);
                result4 += Math.Log(t, e) * Math.Log(t, e);
                i++;
            }


            betha_ = ((n * result1) - (result2 * result3)) / ((n * result4) - (result2 * result2));

        }

        //belum
        public void NanoFunction(double[] x)
        {
            int n = x.Length;
            int i = 1;
            double result1 = 0;
            double result2 = 0;

            foreach (double t in x)
            {
                result1 += Math.Log(t, e);
                result2 += Math.Log((-Math.Log(1 - PDFWeibulRank(i, n), e)), e);
                i++;
            }
            nano_ = (result2 / n) - (betha_ * result1 / n);
            etha_ = etha();
        }

        public double Factorial(double val)
        {
            return val == 1 ? val : val * Factorial(val - 1);
        }

        public double etha()
        {
            double result = Math.Exp(-nano_ / betha_);
            return result;
        }

        public double getMaxValue(double[] thearray)
        {
            double k = new double();
            foreach (double j in thearray)
            {

                if (j > k)
                    k = j;

            }
            return k;
        }

        public int getMaxIndex(double[] thearray)
        {
            double k = 0; int i = 0; int index = 0;
            foreach (double j in thearray)
            {

                if (j > k)
                {
                    k = j;
                    index = i;
                }
                i++;
            }
            return index;
        }

        public double mutlak(double value)
        {
            if (value >= 0)
            {
                return value;
            }
            else
            {
                return value * -1;
            }
        }

        public string ConvertData(double input)
        {
            string result = "";
            string temp = input.ToString();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == ',')
                {
                    result += '.';
                    //return float.Parse(result);
                }
                else
                {
                    result += temp[i];
                }
            }
            return result;
        }

        public double[] MaxDistribusiNormal(double[] durasi)
        {
            double[] result = new double[durasi.Length];

            SetNormalDist(MeanFunction(durasi), VarianceFunction(durasi));

            for (int i = 0; i < durasi.Length; i++)
            {
                result[i] = PDF(durasi[i]);
            }

            return result;
        }

        public double[] MaxDistribusiExp(double[] durasi)
        {
            double[] result = new double[durasi.Length];

            SetExponentialDist(MeanFunction(durasi));

            for (int i = 0; i < durasi.Length; i++)
            {
                double a = PDFExp(durasi[i]);
                result[i] = PDFExp(durasi[i]);
            }

            return result;
        }

        public double[] MaxDistribusiWeibull(double[] durasi)
        {
            double[] result = new double[durasi.Length];

            for (int i = 0; i < durasi.Length; i++)
            {
                SetWeibulDist(durasi);
                result[i] = PDFWeibul(durasi[i]);
            }

            return result;
        }

        #endregion Member Functions
    }
}