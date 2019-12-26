using System.IO;
using System.Text;
using NAudio.Wave;

namespace Synthesizer
{
	class ClipWaveProvider : IWaveProvider
	{
		MemoryStream _rawWaveData;

		public ClipWaveProvider(Clip clip)
		{
			_rawWaveData = new MemoryStream();

			using (var writer = new BinaryWriter(_rawWaveData, Encoding.UTF8, leaveOpen: true))
			{
				for (int i = 0; i < clip.Samples.Length; i++)
					writer.Write((float)clip.Samples[i]);
			}

			_rawWaveData.Position = 0;
		}

		public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(Global.SampleRate, 1);

		public int Read(byte[] buffer, int offset, int count) => _rawWaveData.Read(buffer, offset, count);
	}
}
