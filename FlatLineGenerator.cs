using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class FlatLineGenerator : IGenerator
	{
		public double Value;

		public Clip Generate(int sampleCount)
		{
			var samples = new double[sampleCount];

			for (int i = 0; i < samples.Length; i++)
				samples[i] = Value;

			return new Clip(samples);
		}
	}
}
