﻿using System;
using System.Runtime.InteropServices;
using AudioUnit;
using CoreFoundation;
using ObjCRuntime;

namespace AudioCore.Mac.Common
{
    /// <summary>
    /// Adds extension methods to the <see cref="T:AudioUnit.AudioUnit"/> class.
    /// </summary>
    internal static class AudioUnitExtensions
    {
        #region Constants
        /// <summary>
        /// The audio object ID of the system.
        /// </summary>
        private const int kAudioObjectSystemObject = 1;

        /// <summary>
        /// The property ID for the buffer frame size.
        /// </summary>
        private const int kAudioDevicePropertyBufferFrameSize = 0x6673697a; // This value is equivalent to 'fsiz' in ASCII

        /// <summary>
        /// The property ID for the audio object streams.
        /// </summary>
        private const int kAudioDevicePropertyStreams = 0x73746d23; // This value is equivalent to 'stm#' in ASCII

        /// <summary>
        /// The property ID for the audio object name.
        /// </summary>
        private const int kAudioObjectPropertyName = 0x6c6e616d; // This value is equivalent to 'lnam' in ASCII

        /// <summary>
        /// The property ID for the audio device latency.
        /// </summary>
        private const int kAudioDevicePropertyLatency = 0x6c746e63; // This value is equivalent to 'ltnc' in ASCII

        /// <summary>
        /// The property ID for the audio device safety offset.
        /// </summary>
        private const int kAudioDevicePropertySafetyOffset = 0x73616674; // This value is equivalent to 'saft' in ASCII

        /// <summary>
        /// The ID for power saving being disabled.
        /// </summary>
        private const int kAudioHardwarePowerHintNone = 0;

        /// <summary>
        /// The ID for power saving being enabled.
        /// </summary>
        private const int kAudioHardwarePowerHintFavorSavingPower = 1;

        /// <summary>
        /// The ID for the audio device buffer range.
        /// </summary>
        private const int kAudioDevicePropertyBufferFrameSizeRange = 0x66737a23; // This value is equivalent to 'fsz#' in ASCII
        #endregion

        #region Platform Invokes
        /// <summary>
        /// Gets a property from the audio unit.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inUnit">The handle of the audio unit to get the property from.</param>
        /// <param name="inID">The identifier for the property to be retrieved.</param>
        /// <param name="inScope">The scope of the audio unit property.</param>
        /// <param name="inElement">The audio unit element to retrieve the property from.</param>
        /// <param name="outData">A variable to output the property value to.</param>
        /// <param name="ioDataSize">The size of the variable to output in bytes.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, int inID, AudioUnitScopeType inScope, uint inElement, ref uint outData, ref uint ioDataSize);

        /// <summary>
        /// Sets a property on the audio unit.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if setting the property was successful or not.</returns>
        /// <param name="inUnit">The handle of the audio unit to set the property on.</param>
        /// <param name="inID">The identifier for the property to be set.</param>
        /// <param name="inScope">The scope of the audio unit property.</param>
        /// <param name="inElement">The audio unit element to set the property on.</param>
        /// <param name="inData">The value the property is being set to.</param>
        /// <param name="inDataSize">The size of the property value in bytes.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, int inID, AudioUnitScopeType inScope, uint inElement, ref uint inData, uint inDataSize);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, out uint outData);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, uint[] outData);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, out IntPtr outData);

        /// <summary>
        /// Gets the data from an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="ioDataSize">The size of the data being output.</param>
        /// <param name="outData">The data being output.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectGetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, ref uint ioDataSize, out AudioValueRange outData);

        /// <summary>
        /// Gets the size of the data from an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if getting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to get a property from.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be retrieved.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="outDataSize">The size of the data being out.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectGetPropertyDataSize(uint inObjectID, ref AudioObjectPropertyAddress inAddress, ref uint inQualifierDataSize, ref IntPtr inQualifierData, out uint outDataSize);

        /// <summary>
        /// Sets the data for an audio property.
        /// </summary>
        /// <returns>An <see cref="T:AudioUnit.AudioUnitStatus"/> indicating if setting the property was successful or not.</returns>
        /// <param name="inObjectID">The ID of the audio object to set a property on.</param>
        /// <param name="inAddress">The <see cref="T:AudioCore.Mac.CoreAudioOutput.AudioObjectPropertyAddress"/> representing the property to be set.</param>
        /// <param name="inQualifierDataSize">The size of the input qualifier data.</param>
        /// <param name="inQualifierData">The input qualifier data.</param>
        /// <param name="inDataSize">The size of the data being input.</param>
        /// <param name="inData">The data being input.</param>
        [DllImport(Constants.AudioUnitLibrary)]
        private static extern AudioUnitStatus AudioObjectSetPropertyData(uint inObjectID, ref AudioObjectPropertyAddress inAddress, uint inQualifierDataSize, ref IntPtr inQualifierData, uint inDataSize, ref uint inData);

        /// <summary>
        /// An audio object property address.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct AudioObjectPropertyAddress
        {
            public uint Selector;
            public uint Scope;
            public uint Element;

            public AudioObjectPropertyAddress(uint selector, AudioObjectPropertyScope scope, AudioObjectPropertyElement element)
            {
                Selector = selector;
                Scope = (uint)scope;
                Element = (uint)element;
            }
        }

        /// <summary>
        /// A range of values.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct AudioValueRange
        {
            public double mMinimum;
            public double mMaximum;
        }
        #endregion

        #region AudioUnit Extension Methods
        /// <summary>
        /// Gets the number of frames the audio unit is buffering by.
        /// </summary>
        /// <returns>The number of frames in the buffer.</returns>
        /// <param name="scope">The audio scope to get the number of frames in the buffer from.</param>
        internal static uint GetBufferFrames(this AudioUnit.AudioUnit audioUnit, AudioUnitScopeType scope)
        {
            // The size in bytes of the data to be returned
            uint size = sizeof(uint);
            // The number of frames to be returned, defaulting to 0
            uint bufferFrames = 0;
            // Get the buffer frames property
            AudioUnitStatus status = AudioUnitGetProperty(audioUnit.Handle, kAudioDevicePropertyBufferFrameSize, scope, 0, ref bufferFrames, ref size);
            // If getting the property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve the number of frames in the audio unit buffer - {status.ToString()}");
            }
            // Return the number of frames
            return bufferFrames;
        }

        /// <summary>
        /// Sets the number of frames the audio unit is buffering by.
        /// </summary>
        /// <returns>The number of frames the output audio unit should buffer by.</returns>
        /// <param name="scope">The audio scope on which to set the number of frames the audio unit shoudl buffer by.</param>
        internal static void SetBufferFrames(this AudioUnit.AudioUnit audioUnit, uint bufferSize, AudioUnitScopeType scope)
        {
            // The size in bytes of the data being set
            uint size = sizeof(uint);
            // Set the buffer frames property
            AudioUnitStatus status = AudioUnitSetProperty(audioUnit.Handle, kAudioDevicePropertyBufferFrameSize, scope, 0, ref bufferSize, size);
            // If setting the property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to set the number of frames in the audio unit buffer - {status.ToString()}");
            }
        }

        /// <summary>
        /// Gets the latency of the device in number of frames.
        /// </summary>
        /// <returns>The latency of the device in number of frames.</returns>
        /// <param name="scope">The audio scope to get the device latency from.</param>
        internal static uint GetDeviceLatency(this AudioUnit.AudioUnit audioUnit, AudioUnitScopeType scope)
        {
            // The size in bytes of the data to be returned
            uint size = sizeof(uint);
            // The latency of the device, defaulting to 0
            uint latency = 0;
            // Get the device latency property
            AudioUnitStatus status = AudioUnitGetProperty(audioUnit.Handle, kAudioDevicePropertyLatency, scope, 0, ref latency, ref size);
            // If getting the property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve the latency of the audio device - {status.ToString()}");
            }
            // Return the number of frames
            return latency;
        }

        /// <summary>
        /// Gets the safety offset of the device in number of frames.
        /// </summary>
        /// <returns>The safety offset of the device in number of frames.</returns>
        /// <param name="scope">The audio scope to get the safety offset from.</param>
        internal static uint GetSafetyOffset(this AudioUnit.AudioUnit audioUnit, AudioUnitScopeType scope)
        {
            // The size in bytes of the data to be returned
            uint size = sizeof(uint);
            // The safety offset, defaulting to 0
            uint safetyOffset = 0;
            // Get the safety offset property
            AudioUnitStatus status = AudioUnitGetProperty(audioUnit.Handle, kAudioDevicePropertySafetyOffset, scope, 0, ref safetyOffset, ref size);
            // If getting the property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve the safety offset - {status.ToString()}");
            }
            // Return the number of frames
            return safetyOffset;
        }
        #endregion

        #region Audio Object Property Methods
        /// <summary>
        /// Gets the identifiers of the available audio devices.
        /// </summary>
        /// <returns>An array of device identifiers.</returns>
        internal static uint[] GetDeviceIDs()
        {
            // Initialise an AudioUnitStatus
            AudioUnitStatus status;
            // Setup qualifier data to be used when retrieving devices
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get the size of the devices property
            AudioObjectPropertyAddress devicePropertyAddress = new AudioObjectPropertyAddress((uint)AudioObjectPropertySelector.Devices, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            status = AudioObjectGetPropertyDataSize(kAudioObjectSystemObject, ref devicePropertyAddress, ref inQualifierDataSize, ref inQualifierData, out uint propertySize);
            // If getting the size of the devices property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve the number of devices - {status.ToString()}");
            }
            // Get the number of devices by dividing the size of the devices property by the native size of uint
            uint numberOfDevices = propertySize / (uint)Marshal.SizeOf(typeof(uint));
            // Get the ID's of the devices
            uint[] deviceIDs = new uint[numberOfDevices];
            status = AudioObjectGetPropertyData(kAudioObjectSystemObject, ref devicePropertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, deviceIDs);
            // If getting the devices was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve devices - {status.ToString()}");
            }
            // Return the ID's of the devices
            return deviceIDs;
        }

        /// <summary>
        /// Gets the default audio device identifier.
        /// </summary>
        /// <returns>The default device identifier.</returns>
        /// <param name="scope">The audio scope to get the default device for.</param>
        internal static uint GetDefaultDeviceID(AudioObjectPropertyScope scope)
        {
            // Set the size of the data to be returned in bytes
            uint propertySize = (uint)Marshal.SizeOf(typeof(uint));
            // Setup qualifier data to be used when retrieving the default device
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Set the property address depending on the scope
            AudioObjectPropertyAddress propertyAddress;
            switch (scope)
            {
                case AudioObjectPropertyScope.Input:
                    propertyAddress = new AudioObjectPropertyAddress((uint)AudioObjectPropertySelector.DefaultInputDevice, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
                    break;
                case AudioObjectPropertyScope.Output:
                    propertyAddress = new AudioObjectPropertyAddress((uint)AudioObjectPropertySelector.DefaultOutputDevice, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), "The audio scope must be either the input or output.");
            }
            // Get the ID of the default device
            AudioUnitStatus status = AudioObjectGetPropertyData(kAudioObjectSystemObject, ref propertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out uint deviceID);
            // If getting the property data was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve default device - {status.ToString()}");
            }
            // Return the device ID
            return deviceID;
        }

        /// <summary>
        /// Gets the number of streams provided by an audio device.
        /// </summary>
        /// <returns>The number of streams.</returns>
        /// <param name="deviceID">The identifier of the device to get the streams from.</param>
        /// <param name="scope">The audio scope to get the streams from.</param>
        internal static uint GetStreamCount(uint deviceID, AudioObjectPropertyScope scope)
        {
            // Setup qualifier data to be used when retrieving the number of streams
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get the ID of the default device
            AudioObjectPropertyAddress propertyAddress = new AudioObjectPropertyAddress(kAudioDevicePropertyStreams, scope, AudioObjectPropertyElement.Master);
            AudioUnitStatus status = AudioObjectGetPropertyDataSize(deviceID, ref propertyAddress, ref inQualifierDataSize, ref inQualifierData, out uint propertySize);
            // If getting the property data was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to determine if device {deviceID} has outputs - {status.ToString()}");
            }
            // Return the property size
            return propertySize;
        }

        /// <summary>
        /// Gets the name of an audio device.
        /// </summary>
        /// <returns>The name of the device.</returns>
        /// <param name="deviceID">The identifier of the device to get the name of.</param>
        internal static string GetDeviceName(uint deviceID)
        {
            // Set the size of the data to be returned in bytes
            uint propertySize;
            unsafe
            {
                propertySize = (uint)sizeof(IntPtr);
            }
            // Setup qualifier data to be used when retrieving the device name
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get a pointer to the device name
            IntPtr namePtr = IntPtr.Zero;
            AudioObjectPropertyAddress propertyAddress = new AudioObjectPropertyAddress(kAudioObjectPropertyName, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            AudioUnitStatus status = AudioObjectGetPropertyData(deviceID, ref propertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out namePtr);
            // If getting the property data was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve name for device {deviceID} - {status.ToString()}");
            }
            // Read the device name string from the pointer and return it
            return new CFString(namePtr);
        }

        /// <summary>
        /// Gets if power saving has been enabled.
        /// </summary>
        /// <returns><c>true</c> if power saving enabled is enabled, <c>false</c> otherwise.</returns>
        internal static bool GetPowerSavingEnabled()
        {
            // Set the size of the data to be returned in bytes
            uint propertySize = (uint)Marshal.SizeOf(typeof(uint));
            // Setup qualifier data to be used when retrieving the power saving hint
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get the power saving hint
            AudioObjectPropertyAddress propertyAddress = new AudioObjectPropertyAddress((uint)AudioObjectPropertySelector.PowerHint, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            AudioUnitStatus status = AudioObjectGetPropertyData(kAudioObjectSystemObject, ref propertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out uint powerHint);
            // If getting the property data was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve audio power saving hint - {status.ToString()}");
            }
            // Return if power saving has been enabled
            return powerHint == kAudioHardwarePowerHintFavorSavingPower;
        }

        /// <summary>
        /// Sets if power saving has been enabled.
        /// </summary>
        /// <param name="enabled"><c>true</c> if power saving enabled is enabled, <c>false</c> otherwise.</param>
        internal static void SetPowerSavingEnabled(bool enabled)
        {
            // Set the data to be set
            uint powerHint;
            if (enabled)
            {
                powerHint = kAudioHardwarePowerHintFavorSavingPower;
            }
            else
            {
                powerHint = kAudioHardwarePowerHintNone;
            }
            // Setup qualifier data to be used when setting the power saving hint
            IntPtr inQualifierData = IntPtr.Zero;
            // Set the power saving hint
            AudioObjectPropertyAddress propertyAddress = new AudioObjectPropertyAddress((uint)AudioObjectPropertySelector.PowerHint, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            AudioUnitStatus status = AudioObjectSetPropertyData(kAudioObjectSystemObject, ref propertyAddress, 0, ref inQualifierData, (uint)Marshal.SizeOf(typeof(uint)), ref powerHint);
            // If setting the property data was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to set audio power saving hint - {status.ToString()}");
            }
        }

        /// <summary>
        /// Gets the range of buffer sizes allowed by an audio device.
        /// </summary>
        /// <returns>The name of the device.</returns>
        /// <param name="deviceID">The identifier of the device to get the buffer range of.</param>
        internal static (double minimum, double maximum) GetBufferRange(uint deviceID)
        {
            // Set the size of the data to be returned in bytes
            uint propertySize = (uint)Marshal.SizeOf(typeof(AudioValueRange));
            // Setup qualifier data to be used when retrieving the buffer range
            uint inQualifierDataSize = 0;
            IntPtr inQualifierData = IntPtr.Zero;
            // Get the buffer range property
            AudioObjectPropertyAddress propertyAddress = new AudioObjectPropertyAddress(kAudioDevicePropertyBufferFrameSizeRange, AudioObjectPropertyScope.Global, AudioObjectPropertyElement.Master);
            AudioUnitStatus status = AudioObjectGetPropertyData(deviceID, ref propertyAddress, ref inQualifierDataSize, ref inQualifierData, ref propertySize, out AudioValueRange range);
            // If getting the size of the range property was not successful, throw an exception
            if (status != AudioUnitStatus.NoError)
            {
                throw new Exception($"Unable to retrieve the number of devices - {status.ToString()}");
            }
            // Return the range of buffer sizes
            return (range.mMinimum, range.mMaximum);
        }
        #endregion
    }
}