using System;

namespace Synthesizer
{
	public interface INotifyChanged
	{
		event EventHandler Changed;
	}
}
