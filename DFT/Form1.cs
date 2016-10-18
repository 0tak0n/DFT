using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Imaging;

namespace DFT
{
    public partial class Form1 : Form
    {
        public static int sampleLength = 44100;
        public static int frameRate = 44100;

        Sound sound = new Sound();

        Bitmap bmp = new Bitmap(sampleLength / 2, 1);

        public Form1()
        {
            InitializeComponent();

            sound.Samples = WAV.Read("d:\\demo2.wav").Select(x => (double)x).ToList();

            doCalc();
        }

        private void doCalc()
        {
            FrequencyThroughTime ftt = new FrequencyThroughTime();
            SoundSample ss = new SoundSample() { Sound = sound, Start = 0, Length = (uint)sampleLength };

            int i = 0;

            while ((ss.Start <= sound.Samples.Count - ss.Length) && i < 5)
            {
                //dft
                FrequencySample s = FrequencySample.FromSoundSample(ss);

                ftt.Samples.Add(s);

                ss.Start += ss.Length;
                i++;
            }

            Func<FrequencyThroughTime, Bitmap[]> bitmaptize = (FrequencyThroughTime _ftt) =>
            {
                Bitmap _bmp = new Bitmap(sampleLength / 2, ftt.Samples.Count, PixelFormat.Format32bppArgb);
                Bitmap _bmp2 = new Bitmap(sampleLength / 2, ftt.Samples.Count, PixelFormat.Format32bppArgb);

                for (int x = 0; x < ftt.Samples.Count; x++)
                {
                    FrequencySample s = _ftt.Samples[x];

                    for (int y = 0; y < s.Samples.Count; y++)
                    {
                        int im = (int)(ushort.MaxValue / 2 + (int)(s.Samples[y].Imaginary));
                        int re = (int)(ushort.MaxValue / 2 + (int)(s.Samples[y].Real));

                        _bmp.SetPixel(y, x, Color.FromArgb(
                             0, (im & 0xFF00) >> 8,
                             (re & 0xFF00) >> 8
                            ));
                         
                        _bmp2.SetPixel(y, x, Color.FromArgb(
                            0, im & 0xFF,
                            re & 0xFF
                            ));
                    }
                }
                return new Bitmap[] { _bmp, _bmp2 };
            };

            var reconstructed = ftt.Samples.SelectMany(x => SimpleDFT.SimpleDFT.IDFT(x.Samples)).Select(x => (short)x).ToList();
            /*
            foreach (var r in sound.Samples.Skip(4200).Take(400))
                chart3.Series[0].Points.Add(r);

            foreach (var r in reconstructed.Skip(4200).Take(400))
                chart3.Series[1].Points.Add(r);
            return;
             */
 
            var bmpB = bitmaptize(ftt);
            
            bmpB[0].Save("d:\\demo1a.bmp", ImageFormat.Bmp);
            bmpB[1].Save("d:\\demo1b.bmp", ImageFormat.Bmp);
            WAV.Write("d:\\sample1.wav", reconstructed, ftt.SamplesPerSecond);

            FrequencyThroughTime ftt2 = new FrequencyThroughTime()
            {
                SamplesPerSecond = 22100,
                Samples = ftt.Samples.Select(fs => new FrequencySample() { Samples = fs.Samples.Select(x => x.Magnitude > 20 ? x : Complex.Zero).ToList(), windowLength = fs.windowLength }).ToList()
            };

            var bmpA = bitmaptize(ftt2);

            bmpA[0].Save("d:\\demo1_filtereda.png", ImageFormat.Png);
            bmpA[1].Save("d:\\demo1_filteredb.png", ImageFormat.Png);

            WAV.Write("d:\\sample1_filteredx1.wav", ftt2.Samples.SelectMany(x => SimpleDFT.SimpleDFT.IDFT(x.Samples)).Select(x => (short)x).ToList(), ftt2.SamplesPerSecond);

            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.Image = bmp;
        }

    }

}
