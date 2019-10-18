using System;
using System.Collections.Generic;
using Xamarin.Forms;
using AudioCore.Common;
using AudioCore.Output;

namespace AudioCore.Demo
{
    public partial class EchoPage : DemoPage
    {
        #region Private Fields
        /// <summary>
        /// The test tone input.
        /// </summary>
        private PlatformInput _input;

        /// <summary>
        /// The platform default audio output.
        /// </summary>
        private PlatformOutput _output;

        /// <summary>
        /// The current playback status, <c>true</c> if currently playing, otherwise <c>false</c>.
        /// </summary>
        private bool _playing;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Demo.TestTonePage"/> class.
        /// </summary>
        public EchoPage()
        {
            InitializeComponent();
            // Populate the output device list and select the default device
            List<AudioDevice> outputDevices = PlatformOutput.GetDevices();
            outputPicker.ItemsSource = outputDevices;
            outputPicker.ItemDisplayBinding = new Binding("Name");
            outputPicker.SelectedItem = outputDevices.Find(x => x.Default == true);
            // Populate the input device list and select the default device
            List<AudioDevice> inputDevices = PlatformInput.GetDevices();
            inputPicker.ItemsSource = inputDevices;
            inputPicker.ItemDisplayBinding = new Binding("Name");
            inputPicker.SelectedItem = inputDevices.Find(x => x.Default == true);
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
            if (_input != null)
            {
                _input.Dispose();
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
                // Create the platform audio output
                _output = new PlatformOutput(((AudioDevice)outputPicker.SelectedItem).ID);
                // Create the platform audio input
                _input = new PlatformInput(((AudioDevice)inputPicker.SelectedItem).ID)
                {
                    BufferSize = _output.BufferSize
                };
                // Add the input to the output
                _output.AddInput(_input.Input);
                // Start playback
                _output.Start();
                _input.Start();
                // Set to currently playing, changing the button text
                _playing = true;
                playbackButton.Text = "Stop";
                // Disable the device pickers
                outputPicker.IsEnabled = false;
                inputPicker.IsEnabled = false;
            }
            catch (Exception e) // If an error occurs
            {
                // Release the audio input and output, disposing it first if required
                if (_output != null)
                {
                    _output.Dispose();
                    _output = null;
                }
                if (_input != null)
                {
                    _input.Dispose();
                    _input = null;
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
            _input.Stop();
            _output.Stop();
            // Dispose the output and input
            _output.Dispose();
            _input.Dispose();
            // Release the output and input
            _output = null;
            _input = null;
            // Set to stopped, changing the button text
            _playing = false;
            playbackButton.Text = "Play";
            // Eable the device pickers
            outputPicker.IsEnabled = true;
            inputPicker.IsEnabled = true;
        }
        #endregion
    }
}