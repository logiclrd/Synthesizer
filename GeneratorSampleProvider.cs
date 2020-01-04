using System;

using Synthesizer.Generators;

using NAudio.Wave;

namespace Synthesizer
{
	class GeneratorSampleProvider : ISampleProvider
	{
		IGenerator _generator;
		Clip _buffer;
		int _bufferPosition;

		public GeneratorSampleProvider(IGenerator generator)
		{
			_generator = generator;
			_buffer = new Clip(Global.SampleRate / 1);
			_bufferPosition = _buffer.SampleCount;
		}

		public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(Global.SampleRate, 1);

		public int Read(float[] buffer, int offset, int count)
		{
			int copied = 0;

			if (_bufferPosition < _buffer.SampleCount)
			{
				int copyCount = Math.Min(count, _buffer.SampleCount - _bufferPosition);

				Buffer.BlockCopy(_buffer.Samples, _bufferPosition * 4, buffer, offset * 4, copyCount * 4);

				copied += copyCount;
				offset += copyCount;
				count -= copyCount;
				_bufferPosition += copyCount;
			}

			if (_bufferPosition >= _buffer.SampleCount)
			{
				_generator.Generate(_buffer);
				_bufferPosition = 0;

				if (count > 0)
					copied += Read(buffer, offset, count);
			}

			return copied;
		}
	}
}
