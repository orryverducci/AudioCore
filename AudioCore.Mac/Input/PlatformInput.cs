using System;
using System.Collections.Generic;
using AudioCore.Common;
using AudioCore.Mac.Input;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides the audio device input for the current platform.
    /// </summary>
    public class PlatformInput : CoreAudioInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Input.PlatformInput"/> class using the system default audio input device.
        /// </summary>
        public PlatformInput() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Input.PlatformInput"/> class using a specified audio input device.
        /// </summary>
        /// <param name="deviceID">The ID of the audio input device to be used.</param>
        public PlatformInput(long deviceID) : base(deviceID, -1, -1) {}

        /// <summary>
        /// Gets the available input audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available input devices.</returns>
        public static new List<AudioDevice> GetDevices() => CoreAudioInput.GetDevices();
    }
}