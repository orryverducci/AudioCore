using System;
using System.Threading.Tasks;

namespace AudioCore.Input
{
    public class AudioInput
    {
        #region Private Fields
        /// <summary>
        /// The number of audio channels
        /// </summary>
        private int _channels;

        /// <summary>
        /// The audio sample rate in Hertz
        /// </summary>
        private int _sampleRate;

        /// <summary>
        /// The latency on the audio in milliseconds
        /// </summary>
        private int _latency;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels
        /// </summary>
        /// <value>The number of audio channels</value>
        public int Channels
        {
            get
            {
                return _channels;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The number of audio channels must be greater than 0");
                }
                _channels = value;
            }
        }

        /// <summary>
        /// Gets the audio sample rate in Hertz
        /// </summary>
        /// <value>The audio sample rate in Hertz</value>
        public int SampleRate {
            get
            {
                return _sampleRate;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The sample rate must be greater than 0");
                }
                _sampleRate = value;
            }
        }

        /// <summary>
        /// Gets the latency on the audio in milliseconds
        /// </summary>
        /// <value>The audio latency</value>
        public int Latency {
            get
            {
                return _latency;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The latency must be greater than 0");
                }
                _latency = value;
            }
        }

        /// <summary>
        /// Gets the current playback state
        /// </summary>
        /// <value>Whether the audio input is playing or not</value>
        public PlaybackState PlaybackState { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// Get frames of audio samples from the input
        /// </summary>
        /// <returns>The audio samples</returns>
        /// <param name="framesRequested">The number of frames required</param>
        public virtual async Task<double[]> GetFrames(int framesRequested)
        {
            throw new NotImplementedException("The GetFrames method has not been implemented for this audio input");
        }
        #endregion
    }
}