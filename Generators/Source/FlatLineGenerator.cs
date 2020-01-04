using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators.Source
{
	public class FlatLineGenerator : GeneratorBase
	{
		public float Value;

		public override void Reset()
		{
		}

		public override IGenerator Clone()
		{
			return
				new FlatLineGenerator()
				{
					Value = Value
				};
		}

		public override void Generate(Clip output)
		{
			for (int i = 0; i < output.SampleCount; i++)
				output.Samples[i] = Value;
		}
	}
}
