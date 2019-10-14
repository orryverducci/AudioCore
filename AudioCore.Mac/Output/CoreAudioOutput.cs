using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioToolbox;
using AudioUnit;
using AudioCore.Common;
using AudioCore.Converters;
using AudioCore.Mac.Common;
using AudioCore.Output;

namespace AudioCore.Mac.Output
{
    /// <summary>
    /// Provides audio output on macOS via the Core Audio API.
    /// </summary>
    public class CoreAudioOutput : HardwareAudioOutput, IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The output audio unit.
        /// </summary>
        private AudioUnit.AudioUnit _audioUnit;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device and settings.
        /// </summary>
        public CoreAudioOutput() : this(-1, -1, -1, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device, sample rate and buffer size.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        public CoreAudioOutput(int channels) : this(-1, channels, -1, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device and buffer size.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioOutput(int channels, int sampleRate) : this(-1, channels, sampleRate, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default buffer size.
        /// </summary>
        /// <param name="deviceID">The ID of the audio output device to be used.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioOutput(long deviceID, int channels, int sampleRate) : this(deviceID, channels, sampleRate, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class.
        /// </summary>
        /// <param name="deviceID">The ID of the audio output device to be used.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        /// <param name="bufferSize">The buffer size to be requested in frames.</param>
        public CoreAudioOutput(long deviceID, int channels, int sampleRate, int bufferSize)
        {
            // Get the default output audio component
            AudioComponent audioOutputComponent = AudioComponent.FindComponent(AudioTypeOutput.HAL);
            // Check an audio component was returned
            if (audioOutputComponent == null)
            {
                throw new Exception("Unable to setup audio component.");
            }
            // Create the output audio unit
            _audioUnit = new AudioUnit.AudioUnit(audioOutputComponent);
            // Set the output device if not set to use the system default
            if (deviceID != -1)
            {
                _audioUnit.SetCurrentDevice((uint)deviceID, AudioUnitScopeType.Global, 0);
            }
            // Get the output format for use if any of the arguments are set to the -1 default value
            AudioStreamBasicDescription outputFormat = _audioUnit.GetAudioFormat(AudioUnitScopeType.Output);
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
                Channels = outputFormat.ChannelsPerFrame;
            }
            Format = new AudioCore.Common.AudioFormat(32, SampleType.FloatingPoint);
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
            _audioUnit.SetFormat(streamFormat, AudioUnitScopeType.Input, 0);
            // Set the output buffer size if not set to use the system default
            if (bufferSize != -1)
            {
                _audioUnit.SetBufferFrames((uint)bufferSize, AudioUnitScopeType.Output);
            }
            // Set the output buffer size property
            BufferSize = (int)_audioUnit.GetBufferFrames(AudioUnitScopeType.Output);
            // Set render callback, and check there's no error
            if (_audioUnit.SetRenderCallback(RenderAudio, AudioUnitScopeType.Input, 0) != AudioUnitStatus.NoError)
            {
                throw new Exception("Unable to setup audio output render callback.");
            }
            // Initialise audio unit
            _audioUnit.Initialize();
            // Set the latency
            Latency = (int)Math.Round((1000f / SampleRate) * (_audioUnit.GetLatency() + _audioUnit.GetDeviceLatency(AudioUnitScopeType.Output) + _audioUnit.GetSafetyOffset(AudioUnitScopeType.Output) + BufferSize));
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
            _audioUnit.Dispose();
            PlaybackState = PlaybackState.STOPPED;
        }
        #endregion

        #region Playback Methods
        /// <summary>
        /// Start playback from the audio output.
        /// </summary>
        public override void Start()
        {
            if (PlaybackState != PlaybackState.PLAYING)
            {
                _audioUnit.Start();
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
                _audioUnit.Stop();
                PlaybackState = PlaybackState.STOPPED;
            }
        }
        #endregion

        #region Static Device Methods
        /// <summary>
        /// Gets the available output audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available output devices.</returns>
        public static List<AudioDevice> GetDevices()
        {
            // Create a list of devices
            List<AudioDevice> devices = new List<AudioDevice>();
            // Get the ID's of the devices
            uint[] deviceIDs = AudioUnitExtensions.GetDeviceIDs();
            // Get the default output device
            uint defaultDevice = AudioUnitExtensions.GetDefaultDeviceID(AudioObjectPropertyScope.Output);
            // For each device ID, get details about the device and add it to the list of devices
            foreach (var deviceID in deviceIDs)
            {
                // Get if the device has output streams, skipping over it if it doesn't
                if (AudioUnitExtensions.GetStreamCount(deviceID, AudioObjectPropertyScope.Output) == 0)
                {
                    continue;
                }
                string name = AudioUnitExtensions.GetDeviceName(deviceID);
                // Add the device to the list of devices
                AudioDevice device = new AudioDevice
                {
                    ID = deviceID,
                    Name = name,
                    Default = (deviceID == defaultDevice)
                };
                devices.Add(device);
            }
            // Return the list of devices
            return devices;
        }

        /// <summary>
        /// Gets the minimum and maximum buffer sizes allowed by an audio device.
        /// </summary>
        /// <param name="deviceID">The id of the audio device to get the range of buffer sizes for.</param>
        /// <returns>A tuple contining the minimum and maximum buffer sizes allowed.</returns>
        public static (int minimum, int maximum) GetBufferRange(uint deviceID)
        {
            (double minimum, double maximum) range = AudioUnitExtensions.GetBufferRange(deviceID);
            return ((int)range.minimum, (int)range.maximum);
        }
        #endregion

        #region Render Callback
        /// <summary>
        /// Callback which provides the requested audio samples to the output audio unit.
        /// </summary>
        /// <returns>The status of the render callback.</returns>
        /// <param name="actionFlags">The configuration flags for the audio unit.</param>
        /// <param name="timeStamp">The audio time stamp.</param>
        /// <param name="busNumber">The bus number.</param>
        /// <param name="framesRequired">The number of audio frames required.</param>
        /// <param name="buffers">The output audio buffer.</param>
        private AudioUnitStatus RenderAudio(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint framesRequired, AudioBuffers buffers)
        {
            // Get frames to be output
            float[] frames = GetInputFrames((int)framesRequired);
            // Convert frames to bytes
            byte[] convertedFrames = BitDepthConverter.ToFloat(frames);
            // Output the audio
            Marshal.Copy(convertedFrames, 0, buffers[0].Data, convertedFrames.Length);
            // Return that there was no error
            return AudioUnitStatus.NoError;
        }
        #endregion
    }
}