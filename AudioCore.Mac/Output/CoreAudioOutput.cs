using System;
using AudioToolbox;
using AudioUnit;
using AudioCore.Common;
using AudioCore.Converters;
using AudioCore.Output;

namespace AudioCore.Mac.Output
{
    /// <summary>
    /// Provides audio output on macOS via the Core Audio API.
    /// </summary>
    public class CoreAudioOutput : AudioOutput, IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The output audio unit.
        /// </summary>
        AudioUnit.AudioUnit audioUnit;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system sample rate and audio channels.
        /// </summary>
        public CoreAudioOutput() : this(-1, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system sample rate.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        public CoreAudioOutput(int channels) : this(channels, -1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioOutput(int channels, int sampleRate)
        {
            // Get the default output audio component
            AudioComponent audioOutputComponent = AudioComponent.FindComponent(AudioTypeOutput.Default);
            // Check an audio component was returned
            if (audioOutputComponent == null)
            {
                throw new Exception("Unable to setup audio component.");
            }
            // Create the output audio unit
            audioUnit = new AudioUnit.AudioUnit(audioOutputComponent);
            // Get the output format for use if any of the arguments are set to the -1 default value
            AudioStreamBasicDescription outputFormat = audioUnit.GetAudioFormat(AudioUnitScopeType.Output);
            // Set the audio format properties
            if (sampleRate != -1)
            {
                SampleRate = sampleRate;
            }
            else
            {
                SampleRate = (int)outputFormat.SampleRate;
            }
            if (channels != -1)
            {
                Channels = channels;
            }
            else
            {
                Channels = (int)outputFormat.ChannelsPerFrame;
            }
            BitDepth = 32;
            // Set stream format
            AudioStreamBasicDescription streamFormat = new AudioStreamBasicDescription
            {
                SampleRate = SampleRate,
                Format = AudioFormatType.LinearPCM,
                FormatFlags = AudioFormatFlags.IsFloat | AudioFormatFlags.IsPacked,
                BytesPerPacket = 4 * Channels,
                FramesPerPacket = 1,
                BytesPerFrame = 4 * Channels,
                ChannelsPerFrame = Channels,
                BitsPerChannel = 32
            };
            audioUnit.SetFormat(streamFormat, AudioUnitScopeType.Input, 0);
            // Set render callback, and check there's no error
            if (audioUnit.SetRenderCallback(RenderAudio, AudioUnitScopeType.Input, 0) != AudioUnitStatus.NoError)
            {
                throw new Exception("Unable to setup audio output render callback.");
            }
            // Initialise audio unit
            audioUnit.Initialize();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> so the garbage collector can reclaim the memory that
        /// the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> was occupying.</remarks>
        public void Dispose()
        {
            audioUnit.Dispose();
        }
        #endregion

        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public override void Start()
        {
            if (PlaybackState != PlaybackState.PLAYING)
            {
                audioUnit.Start();
                PlaybackState = PlaybackState.PLAYING;
            }
        }

        /// <summary>
        /// Stop playback from the audio output.
        /// </summary>
        public override void Stop()
        {
            if (PlaybackState != PlaybackState.STOPPED)
            {
                audioUnit.Stop();
                PlaybackState = PlaybackState.STOPPED;
            }
        }

        /// <summary>
        /// Callback which provides the requested audio samples to the output audio unit.
        /// </summary>
        /// <returns>The audio.</returns>
        /// <param name="actionFlags">The configuration flags for the audio unit.</param>
        /// <param name="timeStamp">The audio time stamp.</param>
        /// <param name="busNumber">The bus number.</param>
        /// <param name="framesRequired">The number of audio frames required.</param>
        /// <param name="buffers">The output audio buffer.</param>
        private AudioUnitStatus RenderAudio(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint framesRequired, AudioBuffers buffers)
        {
            // Get frames to be output
            double[] frames = GetInputFrames((int)framesRequired);
            // Convert frames to floats
            float[] convertedFrames = BitDepthConverter.ToFloat(frames);
            // Output the audio
            unsafe
            {
                // Get pointer to the output audio buffer
                float* outputPointer = (float*)buffers[0].Data.ToPointer();
                // Add each frame to the output audio buffers
                for (int i = 0; i < convertedFrames.Length; i++)
                {
                    *outputPointer++ = convertedFrames[i];
                }
            }
            // Return that there was no error
            return AudioUnitStatus.NoError;
        }
    }
}