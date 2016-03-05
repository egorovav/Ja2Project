using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Configuration;

namespace LocalizerNameSpace
{
    public class Localizer
    {
        static Localizer()
        {

            if (File.Exists("Localizer.xml"))
            {
                try
                {
                    using(Stream dataStream = new FileStream("Localizer.xml", FileMode.Open, FileAccess.Read))
                        LoadLocalizer(dataStream);
                }
                catch
                {
                    using(Stream dataStream = new MemoryStream((byte[])ResourceManager.GetObject("Localizer")))
                        LoadLocalizer(dataStream);
                }
            }
            else
            {
                using(Stream dataStream = new MemoryStream((byte[])ResourceManager.GetObject("Localizer")))
                    LoadLocalizer(dataStream);
            }

        }

        private static void LoadLocalizer(Stream dataStream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Data[]));
            Data[] records = (Data[])serializer.Deserialize(dataStream);
            foreach (Data record in records)
            {
                foreach (LocalName localName in record.LocalNames)
                {
                    if (localName.language == BaseUsingConfig.CultureInfo && !localData.ContainsKey(record.name))
                    {
                        localData.Add(record.name, localName.name);
                        break;
                    }
                }
            }
        }

        static Hashtable localData = new Hashtable();
        static object lockFlag = new object();
        static ResourceManager resourceManager;
        /// <summary>
        /// Возвращает текущего менеджера ресурсов (ResourceManager)
        /// </summary>
        static ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    lock (lockFlag)
                    {
                        if (resourceManager == null)
                            resourceManager = new ResourceManager("Localizer.Resource", Assembly.GetExecutingAssembly());
                    }
                }
                return resourceManager;
            }
        }

        public static string GetString(string name)
        {
            if (!localData.ContainsKey(name))
                return name;
            return (string)localData[name];
        }
    }

    public class Data
    {
        [XmlAttribute]
        public string name;

        LocalName[] localNames;
        public LocalName[] LocalNames
        {
            get { return this.localNames; }
            set { this.localNames = value; }
        }
    }

    public class LocalName
    {
        public LocalName()
        {
        }

        [XmlAttribute]
        public string language;

        [XmlAttribute]
        public string name;
    }

    public class BaseUsingConfig
    {
        static Configuration _config;

        private static string cultureInfo;
        public static string CultureInfo
        {
            get
            {
                if (cultureInfo == null)
                {
                    try
                    {
                        KeyValueConfigurationElement cultureInfoObj = config.AppSettings.Settings["cultureInfo"];
                        if (cultureInfoObj != null)
                            cultureInfo = (string)cultureInfoObj.Value;
                        else
                        {
                            cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                            config.AppSettings.Settings.Add("cultureInfo", cultureInfo);
                        }
                    }
                    catch
                    {
                        cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    }
                }
                return cultureInfo;
            }
        }

        protected static System.Configuration.Configuration config
        {
            get
            {
                if (_config == null)
                    _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                return _config;
            }
        }
			
    }
}
