#include "pch.h"
#include "SampleGrabber.h"

using namespace Microsoft::WRL;

ActivatableClass(CSampleGrabber);

HRESULT WINAPI DllGetActivationFactory(_In_ HSTRING activatibleClassId, _Outptr_ IActivationFactory** factory)
{
	auto &module = Microsoft::WRL::Module< Microsoft::WRL::InProc >::GetModule();
	return module.GetActivationFactory(activatibleClassId, factory);
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID FAR* ppv)
{
	auto &module = Microsoft::WRL::Module<Microsoft::WRL::InProc>::GetModule();
	return module.GetClassObject(rclsid, riid, ppv);
}
