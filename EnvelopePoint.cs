using System;

namespace Synthesizer
{
	public struct EnvelopePoint
	{
		public int Index;
		public float Value;

		public EnvelopePoint(int index, float value)
		{
			Index = index;
			Value = value;
		}

		public static EnvelopePoint AtTime(double time, float value = 0.0f)
			=> new EnvelopePoint(
				(int)Math.Round(time * Global.SampleRate),
				value);
	}
}
