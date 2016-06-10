using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.AutoProcessing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.UI.Composition;

namespace SampleGrabberCS.Reference
{
    public sealed class PassthroughEffect : IBasicAudioEffect
    {
        // This is bad and I should feel bad. Remove after testing is done.
        private static object badLock = new object();

        public static object GetBadLock() { return badLock; }

        private AudioEncodingProperties currentEncodingProperties;
        private List<AudioEncodingProperties> supportedEncodingProperties;

        private IPropertySet propertySet;
        private CompositionPropertySet compositionPropertySet;

        public bool UseInputFrameForOutput { get { return false; } }
        public bool TimeIndependent { get { return true; } }
        public bool IsReadyOnly { get { return true; } }

        // Set up constant members in the constructor
        public PassthroughEffect()
        {
            Debug.WriteLine("constructor");
            // tried changing these to stereo, likely need to dig in more to the media type stuff.

            // Support 44.1kHz and 48kHz mono float
            supportedEncodingProperties = new List<AudioEncodingProperties>();
            AudioEncodingProperties encodingProps1 = AudioEncodingProperties.CreatePcm(44100, 1, 32);
            encodingProps1.Subtype = MediaEncodingSubtypes.Float;
            AudioEncodingProperties encodingProps2 = AudioEncodingProperties.CreatePcm(48000, 1, 32);
            encodingProps2.Subtype = MediaEncodingSubtypes.Float;

            supportedEncodingProperties.Add(encodingProps1);
            supportedEncodingProperties.Add(encodingProps2);
        }

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                return supportedEncodingProperties;
            }
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            currentEncodingProperties = encodingProperties;
        }

        unsafe public void ProcessFrame(ProcessAudioFrameContext context)
        {
            AudioFrame inputFrame = context.InputFrame;
            AudioFrame outputFrame = context.OutputFrame;

            using (AudioBuffer inputBuffer = inputFrame.LockBuffer(AudioBufferAccessMode.Read),
                                outputBuffer = outputFrame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference inputReference = inputBuffer.CreateReference(),
                                            outputReference = outputBuffer.CreateReference())
            {
                byte* inputDataInBytes;
                byte* outputDataInBytes;
                uint inputCapacity;
                uint outputCapacity;

                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputDataInBytes, out inputCapacity);
                ((IMemoryBufferByteAccess)outputReference).GetBuffer(out outputDataInBytes, out outputCapacity);

                float* inputDataInFloat = (float*)inputDataInBytes;
                float* outputDataInFloat = (float*)outputDataInBytes;

                // Process audio data
                int dataInFloatLength = (int)inputBuffer.Length / sizeof(float);

                for (int i = 0; i < dataInFloatLength; i++)
                {
                    float inputData = inputDataInFloat[i];
                    outputDataInFloat[i] = inputData;

                    lock(badLock)
                    {
                        if (propertySet.ContainsKey("InputDataRaw"))
                        {
                            propertySet["InputDataRaw"] = inputData;
                        }
                        else
                        {
                            propertySet.Add("InputDataRaw", inputData);
                        }
                    }
                    
                    if (compositionPropertySet != null)
                    {
                        compositionPropertySet.InsertScalar("InputData", inputData * 500);
                    }
                }
            }
        }

        public void Close(MediaEffectClosedReason reason)
        {
            // Clean-up any effect resources
            // This effect doesn't care about close, so there's nothing to do
        }

        public void DiscardQueuedFrames()
        {

        }

        public void SetProperties(IPropertySet configuration)
        {
            this.propertySet = configuration;

            if (propertySet.ContainsKey("compositionPropertySet"))
            {
                compositionPropertySet = (CompositionPropertySet)propertySet["compositionPropertySet"];
            }
        }
    }
}
