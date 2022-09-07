using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using System.Reflection;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// handle Options
    /// </summary>
    public class Options
    {
        // use singleton pattern:  there can be only one Options in the game
        private static readonly Options _instance = new Options();
        public static Options instance { get { return _instance; } }
        private Options() { }

        // special language code for game language
        public const string GameLanguageCode = "00";

        // interval to update current values (seconds)
        public const int DefaultUpdateInterval = 10;
        public int CurrentValueUpdateInterval { get; private set; }
        //private UILabel _intervalValue;

        // status of settings check box
        public bool SaveSettingsAndSnapshots { get; private set; }

        /// <summary>
        /// create options UI
        /// </summary>
        public void CreateUI(UIHelperBase helper)
        {
            // create general options group
            Translation translation = Translation.instance;
            UIHelperBase groupGeneral = helper.AddGroup(translation.Get(Translation.Key.General));

            // construct list of supported language names, first entry is game language
            string[] supportedLanguageCodes = translation.SupportedLanguageCodes;
            string[] languageNames = new string[supportedLanguageCodes.Length + 1];
            languageNames[0] = translation.Get(Translation.Key.GameLanguage);
            for (int i = 0; i < supportedLanguageCodes.Length; i++)
            {
                // get each language name in its own language (i.e. ignore configured language)
                languageNames[i + 1] = translation.Get(Translation.Key.LanguageName, supportedLanguageCodes[i]);
            }

            // compute index of configured language
            int defaultIndex = 0;
            Configuration config = ConfigurationUtil<Configuration>.Load();
            string configuredLanguageCode = config.LanguageCode;
            if (configuredLanguageCode != GameLanguageCode)
            {
                for (int i = 0; i < supportedLanguageCodes.Length; i++)
                {
                    if (configuredLanguageCode == supportedLanguageCodes[i])
                    {
                        defaultIndex = i + 1;
                        break;
                    }
                }
            }

            // allow user to change language
            groupGeneral.AddDropdown(translation.Get(Translation.Key.ChooseYourLanguage), languageNames, defaultIndex, OnLanguageChanged);


            // allow user to set the interval that current values are updated
            //UISlider currentValueUpdateInterval = groupGeneral.AddSlider(translation.Get(Translation.Key.CurrentValueUpdateInterval), 1f, 30f, 1f, config.CurrentValueUpdateInterval, OnUpdateIntervalChanged) as UISlider;

            // adjust panel that holds label and slider for interval
            //UIPanel currentValueUpdateIntervalPanel = currentValueUpdateInterval.parent as UIPanel;
            //currentValueUpdateIntervalPanel.autoSize = false;
            //currentValueUpdateIntervalPanel.size = new Vector2(0.75f * currentValueUpdateIntervalPanel.parent.size.x, currentValueUpdateIntervalPanel.size.y);

            // adjust label for interval
            //UILabel currentValueUpdateIntervalLabel = currentValueUpdateIntervalPanel.Find<UILabel>("Label");
            //currentValueUpdateIntervalLabel.autoHeight = false;
            //currentValueUpdateIntervalLabel.autoSize = false;
            //currentValueUpdateIntervalLabel.size = new Vector2(currentValueUpdateIntervalPanel.size.x, 20f);

            // adjust actual slider for interval
            //UISlider currentValueUpdateIntervalSlider = currentValueUpdateIntervalPanel.Find<UISlider>("Slider");
            //currentValueUpdateIntervalSlider.autoSize = false;
            //currentValueUpdateIntervalSlider.size = new Vector2(currentValueUpdateIntervalPanel.size.x, currentValueUpdateIntervalSlider.size.y);

            // create new label to show the selected interval value to the right of the slider
            //_intervalValue = currentValueUpdateIntervalSlider.AddUIComponent<UILabel>();
            //_intervalValue.autoSize = false;
            //_intervalValue.size = new Vector2(30f, currentValueUpdateIntervalLabel.size.y);
            //_intervalValue.relativePosition = new Vector3(currentValueUpdateIntervalSlider.size.x + 10f, 0f);
            //_intervalValue.text = config.CurrentValueUpdateInterval.ToString();


            // create in-game options only when a game is loaded
            if (RTSLoading.IsGameLoaded)
            {
                // in-game options group
                UIHelperBase groupInGame = helper.AddGroup(translation.Get(Translation.Key.InGame));

                // allow user to export all or selected statistics
                groupInGame.AddButton(translation.Get(Translation.Key.ExportAllStatistics), OnExportAllStatisticsClicked);
                groupInGame.AddButton(translation.Get(Translation.Key.ExportSelectedStatistics), OnExportSelectedStatisticsClicked);

                // show export location to user
                UITextField exportLocation = groupInGame.AddTextfield(translation.Get(Translation.Key.ExportFile), Snapshots.instance.ExportPathFile, (string text) => { } ) as UITextField;
                exportLocation.readOnly = true;
                exportLocation.autoSize = false;
                exportLocation.size = new Vector2(exportLocation.parent.parent.size.x - 30f, 2f * exportLocation.size.y);
                exportLocation.multiline = true;

                // allow user to delete snapshots
                groupInGame.AddSpace(50);
                groupInGame.AddButton(translation.Get(Translation.Key.DeleteSnapshots), OnDeleteSnapshotsClicked);

                // allow user to NOT save settings and snapshots, default is to save
                SaveSettingsAndSnapshots = true;
                groupInGame.AddSpace(30);
                groupInGame.AddCheckbox(translation.Get(Translation.Key.SaveSettingsAndSnapshots), SaveSettingsAndSnapshots, OnSaveCheckChanged);
            }
        }

        /// <summary>
        /// handle user change in language
        /// </summary>
        private void OnLanguageChanged(int index)
        {
            // get the selected language code
            string languageCode;
            if (index == 0)
            {
                languageCode = GameLanguageCode;
            }
            else
            {
                languageCode = Translation.instance.SupportedLanguageCodes[index - 1];
            }

            // save the selected language code so it is available next time it is needed
            Configuration.SaveLanguageCode(languageCode);
            LogUtil.LogInfo($"Languaged changed to [{languageCode}]");

            // inform the Main Options Panel about locale change
            // this will trigger RealTimeStatistics.OnSettingsUI which calls Options.CreateUI to recreate this mod's Options UI with the newly selected language
            MethodInfo onLocaleChanged = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            if (onLocaleChanged != null)
            {
                OptionsMainPanel optionsMainPanel = UIView.library.Get<OptionsMainPanel>("OptionsPanel");
                if (optionsMainPanel != null)
                {
                    onLocaleChanged.Invoke(optionsMainPanel, new object[] { });
                }
            }

            // inform the Content Manager Panel about locale change
            onLocaleChanged = typeof(ContentManagerPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            if (onLocaleChanged != null)
            {
                ContentManagerPanel contentManagerPanel = UIView.library.Get<ContentManagerPanel>("ContentManagerPanel");
                if (contentManagerPanel != null)
                {
                    onLocaleChanged.Invoke(contentManagerPanel, new object[] { });
                }
            }

            // update UI for the rest of this mod's UI
            UserInterface.instance.UpdateUI();
        }

        /// <summary>
        /// handle change in update interval
        /// </summary>
        private void OnUpdateIntervalChanged(float val)
        {
            // save the new value
            CurrentValueUpdateInterval = (int)val;
            Configuration.SaveCurrentValueUpdateInterval(CurrentValueUpdateInterval);

            // show new interval value
            //if (_intervalValue != null)
            //{
            //    _intervalValue.text = CurrentValueUpdateInterval.ToString();
            //}
        }

        /// <summary>
        /// handle click on export all statistics
        /// </summary>
        private void OnExportAllStatisticsClicked()
        {
            // export all statistics
            Snapshots.instance.Export(Snapshots.StatisticsToExport.All);
        }

        /// <summary>
        /// handle click on export selected statistics
        /// </summary>
        private void OnExportSelectedStatisticsClicked()
        {
            // export selected statistics
            Snapshots.instance.Export(Snapshots.StatisticsToExport.Selected);
        }

        /// <summary>
        /// handle click on Delete Snapshots
        /// </summary>
        private void OnDeleteSnapshotsClicked()
        {
            // delete snapshots and update main panel
            Snapshots.instance.Clear();
            UserInterface.instance.UpdateMainPanel();
        }

        /// <summary>
        /// handle check change for Save
        /// </summary>
        private void OnSaveCheckChanged(bool isChecked)
        {
            // save check box status that will be looked at when saving the game
            SaveSettingsAndSnapshots = isChecked;
        }

        /// <summary>
        /// return the selected language code or the game's language code if game language is selected
        /// </summary>
        public string GetLanguageCode()
        {
            // check if should use game language
            Configuration config = ConfigurationUtil<Configuration>.Load();
            string configuredLanguageCode = config.LanguageCode;
            if (configuredLanguageCode == GameLanguageCode)
            {
                // use game language code
                if (LocaleManager.exists)
                {
                    return LocaleManager.instance.language;
                }
                else
                {
                    return Translation.DefaultLanguageCode;
                }
            }
            else
            {
                // use configured language code
                return configuredLanguageCode;
            }
        }
    }
}
