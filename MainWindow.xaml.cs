using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;

namespace Synthesizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		IGenerator _output;

		private void cmdGenerate_Click(object sender, RoutedEventArgs e)
		{
			// First, create the notes that we want.
			var note1Wave = new SineWaveGenerator();

			note1Wave.Frequency = Constants.MiddleC;

			var note2Wave = new SineWaveGenerator();

			note2Wave.Frequency = note1Wave.Frequency * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio;

			var note3Wave = new SineWaveGenerator();

			note3Wave.Frequency = note2Wave.Frequency * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio;

			// Next, give these notes a "shape", rather than just being on at max volume all the time.
			var envelope = new EnvelopeGenerator();

			envelope.Attack = EnvelopePoint.AtTime(0.025, 1.0f);
			envelope.Decay = EnvelopePoint.AtTime(0.1, 0.65f);
			envelope.Sustain = EnvelopePoint.AtTime(0.3, 0.45f);
			envelope.ReleaseIndex = EnvelopePoint.AtTime(0.5).Index;

			// Now, apply the envelope to each note.
			var note1Convolution =
				new ConvolutionGenerator()
				{
					First = note1Wave,
					Second = envelope.Clone(),
				};

			var note2Convolution =
				new ConvolutionGenerator()
				{
					First = note2Wave,
					Second = envelope.Clone(),
				};

			var note3Convolution =
				new ConvolutionGenerator()
				{
					First = note3Wave,
					Second = envelope.Clone(),
				};

			// REpeat them in a pattern.
			var note1Repeater =
				new RepeaterGenerator()
				{
					RepeatAfterSamples = Global.SampleRate * 2 / 2,
					Source = note1Convolution,
				};

			var note2Repeater =
				new RepeaterGenerator()
				{
					RepeatAfterSamples = Global.SampleRate * 2 / 3,
					Source = note2Convolution,
				};

			var note3Repeater =
				new RepeaterGenerator()
				{
					RepeatAfterSamples = Global.SampleRate * 2 / 5,
					Source = note3Convolution,
				};

			// Now mix them together.
			var mix = new MixGenerator();

			mix.Inputs.Add(note1Repeater);
			mix.Inputs.Add(note2Repeater);
			mix.Inputs.Add(note3Repeater);

			// Finally, reduce the volume of these notes, because three of them at maximum volume mixed together will be louder than the maximum volume can handle.
			var volume = new FlatLineGenerator();

			volume.Value = 0.3f;

			_output =
				new ConvolutionGenerator()
				{
					First = mix,
					Second = volume,
				};

			var clip = new Clip(Global.SampleRate * 2);

			//_output.Generate(clip);

			var subclip = new Clip(Global.SampleRate / 4);

			for (int i = 0; i < 8; i++)
			{
				_output.Generate(subclip);

				clip.WriteSamples(i * subclip.SampleCount, subclip.Samples);
			}

			cvVisualizer.Clip = clip;

			_output.Reset();
		}

		IWavePlayer _outputter;

		private void cmdPlay_Click(object sender, RoutedEventArgs e)
		{
			if (_outputter != null)
			{
				try
				{
					_outputter.Stop();
				}
				catch { }
			}

			if (_output != null)
			{
				_outputter = new WaveOut();

				_outputter.Init(new GeneratorSampleProvider(_output));
				_outputter.Play();
			}
		}
	}
}
