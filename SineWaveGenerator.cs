using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class SineWaveGenerator : IGenerator
	{
		public double Frequency;

		public Clip Generate(int sampleCount)
		{
			var samples = new double[sampleCount];

			double samplesPerSineWavePeriod = Global.SampleRate / Frequency;

			double indexMultiplier = Math.PI * 2 / samplesPerSineWavePeriod;

			for (int i = 0; i < samples.Length; i++)
				samples[i] = Math.Cos(i * indexMultiplier);

			return new Clip(samples);
		}
	}
}
