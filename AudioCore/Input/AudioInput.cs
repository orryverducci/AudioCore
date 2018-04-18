using System;
using System.Threading.Tasks;

namespace AudioCore
{
    public class AudioInput
    {
        #region Properties
        /// <summary>
        /// Gets the number of audio channels
        /// </summary>
        /// <value>The number of audio channels</value>
        public int Channels { get; protected set; }

        /// <summary>
        /// Gets the audio sample rate in Hertz
        /// </summary>
        /// <value>The audio sample rate in Hertz</value>
        public int SampleRate { get; protected set; }

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
        public virtual async Task<double[]> GetSamples(int samplesRequested)
        {
            throw new NotImplementedException("The GetSamples method has not been implemented for this audio input");
        }
        #endregion
    }
}