
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace SimpleDFT
{
    class SimpleDFT
    {
        static double[] Cos = new double[1024];
        static double[] Sin = new double[1024];
        static SimpleDFT()
        {
            for (double i = 0; i < 1024; i++)
            {
                Cos[(int)i] = Math.Cos(2 * Math.PI * i / 1024);
                Sin[(int)i] = Math.Sin(2 * Math.PI * i / 1024);
            }
        }
        /// <summary>
        /// Provides the Discrete Fourier Transform for a real-valued input signal
        /// </summary>
        /// <param name="input">the signal to transform</param>
        /// <param name="partials">the maximum number of partials to calculate. If not value is given it defaults to input/2</param>
        /// <returns>The Cos and Sin components of the signal, respectively</returns>
        public static List<Complex> DFT(IList<double> input, uint start, uint length)
        {
            uint len = length;// input.Length;
            List<Complex> freqs = new List<Complex>();

            //double[] cosDFT = new double[len / 2 + 1];
            //double[] sinDFT = new double[len / 2 + 1];

            uint partials = len / 2;
            int offset = (int)start;

            for (int n = 0; n < partials; n++)
            {
                double cos = 0.0;
                double sin = 0.0;
                double value = 2 * Math.PI * n / len;
                double freq = 0;

                for (int i = 0; i < len; i++)
                {
                    cos += input[(int)start + i] * Math.Cos(2 * Math.PI * (double)n / (double)len * i);
                    sin += input[(int)start + i] * Math.Sin(2 * Math.PI * (double)n / (double)len * i);
                    //cos += input[offset + i] * Math.Cos(freq);
                    //sin += input[offset + i] * Math.Sin(freq);
                    // freq += value;
                }

                freqs.Add(new Complex(cos, sin));
            }

            return freqs;
        }

        /// <summary>
        /// Takes the real-valued Cos and Sin components of Fourier transformed signal and reconstructs the time-domain signal
        /// </summary>
        /// <param name="cos">Array of cos components, containing frequency components from 0 to pi. sin.Length must match cos.Length</param>
        /// <param name="sin">Array of sin components, containing frequency components from 0 to pi. sin.Length must match cos.Length</param>
        /// <param name="len">
        /// The length of the output signal. 
        /// If len < (partials-1)*2 then frequency data will be lost in the output signal. 
        /// if no len parameter is given it defaults to (partials-1)*2
        /// </param>
        /// <returns>the real-valued time-domain signal</returns>
        public static double[] IDFT(IList<Complex> samples)
        {
            int len = (samples.Count) * 2;

            double[] output = new double[len];

            int partials = samples.Count;

            for (int n = 0; n < partials; n++)
            {
                for (int i = 0; i < len; i++)
                {
                    output[i]  += Math.Cos(2 * Math.PI * n / len * i) * samples[n].Real;
                    output[i] += Math.Sin(2 * Math.PI * n / len * i) * samples[n].Imaginary;
                }
            }

            //for (int n = 0; n < partials; n++)
            //{
            //    var value = samples[n];

            //    //double freq = 2 * Math.PI * n / len;
            //    int freq = n;

            //    for (int i = 0; i < len; i++)
            //    {
            //        var pos = ((freq * i << 10) / len) & 1023;

            //        output[i] += Cos[pos] * value.Real;
            //        output[i] += Sin[pos] * value.Imaginary;
            //    }
            //}

            return output;
        }
    }
}
