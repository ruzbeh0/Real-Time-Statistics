using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// handle user interface functions for activation button and panel
    /// </summary>
    public class UserInterface
    {
        // use singleton pattern:  there can be only one MCS user interface in the game
        private static readonly UserInterface _instance = new UserInterface();
        public static UserInterface instance { get { return _instance; } }
        private UserInterface() { }

        // the UI elements that get added directly to the main view
        private ActivationButton _activationButton;
        private MainPanel _mainPanel;

        // remember last position of button
        private Vector3 _lastButtonPosition = Vector3.zero;

        /// <summary>
        /// initialize the user interface
        /// </summary>
        public bool Initialize()
        {
            try
            {
                // with singleton pattern, all fields must be initialized or they will contain data from the previous game

                // get the main view that holds most of the UI
                UIView uiView = UIView.GetAView();

                // create a new button on the main view
                // eventually the Start method will be called to complete the initialization
                _activationButton = (ActivationButton)uiView.AddUIComponent(typeof(ActivationButton));
                if (_activationButton == null)
                {
                    LogUtil.LogError($"Unable to create button on main view.");
                    return false;
                }

                // create a new main panel on the main view
                // eventually the Start method will be called to complete the initialization
                _mainPanel = (MainPanel)uiView.AddUIComponent(typeof(MainPanel));
                if (_mainPanel == null)
                {
                    LogUtil.LogError($"Unable to create panel on main view.");
                    return false;
                }

                // move the button and panel to initial position according to the config
                Configuration config = ConfigurationUtil<Configuration>.Load();
                _activationButton.relativePosition = new Vector3(config.ButtonPositionX, config.ButtonPositionY);
                _mainPanel.relativePosition = new Vector3(config.PanelPositionX, config.PanelPositionY);

                // initialize last button position to current button position
                _lastButtonPosition = _activationButton.relativePosition;

                // set event handlers
                _activationButton.eventMouseUp += ActivationButton_eventMouseUp;
                _activationButton.eventClicked += ActivationButton_eventClicked;
                LocaleManager.eventLocaleChanged += LocaleManager_eventLocaleChanged;

                // success
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// deinitialize user interface
        /// </summary>
        public void Deinitialize()
        {
            try
            {
                // remove event handler
                LocaleManager.eventLocaleChanged -= LocaleManager_eventLocaleChanged;

                // destroy objects that were added directly, this also destroys all contained objects
                // must do this explicitly because loading a saved game from the Pause Menu
                // does not destroy the objects implicitly like returning to the Main Menu to load a saved game
                if (_activationButton != null)
                {
                    UnityEngine.Object.Destroy(_activationButton);
                    _activationButton = null;
                }
                if (_mainPanel != null)
                {
                    UnityEngine.Object.Destroy(_mainPanel);
                    _mainPanel = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }

        /// <summary>
        /// handle activation button move
        /// </summary>
        private void ActivationButton_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            // save position
            Configuration.SaveButtonPosition(_activationButton.relativePosition);
        }

        /// <summary>
        /// handle click on activation button
        /// </summary>
        private void ActivationButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // check if button moved by more than a little bit
            if (Vector3.Distance(_activationButton.relativePosition, _lastButtonPosition) > Vector3.kEpsilon)
            {
                // button moved due to drag, ignore clicked event
                _lastButtonPosition = _activationButton.relativePosition;
                return;
            }

            // toggle panel visibility
            ToggleMainPanelVisibility();
            eventParam.Use();
        }

        /// <summary>
        /// handle change in locale (language)
        /// </summary>
        private void LocaleManager_eventLocaleChanged()
        {
            UpdateUI();
        }

        /// <summary>
        /// update UI when language changes
        /// </summary>
        public void UpdateUI()
        {
            // update the button
            if (_activationButton != null)
            {
                _activationButton.UpdateUIText();
            }

            // update the main panel
            if (_mainPanel != null)
            {
                _mainPanel.UpdateUIText();
                _mainPanel.UpdateStatisticAmounts();
                _mainPanel.UpdatePanel();
            }
        }

        /// <summary>
        /// hide the main panel if it is displayed
        /// </summary>
        public void HideMainPanel()
        {
            if (_mainPanel != null && _mainPanel.isVisible)
            {
                ToggleMainPanelVisibility();
            }
        }

        /// <summary>
        /// toggle main panel visibility
        /// </summary>
        private void ToggleMainPanelVisibility()
        {
            if (_mainPanel != null)
            {
                // toggle panel visibility
                _mainPanel.isVisible = !_mainPanel.isVisible;

                // adjust button background images based on panel visibility
                _activationButton.SetBackgroundImages(_mainPanel.isVisible);

                // if panel was just made visible, then bring to front and update it
                if (_mainPanel.isVisible)
                {
                    _mainPanel.BringToFront();
                    UpdateMainPanel();
                }
            }
        }

        /// <summary>
        /// update the main panel
        /// </summary>
        public void UpdateMainPanel()
        {
            if (_mainPanel != null)
            {
                _mainPanel.UpdatePanel();
            }
        }
    }
}
