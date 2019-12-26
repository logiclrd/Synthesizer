using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class SawtoothWaveGenerator : IGenerator
	{
		public double Frequency;

		public Clip Generate(int sampleCount)
		{
			var samples = new double[sampleCount];

			// This generates a wave like this:
			//
			//    /|  /|  /|
			//   /-|-/-| /-|-/
			//     |/  |/  |/
			//

			double samplesPerCycle = Global.SampleRate / Frequency;

			// Along each cycle, we want to count from -1.0 to +1.0, which is an overall change of 2.0.
			double changePerSample = 2.0 / samplesPerCycle;

			double sampleValue = -1.0;

			for (int i = 0; i < samples.Length; i++)
			{
				samples[i] = sampleValue;

				// Move along the sawtooth and restart it when we reach the top end.
				sampleValue += changePerSample;
				if (sampleValue >= 1.0)
					sampleValue -= 2.0;
			}

			return new Clip(samples);
		}
	}
}
