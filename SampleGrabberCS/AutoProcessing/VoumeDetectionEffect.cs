using AudioVisualization.AutoProcessing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace SampleGrabberCS
{
    static class VolumeDetectionPropertyNames
    {
        public static string Volume = "Volume";
    }


    public sealed class VolumeDetectionEffect : IBasicAudioEffect
    {
        private AudioEncodingProperties _currentEncodingProperties;
        private List<AudioEncodingProperties> _supportedEncodingProperties;
        private IPropertySet _effectProperties;

        public VolumeDetectionEffect()
        {
            // Let's only work in 32-bit float
            _supportedEncodingProperties = new List<AudioEncodingProperties>()
            {
                AudioEncodingProperties.CreatePcm(48000, 2, 32),
                AudioEncodingProperties.CreatePcm(44100, 2, 32),
            };
            foreach (var prop in _supportedEncodingProperties) { prop.Subtype = MediaEncodingSubtypes.Float; }
        }

        private double VolumeInDecibels
        {
            get
            {
                return (double)_effectProperties["Volume"];
            }
            set
            {
                _effectProperties["Volume"] = value;
            }
        }

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                return _supportedEncodingProperties;
            }
        }

        /// <summary>
        /// We are not modifying any audio data, so pass-through audio data
        /// automatically
        /// </summary>
        public bool UseInputFrameForOutput
        {
            get
            {
                return true;
            }
        }

        public void Close(MediaEffectClosedReason reason)
        {
            // No resources to clean up, so don't worry about Close
        }

        public void DiscardQueuedFrames()
        {
            // We don't cache frames, so we have nothing to discard
        }

        unsafe public void ProcessFrame(ProcessAudioFrameContext context)
        {
            AudioFrame inputFrame = context.InputFrame;

            using (AudioBuffer inputBuffer = context.InputFrame.LockBuffer(AudioBufferAccessMode.Read))
            using (IMemoryBufferReference inputReference = inputBuffer.CreateReference())
            {
                byte* inputInBytes;
                uint inputCapacity;
                float* inputInFloats;

                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputInBytes, out inputCapacity);

                inputInFloats = (float*)inputInBytes;
                int inputLength = (int)inputBuffer.Length / sizeof(float);
                float sum = 0;

                // Only process one channel for now (will average out unless the audio is severely unbalanced between left/right)
                for (int i = 0; i < inputLength; i += 2)
                {
                    sum += (inputInFloats[i] * inputInFloats[i]);
                }
                double rms = Math.Sqrt(sum / (inputLength / 2));
                this.VolumeInDecibels = 20 * Math.Log10(rms);
            }
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            _currentEncodingProperties = encodingProperties;
        }

        public void SetProperties(IPropertySet configuration)
        {
            _effectProperties = configuration;
        }
    }
}
