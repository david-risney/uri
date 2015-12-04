#pragma once

namespace SHUrl
{
	public ref class SHUrlFactory : Common::UriFactoryBase
	{
	public:
		SHUrlFactory();

		virtual Common::Settings^ CreateEmptySettings();
		virtual System::String^ GetName();
		virtual System::String^ GetDescription();
		virtual System::Collections::Generic::List<Common::UriProperty>^ Create(System::String^ uri, Common::Settings^ settings);
		virtual System::Collections::Generic::List<Common::UriProperty>^ Combine(System::String^ uri, System::String^ relative, Common::Settings^ settings);

	private:
		System::Collections::Generic::List<Common::UriProperty>^ UrlToProperties(const wchar_t* url, Common::Settings^ settings);
		void AddPropertyToList(const wchar_t* url, unsigned int part, const wchar_t* name, System::Collections::Generic::List<Common::UriProperty>^ list, Common::Settings^ settings);
		unsigned int SettingsToFlags(Common::Settings^ settings);
	};

}