#include "stdafx.h"

#include <winstring.h>
#include <roapi.h>
#include <vcclr.h>
#include <WinInet.h>
#include "ICU.h"

#undef _MANAGED // Lucky this works. Not advisable.
#include <wrl.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;
using namespace ABI::Windows::Foundation;
using namespace ICU;
using namespace Common;

ICUFactory::ICUFactory()
{
}

Settings^ ICUFactory::CreateEmptySettings()
{
	Settings^ settings = gcnew Settings();

	settings->Add(Setting("ICU_DECODE", false));
	settings->Add(Setting("ICU_ESCAPE", false));
	settings->Add(Setting("ICU_BROWSER_MODE", false));
	settings->Add(Setting("ICU_ENCODE_PERCENT", false));
	settings->Add(Setting("ICU_ENCODE_SPACES_ONLY", false));
	settings->Add(Setting("ICU_NO_ENCODE", false));
	settings->Add(Setting("ICU_NO_META", false));

	return settings;
}

String^ ICUFactory::GetName()
{
	return "InternetCrackUrl";
}

String^ ICUFactory::GetDescription()
{
	return "Wininet URL parsing functions.";
}

List<UriProperty>^ ICUFactory::Create(String^ uriAsString, Settings^ settings)
{
	List<UriProperty>^ properties = gcnew List<UriProperty>();
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;

	static const size_t bufferSize = 2084;
	URL_COMPONENTSW components;
	components.dwStructSize = sizeof(components);
	components.lpszExtraInfo = new wchar_t[bufferSize];
	components.dwExtraInfoLength = bufferSize;
	components.lpszHostName = new wchar_t[bufferSize];
	components.dwHostNameLength = bufferSize;
	components.lpszPassword = new wchar_t[bufferSize];
	components.dwPasswordLength = bufferSize;
	components.lpszScheme = new wchar_t[bufferSize];
	components.dwSchemeLength = bufferSize;
	components.lpszUrlPath = new wchar_t[bufferSize];
	components.dwUrlPathLength = bufferSize;
	components.lpszUserName = new wchar_t[bufferSize];
	components.dwUserNameLength = bufferSize;

	BOOL result = InternetCrackUrlW(uriAsWChar, uriAsString->Length, SettingsToFlags(settings, true), &components);

	if (result)
	{
		wchar_t canonicalizedUrl[2084] = L"";
		DWORD canonicalizedUrlLength = ARRAYSIZE(canonicalizedUrl);
		result = InternetCanonicalizeUrlW(uriAsWChar, canonicalizedUrl, &canonicalizedUrlLength, SettingsToFlags(settings, false));
		if (result)
		{
			properties = ComponentsToProperties(canonicalizedUrl, &components);
		}
		else
		{
			properties->Add(UriProperty("InternetCanonicalizeUrlFailure", String::Format("{0}", GetLastError())));
		}
	}
	else
	{
		properties->Add(UriProperty("InternetCrackUrlFailure", String::Format("{0}", GetLastError())));
	}

	return properties;
}

List<UriProperty>^ ICUFactory::Combine(String^ uriAsString, String^ relativeAsString, Settings^ settings)
{
	List<UriProperty>^ properties = gcnew List<UriProperty>();
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;
	pin_ptr<const wchar_t> relativeAsPinPtr = PtrToStringChars(relativeAsString);
	const wchar_t* relativeAsWChar = relativeAsPinPtr;

	wchar_t combined[2084] = L"";
	DWORD combinedLength = ARRAYSIZE(combined);
	BOOL result = InternetCombineUrlW(uriAsWChar, relativeAsWChar, combined, &combinedLength, SettingsToFlags(settings, false));

	if (result)
	{
		static const size_t bufferSize = 2084;
		URL_COMPONENTSW components;
		components.dwStructSize = sizeof(components);
		components.lpszExtraInfo = new wchar_t[bufferSize];
		components.dwExtraInfoLength = bufferSize;
		components.lpszHostName = new wchar_t[bufferSize];
		components.dwHostNameLength = bufferSize;
		components.lpszPassword = new wchar_t[bufferSize];
		components.dwPasswordLength = bufferSize;
		components.lpszScheme = new wchar_t[bufferSize];
		components.dwSchemeLength = bufferSize;
		components.lpszUrlPath = new wchar_t[bufferSize];
		components.dwUrlPathLength = bufferSize;
		components.lpszUserName = new wchar_t[bufferSize];
		components.dwUserNameLength = bufferSize;

		BOOL result = InternetCrackUrlW(combined, combinedLength, SettingsToFlags(settings, true), &components);

		if (result)
		{
			properties = ComponentsToProperties(combined, &components);
		}
		else
		{
			properties->Add(UriProperty("InternetCrackUrlFailure", String::Format("{0}", GetLastError())));
		}
	}
	else
	{
		properties->Add(UriProperty("InternetCombineUrlFailure", String::Format("{0}", GetLastError())));
	}
	return properties;
}

List<UriProperty>^ ICUFactory::ComponentsToProperties(const wchar_t* url, URL_COMPONENTSW* components)
{
	List<UriProperty>^ properties = gcnew List<UriProperty>();
	properties->Add(UriProperty("ICU.Url", gcnew String(url)));
	properties->Add(UriProperty("ICU.ExtraInfo", gcnew String(components->lpszExtraInfo)));
	properties->Add(UriProperty("ICU.HostName", gcnew String(components->lpszHostName)));
	properties->Add(UriProperty("ICU.Password", gcnew String(components->lpszPassword)));
	properties->Add(UriProperty("ICU.Scheme", gcnew String(components->lpszScheme)));
	properties->Add(UriProperty("ICU.Path", gcnew String(components->lpszUrlPath)));
	properties->Add(UriProperty("ICU.UserName", gcnew String(components->lpszUserName)));
	properties->Add(UriProperty("ICU.Port", String::Format("{0}", components->nPort)));
	return properties;
}

DWORD ICUFactory::SettingsToFlags(Common::Settings^ settings, bool forCrack)
{
	DWORD flags = 0;
	// crack & combine & canon
	if (settings->Get("ICU_DECODE"))
	{
		flags |= ICU_DECODE;
	}
	if (settings->Get("ICU_ESCAPE"))
	{
		flags |= ICU_ESCAPE;
	}

	if (!forCrack)
	{
		// Combine & Canonicalize
		if (settings->Get("ICU_BROWSER_MODE"))
		{
			flags |= ICU_BROWSER_MODE;
		}
		if (settings->Get("ICU_ENCODE_PERCENT"))
		{
			flags |= ICU_ENCODE_PERCENT;
		}
		if (settings->Get("ICU_ENCODE_SPACES_ONLY"))
		{
			flags |= ICU_ENCODE_SPACES_ONLY;
		}
		if (settings->Get("ICU_NO_ENCODE"))
		{
			flags |= ICU_NO_ENCODE;
		}
		if (settings->Get("ICU_NO_META"))
		{
			flags |= ICU_NO_META;
		}
	}
	return flags;
}
