using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Mac.Input;

namespace AudioCore.Demo
{
    /// <summary>
    /// Provides the audio device input for the platform.
    /// </summary>
    public class PlatformInput : IDisposable
    {
        /// <summary>
        /// Gets the Mac Core Audio input.
        /// </summary>
        /// <value>The input.</value>
        public CoreAudioInput Input { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.PlatformInput"/> class.
        /// </summary>
        public PlatformInput(long deviceID) => Input = new CoreAudioInput(deviceID, -1, -1);

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Demo.PlatformInput"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:AudioCore.Demo.PlatformInput"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="T:AudioCore.Demo.PlatformInput"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:AudioCore.Demo.PlatformInput"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:AudioCore.Demo.PlatformInput"/> was occupying.</remarks>
        public void Dispose() => Input.Dispose();

        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        /// <value>The number of audio channels.</value>
        public int Channels => Input.Channels;

        /// <summary>
        /// Gets the audio sample rate in Hertz.
        /// </summary>
        /// <value>The audio sample rate in Hertz.</value>
        public int SampleRate => Input.SampleRate;

        /// <summary>
        /// Gets or sets the size of the buffer, in number of frames.
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int BufferSize
        {
            get => Input.BufferSize;
            set => Input.BufferSize = value;
        }

        /// <summary>
        /// Gets the available input audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available input devices.</returns>
        public static List<AudioDevice> GetDevices() => CoreAudioInput.GetDevices();

        /// <summary>
        /// Start the audio input.
        /// </summary>
        public void Start() => Input.Start();

        /// <summary>
        /// Stop the audio input.
        /// </summary>
        public void Stop() => Input.Stop();
    }
}