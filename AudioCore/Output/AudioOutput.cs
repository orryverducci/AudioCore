﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AudioCore.Input;

namespace AudioCore.Output
{
    public class AudioOutput
    {
        #region Private Fields
        /// <summary>
        /// The number of audio channels
        /// </summary>
        private int _channels;

        /// <summary>
        /// The audio sample rate in Hertz
        /// </summary>
        private int _sampleRate;

        /// <summary>
        /// The latency on the audio in milliseconds
        /// </summary>
        private int _latency;

        /// <summary>
        /// The audio output bit depth
        /// </summary>
        private int _bitDepth;

        /// <summary>
        /// The audio inputs
        /// </summary>
        private List<AudioInput> _audioInputs = new List<AudioInput>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of audio channels
        /// </summary>
        /// <value>The number of audio channels</value>
        public int Channels
        {
            get
            {
                return _channels;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The number of audio channels must be greater than 0");
                }
                _channels = value;
            }
        }

        /// <summary>
        /// Gets the audio sample rate in Hertz
        /// </summary>
        /// <value>The audio sample rate in Hertz</value>
        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The sample rate must be greater than 0");
                }
                _sampleRate = value;
            }
        }

        /// <summary>
        /// Gets the latency on the audio in milliseconds
        /// </summary>
        /// <value>The audio latency</value>
        public int Latency
        {
            get
            {
                return _latency;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The latency must be greater than 0");
                }
                _latency = value;
            }
        }

        /// <summary>
        /// Gets the audio output bit depth
        /// </summary>
        /// <value>The audio output bit depth</value>
        public int BitDepth
        {
            get
            {
                return _bitDepth;
            }
            protected set
            {
                // Check value is between 8 and 64 bits and a multiple of 8
                if (value < 8 || value > 64)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Bit depth must be between 8 bit and 64 bit");
                }
                else if (value % 8 > 0)
                {
                    throw new ArgumentException("Bit depth must a multiple of 8", nameof(value));
                }
                // Set value
                _bitDepth = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an audio input to the output
        /// </summary>
        /// <param name="input">The audio input to be added</param>
        public void AddInput(AudioInput input)
        {
            // Check the input audio properties matches the audio properties of the output
            if (input.Channels != Channels)
            {
                throw new ArgumentException("The number of input channels does not match the number of output channels", nameof(input));
            }
            else if (input.SampleRate != SampleRate)
            {
                throw new ArgumentException("The input sample rate does not match the output sample rate", nameof(input));
            }
            // Add the input to the list of inputs
            _audioInputs.Add(input);
        }

        /// <summary>
        /// Remove an audio input from the output
        /// </summary>
        /// <param name="input">The audio input to be removed</param>
        public void RemoveInput(AudioInput input)
        {
            // Check the given input is on the list of inputs, and remove it if it is
            if (_audioInputs.Contains(input))
            {
                _audioInputs.Remove(input);
            }
            else
            {
                throw new ArgumentException("The input hasn't been added to this output", nameof(input));
            }
        }

        /// <summary>
        /// Get frames of audio from the audio inputs
        /// </summary>
        /// <param name="framesRequired">The number of frames required</param>
        protected async Task<double[]> GetInputFrames(int framesRequired)
        {
            // Create array of mixed (combined) frames
            double[] mixedFrames = new double[framesRequired * Channels];
            // Get samples from each input that is playing
            foreach (var input in _audioInputs)
            {
                // Only get frames from the input if it is currently playing
                if (input.PlaybackState == PlaybackState.PLAYING)
                {
                    // Get frames from the audio input
                    double[] inputFrames = await input.GetFrames(framesRequired);
                    // Get the shorter length from the returned frames array or the mixed frames array
                    int mixLength = (inputFrames.Length < mixedFrames.Length) ? inputFrames.Length : mixedFrames.Length;
                    // Add the frames from the audio input to the mixed frames
                    for (int i = 0; i < mixLength; i++)
                    {
                        mixedFrames[i] += inputFrames[i];
                    }
                }
            }
            // Return the mixed samples
            return mixedFrames;
        }
        #endregion
    }
}