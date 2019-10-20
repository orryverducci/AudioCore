using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Input;

namespace AudioCore.Output
{
    /// <summary>
    /// Provides an audio output.
    /// </summary>
    public abstract class AudioOutput
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
        /// The audio inputs.
        /// </summary>
        private List<AudioInput> _audioInputs = new List<AudioInput>();

        /// <summary>
        /// The current playback state.
        /// </summary>
        private PlaybackState _playbackState = PlaybackState.STOPPED;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        /// <value>The number of audio channels.</value>
        public int Channels
        {
            get => _channels;
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
            get => _sampleRate;
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
        /// Gets the output audio format.
        /// </summary>
        /// <value>The output audio format.</value>
        public AudioFormat Format { get; protected set; }

        /// <summary>
        /// Gets a read only list of the audio inputs that have been added to the output.
        /// </summary>
        /// <value>A read only list of audio inputs.</value>
        public ReadOnlyCollection<AudioInput> Inputs { get; protected set; }

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        /// <value>Whether the audio output is playing or not.</value>
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            protected set
            {
                _playbackState = value;
                PlaybackStateChanged?.Invoke(this, null);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the playback state property changes.
        /// </summary>
        public event EventHandler PlaybackStateChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Output.AudioOutput"/> class.
        /// </summary>
        public AudioOutput()
        {
            Inputs = _audioInputs.AsReadOnly();
        }
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
        public abstract void Start();

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Get frames of audio from the audio inputs.
        /// </summary>
        /// <param name="framesRequired">The number of frames required.</param>
        protected float[] GetInputFrames(int framesRequired)
        {
            // Create array of mixed (combined) frames
            float[] mixedFrames = new float[framesRequired * Channels];
            // Get samples from each input that is playing
            foreach (var input in _audioInputs)
            {
                // Only get frames from the input if it is currently playing
                if (input.PlaybackState == PlaybackState.PLAYING)
                {
                    // Get frames from the audio input
                    float[] inputFrames = input.GetFrames(framesRequired);
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