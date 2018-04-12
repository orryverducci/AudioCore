using System;

namespace AudioCore
{
    public class TestToneInput : AudioInput
    {
        #region Private Fields
        /// <summary>
        /// The number of the current sample in the sine wave
        /// </summary>
        private int _sampleNumber;

        /// <summary>
        /// The set audio volume in dBFS
        /// </summary>
        private int _volumeDBFS;

        /// <summary>
        /// The set audio volume as a linear value
        /// </summary>
        private double _volumeLinear;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the frequency of the test tone in Hertz
        /// </summary>
        /// <value>The frequency of the test tone in Hertz</value>
        public int Frequency { get; private set; }

        /// <summary>
        /// Gets the volume of the test tone in dBFS
        /// </summary>
        /// <value>The volume of the test tone in dBFS</value>
        public int Volume
        {
            get
            {
                return _volumeDBFS;
            }
            private set
            {
                _volumeDBFS = value;
                // Calculate linear volume (between 0 and 1) from dBFS value
                _volumeLinear = Math.Pow(10, Volume / 20);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
        /// Generates a continuous 1kHz sine wave test tone.
        /// </summary>
        /// <param name="channels">The number of audio channels required</param>
        /// <param name="sampleRate">The audio sample rate required</param>
        /// <param name="volume">The audio output volume in dBFS</param>
        public TestToneInput(int channels, int sampleRate, int volume) : this(channels, sampleRate, 1000, volume) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Foundation.TestToneInput"/> class.
        /// Generates a continuous sine wave test tone at the requested frequency.
        /// </summary>
        /// <param name="channels">The number of audio channels required</param>
        /// <param name="sampleRate">The audio sample rate required</param>
        /// <param name="frequency">The frequency of the tone required in Hertz</param>
        /// <param name="volume">The audio output volume in dBFS</param>
        public TestToneInput(int channels, int sampleRate, int frequency, int volume)
        {
            // Set properties
            Channels = channels;
            SampleRate = sampleRate;
            Frequency = frequency;
            Volume = volume;
            // Set bit depth to 64 bits
            BitDepth = 64;
            // Set playback state as playing
            PlaybackState = PlaybackState.PLAYING;
        }
        #endregion

        #region Methods
        public override double[] GetSamples(int samplesRequested)
        {
            // Create array of samples
            double[] audio = new double[samplesRequested];
            // Generate audio for samples requested
            for (int i = 0; i < samplesRequested; i++)
            {
                // Increase the sine wave sample number by 1
                _sampleNumber++;
                // If the sample reaches the sample rate, reset sample number
                if (_sampleNumber > SampleRate)
                {
                    _sampleNumber = 1;
                }
                // Generate sine wave sample
                audio[i] = Math.Sin(2 * Math.PI * Frequency * (_sampleNumber / SampleRate)) * _volumeLinear;
            }
            // Return samples
            return audio;
        }
        #endregion
    }
}