using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Input;

namespace AudioCore.Output
{
    /// <summary>
    /// Provides an audio output.
    /// </summary>
    public class AudioOutput
    {
        #region Private Fields
        /// <summary>
        /// The number of audio channels.
        /// </summary>
        private int _channels;

        /// <summary>
        /// The audio sample rate in Hertz.
        /// </summary>
        private int _sampleRate;

        /// <summary>
        /// The number of audio frames the output is buffering by.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// The audio output bit depth.
        /// </summary>
        private int _bitDepth;

        /// <summary>
        /// The audio inputs.
        /// </summary>
        private List<AudioInput> _audioInputs = new List<AudioInput>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        /// <value>The number of audio channels.</value>
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
                    throw new ArgumentOutOfRangeException(nameof(value), "The number of audio channels must be greater than 0.");
                }
                _channels = value;
            }
        }

        /// <summary>
        /// Gets the audio sample rate in Hertz.
        /// </summary>
        /// <value>The audio sample rate in Hertz.</value>
        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The sample rate must be greater than 0.");
                }
                _sampleRate = value;
            }
        }

        /// <summary>
        /// Gets the number of audio frames the output is buffering by.
        /// </summary>
        /// <value>The output buffer size.</value>
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
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
        /// Gets the audio output bit depth.
        /// </summary>
        /// <value>The audio output bit depth.</value>
        public int BitDepth
        {
            get
            {
                return _bitDepth;
            }
            protected set
            {
                // Check value is between 8 and 64 bits
                if (value < 8 || value > 64)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Bit depth must be between 8 bit and 64 bit.");
                }
                // Check value is a multiple of 2 (i.e. full bytes), or 24 bits which is also valid
                if ((value & (value - 1)) != 0)
                {
                    throw new ArgumentException("Bit depth must be a power of 2, or 24 bit.", nameof(value));
                }
                // Set value
                _bitDepth = value;
            }
        }

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        /// <value>Whether the audio output is playing or not.</value>
        public PlaybackState PlaybackState { get; protected set; } = PlaybackState.STOPPED;
        #endregion

        #region Methods
        /// <summary>
        /// Add an audio input to the output.
        /// </summary>
        /// <param name="input">The audio input to be added.</param>
        public void AddInput(AudioInput input)
        {
            // Check the input audio properties matches the audio properties of the output
            if (input.Channels != Channels)
            {
                throw new ArgumentException("The number of input channels does not match the number of output channels.", nameof(input));
            }
            else if (input.SampleRate != SampleRate)
            {
                throw new ArgumentException("The input sample rate does not match the output sample rate.", nameof(input));
            }
            // Add the input to the list of inputs
            _audioInputs.Add(input);
        }

        /// <summary>
        /// Remove an audio input from the output.
        /// </summary>
        /// <param name="input">The audio input to be removed.</param>
        public void RemoveInput(AudioInput input)
        {
            // Check the given input is on the list of inputs, and remove it if it is
            if (_audioInputs.Contains(input))
            {
                _audioInputs.Remove(input);
            }
            else
            {
                throw new ArgumentException("The input hasn't been added to this output.", nameof(input));
            }
        }

        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public virtual void Start()
        {
            throw new NotImplementedException("The Start method has not been implemented for this audio output.");
        }

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public virtual void Stop()
        {
            throw new NotImplementedException("The Stop method has not been implemented for this audio output.");
        }

        /// <summary>
        /// Get frames of audio from the audio inputs.
        /// </summary>
        /// <param name="framesRequired">The number of frames required.</param>
        protected double[] GetInputFrames(int framesRequired)
        {
            // Create array of mixed (combined) frames
            double[] mixedFrames = new double[framesRequired * Channels];
            // Get samples from each input that is playing
            foreach (var input in _audioInputs)
            {
                // Only get frames from the input if it is currently playing
                if (input.PlaybackState == PlaybackState.PLAYING)
                {
                    // Get frames from the audio input
                    double[] inputFrames = input.GetFrames(framesRequired);
                    // Get the shorter length from the returned frames array or the mixed frames array
                    int mixLength = (inputFrames.Length < mixedFrames.Length) ? inputFrames.Length : mixedFrames.Length;
                    // Add the frames from the audio input to the mixed frames
                    for (int i = 0; i < mixLength; i++)
                    {
                        mixedFrames[i] += inputFrames[i];
                    }
                }
            }
            // Return the mixed samples
            return mixedFrames;
        }
        #endregion
    }
}