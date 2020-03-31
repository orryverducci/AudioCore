using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AudioCore.Common;
using AudioCore.Input;
using AudioCore.Output;

namespace AudioCore.Demo
{
    public partial class NoisePage : DemoPage
    {
        #region Private Fields
        /// <summary>
        /// The noise input.
        /// </summary>
        private NoiseInput _noiseInput;

        /// <summary>
        /// The platform default audio output.
        /// </summary>
        private PlatformOutput _output;

        /// <summary>
        /// The current playback status, <c>true</c> if currently playing, otherwise <c>false</c>.
        /// </summary>
        private bool _playing;

        /// <summary>
        /// The type of noise.
        /// </summary>
        private NoiseInput.NoiseType _noiseType = NoiseInput.NoiseType.White;

        /// <summary>
        /// The test tone volume.
        /// </summary>
        private int _volume;
        #endregion

        #region Constructor and Page Lifecycle Events
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.NoisePage"/> class.
        /// </summary>
        public NoisePage()
        {
            InitializeComponent();
            // Populate the output device list and select the default device
            List<AudioDevice> devices = PlatformOutput.GetDevices();
            outputPicker.ItemsSource = devices;
            outputPicker.ItemDisplayBinding = new Binding("Name");
            outputPicker.SelectedItem = devices.Find(x => x.Default == true);
        }

        /// <summary>
        /// Disposes all resources held by the page, stopping audio playback.
        /// </summary>
        public override void Dispose()
        {
            if (_output != null)
            {
                _output.Dispose();
            }
        }
        #endregion

        #region UI Methods
        /// <summary>
        /// Handles the noise type being changed.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void NoiseChanged(object sender, EventArgs e)
        {
            switch (typePicker.Items[typePicker.SelectedIndex])
            {
                default:
                    _noiseType = NoiseInput.NoiseType.White;
                    break;
                case "Pink Noise":
                    _noiseType = NoiseInput.NoiseType.Pink;
                    break;
                case "Brown Noise":
                    _noiseType = NoiseInput.NoiseType.Brown;
                    break;
            }
            if (_playing)
            {
                _noiseInput.Type = _noiseType;
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
                _noiseInput.Volume = _volume;
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
        /// Starts the playback of the noise input.
        /// </summary>
        private void StartPlayback()
        {
            // Try to start playback, catching any errors
            try
            {
                // Create the platform audio output
                _output = new PlatformOutput(((AudioDevice)outputPicker.SelectedItem).ID);
                // Create the noise input using the specified volume, using the output sample rate and channel count
                _noiseInput = new NoiseInput(_output.Channels, _output.SampleRate)
                {
                    Type = _noiseType,
                    Volume = _volume
                };
                // Add the test tone input to the output
                _output.AddInput(_noiseInput);
                // Start playback
                _output.Start();
                // Set to currently playing, changing the button text
                _playing = true;
                playbackButton.Text = "Stop";
                // Disable the device picker
                outputPicker.IsEnabled = false;
            }
            catch (Exception e) // If an error occurs
            {
                // Release noise input and audio output, disposing it first if required
                _noiseInput = null;
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
            // Release the output and noise input
            _output = null;
            _noiseInput = null;
            // Set to stopped, changing the button text
            _playing = false;
            playbackButton.Text = "Play";
            // Eable the device picker
            outputPicker.IsEnabled = true;
        }
        #endregion
    }
}