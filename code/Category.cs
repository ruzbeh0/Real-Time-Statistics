using ColossalFramework.UI;
using System.IO;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// properties and UI elements for one category
    /// </summary>
    public class Category
    {
        // constants
        private const float UIHeight = 17f;

        // category type
        public enum CategoryType
        {
            Electricity,
            Water,
            WaterTank,
            Sewage,
            Landfill,
            Garbage,
            Education,
            EducationLevel,
            Happiness,
            Healthcare,
            Deathcare,
            Childcare,
            Eldercare,
            Zoning,
            ZoneLevel,
            ZoneBuildings,
            ZoneDemand,
            Traffic,
            Pollution,
            FireSafety,
            Crime,
            PublicTransportation,
            Population,
            Households,
            Employment,
            OutsideConnections,
            LandValue,
            NaturalResources,
            Heating,
            Tourism,
            Tours,
            TaxRate,
            CityEconomy,
            ResidentialIncome,
            CommercialIncome,
            IndustrialIncome,
            OfficeIncome,
            TourismIncome,
            ServiceExpenses,
            ParkAreas,
            IndustryAreas,
            FishingIndustry,
            CampusAreas,
            TransportEconomy,
            GameLimits,
            Location,
            Citizen
        }

        // main properties set by the constructor
        private readonly CategoryType _type;
        private readonly Translation.Key _descriptionKey;
        private readonly Statistics _statistics;

        // properties needed by the UI
        private string _description;
        private bool _expanded;

        // UI components that are referenced after they are created
        private UIPanel _panel;
        private UISprite _expansionIcon;
        private UILabel _label;

        /// <summary>
        /// constructor to set main properties
        /// </summary>
        public Category(CategoryType type, Translation.Key descriptionKey)
        {
            // initialize
            _type = type;
            _descriptionKey = descriptionKey;
            _statistics = new Statistics();

            // description key depends on DLC
            if (_descriptionKey == Translation.Key.IndustryAreas && SteamHelper.IsDLCOwned(SteamHelper.DLC.UrbanDLC) && !SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC))
            {
                _descriptionKey = Translation.Key.Fishing;
            }

            // initialize UI text
            UpdateUIText();

            // logging to get list of categories
            // LogUtil.LogInfo(_description);
        }

        // read-only accessors
        public CategoryType Type { get { return _type; } }
        public Statistics Statistics {  get { return _statistics; } }
        public string Description { get { return _description; } }

        /// <summary>
        /// whether or not the category is expanded
        /// </summary>
        public bool Expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                _expanded = value;

                // check if panel and expansion icon are set
                if (_panel != null && _expansionIcon != null)
                {
                    // set panel height and expansion icon
                    _panel.height = UIHeight + (value ? Statistic.UIHeight * _statistics.CountEnabled : 0);
                    _expansionIcon.spriteName = (value ? "IconDownArrow2Focused" : "ArrowRightFocused");
                }
            }
        }

        /// <summary>
        /// create category UI on the categories scrollable panel
        /// </summary>
        public bool CreateUI(UIScrollablePanel categoriesScrollablePanel)
        {
            // build name prefix
            string namePrefix = Type.ToString();

            // create panel to hold the UI components
            _panel = categoriesScrollablePanel.AddUIComponent<UIPanel>();
            if (_panel == null)
            {
                LogUtil.LogError($"Unable to create category panel for [{Type}].");
                return false;
            }
            _panel.name = namePrefix + "Panel";
            _panel.autoSize = false;
            _panel.size = new Vector2(categoriesScrollablePanel.size.x - MainPanel.ScrollbarWidth, UIHeight);
            _panel.relativePosition = new Vector3(0f, 0f);  // scrollable panel uses auto layout
            _panel.clipChildren = true;      // prevents contained statistics from being displayed when category is collapsed
            _panel.autoLayoutStart = LayoutStart.TopLeft;
            _panel.autoLayoutDirection = LayoutDirection.Vertical;
            _panel.autoLayout = true;

            // create the expansion panel
            UIPanel expansionPanel = _panel.AddUIComponent<UIPanel>();
            if (expansionPanel == null)
            {
                LogUtil.LogError($"Unable to create category expansion panel for [{Type}].");
                return false;
            }
            expansionPanel.name = namePrefix + "ExpansionPanel";
            expansionPanel.autoSize = false;
            expansionPanel.size = new Vector2(_panel.size.x, UIHeight);
            expansionPanel.relativePosition = new Vector3(0f, 0f);
            expansionPanel.eventClicked += Category_eventClicked;

            // create the expansion icon
            const float componentHeight = UIHeight - 2f;
            _expansionIcon = expansionPanel.AddUIComponent<UISprite>();
            if (_expansionIcon == null)
            {
                LogUtil.LogError($"Unable to create category expansion icon for [{Type}].");
                return false;
            }
            _expansionIcon.name = namePrefix + "ExpansionIcon";
            _expansionIcon.autoSize = false;
            _expansionIcon.size = new Vector2(componentHeight, componentHeight);
            _expansionIcon.relativePosition = new Vector3(0f, 0f);

            // create the label
            _label = expansionPanel.AddUIComponent<UILabel>();
            if (_label == null)
            {
                LogUtil.LogError($"Unable to create category label for [{Type}].");
                return false;
            }
            _label.name = namePrefix + "Label";
            _label.relativePosition = new Vector3(_expansionIcon.relativePosition.x + _expansionIcon.size.x + 3f, 3.5f);
            _label.autoSize = false;
            _label.size = new Vector2(_panel.size.x - _label.relativePosition.x, componentHeight);
            _label.textScale = 0.75f;
            _label.textColor = new Color32(185, 221, 254, 255);;

            // create statistics UI
            if (!_statistics.CreateUI())
            {
                return false;
            }

            // set initial expansion status to the status previously read from the game save file
            Expanded = Expanded;

            // if there are no enabled statistics, then hide and collapse the category
            if (_statistics.CountEnabled == 0)
            {
                _panel.isVisible = false;
                Expanded = false;
            }

            // success
            return true;
        }

        /// <summary>
        /// handle click on category panel
        /// </summary>
        private void Category_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // ignore if event was already used by statistic
            if (eventParam.used)
            {
                return;
            }

            // toggle expansion status
            Expanded = !Expanded;
            eventParam.Use();
        }

        /// <summary>
        /// add a statistic panel to the category panel
        /// </summary>
        public UIPanel AddStatisticPanel()
        {
            return _panel.AddUIComponent<UIPanel>();
        }

        /// <summary>
        /// update UI text
        /// </summary>
        public void UpdateUIText()
        {
            // obtain the translated description
            _description = Translation.instance.Get(_descriptionKey);

            // update the label
            if (_label != null)
            {
                _label.text = _description;
            }

            // update the statistics
            _statistics.UpdateUIText();
        }

        /// <summary>
        /// update statistic amounts
        /// </summary>
        public void UpdateStatisticAmounts(Snapshot snapshot)
        {
            // update the amount for all statistics
            _statistics.UpdateAmounts(snapshot);
        }

        /// <summary>
        /// write the category to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // write category expansion status
            writer.Write(Expanded);

            // write statistics
            _statistics.Serialize(writer);
        }

        /// <summary>
        /// read the category from the game save file
        /// </summary>
        public void Deserialize(BinaryReader reader, int version)
        {
            // read category expansion status
            if (version < 2 && _type == CategoryType.FishingIndustry)
            {
                Expanded = false;
            }
            else
            {
                Expanded = reader.ReadBoolean();
            }

            // read statistics
            _statistics.Deserialize(reader, version);
        }
    }
}
