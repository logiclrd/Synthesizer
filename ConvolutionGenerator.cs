using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class ConvolutionGenerator : IGenerator
	{
		public IGenerator First;
		public IGenerator Second;

		public Clip Generate(int sampleCount)
		{
			// A "convolution" is what you get when you have two sets of numbers that are of the same size,
			// and you go through them pairing up the numbers, multiplying each pair together, to make a
			// new set of the same size.
			//
			// For instance,
			//     (1, 2, 3) * (4, 5, 6) = (4, 10, 18)
			//
			// Because 1 * 4 = 4,      <-- first number from each group
			//         2 * 5 = 10,     <-- second number from each group
			//         3 * 6 = 18      <-- third number from each group

			var firstClip = First.Generate(sampleCount);
			var secondClip = Second.Generate(sampleCount);

			// We could make a place to put the samples, do the calculations, and then throw away firstClip
			// and secondClip (nobody else would ever see them), but we can help the program run faster by
			// reusing one of the clips as the output of the multiplications. So, as we go through the clips,
			// we actually edit firstClip to have the result, and then firstClip becomes the overall result.

			for (int i = 0; i < firstClip.Samples.Length; i++)
				firstClip.Samples[i] = firstClip.Samples[i] * secondClip.Samples[i];

			return firstClip;
		}
	}
}
