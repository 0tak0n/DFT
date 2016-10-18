using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFT
{
    public partial class RangeSample : Form
    {
        int windowLength = 44;

        public RangeSample()
        {
            InitializeComponent();

            for (int s = 1; s < 10; s++){
                windowLength *= 2;
            for (int i = 1; i < 100; i++)
            {
                refresh(i);
            }
            }
            
        }

        public void refresh(double freq)
        {

            List<double> samples = new List<double>();
            double samplingRate = 4400;
            
            for (int i = 0; i < 4400; i++)
            {
                double value = 0;
                
                value += Math.Sin(freq * Math.PI * 2 * i / samplingRate);
                //value += 0.2 * Math.Sin(freq * 1.5 * Math.PI * 2 * i / samplingRate);
                //value = value > 0.5 ? 1 : -1;
                samples.Add(value);
            }

            List<double> windowInt = new List<double>();

            for (int from = 0; from < samples.Count - windowLength; from++)
            {
                double value = 0;

                for (int i = 0; i < windowLength; i++)
                {
                    value += samples[from + i];
                }

                value /= windowLength;
                windowInt.Add(value);
            }


            //total db
            var total = 0.0;
            foreach (var p in windowInt)
                total = Math.Max(total, Math.Abs(p));
            chart1.Series[0].Points.Add(total);    
            /*
            chart1.Series[0].Points.Clear();

            foreach (var p in samples)
                chart1.Series[0].Points.Add(p);

            chart2.Series[0].Points.Clear();

            foreach (var p in windowInt)
                chart2.Series[0].Points.Add(p);
             */

        }

        private void freq_in_ValueChanged(object sender, EventArgs e)
        {
            refresh((double)freq_in.Value);
        }

    }
}
