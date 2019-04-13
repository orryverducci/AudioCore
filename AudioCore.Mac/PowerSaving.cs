using System;
using AudioCore.Mac.Common;

namespace AudioCore.Mac
{
    public class PowerSaving
    {
        /// <summary>
        /// Gets or sets if macOS can save power by increasing the audio buffer size when required, for example when a device is on battery power.
        /// </summary>
        /// <value><c>true</c> if power saving is allowed, otherwise <c>false</c>.</value>
        public static bool AllowPowerSaving
        {
            get => AudioUnitExtensions.GetPowerSavingEnabled();
            set => AudioUnitExtensions.SetPowerSavingEnabled(value);
        }
    }
}