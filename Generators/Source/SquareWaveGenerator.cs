using System;

namespace Synthesizer.Generators.Source
{
	public class SquareWaveGenerator : IGenerator
	{
		public double Frequency;

		double _sampleIndex = 0.0;
		float _phase = 1.0f;

		public void Reset()
		{
			_sampleIndex = 0.0;
			_phase = 1.0f;
		}

		public IGenerator Clone()
		{
			return
				new SquareWaveGenerator()
				{
					Frequency = Frequency,

					_sampleIndex = _sampleIndex,
					_phase = _phase,
				};
		}

		public void Generate(Clip output)
		{
			double samplesPerPhase = Global.SampleRate / (Frequency * 2.0);

			for (int i = 0; i < output.SampleCount; i++)
			{
				output.Samples[i] = _phase;

				_sampleIndex += 1.0;

				if (_sampleIndex > samplesPerPhase)
				{
					_sampleIndex -= samplesPerPhase;
					_phase = -_phase;
				}
			}
		}
	}
}
