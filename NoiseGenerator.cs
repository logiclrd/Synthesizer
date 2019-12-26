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
		public int Seed = DateTime.UtcNow.Ticks.GetHashCode();

		Random _rnd;

		public void Reset()
		{
			_rnd = null;
		}

		public IGenerator Clone()
		{
			return
				new NoiseGenerator()
				{
					SoftenPasses = SoftenPasses,
					Seed = Seed,

					_rnd = Cloner.Clone(_rnd),
				};
		}

		public void Generate(Clip output)
		{
			if (_rnd == null)
				_rnd = new Random(Seed);

			var samples = output.Samples;

			for (int i = 0; i < samples.Length; i++)
				samples[i] = (float)(_rnd.NextDouble() * 2 - 1);

			var tmp = new float[samples.Length];

			for (int i = 0; i < SoftenPasses; i++)
			{
				tmp[0] = samples[0];
				tmp[samples.Length - 1] = samples[samples.Length - 1];

				for (int j = 1; j < samples.Length - 1; j++)
				{
					tmp[j] = (samples[j - 1] + samples[j] + samples[j + 1]) * 0.333333333333333f;
				}

				var swap = tmp;

				tmp = samples;
				samples = swap;
			}

			output.Samples = samples;
		}
	}
}
