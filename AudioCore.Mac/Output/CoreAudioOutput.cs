using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioToolbox;
using AudioUnit;
using CoreFoundation;
using ObjCRuntime;
using AudioCore.Common;
using AudioCore.Converters;
using AudioCore.Mac.Common;
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
        private AudioUnit.AudioUnit audioUnit;
        #endregion

        #region Constructor and Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device and settings.
        /// </summary>
        public CoreAudioOutput() : this(-1, -1, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device and sample rate.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        public CoreAudioOutput(int channels) : this(-1, channels, -1) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class with the system default output device.
        /// </summary>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioOutput(int channels, int sampleRate) : this(-1, channels, sampleRate) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AudioCore.Mac.Output.CoreAudioOutput"/> class.
        /// </summary>
        /// <param name="deviceID">The ID of the audio output device to be used.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <param name="sampleRate">The audio sample rate in Hertz.</param>
        public CoreAudioOutput(long deviceID, int channels, int sampleRate)
        {
            // Get the default output audio component
            AudioComponent audioOutputComponent = AudioComponent.FindComponent(AudioTypeOutput.HAL);
            // Check an audio component was returned
            if (audioOutputComponent == null)
            {
                throw new Exception("Unable to setup audio component.");
            }
            // Create the output audio unit
            audioUnit = new AudioUnit.AudioUnit(audioOutputComponent);
            // Set the output device if not set to use the system default
            if (deviceID != -1)
            {
                audioUnit.SetCurrentDevice((uint)deviceID, AudioUnitScopeType.Global, 0);
            }
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
            audioUnit.SetFormat(streamFormat, AudioUnitScopeType.Input, 0);
            // Set render callback, and check there's no error
            if (audioUnit.SetRenderCallback(RenderAudio, AudioUnitScopeType.Input, 0) != AudioUnitStatus.NoError)
            {
                throw new Exception("Unable to setup audio output render callback.");
            }
            // Initialise audio unit
            audioUnit.Initialize();
            // Set the output buffer size
            BufferSize = (int)audioUnit.GetBufferFrames();
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

        #region Playback Methods
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
        #endregion

        #region Static Device Methods
        /// <summary>
        /// Gets the available output audio devices.
        /// </summary>
        /// <returns>A list of <see cref="T:AudioCore.Common.AudioDevice"/> representing the available output devices.</returns>
        public static List<AudioDevice> GetDevices()
        {
            // Initialise a uint to hold property sizes and an int to hold error codes from retrieving properties
            uint propertySize;
            int error;
            // Create a list of devices
            List<AudioDevice> devices = new List<AudioDevice>();
            // Setup qualifier data to be used when retrieving devices
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get the size of the devices property, throwing an exception if there was an issue
            AudioObjectPropertyAddress devicePropertyAddress = new AudioObjectPropertyAddress(AudioObjectPropertySelector.Devices, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            error = AudioObjectGetPropertyDataSize(1, ref devicePropertyAddress, ref inQualifierDataSize, ref inQualifierData, out propertySize);
            if (error != 0)
            {
                throw new Exception($"Unable to retrieve the number of devices - OSStatus Error: {error}");
            }
            // Get the number of devices by dividing the devices property by the native size of uint
            uint numberOfDevices = propertySize / (uint)Marshal.SizeOf(typeof(uint));
            // Get the ID of the devices, throwing an exception if there was an issue
            uint[] deviceIDs = new uint[numberOfDevices];
            error = AudioObjectGetPropertyData(1, ref devicePropertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, deviceIDs);
            if (error != 0)
            {
                throw new Exception($"Unable to retrieve devices - OSStatus Error: {error}");
            }
            // Get the default output device, throwing an exception if there was an issue
            uint defaultDevice;
            AudioObjectPropertyAddress defaultPropertyAddress = new AudioObjectPropertyAddress(AudioObjectPropertySelector.DefaultOutputDevice, AudioObjectPropertyScope.Output, AudioObjectPropertyElement.Master);
            propertySize = (uint)Marshal.SizeOf(typeof(uint));
            error = AudioObjectGetPropertyData(1, ref defaultPropertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out defaultDevice);
            // For each device ID, get details about the device and add it to the list of devices
            foreach (var deviceID in deviceIDs)
            {
                // Get if the device has output streams, skipping over it if it doesn't and throwing an exception if there was an issue
                AudioObjectPropertyAddress outputStreamsPropertyAddress = new AudioObjectPropertyAddress(0x73746d23 /* stm# */, (uint)AudioObjectPropertyScope.Output, (uint)AudioObjectPropertyElement.Master);
                error = AudioObjectGetPropertyDataSize(deviceID, ref outputStreamsPropertyAddress, ref inQualifierDataSize, ref inQualifierData, out propertySize);
                if (error != 0)
                {
                    throw new Exception($"Unable to determine if device {deviceID} has outputs - OSStatus Error: {error}");
                }
                if (propertySize == 0)
                {
                    continue;
                }
                // Get a pointer to the device name, throwing an exception if there was an issue
                AudioObjectPropertyAddress namePropertyAddress = new AudioObjectPropertyAddress(0x6c6e616d /* lnam */, (uint)AudioObjectPropertyScope.Global, (uint)AudioObjectPropertyElement.Master);
                IntPtr namePtr = IntPtr.Zero;
                unsafe
                {
                    propertySize = (uint)sizeof(IntPtr);
                }
                error = AudioObjectGetPropertyData(deviceID, ref namePropertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out namePtr);
                if (error != 0)
                {
                    throw new Exception($"Unable to retrieve name for device {deviceID} - OSStatus Error: {error}");
                }
                // Read the device name string from the pointer
                CFString name = new CFString(namePtr);
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

        #region Platform Invokes
        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>0 if there was no error, an OSStatus code if there was.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize,  uint[] outData);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>0 if there was no error, an OSStatus code if there was.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, out uint outData);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>0 if there was no error, an OSStatus code if there was.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, out IntPtr outData);

        /// <summary>
        /// Gets the size of the data from an audio property.
        /// </summary>
        /// <returns>0 if there was no error, an OSStatus code if there was.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="outDataSize">The size of the data being out.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int AudioObjectGetPropertyDataSize(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, out uint outDataSize);

        /// <summary>
        /// An audio object property address.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct AudioObjectPropertyAddress
        {
            public uint Selector;
            public uint Scope;
            public uint Element;

            public AudioObjectPropertyAddress(uint selector, uint scope, uint element)
            {
                Selector = selector;
                Scope = scope;
                Element = element;
            }

            public AudioObjectPropertyAddress(AudioObjectPropertySelector selector, AudioObjectPropertyScope scope, AudioObjectPropertyElement element)
            {
                Selector = (uint)selector;
                Scope = (uint)scope;
                Element = (uint)element;
            }
        }
        #endregion

        #region Render Callback
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
            // Convert frames to bytes
            byte[] convertedFrames = BitDepthConverter.ToFloat(frames);
            // Output the audio
            unsafe
            {
                // Get pointer to the output audio buffer
                byte* outputPointer = (byte*)buffers[0].Data.ToPointer();
                // Add each frame to the output audio buffers
                for (int i = 0; i < convertedFrames.Length; i++)
                {
                    *outputPointer++ = convertedFrames[i];
                }
            }
            // Return that there was no error
            return AudioUnitStatus.NoError;
        }
        #endregion
    }
}