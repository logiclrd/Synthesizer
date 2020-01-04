using Synthesizer.Generators;

namespace Synthesizer.Controls
{
	public class InputConnector
	{
		public string Name { get; set; }
		public IGenerator Value { get; set; }
	}
}
