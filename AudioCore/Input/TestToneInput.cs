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
        private double _volumeLinear;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the frequency of the test tone in Hertz.
        /// </summary>
        /// <value>The frequency of the test tone in Hertz.</value>
        public int Frequency
        {
            get
            {
                return _frequency;
            }
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
            get
            {
                return _volumeDBFS;
            }
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
        public ToneType Type { get; set; }
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
        public override double[] GetFrames(int framesRequested)
        {
            // Create array of samples
            double[] audio = new double[framesRequested * Channels];
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
                double sample = 0;
                switch (Type)
                {
                    case ToneType.SineWave:
                        // y = sin(2 * pi * frequency * x)
                        sample = Math.Sin(2 * Math.PI * Frequency * ((double)_frameNumber / (double)SampleRate)) * _volumeLinear;
                        break;
                    case ToneType.SquareWave:
                        // Same as sine wave, but encased in a sign function
                        sample = Math.Sign(Math.Sin(2 * Math.PI * Frequency * ((double)_frameNumber / (double)SampleRate))) * _volumeLinear;
                        break;
                    case ToneType.SawtoothWave:
                        // y = -((2 * amplitude) / pi) * arctan(cot(x * pi / period))
                        sample = ((-4 / Math.PI) * Math.Atan(1d / Math.Tan((_frameNumber * Math.PI) / ((double)SampleRate / (double)Frequency)))) * _volumeLinear;
                        break;
                    case ToneType.TriangleWave:
                        // y = abs(2 * frequency * x % 2 - 1) * amplitude - offset
                        sample = ((Math.Abs((2 * Frequency * ((double)_frameNumber / (double)SampleRate)) % 2 - 1) * 2) - 1) * _volumeLinear;
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