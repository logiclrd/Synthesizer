using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
	public class FlatLineGenerator : IGenerator
	{
		public float Value;

		public void Reset()
		{
		}

		public IGenerator Clone()
		{
			return
				new FlatLineGenerator()
				{
					Value = Value
				};
		}

		public void Generate(Clip output)
		{
			for (int i = 0; i < output.SampleCount; i++)
				output.Samples[i] = Value;
		}
	}
}
