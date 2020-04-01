using System;
using System.Timers;
using AudioCore.Common;

namespace AudioCore.Output
{
    /// <summary>
    /// Provides a dummy audio output.
    /// </summary>
    public class DummyOutput : AudioOutput, IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The timer to be used to get frames of audio.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// The buffer size to be used when getting frames of audio.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// The audio buffer to be used when getting frames of audio.
        /// </summary>
        private float[] _audioBuffer;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Output.DummyOutput"/> class with the default buffer size.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public DummyOutput(int channels, int sampleRate) : this(channels, sampleRate, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Output.DummyOutput"/> class.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        /// <param name="bufferSize">The buffer size to be used in frames.</param>
        public DummyOutput(int channels, int sampleRate, int bufferSize)
        {
            // Check buffer size is valid
            if (bufferSize < 1 && bufferSize != -1)
            {
                throw new ArgumentException("The buffer size must be greater than 1.", "bufferSize");
            }
            // Initialise properties and buffer size
            Channels = channels;
            SampleRate = sampleRate;
            if (bufferSize == -1)
            {
                _bufferSize = 1024;
            }
            else
            {
                _bufferSize = bufferSize;
            }
            // Initialise audio buffer
            _audioBuffer = new float[bufferSize];
            // Setup timer to be used to get frames of audio
            _timer = new Timer((1000d / SampleRate) * bufferSize);
            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Output.DummyOutput"/> object.
        /// </summary>
        /// <remarks>The dummy output doesn't actually have any resources to release, so it's not neccessary to call <see cref="Dispose"/>.
        /// This method is included to provided API compatibility with hardware outputs.</remarks>
        public void Dispose() { }
        #endregion

        #region Playback Methods
        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public override void Start()
        {
            if (PlaybackState != PlaybackState.PLAYING)
            {
                _timer.Start();
                PlaybackState = PlaybackState.PLAYING;
            }
        }

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public override void Stop()
        {
            if (PlaybackState != PlaybackState.STOPPED)
            {
                _timer.Stop();
                PlaybackState = PlaybackState.STOPPED;
            }
        }

        /// <summary>
        /// Retrieves frames of audio from the inputs when the timer elapses.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Wrap the audio buffer in a span
            Span<float> audioBuffer = _audioBuffer.AsSpan();
            // Get frames of audio from the inputs
            GetInputFrames(audioBuffer, _bufferSize);
        }
        #endregion
    }
}