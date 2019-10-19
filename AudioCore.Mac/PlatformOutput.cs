using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Mac.Output;

namespace AudioCore.Output
{
    /// <summary>
    /// Provides an audio device output for the current platform.
    /// </summary>
    public class PlatformOutput : CoreAudioOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Output.PlatformOutput"/> class using the system default audio output device.
        /// </summary>
        public PlatformOutput() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Output.PlatformOutput"/> class using a specified audio output device.
        /// </summary>
        /// <param name="deviceID">The ID of the audio output device to be used.</param>
        public PlatformOutput(long deviceID) : base(deviceID, -1, -1) {}

        /// <summary>
        /// Gets the available output audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available output devices.</returns>
        public static new List<AudioDevice> GetDevices() => CoreAudioOutput.GetDevices();
    }
}