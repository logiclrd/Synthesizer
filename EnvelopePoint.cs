using System;

namespace Synthesizer
{
	public struct EnvelopePoint
	{
		public int Index;
		public double Value;

		public EnvelopePoint(int index, double value)
		{
			Index = index;
			Value = value;
		}

		public static EnvelopePoint AtTime(double time, double value = 0.0)
			=> new EnvelopePoint(
				(int)Math.Round(time * Global.SampleRate),
				value);
	}
}
