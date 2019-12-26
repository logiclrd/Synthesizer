using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Synthesizer
{
	class ClipVisualizer : Control
	{
		Clip _clip;
		int? _lastKnownClipLength;
		double _samplesPerPixelX = 100;
		double _scaleY = 100;
		int? _selectionStartIndex = null;
		int? _selectionEndIndex = null;

		public EventHandler SelectionChanged;
		public EventHandler<int> SampleClicked;

		public new Clip Clip
		{
			get => _clip;
			set
			{
				if (_clip != null)
					_clip.Changed -= _clip_Changed;

				_clip = value;

				if (_clip != null)
					_clip.Changed += _clip_Changed;

				_lastKnownClipLength = _clip?.Samples?.Length;

				InvalidateMeasure();
			}
		}

		public double SamplesPerPixelX
		{
			get => _samplesPerPixelX;
			set
			{
				_samplesPerPixelX = value;
				InvalidateVisual();
			}
		}

		public double ScaleY
		{
			get => _scaleY;
			set
			{
				_scaleY = value;
				InvalidateVisual();
			}
		}

		public int? SelectionStartIndex
		{
			get => _selectionStartIndex;
			set
			{
				_selectionStartIndex = value;
				InvalidateVisual();
			}
		}

		public int? SelectionEndIndex
		{
			get => _selectionEndIndex;
			set
			{
				_selectionEndIndex = value;
				InvalidateVisual();
			}
		}

		private void _clip_Changed(object sender, EventArgs e)
		{
			var clipLength = _clip?.Samples?.Length;

			if (clipLength == _lastKnownClipLength)
				InvalidateVisual();
			else
			{
				InvalidateMeasure();
				_lastKnownClipLength = clipLength;
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (_clip == null)
				return new Size(
					Math.Min(500, constraint.Width),
					constraint.Height);
			else
			{
				double pixelsWide = _clip.Samples.Length / _samplesPerPixelX;

				return new Size(
					Math.Min(pixelsWide, constraint.Width),
					constraint.Height);
			}
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (_clip == null)
			{
				dc.DrawRectangle(
					Brushes.DarkGray,
					null,
					new Rect(0, 0, ActualWidth, ActualHeight));

				return;
			}

			var selectionStartIndex = _selectionStartIndex;
			var selectionEndIndex = _selectionEndIndex;

			bool inSelection = selectionStartIndex.HasValue && selectionEndIndex.HasValue;

			int pixelsWide = (int)Math.Ceiling(_clip.Samples.Length / _samplesPerPixelX);

			dc.DrawRectangle(
				Brushes.Black,
				null,
				new Rect(0, 0, pixelsWide, this.ActualHeight));

			double axisY = this.ActualHeight * 0.5;

			PathFigure waveform = new PathFigure();
			PathFigure selection = null;

			waveform.StartPoint = new Point(0, axisY);

			if (inSelection)
			{
				if (selectionStartIndex > selectionEndIndex)
				{
					var swap = selectionStartIndex;

					selectionStartIndex = selectionEndIndex;
					selectionEndIndex = swap;
				}

				int selectionStartPixel = (int)Math.Floor(selectionStartIndex.Value / _samplesPerPixelX);
				int selectionEndPixel = (int)Math.Ceiling(selectionEndIndex.Value / SamplesPerPixelX);

				if (selectionStartPixel > selectionEndPixel)
				{
					int t = selectionStartPixel;

					selectionStartPixel = selectionEndPixel;
					selectionEndPixel = t;
				}

				dc.DrawRectangle(
					Brushes.Navy,
					null,
					new Rect(selectionStartPixel, 0, selectionEndPixel - selectionStartPixel, this.ActualHeight));

				selection = new PathFigure();
				selection.StartPoint = new Point(selectionStartPixel, axisY);
			}

			int pixelStartIndex = 0;

			int selectionFirstX = -1;

			for (int x = 0; x < pixelsWide; x++)
			{
				int pixelEndIndex = (int)Math.Floor((x + 1) * _samplesPerPixelX);

				if (pixelEndIndex >= _clip.Samples.Length)
					pixelEndIndex = _clip.Samples.Length - 1;

				double highY = axisY;
				double lowY = axisY;

				for (int idx = pixelStartIndex; idx <= pixelEndIndex; idx++)
				{
					double sampleY = axisY - _clip.Samples[idx] * _scaleY;

					if (sampleY < highY)
						highY = sampleY;
					if (sampleY > lowY)
						lowY = sampleY;
				}

				waveform.Segments.Insert(x, new LineSegment(new Point(x, lowY), isStroked: false));
				waveform.Segments.Insert(x, new LineSegment(new Point(x, highY), isStroked: false));

				if (inSelection)
				{
					if ((pixelStartIndex <= selectionEndIndex.Value)
					 && (selectionStartIndex.Value <= pixelEndIndex))
					{
						if (selectionFirstX < 0)
							selectionFirstX = x;

						selection.Segments.Insert(x - selectionFirstX, new LineSegment(new Point(x, lowY), isStroked: false));
						selection.Segments.Insert(x - selectionFirstX, new LineSegment(new Point(x, highY), isStroked: false));
					}
				}

				pixelStartIndex = pixelEndIndex + 1;
			}

			dc.DrawGeometry(
				Brushes.Gray,
				null,
				new PathGeometry() { Figures = new PathFigureCollection() { waveform } });

			if (inSelection)
			{
				dc.DrawGeometry(
					Brushes.Yellow,
					null,
					new PathGeometry() { Figures = new PathFigureCollection() { selection } });
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				var position = e.GetPosition(this);

				_selectionStartIndex = (int)Math.Floor(position.X * _samplesPerPixelX);
				_selectionEndIndex = null;

				InvalidateVisual();
			}
			else
				base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var position = e.GetPosition(this);

				_selectionEndIndex = (int)Math.Floor((position.X + 1) * _samplesPerPixelX);

				InvalidateVisual();
			}
			else
				base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (_selectionStartIndex > _selectionEndIndex)
				{
					var swap = _selectionStartIndex;

					_selectionStartIndex = _selectionEndIndex;
					_selectionEndIndex = swap;
				}

				bool isClick = _selectionEndIndex - _selectionStartIndex < 3 * _samplesPerPixelX;

				if (isClick)
				{
					int clickIndex = _selectionStartIndex.Value;

					_selectionStartIndex = null;
					_selectionEndIndex = null;
					InvalidateVisual();
					SelectionChanged?.Invoke(this, EventArgs.Empty);
					SampleClicked?.Invoke(this, clickIndex);
				}
				else
					SelectionChanged?.Invoke(this, EventArgs.Empty);
			}
			else
				base.OnMouseUp(e);
		}
	}
}
