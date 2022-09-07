using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RealTimeStatistics
{
    /// <summary>
    /// the list of categories
    /// </summary>
    public class Categories : List<Category>
    {
        // use singleton pattern:  there can be only one list of categories in the game
        private static readonly Categories _instance = new Categories();
        public static Categories instance { get { return _instance; } }
        private Categories() { }

        /// <summary>
        /// initialize categories
        /// </summary>
        public void Initialize()
        {
            // define statistic colors
            // generally for colors:
            //      colors are lighter for usage and darker for capacity
            //      usage percent color is same as usage amount color because it is expected that percent and amount will not be shown at the same time and using the same color reduces the number of unique colors to be defined
            #region Colors

            // define multiplier to make a color darker
            const float DarkerMultipler = 0.7f;

            // get colors from InfoManager
            if (!InfoManager.exists)
            {
                LogUtil.LogError("InfoManager not ready.");
                return;
            }
            Color colorNeutral1 = InfoManager.instance.m_properties.m_neutralColor; Color colorNeutral2 = colorNeutral1 * DarkerMultipler;
            InfoProperties.ModeProperties[] modeProperties = InfoManager.instance.m_properties.m_modeProperties;
            Color colorInfoTrafficTarget  = modeProperties[(int)InfoManager.InfoMode.Traffic       ].m_targetColor;
            Color colorInfoPollution      = modeProperties[(int)InfoManager.InfoMode.Pollution     ].m_activeColor;
            Color colorInfoNoisePollution = modeProperties[(int)InfoManager.InfoMode.NoisePollution].m_activeColor;
            Color colorInfoLandValue      = modeProperties[(int)InfoManager.InfoMode.LandValue     ].m_activeColor;
            Color colorInfoHeating1       = modeProperties[(int)InfoManager.InfoMode.Heating       ].m_targetColor; Color32 colorInfoHeating2 = colorInfoHeating1 * DarkerMultipler;
            Color colorInfoTourism        = modeProperties[(int)InfoManager.InfoMode.Tourism       ].m_activeColor;

            // get colors from ZoneManager
            if (!ZoneManager.exists)
            {
                LogUtil.LogError("ZoneManager not ready.");
                return;
            }
            Color[] zoneColors = ZoneManager.instance.m_properties.m_zoneColors;
            Color colorZoneResidentialLow  = zoneColors[(int)ItemClass.Zone.ResidentialLow ];
            Color colorZoneResidentialHigh = zoneColors[(int)ItemClass.Zone.ResidentialHigh];
            Color colorZoneCommercialLow   = zoneColors[(int)ItemClass.Zone.CommercialLow  ];
            Color colorZoneCommercialHigh  = zoneColors[(int)ItemClass.Zone.CommercialHigh ];
            Color colorZoneIndustrial      = zoneColors[(int)ItemClass.Zone.Industrial     ];
            Color colorZoneOffice          = zoneColors[(int)ItemClass.Zone.Office         ];
            Color32 colorZoneUnzoned       = new Color32(152, 106, 65, 255);  // color taken manually from De-Zone tool icon

            // make mid colors halfway between low and high colors
            Color colorZoneResidentialMid = Color.Lerp(colorZoneResidentialLow, colorZoneResidentialHigh, 0.5f);
            Color colorZoneCommercialMid  = Color.Lerp(colorZoneCommercialLow,  colorZoneCommercialHigh,  0.5f);


            // get colors from TransportManager
            if (!TransportManager.exists)
            {
                LogUtil.LogError("TransportManager not ready.");
                return;
            }
            Color[] transportColors = TransportManager.instance.m_properties.m_transportColors;
            Color32 colorTransportBus1           = transportColors[(int)TransportInfo.TransportType.Bus          ]; Color32 colorTransportBus2           = colorTransportBus1          .Multiply(DarkerMultipler); Color32 colorTransportBus3        = colorTransportBus2       .Multiply(DarkerMultipler);
            Color32 colorTransportTrolleybus1    = transportColors[(int)TransportInfo.TransportType.Trolleybus   ]; Color32 colorTransportTrolleybus2    = colorTransportTrolleybus1   .Multiply(DarkerMultipler); Color32 colorTransportTrolleybus3 = colorTransportTrolleybus2.Multiply(DarkerMultipler);
            Color32 colorTransportTram1          = transportColors[(int)TransportInfo.TransportType.Tram         ]; Color32 colorTransportTram2          = colorTransportTram1         .Multiply(DarkerMultipler); Color32 colorTransportTram3       = colorTransportTram2      .Multiply(DarkerMultipler);
            Color32 colorTransportMetro1         = transportColors[(int)TransportInfo.TransportType.Metro        ]; Color32 colorTransportMetro2         = colorTransportMetro1        .Multiply(DarkerMultipler); Color32 colorTransportMetro3      = colorTransportMetro2     .Multiply(DarkerMultipler);
            Color32 colorTransportTrain1         = transportColors[(int)TransportInfo.TransportType.Train        ]; Color32 colorTransportTrain2         = colorTransportTrain1        .Multiply(DarkerMultipler); Color32 colorTransportTrain3      = colorTransportTrain2     .Multiply(DarkerMultipler);
            Color32 colorTransportShip1          = transportColors[(int)TransportInfo.TransportType.Ship         ]; Color32 colorTransportShip2          = colorTransportShip1         .Multiply(DarkerMultipler); Color32 colorTransportShip3       = colorTransportShip2      .Multiply(DarkerMultipler);
            Color32 colorTransportAir1           = transportColors[(int)TransportInfo.TransportType.Airplane     ]; Color32 colorTransportAir2           = colorTransportAir1          .Multiply(DarkerMultipler); Color32 colorTransportAir3        = colorTransportAir2       .Multiply(DarkerMultipler);
            Color32 colorTransportMonorail1      = transportColors[(int)TransportInfo.TransportType.Monorail     ]; Color32 colorTransportMonorail2      = colorTransportMonorail1     .Multiply(DarkerMultipler); Color32 colorTransportMonorail3   = colorTransportMonorail2  .Multiply(DarkerMultipler);
            Color32 colorTransportCableCar1      = transportColors[(int)TransportInfo.TransportType.CableCar     ]; Color32 colorTransportCableCar2      = colorTransportCableCar1     .Multiply(DarkerMultipler); Color32 colorTransportCableCar3   = colorTransportCableCar2  .Multiply(DarkerMultipler);
            Color32 colorTransportTaxi1          = transportColors[(int)TransportInfo.TransportType.Taxi         ]; Color32 colorTransportTaxi2          = colorTransportTaxi1         .Multiply(DarkerMultipler); Color32 colorTransportTaxi3       = colorTransportTaxi2      .Multiply(DarkerMultipler);
            Color32 colorTransportPedestrian1    = transportColors[(int)TransportInfo.TransportType.Pedestrian   ]; Color32 colorTransportPedestrian2    = colorTransportPedestrian1   .Multiply(DarkerMultipler); Color32 colorTransportPedestrian3 = colorTransportPedestrian2.Multiply(DarkerMultipler);
            Color32 colorTransportTouristBus1    = transportColors[(int)TransportInfo.TransportType.TouristBus   ]; Color32 colorTransportTouristBus2    = colorTransportTouristBus1   .Multiply(DarkerMultipler);
            Color32 colorTransportHotAirBalloon1 = transportColors[(int)TransportInfo.TransportType.HotAirBalloon]; Color32 colorTransportHotAirBalloon2 = colorTransportHotAirBalloon1.Multiply(DarkerMultipler);

            // get colors from TransferManager
            if (!TransferManager.exists)
            {
                LogUtil.LogError("TransferManager not ready.");
                return;
            }
            Color[] transferColors = TransferManager.instance.m_properties.m_resourceColors;
            Color32 colorTransferGoods1    = transferColors[(int)TransferManager.TransferReason.Goods]; Color32 colorTransferGoods2    = colorTransferGoods1   .Multiply(DarkerMultipler);
            Color32 colorTransferForestry1 = transferColors[(int)TransferManager.TransferReason.Logs ]; Color32 colorTransferForestry2 = colorTransferForestry1.Multiply(DarkerMultipler); Color32 colorTransferForestry3 = colorTransferForestry2.Multiply(DarkerMultipler);
            Color32 colorTransferFarming1  = transferColors[(int)TransferManager.TransferReason.Grain]; Color32 colorTransferFarming2  = colorTransferFarming1 .Multiply(DarkerMultipler); Color32 colorTransferFarming3  = colorTransferFarming2 .Multiply(DarkerMultipler);
            Color32 colorTransferOre1      = transferColors[(int)TransferManager.TransferReason.Ore  ]; Color32 colorTransferOre2      = colorTransferOre1     .Multiply(DarkerMultipler); Color32 colorTransferOre3      = colorTransferOre2     .Multiply(DarkerMultipler);
            Color32 colorTransferOil1      = transferColors[(int)TransferManager.TransferReason.Oil  ];
            Color32 colorTransferMail1     = transferColors[(int)TransferManager.TransferReason.Mail ]; Color32 colorTransferMail2     = colorTransferMail1    .Multiply(DarkerMultipler); Color32 colorTransferMail3     = colorTransferMail2    .Multiply(DarkerMultipler);
            Color32 colorTransferFish1     = transferColors[(int)TransferManager.TransferReason.Fish ]; Color32 colorTransferFish2     = colorTransferFish1    .Multiply(DarkerMultipler); Color32 colorTransferFish3     = colorTransferFish2    .Multiply(DarkerMultipler);

            // make oil lighter because the original color is very dark
            colorTransferOil1 = colorTransferOil1.Multiply(1.4f);
            Color32 colorTransferOil2 = colorTransferOil1.Multiply(DarkerMultipler);
            Color32 colorTransferOil3 = colorTransferOil2.Multiply(DarkerMultipler);

            // get colors from NaturalResourceManager
            if (!NaturalResourceManager.exists)
            {
                LogUtil.LogError("NaturalResourceManager not ready.");
                return;
            }
            Color[] resourceColors = NaturalResourceManager.instance.m_properties.m_resourceColors;
            Color32 colorResourceForestry1  = resourceColors[(int)NaturalResourceManager.Resource.Forest   ]; Color32 colorResourceForestry2  = colorResourceForestry1 .Multiply(DarkerMultipler);
            Color32 colorResourceFertility1 = resourceColors[(int)NaturalResourceManager.Resource.Fertility]; Color32 colorResourceFertility2 = colorResourceFertility1.Multiply(DarkerMultipler);
            Color32 colorResourceOre1       = resourceColors[(int)NaturalResourceManager.Resource.Ore      ]; Color32 colorResourceOre2       = colorResourceOre1      .Multiply(DarkerMultipler);
            Color32 colorResourceOil1       = resourceColors[(int)NaturalResourceManager.Resource.Oil      ];

            // make oil lighter because the original color is very dark
            colorResourceOil1 = colorResourceOil1.Multiply(1.5f);
            Color32 colorResourceOil2 = colorResourceOil1.Multiply(DarkerMultipler);

            // get colors from EducationInfoViewPanel
            EducationInfoViewPanel educationInfoViewPanel = UIView.library.Get<EducationInfoViewPanel>(typeof(EducationInfoViewPanel).Name);
            if (educationInfoViewPanel == null)
            {
                LogUtil.LogError("Unable to find EducationInfoViewPanel.");
                return;
            }
            Color32 colorUneducated1     = educationInfoViewPanel.m_UneducatedColor;     Color32 colorUneducated2     = colorUneducated1    .Multiply(DarkerMultipler);
            Color32 colorEducated1       = educationInfoViewPanel.m_EducatedColor;       Color32 colorEducated2       = colorEducated1      .Multiply(DarkerMultipler);
            Color32 colorWellEducated1   = educationInfoViewPanel.m_WellEducatedColor;   Color32 colorWellEducated2   = colorWellEducated1  .Multiply(DarkerMultipler);
            Color32 colorHighlyEducated1 = educationInfoViewPanel.m_HighlyEducatedColor; Color32 colorHighlyEducated2 = colorHighlyEducated1.Multiply(DarkerMultipler);

            // get colors from PopulationInfoViewPanel
            PopulationInfoViewPanel populationInfoViewPanel = UIView.library.Get<PopulationInfoViewPanel>(typeof(PopulationInfoViewPanel).Name);
            if (populationInfoViewPanel == null)
            {
                LogUtil.LogError("Unable to find PopulationInfoViewPanel.");
                return;
            }
            Color32 colorChild  = populationInfoViewPanel.m_ChildColor;
            Color32 colorTeen   = populationInfoViewPanel.m_TeenColor;
            Color32 colorYoung  = populationInfoViewPanel.m_YoungColor;
            Color32 colorAdult  = populationInfoViewPanel.m_AdultColor;
            Color32 colorSenior = populationInfoViewPanel.m_SeniorColor;

            // get colors from TourismInfoViewPanel
            TourismInfoViewPanel tourismInfoViewPanel = UIView.library.Get<TourismInfoViewPanel>(typeof(TourismInfoViewPanel).Name);
            if (tourismInfoViewPanel == null)
            {
                LogUtil.LogError("Unable to find TourismInfoViewPanel.");
                return;
            }
            UIRadialChart touristWealthChart   = tourismInfoViewPanel.Find<UIRadialChart>("TouristWealthChart");
            UIRadialChart exchangeStudentChart = tourismInfoViewPanel.Find<UIRadialChart>("ExchangeStudentChart");
            Color32 colorTouristsLowWealth1    = touristWealthChart.GetSlice(0).innerColor; Color32 colorTouristsLowWealth2    = colorTouristsLowWealth1   .Multiply(DarkerMultipler);
            Color32 colorTouristsMediumWealth1 = touristWealthChart.GetSlice(1).innerColor; Color32 colorTouristsMediumWealth2 = colorTouristsMediumWealth1.Multiply(DarkerMultipler);
            Color32 colorTouristsHighWealth1   = touristWealthChart.GetSlice(2).innerColor; Color32 colorTouristsHighWealth2   = colorTouristsHighWealth1  .Multiply(DarkerMultipler);
            Color32 colorExchangeStudent       = exchangeStudentChart.GetSlice(0).innerColor;
            Color32 colorTouristsTotal         = exchangeStudentChart.GetSlice(1).innerColor;

            // get colors from StatisticsPanel
            // statistic color index is same as statistic name index
            StatisticsPanel statisticsPanel = UIView.library.Get<StatisticsPanel>(typeof(StatisticsPanel).Name);
            if (statisticsPanel == null)
            {
                LogUtil.LogError("Unable to find StatisticsPanel.");
                return;
            }
            FieldInfo fiStatisticsNames = typeof(StatisticsPanel).GetField("StatisticsNames", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fiStatisticsNames == null)
            {
                LogUtil.LogError("Unable to find StatisticsPanel.StatisticsNames.");
                return;
            }
            string[] statisticsNames = (string[])fiStatisticsNames.GetValue(statisticsPanel);
            if (statisticsNames == null)
            {
                LogUtil.LogError("Unable to get StatisticsPanel.StatisticsNames.");
                return;
            }
            Color32 colorStatisticsHappiness  = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsBirthRate  = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsDeathRate  = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsPopulation = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsEmployment = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsJobs       = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsCityValue  = new Color32(0, 0, 0, 0);
            Color32 colorStatisticsCityBudget = new Color32(0, 0, 0, 0);
            for (int i = 0; i < statisticsNames.Length; i++)
            {
                switch (statisticsNames[i])
                {
                    case "Happiness":  colorStatisticsHappiness  = statisticsPanel.StatisticsColors[i]; break;
                    case "Birth":      colorStatisticsBirthRate  = statisticsPanel.StatisticsColors[i]; break;
                    case "Death":      colorStatisticsDeathRate  = statisticsPanel.StatisticsColors[i]; break;
                    case "Population": colorStatisticsPopulation = statisticsPanel.StatisticsColors[i]; break;
                    case "Employment": colorStatisticsEmployment = statisticsPanel.StatisticsColors[i]; break;
                    case "Jobs":       colorStatisticsJobs       = statisticsPanel.StatisticsColors[i]; break;
                    case "Value":      colorStatisticsCityValue  = statisticsPanel.StatisticsColors[i]; break;
                    case "Budget":     colorStatisticsCityBudget = statisticsPanel.StatisticsColors[i]; break;
                }
            }

            // define colors for specific statistics
            Color32 colorLocation1 = new Color32(72, 245, 39, 255);
            Color32 colorLocation2 = new Color32(39, 226, 245, 255);
            Color32 colorLocation3 = new Color32(221, 185, 110, 255);
            Color32 colorLocation4 = new Color32(206, 248, 000, 255);

            Color32 colorCitizen1 = new Color32(144, 245, 185, 255);
            Color32 colorCitizen2 = new Color32(235, 232, 53, 255);
            Color32 colorCitizen3 = new Color32(237, 22, 22, 255);

            Color32 colorWater1 = new Color32(134, 187, 241, 255);                 // color taken manually from Water info view icon
            Color32 colorWater2 = new Color32(023, 113, 206, 255);                 // color taken manually from Water info view icon

            Color32 colorWaterTank1 = colorWater1.Multiply(DarkerMultipler);      // darker version of Water
            Color32 colorWaterTank2 = colorWater2.Multiply(DarkerMultipler);      // darker version of Water

            Color32 colorSewage1 = new Color32(038, 153, 134, 255);                // color taken manually from WaterInfoViewPanel sewage section
            Color32 colorSewage2 = colorSewage1.Multiply(DarkerMultipler);

            Color32 colorGarbage1 = new Color32(163, 152, 000, 255);               // color taken manually from Garbage info view icon
            Color32 colorGarbage2 = new Color32(091, 076, 069, 255);               // color taken manually from Garbage info view icon

            Color32 colorLandfill1 = colorGarbage1.Multiply(1.4f);                // lighter version of Garbage
            Color32 colorLandfill2 = colorGarbage2.Multiply(1.4f);                // lighter version of Garbage

            Color32 colorHealthcareAverage = new Color32(013, 166, 066, 255);      // color taken manually from HealthInfoViewPanel Efficiency legend
            Color32 colorHealthcare1 = new Color32(248, 021, 013, 255);            // color taken manually from Healthcare toolbar icon
            Color32 colorHealthcare2 = colorHealthcare1.Multiply(DarkerMultipler);

            Color32 colorDeathcare1 = new Color32(255, 000, 128, 255);             // a shade of red
            Color32 colorDeathcare2 = colorDeathcare1.Multiply(DarkerMultipler);
            Color32 colorDeathcare3 = new Color32(255, 128, 128, 255);             // another shade of red
            Color32 colorDeathcare4 = colorDeathcare3.Multiply(DarkerMultipler);

            // color is half way between Child and Teen
            Color32 colorChildcare1 = new Color32((byte)(colorChild.r / 2 + colorTeen.r / 2), (byte)(colorChild.g / 2 + colorTeen.g / 2), (byte)(colorChild.b / 2 + colorTeen.b / 2), 255);
            Color32 colorChildcare2 = colorChildcare1.Multiply(DarkerMultipler);

            Color32 colorEldercare1 = colorSenior.Multiply(1.25f);                // lighter version of Senior
            Color32 colorEldercare2 = colorEldercare1.Multiply(DarkerMultipler);

            // logic for levels colors is copied from LevelsInfoViewPanel.UpdatePanel for initializing the radial chart colors
            // basically start with a darker (i.e. 70%) version of each zone color and then interpolate between neutral color and that color
            Color colorLevelResidential = colorZoneResidentialMid * 0.7f;
            Color colorLevelCommercial  = colorZoneCommercialMid * 0.7f;
            Color colorLevelIndustrial  = colorZoneIndustrial * 0.7f;
            Color colorLevelOffice      = colorZoneOffice * 0.7f;
            Color colorResidentialLevel1 = Color.Lerp(colorNeutral1, colorLevelResidential, 0.200f);
            Color colorResidentialLevel2 = Color.Lerp(colorNeutral1, colorLevelResidential, 0.400f);
            Color colorResidentialLevel3 = Color.Lerp(colorNeutral1, colorLevelResidential, 0.600f);
            Color colorResidentialLevel4 = Color.Lerp(colorNeutral1, colorLevelResidential, 0.800f);
            Color colorResidentialLevel5 = Color.Lerp(colorNeutral1, colorLevelResidential, 1.000f);
            Color colorCommercialLevel1  = Color.Lerp(colorNeutral1, colorLevelCommercial,  0.333f);
            Color colorCommercialLevel2  = Color.Lerp(colorNeutral1, colorLevelCommercial,  0.667f);
            Color colorCommercialLevel3  = Color.Lerp(colorNeutral1, colorLevelCommercial,  1.000f);
            Color colorIndustrialLevel1  = Color.Lerp(colorNeutral1, colorLevelIndustrial,  0.333f);
            Color colorIndustrialLevel2  = Color.Lerp(colorNeutral1, colorLevelIndustrial,  0.667f);
            Color colorIndustrialLevel3  = Color.Lerp(colorNeutral1, colorLevelIndustrial,  1.000f);
            Color colorOfficeLevel1      = Color.Lerp(colorNeutral1, colorLevelOffice,      0.333f);
            Color colorOfficeLevel2      = Color.Lerp(colorNeutral1, colorLevelOffice,      0.667f);
            Color colorOfficeLevel3      = Color.Lerp(colorNeutral1, colorLevelOffice,      1.000f);

            Color32 colorFireSafety = new Color32(232, 159, 056, 255);             // color taken manually from Fire Department toolbar icon

            Color32 colorCrimeRate = new Color32(128, 128, 128, 255);              // a shade of gray
            Color32 colorCrime1    = new Color32(169, 095, 002, 255);              // color taken manually from Police Department toolbar icon
            Color32 colorCrime2    = colorCrime1.Multiply(DarkerMultipler);

            Color32 colorTransportTotal1 = new Color32(206, 248, 000, 255);        // color taken manually from TransportInfoViewPanel for Total text
            Color32 colorTransportTotal2 = colorTransportTotal1.Multiply(DarkerMultipler);

            Color32 colorHouseholds1 = new Color32(206, 248, 000, 255);            // color taken manually from CityInfoPanel for households text
            Color32 colorHouseholds2 = colorHouseholds1.Multiply(DarkerMultipler);

            Color32 colorEmployment1 = colorStatisticsJobs;
            Color32 colorEmployment2 = colorEmployment1.Multiply(DarkerMultipler);
            Color32 colorEmployment3 = colorEmployment2.Multiply(DarkerMultipler);
            Color32 colorUnemployment1 = colorStatisticsEmployment;
            Color32 colorUnemployment2 = colorUnemployment1.Multiply(DarkerMultipler);

            Color32 colorTransferTotal1 = new Color32(128, 128, 128, 255);         // a shade of gray
            Color32 colorTransferTotal2 = colorTransferTotal1.Multiply(DarkerMultipler);
            Color32 colorTransferTotal3 = colorTransferTotal2.Multiply(DarkerMultipler);

            Color32 colorCityTotalIncome   = new Color32(090, 225, 020, 255);      // color taken manually from EconomyPanel
            Color32 colorCityTotalExpenses = new Color32(254, 150, 089, 255);      // color taken manually from EconomyPanel
            Color32 colorCityTotalProfit   = new Color32((byte)(colorCityTotalIncome.r / 2 + colorCityTotalExpenses.r / 2),     // color is halfway between income and expenses
                                                         (byte)(colorCityTotalIncome.g / 2 + colorCityTotalExpenses.g / 2),
                                                         (byte)(colorCityTotalIncome.b / 2 + colorCityTotalExpenses.b / 2), 255);
            Color32 colorBankBalance       = new Color32(185, 221, 254, 255);      // color taken manually from InfoPanel.IncomePanel

            Color32 colorIncomeSelfSufficient = new Color32(118, 234, 122, 255);   // color taken manually from EconomyPanel

            Color32 colorIncomeLeisure = new Color32(135, 209, 218, 255);          // color taken manually from specialized district icon
            Color32 colorTourismIncome = new Color32(242, 219, 057, 255);          // color taken manually from specialized district icon
            Color32 colorIncomeOrganic = new Color32(132, 159, 000, 255);          // color taken manually from specialized district icon

            Color32 colorIncomeITCluster = new Color32(039, 192, 231, 255);        // color taken manually from specialized district icon

            Color32 colorParks = new Color32(073, 115, 122, 255);                  // color taken manually from the horse of the Parks & Plazas toolbar icon

            Color32 colorEmergency           = new Color32(254, 131, 000, 255);    // color taken manually from Landscape toolbar Disaster tab icon
            Color32 colorUniqueBuildings     = new Color32(082, 108, 113, 255);    // color taken manually from Unique Buildings toolbar icon
            Color32 colorGenericSportsArenas = new Color32(076, 108, 173, 255);    // color taken manually from Education toolbar Varsity Sports tab icon
            Color32 colorEconomy             = new Color32(061, 159, 010, 255);    // color taken manually from Economy toolbar icon
            Color32 colorPolicies            = new Color32(208, 210, 211, 255);    // color taken manually from Policies toolbar icon

            Color32 colorCityPark1      = new Color32(244, 223, 168, 255);         // color taken manually from City Park main gate arch
            Color32 colorCityPark2      = colorCityPark1.Multiply(DarkerMultipler);
            Color32 colorCityPark3      = colorCityPark2.Multiply(DarkerMultipler);
            Color32 colorAmusementPark1 = new Color32(204, 136, 083, 255);         // color taken manually from Amusement Park main gate path
            Color32 colorAmusementPark2 = colorAmusementPark1.Multiply(DarkerMultipler);
            Color32 colorAmusementPark3 = colorAmusementPark2.Multiply(DarkerMultipler);
            Color32 colorZoo1           = new Color32(221, 185, 110, 255);         // color taken manually from Zoo main gate path
            Color32 colorZoo2           = colorZoo1.Multiply(DarkerMultipler);
            Color32 colorZoo3           = colorZoo2.Multiply(DarkerMultipler);
            Color32 colorNatureReserve1 = new Color32(098, 145, 078, 255);         // color taken manually from Nature Reserve main gate building roof
            Color32 colorNatureReserve2 = colorNatureReserve1.Multiply(DarkerMultipler);
            Color32 colorNatureReserve3 = colorNatureReserve2.Multiply(DarkerMultipler);

            Color32 colorTradeSchool1 = new Color32(232, 216, 172, 255);           // color taken manually from Trade School administration building roof
            Color32 colorTradeSchool2 = colorTradeSchool1.Multiply(DarkerMultipler);
            Color32 colorTradeSchool3 = colorTradeSchool2.Multiply(DarkerMultipler);
            Color32 colorLiberalArts1 = new Color32(241, 181, 113, 255);           // color taken manually from Liberal Arts College administration building roof
            Color32 colorLiberalArts2 = colorLiberalArts1.Multiply(DarkerMultipler);
            Color32 colorLiberalArts3 = colorLiberalArts2.Multiply(DarkerMultipler);
            Color32 colorUniversity1  = new Color32(172, 208, 203, 255);           // color taken manually from University administration building roof
            Color32 colorUniversity2  = colorUniversity1.Multiply(DarkerMultipler);
            Color32 colorUniversity3  = colorUniversity2.Multiply(DarkerMultipler);

            Color32 colorTollBooth1 = new Color32(183, 148, 205, 255);             // color taken manually from Road toolbar Toll Booth icon car
            Color32 colorTollBooth2 = colorTollBooth1.Multiply(DarkerMultipler);
            Color32 colorTollBooth3 = colorTollBooth2.Multiply(DarkerMultipler);

            Color32 colorSpaceElevator2 = new Color32(087, 254, 255, 255);         // color taken manually from Space Elevator rings
            Color32 colorSpaceElevator3 = colorSpaceElevator2.Multiply(DarkerMultipler);

            #endregion


            // initialize categories and statistics
            #region Categories and Statistics
            _instance.Clear();
            Category category;

            _instance.Add(category = new Category(Category.CategoryType.Location, Translation.Key.Location));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LocationHome,
                Translation.Key.Home, Translation.Key.None,Translation.Key.Citizens,colorLocation1));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LocationWork,
                Translation.Key.Work, Translation.Key.None,Translation.Key.Citizens, colorLocation2));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LocationVisit,
                Translation.Key.Visit, Translation.Key.None,Translation.Key.Citizens, colorLocation3));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LocationMoving, 
                Translation.Key.Moving, Translation.Key.None, Translation.Key.Citizens, colorLocation4));

            //_instance.Add(category = new Category(Category.CategoryType.Water, Translation.Key.Water));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterConsumptionPercent,                    Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.PctOfPumpingCapacity,       colorWater1                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterConsumption,                           Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.CubicMetersPerWeek,         colorWater1                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterPumpingCapacity,                       Translation.Key.PumpingCapacity,        Translation.Key.None,           Translation.Key.CubicMetersPerWeek,         colorWater2                 ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.WaterTank, Translation.Key.WaterTank));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterTankReservedPercent,                   Translation.Key.Reserved,               Translation.Key.None,           Translation.Key.PctOfStorageCapacity,       colorWaterTank1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterTankReserved,                          Translation.Key.Reserved,               Translation.Key.None,           Translation.Key.CubicMeters,                colorWaterTank1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.WaterTankStorageCapacity,                   Translation.Key.StorageCapacity,        Translation.Key.None,           Translation.Key.CubicMeters,                colorWaterTank2             ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Sewage, Translation.Key.Sewage));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.SewageProductionPercent,                    Translation.Key.Production,             Translation.Key.None,           Translation.Key.PctOfDrainingCapacity,      colorSewage1                ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.SewageProduction,                           Translation.Key.Production,             Translation.Key.None,           Translation.Key.CubicMetersPerWeek,         colorSewage1                ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.SewageDrainingCapacity,                     Translation.Key.DrainingCapacity,       Translation.Key.None,           Translation.Key.CubicMetersPerWeek,         colorSewage2                ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Landfill, Translation.Key.Landfill));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LandfillStoragePercent,                     Translation.Key.Storage,                Translation.Key.None,           Translation.Key.PctOfCapacity,              colorLandfill1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LandfillStorage,                            Translation.Key.Storage,                Translation.Key.None,           Translation.Key.Units,                      colorLandfill1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LandfillCapacity,                           Translation.Key.Capacity,               Translation.Key.None,           Translation.Key.Units,                      colorLandfill2              ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Garbage, Translation.Key.Garbage));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GarbageProductionPercent,                   Translation.Key.Production,             Translation.Key.None,           Translation.Key.PctOfProcessingCapacity,    colorGarbage1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GarbageProduction,                          Translation.Key.Production,             Translation.Key.None,           Translation.Key.UnitsPerWeek,               colorGarbage1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GarbageProcessingCapacity,                  Translation.Key.ProcessingCapacity,     Translation.Key.None,           Translation.Key.UnitsPerWeek,               colorGarbage2               ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Education, Translation.Key.Education));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationElementaryEligiblePercent,         Translation.Key.Elementary,             Translation.Key.Eligible,       Translation.Key.PctOfCapacity,              colorUneducated1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationElementaryEligible,                Translation.Key.Elementary,             Translation.Key.Eligible,       Translation.Key.Students,                   colorUneducated1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationElementaryCapacity,                Translation.Key.Elementary,             Translation.Key.Capacity,       Translation.Key.Students,                   colorUneducated2            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationHighSchoolEligiblePercent,         Translation.Key.HighSchool,             Translation.Key.Eligible,       Translation.Key.PctOfCapacity,              colorEducated1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationHighSchoolEligible,                Translation.Key.HighSchool,             Translation.Key.Eligible,       Translation.Key.Students,                   colorEducated1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationHighSchoolCapacity,                Translation.Key.HighSchool,             Translation.Key.Capacity,       Translation.Key.Students,                   colorEducated2              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationUniversityEligiblePercent,         Translation.Key.University,             Translation.Key.Eligible,       Translation.Key.PctOfCapacity,              colorWellEducated1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationUniversityEligible,                Translation.Key.University,             Translation.Key.Eligible,       Translation.Key.Students,                   colorWellEducated1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationUniversityCapacity,                Translation.Key.University,             Translation.Key.Capacity,       Translation.Key.Students,                   colorWellEducated2          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLibraryUsersPercent,               Translation.Key.PublicLibrary,          Translation.Key.Users,          Translation.Key.PctOfCapacity,              colorHighlyEducated1        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLibraryUsers,                      Translation.Key.PublicLibrary,          Translation.Key.Users,          Translation.Key.Visitors,                   colorHighlyEducated1        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLibraryCapacity,                   Translation.Key.PublicLibrary,          Translation.Key.Capacity,       Translation.Key.Visitors,                   colorHighlyEducated2        ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.EducationLevel, Translation.Key.EducationLevel));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelUneducatedPercent,            Translation.Key.Uneducated,             Translation.Key.None,           Translation.Key.PctOfPopulation,            colorUneducated1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelEducatedPercent,              Translation.Key.Educated,               Translation.Key.None,           Translation.Key.PctOfPopulation,            colorEducated1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelWellEducatedPercent,          Translation.Key.WellEducated,           Translation.Key.None,           Translation.Key.PctOfPopulation,            colorWellEducated1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelHighlyEducatedPercent,        Translation.Key.HighlyEducated,         Translation.Key.None,           Translation.Key.PctOfPopulation,            colorHighlyEducated1        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelUneducated,                   Translation.Key.Uneducated,             Translation.Key.None,           Translation.Key.Citizens,                   colorUneducated1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelEducated,                     Translation.Key.Educated,               Translation.Key.None,           Translation.Key.Citizens,                   colorEducated1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelWellEducated,                 Translation.Key.WellEducated,           Translation.Key.None,           Translation.Key.Citizens,                   colorWellEducated1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EducationLevelHighlyEducated,               Translation.Key.HighlyEducated,         Translation.Key.None,           Translation.Key.Citizens,                   colorHighlyEducated1        ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Happiness, Translation.Key.Happiness));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HappinessGlobal,                            Translation.Key.Global,                 Translation.Key.None,           Translation.Key.Percent,                    colorStatisticsHappiness    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HappinessResidential,                       Translation.Key.Residential,            Translation.Key.None,           Translation.Key.Percent,                    colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HappinessCommercial,                        Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.Percent,                    colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HappinessIndustrial,                        Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.Percent,                    colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HappinessOffice,                            Translation.Key.Office,                 Translation.Key.None,           Translation.Key.Percent,                    colorZoneOffice             ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Healthcare, Translation.Key.Healthcare));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HealthcareAverageHealth,                    Translation.Key.AverageHealth,          Translation.Key.None,           Translation.Key.Percent,                    colorHealthcareAverage      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HealthcareSickPercent,                      Translation.Key.Sick,                   Translation.Key.None,           Translation.Key.PctOfHealCapacity,          colorHealthcare1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HealthcareSick,                             Translation.Key.Sick,                   Translation.Key.None,           Translation.Key.Citizens,                   colorHealthcare1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HealthcareHealCapacity,                     Translation.Key.HealCapacity,           Translation.Key.None,           Translation.Key.Citizens,                   colorHealthcare2            ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Deathcare, Translation.Key.Deathcare));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCemeteryBuriedPercent,             Translation.Key.Cemetery,               Translation.Key.Buried,         Translation.Key.PctOfCapacity,              colorDeathcare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCemeteryBuried,                    Translation.Key.Cemetery,               Translation.Key.Buried,         Translation.Key.Citizens,                   colorDeathcare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCemeteryCapacity,                  Translation.Key.Cemetery,               Translation.Key.Capacity,       Translation.Key.Citizens,                   colorDeathcare2             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCrematoriumDeceasedPercent,        Translation.Key.Crematorium,            Translation.Key.Deceased,       Translation.Key.PctOfCapacity,              colorDeathcare3             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCrematoriumDeceased,               Translation.Key.Crematorium,            Translation.Key.Deceased,       Translation.Key.Citizens,                   colorDeathcare3             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareCrematoriumCapacity,               Translation.Key.Crematorium,            Translation.Key.Capacity,       Translation.Key.Citizens,                   colorDeathcare4             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.DeathcareDeathRate,                         Translation.Key.DeathRate,              Translation.Key.None,           Translation.Key.CitizensPerWeek,            colorStatisticsDeathRate    ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Childcare, Translation.Key.Childcare));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ChildcareAverageHealth,                     Translation.Key.AverageHealth,          Translation.Key.None,           Translation.Key.Percent,                    colorHealthcareAverage      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ChildcareSickPercent,                       Translation.Key.SickChildrenTeens,      Translation.Key.None,           Translation.Key.PctOfChildrenTeens,         colorChildcare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ChildcareSick,                              Translation.Key.SickChildrenTeens,      Translation.Key.None,           Translation.Key.Citizens,                   colorChildcare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ChildcarePopulation,                        Translation.Key.ChildrenTeens,          Translation.Key.None,           Translation.Key.Citizens,                   colorChildcare2             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ChildcareBirthRate,                         Translation.Key.BirthRate,              Translation.Key.None,           Translation.Key.CitizensPerWeek,            colorStatisticsBirthRate    ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Eldercare, Translation.Key.Eldercare));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EldercareAverageHealth,                     Translation.Key.AverageHealth,          Translation.Key.None,           Translation.Key.Percent,                    colorHealthcareAverage      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EldercareSickPercent,                       Translation.Key.SickSeniors,            Translation.Key.None,           Translation.Key.PctOfSeniors,               colorEldercare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EldercareSick,                              Translation.Key.SickSeniors,            Translation.Key.None,           Translation.Key.Citizens,                   colorEldercare1             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EldercarePopulation,                        Translation.Key.Seniors,                Translation.Key.None,           Translation.Key.Citizens,                   colorEldercare2             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EldercareAverageLifeSpan,                   Translation.Key.AverageLifeSpan,        Translation.Key.None,           Translation.Key.Years,                      colorStatisticsDeathRate    ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Zoning, Translation.Key.Zoning));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningResidentialPercent,                   Translation.Key.Residential,            Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningCommercialPercent,                    Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningIndustrialPercent,                    Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningOfficePercent,                        Translation.Key.Office,                 Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningUnzonedPercent,                       Translation.Key.Unzoned,                Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneUnzoned            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningTotal,                                Translation.Key.Total,                  Translation.Key.None,           Translation.Key.Squares,                    colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningResidential,                          Translation.Key.Residential,            Translation.Key.None,           Translation.Key.Squares,                    colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningCommercial,                           Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.Squares,                    colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningIndustrial,                           Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.Squares,                    colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningOffice,                               Translation.Key.Office,                 Translation.Key.None,           Translation.Key.Squares,                    colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoningUnzoned,                              Translation.Key.Unzoned,                Translation.Key.None,           Translation.Key.Squares,                    colorZoneUnzoned            ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.ZoneLevel, Translation.Key.ZoneLevel));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidentialAverage,                Translation.Key.Residential,            Translation.Key.Average,        Translation.Key.Level1To5,                  colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidential1,                      Translation.Key.Residential,            Translation.Key.Level1,         Translation.Key.PctOfResidential,           colorResidentialLevel1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidential2,                      Translation.Key.Residential,            Translation.Key.Level2,         Translation.Key.PctOfResidential,           colorResidentialLevel2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidential3,                      Translation.Key.Residential,            Translation.Key.Level3,         Translation.Key.PctOfResidential,           colorResidentialLevel3      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidential4,                      Translation.Key.Residential,            Translation.Key.Level4,         Translation.Key.PctOfResidential,           colorResidentialLevel4      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelResidential5,                      Translation.Key.Residential,            Translation.Key.Level5,         Translation.Key.PctOfResidential,           colorResidentialLevel5      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelCommercialAverage,                 Translation.Key.Commercial,             Translation.Key.Average,        Translation.Key.Level1To3,                  colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelCommercial1,                       Translation.Key.Commercial,             Translation.Key.Level1,         Translation.Key.PctOfCommercial,            colorCommercialLevel1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelCommercial2,                       Translation.Key.Commercial,             Translation.Key.Level2,         Translation.Key.PctOfCommercial,            colorCommercialLevel2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelCommercial3,                       Translation.Key.Commercial,             Translation.Key.Level3,         Translation.Key.PctOfCommercial,            colorCommercialLevel3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelIndustrialAverage,                 Translation.Key.Industrial,             Translation.Key.Average,        Translation.Key.Level1To3,                  colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelIndustrial1,                       Translation.Key.Industrial,             Translation.Key.Level1,         Translation.Key.PctOfIndustrial,            colorIndustrialLevel1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelIndustrial2,                       Translation.Key.Industrial,             Translation.Key.Level2,         Translation.Key.PctOfIndustrial,            colorIndustrialLevel2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelIndustrial3,                       Translation.Key.Industrial,             Translation.Key.Level3,         Translation.Key.PctOfIndustrial,            colorIndustrialLevel3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelOfficeAverage,                     Translation.Key.Office,                 Translation.Key.Average,        Translation.Key.Level1To3,                  colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelOffice1,                           Translation.Key.Office,                 Translation.Key.Level1,         Translation.Key.PctOfOffice,                colorOfficeLevel1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelOffice2,                           Translation.Key.Office,                 Translation.Key.Level2,         Translation.Key.PctOfOffice,                colorOfficeLevel2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneLevelOffice3,                           Translation.Key.Office,                 Translation.Key.Level3,         Translation.Key.PctOfOffice,                colorOfficeLevel3           ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.ZoneBuildings, Translation.Key.ZoneBuildings));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsResidentialPercent,            Translation.Key.Residential,            Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsCommercialPercent,             Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsIndustrialPercent,             Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsOfficePercent,                 Translation.Key.Office,                 Translation.Key.None,           Translation.Key.PctOfTotal,                 colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsTotal,                         Translation.Key.Total,                  Translation.Key.None,           Translation.Key.HouseholdsPlusJobs,         colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsResidential,                   Translation.Key.Residential,            Translation.Key.None,           Translation.Key.Households,                 colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsCommercial,                    Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.Jobs,                       colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsIndustrial,                    Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.Jobs,                       colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneBuildingsOffice,                        Translation.Key.Office,                 Translation.Key.None,           Translation.Key.Jobs,                       colorZoneOffice             ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.ZoneDemand, Translation.Key.ZoneDemand));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneDemandResidential,                      Translation.Key.Residential,            Translation.Key.None,           Translation.Key.Percent,                    colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneDemandCommercial,                       Translation.Key.Commercial,             Translation.Key.None,           Translation.Key.Percent,                    colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ZoneDemandIndustrialOffice,                 Translation.Key.IndustrialOffice,       Translation.Key.None,           Translation.Key.Percent,                    colorZoneIndustrial         ));

            _instance.Add(category = new Category(Category.CategoryType.Traffic, Translation.Key.Traffic));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TrafficAverageFlow,                         Translation.Key.Average,                Translation.Key.Flow,           Translation.Key.Percent,                    colorInfoTrafficTarget      ));

            //_instance.Add(category = new Category(Category.CategoryType.Pollution, Translation.Key.Pollution));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PollutionAverageGround,                     Translation.Key.Average,                Translation.Key.Ground,         Translation.Key.Percent,                    colorInfoPollution          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PollutionAverageDrinkingWater,              Translation.Key.Average,                Translation.Key.DrinkingWater,  Translation.Key.Percent,                    colorInfoPollution          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PollutionAverageNoise,                      Translation.Key.Average,                Translation.Key.Noise,          Translation.Key.Percent,                    colorInfoNoisePollution     ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.FireSafety, Translation.Key.FireSafety));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.FireSafetyHazard,                           Translation.Key.Hazard,                 Translation.Key.None,           Translation.Key.Percent,                    colorFireSafety             ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Crime, Translation.Key.Crime));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CrimeRate,                                  Translation.Key.Rate,                   Translation.Key.None,           Translation.Key.Percent,                    colorCrimeRate              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CrimeDetainedCriminalsPercent,              Translation.Key.DetainedCriminals,      Translation.Key.None,           Translation.Key.PctOfJailsCapacity,         colorCrime1                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CrimeDetainedCriminals,                     Translation.Key.DetainedCriminals,      Translation.Key.None,           Translation.Key.Citizens,                   colorCrime1                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CrimeJailsCapacity,                         Translation.Key.JailsCapacity,          Translation.Key.None,           Translation.Key.Citizens,                   colorCrime2                 ));

            _instance.Add(category = new Category(Category.CategoryType.PublicTransportation, Translation.Key.PublicTransportation));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTotalTotal,             Translation.Key.Total,                  Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTotal1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTotalResidents,         Translation.Key.Total,                  Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTotal1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTotalTourists,          Translation.Key.Total,                  Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTotal2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationBusTotal,               Translation.Key.Bus,                    Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportBus1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationBusResidents,           Translation.Key.Bus,                    Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportBus1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationBusTourists,            Translation.Key.Bus,                    Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportBus2          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrolleybusTotal,        Translation.Key.Trolleybus,             Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTrolleybus1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrolleybusResidents,    Translation.Key.Trolleybus,             Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTrolleybus1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrolleybusTourists,     Translation.Key.Trolleybus,             Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTrolleybus2   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTramTotal,              Translation.Key.Tram,                   Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTram1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTramResidents,          Translation.Key.Tram,                   Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTram1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTramTourists,           Translation.Key.Tram,                   Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTram2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMetroTotal,             Translation.Key.Metro,                  Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportMetro1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMetroResidents,         Translation.Key.Metro,                  Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportMetro1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMetroTourists,          Translation.Key.Metro,                  Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportMetro2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrainTotal,             Translation.Key.Train,                  Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTrain1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrainResidents,         Translation.Key.Train,                  Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTrain1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTrainTourists,          Translation.Key.Train,                  Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTrain2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationShipTotal,              Translation.Key.Ship,                   Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportShip1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationShipResidents,          Translation.Key.Ship,                   Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportShip1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationShipTourists,           Translation.Key.Ship,                   Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportShip2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationAirTotal,               Translation.Key.Air,                    Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportAir1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationAirResidents,           Translation.Key.Air,                    Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportAir1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationAirTourists,            Translation.Key.Air,                    Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportAir2          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMonorailTotal,          Translation.Key.Monorail,               Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportMonorail1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMonorailResidents,      Translation.Key.Monorail,               Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportMonorail1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationMonorailTourists,       Translation.Key.Monorail,               Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportMonorail2     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationCableCarTotal,          Translation.Key.CableCar,               Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportCableCar1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationCableCarResidents,      Translation.Key.CableCar,               Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportCableCar1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationCableCarTourists,       Translation.Key.CableCar,               Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportCableCar2     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTaxiTotal,              Translation.Key.Taxi,                   Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTaxi1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTaxiResidents,          Translation.Key.Taxi,                   Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTaxi1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PublicTransportationTaxiTourists,           Translation.Key.Taxi,                   Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTaxi2         ));

            //_instance.Add(category = new Category(Category.CategoryType.Citizen, Translation.Key.Citizens));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CitizenOnBike,
            //    Translation.Key.CitizenOnBike, Translation.Key.None, Translation.Key.Citizens, colorCitizen1));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CitizenWaiting,
            //    Translation.Key.CitizenWaiting, Translation.Key.None, Translation.Key.Citizens, colorCitizen2));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CitizenBored,
            //    Translation.Key.CitizenBored, Translation.Key.None, Translation.Key.Citizens, colorCitizen3));

            //_instance.Add(category = new Category(Category.CategoryType.Population, Translation.Key.Population));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationTotal,                            Translation.Key.Total,                  Translation.Key.None,           Translation.Key.Citizens,                   colorStatisticsPopulation   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationChildrenPercent,                  Translation.Key.Children,               Translation.Key.None,           Translation.Key.PctOfPopulation,            colorChild                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationTeensPercent,                     Translation.Key.Teens,                  Translation.Key.None,           Translation.Key.PctOfPopulation,            colorTeen                   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationYoungAdultsPercent,               Translation.Key.YoungAdults,            Translation.Key.None,           Translation.Key.PctOfPopulation,            colorYoung                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationAdultsPercent,                    Translation.Key.Adults,                 Translation.Key.None,           Translation.Key.PctOfPopulation,            colorAdult                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationSeniorsPercent,                   Translation.Key.Seniors,                Translation.Key.None,           Translation.Key.PctOfPopulation,            colorSenior                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationChildren,                         Translation.Key.Children,               Translation.Key.None,           Translation.Key.Citizens,                   colorChild                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationTeens,                            Translation.Key.Teens,                  Translation.Key.None,           Translation.Key.Citizens,                   colorTeen                   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationYoungAdults,                      Translation.Key.YoungAdults,            Translation.Key.None,           Translation.Key.Citizens,                   colorYoung                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationAdults,                           Translation.Key.Adults,                 Translation.Key.None,           Translation.Key.Citizens,                   colorAdult                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.PopulationSeniors,                          Translation.Key.Seniors,                Translation.Key.None,           Translation.Key.Citizens,                   colorSenior                 ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Households, Translation.Key.Households));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HouseholdsOccupiedPercent,                  Translation.Key.Occupied,               Translation.Key.None,           Translation.Key.PctOfAvailable,             colorHouseholds1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HouseholdsOccupied,                         Translation.Key.Occupied,               Translation.Key.None,           Translation.Key.Households,                 colorHouseholds1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HouseholdsAvailable,                        Translation.Key.Available,              Translation.Key.None,           Translation.Key.Households,                 colorHouseholds2            ));

            //_instance.Add(category = new Category(Category.CategoryType.Employment, Translation.Key.Employment));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentPeopleEmployed,                   Translation.Key.PeopleEmployed,         Translation.Key.None,           Translation.Key.Citizens,                   colorEmployment1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentJobsAvailable,                    Translation.Key.JobsAvailable,          Translation.Key.None,           Translation.Key.Jobs,                       colorEmployment2            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentUnfilledJobs,                     Translation.Key.UnfilledJobs,           Translation.Key.None,           Translation.Key.Jobs,                       colorEmployment3            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentUnemploymentPercent,              Translation.Key.Unemployment,           Translation.Key.None,           Translation.Key.PctOfEligible,              colorUnemployment1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentUnemployed,                       Translation.Key.Unemployed,             Translation.Key.None,           Translation.Key.Citizens,                   colorUnemployment1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.EmploymentEligibleWorkers,                  Translation.Key.EligibleWorkers,        Translation.Key.None,           Translation.Key.Citizens,                   colorUnemployment2          ));

            //_instance.Add(category = new Category(Category.CategoryType.OutsideConnections, Translation.Key.OutsideConnections));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportTotal,              Translation.Key.Import,                 Translation.Key.Total,          Translation.Key.UnitsPerWeek,               colorTransferTotal1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportGoods,              Translation.Key.Import,                 Translation.Key.Goods,          Translation.Key.UnitsPerWeek,               colorTransferGoods1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportForestry,           Translation.Key.Import,                 Translation.Key.Forestry,       Translation.Key.UnitsPerWeek,               colorTransferForestry1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportFarming,            Translation.Key.Import,                 Translation.Key.Farming,        Translation.Key.UnitsPerWeek,               colorTransferFarming1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportOre,                Translation.Key.Import,                 Translation.Key.Ore,            Translation.Key.UnitsPerWeek,               colorTransferOre1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportOil,                Translation.Key.Import,                 Translation.Key.Oil,            Translation.Key.UnitsPerWeek,               colorTransferOil1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsImportMail,               Translation.Key.Import,                 Translation.Key.Mail,           Translation.Key.UnitsPerWeek,               colorTransferMail1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportTotal,              Translation.Key.Export,                 Translation.Key.Total,          Translation.Key.UnitsPerWeek,               colorTransferTotal2         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportGoods,              Translation.Key.Export,                 Translation.Key.Goods,          Translation.Key.UnitsPerWeek,               colorTransferGoods2         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportForestry,           Translation.Key.Export,                 Translation.Key.Forestry,       Translation.Key.UnitsPerWeek,               colorTransferForestry2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportFarming,            Translation.Key.Export,                 Translation.Key.Farming,        Translation.Key.UnitsPerWeek,               colorTransferFarming2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportOre,                Translation.Key.Export,                 Translation.Key.Ore,            Translation.Key.UnitsPerWeek,               colorTransferOre2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportOil,                Translation.Key.Export,                 Translation.Key.Oil,            Translation.Key.UnitsPerWeek,               colorTransferOil2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportMail,               Translation.Key.Export,                 Translation.Key.Mail,           Translation.Key.UnitsPerWeek,               colorTransferMail2          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OutsideConnectionsExportFish,               Translation.Key.Export,                 Translation.Key.Fish,           Translation.Key.UnitsPerWeek,               colorTransferFish2          ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.LandValue, Translation.Key.LandValue));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.LandValueAverage,                           Translation.Key.Average,                Translation.Key.None,           Translation.Key.MoneyPerSquareMeter,        colorInfoLandValue          ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.NaturalResources, Translation.Key.NaturalResources));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesForestUsedPercent,          Translation.Key.Forest,                 Translation.Key.Used,           Translation.Key.PctOfForestAvailable,       colorResourceForestry1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesForestUsed,                 Translation.Key.Forest,                 Translation.Key.Used,           Translation.Key.UnitsPerWeek,               colorResourceForestry1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesForestAvailable,            Translation.Key.Forest,                 Translation.Key.Available,      Translation.Key.UnitsPerWeek,               colorResourceForestry2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesFertileLandUsedPercent,     Translation.Key.FertileLand,            Translation.Key.Used,           Translation.Key.PctOfFertileLandAvailable,  colorResourceFertility1     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesFertileLandUsed,            Translation.Key.FertileLand,            Translation.Key.Used,           Translation.Key.Hectare,                    colorResourceFertility1     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesFertileLandAvailable,       Translation.Key.FertileLand,            Translation.Key.Available,      Translation.Key.Hectare,                    colorResourceFertility2     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOreUsedPercent,             Translation.Key.Ore,                    Translation.Key.Used,           Translation.Key.PctPerWeekOfOreAvailable,   colorResourceOre1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOreUsed,                    Translation.Key.Ore,                    Translation.Key.Used,           Translation.Key.UnitsPerWeek,               colorResourceOre1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOreAvailable,               Translation.Key.Ore,                    Translation.Key.Available,      Translation.Key.Units,                      colorResourceOre2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOilUsedPercent,             Translation.Key.Oil,                    Translation.Key.Used,           Translation.Key.PctPerWeekOfOilAvailable,   colorResourceOil1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOilUsed,                    Translation.Key.Oil,                    Translation.Key.Used,           Translation.Key.UnitsPerWeek,               colorResourceOil1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.NaturalResourcesOilAvailable,               Translation.Key.Oil,                    Translation.Key.Available,      Translation.Key.Units,                      colorResourceOil2           ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.Heating, Translation.Key.Heating));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HeatingConsumptionPercent,                  Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.PctOfProduction,            colorInfoHeating1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HeatingConsumption,                         Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.MegaWatts,                  colorInfoHeating1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.HeatingProduction,                          Translation.Key.Production,             Translation.Key.None,           Translation.Key.MegaWatts,                  colorInfoHeating2           ));

            _instance.Add(category = new Category(Category.CategoryType.Tourism, Translation.Key.Tourism));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismCityAttractiveness,                  Translation.Key.CityAttractiveness,     Translation.Key.None,           Translation.Key.Percent,                    colorInfoTourism            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismLowWealthPercent,                    Translation.Key.LowWealth,              Translation.Key.None,           Translation.Key.PctOfTotal,                 colorTouristsLowWealth1     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismMediumWealthPercent,                 Translation.Key.MediumWealth,           Translation.Key.None,           Translation.Key.PctOfTotal,                 colorTouristsMediumWealth1  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismHighWealthPercent,                   Translation.Key.HighWealth,             Translation.Key.None,           Translation.Key.PctOfTotal,                 colorTouristsHighWealth1    ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismTotal,                               Translation.Key.Total,                  Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTouristsTotal          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismLowWealth,                           Translation.Key.LowWealth,              Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTouristsLowWealth2     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismMediumWealth,                        Translation.Key.MediumWealth,           Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTouristsMediumWealth2  ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismHighWealth,                          Translation.Key.HighWealth,             Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTouristsHighWealth2    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismExchangeStudentBonus,                Translation.Key.ExchangeStudentBonus,   Translation.Key.None,           Translation.Key.Percent,                    colorExchangeStudent        ));

            _instance.Add(category = new Category(Category.CategoryType.Tours, Translation.Key.Tours));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursTotalTotal,                            Translation.Key.Total,                  Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTotal1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursTotalResidents,                        Translation.Key.Total,                  Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTotal1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursTotalTourists,                         Translation.Key.Total,                  Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTotal2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursWalkingTourTotal,                      Translation.Key.WalkingTour,            Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportPedestrian1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursWalkingTourResidents,                  Translation.Key.WalkingTour,            Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportPedestrian1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursWalkingTourTourists,                   Translation.Key.WalkingTour,            Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportPedestrian2   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursSightseeingTotal,                      Translation.Key.SightseeingBus,         Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportTouristBus1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursSightseeingResidents,                  Translation.Key.SightseeingBus,         Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportTouristBus1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursSightseeingTourists,                   Translation.Key.SightseeingBus,         Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportTouristBus2   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursBalloonTotal,                          Translation.Key.Balloon,                Translation.Key.None,           Translation.Key.TotalPerWeek,               colorTransportHotAirBalloon1));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursBalloonResidents,                      Translation.Key.Balloon,                Translation.Key.None,           Translation.Key.ResidentsPerWeek,           colorTransportHotAirBalloon1));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ToursBalloonToursits,                       Translation.Key.Balloon,                Translation.Key.None,           Translation.Key.TouristsPerWeek,            colorTransportHotAirBalloon2));

            //_instance.Add(category = new Category(Category.CategoryType.TaxRate, Translation.Key.TaxRate));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateResidentialLow,                      Translation.Key.Residential,            Translation.Key.LowDensity,     Translation.Key.Percent,                    colorZoneResidentialLow     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateResidentialHigh,                     Translation.Key.Residential,            Translation.Key.HighDensity,    Translation.Key.Percent,                    colorZoneResidentialHigh    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateCommercialLow,                       Translation.Key.Commercial,             Translation.Key.LowDensity,     Translation.Key.Percent,                    colorZoneCommercialLow      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateCommercialHigh,                      Translation.Key.Commercial,             Translation.Key.HighDensity,    Translation.Key.Percent,                    colorZoneCommercialHigh     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateIndustrial,                          Translation.Key.Industrial,             Translation.Key.None,           Translation.Key.Percent,                    colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TaxRateOffice,                              Translation.Key.Office,                 Translation.Key.None,           Translation.Key.Percent,                    colorZoneOffice             ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.CityEconomy, Translation.Key.CityEconomy));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyTotalIncome,                     Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyTotalExpenses,                   Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyTotalProfit,                     Translation.Key.Total,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityTotalProfit        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyBankBalance,                     Translation.Key.BankBalance,            Translation.Key.None,           Translation.Key.Money,                      colorBankBalance            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyLoanBalance,                     Translation.Key.LoanBalance,            Translation.Key.None,           Translation.Key.Money,                      colorEconomy                ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyCityValue,                       Translation.Key.CityValue,              Translation.Key.None,           Translation.Key.Money,                      colorStatisticsCityValue    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyCityValuePerCapita,              Translation.Key.CityValue,              Translation.Key.None,           Translation.Key.MoneyPerCapita,             colorStatisticsCityValue    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyGrossDomesticProduct,            Translation.Key.GrossDomesticProduct,   Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorStatisticsCityBudget   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyGrossDomesticProductPerCapita,   Translation.Key.GrossDomesticProduct,   Translation.Key.None,           Translation.Key.MoneyPerWeekPerCapita,      colorStatisticsCityBudget   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyConsumption,                     Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyConsumptionPercent,              Translation.Key.Consumption,            Translation.Key.None,           Translation.Key.PctOfGrossDomesticProduct,  colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyGovernmentSpending,              Translation.Key.GovernmentSpending,     Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorStatisticsDeathRate    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyGovernmentSpendingPercent,       Translation.Key.GovernmentSpending,     Translation.Key.None,           Translation.Key.PctOfGrossDomesticProduct,  colorStatisticsDeathRate    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyExports,                         Translation.Key.Exports,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferTotal2         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyImports,                         Translation.Key.Imports,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferTotal1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyNetExports,                      Translation.Key.NetExports,             Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferTotal3         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CityEconomyNetExportsPercent,               Translation.Key.NetExports,             Translation.Key.None,           Translation.Key.PctOfGrossDomesticProduct,  colorTransferTotal3         ));

            //_instance.Add(category = new Category(Category.CategoryType.ResidentialIncome, Translation.Key.ResidentialIncome));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeTotalPercent,              Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityIncome,            colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeTotal,                     Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneResidentialMid     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensityTotal,           Translation.Key.LowDensity,             Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorZoneResidentialLow     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensity1,               Translation.Key.LowDensity,             Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorResidentialLevel1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensity2,               Translation.Key.LowDensity,             Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorResidentialLevel2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensity3,               Translation.Key.LowDensity,             Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorResidentialLevel3      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensity4,               Translation.Key.LowDensity,             Translation.Key.Level4,         Translation.Key.MoneyPerWeek,               colorResidentialLevel4      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensity5,               Translation.Key.LowDensity,             Translation.Key.Level5,         Translation.Key.MoneyPerWeek,               colorResidentialLevel5      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeLowDensitySelfSufficient,  Translation.Key.LowDensity,             Translation.Key.SelfSufficient, Translation.Key.MoneyPerWeek,               colorIncomeSelfSufficient   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensityTotal,          Translation.Key.HighDensity,            Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorZoneResidentialHigh    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensity1,              Translation.Key.HighDensity,            Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorResidentialLevel1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensity2,              Translation.Key.HighDensity,            Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorResidentialLevel2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensity3,              Translation.Key.HighDensity,            Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorResidentialLevel3      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensity4,              Translation.Key.HighDensity,            Translation.Key.Level4,         Translation.Key.MoneyPerWeek,               colorResidentialLevel4      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensity5,              Translation.Key.HighDensity,            Translation.Key.Level5,         Translation.Key.MoneyPerWeek,               colorResidentialLevel5      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ResidentialIncomeHighDensitySelfSufficient, Translation.Key.HighDensity,            Translation.Key.SelfSufficient, Translation.Key.MoneyPerWeek,               colorIncomeSelfSufficient   ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.CommercialIncome, Translation.Key.CommercialIncome));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeTotalPercent,               Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityIncome,            colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeTotal,                      Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeLowDensityTotal,            Translation.Key.LowDensity,             Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorZoneCommercialLow      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeLowDensity1,                Translation.Key.LowDensity,             Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorCommercialLevel1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeLowDensity2,                Translation.Key.LowDensity,             Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorCommercialLevel2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeLowDensity3,                Translation.Key.LowDensity,             Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorCommercialLevel3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeHighDensityTotal,           Translation.Key.HighDensity,            Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorZoneCommercialHigh     ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeHighDensity1,               Translation.Key.HighDensity,            Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorCommercialLevel1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeHighDensity2,               Translation.Key.HighDensity,            Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorCommercialLevel2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeHighDensity3,               Translation.Key.HighDensity,            Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorCommercialLevel3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeSpecializedTotal,           Translation.Key.SpecializedTotal,       Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorLevelCommercial        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeLeisure,                    Translation.Key.Leisure,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorIncomeLeisure          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeTourism,                    Translation.Key.Tourism,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTourismIncome          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CommercialIncomeOrganic,                    Translation.Key.OrganicAndLocalProduce, Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorIncomeOrganic          ));

            //_instance.Add(category = new Category(Category.CategoryType.IndustrialIncome, Translation.Key.IndustrialIncome));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeTotalPercent,               Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityIncome,            colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeTotal,                      Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneIndustrial         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeGenericTotal,               Translation.Key.Generic,                Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorLevelIndustrial        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeGeneric1,                   Translation.Key.Generic,                Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorIndustrialLevel1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeGeneric2,                   Translation.Key.Generic,                Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorIndustrialLevel2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeGeneric3,                   Translation.Key.Generic,                Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorIndustrialLevel3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeSpecializedTotal,           Translation.Key.SpecializedTotal,       Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferTotal1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeForestry,                   Translation.Key.Forestry,               Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferForestry1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeFarming,                    Translation.Key.Farming,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferFarming1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeOre,                        Translation.Key.Ore,                    Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferOre1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustrialIncomeOil,                        Translation.Key.Oil,                    Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransferOil1           ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.OfficeIncome, Translation.Key.OfficeIncome));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeTotalPercent,                   Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityIncome,            colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeTotal,                          Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeGenericTotal,                   Translation.Key.Generic,                Translation.Key.Total,          Translation.Key.MoneyPerWeek,               colorLevelOffice            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeGeneric1,                       Translation.Key.Generic,                Translation.Key.Level1,         Translation.Key.MoneyPerWeek,               colorOfficeLevel1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeGeneric2,                       Translation.Key.Generic,                Translation.Key.Level2,         Translation.Key.MoneyPerWeek,               colorOfficeLevel2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeGeneric3,                       Translation.Key.Generic,                Translation.Key.Level3,         Translation.Key.MoneyPerWeek,               colorOfficeLevel3           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.OfficeIncomeITCluster,                      Translation.Key.ITCluster,              Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorIncomeITCluster        ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.TourismIncome, Translation.Key.TourismIncome));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismIncomeTotalPercent,                  Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityIncome,            colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismIncomeTotal,                         Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneCommercialMid      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismIncomeCommercialZones,               Translation.Key.CommercialZones,        Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTourismIncome          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismIncomeTransportation,                Translation.Key.PublicTransportation,   Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransportTotal1        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TourismIncomeParkAreas,                     Translation.Key.ParkAreas,              Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorParks                  ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.ServiceExpenses, Translation.Key.ServiceExpenses));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesTotalPercent,                Translation.Key.Total,                  Translation.Key.None,           Translation.Key.PctOfCityExpenses,          colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesTotal,                       Translation.Key.Total,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorZoneOffice             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesRoads,                       Translation.Key.Roads,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorTransportTotal1        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesElectricity,                 Translation.Key.Electricity,            Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorElectricity1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesWaterSewageHeating,          Translation.Key.WaterSewage,            Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorWater1                 ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesGarbage,                     Translation.Key.Garbage,                Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorGarbage1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesHealthcare,                  Translation.Key.Healthcare,             Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorHealthcare1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesFire,                        Translation.Key.Fire,                   Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorFireSafety             ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesEmergency,                   Translation.Key.Emergency,              Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorEmergency              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesPolice,                      Translation.Key.Police,                 Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorCrimeRate              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesEducation,                   Translation.Key.Education,              Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorEducated1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesParksPlazas,                 Translation.Key.ParksPlazasLandscaping, Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorParks                  ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesUniqueBuildings,             Translation.Key.UniqueBuildings,        Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorUniqueBuildings        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesGenericSportsArenas,         Translation.Key.GenericSportsArenas,    Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorGenericSportsArenas    ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesLoans,                       Translation.Key.Loans,                  Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorEconomy                ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ServiceExpensesPolicies,                    Translation.Key.Policies,               Translation.Key.None,           Translation.Key.MoneyPerWeek,               colorPolicies               ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.ParkAreas, Translation.Key.ParkAreas));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasTotalIncomePercent,                Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.PctOfCityIncome,            colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasTotalIncome,                       Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasTotalExpensesPercent,              Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.PctOfCityExpenses,          colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasTotalExpenses,                     Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasTotalProfit,                       Translation.Key.Total,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityTotalProfit        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasCityParkIncome,                    Translation.Key.CityPark,               Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityPark1              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasCityParkExpenses,                  Translation.Key.CityPark,               Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityPark2              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasCityParkProfit,                    Translation.Key.CityPark,               Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityPark3              ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasAmusementParkIncome,               Translation.Key.AmusementPark,          Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorAmusementPark1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasAmusementParkExpenses,             Translation.Key.AmusementPark,          Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorAmusementPark2         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasAmusementParkProfit,               Translation.Key.AmusementPark,          Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorAmusementPark3         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasZooIncome,                         Translation.Key.Zoo,                    Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorZoo1                   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasZooExpenses,                       Translation.Key.Zoo,                    Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorZoo2                   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasZooProfit,                         Translation.Key.Zoo,                    Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorZoo3                   ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasNatureReserveIncome,               Translation.Key.NatureReserve,          Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorNatureReserve1         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasNatureReserveExpenses,             Translation.Key.NatureReserve,          Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorNatureReserve2         ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.ParkAreasNatureReserveProfit,               Translation.Key.NatureReserve,          Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorNatureReserve3         ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.IndustryAreas, Translation.Key.IndustryAreas));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasTotalIncomePercent,            Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.PctOfCityIncome,            colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasTotalIncome,                   Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasTotalExpensesPercent,          Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.PctOfCityExpenses,          colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasTotalExpenses,                 Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasTotalProfit,                   Translation.Key.Total,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityTotalProfit        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasForestryIncome,                Translation.Key.Forestry,               Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferForestry1      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasForestryExpenses,              Translation.Key.Forestry,               Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferForestry2      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasForestryProfit,                Translation.Key.Forestry,               Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferForestry3      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasFarmingIncome,                 Translation.Key.Farming,                Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferFarming1       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasFarmingExpenses,               Translation.Key.Farming,                Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferFarming2       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasFarmingProfit,                 Translation.Key.Farming,                Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferFarming3       ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOreIncome,                     Translation.Key.Ore,                    Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferOre1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOreExpenses,                   Translation.Key.Ore,                    Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferOre2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOreProfit,                     Translation.Key.Ore,                    Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferOre3           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOilIncome,                     Translation.Key.Oil,                    Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferOil1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOilExpenses,                   Translation.Key.Oil,                    Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferOil2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasOilProfit,                     Translation.Key.Oil,                    Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferOil3           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasWarehousesFactoriesIncome,     Translation.Key.WarehousesAndFactories, Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferMail1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasWarehousesFactoriesExpenses,   Translation.Key.WarehousesAndFactories, Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferMail2          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.IndustryAreasWarehousesFactoriesProfit,     Translation.Key.WarehousesAndFactories, Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferMail3          ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.FishingIndustry, Translation.Key.FishingIndustry));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.FishingIndustryFishingIncome,               Translation.Key.Fishing,                Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransferFish1          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.FishingIndustryFishingExpenses,             Translation.Key.Fishing,                Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferFish2          ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.FishingIndustryFishingProfit,               Translation.Key.Fishing,                Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferFish3          ));
            //
            //_instance.Add(category = new Category(Category.CategoryType.CampusAreas, Translation.Key.CampusAreas));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTotalIncomePercent,              Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.PctOfCityIncome,            colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTotalIncome,                     Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityTotalIncome        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTotalExpensesPercent,            Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.PctOfCityExpenses,          colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTotalExpenses,                   Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityTotalExpenses      ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTotalProfit,                     Translation.Key.Total,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityTotalProfit        ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTradeSchoolIncome,               Translation.Key.TradeSchool,            Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTradeSchool1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTradeSchoolExpenses,             Translation.Key.TradeSchool,            Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTradeSchool2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasTradeSchoolProfit,               Translation.Key.TradeSchool,            Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTradeSchool3           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasLiberalArtsCollegeIncome,        Translation.Key.LiberalArtsCollege,     Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorLiberalArts1           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasLiberalArtsCollegeExpenses,      Translation.Key.LiberalArtsCollege,     Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorLiberalArts2           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasLiberalArtsCollegeProfit,        Translation.Key.LiberalArtsCollege,     Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorLiberalArts3           ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasUniversityIncome,                Translation.Key.University,             Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorUniversity1            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasUniversityExpenses,              Translation.Key.University,             Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorUniversity2            ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.CampusAreasUniversityProfit,                Translation.Key.University,             Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorUniversity3            ));

            _instance.Add(category = new Category(Category.CategoryType.TransportEconomy, Translation.Key.TransportEconomy));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTotalIncomePercent,         Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.PctOfCityIncome,            colorCityTotalIncome        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTotalIncome,                Translation.Key.Total,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorCityTotalIncome        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTotalExpensesPercent,       Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.PctOfCityExpenses,          colorCityTotalExpenses      ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTotalExpenses,              Translation.Key.Total,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorCityTotalExpenses      ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTotalProfit,                Translation.Key.Total,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorCityTotalProfit        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyBusIncome,                  Translation.Key.Bus,                    Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportBus1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyBusExpenses,                Translation.Key.Bus,                    Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportBus2          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyBusProfit,                  Translation.Key.Bus,                    Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportBus3          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrolleybusIncome,           Translation.Key.Trolleybus,             Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportTrolleybus1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrolleybusExpenses,         Translation.Key.Trolleybus,             Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportTrolleybus2   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrolleybusProfit,           Translation.Key.Trolleybus,             Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportTrolleybus3   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTramIncome,                 Translation.Key.Tram,                   Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportTram1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTramExpenses,               Translation.Key.Tram,                   Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportTram2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTramProfit,                 Translation.Key.Tram,                   Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportTram3         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMetroIncome,                Translation.Key.Metro,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportMetro1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMetroExpenses,              Translation.Key.Metro,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportMetro2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMetroProfit,                Translation.Key.Metro,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportMetro3        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrainIncome,                Translation.Key.Train,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportTrain1        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrainExpenses,              Translation.Key.Train,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportTrain2        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTrainProfit,                Translation.Key.Train,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportTrain3        ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyShipIncome,                 Translation.Key.Ship,                   Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportShip1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyShipExpenses,               Translation.Key.Ship,                   Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportShip2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyShipProfit,                 Translation.Key.Ship,                   Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportShip3         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyAirIncome,                  Translation.Key.Air,                    Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportAir1          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyAirExpenses,                Translation.Key.Air,                    Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportAir2          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyAirProfit,                  Translation.Key.Air,                    Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportAir3          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMonorailIncome,             Translation.Key.Monorail,               Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportMonorail1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMonorailExpenses,           Translation.Key.Monorail,               Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportMonorail2     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMonorailProfit,             Translation.Key.Monorail,               Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportMonorail3     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyCableCarIncome,             Translation.Key.CableCar,               Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportCableCar1     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyCableCarExpenses,           Translation.Key.CableCar,               Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportCableCar2     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyCableCarProfit,             Translation.Key.CableCar,               Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportCableCar3     ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTaxiIncome,                 Translation.Key.Taxi,                   Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportTaxi1         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTaxiExpenses,               Translation.Key.Taxi,                   Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportTaxi2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTaxiProfit,                 Translation.Key.Taxi,                   Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportTaxi3         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyToursIncome,                Translation.Key.Tours,                  Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTransportPedestrian1   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyToursExpenses,              Translation.Key.Tours,                  Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransportPedestrian2   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyToursProfit,                Translation.Key.Tours,                  Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransportPedestrian3   ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTollBoothIncome,            Translation.Key.TollBooth,              Translation.Key.Income,         Translation.Key.MoneyPerWeek,               colorTollBooth1             ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTollBoothExpenses,          Translation.Key.TollBooth,              Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTollBooth2             ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyTollBoothProfit,            Translation.Key.TollBooth,              Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTollBooth3             ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMailExpenses,               Translation.Key.Mail,                   Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorTransferMail2          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomyMailProfit,                 Translation.Key.Mail,                   Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorTransferMail3          ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomySpaceElevatorExpenses,      Translation.Key.SpaceElevator,          Translation.Key.Expenses,       Translation.Key.MoneyPerWeek,               colorSpaceElevator2         ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.TransportEconomySpaceElevatorProfit,        Translation.Key.SpaceElevator,          Translation.Key.Profit,         Translation.Key.MoneyPerWeek,               colorSpaceElevator3         ));

            _instance.Add(category = new Category(Category.CategoryType.GameLimits, Translation.Key.GameLimits));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsBuildingsUsedPercent,             Translation.Key.Buildings,              Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsBuildingsUsed,                    Translation.Key.Buildings,              Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsBuildingsCapacity,                Translation.Key.Buildings,              Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizensUsedPercent,              Translation.Key.Citizens,               Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizensUsed,                     Translation.Key.Citizens,               Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizensCapacity,                 Translation.Key.Citizens,               Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenUnitsUsedPercent,          Translation.Key.CitizenUnits,           Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenUnitsUsed,                 Translation.Key.CitizenUnits,           Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenUnitsCapacity,             Translation.Key.CitizenUnits,           Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenInstancesUsedPercent,      Translation.Key.CitizenInstances,       Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenInstancesUsed,             Translation.Key.CitizenInstances,       Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsCitizenInstancesCapacity,         Translation.Key.CitizenInstances,       Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDisastersUsedPercent,             Translation.Key.Disasters,              Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDisastersUsed,                    Translation.Key.Disasters,              Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDisastersCapacity,                Translation.Key.Disasters,              Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDistrictsUsedPercent,             Translation.Key.Districts,              Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDistrictsUsed,                    Translation.Key.Districts,              Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsDistrictsCapacity,                Translation.Key.Districts,              Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsEventsUsedPercent,                Translation.Key.Events,                 Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsEventsUsed,                       Translation.Key.Events,                 Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsEventsCapacity,                   Translation.Key.Events,                 Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsGameAreasUsedPercent,             Translation.Key.GameAreas,              Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsGameAreasUsed,                    Translation.Key.GameAreas,              Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsGameAreasCapacity,                Translation.Key.GameAreas,              Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkLanesUsedPercent,          Translation.Key.NetworkLanes,           Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkLanesUsed,                 Translation.Key.NetworkLanes,           Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkLanesCapacity,             Translation.Key.NetworkLanes,           Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkNodesUsedPercent,          Translation.Key.NetworkNodes,           Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkNodesUsed,                 Translation.Key.NetworkNodes,           Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkNodesCapacity,             Translation.Key.NetworkNodes,           Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkSegmentsUsedPercent,       Translation.Key.NetworkSegments,        Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkSegmentsUsed,              Translation.Key.NetworkSegments,        Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsNetworkSegmentsCapacity,          Translation.Key.NetworkSegments,        Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPaintedAreasUsedPercent,          Translation.Key.PaintedAreas,           Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPaintedAreasUsed,                 Translation.Key.PaintedAreas,           Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPaintedAreasCapacity,             Translation.Key.PaintedAreas,           Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPathUnitsUsedPercent,             Translation.Key.PathUnits,              Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPathUnitsUsed,                    Translation.Key.PathUnits,              Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPathUnitsCapacity,                Translation.Key.PathUnits,              Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPropsUsedPercent,                 Translation.Key.Props,                  Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPropsUsed,                        Translation.Key.Props,                  Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsPropsCapacity,                    Translation.Key.Props,                  Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioChannelsUsedPercent,         Translation.Key.RadioChannels,          Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioChannelsUsed,                Translation.Key.RadioChannels,          Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioChannelsCapacity,            Translation.Key.RadioChannels,          Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioContentsUsedPercent,         Translation.Key.RadioContents,          Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioContentsUsed,                Translation.Key.RadioContents,          Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsRadioContentsCapacity,            Translation.Key.RadioContents,          Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTransportLinesUsedPercent,        Translation.Key.TransportLines,         Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTransportLinesUsed,               Translation.Key.TransportLines,         Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTransportLinesCapacity,           Translation.Key.TransportLines,         Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTreesUsedPercent,                 Translation.Key.Trees,                  Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTreesUsed,                        Translation.Key.Trees,                  Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsTreesCapacity,                    Translation.Key.Trees,                  Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesUsedPercent,              Translation.Key.Vehicles,               Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesUsed,                     Translation.Key.Vehicles,               Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesCapacity,                 Translation.Key.Vehicles,               Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesParkedUsedPercent,        Translation.Key.VehiclesParked,         Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesParkedUsed,               Translation.Key.VehiclesParked,         Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsVehiclesParkedCapacity,           Translation.Key.VehiclesParked,         Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsZoneBlocksUsedPercent,            Translation.Key.ZoneBlocks,             Translation.Key.Used,           Translation.Key.PctOfCapacity,              colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsZoneBlocksUsed,                   Translation.Key.ZoneBlocks,             Translation.Key.Used,           Translation.Key.Amount,                     colorNeutral1               ));
            //category.Statistics.Add(new Statistic(category, Statistic.StatisticType.GameLimitsZoneBlocksCapacity,               Translation.Key.ZoneBlocks,             Translation.Key.Capacity,       Translation.Key.Amount,                     colorNeutral2               ));

            #endregion


            // verify each category type is created exactly once
            foreach (Category.CategoryType categoryType in Enum.GetValues(typeof(Category.CategoryType)))
            {
                int found = 0;
                foreach (Category cat in _instance)
                {
                    if (categoryType == cat.Type)
                    {
                        found++;
                    }
                }
                if (found != 1)
                {
                    LogUtil.LogError($"Category type [{categoryType}] is created {found} times, but should be created exactly once.");
                }
            }

            // verify statistic types
            Statistic.StatisticType[] statisticTypes = (Statistic.StatisticType[])Enum.GetValues(typeof(Statistic.StatisticType));
            foreach (Statistic.StatisticType statisticType in statisticTypes)
            {
                // verify each statistic type is created exactly once in all categories
                int found = 0;
                foreach (Category cat in _instance)
                {
                    foreach (Statistic statistic in cat.Statistics)
                    {
                        if (statisticType == statistic.Type)
                        {
                            found++;
                        }
                    }
                }
                if (found != 1)
                {
                    LogUtil.LogError($"Statistic type [{statisticType}] is created {found} times, but should be created exactly once.");
                }

                // verify statistic type has a field or property in the snapshot
                Snapshot.GetFieldProperty(statisticType, out FieldInfo field, out PropertyInfo property);
                if (field == null && property == null)
                {
                    LogUtil.LogError($"Statistic type [{statisticType}] is not defined as a field or property in the snapshot.");
                }
            }

            // verify every field and property in the snapshot (except SnapshotDate) has a statistic type
            List<string> fieldPropertyNames = new List<string>();
            foreach (FieldInfo field in typeof(Snapshot).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                fieldPropertyNames.Add(field.Name);
            }
            foreach (PropertyInfo property in typeof(Snapshot).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                fieldPropertyNames.Add(property.Name);
            }
            foreach (string fieldPropertyName in fieldPropertyNames)
            {
                if (fieldPropertyName != "SnapshotDate")
                {
                    bool found = false;
                    foreach (Statistic.StatisticType statisticType in statisticTypes)
                    {
                        if (fieldPropertyName == statisticType.ToString())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        LogUtil.LogError($"Snapshot field/property [{fieldPropertyName}] is not defined as a statistic type.");
                    }
                }
            }
        }

        /// <summary>
        /// deinitialize categories
        /// </summary>
        public void Deinitialize()
        {
            _instance.Clear();
        }

        /// <summary>
        /// return the count of selected statistics
        /// </summary>
        public int CountSelected
        {
            get
            {
                int count = 0;
                foreach (Category category in _instance)
                {
                    count += category.Statistics.CountSelected;
                }
                return count;
            }
        }

        /// <summary>
        /// return selected statistics
        /// </summary>
        public Statistics SelectedStatistics
        {
            get
            {
                Statistics selectedStatistics = new Statistics();
                foreach (Category category in _instance)
                {
                    foreach (Statistic statistic in category.Statistics)
                    {
                        if (statistic.Selected)
                        {
                            selectedStatistics.Add(statistic);
                        }
                    }
                }
                return selectedStatistics;
            }
        }

        /// <summary>
        /// return all statistics
        /// </summary>
        public Statistics AllStatistics
        {
            get
            {
                Statistics allStatistics = new Statistics();
                foreach (Category category in _instance)
                {
                    foreach (Statistic statistic in category.Statistics)
                    {
                        allStatistics.Add(statistic);
                    }
                }
                return allStatistics;
            }
        }

        /// <summary>
        /// create UI
        /// </summary>
        public bool CreateUI(UIScrollablePanel categoriesScrollablePanel)
        {
            // create UI for each category
            foreach (Category category in _instance)
            {
                if (!category.CreateUI(categoriesScrollablePanel))
                {
                    return false;
                }
            }

            // success
            return true;
        }

        /// <summary>
        /// update UI text
        /// </summary>
        public void UpdateUIText()
        {
            foreach (Category category in _instance)
            {
                category.UpdateUIText();
            }
        }

        /// <summary>
        /// update all statistic amounts
        /// </summary>
        public void UpdateStatisticAmounts(Snapshot snapshot)
        {
            foreach (Category category in _instance)
            {
                category.UpdateStatisticAmounts(snapshot);
            }
        }

        /// <summary>
        /// expand all categories
        /// </summary>
        public void ExpandAll()
        {
            foreach (Category category in _instance)
            {
                category.Expanded = true;
            }
        }

        /// <summary>
        /// collapse all categories
        /// </summary>
        public void CollapseAll()
        {
            foreach (Category category in _instance)
            {
                category.Expanded = false;
            }
        }

        /// <summary>
        /// deselect all statistics
        /// </summary>
        public void DeselectAllStatistics()
        {
            foreach (Category category in _instance)
            {
                foreach (Statistic statistic in category.Statistics)
                {
                    statistic.Selected = false;
                }
            }
        }

        /// <summary>
        /// write the categories to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // write each category
            foreach (Category category in _instance)
            {
                category.Serialize(writer);
            }
        }

        /// <summary>
        /// read the categories from the game save file
        /// </summary>
        public void Deserialize(BinaryReader reader, int version)
        {
            // read each category
            foreach (Category category in _instance)
            {
                category.Deserialize(reader, version);
            }
        }
    }
}
