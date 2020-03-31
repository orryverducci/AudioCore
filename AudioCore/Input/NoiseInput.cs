using System;
using AudioCore.Common;

namespace AudioCore.Input
{
    /// <summary>
    /// Provides a generated noise audio input.
    /// </summary>
    public class NoiseInput : AudioInput
    {
        #region Enumerations
        /// <summary>
        /// Types of noise.
        /// </summary>
        public enum NoiseType
        {
            White,
            Pink,
            Brown
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// The random number generator used to generate noise.
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// The set audio volume in dBFS.
        /// </summary>
        private int _volumeDBFS;

        /// <summary>
        /// The set audio volume as a linear value.
        /// </summary>
        private float _volumeLinear = 1;

        /// <summary>
        /// A buffer containing an array samples to be used when generating pink noise.
        /// </summary>
        private float[] _pinkNoiseBuffer = new float[7];

        /// <summary>
        /// A buffer containing the previously output brown noise sample.
        /// </summary>
        private float _brownNoiseBuffer = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the volume of the test tone in dBFS.
        /// </summary>
        /// <value>The volume of the noise in dBFS.</value>
        public int Volume
        {
            get => _volumeDBFS;
            set
            {
                _volumeDBFS = value;
                // Calculate linear volume (between 0 and 1) from dBFS value
                _volumeLinear = MathF.Pow(10, value / 20f);
            }
        }

        /// <summary>
        /// Gets or sets the type of noise to be generated. Set to white noise by default.
        /// </summary>
        /// <value>The type of noise to be generated.</value>
        public NoiseType Type { get; set; } = NoiseType.White;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Input.NoiseInput"/> class.
        /// </summary>
        /// <param name="channels">The number of audio channels required.</param>
        /// <param name="sampleRate">The audio sample rate required in Hertz.</param>
        public NoiseInput(int channels, int sampleRate)
        {
            // Set properties
            Channels = channels;
            SampleRate = sampleRate;
            // Set playback state as playing
            PlaybackState = PlaybackState.PLAYING;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get noise audio frames.
        /// </summary>
        /// <returns>The noise audio frames.</returns>
        /// <param name="audioBuffer">The buffer of audio samples to be filled by the input.</param>
        /// <param name="framesRequested">The number of frames required.</param>
        public override void GetFrames(Span<float> audioBuffer, int framesRequested)
        {
            // Initialise variables to store generated samples
            float whiteSample, sample = 0;
            // Generate audio for frames requested
            for (int i = 0; i < (framesRequested); i++)
            {
                // Generate a white noise sample
                whiteSample = ((float)_random.NextDouble() * 2) - 1;
                // Generate a sample for the chosen type of noise
                switch (Type)
                {
                    case NoiseType.White:
                        sample = whiteSample * _volumeLinear;
                        break;
                    case NoiseType.Pink:
                        _pinkNoiseBuffer[0] = 0.99886f * _pinkNoiseBuffer[0] + whiteSample * 0.0555179f;
                        _pinkNoiseBuffer[1] = 0.99332f * _pinkNoiseBuffer[1] + whiteSample * 0.0750759f;
                        _pinkNoiseBuffer[2] = 0.96900f * _pinkNoiseBuffer[2] + whiteSample * 0.1538520f;
                        _pinkNoiseBuffer[3] = 0.86650f * _pinkNoiseBuffer[3] + whiteSample * 0.3104856f;
                        _pinkNoiseBuffer[4] = 0.55000f * _pinkNoiseBuffer[4] + whiteSample * 0.5329522f;
                        _pinkNoiseBuffer[5] = -0.7616f * _pinkNoiseBuffer[5] - whiteSample * 0.0168980f;
                        sample = (_pinkNoiseBuffer[0] + _pinkNoiseBuffer[1] + _pinkNoiseBuffer[2] + _pinkNoiseBuffer[3] + _pinkNoiseBuffer[4] + _pinkNoiseBuffer[5] + _pinkNoiseBuffer[6] + whiteSample * 0.5362f) * 0.11f * _volumeLinear;
                        _pinkNoiseBuffer[6] = whiteSample * 0.115926f;
                        break;
                    case NoiseType.Brown:
                        _brownNoiseBuffer = (_brownNoiseBuffer + (0.02f * whiteSample)) / 1.02f;
                        sample = _brownNoiseBuffer * 3.5f * _volumeLinear;
                        break;
                }
                // Copy sample to each channel
                int firstChannelSample = i * Channels;
                for (int x = 0; x < Channels; x++)
                {
                    audioBuffer[firstChannelSample + x] = sample;
                }
            }
        }
        #endregion
    }
}