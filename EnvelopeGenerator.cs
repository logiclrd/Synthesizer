using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class EnvelopeGenerator : IGenerator
	{
		public EnvelopePoint Attack;
		public EnvelopePoint Decay;
		public EnvelopePoint Sustain;
		public int ReleaseIndex;

		int _sampleIndex;

		public void Reset()
		{
			_sampleIndex = 0;
		}

		public IGenerator Clone()
		{
			return
				new EnvelopeGenerator()
				{
					Attack = Attack,
					Decay = Decay,
					Sustain = Sustain,
					ReleaseIndex = ReleaseIndex,

					_sampleIndex = _sampleIndex,
				};
		}

		public void Generate(Clip output)
		{
			// Try to do something sensible if we're given nonsense to work with. Specifically,
			// the four points of the envelope need to be in order!

			if (Attack.Index < 0)
				Attack.Index = 0;
			if (Decay.Index < Attack.Index)
				Decay.Index = Attack.Index;
			if (Sustain.Index < Decay.Index)
				Sustain.Index = Decay.Index;
			if (ReleaseIndex < Sustain.Index)
				ReleaseIndex = Sustain.Index;

			var release = new EnvelopePoint() { Index = ReleaseIndex, Value = 0 };

			for (int i = 0; i < output.SampleCount; i++)
			{
				int index = _sampleIndex++;

				// Figure out which part of the envelope we are in, giving priority to the parts on the right first.
				if (index > ReleaseIndex)
				{
					output.Samples[i] = 0;
					continue;
				}

				EnvelopePoint from, to;

				if (index > Sustain.Index)
				{
					from = Sustain;
					to = release;
				}
				else if (index > Decay.Index)
				{
					from = Decay;
					to = Sustain;
				}
				else if (index > Attack.Index)
				{
					from = Attack;
					to = Decay;
				}
				else
				{
					from = new EnvelopePoint(); // From the very start, all zeroes.
					to = Attack;
				}

				if (from.Index == to.Index)
					output.Samples[i] = to.Value;
				else
				{
					// This is called "interpolation", and is the math of drawing a line between the two points.
					// Note that fromWeight and toWeight always add up to 1.0, by definition. So, if we multiply
					// a number by fromWeight, and then by toWeight, and we add the two parts together, we get
					// the original number back. If we multiply one number by fromWeight and a different number
					// by toWeight, then we end up with a number between those two numbers, where the higher the
					// weight is, the closer it is to that number.

					float toWeight = (index - from.Index) / (float)(to.Index - from.Index);
					float fromWeight = 1.0f - toWeight;

					output.Samples[i] = from.Value * fromWeight + to.Value * toWeight;
				}
			}
		}
	}
}
