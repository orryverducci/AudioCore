using System;

namespace AudioCore.Input
{
    public abstract class HardwareAudioInput : BufferedAudioInput
    {
        #region Private Fields
        /// <summary>
        /// The number of audio frames the input audio hardware is buffering by.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// The latency of the input in milliseconds.
        /// </summary>
        private int _latency;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio frames the input audio hardware is buffering by.
        /// </summary>
        /// <value>The output buffer size.</value>
        public int HardwareBufferSize
        {
            get => _bufferSize;
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The buffer size must be 0 or greater.");
                }
                _bufferSize = value;
            }
        }

        /// <summary>
        /// Gets the latency of the audio input in milliseconds (excluding software buffering).
        /// </summary>
        /// <value>The latency of the audio input in milliseconds.</value>
        public int Latency
        {
            get => _latency;
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The latency must be 0 or greater.");
                }
                _latency = value;
            }
        }
        #endregion
    }
}