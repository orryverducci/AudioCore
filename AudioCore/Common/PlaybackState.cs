using System;

namespace AudioCore.Common
{
    /// <summary>
    /// Playback states.
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        /// Playback is stopped.
        /// </summary>
        STOPPED,
        /// <summary>
        /// Audio is currently buffering.
        /// </summary>
        BUFFERING,
        /// <summary>
        /// Currently playing.
        /// </summary>
        PLAYING
    }
}