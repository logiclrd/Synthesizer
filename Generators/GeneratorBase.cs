using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators
{
	public abstract class GeneratorBase : IGenerator
	{
		public string Name { get; set; }

		ConcurrentDictionary<Type, string> _typeNames = new ConcurrentDictionary<Type, string>();

		public string TypeName
		{
			get
			{
				return _typeNames.GetOrAdd(
					GetType(),
					(type) =>
					{
						string name = type.Name;

						var builder = new StringBuilder(name);

						if (name.EndsWith("Generator"))
							builder.Length -= "Generator".Length;

						for (int i = 1; i < builder.Length; i++)
							if (char.IsUpper(builder[i]))
								builder.Insert(i++, ' ');

						return builder.ToString();
					});
			}
		}

		public abstract IGenerator Clone();
		public abstract void Generate(Clip output);
		public abstract void Reset();
	}
}
