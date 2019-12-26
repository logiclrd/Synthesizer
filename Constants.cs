using System;

namespace Synthesizer
{
	class Constants
	{
		public const int ABelowMiddleC = 440;

		// This here is the number that when you multiply it by itself 12 times, the result is 2. This is equivalent
		// to saying that if you go up 12 semitones, you've doubled your frequency, which itself means that you've
		// gone up one octave.
		//
		// This number is calculated by taking the twelfth root of 2.0. The "square root" is the number that when you
		// multiply it by itself, you get 2. The "cube root" is the number that when you multiply it by itself 3 times,
		// you get 3.
		public const double SemitoneRatio = 1.05946309436; // Math.Pow(2.0, 1.0 / 12.0);
	}
}
