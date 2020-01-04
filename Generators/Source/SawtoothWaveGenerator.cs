using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators.Source
{
	public class SawtoothWaveGenerator : GeneratorBase
	{
		public float Frequency;

		float _sampleValue = -1.0f;

		public override void Reset()
		{
			_sampleValue = -1.0f;
		}

		public override IGenerator Clone()
		{
			return
				new SawtoothWaveGenerator()
				{
					Frequency = Frequency,

					_sampleValue = _sampleValue,
				};
		}

		public override void Generate(Clip output)
		{
			// This generates a wave like this:
			//
			//    /|  /|  /|
			//   /-|-/-| /-|-/
			//     |/  |/  |/
			//

			float samplesPerCycle = Global.SampleRate / Frequency;

			// Along each cycle, we want to count from -1.0 to +1.0, which is an overall change of 2.0.
			float changePerSample = 2.0f / samplesPerCycle;

			for (int i = 0; i < output.SampleCount; i++)
			{
				output.Samples[i] = _sampleValue;

				// Move along the sawtooth and restart it when we reach the top end.
				_sampleValue += changePerSample;
				if (_sampleValue >= 1.0f)
					_sampleValue -= 2.0f;
			}
		}
	}
}
