using ColossalFramework.UI;
using System;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// the button to activate the panel
    /// </summary>
    public class ActivationButton : UIButton
    {
        // default button position
        public const float DefaultPositionX = 80f;
        public const float DefaultPositionY = 40f;

        // UI components that are referenced after they are created
        private UIDragHandle _dragHandle;

        /// <summary>
        /// Start is called after the button is created
        /// set up the button
        /// </summary>
        public override void Start()
        {
            // do base processing
            base.Start();

            try
            {
                // set properties
                name = "RealTimeStatisticsActivationButton";
                opacity = 1f;
                size = new Vector2(38f, 38f);
                isVisible = true;
                text = "RTS";

                // set foreground and background images
                //normalFgSprite = "ThumbStatistics";
                scaleFactor = 0.7f;
                SetBackgroundImages(false);

                // attach drag handle
                _dragHandle = AddUIComponent<UIDragHandle>();
                if (_dragHandle == null)
                {
                    LogUtil.LogError($"Unable to create drag handle on [{name}].");
                    return;
                }
                _dragHandle.name = "DragHandle";
                _dragHandle.FitTo(this);

                // initialize UI text
                UpdateUIText();
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }

        /// <summary>
        /// adjust button background images based on main panel visibility
        /// </summary>
        public void SetBackgroundImages(bool mainPanelVisible)
        {
            normalBgSprite = focusedBgSprite = pressedBgSprite = (mainPanelVisible ? "RoundBackBigFocused" : "RoundBackBig");
            hoveredBgSprite = (mainPanelVisible ? "RoundBackBigFocused" : "RoundBackBigHovered");
        }

        /// <summary>
        /// update UI text for current language
        /// </summary>
        public void UpdateUIText()
        {
            _dragHandle.tooltip = Translation.instance.Get(Translation.Key.Title);
        }

        /// <summary>
        /// Update is called every frame
        /// </summary>
        public override void Update()
        {
            // do base processing
            base.Update();

            // if escape is pressed while there is no info view and no tool, then hide main panel
            if (InfoManager.exists && InfoManager.instance.CurrentMode == InfoManager.InfoMode.None)
            {
                if (ToolsModifierControl.toolController.CurrentTool.GetType() == typeof(DefaultTool))
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        UserInterface.instance.HideMainPanel();
                    }
                }
            }
        }
    }
}
