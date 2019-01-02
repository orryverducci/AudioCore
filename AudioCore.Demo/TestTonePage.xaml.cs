using System;
using Xamarin.Forms;
using AudioCore.Input;

namespace AudioCore.Demo
{
    public partial class TestTonePage : ContentPage
    {
        #region Private Fields
        /// <summary>
        /// The test tone input.
        /// </summary>
        private TestToneInput _testToneInput;
        /// <summary>
        /// The platform default audio output.
        /// </summary>
        private PlatformOutput _output;
        /// <summary>
        /// The current playback status, <c>true</c> if currently playing, otherwise <c>false</c>.
        /// </summary>
        private bool _playing = false;
        #endregion

        #region Constructor and Page Lifecycle Events
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.TestTonePage"/> class.
        /// </summary>
        public TestTonePage()
        {
            InitializeComponent();
            // Set the event handler for the page disappearing event
            Disappearing += PageDisappearing;
        }

        /// <summary>
        /// Event handler to dispose the platform audio output when the page disappears.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        void PageDisappearing(object sender, EventArgs e)
        {
            if (_output != null)
            {
                _output.Dispose();
            }
        }
        #endregion

        #region UI Methods
        /// <summary>
        /// Handles the play/stop button being clicked or tapped.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void PlaybackButtonClicked(object sender, EventArgs e)
        {
            // Start playback if not currently playing, otherwise stop it
            if (!_playing)
            {
                StartPlayback();
            }
            else
            {
                StopPlayback();
            }
        }
        #endregion

        #region Playback Methods
        /// <summary>
        /// Starts the playback of the test tone input.
        /// </summary>
        private void StartPlayback()
        {
            // Try to start playback, catching any errors
            try
            {
                // Create the platform default audio output
                _output = new PlatformOutput();
                // Convert the linear volume (between 0 and 1) to dBFS
                int dbfsVolume = (int)(Math.Log10(volumeSlider.Value) * 20);
                // Create the test tone input using the specified frequency and volume, using the output sample rate and channel count
                _testToneInput = new TestToneInput(_output.Channels, _output.SampleRate)
                {
                    Frequency = (int)frequencySlider.Value,
                    Volume = dbfsVolume
                };
                // Add the test tone input to the output
                _output.AddInput(_testToneInput);
                // Start playback
                _output.Start();
                // Set to currently playing, changing the button text and disabling the UI sliders
                _playing = true;
                playbackButton.Text = "Stop";
                frequencySlider.IsEnabled = false;
                volumeSlider.IsEnabled = false;
            }
            catch (Exception e) // If an error occurs
            {
                // Release test tone input and audio output, disposing it first if required
                _testToneInput = null;
                if (_output != null)
                {
                    _output.Dispose();
                    _output = null;
                }
                // Display error message
                DisplayAlert("Unable to start playback", e.Message, "OK");
            }
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        private void StopPlayback()
        {
            // Stop playback
            _output.Stop();
            // Dispose the output
            _output.Dispose();
            // Release the output and test tone input
            _output = null;
            _testToneInput = null;
            // Set to stopped, changing the button text and enabling the UI sliders
            _playing = false;
            playbackButton.Text = "Play";
            frequencySlider.IsEnabled = true;
            volumeSlider.IsEnabled = true;
        }
        #endregion
    }
}