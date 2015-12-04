#include <windows.foundation.h>
#pragma once

namespace WFUri
{
	public ref class WFUriFactory : Common::UriFactoryBase
	{
	public:
		virtual Common::Settings^ CreateEmptySettings();
		virtual System::String^ GetName();
		virtual System::String^ GetDescription();
		virtual System::Collections::Generic::List<Common::UriProperty>^ Create(System::String^ uri, Common::Settings^ settings);
		virtual System::Collections::Generic::List<Common::UriProperty>^ Combine(System::String^ uri, System::String^ relative, Common::Settings^ settings);

	private:
		System::Collections::Generic::List<Common::UriProperty>^ UriToProperties(ABI::Windows::Foundation::IUriRuntimeClass* uri);
	};
}
