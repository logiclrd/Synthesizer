using System;
using System.Collections.Generic;

namespace Synthesizer.Generators.Mix
{
	public class MixGenerator : GeneratorBase
	{
		public List<IGenerator> Inputs = new List<IGenerator>();

		public override void Reset()
		{
			Inputs.ForEach(input => input.Reset());
		}

		public override IGenerator Clone()
		{
			var clone = new MixGenerator();

			foreach (var input in Inputs)
				clone.Inputs.Add(input.Clone());

			return clone;
		}

		public override void Generate(Clip output)
		{
			var buffer = Clip.SameSizeAs(output);

			output.Clear();

			foreach (var input in Inputs)
			{
				input.Generate(buffer);

				for (int sampleIndex = 0; sampleIndex < output.SampleCount; sampleIndex++)
					output.Samples[sampleIndex] += buffer.Samples[sampleIndex];
			}
		}
	}
}
