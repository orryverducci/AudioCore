using System;
using System.Threading.Tasks;
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
        private float[] _buffer;

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
                _buffer = new float[value * Channels * 2];
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

        /// <summary>
        /// Gets or sets if an exception should be thrown when the audio buffer overflows.
        /// </summary>
        public bool ErrorOnOverflow { get; set; } = false;
        #endregion

        #region Events
        /// <summary>
        /// Event signalling that new audio samples are available in the buffer.
        /// </summary>
        public event EventHandler SamplesAvailable;

        /// <summary>
        /// Event signalling that more samples are being written to the buffer than it can hold.
        /// </summary>
        public event EventHandler BufferOverflow;
        #endregion

        #region Buffer Methods
        /// <summary>
        /// Writes audio samples to the buffer.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        protected void Write(Span<float> samples)
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
                    Span<float> bufferSlice = _buffer.AsSpan().Slice(_writePosition, samplesToWrite);
                    samples.Slice(samplesWritten, samplesToWrite).CopyTo(bufferSlice);
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
                // If the buffer overflowed, fire overflow event or throw an exception if enabled
                if (maxSamples < samples.Length)
                {
                    if (ErrorOnOverflow)
                    {
                        throw new InvalidOperationException("The audio buffer has overflowed.");
                    }
                    Task.Run(() => BufferOverflow?.Invoke(this, null)).ConfigureAwait(false);
                }
            }
            // Fire data available event
            Task.Run(() => SamplesAvailable?.Invoke(this, null)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get frames of audio samples from the input.
        /// </summary>
        /// <returns>The audio samples.</returns>
        /// <param name="audioBuffer">The buffer of audio samples to be filled by the input.</param>
        /// <param name="framesRequested">The number of frames required.</param>
        public override void GetFrames(Span<float> audioBuffer, int framesRequested)
        {
            // Copy the samples from the buffer
            lock (_lock)
            {
                // Get the number of samples that can be returned, which is the smaller of either the number requested or the number available
                int samplesToReturn = Math.Min(_sampleCount, audioBuffer.Length);
                // If playing keep copying samples until we've got all that can be returned
                if (PlaybackState == PlaybackState.PLAYING)
                {
                    // If we've run out of samples, stop playback and start buffering
                    if (samplesToReturn == 0)
                    {
                        PlaybackState = PlaybackState.BUFFERING;
                        return;
                    }
                    int samplesRead = 0;
                    while (samplesRead < samplesToReturn)
                    {
                        // Determine the number of samples that can be read before the end of the buffer has been reached
                        int samplesToRead = Math.Min(_buffer.Length - _readPosition, samplesToReturn - samplesRead);
                        // Get a span representing the samples to be copied from the buffer
                        Span<float> bufferSamples = _buffer.AsSpan(_readPosition, samplesToRead);
                        // Get a span representing the section of the output audio buffer the samples are to be copied to
                        Span<float> audioBufferSlice = audioBuffer.Slice(samplesRead, samplesToRead);
                        // Copy samples from the buffer to the output
                        bufferSamples.CopyTo(audioBufferSlice);
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
            // Apply gain to the samples
            for (int i = 0; i < audioBuffer.Length; i++)
            {
                audioBuffer[i] = audioBuffer[i] * Gain;
            }
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