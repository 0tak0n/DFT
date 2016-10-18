using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing.Imaging;

namespace DFT
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            FrequencyThroughTime ftt = new FrequencyThroughTime() { SamplesPerSecond = 44010};


            Bitmap bmpA = new Bitmap(Image.FromFile("d:\\demo1_filtereda.png", true));
            Bitmap bmpB = new Bitmap(Image.FromFile("d:\\demo1_filteredb.png", true));

            for (int y = 0; y < bmpA.Height; y++)
            {
                FrequencySample fs = new FrequencySample() { windowLength = (uint)bmpA.Width, Samples = new List<Complex>() };

                for (int x = 0; x < bmpA.Width; x++)
                {
                    Color c = bmpA.GetPixel(x, y);
                    Color c2 = bmpB.GetPixel(x, y);

                    fs.Samples.Add(new Complex(((c.B) << 8) + c2.B - short.MaxValue, ((c.G) << 8) + c2.G - short.MaxValue));
                }

                ftt.Samples.Add(fs);
            }

            WAV.Write("d:\\sample2.wav", ftt.Samples.SelectMany(x => SimpleDFT.SimpleDFT.IDFT(x.Samples)).Select(x => (short)x).ToList(), ftt.SamplesPerSecond);
        }
    }
}
