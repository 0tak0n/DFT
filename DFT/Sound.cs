using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace DFT
{
    public class Sound
    {
        public uint SamplesPerSecond = 44000;
        public IList<double> Samples;

        public static implicit operator SoundSample(Sound s)
        {
            return new SoundSample() { Sound = s, Start = 0, Length = (uint)s.Samples.Count };
        }
    }

    public struct SoundSample
    {
        public Sound Sound;
        public uint Start;
        public uint Length;
    }

    public class FrequencySample
    {
        public uint windowLength;

        public List<Complex> Samples;

        public static FrequencySample FromSoundSample(SoundSample sound)
        {
            List<Complex> d = SimpleDFT.SimpleDFT.DFT(sound.Sound.Samples, sound.Start, sound.Length);

            //normalize
            double normalizer = 1d / d.Count;

            for (int i = 0; i < d.Count; i++)
            {
                d[i] = d[i] * normalizer;
            }

            return new FrequencySample() { Samples = d };
        }
    }

    public class FrequencyThroughTime
    {
        public uint SamplesPerSecond = 44100;
        public List<FrequencySample> Samples = new List<FrequencySample>();
    }
}
