using System;

namespace AudioCore.Common
{
    /// <summary>
    /// Represents an audio device.
    /// </summary>
    public class AudioDevice
    {
        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>The device identifier.</value>
        public uint ID { get; set; }

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        /// <value>The device name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets if the device is the default.
        /// </summary>
        /// <value><c>true</c> if device is the default, otherwise <c>false</c>.</value>
        public bool Default { get; set; }
    }
}