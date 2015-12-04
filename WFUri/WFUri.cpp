// This is the main DLL file.

#include "stdafx.h"

#include "WFUri.h"
#include <winstring.h>
#include <roapi.h>
#include <vcclr.h>

#undef _MANAGED // Lucky this works. Not advisable.
#include <wrl.h>

using namespace ABI::Windows::Foundation;
using namespace Common;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;
using namespace System;
using namespace System::Collections::Generic;
using namespace WFUri;

Settings^ WFUriFactory::CreateEmptySettings()
{
	Settings^ settings = gcnew Settings();
	return settings;
}

String^ WFUriFactory::GetName()
{
	return "Windows.Foundation.Uri";
}

String^ WFUriFactory::GetDescription()
{
	return "The Windows.Foundation.Uri URI parser from WinRT";
}

List<UriProperty>^ WFUriFactory::Create(String^ uriAsString, Settings^ settings)
{
	ComPtr<IUriRuntimeClassFactory> uriFactory;
	RoGetActivationFactory(HStringReference(RuntimeClass_Windows_Foundation_Uri).Get(), IID_IUriRuntimeClassFactory, &uriFactory);
	ComPtr<IUriRuntimeClass> uri;
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;
	List<UriProperty>^ properties = gcnew List<UriProperty>();

	HRESULT hr = uriFactory->CreateUri(HStringReference(uriAsWChar).Get(), &uri);

	if (SUCCEEDED(hr))
	{
		properties = UriToProperties(uri.Get());
	}
	else
	{
		properties->Add(UriProperty("W.F.Uri.CreateUriFailure", String::Format("{0}", hr)));
	}

	return properties;
}

List<UriProperty>^ WFUriFactory::Combine(String^ uriAsString, String^ relativeAsString, Settings^ settings)
{
	ComPtr<IUriRuntimeClassFactory> uriFactory;
	RoGetActivationFactory(HStringReference(RuntimeClass_Windows_Foundation_Uri).Get(), IID_IUriRuntimeClassFactory, &uriFactory);
	ComPtr<IUriRuntimeClass> uri;
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;
	ComPtr<IUriRuntimeClass> relative;
	pin_ptr<const wchar_t> relativeAsPinPtr = PtrToStringChars(relativeAsString);
	const wchar_t* relativeAsWChar = relativeAsPinPtr;
	List<UriProperty>^ properties = gcnew List<UriProperty>();

	HRESULT hr = uriFactory->CreateWithRelativeUri(HStringReference(uriAsWChar).Get(), HStringReference(relativeAsWChar).Get(), &uri);
	if (SUCCEEDED(hr))
	{
		properties = UriToProperties(uri.Get());
	}
	else
	{
		properties->Add(UriProperty("W.F.Uri.CreateWithRelativeUriFailure", String::Format("CreateWithRelativeUri failure: {0}", hr)));
	}

	return properties;
}

#define IF_FAILED_THROW(expr) { HRESULT hr = (expr); if (FAILED(hr)) throw gcnew Exception(String::Format("Property failure: {0}", hr)); }

List<UriProperty>^ WFUriFactory::UriToProperties(ABI::Windows::Foundation::IUriRuntimeClass* uri)
{
	List<UriProperty>^ properties = gcnew List<UriProperty>();
	HString prop;
	ComPtr<ABI::Windows::Foundation::IUriRuntimeClassWithAbsoluteCanonicalUri> uriMoreProperties;

	uri->QueryInterface(IID_PPV_ARGS(&uriMoreProperties));

	IF_FAILED_THROW(uri->get_AbsoluteUri(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.AbsoluteUri", gcnew String(prop.GetRawBuffer(nullptr))));
	
	boolean suspicious = false;
	IF_FAILED_THROW(uri->get_Suspicious(&suspicious));
	properties->Add(UriProperty("W.F.Uri.Suspicious", suspicious ? "True" : "False"));

	IF_FAILED_THROW(uri->get_DisplayUri(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.DisplayUri", gcnew String(prop.GetRawBuffer(nullptr))));
	
	IF_FAILED_THROW(uriMoreProperties->get_AbsoluteCanonicalUri(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.AbsoluteCanonicalUri", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uriMoreProperties->get_DisplayIri(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.DisplayIri", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_RawUri(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.RawUri", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Domain(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Domain", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Extension(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Extension", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Fragment(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Fragment", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Host(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Host", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Password(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Password", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_Path(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Path", gcnew String(prop.GetRawBuffer(nullptr))));

	INT32 port = 0;
	IF_FAILED_THROW(uri->get_Port(&port));
	properties->Add(UriProperty("W.F.Uri.Port", String::Format("{0}", port)));

	IF_FAILED_THROW(uri->get_Query(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.Query", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_SchemeName(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.SchemeName", gcnew String(prop.GetRawBuffer(nullptr))));

	IF_FAILED_THROW(uri->get_UserName(prop.GetAddressOf()));
	properties->Add(UriProperty("W.F.Uri.UserName", gcnew String(prop.GetRawBuffer(nullptr))));

	return properties;
}


