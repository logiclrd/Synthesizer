using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators
{
	public interface IGenerator
	{
		string Name { get; set; }
		string TypeName { get; }

		void Reset();
		IGenerator Clone();
		void Generate(Clip output);
	}
}
