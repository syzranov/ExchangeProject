using System;
using System.Configuration;

namespace Exchange.Utils
{
    public class ConfigTools
    {
        public static readonly ConfigTools Instance = new ConfigTools();

        public T Get<T>(string propertyName)
        {
            return (T)Convert.ChangeType(ReadAppSettings(propertyName), typeof(T));
        }

        private object ReadAppSettings(string propertyName)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("[ReadAppSettings: {0}]", 
                        "AppSettings is empty Use GetSection command first."));
                }

                return appSettings[propertyName];
            }
            catch (ConfigurationErrorsException e)
            {
                throw new ConfigurationErrorsException(
                    string.Format("[ReadAppSettings: {0}]", e.ToString()));
            }
        }

    }
}
