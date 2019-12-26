using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class TriangleWaveGenerator : IGenerator
	{
		public double Frequency;

		public Clip Generate(int sampleCount)
		{
			var samples = new double[sampleCount];
			
			// This generates a wave like this:
			//
			//    /\    /\    /\    /\
			//   /--\--/--\--/--\--/--\--/
			//       \/    \/    \/    \/
			//
			// In order to generate this, we generate a sawtooth that goes twice as high, and then we "reflect" the
			// upper half of the sawtooth like it was a mirror:
			//
			//        /     /
			//       /|    /|
			//      / |   / |
			// ____/__|__/__|
			//    /   | /   |
			//   /    |/    |
			//
			//      ^^^   ^^^   These parts get turned upside down so that the ramp back down smoothly to the bottom end.
			//
			// For this to work, the sawtooth goes from the range -1.0 through to +3.0, and then when it's between +1.0
			// and +3.0, it gets turned upside down so that instead of proceeding up from +1.0 to +3.0, it proceeds down
			// from +1.0 to -1.0 again.
			//
			// In the loop, then you can see it increasing the sample value regularly until it gets to 3.0, at which point
			// 4.0 is subtracted so that it starts over again at -1.0. When it's writing the sample value into the output,
			// if it's above 1.0, then it gets reflected.

			double samplesPerTriangleCycle = Global.SampleRate / Frequency;

			// Along each cycle, we want to count from -1.0 to +3.0, which is an overall change of 4.0.
			double changePerSample = 4.0 / samplesPerTriangleCycle;

			double sampleValue = -1.0;

			for (int i = 0; i < samples.Length; i++)
			{
				// Reflect the top half of the sawtooth.
				if (sampleValue < 1.0)
					samples[i] = sampleValue;
				else
					samples[i] = 2.0 - sampleValue;

				// Move along the sawtooth and restart it when we reach the top end.
				sampleValue += changePerSample;
				if (sampleValue >= 3.0)
					sampleValue -= 4.0;
			}

			return new Clip(samples);
		}
	}
}
