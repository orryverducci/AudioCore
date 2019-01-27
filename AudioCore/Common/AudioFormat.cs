using System;

namespace AudioCore.Common
{
    /// <summary>
    /// Represents the data format of a sample of audio.
    /// </summary>
    public class AudioFormat
    {
        #region Properties
        /// <summary>
        /// Gets the audio bit depth.
        /// </summary>
        /// <value>The audio bit depth.</value>
        public int BitDepth { get; private set; }

        /// <summary>
        /// Gets the type of each sample.
        /// </summary>
        /// <value>The audio sample type.</value>
        public SampleType SampleType { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Common.AudioFormat"/> class.
        /// </summary>
        /// <param name="bitDepth">The audio bit depth.</param>
        /// <param name="sampleType">The audio sample type.</param>
        public AudioFormat(int bitDepth, SampleType sampleType)
        {
            // Check the bit depth is in range for the chosen sample type
            if (sampleType == SampleType.FloatingPoint && (bitDepth < 32 || bitDepth > 64))
            {
                throw new ArgumentOutOfRangeException(nameof(bitDepth), "Bit depth must be 32 bit or 64 bit for floating-point sample types.");
            }
            else if (bitDepth < 8 || bitDepth > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(bitDepth), "Bit depth must be between 8 bit and 64 bit for integer sample types.");
            }
            // Check the bit depth is a multiple of 2 (i.e. full bytes), or 24 bits which is also valid
            if ((bitDepth & (bitDepth - 1)) != 0)
            {
                throw new ArgumentException("Bit depth must be a power of 2, or 24 bit.", nameof(bitDepth));
            }
            // Set properties
            BitDepth = bitDepth;
            SampleType = sampleType;
        }
        #endregion
    }
}