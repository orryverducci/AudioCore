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