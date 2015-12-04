using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;

namespace UriApp
{
    public class CreateUriImports
    {
        [DllImport("urlmon.dll")]
        public static extern uint CreateUri(
            [MarshalAs(UnmanagedType.LPWStr)] string sURI,
            [MarshalAs(UnmanagedType.U4)] CreateUriFlags dwFlags,
            IntPtr dwReserved,
            [MarshalAs(UnmanagedType.IUnknown)] out object pURI);

        [DllImport("urlmon.dll")]
        public static extern uint CoInternetCombineIUri(
            [MarshalAs(UnmanagedType.IUnknown)] object uri,
            [MarshalAs(UnmanagedType.IUnknown)] object relative,
            [MarshalAs(UnmanagedType.U4)] CombineFlags flags,
            [MarshalAs(UnmanagedType.IUnknown)] out object result);

        public enum CombineFlags
        {
            URL_DONT_SIMPLIFY = 0x08000000
        }

        public enum CreateUriFlags
        {
            Uri_CREATE_ALLOW_RELATIVE = 0x0001,
            Uri_CREATE_ALLOW_IMPLICIT_WILDCARD_SCHEME =0x0002,
            Uri_CREATE_ALLOW_IMPLICIT_FILE_SCHEME =0x0004,
            Uri_CREATE_NOFRAG =0x0008,
            Uri_CREATE_NO_CANONICALIZE=0x0010,
            Uri_CREATE_FILE_USE_DOS_PATH =0x0020,
            Uri_CREATE_NO_DECODE_EXTRA_INFO =0x0080,
            Uri_CREATE_NO_CRACK_UNKNOWN_SCHEMES =0x0400,
            Uri_CREATE_NO_PRE_PROCESS_HTML_URI =0x1000,
            Uri_CREATE_IE_SETTINGS =0x2000,
            Uri_CREATE_NO_ENCODE_FORBIDDEN_CHARACTERS =0x8000,
            Uri_CREATE_NORMALIZE_INTL_CHARACTERS =0x00010000
        }
    }

    [ComImport]
    [Guid("A39EE748-6A27-4817-A6F2-13914BEF5890"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUri
    {
        [PreserveSig] UInt32 GetPropertyBSTR([In]CreateUriUriProperty uriProp, [Out]out string strProperty, [In]UInt32 dwFlags);
        [PreserveSig] UInt32 GetPropertyLength([In]CreateUriUriProperty uriProp, [Out] out UInt32 pcPropLen, [In]UInt32 dwFlags);
        [PreserveSig] UInt32 GetPropertyDWORD([In]CreateUriUriProperty uriProp, [Out] out UInt32 pcPropValue, [In]UInt32 dwFlags);
        UInt32 HasProperty([In]CreateUriUriProperty uriProp, [Out] out bool fHasProperty);
        [PreserveSig] UInt32 GetAbsoluteUri([MarshalAs(UnmanagedType.BStr)][Out] out string sAbsoluteUri);
        [PreserveSig] UInt32 GetAuthority([MarshalAs(UnmanagedType.BStr)][Out] out string sAuthority);
        [PreserveSig] UInt32 GetDisplayUri([MarshalAs(UnmanagedType.BStr)][Out] out string sDisplayString);
        [PreserveSig] UInt32 GetDomain([MarshalAs(UnmanagedType.BStr)][Out] out string sDomain);
        [PreserveSig] UInt32 GetExtension([MarshalAs(UnmanagedType.BStr)][Out] out string sExtension);
        [PreserveSig] UInt32 GetFragment([MarshalAs(UnmanagedType.BStr)][Out] out string sFragment);
        [PreserveSig] UInt32 GetHost([MarshalAs(UnmanagedType.BStr)][Out] out string sHost);
        [PreserveSig] UInt32 GetPassword([MarshalAs(UnmanagedType.BStr)][Out] out string sPassword);
        [PreserveSig] UInt32 GetPath([MarshalAs(UnmanagedType.BStr)][Out] out string sPath);
        [PreserveSig] UInt32 GetPathAndQuery([MarshalAs(UnmanagedType.BStr)][Out] out string sPathAndQuery);
        [PreserveSig] UInt32 GetQuery([MarshalAs(UnmanagedType.BStr)][Out] out string sQuery);
        [PreserveSig] UInt32 GetRawUri([MarshalAs(UnmanagedType.BStr)][Out] out string sRawUri);
        [PreserveSig] UInt32 GetSchemeName([MarshalAs(UnmanagedType.BStr)][Out] out string sSchemeName);
        [PreserveSig] UInt32 GetUserInfo([MarshalAs(UnmanagedType.BStr)][Out] out string sUserInfo);
        [PreserveSig] UInt32 GetUserName([MarshalAs(UnmanagedType.BStr)][Out] out string sUserName);
        [PreserveSig] UInt32 GetHostType([MarshalAs(UnmanagedType.U4)][Out] out uint dwHostType);
        [PreserveSig] UInt32 GetPort([MarshalAs(UnmanagedType.U4)][Out] out uint dwPort);
        [PreserveSig] UInt32 GetScheme([MarshalAs(UnmanagedType.U4)][Out] out uint dwScheme);
        [PreserveSig] UInt32 GetZone([MarshalAs(UnmanagedType.U4)][Out] out uint dwZone);
        [PreserveSig] UInt32 GetProperties([MarshalAs(UnmanagedType.U4)][Out] out uint dwFlags);
        UInt32 IsEqual([In]IUri pUri, [MarshalAs(UnmanagedType.U4)][Out] bool fEqual);
    }

    public enum CreateUriUriProperty
    {
        ABSOLUTE_URI = 0,
        STRING_START = ABSOLUTE_URI,
        AUTHORITY = 1,
        DISPLAY_URI = 2,
        DOMAIN = 3,
        EXTENSION = 4,
        FRAGMENT = 5,
        HOST = 6,
        PASSWORD = 7,
        PATH = 8,
        PATH_AND_QUERY = 9,
        QUERY = 10,
        RAW_URI = 11,
        SCHEME_NAME = 12,
        USER_INFO = 13,
        USER_NAME = 14,
        STRING_LAST = USER_NAME,
        HOST_TYPE = 15,
        DWORD_START = HOST_TYPE,
        PORT = 16,
        SCHEME = 17,
        ZONE = 18,
        DWORD_LAST = ZONE
    }

    class CreateUri : UriFactoryBase
    {
        private CreateUriImports.CreateUriFlags ConvertSettings(Settings settings)
        {
            CreateUriImports.CreateUriFlags flags = 0;

            flags |= settings.Get("Uri_CREATE_ALLOW_RELATIVE") ? CreateUriImports.CreateUriFlags.Uri_CREATE_ALLOW_RELATIVE : 0;
            flags |= settings.Get("Uri_CREATE_ALLOW_IMPLICIT_WILDCARD_SCHEME") ? CreateUriImports.CreateUriFlags.Uri_CREATE_ALLOW_IMPLICIT_WILDCARD_SCHEME : 0;
            flags |= settings.Get("Uri_CREATE_ALLOW_IMPLICIT_FILE_SCHEME") ? CreateUriImports.CreateUriFlags.Uri_CREATE_ALLOW_IMPLICIT_FILE_SCHEME : 0;
            flags |= settings.Get("Uri_CREATE_NOFRAG") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NOFRAG : 0;
            flags |= settings.Get("Uri_CREATE_NO_CANONICALIZE") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NO_CANONICALIZE : 0;
            flags |= settings.Get("Uri_CREATE_FILE_USE_DOS_PATH") ? CreateUriImports.CreateUriFlags.Uri_CREATE_FILE_USE_DOS_PATH : 0;
            flags |= settings.Get("Uri_CREATE_NO_DECODE_EXTRA_INFO") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NO_DECODE_EXTRA_INFO : 0;
            flags |= settings.Get("Uri_CREATE_NO_CRACK_UNKNOWN_SCHEMES") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NO_CRACK_UNKNOWN_SCHEMES : 0;
            flags |= settings.Get("Uri_CREATE_NO_PRE_PROCESS_HTML_URI") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NO_PRE_PROCESS_HTML_URI : 0;
            flags |= settings.Get("Uri_CREATE_IE_SETTINGS") ? CreateUriImports.CreateUriFlags.Uri_CREATE_IE_SETTINGS : 0;
            flags |= settings.Get("Uri_CREATE_NO_ENCODE_FORBIDDEN_CHARACTERS") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NO_ENCODE_FORBIDDEN_CHARACTERS : 0;
            flags |= settings.Get("Uri_CREATE_NORMALIZE_INTL_CHARACTERS") ? CreateUriImports.CreateUriFlags.Uri_CREATE_NORMALIZE_INTL_CHARACTERS : 0;

            return flags;
        }

        private delegate uint IUriMethod(out string str);
        private UriProperty ConvertMethod(IUriMethod uriMethod)
        {
            string str;
            uint result = uriMethod(out str);
            string propertyName = "IUri." + uriMethod.Method.Name.Substring(3);
            UriProperty property;
            if (result == 0)
            {
                property = new UriProperty(propertyName, str);
            }
            else if (result == 1)
            {
                property = new UriProperty(propertyName, null);
            }
            else
            {
                property = new UriProperty(propertyName, null, result.ToString());
            }
            return property;
        }

        private string HostTypeToName(uint hostType)
        {
            switch (hostType)
            {
                case 0:
                    return "Unknown";
                case 1:
                    return "Dns";
                case 2:
                    return "IPv4";
                case 3:
                    return "IPv6";
                case 4:
                    return "Idn";
                default:
                    return "Error";
            }
        }

        private List<UriProperty> ConvertIUri(IUri uri)
        {
            List<UriProperty> properties = new List<UriProperty>();
            properties.Add(ConvertMethod(uri.GetAbsoluteUri));
            properties.Add(ConvertMethod(uri.GetDisplayUri));
            properties.Add(ConvertMethod(uri.GetRawUri));
            properties.Add(ConvertMethod(uri.GetSchemeName));
            properties.Add(ConvertMethod(uri.GetAuthority));
            properties.Add(ConvertMethod(uri.GetUserInfo));
            properties.Add(ConvertMethod(uri.GetUserName));
            properties.Add(ConvertMethod(uri.GetPassword));
            properties.Add(ConvertMethod(uri.GetHost));
            uint port = 0;
            uri.GetPort(out port);
            properties.Add(new UriProperty("IUri.Port", String.Format("{0}", port)));

            uint hostType = 0;
            uri.GetHostType(out hostType);
            properties.Add(new UriProperty("IUri.HostType", HostTypeToName(hostType)));

            properties.Add(ConvertMethod(uri.GetDomain));
            properties.Add(ConvertMethod(uri.GetPath));
            properties.Add(ConvertMethod(uri.GetExtension));
            properties.Add(ConvertMethod(uri.GetPathAndQuery));
            properties.Add(ConvertMethod(uri.GetQuery));
            properties.Add(ConvertMethod(uri.GetFragment));
            return properties;
        }

        public List<UriProperty> Combine(string uriAsString, string relativeAsString, Settings settings)
        {
            CreateUriImports.CreateUriFlags flags = ConvertSettings(settings);
            List<UriProperty> properties = null;
            IUri uri;
            object uriAsObject;
            IUri relative;
            object relativeAsObject;
            IUri result;
            object resultAsObject;
            uint failure = 0;


            failure = CreateUriImports.CreateUri(uriAsString, flags, (IntPtr)0, out uriAsObject);
            if (failure == 0)
            {
                uri = (IUri)uriAsObject;

                failure = CreateUriImports.CreateUri(relativeAsString, flags | CreateUriImports.CreateUriFlags.Uri_CREATE_ALLOW_RELATIVE, (IntPtr)0, out relativeAsObject);
                if (failure == 0)
                {
                    relative = (IUri)relativeAsObject;

                    failure = CreateUriImports.CoInternetCombineIUri(uri, relative, (CreateUriImports.CombineFlags)0, out resultAsObject);
                    if (failure == 0)
                    {
                        result = (IUri)resultAsObject;
                        properties = ConvertIUri(result);
                    }
                }
            }

            if (properties == null)
            {
                properties = new List<UriProperty>();
                properties.Add(new UriProperty("IUri.CreationFailure", failure.ToString()));
            }

            return properties;
        }

        public List<UriProperty> Create(string uriAsString, Settings settings)
        {
            CreateUriImports.CreateUriFlags flags = ConvertSettings(settings);
            IUri uri;
            object uriAsObject;
            List<UriProperty> properties = null;

            uint failure = CreateUriImports.CreateUri(uriAsString, flags, (IntPtr)0, out uriAsObject);
            if (failure == 0)
            {
                uri = (IUri)uriAsObject;
                properties = ConvertIUri(uri);
            }

            if (properties == null)
            {
                properties = new List<UriProperty>();
                properties.Add(new UriProperty("IUri.CreationFailure", failure.ToString()));
            }

            return properties;
        }

        public Settings CreateEmptySettings()
        {
            Settings settings = new Settings();
            settings.Add(new Setting("Uri_CREATE_ALLOW_RELATIVE", false));
            settings.Add(new Setting("Uri_CREATE_ALLOW_IMPLICIT_WILDCARD_SCHEME", false));
            settings.Add(new Setting("Uri_CREATE_ALLOW_IMPLICIT_FILE_SCHEME", false));
            settings.Add(new Setting("Uri_CREATE_NOFRAG", false));
            settings.Add(new Setting("Uri_CREATE_NO_CANONICALIZE", false));
            settings.Add(new Setting("Uri_CREATE_FILE_USE_DOS_PATH", false));
            settings.Add(new Setting("Uri_CREATE_NO_DECODE_EXTRA_INFO", false));
            settings.Add(new Setting("Uri_CREATE_NO_CRACK_UNKNOWN_SCHEMES", false));
            settings.Add(new Setting("Uri_CREATE_NO_PRE_PROCESS_HTML_URI", false));
            settings.Add(new Setting("Uri_CREATE_IE_SETTINGS", false));
            settings.Add(new Setting("Uri_CREATE_NO_ENCODE_FORBIDDEN_CHARACTERS", false));
            settings.Add(new Setting("Uri_CREATE_NORMALIZE_INTL_CHARACTERS", false));

            return settings;
        }

        public string GetName()
        {
            return "IUri";
        }

        public string GetDescription() { return "The CreateUri and IUri URI parser from Win32 urlmon.dll"; }
    }
}
