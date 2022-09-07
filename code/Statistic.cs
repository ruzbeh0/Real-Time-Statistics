using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RealTimeStatistics
{

    /// <summary>
    /// properties and UI elements for one statistic
    /// </summary>
    public class Statistic
    {
        // constants
        public const float UIHeight = 15f;

        // statistic type
        // all the enums are prefixed with the category to ensure they are differentiated (e.g. Electricity Consumption vs Water Consumption)
        // the enum member names must exactly match the Snapshot field/property names
        // the enums are in the same order that the categories and statistics are shown on the UI,
        // but the order here is not important because the value of the enum is not saved
        public enum StatisticType
        {
            ElectricityConsumptionPercent, ElectricityConsumption, ElectricityProduction,
            WaterConsumptionPercent, WaterConsumption, WaterPumpingCapacity,
            WaterTankReservedPercent, WaterTankReserved, WaterTankStorageCapacity,
            SewageProductionPercent, SewageProduction, SewageDrainingCapacity,
            LandfillStoragePercent, LandfillStorage, LandfillCapacity,
            GarbageProductionPercent, GarbageProduction, GarbageProcessingCapacity,
            EducationElementaryEligiblePercent, EducationElementaryEligible, EducationElementaryCapacity,
            EducationHighSchoolEligiblePercent, EducationHighSchoolEligible, EducationHighSchoolCapacity,
            EducationUniversityEligiblePercent, EducationUniversityEligible, EducationUniversityCapacity,
            EducationLibraryUsersPercent, EducationLibraryUsers, EducationLibraryCapacity,
            EducationLevelUneducatedPercent, EducationLevelEducatedPercent, EducationLevelWellEducatedPercent, EducationLevelHighlyEducatedPercent,
            EducationLevelUneducated, EducationLevelEducated, EducationLevelWellEducated, EducationLevelHighlyEducated,
            HappinessGlobal, HappinessResidential, HappinessCommercial, HappinessIndustrial, HappinessOffice,
            HealthcareAverageHealth, HealthcareSickPercent, HealthcareSick, HealthcareHealCapacity,
            DeathcareCemeteryBuriedPercent, DeathcareCemeteryBuried, DeathcareCemeteryCapacity,
            DeathcareCrematoriumDeceasedPercent, DeathcareCrematoriumDeceased, DeathcareCrematoriumCapacity,
            DeathcareDeathRate,
            ChildcareAverageHealth, ChildcareSickPercent, ChildcareSick, ChildcarePopulation, ChildcareBirthRate,
            EldercareAverageHealth, EldercareSickPercent, EldercareSick, EldercarePopulation, EldercareAverageLifeSpan,
            ZoningResidentialPercent, ZoningCommercialPercent, ZoningIndustrialPercent, ZoningOfficePercent, ZoningUnzonedPercent,
            ZoningTotal, ZoningResidential, ZoningCommercial, ZoningIndustrial, ZoningOffice, ZoningUnzoned,
            ZoneLevelResidentialAverage, ZoneLevelResidential1, ZoneLevelResidential2, ZoneLevelResidential3, ZoneLevelResidential4, ZoneLevelResidential5,
            ZoneLevelCommercialAverage, ZoneLevelCommercial1, ZoneLevelCommercial2, ZoneLevelCommercial3,
            ZoneLevelIndustrialAverage, ZoneLevelIndustrial1, ZoneLevelIndustrial2, ZoneLevelIndustrial3,
            ZoneLevelOfficeAverage, ZoneLevelOffice1, ZoneLevelOffice2, ZoneLevelOffice3,
            ZoneBuildingsResidentialPercent, ZoneBuildingsCommercialPercent, ZoneBuildingsIndustrialPercent, ZoneBuildingsOfficePercent,
            ZoneBuildingsTotal, ZoneBuildingsResidential, ZoneBuildingsCommercial, ZoneBuildingsIndustrial, ZoneBuildingsOffice,
            ZoneDemandResidential, ZoneDemandCommercial, ZoneDemandIndustrialOffice,
            TrafficAverageFlow,
            PollutionAverageGround, PollutionAverageDrinkingWater, PollutionAverageNoise,
            FireSafetyHazard,
            CrimeRate, CrimeDetainedCriminalsPercent, CrimeDetainedCriminals, CrimeJailsCapacity,
            PublicTransportationTotalTotal, PublicTransportationTotalResidents, PublicTransportationTotalTourists,
            PublicTransportationBusTotal, PublicTransportationBusResidents, PublicTransportationBusTourists,
            PublicTransportationTrolleybusTotal, PublicTransportationTrolleybusResidents, PublicTransportationTrolleybusTourists,
            PublicTransportationTramTotal, PublicTransportationTramResidents, PublicTransportationTramTourists,
            PublicTransportationMetroTotal, PublicTransportationMetroResidents, PublicTransportationMetroTourists,
            PublicTransportationTrainTotal, PublicTransportationTrainResidents, PublicTransportationTrainTourists,
            PublicTransportationShipTotal, PublicTransportationShipResidents, PublicTransportationShipTourists,
            PublicTransportationAirTotal, PublicTransportationAirResidents, PublicTransportationAirTourists,
            PublicTransportationMonorailTotal, PublicTransportationMonorailResidents, PublicTransportationMonorailTourists,
            PublicTransportationCableCarTotal, PublicTransportationCableCarResidents, PublicTransportationCableCarTourists,
            PublicTransportationTaxiTotal, PublicTransportationTaxiResidents, PublicTransportationTaxiTourists,
            PopulationTotal,
            PopulationChildrenPercent, PopulationTeensPercent, PopulationYoungAdultsPercent, PopulationAdultsPercent, PopulationSeniorsPercent,
            PopulationChildren, PopulationTeens, PopulationYoungAdults, PopulationAdults, PopulationSeniors,
            HouseholdsOccupiedPercent, HouseholdsOccupied, HouseholdsAvailable,
            EmploymentPeopleEmployed, EmploymentJobsAvailable, EmploymentUnfilledJobs,
            EmploymentUnemploymentPercent, EmploymentUnemployed, EmploymentEligibleWorkers,
            OutsideConnectionsImportTotal, OutsideConnectionsImportGoods, OutsideConnectionsImportForestry, OutsideConnectionsImportFarming, OutsideConnectionsImportOre, OutsideConnectionsImportOil, OutsideConnectionsImportMail,
            OutsideConnectionsExportTotal, OutsideConnectionsExportGoods, OutsideConnectionsExportForestry, OutsideConnectionsExportFarming, OutsideConnectionsExportOre, OutsideConnectionsExportOil, OutsideConnectionsExportMail, OutsideConnectionsExportFish,
            LandValueAverage,
            NaturalResourcesForestUsedPercent, NaturalResourcesForestUsed, NaturalResourcesForestAvailable,
            NaturalResourcesFertileLandUsedPercent, NaturalResourcesFertileLandUsed, NaturalResourcesFertileLandAvailable,
            NaturalResourcesOreUsedPercent, NaturalResourcesOreUsed, NaturalResourcesOreAvailable,
            NaturalResourcesOilUsedPercent, NaturalResourcesOilUsed, NaturalResourcesOilAvailable,
            HeatingConsumptionPercent, HeatingConsumption, HeatingProduction,
            TourismCityAttractiveness,
            TourismLowWealthPercent, TourismMediumWealthPercent, TourismHighWealthPercent,
            TourismTotal, TourismLowWealth, TourismMediumWealth, TourismHighWealth,
            TourismExchangeStudentBonus,
            ToursTotalTotal, ToursTotalResidents, ToursTotalTourists,
            ToursWalkingTourTotal, ToursWalkingTourResidents, ToursWalkingTourTourists,
            ToursSightseeingTotal, ToursSightseeingResidents, ToursSightseeingTourists,
            ToursBalloonTotal, ToursBalloonResidents, ToursBalloonToursits,
            TaxRateResidentialLow, TaxRateResidentialHigh, TaxRateCommercialLow, TaxRateCommercialHigh, TaxRateIndustrial, TaxRateOffice,
            CityEconomyTotalIncome, CityEconomyTotalExpenses, CityEconomyTotalProfit,
            CityEconomyBankBalance, CityEconomyLoanBalance,
            CityEconomyCityValue, CityEconomyCityValuePerCapita,
            CityEconomyGrossDomesticProduct, CityEconomyGrossDomesticProductPerCapita,
            CityEconomyConsumption, CityEconomyConsumptionPercent,
            CityEconomyGovernmentSpending, CityEconomyGovernmentSpendingPercent,
            CityEconomyExports, CityEconomyImports, CityEconomyNetExports, CityEconomyNetExportsPercent,
            ResidentialIncomeTotalPercent, ResidentialIncomeTotal,
            ResidentialIncomeLowDensityTotal, ResidentialIncomeLowDensity1, ResidentialIncomeLowDensity2, ResidentialIncomeLowDensity3, ResidentialIncomeLowDensity4, ResidentialIncomeLowDensity5, ResidentialIncomeLowDensitySelfSufficient,
            ResidentialIncomeHighDensityTotal, ResidentialIncomeHighDensity1, ResidentialIncomeHighDensity2, ResidentialIncomeHighDensity3, ResidentialIncomeHighDensity4, ResidentialIncomeHighDensity5, ResidentialIncomeHighDensitySelfSufficient,
            CommercialIncomeTotalPercent, CommercialIncomeTotal,
            CommercialIncomeLowDensityTotal, CommercialIncomeLowDensity1, CommercialIncomeLowDensity2, CommercialIncomeLowDensity3,
            CommercialIncomeHighDensityTotal, CommercialIncomeHighDensity1, CommercialIncomeHighDensity2, CommercialIncomeHighDensity3,
            CommercialIncomeSpecializedTotal, CommercialIncomeLeisure, CommercialIncomeTourism, CommercialIncomeOrganic,
            IndustrialIncomeTotalPercent, IndustrialIncomeTotal,
            IndustrialIncomeGenericTotal, IndustrialIncomeGeneric1, IndustrialIncomeGeneric2, IndustrialIncomeGeneric3,
            IndustrialIncomeSpecializedTotal, IndustrialIncomeForestry, IndustrialIncomeFarming, IndustrialIncomeOre, IndustrialIncomeOil,
            OfficeIncomeTotalPercent, OfficeIncomeTotal,
            OfficeIncomeGenericTotal, OfficeIncomeGeneric1, OfficeIncomeGeneric2, OfficeIncomeGeneric3,
            OfficeIncomeITCluster,
            TourismIncomeTotalPercent, TourismIncomeTotal,
            TourismIncomeCommercialZones, TourismIncomeTransportation, TourismIncomeParkAreas,
            ServiceExpensesTotalPercent, ServiceExpensesTotal,
            ServiceExpensesRoads, ServiceExpensesElectricity, ServiceExpensesWaterSewageHeating, ServiceExpensesGarbage,
            ServiceExpensesHealthcare, ServiceExpensesFire, ServiceExpensesEmergency, ServiceExpensesPolice, ServiceExpensesEducation,
            ServiceExpensesParksPlazas, ServiceExpensesUniqueBuildings, ServiceExpensesGenericSportsArenas, ServiceExpensesLoans, ServiceExpensesPolicies,
            ParkAreasTotalIncomePercent, ParkAreasTotalIncome, ParkAreasTotalExpensesPercent, ParkAreasTotalExpenses, ParkAreasTotalProfit,
            ParkAreasCityParkIncome, ParkAreasCityParkExpenses, ParkAreasCityParkProfit,
            ParkAreasAmusementParkIncome, ParkAreasAmusementParkExpenses, ParkAreasAmusementParkProfit,
            ParkAreasZooIncome, ParkAreasZooExpenses, ParkAreasZooProfit,
            ParkAreasNatureReserveIncome, ParkAreasNatureReserveExpenses, ParkAreasNatureReserveProfit,
            IndustryAreasTotalIncomePercent, IndustryAreasTotalIncome, IndustryAreasTotalExpensesPercent, IndustryAreasTotalExpenses, IndustryAreasTotalProfit,
            IndustryAreasForestryIncome, IndustryAreasForestryExpenses, IndustryAreasForestryProfit,
            IndustryAreasFarmingIncome, IndustryAreasFarmingExpenses, IndustryAreasFarmingProfit,
            IndustryAreasOreIncome, IndustryAreasOreExpenses, IndustryAreasOreProfit,
            IndustryAreasOilIncome, IndustryAreasOilExpenses, IndustryAreasOilProfit,
            IndustryAreasWarehousesFactoriesIncome, IndustryAreasWarehousesFactoriesExpenses, IndustryAreasWarehousesFactoriesProfit,
            FishingIndustryFishingIncome, FishingIndustryFishingExpenses, FishingIndustryFishingProfit,
            CampusAreasTotalIncomePercent, CampusAreasTotalIncome, CampusAreasTotalExpensesPercent, CampusAreasTotalExpenses, CampusAreasTotalProfit,
            CampusAreasTradeSchoolIncome, CampusAreasTradeSchoolExpenses, CampusAreasTradeSchoolProfit,
            CampusAreasLiberalArtsCollegeIncome, CampusAreasLiberalArtsCollegeExpenses, CampusAreasLiberalArtsCollegeProfit,
            CampusAreasUniversityIncome, CampusAreasUniversityExpenses, CampusAreasUniversityProfit,
            TransportEconomyTotalIncomePercent, TransportEconomyTotalIncome, TransportEconomyTotalExpensesPercent, TransportEconomyTotalExpenses, TransportEconomyTotalProfit,
            TransportEconomyBusIncome, TransportEconomyBusExpenses, TransportEconomyBusProfit,
            TransportEconomyTrolleybusIncome, TransportEconomyTrolleybusExpenses, TransportEconomyTrolleybusProfit,
            TransportEconomyTramIncome, TransportEconomyTramExpenses, TransportEconomyTramProfit,
            TransportEconomyMetroIncome, TransportEconomyMetroExpenses, TransportEconomyMetroProfit,
            TransportEconomyTrainIncome, TransportEconomyTrainExpenses, TransportEconomyTrainProfit,
            TransportEconomyShipIncome, TransportEconomyShipExpenses, TransportEconomyShipProfit,
            TransportEconomyAirIncome, TransportEconomyAirExpenses, TransportEconomyAirProfit,
            TransportEconomyMonorailIncome, TransportEconomyMonorailExpenses, TransportEconomyMonorailProfit,
            TransportEconomyCableCarIncome, TransportEconomyCableCarExpenses, TransportEconomyCableCarProfit,
            TransportEconomyTaxiIncome, TransportEconomyTaxiExpenses, TransportEconomyTaxiProfit,
            TransportEconomyToursIncome, TransportEconomyToursExpenses, TransportEconomyToursProfit,
            TransportEconomyTollBoothIncome, TransportEconomyTollBoothExpenses, TransportEconomyTollBoothProfit,
            TransportEconomyMailExpenses, TransportEconomyMailProfit,
            TransportEconomySpaceElevatorExpenses, TransportEconomySpaceElevatorProfit,
            GameLimitsBuildingsUsedPercent, GameLimitsBuildingsUsed, GameLimitsBuildingsCapacity,
            GameLimitsCitizensUsedPercent, GameLimitsCitizensUsed, GameLimitsCitizensCapacity,
            GameLimitsCitizenUnitsUsedPercent, GameLimitsCitizenUnitsUsed, GameLimitsCitizenUnitsCapacity,
            GameLimitsCitizenInstancesUsedPercent, GameLimitsCitizenInstancesUsed, GameLimitsCitizenInstancesCapacity,
            GameLimitsDisastersUsedPercent, GameLimitsDisastersUsed, GameLimitsDisastersCapacity,
            GameLimitsDistrictsUsedPercent, GameLimitsDistrictsUsed, GameLimitsDistrictsCapacity,
            GameLimitsEventsUsedPercent, GameLimitsEventsUsed, GameLimitsEventsCapacity,
            GameLimitsGameAreasUsedPercent, GameLimitsGameAreasUsed, GameLimitsGameAreasCapacity,
            GameLimitsNetworkLanesUsedPercent, GameLimitsNetworkLanesUsed, GameLimitsNetworkLanesCapacity,
            GameLimitsNetworkNodesUsedPercent, GameLimitsNetworkNodesUsed, GameLimitsNetworkNodesCapacity,
            GameLimitsNetworkSegmentsUsedPercent, GameLimitsNetworkSegmentsUsed, GameLimitsNetworkSegmentsCapacity,
            GameLimitsPaintedAreasUsedPercent, GameLimitsPaintedAreasUsed, GameLimitsPaintedAreasCapacity,
            GameLimitsPathUnitsUsedPercent, GameLimitsPathUnitsUsed, GameLimitsPathUnitsCapacity,
            GameLimitsPropsUsedPercent, GameLimitsPropsUsed, GameLimitsPropsCapacity,
            GameLimitsRadioChannelsUsedPercent, GameLimitsRadioChannelsUsed, GameLimitsRadioChannelsCapacity,
            GameLimitsRadioContentsUsedPercent, GameLimitsRadioContentsUsed, GameLimitsRadioContentsCapacity,
            GameLimitsTransportLinesUsedPercent, GameLimitsTransportLinesUsed, GameLimitsTransportLinesCapacity,
            GameLimitsTreesUsedPercent, GameLimitsTreesUsed, GameLimitsTreesCapacity,
            GameLimitsVehiclesUsedPercent, GameLimitsVehiclesUsed, GameLimitsVehiclesCapacity,
            GameLimitsVehiclesParkedUsedPercent, GameLimitsVehiclesParkedUsed, GameLimitsVehiclesParkedCapacity,
            GameLimitsZoneBlocksUsedPercent, GameLimitsZoneBlocksUsed, GameLimitsZoneBlocksCapacity, 
            LocationHome, LocationWork, LocationMoving, LocationVisit, CitizenOnBike, CitizenWaiting, CitizenBored
        }

        // main properties set by the constructor
        private readonly Category _category;
        private readonly StatisticType _type;
        private readonly Translation.Key _descriptionKey1;
        private readonly Translation.Key _descriptionKey2;
        private readonly Translation.Key _unitsKey;
        private readonly Color32 _textColor;
        private readonly FieldInfo _snapshotField;
        private readonly PropertyInfo _snapshotProperty;

        // properties needed for UI operations
        private bool _selected;
        private bool _enabled;
        private string _descriptionUnits;

        // UI elements that are referenced after they are created
        private UIPanel _panel;
        private UISprite _checkbox;
        private UILabel _description;
        private UILabel _amount;

        // needed by the graph
        private string _categoryDescription;
        private string _categoryDescriptionUnits;
        private string _units;
        private readonly Color32 _lineColor;
        private readonly string _numberFormat;

        /// <summary>
        /// constructor to set main and other properties
        /// </summary>
        public Statistic(Category category, StatisticType type, Translation.Key descriptionKey1, Translation.Key descriptionKey2, Translation.Key unitsKey, Color32 textColor)
        {
            // save params
            _category = category;
            _type = type;
            _descriptionKey1 = descriptionKey1;
            _descriptionKey2 = descriptionKey2;
            _unitsKey = unitsKey;
            _textColor = textColor;

            // description key depends on DLC
            if (_descriptionKey1 == Translation.Key.WaterSewage && SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC))
            {
                _descriptionKey1 = Translation.Key.WaterSewageHeating;
            }

            // compute line color from text color
            // for unknown reasons, graph lines appear brighter than the text, so make the line color a bit darker than the text so they match more closely
            const float LineColorMultiplier = 0.75f;
            _lineColor = new Color32((byte)(_textColor.r * LineColorMultiplier), (byte)(_textColor.g * LineColorMultiplier), (byte)(_textColor.b * LineColorMultiplier), 255);

            // set number format
            if (_type.ToString().EndsWith("Percent") || _type.ToString().EndsWith("PerCapita") || _type == StatisticType.TourismExchangeStudentBonus)
            {
                _numberFormat = "N1";
            }
            else if (_type == StatisticType.ZoneLevelResidentialAverage ||
                     _type == StatisticType.ZoneLevelCommercialAverage  ||
                     _type == StatisticType.ZoneLevelIndustrialAverage  ||
                     _type == StatisticType.ZoneLevelOfficeAverage)
            {
                _numberFormat = "N2";
            }
            else
            {
                _numberFormat = "N0";
            }

            // get field or property info
            Snapshot.GetFieldProperty(_type, out _snapshotField, out _snapshotProperty);

            // initialize UI text
            UpdateUIText();

            // logging to get list of statistics
            // LogUtil.LogInfo("• " + _descriptionUnits);
        }

        // read-only accessors
        public StatisticType Type { get { return _type; } }
        public bool Enabled { get { return _enabled; } }
        public string CategoryDescription { get { return _categoryDescription; } }
        public string CategoryDescriptionUnits { get { return _categoryDescriptionUnits; } }
        public string Units { get { return _units; } }
        public Color32 LineColor { get { return _lineColor; } }
        public string NumberFormat { get { return _numberFormat; } }

        /// <summary>
        /// whether or not the statistic is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;

                // set the checkbox sprite
                if (_checkbox != null)
                {
                    _checkbox.spriteName = (value ? "AchievementCheckedTrue" : "AchievementCheckedFalse");
                }
            }
        }

        /// <summary>
        /// create statistic UI in the category
        /// </summary>
        public bool CreateUI()
        {
            // build name prefix
            string namePrefix = _type.ToString();

            // create a panel to hold the UI components
            _panel = _category.AddStatisticPanel();
            if (_panel == null)
            {
                LogUtil.LogError($"Unable to create statistic panel for [{_type}].");
                return false;
            }
            _panel.name = namePrefix + "Panel";
            _panel.autoSize = false;
            _panel.size = new Vector2(_panel.parent.size.x, UIHeight);
            _panel.relativePosition = new Vector3(0f, 0f);    // category panel uses auto layout
            _panel.eventClicked += Statistic_eventClicked;

            // create the checkbox
            const float componentHeight = UIHeight - 2f;
            _checkbox = _panel.AddUIComponent<UISprite>();
            if (_checkbox == null)
            {
                LogUtil.LogError($"Unable to create statistic checkbox for [{_type}].");
                return false;
            }
            _checkbox.name = namePrefix + "Checkbox";
            _checkbox.autoSize = false;
            _checkbox.size = new Vector2(componentHeight, componentHeight);
            _checkbox.relativePosition = new Vector3(20f, 0f);

            // create the description label
            _description = _panel.AddUIComponent<UILabel>();
            if (_description == null)
            {
                LogUtil.LogError($"Unable to create statistic description label for [{_type}].");
                return false;
            }
            _description.name = namePrefix + "Description";
            _description.textScale = 0.625f;
            _description.relativePosition = new Vector3(_checkbox.relativePosition.x + _checkbox.size.x + 3f, 3f);
            _description.autoSize = false;
            _description.size = new Vector2(_panel.size.x - _description.relativePosition.x, componentHeight);

            // create the amount label on top of the description
            _amount = _panel.AddUIComponent<UILabel>();
            if (_amount == null)
            {
                LogUtil.LogError($"Unable to create statistic amount label for [{_type}].");
                return false;
            }
            _amount.name = namePrefix + "Amount";
            _amount.textScale = _description.textScale;
            _amount.autoSize = false;
            _amount.size = new Vector2(_description.size.x - 100f, componentHeight);
            _amount.relativePosition = new Vector3(_panel.size.x - _amount.size.x - 2f, _description.relativePosition.y);
            _amount.textAlignment = UIHorizontalAlignment.Right;
            _amount.BringToFront();

            // set initial selection status to the status previously read from the game save file
            Selected = Selected;

            // get DLC flags (used often)
            // there are no dependencies on Match Day (aka Football) and Concerts (aka MusicFestival) DLCs
            bool dlcAfterDark           = SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);           // 09/24/15
            bool dlcSnowfall            = SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC);            // 02/18/16
            bool dlcNaturalDisasters    = SteamHelper.IsDLCOwned(SteamHelper.DLC.NaturalDisastersDLC);    // 11/29/16
            bool dlcMassTransit         = SteamHelper.IsDLCOwned(SteamHelper.DLC.InMotionDLC);            // 05/18/17
            bool dlcGreenCities         = SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);         // 10/19/17
            bool dlcParkLife            = SteamHelper.IsDLCOwned(SteamHelper.DLC.ParksDLC);               // 05/24/18
            bool dlcIndustries          = SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC);            // 10/23/18
            bool dlcCampus              = SteamHelper.IsDLCOwned(SteamHelper.DLC.CampusDLC);              // 05/21/19
            bool dlcSunsetHarbor        = SteamHelper.IsDLCOwned(SteamHelper.DLC.UrbanDLC);               // 03/26/20
            bool dlcAirports            = SteamHelper.IsDLCOwned(SteamHelper.DLC.AirportDLC);             // 01/25/22

            // disable statistic for inactive DLC
            // statistic is still present, just hidden so it cannot be selected
            _enabled = true;
            switch (_type)
            {
                case StatisticType.WaterTankReservedPercent:
                case StatisticType.WaterTankReserved:
                case StatisticType.WaterTankStorageCapacity:
                    DisableForInactiveDLC(dlcNaturalDisasters);
                    break;

                case StatisticType.PublicTransportationTrolleybusTotal:
                case StatisticType.PublicTransportationTrolleybusResidents:
                case StatisticType.PublicTransportationTrolleybusTourists:
                    DisableForInactiveDLC(dlcSunsetHarbor);
                    break;

                case StatisticType.PublicTransportationTramTotal:
                case StatisticType.PublicTransportationTramResidents:
                case StatisticType.PublicTransportationTramTourists:
                    DisableForInactiveDLC(dlcSnowfall);
                    break;

                case StatisticType.PublicTransportationMonorailTotal:
                case StatisticType.PublicTransportationMonorailResidents:
                case StatisticType.PublicTransportationMonorailTourists:
                    DisableForInactiveDLC(dlcMassTransit);
                    break;

                case StatisticType.PublicTransportationCableCarTotal:
                case StatisticType.PublicTransportationCableCarResidents:
                case StatisticType.PublicTransportationCableCarTourists:
                    DisableForInactiveDLC(dlcMassTransit);
                    break;

                case StatisticType.PublicTransportationTaxiTotal:
                case StatisticType.PublicTransportationTaxiResidents:
                case StatisticType.PublicTransportationTaxiTourists:
                    DisableForInactiveDLC(dlcAfterDark);
                    break;

                case StatisticType.OutsideConnectionsImportMail:
                case StatisticType.OutsideConnectionsExportMail:
                    DisableForInactiveDLC(dlcIndustries);
                    break;

                case StatisticType.OutsideConnectionsExportFish:
                    DisableForInactiveDLC(dlcSunsetHarbor);
                    break;

                case StatisticType.HeatingConsumptionPercent:
                case StatisticType.HeatingConsumption:
                case StatisticType.HeatingProduction:
                    DisableForInactiveDLC(dlcSnowfall);
                    break;

                case StatisticType.TourismExchangeStudentBonus:
                    DisableForInactiveDLC(dlcCampus);
                    break;

                case StatisticType.ToursTotalTotal:
                case StatisticType.ToursTotalResidents:
                case StatisticType.ToursTotalTourists:
                case StatisticType.ToursWalkingTourTotal:
                case StatisticType.ToursWalkingTourResidents:
                case StatisticType.ToursWalkingTourTourists:
                case StatisticType.ToursSightseeingTotal:
                case StatisticType.ToursSightseeingResidents:
                case StatisticType.ToursSightseeingTourists:
                case StatisticType.ToursBalloonTotal:
                case StatisticType.ToursBalloonResidents:
                case StatisticType.ToursBalloonToursits:
                    DisableForInactiveDLC(dlcParkLife);
                    break;

                case StatisticType.ResidentialIncomeLowDensitySelfSufficient:
                case StatisticType.ResidentialIncomeHighDensitySelfSufficient:
                    DisableForInactiveDLC(dlcGreenCities);
                    break;

                case StatisticType.CommercialIncomeSpecializedTotal:
                case StatisticType.CommercialIncomeLeisure:
                case StatisticType.CommercialIncomeTourism:
                    DisableForInactiveDLC(dlcAfterDark);
                    break;

                case StatisticType.CommercialIncomeOrganic:
                    DisableForInactiveDLC(dlcGreenCities);
                    break;

                case StatisticType.OfficeIncomeGenericTotal:
                case StatisticType.OfficeIncomeITCluster:
                    DisableForInactiveDLC(dlcGreenCities);
                    break;

                case StatisticType.TourismIncomeParkAreas:
                    DisableForInactiveDLC(dlcParkLife);
                    break;

                case StatisticType.ServiceExpensesEmergency:
                    DisableForInactiveDLC(dlcNaturalDisasters);
                    break;

                case StatisticType.ServiceExpensesGenericSportsArenas:
                    DisableForInactiveDLC(dlcCampus);
                    break;

                case StatisticType.ParkAreasTotalIncomePercent:
                case StatisticType.ParkAreasTotalIncome:
                case StatisticType.ParkAreasTotalExpensesPercent:
                case StatisticType.ParkAreasTotalExpenses:
                case StatisticType.ParkAreasTotalProfit:
                case StatisticType.ParkAreasCityParkIncome:
                case StatisticType.ParkAreasCityParkExpenses:
                case StatisticType.ParkAreasCityParkProfit:
                case StatisticType.ParkAreasAmusementParkIncome:
                case StatisticType.ParkAreasAmusementParkExpenses:
                case StatisticType.ParkAreasAmusementParkProfit:
                case StatisticType.ParkAreasZooIncome:
                case StatisticType.ParkAreasZooExpenses:
                case StatisticType.ParkAreasZooProfit:
                case StatisticType.ParkAreasNatureReserveIncome:
                case StatisticType.ParkAreasNatureReserveExpenses:
                case StatisticType.ParkAreasNatureReserveProfit:
                    DisableForInactiveDLC(dlcParkLife);
                    break;

                case StatisticType.IndustryAreasTotalIncomePercent:
                case StatisticType.IndustryAreasTotalIncome:
                case StatisticType.IndustryAreasTotalExpensesPercent:
                case StatisticType.IndustryAreasTotalExpenses:
                case StatisticType.IndustryAreasTotalProfit:
                case StatisticType.IndustryAreasForestryIncome:
                case StatisticType.IndustryAreasForestryExpenses:
                case StatisticType.IndustryAreasForestryProfit:
                case StatisticType.IndustryAreasFarmingIncome:
                case StatisticType.IndustryAreasFarmingExpenses:
                case StatisticType.IndustryAreasFarmingProfit:
                case StatisticType.IndustryAreasOreIncome:
                case StatisticType.IndustryAreasOreExpenses:
                case StatisticType.IndustryAreasOreProfit:
                case StatisticType.IndustryAreasOilIncome:
                case StatisticType.IndustryAreasOilExpenses:
                case StatisticType.IndustryAreasOilProfit:
                case StatisticType.IndustryAreasWarehousesFactoriesIncome:
                case StatisticType.IndustryAreasWarehousesFactoriesExpenses:
                case StatisticType.IndustryAreasWarehousesFactoriesProfit:
                    DisableForInactiveDLC(dlcIndustries);
                    break;

                case StatisticType.FishingIndustryFishingIncome:
                case StatisticType.FishingIndustryFishingExpenses:
                case StatisticType.FishingIndustryFishingProfit:
                    DisableForInactiveDLC(dlcSunsetHarbor);
                    break;

                case StatisticType.CampusAreasTotalIncomePercent:
                case StatisticType.CampusAreasTotalIncome:
                case StatisticType.CampusAreasTotalExpensesPercent:
                case StatisticType.CampusAreasTotalExpenses:
                case StatisticType.CampusAreasTotalProfit:
                case StatisticType.CampusAreasTradeSchoolIncome:
                case StatisticType.CampusAreasTradeSchoolExpenses:
                case StatisticType.CampusAreasTradeSchoolProfit:
                case StatisticType.CampusAreasLiberalArtsCollegeIncome:
                case StatisticType.CampusAreasLiberalArtsCollegeExpenses:
                case StatisticType.CampusAreasLiberalArtsCollegeProfit:
                case StatisticType.CampusAreasUniversityIncome:
                case StatisticType.CampusAreasUniversityExpenses:
                case StatisticType.CampusAreasUniversityProfit:
                    DisableForInactiveDLC(dlcCampus);
                    break;

                case StatisticType.TransportEconomyTrolleybusIncome:
                case StatisticType.TransportEconomyTrolleybusExpenses:
                case StatisticType.TransportEconomyTrolleybusProfit:
                    DisableForInactiveDLC(dlcSunsetHarbor);
                    break;

                case StatisticType.TransportEconomyTramIncome:
                case StatisticType.TransportEconomyTramExpenses:
                case StatisticType.TransportEconomyTramProfit:
                    DisableForInactiveDLC(dlcSnowfall);
                    break;

                case StatisticType.TransportEconomyMonorailIncome:
                case StatisticType.TransportEconomyMonorailExpenses:
                case StatisticType.TransportEconomyMonorailProfit:
                    DisableForInactiveDLC(dlcMassTransit);
                    break;

                case StatisticType.TransportEconomyCableCarIncome:
                case StatisticType.TransportEconomyCableCarExpenses:
                case StatisticType.TransportEconomyCableCarProfit:
                    DisableForInactiveDLC(dlcMassTransit);
                    break;

                case StatisticType.TransportEconomyTaxiIncome:
                case StatisticType.TransportEconomyTaxiExpenses:
                case StatisticType.TransportEconomyTaxiProfit:
                    DisableForInactiveDLC(dlcAfterDark);
                    break;

                case StatisticType.TransportEconomyToursIncome:
                case StatisticType.TransportEconomyToursExpenses:
                case StatisticType.TransportEconomyToursProfit:
                    DisableForInactiveDLC(dlcParkLife);
                    break;

                case StatisticType.TransportEconomyMailExpenses:
                case StatisticType.TransportEconomyMailProfit:
                    DisableForInactiveDLC(dlcIndustries);
                    break;

                case StatisticType.GameLimitsDisastersUsedPercent:
                case StatisticType.GameLimitsDisastersUsed:
                case StatisticType.GameLimitsDisastersCapacity:
                    DisableForInactiveDLC(dlcNaturalDisasters);
                    break;

                case StatisticType.GameLimitsPaintedAreasUsedPercent:
                case StatisticType.GameLimitsPaintedAreasUsed:
                case StatisticType.GameLimitsPaintedAreasCapacity:
                    DisableForInactiveDLC(dlcParkLife || dlcIndustries || dlcCampus || dlcAirports);
                    break;
            }

            // success
            return true;
        }

        /// <summary>
        /// handle click on statistic panel
        /// </summary>
        private void Statistic_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // toggle only if currently selected (i.e. will be unselected) OR not currently selected and selecting it would not exceed max selected
            if (Selected || (!Selected && Categories.instance.CountSelected < MainPanel.MaximumSelectedStatistics))
            {
                // toggle selected status and update main panel
                Selected = !Selected;
                UserInterface.instance.UpdateMainPanel();
                eventParam.Use();
            }
        }

        /// <summary>
        /// disable, deselect, and hide the statistic if DLC is inactive
        /// </summary>
        private void DisableForInactiveDLC(bool dlc)
        {
            if (!dlc)
            {
                _enabled = false;
                Selected = false;
                _panel.isVisible = false;
            }
        }

        /// <summary>
        /// update UI text
        /// </summary>
        public void UpdateUIText()
        {
            // obtain the translated description, which may or may not have a second part
            string description = Translation.instance.Get(_descriptionKey1);
            if (_descriptionKey2 != Translation.Key.None)
            {
                description += "-" + Translation.instance.Get(_descriptionKey2);
            }

            // obtain the translated units
            _units = Translation.instance.Get(_unitsKey);

            // combine description and units
            _descriptionUnits = description + " (" + _units + ")";

            // combine categtory and description
            _categoryDescription = _category.Description + "-" + description;

            // combine categtory, description, and units
            _categoryDescriptionUnits = _category.Description + "-" + _descriptionUnits;

            // update description
            if (_description != null)
            {
                _description.text = _descriptionUnits;
                _description.textColor = _textColor;
            }

            // update amount
            if (_amount != null)
            {
                _amount.textColor = _textColor;
            }
        }

        /// <summary>
        /// update the amount from the snapshot
        /// </summary>
        public void UpdateAmount(Snapshot snapshot)
        {
            // make sure label is valid
            if (_amount != null)
            {
                // get the value from the field or property
                object snapshotValue = null;
                if (_snapshotField != null)
                {
                    snapshotValue = _snapshotField.GetValue(snapshot);
                }
                else if (_snapshotProperty != null)
                {
                    snapshotValue = _snapshotProperty.GetValue(snapshot, null);
                }

                // display the value
                if (snapshotValue == null)
                {
                    _amount.text = "";
                }
                else
                {
                    _amount.text = System.Convert.ToDouble(snapshotValue).ToString(_numberFormat, LocaleManager.cultureInfo);
                }
            }
        }

        /// <summary>
        /// write the statistic to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // write selection status
            writer.Write(Selected);
        }

        /// <summary>
        /// read the statistic from the game save file
        /// </summary>
        public void Deserialize(BinaryReader reader, int version)
        {
            // check version
            if (version < 3 &&
                (_type == StatisticType.CityEconomyLoanBalance                      ||
                 _type == StatisticType.CityEconomyCityValue                        ||
                 _type == StatisticType.CityEconomyCityValuePerCapita               ||
                 _type == StatisticType.CityEconomyGrossDomesticProduct             ||
                 _type == StatisticType.CityEconomyGrossDomesticProductPerCapita    ||
                 _type == StatisticType.CityEconomyConsumption                      ||
                 _type == StatisticType.CityEconomyConsumptionPercent               ||
                 _type == StatisticType.CityEconomyGovernmentSpending               ||
                 _type == StatisticType.CityEconomyGovernmentSpendingPercent        ||
                 _type == StatisticType.CityEconomyExports                          ||
                 _type == StatisticType.CityEconomyImports                          ||
                 _type == StatisticType.CityEconomyNetExports                       ||
                 _type == StatisticType.CityEconomyNetExportsPercent))
            {
                // not selected by default
                Selected = false;
            }
            else
            {
                // read selection status
                Selected = reader.ReadBoolean();
            }
        }
    }
}
