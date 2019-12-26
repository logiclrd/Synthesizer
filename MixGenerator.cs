using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthesizer
{
	public class MixGenerator : IGenerator
	{
		public List<IGenerator> Inputs = new List<IGenerator>();

		public Clip Generate(int sampleCount)
		{
			var clips = Inputs.Select(input => input.Generate(sampleCount)).ToList();

			// We mix everything into the first clip we got and then just return that clip.
			for (int clipIndex = 1; clipIndex < clips.Count; clipIndex++)
			{
				for (int sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
					clips[0].Samples[sampleIndex] += clips[clipIndex].Samples[sampleIndex];
			}

			return clips[0];
		}
	}
}
