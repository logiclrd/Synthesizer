﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer.Generators
{
	public interface IGenerator
	{
		void Reset();
		IGenerator Clone();
		void Generate(Clip output);
	}
}