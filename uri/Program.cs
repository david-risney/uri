using System;
using System.Collections.Generic;
using Common;

namespace UriApp
{
    class CommandLineSettings
    {
        public Settings settings = new Settings();
        public string uriAsString = null;
        public string relativeAsString = null;
        public bool groupProperties = false;
        public bool showMissing = false;
        public bool encodeNonAscii = false;
        public List<string> factoryNames = new List<string>();

        private static bool MatchArg(string arg, string match, string matchShort)
        {
            return arg.ToLower().Equals(match.ToLower()) ||
                arg.ToLower().Equals("-" + match.ToLower()) ||
                arg.ToLower().Equals(matchShort.ToLower()) ||
                arg.ToLower().Equals("-" + matchShort.ToLower());
        }

        public static CommandLineSettings Parse(string[] args)
        {
            CommandLineSettings commandLineSettings = new CommandLineSettings();

            for (uint argsIdx = 0; argsIdx < args.Length;)
            {
                if (MatchArg(args[argsIdx], "settings", "s"))
                {
                    if (argsIdx + 1 >= args.Length)
                    {
                        throw new FormatException("-settings must be followed by a comma delimited list of bool settings to turn on.");
                    }
                    string settingsFull = args[++argsIdx];
                    ++argsIdx;
                    foreach (string setting in settingsFull.Split(','))
                    {
                        commandLineSettings.settings.Add(new Setting(setting, true));
                    }
                }
                else if (MatchArg(args[argsIdx], "factories", "f"))
                {
                    if (argsIdx + 1 >= args.Length)
                    {
                        throw new FormatException("-factories must be followed by a delimited list of factories.");
                    }
                    commandLineSettings.factoryNames.AddRange(args[++argsIdx].Split(','));
                    ++argsIdx;
                }
                else if (MatchArg(args[argsIdx], "create", "c"))
                {
                    if (argsIdx + 1 >= args.Length)
                    {
                        throw new FormatException("-create must be followed by a URI to create.");
                    }
                    if (commandLineSettings.uriAsString != null)
                    {
                        throw new FormatException("Only one use of -create or -combine allowed.");
                    }
                    string uriAsString = args[++argsIdx];
                    ++argsIdx;
                    commandLineSettings.uriAsString = uriAsString;
                }
                else if (MatchArg(args[argsIdx], "combine", "r"))
                {
                    if (argsIdx + 2 >= args.Length)
                    {
                        throw new FormatException("-combine must be followed by a base URI and a relative URI to combine.");
                    }
                    if (commandLineSettings.uriAsString != null)
                    {
                        throw new FormatException("Only one use of -create or -combine allowed.");
                    }
                    string uriAsString = args[++argsIdx];
                    string relativeAsString = args[++argsIdx];
                    ++argsIdx;
                    commandLineSettings.uriAsString = uriAsString;
                    commandLineSettings.relativeAsString = relativeAsString;
                }
                else if (MatchArg(args[argsIdx], "groupByValue", "g"))
                {
                    ++argsIdx;
                    commandLineSettings.groupProperties = true;
                }
                else if (MatchArg(args[argsIdx], "showMissing", "m"))
                {
                    ++argsIdx;
                    commandLineSettings.showMissing = true;
                }
                else if (MatchArg(args[argsIdx], "encodeNonAscii", "e"))
                {
                    ++argsIdx;
                    commandLineSettings.encodeNonAscii = true;
                }
                else if (MatchArg(args[argsIdx], "fixCodePage", "f"))
                {
                    ++argsIdx;
                    Console.InputEncoding = (new System.Text.UTF8Encoding());
                    Console.OutputEncoding = (new System.Text.UTF8Encoding());
                }
                else
                {
                    throw new FormatException("Unknown parameter: " + args[argsIdx]);
                }
            }
            if (commandLineSettings.uriAsString == null)
            {
                throw new FormatException("Must specify -create or -combine.");
            }
            if (commandLineSettings.encodeNonAscii)
            {
                commandLineSettings.uriAsString = Encoder.Decode(commandLineSettings.uriAsString);
                if (commandLineSettings.relativeAsString != null)
                {
                    commandLineSettings.relativeAsString = Encoder.Decode(commandLineSettings.relativeAsString);
                }
            }

            return commandLineSettings;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run(args);
        }

        private CommandLineSettings commandLineSettings;
        private List<UriFactoryBase> allFactories;

        private Program()
        {
            this.allFactories = new List<UriFactoryBase>();

            this.allFactories.Add(new DotNetUri());
            this.allFactories.Add(new CreateUri());
            this.allFactories.Add(new WFUri.WFUriFactory());
            this.allFactories.Add(new ICU.ICUFactory());
            this.allFactories.Add(new SHUrl.SHUrlFactory());
        }

        private void Run(string[] args)
        {
            try
            {
                this.commandLineSettings = CommandLineSettings.Parse(args);
                List<UriFactoryBase> factories = new List<UriFactoryBase>();

                foreach (UriFactoryBase factory in this.allFactories)
                {
                    if (commandLineSettings.factoryNames.Count == 0 || 
                        commandLineSettings.factoryNames.Contains(factory.GetName()))
                    {
                        factories.Add(factory);   
                    }
                }

                List<UriProperty> allProperties = new List<UriProperty>();

                foreach (UriFactoryBase factory in factories)
                {
                    if (this.commandLineSettings.relativeAsString == null)
                    {
                        List<UriProperty> properties = factory.Create(this.commandLineSettings.uriAsString, this.commandLineSettings.settings);
                        allProperties.AddRange(properties);
                    }
                    else
                    {
                        List<UriProperty> properties = factory.Combine(this.commandLineSettings.uriAsString, this.commandLineSettings.relativeAsString, this.commandLineSettings.settings);
                        allProperties.AddRange(properties);
                    }
                }

                if (!this.commandLineSettings.groupProperties)
                {
                    DisplayProperties(allProperties);
                }
                else
                {
                    DisplayGroupedProperties(allProperties);
                }
            }
            catch (FormatException error)
            {
                Console.WriteLine("uri.exe - Demonstrate the results of different Windows URI parsers.\n");
                if (error.Message != null && error.Message.Length != 0)
                {
                    Console.WriteLine("Error: " + error.Message + "\n");
                }
                Console.WriteLine("Usage:\n" +
                    "\turi.exe -create <uri> [options]\n" +
                    "\turi.exe -combine <base> <relative> [options]\n" +
                    "\n" +
                    "\tOptions:\n" +
                    "\t\t-groupByValue - Group properties that have matching values\n" +
                    "\t\t-showMissing - Show properties that are empty or missing\n" +
                    "\t\t-encodeNonAscii - Encode and decode the sequence \\uABCD as Unicode character U+ABCD for non-ASCII characters\n" +
                    "\t\t-settings <setting name>,<setting name>,... - Turn on some per URI parser settings. All default to off.\n" +
                    "\t\t-factories <factory name>,<factory name>,... - Turn on specific URI parsers. The default is to use all.\n" +
                    "\n");

                Console.WriteLine("\tURI Factories:");
                foreach (UriFactoryBase factory in this.allFactories)
                {
                    Console.WriteLine("\t\t" + factory.GetName() + " - " + factory.GetDescription());
                }
                Console.WriteLine("");

                foreach (UriFactoryBase factory in this.allFactories)
                {
                    Settings settings = factory.CreateEmptySettings();
                    if (settings.settings.Count > 0)
                    {
                        Console.WriteLine("\t" + factory.GetName() + " settings:");
                        foreach (Setting setting in settings.settings)
                        {
                            Console.WriteLine("\t\t" + setting.name);
                        }
                    }
                }
            }
        }

        private void DisplayGroupedProperties(List<UriProperty> properties)
        {
            Dictionary<string, List<UriProperty>> valueToGroupedProperties = new Dictionary<string, List<UriProperty>>();
            foreach (UriProperty property in properties)
            {
                List<UriProperty> names;
                if (!valueToGroupedProperties.TryGetValue("" + property.value, out names))
                {
                    names = new List<UriProperty>();
                    valueToGroupedProperties.Add("" + property.value, names);
                }
                names.Add(property);
            }
            List<string> values = new List<string>();
            values.AddRange(valueToGroupedProperties.Keys);
            values.Sort();

            foreach (string value in values)
            {
                List<UriProperty> groupedProperties = valueToGroupedProperties[value];
                foreach (UriProperty property in groupedProperties)
                {
                    DisplayProperty(property);
                }
                Console.WriteLine();
            }
        }

        private void DisplayProperties(List<UriProperty> properties)
        {
            foreach (UriProperty property in properties)
            {
                DisplayProperty(property);
            }
            Console.WriteLine();
        }

        private void DisplayProperty(UriProperty property)
        {
            if ((property.exists && property.value.Length > 0) || this.commandLineSettings.showMissing)
            {
                Console.Write(property.name + " = ");
                string value = "none";
                if (property.exists)
                {
                    value = property.value;
                    if (this.commandLineSettings.encodeNonAscii)
                    {
                        value = Encoder.Encode(value);
                    }
                    value = "<" + value + ">";
                }
                Console.WriteLine(value);
            }
        }
    }
}
