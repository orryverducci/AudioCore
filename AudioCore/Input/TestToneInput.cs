using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides an audio input generated from a continuous test tone.
    /// </summary>
    public class TestToneInput : AudioInput
    {
        #region Enumerations
        /// <summary>
        /// Types of test tone.
        /// </summary>
        public enum ToneType
        {
            SineWave,
            SquareWave,
            SawtoothWave,
            TriangleWave
        }
        #endregion

        #region Constants
        /// <summary>
        /// Math constant representing 2 * pi.
        /// </summary>
        private const float Tau = 2 * MathF.PI;
        #endregion

        #region Private Fields
        /// <summary>
        /// The frequency of the test tone in Hertz.
        /// </summary>
        private int _frequency = 1000;

        /// <summary>
        /// The number of the current frame in the sine wave.
        /// </summary>
        private int _frameNumber;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the frequency of the test tone in Hertz.
        /// </summary>
        /// <value>The frequency of the test tone in Hertz.</value>
        public int Frequency
        {
            get => _frequency;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The frequency must be greater than 0.");
                }
                _frequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of test tone to be generated.
        /// </summary>
        /// <value>The type.</value>
        public ToneType Type { get; set; } = ToneType.SineWave;

        /// <summary>
        /// Gets or sets if the phase should be reversed every other channel.
        /// </summary>
        public bool ReversePhase { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Input.TestToneInput"/> class.
        /// </summary>
        /// <param name="channels">The number of audio channels required.</param>
        /// <param name="sampleRate">The audio sample rate required in Hertz.</param>
        public TestToneInput(int channels, int sampleRate)
        {
            // Set properties
            Channels = channels;
            SampleRate = sampleRate;
            // Set playback state as playing
            PlaybackState = PlaybackState.PLAYING;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get test tone audio frames.
        /// </summary>
        /// <returns>The test tone audio frames.</returns>
        /// <param name="audioBuffer">The buffer of audio samples to be filled by the input.</param>
        /// <param name="framesRequested">The number of frames required.</param>
        public override void GetFrames(Span<float> audioBuffer, int framesRequested)
        {
            // Initialise variable to store generated sample
            float sample = 0;
            // Generate audio for frames requested
            for (int i = 0; i < (framesRequested); i++)
            {
                // Increase the sine wave frame number by 1
                _frameNumber++;
                // If the sample reaches the sample rate, reset sample number
                if (_frameNumber > SampleRate)
                {
                    _frameNumber = 1;
                }
                // Generate sample for the chosen type of wave
                switch (Type)
                {
                    case ToneType.SineWave:
                        // y = sin(2 * pi * frequency * x)
                        sample = MathF.Sin(Tau * Frequency * ((float)_frameNumber / (float)SampleRate)) * Gain;
                        break;
                    case ToneType.SquareWave:
                        // Same as sine wave, but encased in a sign function
                        sample = MathF.Sign(MathF.Sin(Tau * Frequency * ((float)_frameNumber / (float)SampleRate))) * Gain;
                        break;
                    case ToneType.SawtoothWave:
                        // y = -((2 * amplitude) / pi) * arctan(cot(x * pi / period))
                        sample = ((-Tau) * MathF.Atan(1f / MathF.Tan((_frameNumber * MathF.PI) / ((float)SampleRate / (float)Frequency)))) * Gain;
                        break;
                    case ToneType.TriangleWave:
                        // y = abs(2 * frequency * x % 2 - 1) * amplitude - offset
                        sample = ((MathF.Abs((2 * Frequency * ((float)_frameNumber / (float)SampleRate)) % 2 - 1) * 2) - 1) * Gain;
                        break;
                }
                // Copy sample to each channel
                int firstChannelSample = i * Channels;
                for (int x = 0; x < Channels; x++)
                {
                    if (ReversePhase && x % 2 != 0)
                    {
                        audioBuffer[firstChannelSample + x] = -sample;
                    }
                    else
                    {
                        audioBuffer[firstChannelSample + x] = sample;
                    }
                }
            }
        }
        #endregion
    }
}