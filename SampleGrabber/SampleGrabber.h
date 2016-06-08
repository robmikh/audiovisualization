#pragma once
#include "SampleGrabberMFT.h"

#include <mfapi.h>
#include <mftransform.h>
#include <mfidl.h>
#include <mferror.h>

#pragma comment(lib, "mf.lib")
#pragma comment(lib, "mfuuid.lib")
#pragma comment(lib, "mfplat.lib")

#include <wrl\implements.h>
#include <wrl\module.h>
#include <windows.media.h>
#include <wrl\wrappers\corewrappers.h>
#include "DirectXMath.h"
#include <malloc.h>

class CSampleGrabber
	: public Microsoft::WRL::RuntimeClass<
	Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >,
	ABI::Windows::Media::IMediaExtension,
	IMFTransform >
{
	InspectableClass(RuntimeClass_SG_SampleGrabberTransform, BaseTrust)

public:
	CSampleGrabber();

	~CSampleGrabber();

	STDMETHOD(RuntimeClassInitialize)();

	// IMediaExtension
	STDMETHODIMP SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

	// IMFTransform
	STDMETHODIMP GetStreamLimits(
		DWORD   *pdwInputMinimum,
		DWORD   *pdwInputMaximum,
		DWORD   *pdwOutputMinimum,
		DWORD   *pdwOutputMaximum
		);

	STDMETHODIMP GetStreamCount(
		DWORD   *pcInputStreams,
		DWORD   *pcOutputStreams
		);

	STDMETHODIMP GetStreamIDs(
		DWORD   dwInputIDArraySize,
		DWORD   *pdwInputIDs,
		DWORD   dwOutputIDArraySize,
		DWORD   *pdwOutputIDs
		);

	STDMETHODIMP GetInputStreamInfo(
		DWORD                     dwInputStreamID,
		MFT_INPUT_STREAM_INFO *   pStreamInfo
		);

	STDMETHODIMP GetOutputStreamInfo(
		DWORD                     dwOutputStreamID,
		MFT_OUTPUT_STREAM_INFO *  pStreamInfo
		);

	STDMETHODIMP GetAttributes(IMFAttributes** pAttributes);

	STDMETHODIMP GetInputStreamAttributes(
		DWORD           dwInputStreamID,
		IMFAttributes   **ppAttributes
		);

	STDMETHODIMP GetOutputStreamAttributes(
		DWORD           dwOutputStreamID,
		IMFAttributes   **ppAttributes
		);

	STDMETHODIMP DeleteInputStream(DWORD dwStreamID);

	STDMETHODIMP AddInputStreams(
		DWORD   cStreams,
		DWORD   *adwStreamIDs
		);

	STDMETHODIMP GetInputAvailableType(
		DWORD           dwInputStreamID,
		DWORD           dwTypeIndex, // 0-based
		IMFMediaType    **ppType
		);

	STDMETHODIMP GetOutputAvailableType(
		DWORD           dwOutputStreamID,
		DWORD           dwTypeIndex, // 0-based
		IMFMediaType    **ppType
		);

	STDMETHODIMP SetInputType(
		DWORD           dwInputStreamID,
		IMFMediaType    *pType,
		DWORD           dwFlags
		);

	STDMETHODIMP SetOutputType(
		DWORD           dwOutputStreamID,
		IMFMediaType    *pType,
		DWORD           dwFlags
		);

	STDMETHODIMP GetInputCurrentType(
		DWORD           dwInputStreamID,
		IMFMediaType    **ppType
		);

	STDMETHODIMP GetOutputCurrentType(
		DWORD           dwOutputStreamID,
		IMFMediaType    **ppType
		);

	STDMETHODIMP GetInputStatus(
		DWORD           dwInputStreamID,
		DWORD           *pdwFlags
		);

	STDMETHODIMP GetOutputStatus(DWORD *pdwFlags);

	STDMETHODIMP SetOutputBounds(
		LONGLONG        hnsLowerBound,
		LONGLONG        hnsUpperBound
		);

	STDMETHODIMP ProcessEvent(
		DWORD              dwInputStreamID,
		IMFMediaEvent      *pEvent
		);

	STDMETHODIMP ProcessMessage(
		MFT_MESSAGE_TYPE    eMessage,
		ULONG_PTR           ulParam
		);

	STDMETHODIMP ProcessInput(
		DWORD               dwInputStreamID,
		IMFSample           *pSample,
		DWORD               dwFlags
		);

	STDMETHODIMP ProcessOutput(
		DWORD                   dwFlags,
		DWORD                   cOutputBufferCount,
		MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
		DWORD                   *pdwStatus
		);

private:
	Microsoft::WRL::ComPtr<IMFAttributes>		m_pAttributes;
	Microsoft::WRL::ComPtr<IMFMediaType>		m_pInputType;              // Input media type.
	Microsoft::WRL::ComPtr<IMFMediaType>		m_pOutputType;             // Output media type.
	Microsoft::WRL::Wrappers::CriticalSection	m_cs;
	Microsoft::WRL::ComPtr<IMFSample>			m_pSample;
	Microsoft::WRL::ComPtr <IMFMediaType>		m_pMediaType;
	bool										m_bStreamingInitialized;
	HRESULT BeginStreaming();
	HRESULT EndStreaming();
	HRESULT OnFlush();


};

