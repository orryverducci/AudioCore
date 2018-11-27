using System;
using AudioToolbox;
using AudioUnit;
using AudioCore.Common;
using AudioCore.Converters;
using AudioCore.Output;

namespace AudioCore.Mac.Output
{
    public class CoreAudioOutput : AudioOutput, IDisposable
    {
        #region Private Fields
        AudioUnit.AudioUnit audioUnit;
        #endregion

        #region Constructor and Dispose
        public CoreAudioOutput(int channels, int sampleRate, int bitDepth)
        {
            // Set the audio format properties
            SampleRate = sampleRate;
            Channels = channels;
            BitDepth = bitDepth;
            // Get the default output audio component
            AudioComponent audioOutputComponent = AudioComponent.FindComponent(AudioTypeOutput.Default);
            // Check an audio component was returned
            if (audioOutputComponent == null)
            {
                throw new Exception("Unable to setup audio component");
            }
            // Create the output audio unit
            audioUnit = new AudioUnit.AudioUnit(audioOutputComponent);
            // Set stream format
            AudioStreamBasicDescription streamFormat = new AudioStreamBasicDescription
            {
                SampleRate = SampleRate,
                Format = AudioFormatType.LinearPCM,
                FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked,
                BytesPerPacket = (BitDepth / 8) * Channels,
                FramesPerPacket = 1,
                BytesPerFrame = (BitDepth / 8) * Channels,
                ChannelsPerFrame = Channels,
                BitsPerChannel = BitDepth
            };
            audioUnit.SetFormat(streamFormat, AudioUnitScopeType.Input, 0);
            // Set render callback, and check there's no error
            if (audioUnit.SetRenderCallback(RenderAudio, AudioUnitScopeType.Input, 0) != AudioUnitStatus.NoError)
            {
                throw new Exception("Unable to setup audio output render callback");
            }
            // Initialise audio unit
            audioUnit.Initialize();
        }

        public void Dispose()
        {
            audioUnit.Dispose();
        }
        #endregion

        /// <summary>
        /// Start playback from the audio output
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
        /// Stop playback from the audio output
        /// </summary>
        public override void Stop()
        {
            if (PlaybackState != PlaybackState.STOPPED)
            {
                audioUnit.Stop();
                PlaybackState = PlaybackState.STOPPED;
            }
        }

        private AudioUnitStatus RenderAudio(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint framesRequired, AudioBuffers buffers)
        {
            // Get frames to be output
            double[] frames = GetInputFrames((int)framesRequired).Result;
            // Convert frames to 16 bit integer
            short[] convertedFrames = BitDepthConverter.To16Bit(frames);
            // Output the audio
            unsafe
            {
                // Get pointer to the output audio buffer
                short* outputPointer = (short*)buffers[0].Data.ToPointer();
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