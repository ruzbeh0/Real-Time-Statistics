using ColossalFramework.UI;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// UI to allow the user to select a range of years or dates to show
    /// </summary>
    public class ShowRange
    {
        // use singleton pattern:  there can be only one Show Range interface in the game
        private static readonly ShowRange _instance = new ShowRange();
        public static ShowRange instance { get { return _instance; } }
        private ShowRange() { }

        // initial values obtained from game save file
        private bool _initialValueShowAll;
        private float _initialValueFromSlider;
        private float _initialValueToSlider;

        // UI components that are referenced after they are created
        //private UILabel _showRangeLabel;
        //private UISprite _showAllRadio;
        //private UILabel _showAllLabel;
        //private UISprite _showFromToRadio;
        //private UILabel _showFromToLabel;
        private UISlider _fromSlider;
        private UISlider _toSlider;
        //private UILabel _fromLabel;
        //private UILabel _toLabel;

        // values for handling Real Time mod
        private bool _realTimeModEnabled;
        private DateTime _sliderBaseDate;

        /// <summary>
        /// initialize show range
        /// </summary>
        public void Initialize()
        {
            // with singleton pattern, all fields must be initialized or they will contain data from the previous game

            // initialize initial values
            _initialValueShowAll = true;
            _initialValueFromSlider = 0f;
            _initialValueToSlider = 0f;

            // clear all the UI components
            ClearUIComponents();

            // initialize for Real Time mod
            _realTimeModEnabled = ModUtil.IsWorkshopModEnabled(ModUtil.ModIDRealTime);
            _sliderBaseDate = DateTime.MinValue.Date;
        }

        /// <summary>
        /// deinitialize show Range
        /// </summary>
        public void Deinitialize()
        {
            // clear all the UI components
            ClearUIComponents();
        }

        /// <summary>
        /// clear UI components
        /// </summary>
        private void ClearUIComponents()
        {
            // clear all UI references
            //_showRangeLabel = null;
            //_showAllRadio = null;
            //_showAllLabel = null;
            //_showFromToRadio = null;
            //_showFromToLabel = null;
            _fromSlider = null;
            _toSlider = null;
            //_fromLabel = null;
            //_toLabel = null;
        }

        /// <summary>
        /// create the Show Range UI on the options panel
        /// </summary>
        public bool CreateUI(UIPanel optionsPanel, float left, Color32 textColor)
        {
            // create Show Range panel
            //UIPanel showRangePanel = optionsPanel.AddUIComponent<UIPanel>();
            //if (showRangePanel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show Range panel.");
            //    return false;
            //}
            //showRangePanel.name = "ShowRangePanel";
            //showRangePanel.autoSize = false;
            //showRangePanel.size = new Vector2(optionsPanel.size.x - left, optionsPanel.size.y);
            //showRangePanel.relativePosition = new Vector3(left, 0f);
            //
            //// create Show Range label
            //_showRangeLabel = showRangePanel.AddUIComponent<UILabel>();
            //if (_showRangeLabel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show Range label.");
            //    return false;
            //}
            //_showRangeLabel.name = "ShowRangeLabel";
            //_showRangeLabel.autoSize = false;
            //_showRangeLabel.size = new Vector2(showRangePanel.size.x, 15f);
            //_showRangeLabel.relativePosition = new Vector3(5f, 10f);
            //_showRangeLabel.textScale = 0.75f;
            //_showRangeLabel.textColor = textColor;

            // create Show All panel
            //UIPanel showAllPanel = showRangePanel.AddUIComponent<UIPanel>();
            //if (showAllPanel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show All panel.");
            //    return false;
            //}
            //showAllPanel.name = "ShowAllPanel";
            //showAllPanel.autoSize = false;
            //showAllPanel.size = new Vector2(80f, 15f);
            //showAllPanel.relativePosition = new Vector3(5f, _showRangeLabel.relativePosition.y + _showRangeLabel.size.y + 2f);
            //showAllPanel.eventClicked += ShowRangeRadioButton_eventClicked;

            // create Show All radio button
            //_showAllRadio = showAllPanel.AddUIComponent<UISprite>();
            //if (_showAllRadio == null)
            //{
            //    Debug.LogError($"Unable to create Show All radio button.");
            //    return false;
            //}
            //_showAllRadio.name = "ShowAllRadio";
            //_showAllRadio.autoSize = false;
            //_showAllRadio.size = new Vector2(15f, 15f);
            //_showAllRadio.relativePosition = new Vector3(0f, 0f);

            // create Show All label
            //_showAllLabel = showAllPanel.AddUIComponent<UILabel>();
            //if (_showAllLabel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show All label.");
            //    return false;
            //}
            //_showAllLabel.name = "ShowAllLabel";
            //_showAllLabel.autoSize = false;
            //_showAllLabel.size = new Vector2(showAllPanel.size.x - 17f, 15f);
            //_showAllLabel.relativePosition = new Vector3(17f, 2.5f);
            //_showAllLabel.textScale = 0.75f;
            //_showAllLabel.textColor = textColor;
            //
            //// create Show FromTo panel
            //UIPanel showFromToPanel = showRangePanel.AddUIComponent<UIPanel>();
            //if (showFromToPanel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show FromTo panel.");
            //    return false;
            //}
            //showFromToPanel.name = "ShowFromToPanel";
            //showFromToPanel.autoSize = false;
            //showFromToPanel.size = new Vector2(100f, 15f);
            //showFromToPanel.relativePosition = new Vector3(showAllPanel.relativePosition.x + showAllPanel.size.x + 10f, showAllPanel.relativePosition.y);
            //showFromToPanel.eventClicked += ShowRangeRadioButton_eventClicked;

            // create Show FromTo radio button
            //_showFromToRadio = showFromToPanel.AddUIComponent<UISprite>();
            //if (_showFromToRadio == null)
            //{
            //    Debug.LogError($"Unable to create Show FromTo radio button.");
            //    return false;
            //}
            //_showFromToRadio.name = "ShowFromToRadio";
            //_showFromToRadio.autoSize = false;
            //_showFromToRadio.size = new Vector2(15f, 15f);
            //_showFromToRadio.relativePosition = new Vector3(0f, 0f);

            // create Show FromTo label
            //_showFromToLabel = showFromToPanel.AddUIComponent<UILabel>();
            //if (_showFromToLabel == null)
            //{
            //    LogUtil.LogError($"Unable to create Show FromTo label.");
            //    return false;
            //}
            //_showFromToLabel.name = "ShowFromToLabel";
            //_showFromToLabel.autoSize = false;
            //_showFromToLabel.size = new Vector2(showFromToPanel.size.x - 17f, 15f);
            //_showFromToLabel.relativePosition = new Vector3(17f, 2.5f);
            //_showFromToLabel.textScale = 0.75f;
            //_showFromToLabel.textColor = textColor;
            //
            //// create From slider from template
            //UIPanel fromSliderPanel = showRangePanel.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsSliderTemplate")) as UIPanel;
            //if (fromSliderPanel == null)
            //{
            //    LogUtil.LogError($"Unable to attach From slider panel.");
            //    return false;
            //}
            //fromSliderPanel.autoSize = false;
            //float fromToLabelWidth = (_realTimeModEnabled ? 70f : 40f);
            //fromSliderPanel.size = new Vector2(showRangePanel.size.x - 10f - fromToLabelWidth, 15f);
            //fromSliderPanel.relativePosition = new Vector3(10f, showAllPanel.relativePosition.y + showAllPanel.size.y);

            // hide label from template
            //UILabel fromSliderLabel = fromSliderPanel.Find<UILabel>("Label");
            //if (fromSliderLabel == null)
            //{
            //    LogUtil.LogError($"Unable to find From label.");
            //    return false;
            //}
            //fromSliderLabel.isVisible = false;
            //
            //// get the From slider
            //_fromSlider = fromSliderPanel.Find<UISlider>("Slider");
            //if (_fromSlider == null)
            //{
            //    LogUtil.LogError($"Unable to find From slider.");
            //    return false;
            //}
            //_fromSlider.autoSize = false;
            //_fromSlider.size = new Vector2(fromSliderPanel.size.x, fromSliderPanel.size.y);
            //_fromSlider.relativePosition = new Vector3(0f, 0f);
            //_fromSlider.orientation = UIOrientation.Horizontal;
            //_fromSlider.stepSize = 1f;      // 1 year or 1 day
            //_fromSlider.scrollWheelAmount = (_realTimeModEnabled ? 30f : 5f);
            //_fromSlider.minValue = -3f;
            //_fromSlider.maxValue = -1f;
            //_fromSlider.value = -2f;

            // create From label
            //_fromLabel = showRangePanel.AddUIComponent<UILabel>();
            //if (_fromLabel == null)
            //{
            //    LogUtil.LogError($"Unable to create From label.");
            //    return false;
            //}
            //_fromLabel.name = "FromLabel";
            //_fromLabel.autoSize = false;
            //_fromLabel.size = new Vector2(fromToLabelWidth, fromSliderPanel.size.y);
            //_fromLabel.relativePosition = new Vector3(fromSliderPanel.relativePosition.x + fromSliderPanel.size.x + 5f, fromSliderPanel.relativePosition.y + 9f);
            //_fromLabel.text = (_realTimeModEnabled ? "00/00/0000 00:00" : "0000");
            //_fromLabel.textScale = 0.625f;
            //_fromLabel.textColor = textColor;
            //_fromLabel.textAlignment = UIHorizontalAlignment.Left;

            // create To slider from template
            //UIPanel toSliderPanel = showRangePanel.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsSliderTemplate")) as UIPanel;
            //if (toSliderPanel == null)
            //{
            //    LogUtil.LogError($"Unable to attach To slider.");
            //    return false;
            //}
            //toSliderPanel.autoSize = false;
            //toSliderPanel.size = new Vector2(fromSliderPanel.size.x, fromSliderPanel.size.y);
            //toSliderPanel.relativePosition = new Vector3(fromSliderPanel.relativePosition.x, fromSliderPanel.relativePosition.y + fromSliderPanel.size.y + 3f);
            //
            //// hide label from template
            //UILabel toSliderLabel = toSliderPanel.Find<UILabel>("Label");
            //if (toSliderLabel == null)
            //{
            //    LogUtil.LogError($"Unable to find To label.");
            //    return false;
            //}
            //toSliderLabel.isVisible = false;
            //
            //// get the To slider
            //_toSlider = toSliderPanel.Find<UISlider>("Slider");
            //if (_toSlider == null)
            //{
            //    LogUtil.LogError($"Unable to find To slider.");
            //    return false;
            //}
            //_toSlider.autoSize = false;
            //_toSlider.size = new Vector2(toSliderPanel.size.x, toSliderPanel.size.y);
            //_toSlider.relativePosition = new Vector3(0f, 0f);
            //_toSlider.orientation = UIOrientation.Horizontal;
            //_toSlider.stepSize = 1f;    // 1 year or 1 day
            //_toSlider.scrollWheelAmount = (_realTimeModEnabled ? 30f : 5f);
            //_toSlider.minValue = -3f;
            //_toSlider.maxValue = -1f;
            //_toSlider.value = -2f;

            // create To label
            //_toLabel = showRangePanel.AddUIComponent<UILabel>();
            //if (_toLabel == null)
            //{
            //    LogUtil.LogError($"Unable to create To label.");
            //    return false;
            //}
            //_toLabel.name = "ToLabel";
            //_toLabel.autoSize = false;
            //_toLabel.size = new Vector2(_fromLabel.size.x, toSliderPanel.size.y);
            //_toLabel.relativePosition = new Vector3(toSliderPanel.relativePosition.x + toSliderPanel.size.x + 5f, toSliderPanel.relativePosition.y + 9f);
            //_toLabel.text = (_realTimeModEnabled ? "00/00/0000 00:00" : "0000");
            //_toLabel.textScale = 0.625f;
            //_toLabel.textColor = textColor;
            //_toLabel.textAlignment = UIHorizontalAlignment.Left;

            // initialize slider min and max values
            // the slider clamps the value to be in the min/max range, so min/max must be initalized before the slider values
            //UpdatePanel();
            //
            //// set initial Show Range option values to the values previously read from the game save file
            //ShowAll = _initialValueShowAll;
            //_fromSlider.value = _initialValueFromSlider;
            //_toSlider.value = _initialValueToSlider;
            //
            //// initialize slider labels
            //UpdateSliderLabels();
            //
            //// now set slider event handlers
            //_fromSlider.eventValueChanged += FromSlider_eventValueChanged;
            //_toSlider.eventValueChanged += ToSlider_eventValueChanged;
            //
            //// initialize UI text
            //UpdateUIText();

            // success
            return true;
        }

        /// <summary>
        /// update UI text based on current language
        /// </summary>
        public void UpdateUIText()
        {
            // update labels
            //Translation translation = Translation.instance;
            //_showRangeLabel.text = translation.Get(_realTimeModEnabled ? Translation.Key.DatesToShow : Translation.Key.YearsToShow);
            //_showAllLabel.text = translation.Get(Translation.Key.All);
            //_showFromToLabel.text = translation.Get(Translation.Key.FromTo);
        }

        /// <summary>
        /// handle click on All or FromTo
        /// </summary>
        private void ShowRangeRadioButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // toggle radio buttons
            //ShowAll = !ShowAll;

            // update main panel
            //UserInterface.instance.UpdateMainPanel();
        }

        /// <summary>
        /// handle change in From slider
        /// </summary>
        private void FromSlider_eventValueChanged(UIComponent component, float value)
        {
            // update slider label
            //UpdateSliderLabel(_fromLabel, _fromSlider);
            //
            //// update main panel only if using the sliders
            //if (!ShowAll)
            //{
            //    UserInterface.instance.UpdateMainPanel();
            //}
        }

        /// <summary>
        /// handle change in To slider
        /// </summary>
        private void ToSlider_eventValueChanged(UIComponent component, float value)
        {
            // update slider label
            //UpdateSliderLabel(_toLabel, _toSlider);
            //
            //// update main panel only if using the sliders
            //if (!ShowAll)
            //{
            //    UserInterface.instance.UpdateMainPanel();
            //}
        }

        /// <summary>
        /// update the label associated with a slider
        /// </summary>
        private void UpdateSliderLabel(UILabel label, UISlider slider)
        {
            // make sure label and slider were created
            //if (label != null && slider != null)
            //{
            //    if (_realTimeModEnabled)
            //    {
            //        // compute and display date
            //        label.text = _sliderBaseDate.AddDays((int)slider.value).Date.ToString("dd/MM/yyyy HH:mm");
            //    }
            //    else
            //    {
            //        // display year
            //        label.text = ((int)slider.value).ToString();
            //    }
            //    label.Invalidate();
            //}
        }

        /// <summary>
        /// update labels on both sliders with the current value
        /// </summary>
        private void UpdateSliderLabels()
        {
            //UpdateSliderLabel(_fromLabel, _fromSlider);
            //UpdateSliderLabel(_toLabel, _toSlider);
        }

        /// <summary>
        /// update the Show Range panel
        /// </summary>
        public void UpdatePanel()
        {
            // no need to lock the thread here for working with snapshots because this routine is called from only 2 places:
            //      ShowRange.CreateUI in which case threading is not yet a concern
            //      MainPanel.UpdatePanel which has already locked the thread

            // get first and last snapshot dates
            DateTime firstDate;
            DateTime lastDate;
            Snapshots snapshotsInstance = Snapshots.instance;
            int snapshotsCount = snapshotsInstance.Count;
            if (snapshotsCount == 0)
            {
                // use current game date
                firstDate = lastDate = SimulationManager.instance.m_currentGameTime.Date;
            }
            else
            {
                // use first and last snapshot dates
                firstDate = snapshotsInstance[0].SnapshotDate;
                lastDate = snapshotsInstance[snapshotsCount - 1].SnapshotDate;
            }

            // copmpute new min and max values for the sliders
            float newMinValue;
            float newMaxValue;
            bool sliderBaseDateChanged;
            if (_realTimeModEnabled)
            {
                // the slider value represents the number of days from the base date
                sliderBaseDateChanged = (_sliderBaseDate != firstDate);
                _sliderBaseDate = firstDate;

                // sliders go from first date to last date, but make sure there is at least 1 day
                newMinValue = 0;
                newMaxValue = (lastDate == firstDate ? 1f : (float)((lastDate - firstDate).TotalDays));
            }
            else
            {
                // slider base date is not used, so not changed
                sliderBaseDateChanged = false;

                // compute first year and last year
                // if last date is January 1, then last year is that year
                // if last date is not Jan 1, then last year is the year after the last date
                int firstYear = firstDate.Year;
                int lastYear = lastDate.Year + (lastDate.Month == 1 && lastDate.Day == 1 ? 0 : 1);

                // sliders go from first year to last year, but make sure there is at least 1 year
                newMinValue = firstYear;
                newMaxValue = (lastYear == firstYear ? firstYear + 1 : lastYear);
            }

            // check if need to change slider min or max values
            // only need to check one slider because both sliders use the same min and max
            //if (_fromSlider.minValue != newMinValue || _fromSlider.maxValue != newMaxValue || sliderBaseDateChanged)
            //{
            //    // set min and max values on both sliders
            //    // setting the min and max values forces the slider value to be in the range
            //    _fromSlider.minValue = _toSlider.minValue = newMinValue;
            //    _fromSlider.maxValue = _toSlider.maxValue = newMaxValue;
            //
            //    // the UISlider does not automatically update the thumb when the min and max values are changed, so update the thumb explicitly
            //    MethodInfo updateValueIndicators = typeof(UISlider).GetMethod("UpdateValueIndicators", BindingFlags.NonPublic | BindingFlags.Instance);
            //    updateValueIndicators.Invoke(_fromSlider, new object[] { _fromSlider.value });
            //    updateValueIndicators.Invoke(_toSlider, new object[] { _toSlider.value });
            //
            //    // update slider labels
            //    UpdateSliderLabels();
            //}
        }

        /// <summary>
        /// the status of the Show All radio button
        /// </summary>
        public bool ShowAll
        {
            get
            {
                //return IsRadioButtonSelected(_showAllRadio);
                return true;
            }
            private set
            {
                // set both radio buttons
                //SetRadioButton(_showAllRadio, value);
                //SetRadioButton(_showFromToRadio, !value);
            }

        }

        /// <summary>
        /// get the from/to year/date from the slider
        /// </summary>
        public int FromYear { get { return (_fromSlider != null ? (int)_fromSlider.value : 0); } }
        public int ToYear   { get { return (_toSlider   != null ? (int)_toSlider.value   : 0); } }
        public DateTime FromDate { get { return (_fromSlider != null ? _sliderBaseDate.AddDays((int)_fromSlider.value) : DateTime.MinValue.Date); } }
        public DateTime ToDate   { get { return (_toSlider   != null ? _sliderBaseDate.AddDays((int)_toSlider.value  ) : DateTime.MaxValue.Date); } }

        /// <summary>
        /// set the radio button (i.e. sprite) status
        /// </summary>
        private void SetRadioButton(UISprite radioButton, bool value)
        {
            if (radioButton != null)
            {
                radioButton.spriteName = (value ? "check-checked" : "check-unchecked");
            }
        }

        /// <summary>
        /// return whether or not the radio button (i.e. sprite) is selected
        /// </summary>
        private bool IsRadioButtonSelected(UISprite radioButton)
        {
            if (radioButton != null)
            {
                return (radioButton.spriteName == "check-checked");
            }
            return false;
        }

        /// <summary>
        /// write the show range user selections to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // write current values; these will be used as initial values when the game is loaded
            writer.Write(ShowAll);
            writer.Write(_fromSlider != null ? _fromSlider.value : 0f);
            writer.Write(_toSlider   != null ? _toSlider.value   : 0f);
        }

        /// <summary>
        /// read the show range user selections from the game save file
        /// </summary>
        public void Deserialize(BinaryReader reader, int version)
        {
            // read initial values
            _initialValueShowAll    = reader.ReadBoolean();
            _initialValueFromSlider = reader.ReadSingle();
            _initialValueToSlider   = reader.ReadSingle();
        }
    }
}
