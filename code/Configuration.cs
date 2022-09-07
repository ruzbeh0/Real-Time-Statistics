using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// define global (i.e. for this mod but not game specific) configuration properties
    /// </summary>
    /// <remarks>convention for the config file name seems to be the mod name + "Config.xml"</remarks>
    [ConfigurationFileName("RealTimeStatisticsConfig.xml")]
    public class Configuration
    {
        // it is important to set default config values in case there is no config file

        // button position
        public float ButtonPositionX = ActivationButton.DefaultPositionX;
        public float ButtonPositionY = ActivationButton.DefaultPositionY;

        // panel position
        public float PanelPositionX = MainPanel.DefaultPositionX;
        public float PanelPositionY = MainPanel.DefaultPositionY;

        // language code
        public string LanguageCode = Options.GameLanguageCode;

        // current value update interval
        public int CurrentValueUpdateInterval = Options.DefaultUpdateInterval;

        /// <summary>
        /// save the button position to the global config file
        /// </summary>
        public static void SaveButtonPosition(Vector3 position)
        {
            Configuration config = ConfigurationUtil<Configuration>.Load();
            config.ButtonPositionX = position.x;
            config.ButtonPositionY = position.y;
            ConfigurationUtil<Configuration>.Save();
        }

        /// <summary>
        /// save the panel position to the global config file
        /// </summary>
        public static void SavePanelPosition(Vector3 position)
        {
            Configuration config = ConfigurationUtil<Configuration>.Load();
            config.PanelPositionX = position.x;
            config.PanelPositionY = position.y;
            ConfigurationUtil<Configuration>.Save();
        }

        /// <summary>
        /// save the language code to the global config file
        /// </summary>
        public static void SaveLanguageCode(string languageCode)
        {
            Configuration config = ConfigurationUtil<Configuration>.Load();
            config.LanguageCode = languageCode;
            ConfigurationUtil<Configuration>.Save();
        }

        /// <summary>
        /// save the current value update interval to the global config file
        /// </summary>
        public static void SaveCurrentValueUpdateInterval(int interval)
        {
            Configuration config = ConfigurationUtil<Configuration>.Load();
            config.CurrentValueUpdateInterval = interval;
            ConfigurationUtil<Configuration>.Save();
        }
    }
}