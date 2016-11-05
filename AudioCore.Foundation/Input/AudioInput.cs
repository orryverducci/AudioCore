using System;

namespace AudioCore.Foundation
{
    public class AudioInput
    {
        #region Private Fields
        /// <summary>
        /// The audio bit depth in bits
        /// </summary>
        private int _bitDepth;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels
        /// </summary>
        /// <value>The number of audio channels</value>
        public int Channels { get; protected set; }

        /// <summary>
        /// Gets the audio sample rate
        /// </summary>
        /// <value>The audio sample rate</value>
        public int SampleRate { get; protected set; }

        /// <summary>
        /// Gets the audio bit depth
        /// </summary>
        /// <value>The audio bit depth in bits</value>
        public int BitDepth
        {
            get
            {
                return _bitDepth;
            }
            protected set
            {
                // Check value is valid
                if (value > 64 || value < 8)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Bit depth must be between 8 bit and 64 bit");
                }
                else if (value % 8 > 0)
                {
                    throw new ArgumentException("Bit depth must a multiple of 8", nameof(value));
                }
                // Set value
                _bitDepth = value;
            }
        }

        /// <summary>
        /// Gets the latency on the audio in milliseconds
        /// </summary>
        /// <value>The audio latency</value>
        public int Latency { get; protected set; }

        /// <summary>
        /// Gets the current playback state
        /// </summary>
        /// <value>Whether the audio input is playing or not</value>
        public PlaybackState PlaybackState { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// Get audio samples from the input
        /// </summary>
        /// <returns>The audio samples</returns>
        /// <param name="samplesRequested">The number of samples required</param>
        public virtual double[] GetSamples(int samplesRequested)
        {
            throw new NotImplementedException("The GetSamples method has not been implemented for this audio input");
        }
        #endregion
    }
}