using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AudioCore.Common;
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
        private bool _playing;

        /// <summary>
        /// The type of test tone.
        /// </summary>
        private TestToneInput.ToneType _toneType = TestToneInput.ToneType.SineWave;

        /// <summary>
        /// The test tone frequency.
        /// </summary>
        private int _frequency = 1000;

        /// <summary>
        /// The test tone volume.
        /// </summary>
        private int _volume;
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
            // Populate the output device list and select the default device
            List<AudioDevice> devices = PlatformOutput.GetDevices();
            outputPicker.ItemsSource = devices;
            outputPicker.ItemDisplayBinding = new Binding("Name");
            outputPicker.SelectedItem = devices.Find(x => x.Default == true);
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
        /// Handles the test tone type being changed.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void ToneChanged(object sender, EventArgs e)
        {
            switch (typePicker.Items[typePicker.SelectedIndex])
            {
                default:
                    _toneType = TestToneInput.ToneType.SineWave;
                    break;
                case "Square Wave":
                    _toneType = TestToneInput.ToneType.SquareWave;
                    break;
                case "Sawtooth Wave":
                    _toneType = TestToneInput.ToneType.SawtoothWave;
                    break;
                case "Triangle Wave":
                    _toneType = TestToneInput.ToneType.TriangleWave;
                    break;
            }
            if (_playing)
            {
                _testToneInput.Type = _toneType;
            }
        }

        /// <summary>
        /// Handles the frequency slider value being changed.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void FrequencyChanged(object sender, EventArgs e)
        {
            _frequency = (int)frequencySlider.Value;
            if (_playing)
            {
                _testToneInput.Frequency = _frequency;
            }
        }

        /// <summary>
        /// Handles the volume slider value being changed.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void VolumeChanged(object sender, EventArgs e)
        {
            _volume = (int)(Math.Log10(volumeSlider.Value) * 20);
            if (_playing)
            {
                _testToneInput.Volume = _volume;
            }
        }

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
                // Create the platform audio output
                _output = new PlatformOutput(((AudioDevice)outputPicker.SelectedItem).ID);
                // Create the test tone input using the specified frequency and volume, using the output sample rate and channel count
                _testToneInput = new TestToneInput(_output.Channels, _output.SampleRate)
                {
                    Frequency = _frequency,
                    Type = _toneType,
                    Volume = _volume
                };
                // Add the test tone input to the output
                _output.AddInput(_testToneInput);
                // Start playback
                _output.Start();
                // Set to currently playing, changing the button text
                _playing = true;
                playbackButton.Text = "Stop";
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
            // Set to stopped, changing the button text
            _playing = false;
            playbackButton.Text = "Play";
        }
        #endregion
    }
}