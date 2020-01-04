using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators.Mix
{
	public class ConvolutionGenerator : GeneratorBase
	{
		public IGenerator First;
		public IGenerator Second;

		public override void Reset()
		{
			First?.Reset();
			Second?.Reset();
		}

		public override IGenerator Clone()
		{
			return
				new ConvolutionGenerator()
				{
					First = First.Clone(),
					Second = Second.Clone(),
				};
		}

		public override void Generate(Clip output)
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

			var buffer = Clip.SameSizeAs(output);

			First.Generate(output);
			Second.Generate(buffer);

			for (int i = 0; i < output.SampleCount; i++)
				output.Samples[i] = output.Samples[i] * buffer.Samples[i];
		}
	}
}
