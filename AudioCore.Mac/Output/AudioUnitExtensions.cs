using System;
using System.Runtime.InteropServices;
using AudioUnit;
using ObjCRuntime;

namespace AudioCore.Mac.Output
{
    /// <summary>
    /// Adds extension methods to the <see cref="T:AudioUnit.AudioUnit"/> class, providing latency information
    /// </summary>
    internal static class AudioUnitExtensions
    {
        /// <summary>
        /// The property ID for the buffer frame size.
        /// </summary>
        private const int kAudioDevicePropertyBufferFrameSize = 0x6673697a; // This value is equivalent to 'fsiz' in ASCII

        /// <summary>
        /// Gets a property from the audio unit.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inUnit">The handle of the audio unit to get the property from.</param>
        /// <param name="inID">The identifier for the property to be retrieved.</param>
        /// <param name="inScope">The scope of the audio unit property.</param>
        /// <param name="inElement">The audio unit element to retrieve the property from.</param>
        /// <param name="outData">A variable to output the property value to.</param>
        /// <param name="ioDataSize">The size of the variable to output in bytes.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, int inID, AudioUnitScopeType inScope, uint inElement, ref uint outData, ref uint ioDataSize);

        /// <summary>
        /// Gets the number of frames the output audio unit is buffering by.
        /// </summary>
        /// <returns>The number of frames in the buffer.</returns>
        internal static uint GetBufferFrames(this AudioUnit.AudioUnit audioUnit)
        {
            // The size in bytes of the data to be returned
            uint size = sizeof(uint);
            // The number of frames to be returned, defaulting to 0
            uint bufferFrames = 0;
            // Get the buffer frames property
            AudioUnitStatus status = AudioUnitGetProperty(audioUnit.Handle, kAudioDevicePropertyBufferFrameSize, AudioUnitScopeType.Global, 0, ref bufferFrames, ref size);
            // If getting the property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception(status.ToString());
            }
            // Return the number of frames
            return bufferFrames;
        }
    }
}