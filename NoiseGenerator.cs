using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class NoiseGenerator : IGenerator
	{
		public int SoftenPasses = 10;

		public Clip Generate(int sampleCount)
		{
			Random rnd = new Random();

			var samples = new double[sampleCount];

			for (int i = 0; i < samples.Length; i++)
				samples[i] = rnd.NextDouble() * 2 - 1;

			var tmp = new double[samples.Length];

			for (int i = 0; i < SoftenPasses; i++)
			{
				tmp[0] = samples[0];
				tmp[samples.Length - 1] = samples[samples.Length - 1];

				for (int j = 1; j < samples.Length - 1; j++)
				{
					tmp[j] = (samples[j - 1] + samples[j] + samples[j + 1]) * 0.333333333333333;
				}

				var swap = tmp;

				tmp = samples;
				samples = swap;
			}

			return new Clip(samples);
		}
	}
}
