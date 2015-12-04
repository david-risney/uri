#pragma once

namespace ICU
{
	public ref class ICUFactory : Common::UriFactoryBase
	{
	public:
		ICUFactory();

		virtual Common::Settings^ CreateEmptySettings();
		virtual System::String^ GetName();
		virtual System::String^ GetDescription();
		virtual System::Collections::Generic::List<Common::UriProperty>^ Create(System::String^ uri, Common::Settings^ settings);
		virtual System::Collections::Generic::List<Common::UriProperty>^ Combine(System::String^ uri, System::String^ relative, Common::Settings^ settings);

	private:
		System::Collections::Generic::List<Common::UriProperty>^ ComponentsToProperties(const wchar_t* url, URL_COMPONENTSW* components);
		DWORD SettingsToFlags(Common::Settings^ settings, bool forCrack);
	};

}