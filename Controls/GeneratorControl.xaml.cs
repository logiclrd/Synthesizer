using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
		}

		static readonly DependencyPropertyKey InputCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(InputCount), typeof(int), typeof(GeneratorControl), new UIPropertyMetadata(defaultValue: 0));
		static readonly DependencyPropertyKey DynamicInputCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof(DynamicInputCount), typeof(bool), typeof(GeneratorControl), new UIPropertyMetadata(defaultValue: 0));

		public static readonly DependencyProperty GeneratorProperty = DependencyProperty.Register(nameof(Generator), typeof(IGenerator), typeof(GeneratorControl), new UIPropertyMetadata(Generator_Changed));
		public static readonly DependencyProperty InputCountProperty = InputCountPropertyKey.DependencyProperty;
		public static readonly DependencyProperty DynamicInputCountProperty = DynamicInputCountPropertyKey.DependencyProperty;

		public IGenerator Generator
		{
			get => (IGenerator)GetValue(GeneratorProperty);
			set => SetValue(GeneratorProperty, value);
		}

		public int InputCount
		{
			get => (int)GetValue(InputCountProperty);
			private set => SetValue(InputCountPropertyKey, value);
		}

		public bool DynamicInputCount
		{
			get => (bool)GetValue(DynamicInputCountProperty);
			private set => SetValue(DynamicInputCountPropertyKey, value);
		}

		static void Generator_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((d is GeneratorControl control)
			 && (e.NewValue is IGenerator generator))
			{
				int inputCount = 0;
				bool dynamicInputCount = false;

				foreach (var field in generator.GetType().GetFields())
				{
					if (typeof(IGenerator).IsAssignableFrom(field.FieldType))
						inputCount++;
					else if (field.FieldType.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IList<>)) && typeof(IGenerator).IsAssignableFrom(i.GetGenericArguments()[0])))
						dynamicInputCount = true;
				}

				control.InputCount = inputCount + (dynamicInputCount ? 1 : 0);
				control.DynamicInputCount = dynamicInputCount;
			}
		}
	}
}
