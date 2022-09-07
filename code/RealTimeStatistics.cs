using ColossalFramework.Globalization;
using ICities;

namespace RealTimeStatistics
{
    public class RealTimeStatistics : IUserMod
    {
        // required name and description of this mod, always in the game language (not the language selected in Options)
        public string Name { get { return Translation.instance.Get(Translation.Key.Title, (LocaleManager.exists ? LocaleManager.instance.language : Options.instance.GetLanguageCode())); } }
        public string Description { get { return Translation.instance.Get(Translation.Key.Description, (LocaleManager.exists ? LocaleManager.instance.language : Options.instance.GetLanguageCode())); } }

        /// <summary>
        /// user settings
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // create options UI
            Options.instance.CreateUI(helper);
        }
    }
}