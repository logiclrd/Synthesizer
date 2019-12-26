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

		Clip _clip;

		private void cmdGenerate_Click(object sender, RoutedEventArgs e)
		{
			// First, create the notes that we want.
			var note1TriangleWave = new TriangleWaveGenerator();

			note1TriangleWave.Frequency = Constants.ABelowMiddleC;

			var note2TriangleWave = new TriangleWaveGenerator();

			note2TriangleWave.Frequency = note1TriangleWave.Frequency * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio;

			var note3TriangleWave = new TriangleWaveGenerator();

			note3TriangleWave.Frequency = note2TriangleWave.Frequency * Constants.SemitoneRatio * Constants.SemitoneRatio * Constants.SemitoneRatio;

			// Next, give these notes a "shape", rather than just being on at max volume all the time.
			var envelope = new EnvelopeGenerator();

			envelope.Attack = EnvelopePoint.AtTime(0.025, 1.0);
			envelope.Decay = EnvelopePoint.AtTime(0.2, 0.65);
			envelope.Sustain = EnvelopePoint.AtTime(1.0, 0.45);
			envelope.ReleaseIndex = EnvelopePoint.AtTime(1.5).Index;

			// Now, apply the envelope to each note.
			var note1Convolution =
				new ConvolutionGenerator()
				{
					First = note1TriangleWave,
					Second = envelope,
				};

			var note2Convolution =
				new ConvolutionGenerator()
				{
					First = note2TriangleWave,
					Second = envelope,
				};

			var note3Convolution =
				new ConvolutionGenerator()
				{
					First = note3TriangleWave,
					Second = envelope,
				};

			// Now mix them together.
			var mix = new MixGenerator();

			mix.Inputs.Add(note1Convolution);
			mix.Inputs.Add(note2Convolution);
			mix.Inputs.Add(note3Convolution);

			// Finally, reduce the volume of these notes, because three of them at maximum volume mixed together will be louder than the maximum volume can handle.
			var volume = new FlatLineGenerator();

			volume.Value = 0.3;

			var output = new ConvolutionGenerator();

			output.First = mix;
			output.Second = volume;

			_clip = output.Generate(Global.SampleRate * 2);

			cvVisualizer.Clip = _clip;
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

			if (_clip != null)
			{
				_outputter = new WaveOut();

				_outputter.Init(_clip.GetWaveProvider());
				_outputter.Play();
			}
		}
	}
}
