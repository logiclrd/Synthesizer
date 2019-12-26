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
		public float[] Samples;

		public int SampleCount => Samples.Length;

		public Clip()
		{
		}

		public Clip(int numSamples)
		{
			Samples = new float[numSamples];
		}

		public Clip(float[] samples)
		{
			Samples = samples;
		}

		public static Clip SameSizeAs(Clip output)
			=> new Clip(output.SampleCount);

		public event EventHandler Changed;

		public void WriteSamples(int start, float[] newSamples)
		{
			if (Samples.Length < start + newSamples.Length)
				Array.Resize(ref Samples, start + newSamples.Length);

			Array.Copy(newSamples, 0, Samples, start, newSamples.Length);

			Changed?.Invoke(this, EventArgs.Empty);
		}

		public void Clear()
		{
			Array.Clear(Samples, 0, Samples.Length);
		}
    }
}
