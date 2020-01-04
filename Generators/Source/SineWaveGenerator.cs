using System;

namespace Synthesizer.Generators.Source
{
	public class SineWaveGenerator : GeneratorBase
	{
		public double Frequency;

		double _sinePhase = 0.0;

		public override void Reset()
		{
			_sinePhase = 0;
		}

		public override IGenerator Clone()
		{
			return
				new SineWaveGenerator()
				{
					Frequency = Frequency,

					_sinePhase = _sinePhase,
				};
		}

		public override void Generate(Clip output)
		{
			double samplesPerSineWavePeriod = Global.SampleRate / Frequency;

			double phaseChangePerSample = Math.PI * 2 / samplesPerSineWavePeriod;

			for (int i = 0; i < output.SampleCount; i++)
			{
				output.Samples[i] = (float)Math.Sin(_sinePhase);
				_sinePhase += phaseChangePerSample;
			}
		}
	}
}
