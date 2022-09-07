using ColossalFramework.UI;
using UnityEngine;
using System;
using System.Reflection;
using ColossalFramework.Globalization;

namespace RealTimeStatistics
{
    /// <summary>
    /// the main panel to display things on the screen
    /// </summary>
    public class MainPanel : UIPanel
    {
        // default panel position, these values are used to detect when the panel position was not read from the config file
        public static readonly float DefaultPositionX = -1f;
        public static readonly float DefaultPositionY = -1f;

        // other constants
        public const int MaximumSelectedStatistics = 10;
        public const float ScrollbarWidth = 16f;

        // UI elements that need handling
        private UILabel _title;
        private UIImprovedGraph _graph;
        private UIButton _expandAll;
        private UIButton _collapseAll;
        private UILabel _selected;
        private UIButton _deselectAll;
        private UILabel _snapshotCount;

        // values for updating statistic amounts
        private long _previousTicks;
        private bool _initialized;

        /// <summary>
        /// Start is called after the panel is created
        /// set up the panel
        /// </summary>
        public override void Start()
        {
            // do base processing
            base.Start();

            try
            {
                // set properties
                name = "RealTimeStatisticsPanel";
                backgroundSprite = "MenuPanel2";
                opacity = 1f;
                isVisible = false;  // always default to hidden
                canFocus = true;
                autoSize = false;

                // the width is the maximum that still fits on the screen using the game's narrower 4:3 aspect ratio
                // the height is just enough so that the vertical scroll bar is not needed when all categories are collapsed
                // the user's screen resolution setting and the game's graphics display settings resolution do not matter
                size = new Vector2(1430f, 920f);

                // if panel positon is negative (i.e. default), then center panel on the view
                if (relativePosition.x < 0f)
                {
                    relativePosition = new Vector3((GetUIView().GetScreenResolution().x - size.x) / 2f, (GetUIView().GetScreenResolution().y - size.y) / 2f);
                }

                // create heading UI components
                if (!CreateHeadingUIComponents()) return;

                // define the color for the graph, options, and statistics panels
                Color32 panelColor = new Color32(45, 52, 61, 255);

                // create graph panel
                UIPanel graphPanel = AddUIComponent<UIPanel>();
                if (graphPanel == null)
                {
                    LogUtil.LogError($"Unable to create graph panel.");
                    return;
                }
                graphPanel.name = "GraphPanel";
                graphPanel.autoSize = false;
                graphPanel.size = new Vector3(1000f, size.y - 60f);
                graphPanel.relativePosition = new Vector3(10f, 50f);
                graphPanel.backgroundSprite = "GenericPanel";
                graphPanel.color = panelColor;

                // create the graph on the graph panel
                _graph = graphPanel.AddUIComponent<UIImprovedGraph>();
                if (_graph == null)
                {
                    LogUtil.LogError($"Unable to create graph.");
                    return;
                }
                _graph.name = "StatisticsGraph";
                _graph.autoSize = false;
                _graph.size = new Vector2(graphPanel.size.x - 4f, graphPanel.size.y - 4f); // just inside the graph panel
                _graph.relativePosition = new Vector3(2f, 2f);
                _graph.spriteName = "PieChartWhiteBg";
                _graph.AxesColor = new Color32(116, 149, 165, 255);
                _graph.HelpAxesColor = _graph.AxesColor.Multiply(0.1f);
                _graph.GraphRect = new Rect(0.1f, 0.04f, 0.88f, 0.94f);

                // create options panel
                UIPanel optionsPanel = AddUIComponent<UIPanel>();
                if (optionsPanel == null)
                {
                    LogUtil.LogError($"Unable to create options panel.");
                    return;
                }
                optionsPanel.name = "OptionsPanel";
                optionsPanel.autoSize = false;
                optionsPanel.size = new Vector3(size.x - graphPanel.size.x - 30f, 100f);
                optionsPanel.relativePosition = new Vector3(graphPanel.relativePosition.x + graphPanel.size.x + 10f, graphPanel.relativePosition.y);
                optionsPanel.backgroundSprite = "GenericPanel";
                optionsPanel.color = panelColor;

                // create options UI components on the options panel
                if (!CreateOptionsUIComponents(optionsPanel)) return;

                // create categories background panel
                UIPanel categoriesBackgroundPanel = AddUIComponent<UIPanel>();
                if (categoriesBackgroundPanel == null)
                {
                    LogUtil.LogError($"Unable to create categories background panel.");
                    return;
                }
                categoriesBackgroundPanel.name = "CategoriesBackgroundPanel";
                categoriesBackgroundPanel.autoSize = false;
                categoriesBackgroundPanel.size = new Vector3(optionsPanel.size.x, graphPanel.size.y - optionsPanel.size.y - 10f);
                categoriesBackgroundPanel.relativePosition = new Vector3(optionsPanel.relativePosition.x, optionsPanel.relativePosition.y + optionsPanel.size.y + 10f);
                categoriesBackgroundPanel.backgroundSprite = "GenericPanel";
                categoriesBackgroundPanel.color = panelColor;

                // create categories scrollable panel on the categories background panel
                if (!CreateCategoriesScrollablePanel(categoriesBackgroundPanel, out UIScrollablePanel categoriesScrollablePanel)) return;

                // create categories UI on the categories scrollable panel
                if (!Categories.instance.CreateUI(categoriesScrollablePanel)) return;

                // add event handler
                eventClicked += MainPanel_eventClicked;

                // initialize UI text
                UpdateUIText();

                // add event
                eventVisibilityChanged += MainPanel_eventVisibilityChanged;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }

        /// <summary>
        /// create heading UI components
        /// </summary>
        private bool CreateHeadingUIComponents()
        {
            // create icon in upper left
            //UISprite panelIcon = AddUIComponent<UISprite>();
            //if (panelIcon == null)
            //{
            //    LogUtil.LogError($"Unable to create statistics icon.");
            //    return false;
            //}
            //panelIcon.name = "Icon";
            //panelIcon.autoSize = false;
            //panelIcon.size = new Vector2(36f, 36f);
            //panelIcon.relativePosition = new Vector3(10f, 2f);
            //panelIcon.spriteName = "ThumbStatistics";

            // create the title label
            _title = AddUIComponent<UILabel>();
            if (_title == null)
            {
                LogUtil.LogError($"Unable to create title label.");
                return false;
            }
            _title.name = "Title";
            _title.autoSize = false;
            _title.size = new Vector2(size.x, 35f);
            _title.relativePosition = new Vector3(0f, 4f);
            _title.textAlignment = UIHorizontalAlignment.Center;
            _title.textScale = 1.75f;
            _title.useGradient = true;
            _title.textColor = new Color32(255, 255, 254, 255);      // gradient colors were taken manually from the game's City Statistics panel title text
            _title.bottomColor = new Color32(76, 232, 255, 255);

            // create close button
            UIButton closeButton = AddUIComponent<UIButton>();
            if (closeButton == null)
            {
                LogUtil.LogError($"Unable to create close button.");
                return false;
            }
            closeButton.name = "Close";
            closeButton.autoSize = false;
            closeButton.size = new Vector2(32f, 32f);
            closeButton.relativePosition = new Vector3(width - 34f, 2f);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.eventClicked += CloseButton_eventClicked;

            // create drag handle
            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            if (dragHandle == null)
            {
                LogUtil.LogError($"Unable to create drag handle.");
                return false;
            }
            dragHandle.name = "DragHandle";
            dragHandle.autoSize = false;
            dragHandle.size = new Vector3(size.x, 40f);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.eventMouseUp += DragHandle_eventMouseUp;
            dragHandle.constrainToScreen = false;   // allow panel to dragged off screen

            // make sure drag handle is in front of icon and title
            dragHandle.BringToFront();

            // make sure close button is in front of drag handle
            closeButton.BringToFront();

            // success
            return true;
        }

        /// <summary>
        /// create options UI components on the option panel
        /// </summary>
        private bool CreateOptionsUIComponents(UIPanel optionsPanel)
        {
            // create Expand All button
            const float ButtonWidth = 130f;
            const float ButtonHeight = 20f;
            _expandAll = optionsPanel.AddUIComponent<UIButton>();
            if (_expandAll == null)
            {
                LogUtil.LogError($"Unable to create Expand All button.");
                return false;
            }
            _expandAll.name = "ExpandAll";
            _expandAll.autoSize = false;
            _expandAll.size = new Vector2(ButtonWidth, ButtonHeight);
            _expandAll.relativePosition = new Vector3(5f, (optionsPanel.size.y - 4f * ButtonHeight) / 5f);
            _expandAll.textScale = 0.75f;
            _expandAll.horizontalAlignment = UIHorizontalAlignment.Center;
            _expandAll.verticalAlignment = UIVerticalAlignment.Middle;
            _expandAll.normalBgSprite = "ButtonMenu";
            _expandAll.hoveredBgSprite = "ButtonMenuHovered";
            _expandAll.pressedBgSprite = "ButtonMenuPressed";
            _expandAll.eventClicked += ExpandAll_eventClicked;

            // create Collapse All button
            _collapseAll = optionsPanel.AddUIComponent<UIButton>();
            if (_collapseAll == null)
            {
                LogUtil.LogError($"Unable to create Collapse All button.");
                return false;
            }
            _collapseAll.name = "CollapseAll";
            _collapseAll.autoSize = false;
            _collapseAll.size = new Vector2(ButtonWidth, ButtonHeight);
            _collapseAll.relativePosition = new Vector3(_expandAll.relativePosition.x, 2f * _expandAll.relativePosition.y + 1f * ButtonHeight);
            _collapseAll.textScale = 0.75f;
            _collapseAll.horizontalAlignment = UIHorizontalAlignment.Center;
            _collapseAll.verticalAlignment = UIVerticalAlignment.Middle;
            _collapseAll.normalBgSprite = "ButtonMenu";
            _collapseAll.hoveredBgSprite = "ButtonMenuHovered";
            _collapseAll.pressedBgSprite = "ButtonMenuPressed";
            _collapseAll.eventClicked += CollapseAll_eventClicked;

            // create number selected label
            _selected = optionsPanel.AddUIComponent<UILabel>();
            if (_selected == null)
            {
                LogUtil.LogError($"Unable to create Number Selected label.");
                return false;
            }
            _selected.name = "NumberSelected";
            _selected.autoSize = false;
            _selected.size = new Vector2(ButtonWidth, ButtonHeight);
            _selected.relativePosition = new Vector3(_expandAll.relativePosition.x, 3f * _expandAll.relativePosition.y + 2f * ButtonHeight);
            _selected.textScale = 0.75f;
            _selected.textAlignment = UIHorizontalAlignment.Center;
            _selected.verticalAlignment = UIVerticalAlignment.Bottom;
            _selected.textColor = _expandAll.textColor;

            // create Deselect All button
            _deselectAll = optionsPanel.AddUIComponent<UIButton>();
            if (_deselectAll == null)
            {
                LogUtil.LogError($"Unable to create Deselect All button.");
                return false;
            }
            _deselectAll.name = "DeselectAll";
            _deselectAll.autoSize = false;
            _deselectAll.size = new Vector2(ButtonWidth, ButtonHeight);
            _deselectAll.relativePosition = new Vector3(_expandAll.relativePosition.x, 4f * _expandAll.relativePosition.y + 3f * ButtonHeight);
            _deselectAll.textScale = 0.75f;
            _deselectAll.horizontalAlignment = UIHorizontalAlignment.Center;
            _deselectAll.verticalAlignment = UIVerticalAlignment.Middle;
            _deselectAll.normalBgSprite = "ButtonMenu";
            _deselectAll.hoveredBgSprite = "ButtonMenuHovered";
            _deselectAll.pressedBgSprite = "ButtonMenuPressed";
            _deselectAll.eventClicked += DeselectAll_eventClicked;

            // create Show Range UI on the options panel
            float showRangeLeft = _expandAll.relativePosition.x + ButtonWidth + 5f;
            if (!ShowRange.instance.CreateUI(optionsPanel, showRangeLeft, _expandAll.textColor)) return false;

            // create snapshot count label
            _snapshotCount = optionsPanel.AddUIComponent<UILabel>();
            if (_snapshotCount == null)
            {
                LogUtil.LogError($"Unable to create Deselect All button.");
                return false;
            }
            _snapshotCount.name = "SnapshotCount";
            _snapshotCount.autoSize = false;
            _snapshotCount.size = new Vector2(optionsPanel.size.x - showRangeLeft, ButtonHeight);
            _snapshotCount.relativePosition = new Vector3(optionsPanel.size.x - 5f - _snapshotCount.size.x, optionsPanel.size.y - ButtonHeight);
            _snapshotCount.textScale = 0.625f;
            _snapshotCount.textAlignment = UIHorizontalAlignment.Right;
            _snapshotCount.verticalAlignment = UIVerticalAlignment.Bottom;
            _snapshotCount.textColor = _expandAll.textColor;

            // success
            return true;
        }

        /// <summary>
        /// create the categories scrollable panel on which the category panels will be created
        /// </summary>
        private bool CreateCategoriesScrollablePanel(UIPanel categoriesBackgroundPanel, out UIScrollablePanel categoriesScrollablePanel)
        {
            // create scrollable panel on the background panel
            categoriesScrollablePanel = categoriesBackgroundPanel.AddUIComponent<UIScrollablePanel>();
            if (categoriesScrollablePanel == null)
            {
                LogUtil.LogError($"Unable to create categories scrollable panel.");
                return false;
            }
            categoriesScrollablePanel.name = "CategoriesScrollablePanel";
            categoriesScrollablePanel.FitTo(categoriesBackgroundPanel);
            categoriesScrollablePanel.backgroundSprite = string.Empty;
            categoriesScrollablePanel.clipChildren = true;      // prevents contained categories from being displayed when they are scrolled out of view
            categoriesScrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
            categoriesScrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
            categoriesScrollablePanel.autoLayout = true;
            categoriesScrollablePanel.scrollWheelDirection = UIOrientation.Vertical;
            categoriesScrollablePanel.builtinKeyNavigation = true;
            categoriesScrollablePanel.scrollWithArrowKeys = true;

            // create vertical scroll bar on scrollable panel
            UIScrollbar statisticsScrollbar = categoriesBackgroundPanel.AddUIComponent<UIScrollbar>();
            if (statisticsScrollbar == null)
            {
                LogUtil.LogError($"Unable to create statistics scrollbar.");
                return false;
            }
            statisticsScrollbar.name = "StatisticsScrollbar";
            statisticsScrollbar.size = new Vector2(ScrollbarWidth, categoriesScrollablePanel.size.y);
            statisticsScrollbar.relativePosition = new Vector2(categoriesScrollablePanel.width - ScrollbarWidth, 0f);
            statisticsScrollbar.orientation = UIOrientation.Vertical;
            statisticsScrollbar.stepSize = 10f;
            statisticsScrollbar.incrementAmount = 50f;
            statisticsScrollbar.scrollEasingType = ColossalFramework.EasingType.BackEaseOut;
            categoriesScrollablePanel.verticalScrollbar = statisticsScrollbar;

            // create scroll bar track on scroll bar
            UISlicedSprite statisticsScrollbarTrack = statisticsScrollbar.AddUIComponent<UISlicedSprite>();
            if (statisticsScrollbarTrack == null)
            {
                LogUtil.LogError($"Unable to create statistics scrollbar track.");
                return false;
            }
            statisticsScrollbarTrack.name = "StatisticsScrollbarTrack";
            statisticsScrollbarTrack.size = new Vector2(ScrollbarWidth, categoriesScrollablePanel.size.y);
            statisticsScrollbarTrack.relativePosition = Vector3.zero;
            statisticsScrollbarTrack.spriteName = "ScrollbarTrack";
            statisticsScrollbar.trackObject = statisticsScrollbarTrack;

            // create scroll bar thumb on scroll bar track
            UISlicedSprite statisticsScrollbarThumb = statisticsScrollbarTrack.AddUIComponent<UISlicedSprite>();
            if (statisticsScrollbarThumb == null)
            {
                LogUtil.LogError($"Unable to create statistics scrollbar thumb.");
                return false;
            }
            statisticsScrollbarThumb.name = "StatisticsScrollbarThumb";
            statisticsScrollbarThumb.autoSize = true;
            statisticsScrollbarThumb.size = new Vector2(ScrollbarWidth - 4f, 0f);
            statisticsScrollbarThumb.relativePosition = Vector3.zero;
            statisticsScrollbarThumb.spriteName = "ScrollbarThumb";
            statisticsScrollbar.thumbObject = statisticsScrollbarThumb;

            // success
            return true;
        }


        #region Event Handlers

        /// <summary>
        /// handle panel move
        /// </summary>
        private void DragHandle_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            // save position
            Configuration.SavePanelPosition(relativePosition);
        }

        /// <summary>
        /// handle click on Close button
        /// </summary>
        private void CloseButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // hide the panel
            UserInterface.instance.HideMainPanel();
        }

        /// <summary>
        /// handle click on Expand All button
        /// </summary>
        private void ExpandAll_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // expand all categories, this does not change the graph
            Categories.instance.ExpandAll();
        }

        /// <summary>
        /// handle click on Collapse All button
        /// </summary>
        private void CollapseAll_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // collapse all categories, this does not change the graph
            Categories.instance.CollapseAll();
        }

        /// <summary>
        /// handle click on Deselect All button
        /// </summary>
        private void DeselectAll_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // deselect all statistics
            Categories.instance.DeselectAllStatistics();

            // update panel
            UserInterface.instance.UpdateMainPanel();
        }

        /// <summary>
        /// handle click on panel
        /// </summary>
        private void MainPanel_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // bring panel to front
            if (!eventParam.used)
            {
                BringToFront();
            }
        }

        /// <summary>
        /// handle change in panel visibility
        /// </summary>
        private void MainPanel_eventVisibilityChanged(UIComponent component, bool value)
        {
            // when made visible, update statistic amounts on next Update cycle
            if (value)
            {
                _previousTicks = 0;
            }
        }

        #endregion


        /// <summary>
        /// update UI text
        /// </summary>
        public void UpdateUIText()
        {
            // update panel title
            Translation translation = Translation.instance;
            _title.text = translation.Get(Translation.Key.Title);

            // update options buttons
            _expandAll.text = translation.Get(Translation.Key.ExpandAll);
            _collapseAll.text = translation.Get(Translation.Key.CollapseAll);
            _deselectAll.text = translation.Get(Translation.Key.DeselectAll);

            // update shows years
            ShowRange.instance.UpdateUIText();

            // update snapshot count
            UpdateSnapshotCount();

            // update categories and statistics
            Categories.instance.UpdateUIText();
        }

        /// <summary>
        /// update statistic amounts with a new nonlogged snapshot
        /// </summary>
        public void UpdateStatisticAmounts()
        {
            Snapshot snapshot = Snapshot.TakeSnapshot(SimulationManager.instance.m_currentGameTime.Date, false);
            Categories.instance.UpdateStatisticAmounts(snapshot);
            _previousTicks = DateTime.Now.Ticks;
        }

        /// <summary>
        /// update snapshot count
        /// </summary>
        private void UpdateSnapshotCount()
        {
            _snapshotCount.text = Translation.instance.Get(Translation.Key.SnapshotCount) + " " + Snapshots.instance.Count.ToString("N0", LocaleManager.cultureInfo);
        }

        /// <summary>
        /// update the panel
        /// </summary>
        public void UpdatePanel()
        {
            // update the selected statistics
            // the user interface logic prevents the count from being more than the maximum
            _selected.text = string.Format(Translation.instance.Get(Translation.Key.Selected), Categories.instance.CountSelected, MaximumSelectedStatistics);

            // clear the graph
            _graph.Clear();

            // lock thread while working with the snapshots because this routine is called from two threads:
            // the simulation thread which writes the snapshots and the UI thread which reads the snapshots
            Snapshots snapshotsInstance = Snapshots.instance;
            snapshotsInstance.LockThread();

            try
            {
                // update the Show Range panel
                ShowRange showRangeInstance = ShowRange.instance;
                showRangeInstance.UpdatePanel();

                // update snapshot count
                UpdateSnapshotCount();

                // check for data
                int snapshotsCount = snapshotsInstance.Count;
                if (snapshotsCount == 0)
                {
                    // add one dummy point for the current game date so the graph axes are drawn
                    _graph.SetDates(new DateTime[] { SimulationManager.instance.m_currentGameTime.Date });
                    _graph.AddCurve("No Description", "No Units", "N0", new double?[] { null }, 0.01f, new Color32(0, 0, 0, 0));
                }
                else
                {
                    // compute the first and last snapshot indexes
                    int firstIndexToInclude;
                    int lastIndexToInclude;
                    if (showRangeInstance.ShowAll)
                    {
                        // Show current day
                        
                        
                        lastIndexToInclude = snapshotsCount - 1;
                        firstIndexToInclude = 0;
                        for (int i = lastIndexToInclude; i >= 0; i--)
                        {
                            // first index is the last snapshot before or on the selected From date
                            DateTime snapshotDate = snapshotsInstance[i].SnapshotDate;

                            if (snapshotDate.Hour == 0)
                            {
                                firstIndexToInclude = i - 1;
                                break;
                            } 
                        }
                    }
                    else
                    {
                        // use selected FromTo dates or years

                        // initialize with invalid index values to indicate no index yet found
                        const int InvalidIndex = -1;
                        firstIndexToInclude = InvalidIndex;
                        lastIndexToInclude = InvalidIndex;

                        // check for Real Time mod
                        if (ModUtil.IsWorkshopModEnabled(ModUtil.ModIDRealTime))
                        {
                            // get the selected From and To dates, could be the same date
                            DateTime selectedFromDate = showRangeInstance.FromDate;
                            DateTime selectedToDate   = showRangeInstance.ToDate;

                            // if From date is more than To date, then swap the two dates
                            // i.e. the user can use either slider to specify From and To dates
                            if (selectedFromDate > selectedToDate)
                            {
                                DateTime tempDate = selectedFromDate;
                                selectedFromDate = selectedToDate;
                                selectedToDate = tempDate;
                            }

                            // compute the first and last snapshot index to include based on the selected From and To dates
                            // first and last indexes can be the same because From and To dates can be the same date
                            DateTime firstDate = snapshotsInstance[0].SnapshotDate;
                            for (int i = 0; i < snapshotsCount; i++)
                            {
                                // first index is the last snapshot before or on the selected From date
                                DateTime snapshotDate = snapshotsInstance[i].SnapshotDate;
                                
                                if (snapshotDate <= selectedFromDate)
                                {
                                    firstIndexToInclude = i;
                                    firstDate = snapshotDate;
                                }

                                // update first index if day changes
                                if(snapshotDate > firstDate)
                                {
                                    firstIndexToInclude = i;
                                    firstDate = snapshotDate;
                                }

                                // last index is the first snapshot that is on or after the selected To date
                                if (snapshotDate >= selectedToDate)
                                {
                                    lastIndexToInclude = i;

                                    // once last index is found, stop checking
                                    break;
                                }
                            }
                            LogUtil.LogInfo($"selectedFromDate [{selectedFromDate.ToString("yyyy/MM/dd HH:mm")}].");
                            LogUtil.LogInfo($"selectedToDate [{selectedToDate.ToString("yyyy/MM/dd HH:mm")}].");
                        }
                        else
                        {
                            // get the selected From and To years, could be the same year
                            int selectedFromYear = showRangeInstance.FromYear;
                            int selectedToYear   = showRangeInstance.ToYear;

                            // if From year is more than To year, then swap the two years
                            // i.e. the user can use either slider to specify From and To years
                            if (selectedFromYear > selectedToYear)
                            {
                                int tempYear = selectedFromYear;
                                selectedFromYear = selectedToYear;
                                selectedToYear = tempYear;
                            }

                            // if To year is same as From year, make To year 1 more than From year so there is always at least one year between From and To
                            if (selectedToYear == selectedFromYear)
                            {
                                selectedToYear = selectedFromYear + 1;
                            }

                            // compute the first and last snapshot index to include based on the selected From and To years
                            for (int i = 0; i < snapshotsCount; i++)
                            {
                                // first index is the last snapshot before the selected From year OR the first snapshot in the selected From year
                                int snapshotYear = snapshotsInstance[i].SnapshotDate.Year;
                                if (snapshotYear < selectedFromYear)
                                {
                                    // the snapshot is before the selected From year, use this snapshot
                                    firstIndexToInclude = i;
                                }
                                else if (snapshotYear == selectedFromYear)
                                {
                                    // the snapshot is in the selected From year
                                    // check if already have a first index
                                    if (firstIndexToInclude == InvalidIndex)
                                    {
                                        // don't yet have a first index, so use this snapshot
                                        firstIndexToInclude = i;
                                    }
                                    else
                                    {
                                        // already have a first index
                                        // if this snapshot year is later than the year of the already found first index, then use this snapshot
                                        if (snapshotYear > snapshotsInstance[firstIndexToInclude].SnapshotDate.Year)
                                        {
                                            firstIndexToInclude = i;
                                        }
                                    }
                                }

                                // last index is the first snapshot in or after the selected To year
                                // this will usually be the snapshot for January 1 of the To year
                                if (snapshotYear >= selectedToYear)
                                {
                                    lastIndexToInclude = i;

                                    // once last index is found, stop checking
                                    break;
                                }
                            }

                            // if first index was not found (because the From year is before all snapshots), then use the first snapshot
                            // this should never happen because the slider min value is the year of the first snapshot
                            if (firstIndexToInclude == InvalidIndex)
                            {
                                LogUtil.LogError($"Index not found for first snapshot to include.  Reverting to index for first snapshot.");
                                firstIndexToInclude = 0;
                            }

                            // if last index was not found (because the To year is after all snapshots), then use last snapshot
                            // this can happen because, except when last snapshot is January 1, the slider max value is the year after the year of the last snapshot
                            if (lastIndexToInclude == InvalidIndex)
                            {
                                lastIndexToInclude = snapshotsCount - 1;
                            }
                        }

                        // if first and last indexes don't make sense, then revert to using entire snapshot range
                        // this should never happen
                        if (firstIndexToInclude == InvalidIndex || lastIndexToInclude == InvalidIndex || firstIndexToInclude > lastIndexToInclude)
                        {
                            LogUtil.LogError($"Invalid snapshot indexes:  firstIndexToInclude={firstIndexToInclude} lastIndexToInclude={lastIndexToInclude}.  Reverting to entire range.");
                            firstIndexToInclude = 0;
                            lastIndexToInclude = snapshotsCount - 1;
                        }
                    }


                    // get the snapshot field/property for each selected statistic
                    // every statistic has either a field or a property in a snapshot
                    FieldInfo[] snapshotFields = new FieldInfo[MaximumSelectedStatistics];
                    PropertyInfo[] snapshotProperties = new PropertyInfo[MaximumSelectedStatistics];
                    Statistics selectedStatistics = Categories.instance.SelectedStatistics;
                    for (int i = 0; i < selectedStatistics.Count; i++)
                    {
                        Snapshot.GetFieldProperty(selectedStatistics[i].Type, out snapshotFields[i], out snapshotProperties[i]);
                    }

                    // the number of points that can be included on the graph is somehow limited by the graph calculations depending on the number of curves and the number of points per curve
                    // with a maximum of 10 (MaximumSelectedStatistics) curves, the graph allows a maximum of about 700 points per curve before the calculations break down
                    // but 700 points per curve results in the points being too close together
                    // so compute the number of points to combine so that there are never more than 300 points per curve
                    int snapshotsToInclude = lastIndexToInclude - firstIndexToInclude + 1;
                    int pointsToCombine = Mathf.CeilToInt(snapshotsToInclude / 300f);
                    int arraySize = Mathf.CeilToInt((float)snapshotsToInclude / pointsToCombine);

                    // define totals and counts that will be used to compute averages
                    long totalSeconds = 0;
                    int countDates = 0;
                    double[] totals = new double[MaximumSelectedStatistics];
                    int[] counts = new int[MaximumSelectedStatistics];

                    // define arrays needed by the graph
                    DateTime[] dates = new DateTime[arraySize];
                    double?[][] values = new double?[MaximumSelectedStatistics][];
                    for (int i = 0; i < MaximumSelectedStatistics; i++)
                    {
                        values[i] = new double?[arraySize];
                    }

                    // do each snapshot
                    int arrayIndex = 0;
                    for (int snapshotIndex = firstIndexToInclude; snapshotIndex <= lastIndexToInclude; snapshotIndex++)
                    {
                        // get the snapshot
                        Snapshot snapshot = snapshotsInstance[snapshotIndex];

                        // accumulate total seconds and count for the date
                        // seconds are accumulated instead of ticks to avoid the possibility of overflowing the total
                        // because all the snapshot dates have no time, this should always divide evenly
                        totalSeconds += snapshot.SnapshotDate.Ticks / TimeSpan.TicksPerSecond;
                        countDates++;

                        // accumulate total and count for each selected statistic
                        for (int i = 0; i < selectedStatistics.Count; i++)
                        {
                            // get the snapshot value from either the field or the property
                            object snapshotValue = null;
                            if (snapshotFields[i] != null)
                            {
                                snapshotValue = snapshotFields[i].GetValue(snapshot);
                            }
                            else if (snapshotProperties[i] != null)
                            {
                                snapshotValue = snapshotProperties[i].GetValue(snapshot, null);
                            }

                            // add the snapshot value to the total and count it
                            if (snapshotValue != null)
                            {
                                totals[i] += Convert.ToDouble(snapshotValue);
                                counts[i]++;
                            }
                        }

                        // check if the number of points to combine have been accumulated
                        // also handle the last array entry, which may not count up to pointsToCombine
                        if (countDates == pointsToCombine || snapshotIndex == lastIndexToInclude)
                        {
                            // compute average date
                            // any time remains so the dot is drawn in the correct position on the graph
                            long averageSeconds = totalSeconds / countDates;
                            dates[arrayIndex] = new DateTime(averageSeconds * TimeSpan.TicksPerSecond);
                            totalSeconds = 0;
                            countDates = 0;

                            // do each selected statistic
                            for (int i = 0; i < selectedStatistics.Count; i++)
                            {
                                // compute average value
                                if (counts[i] == 0)
                                {
                                    values[i][arrayIndex] = null;
                                }
                                else
                                {
                                    values[i][arrayIndex] = totals[i] / counts[i];
                                }

                                // reset total and count
                                totals[i] = 0;
                                counts[i] = 0;
                            }

                            // go to next array index
                            arrayIndex++;
                        }
                    }

                    // set the graph dates
                    _graph.SetDates(dates);

                    // add graph curves
                    if (selectedStatistics.Count == 0)
                    {
                        // add one dummy curve so the graph axes are drawn with the correct dates
                        _graph.AddCurve("No Description", "No Units", "N0", new double?[dates.Length], 0.01f, new Color32(0, 0, 0, 0));
                    }
                    else
                    {
                        // add a curve for each selected statistic
                        const float CurveWidth = 0.6f;      // thick enough so lines are still drawn at low resolutions
                        for (int i = 0; i < selectedStatistics.Count; i++)
                        {
                            Statistic selectedStatistic = selectedStatistics[i];
                            _graph.AddCurve(selectedStatistic.CategoryDescription, selectedStatistic.Units, selectedStatistic.NumberFormat, values[i], CurveWidth, selectedStatistic.LineColor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
            finally
            {
                // make sure thread is unlocked
                Snapshots.instance.UnlockThread();

                // refresh the graph
                _graph.Invalidate();
            }
        }

        /// <summary>
        /// Update is called every frame
        /// </summary>
        public override void Update()
        {
            // do base processing
            base.Update();

            // on first call, update statistic amounts
            if (!_initialized)
            {
                UpdateStatisticAmounts();
                _initialized = true;
            }

            // if visible, update statistic amounts after configured interval has elapsed
            Configuration config = ConfigurationUtil<Configuration>.Load();
            if (isVisible && (DateTime.Now.Ticks - _previousTicks) / (double)TimeSpan.TicksPerSecond >= config.CurrentValueUpdateInterval)
            {
                UpdateStatisticAmounts();
            }
        }
    }
}
