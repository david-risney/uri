#include "Stdafx.h"
#include "SHUrl.h"
#include <roapi.h>
#include <vcclr.h>
#include <Shlwapi.h>

using namespace SHUrl;
using namespace Common;
using namespace System;
using namespace System::Collections::Generic;

SHUrlFactory::SHUrlFactory()
{
}

Settings^ SHUrlFactory::CreateEmptySettings()
{
	Settings^ settings = gcnew Settings();
	settings->Add(Setting("URL_PARTFLAG_KEEPSCHEME", false));
	settings->Add(Setting("URL_DONT_SIMPLIFY", false));
	settings->Add(Setting("URL_ESCAPE_PERCENT", false));
	settings->Add(Setting("URL_ESCAPE_SPACES_ONLY", false));
	settings->Add(Setting("URL_ESCAPE_UNSAFE", false));
	settings->Add(Setting("URL_NO_META", false));
	settings->Add(Setting("URL_PLUGGABLE_PROTOCOL", false));
	settings->Add(Setting("URL_UNESCAPE", false));
	settings->Add(Setting("URL_ESCAPE_AS_UTF8", false));
	return settings;
}

String^ SHUrlFactory::GetName()
{
	return "Shlwapi";
}
String^ SHUrlFactory::GetDescription()
{
	return "Shlwapi URI parsing functions, UrlGetPart, UrlCombine, & UrlCanonicalize";
}

List<UriProperty>^ SHUrlFactory::Create(String^ uriAsString, Settings^ settings)
{
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;
	return UrlToProperties(uriAsWChar, settings);
}

List<UriProperty>^ SHUrlFactory::Combine(String^ uriAsString, String^ relativeAsString, Settings^ settings)
{
	pin_ptr<const wchar_t> uriAsPinPtr = PtrToStringChars(uriAsString);
	const wchar_t* uriAsWChar = uriAsPinPtr;
	pin_ptr<const wchar_t> relativeAsPinPtr = PtrToStringChars(relativeAsString);
	const wchar_t* relativeAsWChar = relativeAsPinPtr;
	wchar_t combined[2084] = L"";
	DWORD combinedLength = ARRAYSIZE(combined);

	HRESULT hr = UrlCombineW(uriAsWChar, relativeAsWChar, combined, &combinedLength, SettingsToFlags(settings));

	if (SUCCEEDED(hr))
	{
		return UrlToProperties(uriAsWChar, settings);
	}
	else
	{
		List<UriProperty>^ properties = gcnew List<UriProperty>();
		properties->Add(UriProperty("UrlCombineWError", String::Format("{0}", hr)));
		return properties;
	}	
}

List<UriProperty>^ SHUrlFactory::UrlToProperties(const wchar_t* url, Settings^ settings)
{
	List<UriProperty>^ properties = gcnew List<UriProperty>();
	wchar_t normalized[2084] = L"";
	DWORD normalizedLength = ARRAYSIZE(normalized);
	HRESULT hr = UrlCanonicalizeW(url, normalized, &normalizedLength, SettingsToFlags(settings));
	if (SUCCEEDED(hr))
	{
		properties->Add(UriProperty("SHUrl.OriginalUrl", gcnew String(url)));
		properties->Add(UriProperty("SHUrl.CanonicalizedUrl", gcnew String(normalized)));
		AddPropertyToList(url, URL_PART_HOSTNAME, L"SHUrl.HostName", properties, settings);
		AddPropertyToList(url, URL_PART_PASSWORD, L"SHUrl.Password", properties, settings);
		AddPropertyToList(url, URL_PART_PORT, L"SHUrl.Port", properties, settings);
		AddPropertyToList(url, URL_PART_QUERY, L"SHUrl.Query", properties, settings);
		AddPropertyToList(url, URL_PART_SCHEME, L"SHUrl.Scheme", properties, settings);
		AddPropertyToList(url, URL_PART_USERNAME, L"SHUrl.UserName", properties, settings);
	}
	else
	{
		properties->Add(UriProperty("UrlCanonicalizeWError", String::Format("{0}", hr)));
	}
	return properties;
}

void SHUrlFactory::AddPropertyToList(const wchar_t* url, unsigned int part, const wchar_t* name, List<UriProperty>^ list, Settings^ settings)
{
	wchar_t buffer[2084] = L"";
	DWORD bufferLength = ARRAYSIZE(buffer);

	HRESULT hr = UrlGetPartW(url, buffer, &bufferLength, part, SettingsToFlags(settings));
	if (SUCCEEDED(hr))
	{
		list->Add(UriProperty(gcnew String(name), gcnew String(buffer)));
	}
	else if (hr == E_INVALIDARG)
	{
		list->Add(UriProperty(gcnew String(name), nullptr));
	}
	else
	{
		list->Add(UriProperty("UrlGetPartWFailure", String::Format("{0}", hr)));
	}
}

unsigned int SHUrlFactory::SettingsToFlags(Common::Settings^ settings)
{
	unsigned int flags = 0;

	if (settings->Get("URL_PARTFLAG_KEEPSCHEME"))
	{
		flags |= URL_PARTFLAG_KEEPSCHEME;
	}
	if (settings->Get("URL_DONT_SIMPLIFY"))
	{
		flags |= URL_DONT_SIMPLIFY;
	}
	if (settings->Get("URL_ESCAPE_PERCENT"))
	{
		flags |= URL_ESCAPE_PERCENT;
	}
	if (settings->Get("URL_ESCAPE_SPACES_ONLY"))
	{
		flags |= URL_ESCAPE_SPACES_ONLY;
	}
	if (settings->Get("URL_ESCAPE_UNSAFE"))
	{
		flags |= URL_ESCAPE_UNSAFE;
	}
	if (settings->Get("URL_NO_META"))
	{
		flags |= URL_NO_META;
	}
	if (settings->Get("URL_PLUGGABLE_PROTOCOL"))
	{
		flags |= URL_PLUGGABLE_PROTOCOL;
	}
	if (settings->Get("URL_UNESCAPE"))
	{
		flags |= URL_UNESCAPE;
	}
	if (settings->Get("URL_ESCAPE_AS_UTF8"))
	{
		flags |= URL_ESCAPE_AS_UTF8;
	}

	return flags;
}