#include "pch.h"
#include "SampleGrabber.h"

CSampleGrabber::CSampleGrabber() : m_pSample(nullptr), m_pOutputType(nullptr), m_pInputType(nullptr)
{
	TRACE(L"construct");
}

CSampleGrabber::~CSampleGrabber()
{
	m_pAttributes.Reset();
}

// Initialize the instance.
STDMETHODIMP CSampleGrabber::RuntimeClassInitialize()
{
	// Create the attribute store.
	return MFCreateAttributes(m_pAttributes.GetAddressOf(), 3);
}

// IMediaExtension methods

//-------------------------------------------------------------------
// SetProperties
// Sets the configuration of the effect
//-------------------------------------------------------------------
HRESULT CSampleGrabber::SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
	return S_OK;
}

HRESULT CSampleGrabber::GetStreamLimits(
	DWORD   *pdwInputMinimum,
	DWORD   *pdwInputMaximum,
	DWORD   *pdwOutputMinimum,
	DWORD   *pdwOutputMaximum
	)
{
	if ((pdwInputMinimum == NULL) ||
		(pdwInputMaximum == NULL) ||
		(pdwOutputMinimum == NULL) ||
		(pdwOutputMaximum == NULL))
	{
		return E_POINTER;
	}

	// This MFT has a fixed number of streams.
	*pdwInputMinimum = 1;
	*pdwInputMaximum = 1;
	*pdwOutputMinimum = 1;
	*pdwOutputMaximum = 1;
	return S_OK;
}

//-------------------------------------------------------------------
// GetStreamCount
// Returns the actual number of streams.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetStreamCount(
	DWORD   *pcInputStreams,
	DWORD   *pcOutputStreams
	)
{
	if ((pcInputStreams == NULL) || (pcOutputStreams == NULL))

	{
		return E_POINTER;
	}

	// This MFT has a fixed number of streams.
	*pcInputStreams = 1;
	*pcOutputStreams = 1;
	return S_OK;
}

//-------------------------------------------------------------------
// GetStreamIDs
// Returns stream IDs for the input and output streams.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetStreamIDs(
	DWORD   dwInputIDArraySize,
	DWORD   *pdwInputIDs,
	DWORD   dwOutputIDArraySize,
	DWORD   *pdwOutputIDs
	)
{
	// It is not required to implement this method if the MFT has a fixed number of
	// streams AND the stream IDs are numbered sequentially from zero (that is, the
	// stream IDs match the stream indexes).

	// In that case, it is OK to return E_NOTIMPL.
	return E_NOTIMPL;
}

//-------------------------------------------------------------------
// GetInputStreamInfo
// Returns information about an input stream.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetInputStreamInfo(
	DWORD                     dwInputStreamID,
	MFT_INPUT_STREAM_INFO *   pStreamInfo
	)
{
	if (pStreamInfo == NULL)
	{
		return E_POINTER;
	}

	auto lock = m_cs.Lock();

	if (dwInputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	//TODO: may need to do something to the stream info here

	return S_OK;
}

//-------------------------------------------------------------------
// GetOutputStreamInfo
// Returns information about an output stream.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetOutputStreamInfo(
	DWORD                     dwOutputStreamID,
	MFT_OUTPUT_STREAM_INFO *  pStreamInfo
	)
{
	if (pStreamInfo == NULL)
	{
		return E_POINTER;
	}

	auto lock = m_cs.Lock();

	if (dwOutputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	// NOTE: This method should succeed even when there is no media type on the
	//       stream. If there is no media type, we only need to fill in the dwFlags
	//       member of MFT_OUTPUT_STREAM_INFO. The other members depend on having a
	//       a valid media type.

	
	// Flags
	pStreamInfo->dwFlags =
		MFT_OUTPUT_STREAM_WHOLE_SAMPLES |         // Output buffers contain complete audio frames.
		MFT_OUTPUT_STREAM_CAN_PROVIDE_SAMPLES |   // The MFT can allocate output buffers, or use caller-allocated buffers.
		MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE;      // Samples (ie, audio frames) are fixed size.

	pStreamInfo->cbSize = 0;   // If no media type is set, use zero.
	pStreamInfo->cbAlignment = 0;

	if (m_pOutputType != nullptr){
		pStreamInfo->cbSize = MFGetAttributeUINT32(m_pOutputType.Get(), MF_MT_AUDIO_BLOCK_ALIGNMENT, 0);
	}

	return S_OK;
}

//-------------------------------------------------------------------
// GetAttributes
// Returns the attributes for the MFT.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetAttributes(IMFAttributes** ppAttributes)
{
	if (ppAttributes == NULL)
	{
		return E_POINTER;
	}

	auto lock = m_cs.Lock();

	ASSERT(ppAttributes);
	m_pAttributes.CopyTo(ppAttributes);

	return S_OK;
}

//-------------------------------------------------------------------
// GetInputStreamAttributes
// Returns stream-level attributes for an input stream.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetInputStreamAttributes(
	DWORD           dwInputStreamID,
	IMFAttributes   **ppAttributes
	)
{
	// This MFT does not support any stream-level attributes, so the method is not implemented.
	return E_NOTIMPL;
}


//-------------------------------------------------------------------
// GetOutputStreamAttributes
// Returns stream-level attributes for an output stream.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetOutputStreamAttributes(
	DWORD           dwOutputStreamID,
	IMFAttributes   **ppAttributes
	)
{
	// This MFT does not support any stream-level attributes, so the method is not implemented.
	return E_NOTIMPL;
}


//-------------------------------------------------------------------
// DeleteInputStream
//-------------------------------------------------------------------

HRESULT CSampleGrabber::DeleteInputStream(DWORD dwStreamID)
{
	// This MFT has a fixed number of input streams, so the method is not supported.
	return E_NOTIMPL;
}


//-------------------------------------------------------------------
// AddInputStreams
//-------------------------------------------------------------------

HRESULT CSampleGrabber::AddInputStreams(
	DWORD   cStreams,
	DWORD   *adwStreamIDs
	)
{
	// This MFT has a fixed number of output streams, so the method is not supported.
	return E_NOTIMPL;
}



//-------------------------------------------------------------------
// GetInputAvailableType
// Returns a preferred input type.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetInputAvailableType(
	DWORD           dwInputStreamID,
	DWORD           dwTypeIndex, // 0-based
	IMFMediaType    **ppType
	)
{
	if (ppType == NULL)
	{
		return E_INVALIDARG;
	}

	auto lock = m_cs.Lock();

	if (dwInputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	HRESULT hr = S_OK;

	// If the output type is set, return that type as our preferred input type.
	if (m_pOutputType == NULL)
	{
		// The output type is not set. Create a partial media type.

		Microsoft::WRL::ComPtr<IMFMediaType> pmt;

		HRESULT hr = MFCreateMediaType(pmt.GetAddressOf());
		if (FAILED(hr))
		{
			return hr;
		}

		if (dwTypeIndex == 0) {
			hr = pmt->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
			if (FAILED(hr))
			{
				return hr;
			}

			hr = pmt->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
			if (FAILED(hr))
			{
				return hr;
			}
		}
		else if (dwTypeIndex == 1) {

		}

		
		pmt.CopyTo(ppType);
		return S_OK;


	}
	else if (dwTypeIndex > 0)
	{
		hr = MF_E_NO_MORE_TYPES;
	}
	else
	{
		ASSERT(m_pOutputType);
		m_pOutputType.CopyTo(ppType);
	}

	return hr;
}

//-------------------------------------------------------------------
// GetOutputAvailableType
// Returns a preferred output type.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetOutputAvailableType(
	DWORD           dwOutputStreamID,
	DWORD           dwTypeIndex, // 0-based
	IMFMediaType    **ppType
	)
{
	if (ppType == NULL)
	{
		return E_INVALIDARG;
	}

	auto lock = m_cs.Lock();

	if (dwOutputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	HRESULT hr = S_OK;

	// MFMediaType_Audio
	// MFAudioFormat_PCM
	Microsoft::WRL::ComPtr<IMFMediaType> wav;
	//hr = MFCreateMediaType(&wav);


	if (m_pInputType == NULL)
	{
		// The input type is not set. Create a partial media type.
		ASSERT(0);
		//hr = OnGetPartialType(dwTypeIndex, ppType);
	}
	else if (dwTypeIndex > 0)
	{
		hr = MF_E_NO_MORE_TYPES;
	}
	else
	{
		//hr = ConvertAudioTypeToPCM(&m_pInputType, wav.Get());
		//ASSERT(m_pInputType);

		m_pInputType.CopyTo(ppType);
	}

	return hr;
}




//-------------------------------------------------------------------
// SetInputType
//-------------------------------------------------------------------

HRESULT CSampleGrabber::SetInputType(
	DWORD           dwInputStreamID,
	IMFMediaType    *pType, // Can be NULL to clear the input type.
	DWORD           dwFlags
	)
{
	// Validate flags.
	if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
	{
		return E_INVALIDARG;
	}

	auto lock = m_cs.Lock();

	if (dwInputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	// Does the caller want us to set the type, or just test it?
	BOOL bReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

	// If we have an input sample, the client cannot change the type now.
	if (m_pSample!=nullptr)
	{
		return MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
	}

	// Validate the type, if non-NULL.
	if (pType) {
		if (m_pOutputType != nullptr)
		{
			DWORD flags = 0;
			auto hr = pType->IsEqual(m_pOutputType.Get(), &flags);

			// IsEqual can return S_FALSE. Treat this as failure.
			if (hr != S_OK)
			{
				return MF_E_INVALIDMEDIATYPE;
			}
		}
		else {
			GUID major_type;
			HRESULT hr = pType->GetGUID(MF_MT_MAJOR_TYPE, &major_type);
			if (FAILED(hr)) return E_FAIL;
			if (major_type != MFMediaType_Audio) return MF_E_INVALIDMEDIATYPE;
			//TODO: may need to do more stringent test
		}
	}

	// The type is OK. Set the type, unless the caller was just testing.
	if (bReallySet)
	{
		//m_pInputType.Attach(pType);		
		m_pInputType = pType;		

		// When the type changes, end streaming.
		return EndStreaming();
	}
	return S_OK;
}

HRESULT CSampleGrabber::SetOutputType(
	DWORD           dwOutputStreamID,
	IMFMediaType    *pType, // Can be NULL to clear the output type.
	DWORD           dwFlags
	)
{
	// Validate flags.
	if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
	{
		return E_INVALIDARG;
	}

	auto lock = m_cs.Lock();

	if (dwOutputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	BOOL bReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

	// If we have an input sample, the client cannot change the type now.
	if (m_pSample != nullptr)
	{
		return MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
	}

	if (pType)
	{
		if (m_pInputType != nullptr)
		{
			DWORD flags = 0;
			auto hr = pType->IsEqual(m_pInputType.Get(), &flags);

			// IsEqual can return S_FALSE. Treat this as failure.
			if (hr != S_OK)
			{
				return MF_E_INVALIDMEDIATYPE;
			}

			m_pOutputType.Attach(pType);
		}
		else {
			GUID major_type;
			HRESULT hr = pType->GetGUID(MF_MT_MAJOR_TYPE, &major_type);
			if (FAILED(hr)) return E_FAIL;
			if (major_type != MFMediaType_Audio) return MF_E_INVALIDMEDIATYPE;
			//TODO: may need to do more stringent test
			return S_OK;
		}
	}

	return S_OK;
}

//-------------------------------------------------------------------
// GetInputCurrentType
// Returns the current input type.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetInputCurrentType(
	DWORD           dwInputStreamID,
	IMFMediaType    **ppType
	)
{
	if (ppType == NULL)
	{
		return E_POINTER;
	}

	HRESULT hr = S_OK;

	auto lock = m_cs.Lock();

	if (dwInputStreamID!=0)
	{
		hr = MF_E_INVALIDSTREAMNUMBER;
	}
	else if (!m_pInputType)
	{
		hr = MF_E_TRANSFORM_TYPE_NOT_SET;
	}
	else
	{
		m_pInputType.CopyTo(ppType);
	}
	return hr;
}

//-------------------------------------------------------------------
// GetOutputCurrentType
// Returns the current output type.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetOutputCurrentType(
	DWORD           dwOutputStreamID,
	IMFMediaType    **ppType
	)
{
	if (ppType == NULL)
	{
		return E_POINTER;
	}

	HRESULT hr = S_OK;

	auto lock = m_cs.Lock();

	if (dwOutputStreamID!=0)
	{
		hr = MF_E_INVALIDSTREAMNUMBER;
	}
	else if (!m_pOutputType)
	{
		hr = MF_E_TRANSFORM_TYPE_NOT_SET;
	}
	else
	{
		m_pOutputType.CopyTo(ppType);
	}

	return hr;
}


//-------------------------------------------------------------------
// GetInputStatus
// Query if the MFT is accepting more input.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetInputStatus(
	DWORD           dwInputStreamID,
	DWORD           *pdwFlags
	)
{
	if (pdwFlags == NULL)
	{
		return E_POINTER;
	}

	auto lock = m_cs.Lock();

	if (dwInputStreamID==0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	// If an input sample is already queued, do not accept another sample until the 
	// client calls ProcessOutput or Flush.

	// NOTE: It is possible for an MFT to accept more than one input sample. For 
	// example, this might be required in a video decoder if the frames do not 
	// arrive in temporal order. In the case, the decoder must hold a queue of 
	// samples. For the video effect, each sample is transformed independently, so
	// there is no reason to queue multiple input samples.

	if (m_pSample == NULL)
	{
		*pdwFlags = MFT_INPUT_STATUS_ACCEPT_DATA;
	}
	else
	{
		*pdwFlags = 0;
	}

	return S_OK;
}

//-------------------------------------------------------------------
// GetOutputStatus
// Query if the MFT can produce output.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::GetOutputStatus(DWORD *pdwFlags)
{
	if (pdwFlags == NULL)
	{
		return E_POINTER;
	}

	auto lock = m_cs.Lock();

	// The MFT can produce an output sample if (and only if) there an input sample.
	if (m_pSample != NULL)
	{
		*pdwFlags = MFT_OUTPUT_STATUS_SAMPLE_READY;
	}
	else
	{
		*pdwFlags = 0;
	}

	return S_OK;
}

//-------------------------------------------------------------------
// SetOutputBounds
// Sets the range of time stamps that the MFT will output.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::SetOutputBounds(
	LONGLONG        hnsLowerBound,
	LONGLONG        hnsUpperBound
	)
{
	// Implementation of this method is optional.
	return E_NOTIMPL;
}

//-------------------------------------------------------------------
// ProcessEvent
// Sends an event to an input stream.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::ProcessEvent(
	DWORD              dwInputStreamID,
	IMFMediaEvent      *pEvent
	)
{
	// This MFT does not handle any stream events, so the method can
	// return E_NOTIMPL. This tells the pipeline that it can stop
	// sending any more events to this MFT.
	return E_NOTIMPL;
}


//-------------------------------------------------------------------
// ProcessMessage
//-------------------------------------------------------------------

HRESULT CSampleGrabber::ProcessMessage(
	MFT_MESSAGE_TYPE    eMessage,
	ULONG_PTR           ulParam
	)
{
	auto lock = m_cs.Lock();

	HRESULT hr = S_OK;

	switch (eMessage)
	{
	case MFT_MESSAGE_COMMAND_FLUSH:
		// Flush the MFT.
		hr = OnFlush();
		break;

	case MFT_MESSAGE_COMMAND_DRAIN:
		// Drain: Tells the MFT to reject further input until all pending samples are
		// processed. That is our default behavior already, so there is nothing to do.
		//
		// For a decoder that accepts a queue of samples, the MFT might need to drain
		// the queue in response to this command.
		break;

	case MFT_MESSAGE_SET_D3D_MANAGER:
		// Sets a pointer to the IDirect3DDeviceManager9 interface.

		// The pipeline should never send this message unless the MFT sets the MF_SA_D3D_AWARE 
		// attribute set to TRUE. Because this MFT does not set MF_SA_D3D_AWARE, it is an error
		// to send the MFT_MESSAGE_SET_D3D_MANAGER message to the MFT. Return an error code in
		// this case.

		// NOTE: If this MFT were D3D-enabled, it would cache the IDirect3DDeviceManager9 
		// pointer for use during streaming.

		hr = E_NOTIMPL;
		break;

	case MFT_MESSAGE_NOTIFY_BEGIN_STREAMING:
		hr = BeginStreaming();
		break;

	case MFT_MESSAGE_NOTIFY_END_STREAMING:
		hr = EndStreaming();
		break;

		// The next two messages do not require any action from this MFT.

	case MFT_MESSAGE_NOTIFY_END_OF_STREAM:
		break;

	case MFT_MESSAGE_NOTIFY_START_OF_STREAM:
		break;
	}

	return hr;
}

//-------------------------------------------------------------------
// ProcessInput
// Process an input sample.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::ProcessInput(
	DWORD               dwInputStreamID,
	IMFSample           *pSample,
	DWORD               dwFlags
	)
{
	// Check input parameters.
	if (pSample == NULL)
	{
		return E_POINTER;
	}

	if (dwFlags != 0)
	{
		return E_INVALIDARG; // dwFlags is reserved and must be zero.
	}

	HRESULT hr = S_OK;

	auto lock = m_cs.Lock();

	// Validate the input stream number.
	if (dwInputStreamID!=0)
	{
		return MF_E_INVALIDSTREAMNUMBER;
	}

	// Check for valid media types.
	// The client must set input and output types before calling ProcessInput.
	if (!m_pInputType || !m_pOutputType)
	{
		return MF_E_NOTACCEPTING;
	}

	// Check if an input sample is already queued.
	if (m_pSample != NULL)
	{
		return MF_E_NOTACCEPTING;   // We already have an input sample.
	}

	// Initialize streaming.
	hr = BeginStreaming();
	if (FAILED(hr))
	{
		return hr;
	}

	// Cache the sample. We do the actual work in ProcessOutput.
	m_pSample = pSample;

	return hr;
}

//-------------------------------------------------------------------
// ProcessOutput
// Process an output sample.
//-------------------------------------------------------------------

HRESULT CSampleGrabber::ProcessOutput(
	DWORD                   dwFlags,
	DWORD                   cOutputBufferCount,
	MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
	DWORD                   *pdwStatus
	)
{
	if (dwFlags != 0)
	{
		return E_INVALIDARG;
	}

	if (pOutputSamples == NULL || pdwStatus == NULL)
	{
		return E_POINTER;
	}

	// There must be exactly one output buffer.
	if (cOutputBufferCount != 1)
	{
		return E_INVALIDARG;
	}

	HRESULT hr = S_OK;


	auto lock = m_cs.Lock();

	if (m_pSample == nullptr)
	{
		return MF_E_TRANSFORM_NEED_MORE_INPUT;
	}

	// Initialize streaming.

	hr = BeginStreaming();
	if (FAILED(hr))
	{
		return hr;
	}

#pragma region MyRegion
	 //Get the input buffer.
	Microsoft::WRL::ComPtr<IMFMediaBuffer> pInput;

	hr = m_pSample->ConvertToContiguousBuffer(&pInput);
	if (FAILED(hr))
	{
		return hr;
	}  

	DWORD currentLengthIn(0);
	DWORD maxLengthIn(0);
	DWORD currentLengthOut(0);
	DWORD maxLengthOut(0);

	BYTE* pInputBytes = nullptr;
	BYTE* pOutputBytes = nullptr;

	hr = pInput->Lock(&pInputBytes, &maxLengthIn, &currentLengthIn);
	if (FAILED(hr))
	{
	return hr;
	}

	DirectX::XMVECTOR* vectorInputBuffer = (DirectX::XMVECTOR*)_aligned_malloc(currentLengthIn/4 * sizeof(DirectX::XMVECTOR), 16);

	for (int i = 0; i < currentLengthIn/4; i++) {
		vectorInputBuffer[i] = DirectX::XMLoadFloat((float*)(pInputBytes + (i*4)));
	}

	_aligned_free(vectorInputBuffer);

#pragma endregion

	if (pOutputSamples->pSample != nullptr)
	{
		ASSERT(false);
	}
	else {
		//pOutputSamples[0].pSample = m_pSample.Detach();
		pOutputSamples->pSample = m_pSample.Detach();
	}

	// Set status flags.
	pOutputSamples[0].dwStatus = 0;
	*pdwStatus = 0;
	
	return hr;
}

HRESULT CSampleGrabber::EndStreaming()
{
	m_bStreamingInitialized = false;
	return S_OK;
}

// Initialize streaming parameters.
//
// This method is called if the client sends the MFT_MESSAGE_NOTIFY_BEGIN_STREAMING
// message, or when the client processes a sample, whichever happens first.

HRESULT CSampleGrabber::BeginStreaming()
{
	HRESULT hr = S_OK;

	if (!m_bStreamingInitialized)
	{
		m_bStreamingInitialized = true;
	}

	return hr;
}

// Flush the MFT.

HRESULT CSampleGrabber::OnFlush()
{
	// For this MFT, flushing just means releasing the input sample.
	m_pSample.Reset();
	return S_OK;
}
