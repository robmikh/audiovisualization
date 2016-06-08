#pragma once
#include <mfapi.h>
#include <mferror.h>

	HRESULT CreateFloat32AudioType(
		UINT32 sampleRate,        // Samples per second
		UINT32 bitsPerSample,     // Bits per sample
		UINT32 cChannels,         // Number of channels
		IMFMediaType **ppType     // Receives a pointer to the media type.
	);

	HRESULT ConvertAudioTypeToFloat32(
		IMFMediaType *pType,        // Pointer to an encoded audio type.
		IMFMediaType **ppType       // Receives a matching PCM audio type.
	);

