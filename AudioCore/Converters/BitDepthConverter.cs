using System;

namespace AudioCore.Converters
{
    /// <summary>
    /// Provides methods to convert samples to PCM data at various bit depths, and to convert them back.
    /// </summary>
    public static class BitDepthConverter
    {
        #region Generic Convert Methods
        /// <summary>
        /// Converts the <paramref name="samples"/> to PCM audio data at the specified bit depth.
        /// </summary>
        /// <returns>Bytes containing the PCM audio data at the specified bit depth.</returns>
        /// <param name="samples">The audio samples to be converted.</param>
        /// <param name="outputBitDepth">The output bit depth required.</param>
        /// <param name="floatingPoint">If the samples should be converted to floating-point data. This parameter is optional and set to <c>false</c> by default.</param>
        public static byte[] ToPCM(float[] samples, byte outputBitDepth, bool floatingPoint = false)
        {
            // If not converting to floating-point, use the correct integer conversion for the specified bit depth
            if (!floatingPoint)
            {
                switch (outputBitDepth)
                {
                    case 8:
                        return To8Bit(samples);
                    case 16:
                        return To16Bit(samples);
                    case 24:
                        return To24Bit(samples);
                    case 32:
                        return To32Bit(samples);
                    default:
                        // Not a valid bit depth, throw the relevent error
                        if (outputBitDepth < 8 || outputBitDepth > 32)
                        {
                            throw new ArgumentOutOfRangeException(nameof(outputBitDepth), "Bit depth must be between 8 bit and 32 bit.");
                        }
                        throw new ArgumentException("Bit depth must be a power of 2, or 24 bit.", nameof(outputBitDepth));
                }
            }
            // Otherwise use the correct floating-point conversion for the specified bit depth
            switch (outputBitDepth)
            {
                case 16:
                    return ToFloat(samples);
                case 32:
                    return ToDouble(samples);
                default:
                    // Not a valid bit depth, throw the relevent error
                    throw new ArgumentOutOfRangeException(nameof(outputBitDepth), "Bit depth must be 32 bit or 64 bit when using floating point samples.");
            }
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">The bytes to convert to samples.</param>
        /// <param name="bitDepth">The input bit depth.</param>
        /// <param name="floatingPoint">If the data is floating-point. This parameter is optional and set to <c>false</c> by default.</param>
        public static float[] FromPCM(byte[] bytes, byte bitDepth, bool floatingPoint = false)
        {
            // If not converting from floating-point, use the correct integer conversion for the specified bit depth
            if (!floatingPoint)
            {
                switch (bitDepth)
                {
                    case 8:
                        return From8Bit(bytes);
                    case 16:
                        return From16Bit(bytes);
                    case 32:
                        return From32Bit(bytes);
                    default:
                        // Not a valid bit depth, throw relevent error
                        if (bitDepth < 8 || bitDepth > 32)
                        {
                            throw new ArgumentOutOfRangeException(nameof(bitDepth), "Bit depth must be between 8 bit and 32 bit.");
                        }
                        throw new ArgumentException("Bit depth must be a power of 2, or 24 bit.", nameof(bitDepth));
                }
            }
            // Otherwise use the correct floating-point conversion for the specified bit depth
            switch (bitDepth)
            {
                case 16:
                    return FromFloat(bytes);
                case 32:
                    return FromDouble(bytes);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bitDepth), "Bit depth must be 32 bit or 64 bit when using floating point samples.");
            }
        }
        #endregion

        #region Convert To Methods
        /// <summary>
        /// Converts the <paramref name="samples"/> to 8 bit PCM audio data.
        /// </summary>
        /// <returns>Bytes containing 8 bit PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        /// <param name="signed">If the PCM audio data should be signed. This parameter is optional and set to <c>false</c> by default.</param>
        public static byte[] To8Bit(float[] samples, bool signed = false)
        {
            // Create an array of bytes big enough to hold all the converted samples
            byte[] convertedSamples = new byte[samples.Length];
            // Allocate memory for the sample being processed
            double sample;
            // Convert each sample to a byte
            for (int i = 0; i < samples.Length; i++)
            {
                // Get sample
                sample = samples[i];
                // Limit sample if is above 1, or below -1
                if (sample > 1.0)
                {
                    sample = 1.0;
                }
                else if (sample < -1.0)
                {
                    sample = -1.0;
                }
                // Convert sample to integer scale
                if (!signed)
                {
                    sample = (sample + 1) * 127.5;
                }
                else
                {
                    sample *= 127;
                }
                // Save sample as a 8 bit integer
                convertedSamples[i] = (byte)Math.Round(sample);
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="samples"/> to 16 bit PCM audio data.
        /// </summary>
        /// <returns>Bytes containing 16 bit PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        /// <param name="signed">If the PCM audio data should be signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static byte[] To16Bit(float[] samples, bool signed = true)
        {
            // Create an array of 16 bit integers big enough to hold all the samples
            ushort[] convertedSamples = new ushort[samples.Length];
            // Allocate memory for the sample being processed
            double sample;
            // Convert each sample to a 16 bit integer
            for (int i = 0; i < samples.Length; i++)
            {
                // Get sample
                sample = samples[i];
                // Limit sample if is above 1, or below -1
                if (sample > 1.0)
                {
                    sample = 1.0;
                }
                else if (sample < -1.0)
                {
                    sample = -1.0;
                }
                // Convert sample to integer scale
                if (signed)
                {
                    sample *= 32767;
                }
                else
                {
                    sample = (sample + 1) * 32767.5;
                }
                // Save sample as a 16 bit integer
                convertedSamples[i] = (ushort)Math.Round(sample);
            }
            // Return converted samples as bytes
            byte[] convertedBytes = new byte[samples.Length * 2];
            Buffer.BlockCopy(convertedSamples, 0, convertedBytes, 0, convertedBytes.Length);
            return convertedBytes;
        }

        /// <summary>
        /// Converts the <paramref name="samples"/> to 24 bit PCM audio data.
        /// </summary>
        /// <returns>Bytes containing 24 bit PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        /// <param name="signed">If the PCM audio data should be signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static byte[] To24Bit(float[] samples, bool signed = true)
        {
            // Create an array of bytes big enough to hold all the samples as 24 bit PCM data
            byte[] convertedSamples = new byte[samples.Length * 3];
            // Allocate memory for the sample being processed, and the converted bytes
            double sample;
            byte[] convertedBytes;
            // Convert each sample to a 16 bit integer
            for (int i = 0; i < samples.Length; i++)
            {
                // Get sample
                sample = samples[i];
                // Limit sample if is above 1, or below -1
                if (sample > 1.0)
                {
                    sample = 1.0;
                }
                else if (sample < -1.0)
                {
                    sample = -1.0;
                }
                // Convert sample to integer scale
                if (signed)
                {
                    sample *= 8388607;
                }
                else
                {
                    sample = (sample + 1) * 8388607.5;
                }
                // Save sample as a 24 bit integer within the array of bytes
                convertedBytes = BitConverter.GetBytes((int)Math.Round(sample));
                Buffer.BlockCopy(convertedBytes, 0, convertedSamples, i * 3, 3);
            }
            // Return converted samples as bytes
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="samples"/> to 32 bit PCM audio data.
        /// </summary>
        /// <returns>Bytes containing 32 bit PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        /// <param name="signed">If the PCM audio data should be signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static byte[] To32Bit(float[] samples, bool signed = true)
        {
            // Create an array of 32 bit integers big enough to hold all the samples
            uint[] convertedSamples = new uint[samples.Length];
            // Allocate memory for the sample being processed
            double sample;
            // Convert each sample to a 32 bit integer
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get sample
                sample = samples[i];
                // Limit sample if is above 1, or below -1
                if (sample > 1.0)
                {
                    sample = 1.0;
                }
                else if (sample < -1.0)
                {
                    sample = -1.0;
                }
                // Convert sample to integer scale
                if (signed)
                {
                    sample *= 2147483647;
                }
                else
                {
                    sample = (sample + 1) * 2147483647.5;
                }
                // Save sample as a 32 bit integer
                convertedSamples[i] = (uint)Math.Round(sample);
            }
            // Return converted samples as bytes
            byte[] convertedBytes = new byte[samples.Length * 4];
            Buffer.BlockCopy(convertedSamples, 0, convertedBytes, 0, convertedBytes.Length);
            return convertedBytes;
        }

        /// <summary>
        /// Converts the <paramref name="samples"/> to 32 bit floating-point PCM audio data.
        /// As samples in AudioCore are already 32 bit floating-point PCM, this is a convenience method which returns the samples as bytes.
        /// </summary>
        /// <returns>Bytes containing 32 bit floating-point PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static byte[] ToFloat(float[] samples)
        {
            // Return samples as bytes
            byte[] convertedSamples = new byte[samples.Length * 4];
            Buffer.BlockCopy(samples, 0, convertedSamples, 0, convertedSamples.Length);
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="samples"/> to 64 bit floating-point PCM audio data.
        /// </summary>
        /// <returns>Bytes containing 64 bit floating-point PCM audio data.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static byte[] ToDouble(float[] samples)
        {
            // Create an array of floats big enough to hold all the samples
            double[] convertedSamples = new double[samples.Length];
            // Convert each sample to a double
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                convertedSamples[i] = (double)samples[i];
            }
            // Return converted samples as bytes
            byte[] convertedBytes = new byte[samples.Length * 8];
            Buffer.BlockCopy(convertedSamples, 0, convertedBytes, 0, convertedBytes.Length);
            return convertedBytes;
        }
        #endregion

        #region Convert From Methods
        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 8 bit PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 8 bit PCM audio data.</param>
        /// <param name="signed">If the PCM audio data is signed. This parameter is optional and set to <c>false</c> by default.</param>
        public static float[] From8Bit(byte[] bytes, bool signed = false)
        {
            // Create an array of doubles big enough to hold all the samples
            float[] convertedSamples = new float[bytes.Length];
            // Convert each byte to a floating-point sample
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                if (!signed)
                {
                    convertedSamples[i] = (bytes[i] / 127.5f) - 1;
                }
                else
                {
                    convertedSamples[i] = ((sbyte)bytes[i] / 127f);
                }
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 16 bit PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 16 bit PCM audio data.</param>
        /// <param name="signed">If the PCM audio data is signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static float[] From16Bit(byte[] bytes, bool signed = true)
        {
            // Create an array of doubles big enough to hold all the samples
            float[] convertedSamples = new float[bytes.Length / 2];
            // Convert each block of bytes to a floating-point sample
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                if (signed)
                {
                    convertedSamples[i] = BitConverter.ToInt16(bytes, i * 2) / 32767f;
                }
                else
                {
                    convertedSamples[i] = (BitConverter.ToUInt16(bytes, i * 2) / 32767.5f) - 1;
                }
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 24 bit PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 24 bit PCM audio data.</param>
        /// <param name="signed">If the PCM audio data is signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static float[] From24Bit(byte[] bytes, bool signed = true)
        {
            // Create an array of doubles big enough to hold all the samples
            float[] convertedSamples = new float[bytes.Length / 3];
            // Create an array of 4 bytes to store blocks of bytes while they are being processed
            byte[] sampleBytes = new byte[4];
            // Convert each block of bytes to a floating-point sample
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get the block of bytes for the sample
                Buffer.BlockCopy(bytes, i * 3, convertedSamples, 0, 3);
                // Convert bytes to floating-point sample
                if (signed)
                {
                    convertedSamples[i] = BitConverter.ToInt32(sampleBytes, 0) / 8388607f;
                }
                else
                {
                    convertedSamples[i] = (BitConverter.ToInt32(sampleBytes, 0) / 8388607.5f) - 1;
                }
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 32 bit PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 32 bit PCM audio data.</param>
        /// <param name="signed">If the PCM audio data is signed. This parameter is optional and set to <c>true</c> by default.</param>
        public static float[] From32Bit(byte[] bytes, bool signed = true)
        {
            // Create an array of doubles big enough to hold all the samples
            float[] convertedSamples = new float[bytes.Length / 4];
            // Convert each block of bytes to a floating-point sample
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                if (signed)
                {
                    convertedSamples[i] = BitConverter.ToInt32(bytes, i * 4) / 2147483647f;
                }
                else
                {
                    convertedSamples[i] = (BitConverter.ToUInt32(bytes, i * 4) / 2147483647.5f) - 1;
                }
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 32 bit floating-point PCM audio data to samples.
        /// As samples in AudioCore are already 32 bit floating-point PCM, this is a convenience method which returns the bytes as samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 32 bit floating-point PCM audio data.</param>
        public static float[] FromFloat(byte[] bytes)
        {
            // Return bytes as samples
            float[] convertedSamples = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, convertedSamples, 0, bytes.Length);
            return convertedSamples;
        }

        /// <summary>
        /// Converts the <paramref name="bytes"/> containing 64 bit floating-point PCM audio data to samples.
        /// </summary>
        /// <returns>The converted audio samples.</returns>
        /// <param name="bytes">Bytes containing 64 bit floating-point PCM audio data.</param>
        public static float[] FromDouble(byte[] bytes)
        {
            // Create an array of doubles big enough to hold all the samples
            float[] convertedSamples = new float[bytes.Length / 8];
            // Convert each block of bytes to a 64 bit floating-point sample
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                convertedSamples[i] = (float)BitConverter.ToDouble(bytes, i * 8);
            }
            // Return converted samples
            return convertedSamples;
        }
        #endregion
    }
}