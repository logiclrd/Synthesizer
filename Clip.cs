using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Synthesizer
{
	public class Clip : INotifyChanged
	{
		public double[] Samples;

		public Clip()
		{
		}

		public Clip(double[] samples)
		{
			Samples = samples;
		}

		public event EventHandler Changed;

		public void WriteSamples(int start, double[] newSamples)
		{
			if (Samples.Length < start + newSamples.Length)
				Array.Resize(ref Samples, start + newSamples.Length);

			Array.Copy(newSamples, 0, Samples, start, newSamples.Length);

			Changed?.Invoke(this, EventArgs.Empty);
		}

		public IWaveProvider GetWaveProvider() => new ClipWaveProvider(this);
	}
}
