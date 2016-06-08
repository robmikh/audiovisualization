#include "pch.h"
#include "MFHelpers.h"

HRESULT ConvertAudioTypeToFloat32(
	IMFMediaType *pType,        // Pointer to an encoded audio type.
	IMFMediaType **ppType       // Receives a matching PCM audio type.
)
{
	HRESULT hr = S_OK;

	GUID majortype = { 0 };
	GUID subtype = { 0 };

	UINT32 cChannels = 0;
	UINT32 samplesPerSec = 0;
	UINT32 bitsPerSample = 0;

	hr = pType->GetMajorType(&majortype);
	if (FAILED(hr))
	{
		return hr;
	}

	if (majortype != MFMediaType_Audio)
	{
		return MF_E_INVALIDMEDIATYPE;
	}

	// Get the audio subtype.
	hr = pType->GetGUID(MF_MT_SUBTYPE, &subtype);
	if (FAILED(hr))
	{
		return hr;
	}

	if (subtype == MFAudioFormat_Float)
	{
		// This is already a PCM audio type. Return the same pointer.

		*ppType = pType;
		(*ppType)->AddRef();

		return S_OK;
	}

	// Get the sample rate and other information from the audio format.

	cChannels = MFGetAttributeUINT32(pType, MF_MT_AUDIO_NUM_CHANNELS, 0);
	samplesPerSec = MFGetAttributeUINT32(pType, MF_MT_AUDIO_SAMPLES_PER_SECOND, 0);
	bitsPerSample = MFGetAttributeUINT32(pType, MF_MT_AUDIO_BITS_PER_SAMPLE, 16);

	// Note: Some encoded audio formats do not contain a value for bits/sample.
	// In that case, use a default value of 16. Most codecs will accept this value.

	if (cChannels == 0 || samplesPerSec == 0)
	{
		return MF_E_INVALIDTYPE;
	}

	// Create the corresponding PCM audio type.
	hr = CreateFloat32AudioType(samplesPerSec, bitsPerSample, cChannels, ppType);

	return hr;
}

HRESULT CreateFloat32AudioType(
	UINT32 sampleRate,        // Samples per second
	UINT32 bitsPerSample,     // Bits per sample
	UINT32 cChannels,         // Number of channels
	IMFMediaType **ppType     // Receives a pointer to the media type.
)
{
	HRESULT hr = S_OK;

	Microsoft::WRL::ComPtr<IMFMediaType> pType = NULL;

	// Calculate derived values.
	UINT32 blockAlign = cChannels * (bitsPerSample / 8);
	UINT32 bytesPerSecond = blockAlign * sampleRate;

	// Create the empty media type.
	hr = MFCreateMediaType(&pType);
	if (FAILED(hr))
	{
		goto done;
	}

	// Set attributes on the type.
	hr = pType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_Float);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, cChannels);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, sampleRate);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, blockAlign);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, bytesPerSecond);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, bitsPerSample);
	if (FAILED(hr))
	{
		goto done;
	}

	hr = pType->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, TRUE);
	if (FAILED(hr))
	{
		goto done;
	}

	pType.CopyTo(ppType);

done:
	return hr;
}