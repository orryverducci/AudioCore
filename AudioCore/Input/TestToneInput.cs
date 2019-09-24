using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides an audio input generated from a continuous sine wave test tone.
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

        #region Private Fields
        /// <summary>
        /// The frequency of the test tone in Hertz.
        /// </summary>
        private int _frequency = 1000;

        /// <summary>
        /// The number of the current frame in the sine wave.
        /// </summary>
        private int _frameNumber;

        /// <summary>
        /// The set audio volume in dBFS.
        /// </summary>
        private int _volumeDBFS;

        /// <summary>
        /// The set audio volume as a linear value.
        /// </summary>
        private double _volumeLinear = 1;
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
        /// Gets or sets the volume of the test tone in dBFS.
        /// </summary>
        /// <value>The volume of the test tone in dBFS.</value>
        public int Volume
        {
            get => _volumeDBFS;
            set
            {
                _volumeDBFS = value;
                // Calculate linear volume (between 0 and 1) from dBFS value
                _volumeLinear = Math.Pow(10, value / 20d);
            }
        }

        /// <summary>
        /// Gets or sets the type of test tone to be generated.
        /// </summary>
        /// <value>The type.</value>
        public ToneType Type { get; set; } = ToneType.SineWave;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
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
        /// <param name="framesRequested">The number of frames required.</param>
        public override float[] GetFrames(int framesRequested)
        {
            // Create array of samples
            float[] audio = new float[framesRequested * Channels];
            // Generate audio for frames requested
            for (int i = 0; i < (framesRequested); i++)
            {
                // Increase the sine wave sample number by 1
                _frameNumber++;
                // If the sample reaches the sample rate, reset sample number
                if (_frameNumber > SampleRate)
                {
                    _frameNumber = 1;
                }
                // Generate sample for the chosen type of wave
                float sample = 0;
                switch (Type)
                {
                    case ToneType.SineWave:
                        // y = sin(2 * pi * frequency * x)
                        sample = (float)(Math.Sin(2 * Math.PI * Frequency * ((float)_frameNumber / (float)SampleRate)) * _volumeLinear);
                        break;
                    case ToneType.SquareWave:
                        // Same as sine wave, but encased in a sign function
                        sample = (float)(Math.Sign(Math.Sin(2 * Math.PI * Frequency * ((float)_frameNumber / (float)SampleRate))) * _volumeLinear);
                        break;
                    case ToneType.SawtoothWave:
                        // y = -((2 * amplitude) / pi) * arctan(cot(x * pi / period))
                        sample = (float)(((-2 / Math.PI) * Math.Atan(1d / Math.Tan((_frameNumber * Math.PI) / ((float)SampleRate / (float)Frequency)))) * _volumeLinear);
                        break;
                    case ToneType.TriangleWave:
                        // y = abs(2 * frequency * x % 2 - 1) * amplitude - offset
                        sample = (float)(((Math.Abs((2 * Frequency * ((float)_frameNumber / (float)SampleRate)) % 2 - 1) * 2) - 1) * _volumeLinear);
                        break;
                }
                // Copy sample to each channel
                int firstChannelSample = i * Channels;
                for (int x = 0; x < Channels; x++)
                {
                    audio[firstChannelSample + x] = sample;
                }
            }
            // Return samples
            return audio;
        }
        #endregion
    }
}