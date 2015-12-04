using System;
using System.Collections.Generic;
using Common;

namespace UriApp
{
    class DotNetUri : UriFactoryBase
    {
        public Settings CreateEmptySettings()
        {
            Settings settings = new Settings();
            settings.Add(new Setting("AllowRelative", false));
            return settings;
        }

        public string GetName() { return "System.Uri"; }
        public string GetDescription() { return "The System.Uri URI parser from .NET"; }

        private List<UriProperty> GetProperties(Uri uri)
        {
            List<UriProperty> properties = new List<UriProperty>();
            properties.Add(new UriProperty("S.Uri.AbsoluteUri", uri.AbsoluteUri));
            properties.Add(new UriProperty("S.Uri.OriginalString", uri.OriginalString));
            properties.Add(new UriProperty("S.Uri.Scheme", uri.Scheme));
            properties.Add(new UriProperty("S.Uri.Authority", uri.Authority));
            properties.Add(new UriProperty("S.Uri.UserInfo", uri.UserInfo));
            properties.Add(new UriProperty("S.Uri.Host", uri.Host));
            properties.Add(new UriProperty("S.Uri.HostNameType", uri.HostNameType.ToString()));
            properties.Add(new UriProperty("S.Uri.DnsSafeHost", uri.DnsSafeHost));
            properties.Add(new UriProperty("S.Uri.Port", uri.Port.ToString()));
            properties.Add(new UriProperty("S.Uri.AbsolutePath", uri.AbsolutePath));
            properties.Add(new UriProperty("S.Uri.LocalPath", uri.LocalPath));
            properties.Add(new UriProperty("S.Uri.PathAndQuery", uri.PathAndQuery));
            properties.Add(new UriProperty("S.Uri.Query", uri.Query));
            properties.Add(new UriProperty("S.Uri.Fragment", uri.Fragment));
            return properties;
        }

        public List<UriProperty> Create(string uriAsString, Settings settings)
        {
            List<UriProperty> properties;
            try
            {
                Uri uri = new Uri(uriAsString, settings.Get("AllowRelative") ? UriKind.Absolute : UriKind.RelativeOrAbsolute);
                properties = GetProperties(uri);
            }
            catch (Exception e)
            {
                properties = new List<UriProperty>();
                properties.Add(new UriProperty("S.Uri.CreationFailure", e.Message));
            }
            return properties;
        }

        public List<UriProperty> Combine(string uriAsString, string relativeAsString, Settings settings)
        {
            List<UriProperty> properties;
            try
            {
                Uri uri = new Uri(uriAsString);
                Uri relative = new Uri(relativeAsString, UriKind.RelativeOrAbsolute);
                properties = GetProperties(new Uri(uri, relative));
            }
            catch (Exception e)
            {
                properties = new List<UriProperty>();
                properties.Add(new UriProperty("S.Uri.CreationFailure", e.Message));
            }
            return properties;
        }
    }

}
