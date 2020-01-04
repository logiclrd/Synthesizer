using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Synthesizer.Generators;

namespace Synthesizer.Controls
{
	/// <summary>
	/// Interaction logic for GeneratorControl.xaml
	/// </summary>
	public partial class GeneratorControl : UserControl
	{
		public GeneratorControl()
		{
			InitializeComponent();

			this.DataContext = this;
			this.InputConnectors = new ObservableCollection<InputConnector>();
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var result = base.MeasureOverride(constraint);

			cdBorderOverlayInputColumn.Width = new GridLength(cdRealInputColumn.ActualWidth);

			return result;
		}

		static readonly DependencyPropertyKey GeneratorNamePropertyKey = DependencyProperty.RegisterReadOnly(nameof(GeneratorName), typeof(string), typeof(GeneratorControl), new UIPropertyMetadata());
		static readonly DependencyPropertyKey GeneratorTypePropertyKey = DependencyProperty.RegisterReadOnly(nameof(GeneratorType), typeof(string), typeof(GeneratorControl), new UIPropertyMetadata());
		static readonly DependencyPropertyKey InputConnectorsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(InputConnectors), typeof(ObservableCollection<InputConnector>), typeof(GeneratorControl), new UIPropertyMetadata(defaultValue: null));
		static readonly DependencyPropertyKey IsDynamicInputCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDynamicInputCount), typeof(bool), typeof(GeneratorControl), new UIPropertyMetadata(defaultValue: false));

		public static readonly DependencyProperty GeneratorProperty = DependencyProperty.Register(nameof(Generator), typeof(IGenerator), typeof(GeneratorControl), new UIPropertyMetadata(Generator_Changed));
		public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(nameof(Colour), typeof(Brush), typeof(GeneratorControl), new UIPropertyMetadata(Brushes.LightBlue));
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(GeneratorControl));

		public static readonly DependencyProperty GeneratorNameProperty = GeneratorNamePropertyKey.DependencyProperty;
		public static readonly DependencyProperty GeneratorTypeProperty = GeneratorTypePropertyKey.DependencyProperty;
		public static readonly DependencyProperty InputConnectorsProperty = InputConnectorsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty IsDynamicInputCountProperty = IsDynamicInputCountPropertyKey.DependencyProperty;

		public IGenerator Generator
		{
			get => (IGenerator)GetValue(GeneratorProperty);
			set => SetValue(GeneratorProperty, value);
		}

		public Brush Colour
		{
			get => (Brush)GetValue(ColourProperty);
			set => SetValue(ColourProperty, value);
		}

		public bool IsSelected
		{
			get => (bool)GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		public string GeneratorName
		{
			get => (string)GetValue(GeneratorNameProperty);
			set => SetValue(GeneratorNamePropertyKey, value);
		}

		public string GeneratorType
		{
			get => (string)GetValue(GeneratorTypeProperty);
			set => SetValue(GeneratorTypePropertyKey, value);
		}

		public ObservableCollection<InputConnector> InputConnectors
		{
			get => (ObservableCollection<InputConnector>)GetValue(InputConnectorsProperty);
			private set => SetValue(InputConnectorsPropertyKey, value);
		}

		public bool IsDynamicInputCount
		{
			get => (bool)GetValue(IsDynamicInputCountProperty);
			private set => SetValue(IsDynamicInputCountPropertyKey, value);
		}

		static void Generator_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((d is GeneratorControl control)
			 && (e.NewValue is IGenerator generator))
				control.RecalculateInputs(generator);
		}

		void RecalculateInputs(IGenerator generator = null)
		{
			if (generator == null)
				generator = this.Generator;

			GeneratorName = generator.Name;
			GeneratorType = generator.TypeName;

			InputConnectors.Clear();
			IsDynamicInputCount = false;

			string dynamicInputFieldName = null;
			IList dynamicInputSource = null;

			foreach (var field in generator.GetType().GetFields())
			{
				if (typeof(IGenerator).IsAssignableFrom(field.FieldType))
				{
					InputConnectors.Add(
						new InputConnector()
						{
							Name = field.Name,
							Value = (IGenerator)field.GetValue(generator)
						});
				}
				else if (field.FieldType.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IList<>)) && typeof(IGenerator).IsAssignableFrom(i.GetGenericArguments()[0])))
				{
					dynamicInputFieldName = field.Name;
					dynamicInputSource = (IList)field.GetValue(generator);
				}
			}

			if (dynamicInputSource != null)
			{
				for (int i = 0; i < dynamicInputSource.Count; i++)
				{
					InputConnectors.Add(
						new InputConnector()
						{
							Name = dynamicInputFieldName + "[" + i + "]",
							Value = dynamicInputSource[i] as IGenerator,
						});
				}

				IsDynamicInputCount = true;
			}
		}
	}
}
