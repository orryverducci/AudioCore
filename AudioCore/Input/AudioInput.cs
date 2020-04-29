using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides an audio input.
    /// </summary>
    public abstract class AudioInput
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
        /// The current playback state.
        /// </summary>
        private PlaybackState _playbackState = PlaybackState.STOPPED;

        /// <summary>
        /// The set audio volume in dBFS.
        /// </summary>
        private int _volume;
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
        /// Gets the audio sample rate in Hertz
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
        /// Gets the input audio format.
        /// </summary>
        /// <value>The input audio format.</value>
        public AudioFormat Format { get; protected set; } = new AudioFormat(32, SampleType.FloatingPoint);

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        /// <value>Whether the audio input is playing or not.</value>
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            protected set
            {
                _playbackState = value;
                PlaybackStateChanged?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Gets or sets the volume of the input in dBFS.
        /// </summary>
        /// <value>The volume of the input in dBFS.</value>
        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                // Calculate linear volume (between 0 and 1) from dBFS value
                Gain = MathF.Pow(10, value / 20f);
            }
        }
        #endregion

        #region Protected Fields
        /// <summary>
        /// Gets the gain being applied to the audio.
        /// </summary>
        protected float Gain { get; private set; } = 1;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the playback state property changes.
        /// </summary>
        public event EventHandler PlaybackStateChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Start playback from the audio input.
        /// </summary>
        public virtual void Start()
        {
            if (PlaybackState == PlaybackState.STOPPED)
            {
                PlaybackState = PlaybackState.PLAYING;
            }
        }

        /// <summary>
        /// Stop playback from the audio input.
        /// </summary>
        public virtual void Stop()
        {
            if (PlaybackState != PlaybackState.STOPPED)
            {
                PlaybackState = PlaybackState.STOPPED;
            }
        }

        /// <summary>
        /// Get frames of audio samples from the input.
        /// </summary>
        /// <returns>The audio samples.</returns>
        /// <param name="audioBuffer">The buffer of audio samples to be filled by the input.</param>
        /// <param name="framesRequested">The number of frames required.</param>
        public abstract void GetFrames(Span<float> audioBuffer, int framesRequested);
        #endregion
    }
}