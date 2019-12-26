using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    public class RepeaterGenerator : IGenerator
    {
        public int RepeatAfterSamples;
        public int FadeOutBeforeRepeatSampleCount = 125;
        public IGenerator Source;

        int _sampleIndex;

        public void Reset()
        {
            _sampleIndex = 0;
        }

        public IGenerator Clone()
        {
            return
                new RepeaterGenerator()
                {
                    RepeatAfterSamples = RepeatAfterSamples,
                    Source = Source,

                    _sampleIndex = _sampleIndex,
                };
        }

        public void Generate(Clip output)
        {
            int samplesUntilRepeat = RepeatAfterSamples - _sampleIndex;

            if (samplesUntilRepeat > output.SampleCount)
            {
                Source.Generate(output);
                _sampleIndex += output.SampleCount;

                int remainingCount = samplesUntilRepeat - output.SampleCount;

                if (remainingCount < FadeOutBeforeRepeatSampleCount)
                {
                    int fadeCount = FadeOutBeforeRepeatSampleCount - remainingCount;

                    for (int i = 0; i < FadeOutBeforeRepeatSampleCount; i++)
                    {
                        int index = output.SampleCount - fadeCount + i;

                        if ((index >= 0) && (i < output.SampleCount))
                        {
                            float volume = 1.0f - (i + 1) / (float)FadeOutBeforeRepeatSampleCount;

                            output.Samples[index] *= volume;
                        }
                    }
                }
            }
            else
            {
                var remaining = new Clip(samplesUntilRepeat);
                var after = new Clip(output.SampleCount - samplesUntilRepeat);

                Source.Generate(remaining);
                Source.Reset();

                _sampleIndex = 0;

                this.Generate(after);

                for (int i = 0; i < FadeOutBeforeRepeatSampleCount; i++)
                {
                    int index = remaining.SampleCount - FadeOutBeforeRepeatSampleCount + i;

                    if (index >= 0)
                    {
                        float volume = 1.0f - (i + 1) / (float)FadeOutBeforeRepeatSampleCount;

                        remaining.Samples[index] *= volume;
                    }
                }

                output.WriteSamples(0, remaining.Samples);
                output.WriteSamples(remaining.SampleCount, after.Samples);
            }
        }
    }
}
