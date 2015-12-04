using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public struct UriProperty
    {
        public UriProperty(string name, string value)
        {
            this.name = name;
            this.value = value;
            this.error = null;
            this.exists = value != null;
        }

        public UriProperty(string name, string value, string error)
        {
            this.name = name;
            this.value = value;
            this.error = error;
            this.exists = value != null;
        }

        public string name;
        public string value;
        public bool exists;
        public string error;
    }

    public struct Setting
    {
        public Setting(string name, bool value)
        {
            this.name = name;
            this.value = value;
        }

        public string name;
        public bool value;
    }

    public class Settings
    {
        public Settings()
        {
            this.settings = new List<Setting>();
        }

        public void Add(Setting setting)
        {
            this.settings.Add(setting);
        }

        public bool Get(string name)
        {
            foreach (Setting setting in this.settings)
            {
                if (setting.name.Equals(name))
                {
                    return setting.value;
                }
            }
            return false;
        }

        public List<Setting> settings;
    }

    public interface UriFactoryBase
    {
        Settings CreateEmptySettings();
        string GetName();
        string GetDescription();
        List<UriProperty> Create(string uri, Settings settings);
        List<UriProperty> Combine(string uri, string relative, Settings settings);
    }
}
