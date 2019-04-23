using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides an audio input with buffering.
    /// </summary>
    public abstract class BufferedAudioInput : AudioInput
    {
        #region Private Fields
        /// <summary>
        /// The buffer.
        /// </summary>
        private double[] _buffer;

        /// <summary>
        /// An object to be used in locks to ensure that buffer operations are carried out on a single thread at a time.
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// The size of the buffer in samples.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// The buffer write position.
        /// </summary>
        private int _writePosition;

        /// <summary>
        /// The buffer read position.
        /// </summary>
        private int _readPosition;

        /// <summary>
        /// The number of samples currently in the buffer.
        /// </summary>
        private int _sampleCount;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the buffer, in number of frames.
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int BufferSize
        {
            get => _bufferSize / Channels / 2;
            set
            {
                ResetBuffer();
                _buffer = new double[value * Channels * 2];
                _bufferSize = value * Channels * 2;
            }
        }

        /// <summary>
        /// Gets the number of frames currently in the buffer.
        /// </summary>
        /// <value>The number of frames in the buffer.</value>
        public int FrameCount
        {
            get => _sampleCount / Channels;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event signalling that new audio samples are available in the buffer.
        /// </summary>
        public event EventHandler SamplesAvailable;
        #endregion

        #region Playback Methods
        /// <summary>
        /// Start the audio input.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop the audio input.
        /// </summary>
        public abstract void Stop();
        #endregion

        #region Buffer Methods
        /// <summary>
        /// Writes audio samples to the buffer.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        protected void Write(double[] samples)
        {
            // Check the buffer has been initialised
            if (_buffer == null)
            {
                throw new InvalidOperationException("The buffer has not yet been initialised. Set a buffer size to initialise it.");
            }
            // Write the samples
            lock (_lock)
            {
                // Determine the maximum number of samples the buffer can be filled with, either the number of sample provided or the space left in the buffer
                int maxSamples = Math.Min(samples.Length, _buffer.Length - _sampleCount);
                // Keep writing samples to the buffer until the maximum number possible has been reached
                int samplesWritten = 0;
                while (samplesWritten < maxSamples)
                {
                    // Determine the number of samples that can be written before the end of the buffer has been reached
                    int samplesToWrite = Math.Min(_buffer.Length - _writePosition, samples.Length - samplesWritten);
                    // Write to the buffer the samples that can be written
                    Array.Copy(samples, samplesWritten, _buffer, _writePosition, samplesToWrite);
                    samplesWritten += samplesToWrite;
                    _writePosition += samplesToWrite;
                    // If the end of the buffer has been reached, wrap the write position round to the start
                    if (_writePosition >= _buffer.Length)
                    {
                        _writePosition = 0;
                    }
                }
                _sampleCount += samplesWritten;
                // If buffering and we have more samples than the buffer size, start playback
                if (PlaybackState == PlaybackState.BUFFERING && (_sampleCount >= BufferSize))
                {
                    PlaybackState = PlaybackState.PLAYING;
                }
                // If the buffer overflowed, throw an exception
                if (maxSamples < samples.Length)
                {
                    throw new InvalidOperationException("The audio buffer has overflowed.");
                }
            }
            // Fire data available event
            SamplesAvailable?.Invoke(this, null);
        }

        /// <summary>
        /// Get frames of audio samples from the input.
        /// </summary>
        /// <returns>The audio samples.</returns>
        /// <param name="framesRequested">The number of frames required.</param>
        public override double[] GetFrames(int framesRequested)
        {
            // Create an array of samples to be returned
            double[] samples = new double[framesRequested * Channels];
            // Copy the samples from the buffer
            lock (_lock)
            {
                // Get the number of samples that can be returned, which is the smaller of either the number requested or the number available
                int samplesToReturn = Math.Min(_sampleCount, samples.Length);
                // If playing and we've run out of samples, stop playback and start buffering
                if (PlaybackState == PlaybackState.PLAYING && samplesToReturn == 0)
                {
                    PlaybackState = PlaybackState.BUFFERING;
                }
                // If playing keep copying samples until we've got all that can be returned
                if (PlaybackState == PlaybackState.PLAYING)
                {
                    int samplesRead = 0;
                    while (samplesRead < samplesToReturn)
                    {
                        // Determine the number of samples that can be read before the end of the buffer has been reached
                        int samplesToRead = Math.Min(_buffer.Length - _readPosition, samplesToReturn - samplesRead);
                        // Copy samples from the buffer to the samples to be returned
                        Array.Copy(_buffer, _readPosition, samples, samplesRead, samplesToRead);
                        samplesRead += samplesToRead;
                        _readPosition += samplesToRead;
                        // If the end of the buffer has been reached, wrap the read position round to the start
                        if (_readPosition >= _buffer.Length)
                        {
                            _readPosition = 0;
                        }
                    }
                    _sampleCount -= samplesRead;
                }
            }
            // Return the samples
            return samples;
        }

        /// <summary>
        /// Resets the buffer, clearing all buffered samples.
        /// </summary>
        protected void ResetBuffer()
        {
            lock (_lock)
            {
                _writePosition = 0;
                _readPosition = 0;
                _sampleCount = 0;
            }
        }
        #endregion
    }
}