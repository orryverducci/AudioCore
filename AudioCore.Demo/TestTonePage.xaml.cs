using System;
using Xamarin.Forms;
using AudioCore.Common;
using AudioCore.Input;
using AudioCore.Mac.Output;

namespace AudioCore.Demo
{
    public partial class TestTonePage : ContentPage
    {
        private TestToneInput testToneInput = new TestToneInput(2, 44100);
        private AudioToolboxOutput output = new AudioToolboxOutput(2, 44100, 16);

        public TestTonePage()
        {
            InitializeComponent();
            output.AddInput(testToneInput);
        }

        private void PlaybackButtonClicked(object sender, EventArgs e)
        {
            switch(output.PlaybackState)
            {
                case PlaybackState.STOPPED:
                    output.Start();
                    playbackButton.Text = "Stop";
                    break;
                case PlaybackState.PLAYING:
                    output.Stop();
                    playbackButton.Text = "Play";
                    break;
            }
        }
    }
}