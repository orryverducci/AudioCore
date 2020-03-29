using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioToolbox;
using AudioUnit;
using AudioCore.Common;
using AudioCore.Converters;
using AudioCore.Mac.Common;
using AudioCore.Input;

namespace AudioCore.Mac.Input
{
    /// <summary>
    /// Provides input from an audio device on macOS via the Core Audio API.
    /// </summary>
    public class CoreAudioInput : HardwareAudioInput, IDisposable
    {
        #region Private Fields
        /// <summary>
        /// The input audio unit.
        /// </summary>
        private AudioUnit.AudioUnit _audioUnit;

        /// <summary>
        /// An audio buffer to be filled by the input.
        /// </summary>
        AudioBuffers _buffer = new AudioBuffers(1);
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> class with the system default input device and settings.
        /// </summary>
        public CoreAudioInput() : this(-1, -1, -1, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> class with the system default input device, sample rate and buffer size.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        public CoreAudioInput(int channels) : this(-1, channels, -1, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> class with the system default input device buffer size.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioInput(int channels, int sampleRate) : this(-1, channels, sampleRate, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> class with the system default buffer size.
        /// </summary>
        /// <param name="deviceID">The ID of the audio input device to be used.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioInput(long deviceID, int channels, int sampleRate) : this(deviceID, channels, sampleRate, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> class.
        /// </summary>
        /// <param name="deviceID">The ID of the audio input device to be used.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        /// <param name="bufferSize">The hardware buffer size to be requested in frames.</param>
        public CoreAudioInput(long deviceID, int channels, int sampleRate, int bufferSize)
        {
            // Get the default output audio component, the CoreAudio API requires an output audio unit for input
            AudioComponent audioOutputComponent = AudioComponent.FindComponent(AudioTypeOutput.HAL);
            // Check an audio component was returned
            if (audioOutputComponent == null)
            {
                throw new Exception("Unable to setup audio component.");
            }
            // Create the output audio unit
            _audioUnit = new AudioUnit.AudioUnit(audioOutputComponent);
            // Enable input and disable output
            _audioUnit.SetEnableIO(true, AudioUnitScopeType.Input, 1);
            _audioUnit.SetEnableIO(false, AudioUnitScopeType.Output);
            // Set the input device if not set to use the system default
            if (deviceID != -1)
            {
                _audioUnit.SetCurrentDevice((uint)deviceID, AudioUnitScopeType.Global, 0);
            }
            // Get the input format for use if any of the arguments are set to the -1 default value
            AudioStreamBasicDescription inputFormat = _audioUnit.GetAudioFormat(AudioUnitScopeType.Input, 1);
            // Set the audio format properties
            if (sampleRate != -1)
            {
                SampleRate = sampleRate;
            }
            else
            {
                SampleRate = (int)inputFormat.SampleRate;
            }
            if (channels != -1)
            {
                Channels = channels;
            }
            else
            {
                Channels = inputFormat.ChannelsPerFrame;
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
            _audioUnit.SetFormat(streamFormat, AudioUnitScopeType.Output, 1);
            // Set the input hardware buffer size if not set to use the system default
            if (bufferSize != -1)
            {
                _audioUnit.SetBufferFrames((uint)bufferSize, AudioUnitScopeType.Input);
            }
            // Set the input hardware buffer size property
            HardwareBufferSize = (int)_audioUnit.GetBufferFrames(AudioUnitScopeType.Input);
            // Set input callback, and check there's no error
            if (_audioUnit.SetInputCallback(RetrieveAudio, AudioUnitScopeType.Global, 1) != AudioUnitStatus.NoError)
            {
                throw new Exception("Unable to setup audio input callback.");
            }
            // Initialise audio unit
            _audioUnit.Initialize();
            // Set the latency
            Latency = (int)Math.Round((1000f / SampleRate) * (_audioUnit.GetLatency() + _audioUnit.GetDeviceLatency(AudioUnitScopeType.Input) + _audioUnit.GetSafetyOffset(AudioUnitScopeType.Input) + HardwareBufferSize));
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:AudioCore.Mac.Input.CoreAudioInput"/> was occupying.</remarks>
        public void Dispose()
        {
            _audioUnit.Dispose();
            PlaybackState = PlaybackState.STOPPED;
        }
        #endregion

        #region Playback Methods
        /// <summary>
        /// Start the audio input.
        /// </summary>
        public override void Start()
        {
            if (PlaybackState == PlaybackState.STOPPED)
            {
                _audioUnit.Start();
                PlaybackState = PlaybackState.BUFFERING;
            }
        }

        /// <summary>
        /// Stop the audio input.
        /// </summary>
        public override void Stop()
        {
            if (PlaybackState != PlaybackState.STOPPED)
            {
                _audioUnit.Stop();
                ResetBuffer();
                PlaybackState = PlaybackState.STOPPED;
            }
        }
        #endregion

        #region Static Device Methods
        /// <summary>
        /// Gets the available input audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available input devices.</returns>
        public static List<AudioDevice> GetDevices()
        {
            // Create a list of devices
            List<AudioDevice> devices = new List<AudioDevice>();
            // Get the ID's of the devices
            uint[] deviceIDs = AudioUnitExtensions.GetDeviceIDs();
            // Get the default input device
            uint defaultDevice = AudioUnitExtensions.GetDefaultDeviceID(AudioObjectPropertyScope.Input);
            // For each device ID, get details about the device and add it to the list of devices
            foreach (var deviceID in deviceIDs)
            {
                // Get if the device has input streams, skipping over it if it doesn't
                if (AudioUnitExtensions.GetStreamCount(deviceID, AudioObjectPropertyScope.Input) == 0)
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
        #endregion

        #region Audio Methods
        /// <summary>
        /// Callback which gets the audio samples from the input device.
        /// </summary>
        /// <returns>The status of the input callback.</returns>
        /// <param name="actionFlags">The configuration flags for the audio unit.</param>
        /// <param name="timeStamp">The audio time stamp.</param>
        /// <param name="busNumber">The bus number.</param>
        /// <param name="framesAvailable">The number of audio frames available.</param>
        /// <param name="audioUnit">The audio unit.</param>
        private AudioUnitStatus RetrieveAudio(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp, uint busNumber, uint framesAvailable, AudioUnit.AudioUnit audioUnit)
        {
            // Get the audio samples
            audioUnit.Render(ref actionFlags, timeStamp, busNumber, framesAvailable, _buffer);
            // Wrap the audio sample data in a span of floats
            Span<float> samples;
            unsafe
            {
                samples = new Span<float>(_buffer[0].Data.ToPointer(), (int)framesAvailable * Channels);
            }
            // Write the samples to the buffer
            Write(samples);
            // Return that there was no error
            return AudioUnitStatus.NoError;
        }
        #endregion
    }
}