using System;

namespace AudioCore.Converters
{
    public static class BitDepthConverter
    {
        #region Convert To Methods
        /// <summary>
        /// Convert the <paramref name="samples"/> to 8 bit samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static sbyte[] To8Bit(double[] samples)
        {
            // Create an array of 8 bit integers big enough to hold all the samples
            sbyte[] convertedSamples = new sbyte[samples.Length];
            // Convert each sample to a 8 bit integer
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get sample
                double sample = samples[i];
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
                sample *= sbyte.MaxValue;
                // Save sample as a 8 bit integer
                convertedSamples[i] = (sbyte)Math.Round(sample);
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Convert the <paramref name="samples"/> to 16 bit samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static short[] To16Bit(double[] samples)
        {
            // Create an array of 16 bit integers big enough to hold all the samples
            short[] convertedSamples = new short[samples.Length];
            // Convert each sample to a 16 bit integer
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get sample
                double sample = samples[i];
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
                sample *= short.MaxValue;
                // Save sample as a 16 bit integer
                convertedSamples[i] = (short)Math.Round(sample);
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Convert the <paramref name="samples"/> to 32 bit samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static int[] To32Bit(double[] samples)
        {
            // Create an array of 32 bit integers big enough to hold all the samples
            int[] convertedSamples = new int[samples.Length];
            // Convert each sample to a 32 bit integer
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get sample
                double sample = samples[i];
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
                sample *= int.MaxValue;
                // Save sample as a 32 bit integer
                convertedSamples[i] = (int)Math.Round(sample);
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Convert the <paramref name="samples"/> to 64 bit samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static long[] To64Bit(double[] samples)
        {
            // Create an array of 64 bit integers big enough to hold all the samples
            long[] convertedSamples = new long[samples.Length];
            // Convert each sample to a 64 bit integer
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Get sample
                double sample = samples[i];
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
                sample *= long.MaxValue;
                // Save sample as a 64 bit integer
                convertedSamples[i] = (long)Math.Round(sample);
            }
            // Return converted samples
            return convertedSamples;
        }

        /// <summary>
        /// Convert the <paramref name="samples"/> to 32 bit floating-point samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static float[] ToFloat(double[] samples)
        {
            // Create an array of floats big enough to hold all the samples
            float[] convertedSamples = new float[samples.Length];
            // Convert each sample to a float
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Save sample as a 16 bit integer
                convertedSamples[i] = (float)samples[i];
            }
            // Return converted samples
            return convertedSamples;
        }
        #endregion

        #region Convert From Methods
        /// <summary>
        /// Convert the 8 bit <paramref name="samples"/> to 64 bit floating-point samples.
        /// </summary>
        /// <returns>The converted samples.</returns>
        /// <param name="samples">The samples to convert.</param>
        public static double[] From8Bit(sbyte[] samples)
        {
            // Create an array of doubles big enough to hold all the samples
            double[] convertedSamples = new double[samples.Length];
            // Convert each sample to a double
            for (int i = 0; i < convertedSamples.Length; i++)
            {
                // Save sample as a double
                convertedSamples[i] = samples[i];
                // Convert sample to floating-point scale
                convertedSamples[i] /= sbyte.MaxValue;
            }
            // Return converted samples
            return convertedSamples;
        }
        #endregion
    }
}