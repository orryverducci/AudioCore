using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides an audio input generated from a continuous sine wave test tone.
    /// </summary>
    public class TestToneInput : AudioInput
    {
        #region Private Fields
        /// <summary>
        /// The frequency of the test tone in Hertz.
        /// </summary>
        private int _frequency;

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
        /// Gets the frequency of the test tone in Hertz.
        /// </summary>
        /// <value>The frequency of the test tone in Hertz.</value>
        public int Frequency
        {
            get
            {
                return _frequency;
            }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The frequency must be greater than 0.");
                }
                _frequency = value;
            }
        }

        /// <summary>
        /// Gets the volume of the test tone in dBFS.
        /// </summary>
        /// <value>The volume of the test tone in dBFS.</value>
        public int Volume
        {
            get
            {
                return _volumeDBFS;
            }
            private set
            {
                _volumeDBFS = value;
                // Calculate linear volume (between 0 and 1) from dBFS value
                _volumeLinear = Math.Pow(10, Volume / 20);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
        /// Generates a continuous 1kHz sine wave test tone at full volume.
        /// </summary>
        /// <param name="channels">The number of audio channels required.</param>
        /// <param name="sampleRate">The audio sample rate required in Hertz.</param>
        public TestToneInput(int channels, int sampleRate) : this(channels, sampleRate, 1000, 0) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
        /// Generates a continuous 1kHz sine wave test tone at the requested volume.
        /// </summary>
        /// <param name="channels">The number of audio channels required.</param>
        /// <param name="sampleRate">The audio sample rate required in Hertz.</param>
        /// <param name="volume">The audio output volume in dBFS.</param>
        public TestToneInput(int channels, int sampleRate, int volume) : this(channels, sampleRate, 1000, volume) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
        /// Generates a continuous sine wave test tone at the requested frequency and volume.
        /// </summary>
        /// <param name="channels">The number of audio channels required.</param>
        /// <param name="sampleRate">The audio sample rate required in Hertz.</param>
        /// <param name="frequency">The frequency of the tone required in Hertz.</param>
        /// <param name="volume">The audio output volume in dBFS.</param>
        public TestToneInput(int channels, int sampleRate, int frequency, int volume)
        {
            // Set properties
            Channels = channels;
            SampleRate = sampleRate;
            Frequency = frequency;
            Volume = volume;
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
                // Generate sine wave value
                double sineValue = Math.Sin(2 * Math.PI * Frequency * ((double)_frameNumber / (double)SampleRate)) * _volumeLinear;
                // Copy sine wave value to samples for each channel
                int firstChannelSample = i * Channels;
                for (int x = 0; x < Channels; x++)
                {
                    audio[firstChannelSample + x] = sineValue;
                }
            }
            // Return samples
            return audio;
        }
        #endregion
    }
}