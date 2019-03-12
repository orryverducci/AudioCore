using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Input;
using AudioCore.Mac.Output;

namespace AudioCore.Demo
{
    /// <summary>
    /// Provides the audio device output for the platform.
    /// </summary>
    public class PlatformOutput : IDisposable
    {
        /// <summary>
        /// Mac Core Audio output.
        /// </summary>
        private CoreAudioOutput coreAudioOutput;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.PlatformOutput"/> class.
        /// </summary>
        public PlatformOutput(long deviceID)
        {
            coreAudioOutput = new CoreAudioOutput(deviceID, -1, -1);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Demo.PlatformOutput"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:AudioCore.Demo.PlatformOutput"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="T:AudioCore.Demo.PlatformOutput"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:AudioCore.Demo.PlatformOutput"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:AudioCore.Demo.PlatformOutput"/> was occupying.</remarks>
        public void Dispose()
        {
            coreAudioOutput.Dispose();
        }

        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        /// <value>The number of audio channels.</value>
        public int Channels
        {
            get => coreAudioOutput.Channels;
        }

        /// <summary>
        /// Gets the audio sample rate in Hertz.
        /// </summary>
        /// <value>The audio sample rate in Hertz.</value>
        public int SampleRate
        {
            get => coreAudioOutput.SampleRate;
        }

        /// <summary>
        /// Gets the current playback state of the audio output.
        /// </summary>
        /// <value>The playback state.</value>
        public PlaybackState PlaybackState
        {
            get => coreAudioOutput.PlaybackState;
        }

        /// <summary>
        /// Add an audio input to the output.
        /// </summary>
        /// <param name="input">The audio input to be added.</param>
        public void AddInput(AudioInput input)
        {
            coreAudioOutput.AddInput(input);
        }

        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public void Start()
        {
            coreAudioOutput.Start();
        }

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public void Stop()
        {
            coreAudioOutput.Stop();
        }

        /// <summary>
        /// Gets the available output audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available output devices.</returns>
        public static List<AudioDevice> GetDevices()
        {
            return CoreAudioOutput.GetDevices();
        }
    }
}