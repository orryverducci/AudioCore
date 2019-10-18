using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Input;
using AudioCore.Mac.Output;

namespace AudioCore.Output
{
    /// <summary>
    /// Provides an audio device output for the current platform.
    /// </summary>
    public class PlatformOutput
    {
        #region Private Fields
        /// <summary>
        /// Mac Core Audio output.
        /// </summary>
        private CoreAudioOutput coreAudioOutput;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        /// <value>The number of audio channels.</value>
        public int Channels => coreAudioOutput.Channels;

        /// <summary>
        /// Gets the audio sample rate in Hertz.
        /// </summary>
        /// <value>The audio sample rate in Hertz.</value>
        public int SampleRate => coreAudioOutput.SampleRate;

        /// <summary>
        /// Gets the output audio format.
        /// </summary>
        /// <value>The output audio format.</value>
        public AudioFormat Format => coreAudioOutput.Format;

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        /// <value>Whether the audio output is playing or not.</value>
        public PlaybackState PlaybackState => coreAudioOutput.PlaybackState;

        /// <summary>
        /// Gets the number of audio frames the output is buffering by.
        /// </summary>
        /// <value>The output buffer size.</value>
        public int BufferSize => coreAudioOutput.BufferSize;

        /// <summary>
        /// Gets the latency of the audio output in milliseconds.
        /// </summary>
        /// <value>The latency of the audio output in milliseconds.</value>
        public int Latency => coreAudioOutput.Latency;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the playback state property changes.
        /// </summary>
        public event EventHandler PlaybackStateChanged;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.PlatformOutput"/> class using the system default audio output.
        /// </summary>
        public PlatformOutput() : this(-1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.PlatformOutput"/> class using a specified audio output.
        /// </summary>
        /// <param name="deviceID">The ID of the audio output device to be used.</param>
        public PlatformOutput(long deviceID)
        {
            coreAudioOutput = new CoreAudioOutput(deviceID, -1, -1);
            coreAudioOutput.PlaybackStateChanged += (sender, e) =>
            {
                PlaybackStateChanged?.Invoke(sender, e);
            };
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Demo.PlatformOutput"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:AudioCore.Demo.PlatformOutput"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="T:AudioCore.Demo.PlatformOutput"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:AudioCore.Demo.PlatformOutput"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:AudioCore.Demo.PlatformOutput"/> was occupying.</remarks>
        public void Dispose() => coreAudioOutput.Dispose();
        #endregion

        #region Methods
        /// <summary>
        /// Add an audio input to the output.
        /// </summary>
        /// <param name="input">The audio input to be added.</param>
        public void AddInput(AudioInput input) => coreAudioOutput.AddInput(input);

        /// <summary>
        /// Remove an audio input from the output.
        /// </summary>
        /// <param name="input">The audio input to be removed.</param>
        public void RemoveInput(AudioInput input) => coreAudioOutput.RemoveInput(input);

        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public void Start() => coreAudioOutput.Start();

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public void Stop() => coreAudioOutput.Stop();

        /// <summary>
        /// Gets the available output audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available output devices.</returns>
        public static List<AudioDevice> GetDevices() => CoreAudioOutput.GetDevices();
        #endregion
    }
}