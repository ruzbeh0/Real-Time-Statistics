using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static DistrictPolicies;
using UnityEngine.SocialPlatforms;

namespace RealTimeStatistics
{
    /// <summary>
    /// a snapshot in time of statistical data
    /// </summary>
    public class Snapshot : IComparable<Snapshot>
    {
        // size of one side of a square in meters
        private const float SquareSize = 8f;


        // date of the snapshot
        public DateTime SnapshotDate;

        // the following field/property names must exactly match the StatisticType enum member names

        // values use the same type as the game's type for that value, this maintains the same precision and range as the game

        // statistics from DLCs are always nullable so the value can be null when the required DLC is not active
        // in the original version 1.0 of this mod, all statistics from the base game are non-nullable because they can never be missing
        // in subsequent versions of this mod, an added statistic must be nullable so it can be set to null when reading previously saved snapshots that do not have the statistic

        // computed percents are defined as floats because more precision is not needed for a percent

        #region Snapshot Values

        // Location
        public int LocationHome = 0;
        public int LocationWork = 0;
        public int LocationVisit = 0;
        public int LocationMoving = 0;

        // Citizen
        public int CitizenOnBike = 0;
        public int CitizenWaitingTransport = 0;
        public int CitizenBored = 0;

        // Water
        public float WaterConsumptionPercent { get { return ComputePercent(WaterConsumption, WaterPumpingCapacity); } }
        public int WaterConsumption;
        public int WaterPumpingCapacity;

        // Water Tank
        public float? WaterTankReservedPercent { get { return ComputePercent(WaterTankReserved, WaterTankStorageCapacity); } }
        public int? WaterTankReserved;
        public int? WaterTankStorageCapacity;

        // Sewage
        public float SewageProductionPercent { get { return ComputePercent(SewageProduction, SewageDrainingCapacity); } }
        public int SewageProduction;
        public int SewageDrainingCapacity;

        // Landfill
        public float LandfillStoragePercent { get { return ComputePercent(LandfillStorage, LandfillCapacity); } }
        public int LandfillStorage;
        public int LandfillCapacity;

        // Garbage
        public float GarbageProductionPercent { get { return ComputePercent(GarbageProduction, GarbageProcessingCapacity); } }
        public int GarbageProduction;
        public int GarbageProcessingCapacity;

        // Education
        public float EducationElementaryEligiblePercent { get { return ComputePercent(EducationElementaryEligible, EducationElementaryCapacity); } }
        public int EducationElementaryEligible;
        public int EducationElementaryCapacity;
        public float EducationHighSchoolEligiblePercent { get { return ComputePercent(EducationHighSchoolEligible, EducationHighSchoolCapacity); } }
        public int EducationHighSchoolEligible;
        public int EducationHighSchoolCapacity;
        public float EducationUniversityEligiblePercent { get { return ComputePercent(EducationUniversityEligible, EducationUniversityCapacity); } }
        public int EducationUniversityEligible;
        public int EducationUniversityCapacity;
        public float EducationLibraryUsersPercent { get { return ComputePercent(EducationLibraryUsers, EducationLibraryCapacity); } }
        public int EducationLibraryUsers;
        public int EducationLibraryCapacity;

        // Education Level
        public float EducationLevelUneducatedPercent { get { return ComputePercent(EducationLevelUneducated, PopulationTotal); } }
        public float EducationLevelEducatedPercent { get { return ComputePercent(EducationLevelEducated, PopulationTotal); } }
        public float EducationLevelWellEducatedPercent { get { return ComputePercent(EducationLevelWellEducated, PopulationTotal); } }
        public float EducationLevelHighlyEducatedPercent { get { return ComputePercent(EducationLevelHighlyEducated, PopulationTotal); } }
        public uint EducationLevelUneducated;
        public uint EducationLevelEducated;
        public uint EducationLevelWellEducated;
        public uint EducationLevelHighlyEducated;

        // Happiness
        public byte HappinessGlobal;
        public byte HappinessResidential;
        public byte HappinessCommercial;
        public byte HappinessIndustrial;
        public byte HappinessOffice;

        // Healthcare
        public byte HealthcareAverageHealth;
        public float HealthcareSickPercent { get { return ComputePercent(HealthcareSick, HealthcareHealCapacity); } }
        public int HealthcareSick;
        public int HealthcareHealCapacity;

        // Deathcare
        public float DeathcareCemeteryBuriedPercent { get { return ComputePercent(DeathcareCemeteryBuried, DeathcareCemeteryCapacity); } }
        public int DeathcareCemeteryBuried;
        public int DeathcareCemeteryCapacity;
        public float DeathcareCrematoriumDeceasedPercent { get { return ComputePercent(DeathcareCrematoriumDeceased, DeathcareCrematoriumCapacity); } }
        public int DeathcareCrematoriumDeceased;
        public int DeathcareCrematoriumCapacity;
        public uint DeathcareDeathRate;

        // Childcare
        public byte ChildcareAverageHealth;
        public float ChildcareSickPercent { get { return ComputePercent(ChildcareSick, ChildcarePopulation); } }
        public uint ChildcareSick;
        public uint ChildcarePopulation { get { return PopulationChildren + PopulationTeens; } }
        public uint ChildcareBirthRate;

        // Eldercare
        public byte EldercareAverageHealth;
        public float EldercareSickPercent { get { return ComputePercent(EldercareSick, EldercarePopulation); } }
        public uint EldercareSick;
        public uint EldercarePopulation { get { return PopulationSeniors; } }
        public int EldercareAverageLifeSpan;

        // Zoning
        public float ZoningResidentialPercent { get { return ComputePercent(ZoningResidential, ZoningTotal); } }
        public float ZoningCommercialPercent { get { return ComputePercent(ZoningCommercial, ZoningTotal); } }
        public float ZoningIndustrialPercent { get { return ComputePercent(ZoningIndustrial, ZoningTotal); } }
        public float ZoningOfficePercent { get { return ComputePercent(ZoningOffice, ZoningTotal); } }
        public float ZoningUnzonedPercent { get { return ComputePercent(ZoningUnzoned, ZoningTotal); } }
        public int ZoningTotal { get { return ZoningResidential + ZoningCommercial + ZoningIndustrial + ZoningOffice + ZoningUnzoned; } }
        public int ZoningResidential;
        public int ZoningCommercial;
        public int ZoningIndustrial;
        public int ZoningOffice;
        public int ZoningUnzoned;

        // Zone Level
        public float ZoneLevelResidentialAverage { get { return ComputeZoneLevelAverage(ZoneLevelResidential1, ZoneLevelResidential2, ZoneLevelResidential3, ZoneLevelResidential4, ZoneLevelResidential5); } }
        public byte ZoneLevelResidential1;
        public byte ZoneLevelResidential2;
        public byte ZoneLevelResidential3;
        public byte ZoneLevelResidential4;
        public byte ZoneLevelResidential5;
        public float ZoneLevelCommercialAverage { get { return ComputeZoneLevelAverage(ZoneLevelCommercial1, ZoneLevelCommercial2, ZoneLevelCommercial3); } }
        public byte ZoneLevelCommercial1;
        public byte ZoneLevelCommercial2;
        public byte ZoneLevelCommercial3;
        public float ZoneLevelIndustrialAverage { get { return ComputeZoneLevelAverage(ZoneLevelIndustrial1, ZoneLevelIndustrial2, ZoneLevelIndustrial3); } }
        public byte ZoneLevelIndustrial1;
        public byte ZoneLevelIndustrial2;
        public byte ZoneLevelIndustrial3;
        public float ZoneLevelOfficeAverage { get { return ComputeZoneLevelAverage(ZoneLevelOffice1, ZoneLevelOffice2, ZoneLevelOffice3); } }
        public byte ZoneLevelOffice1;
        public byte ZoneLevelOffice2;
        public byte ZoneLevelOffice3;

        // Zone Buildings
        public float ZoneBuildingsResidentialPercent { get { return ComputePercent(ZoneBuildingsResidential, ZoneBuildingsTotal); } }
        public float ZoneBuildingsCommercialPercent { get { return ComputePercent(ZoneBuildingsCommercial, ZoneBuildingsTotal); } }
        public float ZoneBuildingsIndustrialPercent { get { return ComputePercent(ZoneBuildingsIndustrial, ZoneBuildingsTotal); } }
        public float ZoneBuildingsOfficePercent { get { return ComputePercent(ZoneBuildingsOffice, ZoneBuildingsTotal); } }
        public uint ZoneBuildingsTotal { get { return ZoneBuildingsResidential + ZoneBuildingsCommercial + ZoneBuildingsIndustrial + ZoneBuildingsOffice; } }
        public uint ZoneBuildingsResidential;
        public uint ZoneBuildingsCommercial;
        public uint ZoneBuildingsIndustrial;
        public uint ZoneBuildingsOffice;

        // Zone Demand
        public int ZoneDemandResidential;
        public int ZoneDemandCommercial;
        public int ZoneDemandIndustrialOffice;

        // Traffic
        public uint TrafficAverageFlow;

        // Pollution
        public int PollutionAverageGround;
        public int PollutionAverageDrinkingWater;
        public int PollutionAverageNoise;

        // Fire Safety
        public int FireSafetyHazard;

        // Crime
        public byte CrimeRate;
        public float CrimeDetainedCriminalsPercent { get { return ComputePercent(CrimeDetainedCriminals, CrimeJailsCapacity); } }
        public int CrimeDetainedCriminals;
        public int CrimeJailsCapacity;

        // Public Transportation
        public uint PublicTransportationTotalTotal { get { return PublicTransportationTotalResidents + PublicTransportationTotalTourists; } }
        public uint PublicTransportationTotalResidents { get { return PublicTransportationBusResidents +
                                                                           (PublicTransportationTrolleybusResidents ?? 0) +
                                                                           (PublicTransportationTramResidents ?? 0) +
                                                                           PublicTransportationMetroResidents +
                                                                           PublicTransportationTrainResidents +
                                                                           PublicTransportationShipResidents +
                                                                           PublicTransportationAirResidents +
                                                                           (PublicTransportationMonorailResidents ?? 0) +
                                                                           (PublicTransportationCableCarResidents ?? 0) +
                                                                           (PublicTransportationTaxiResidents ?? 0); } }
        public uint PublicTransportationTotalTourists { get { return PublicTransportationBusTourists +
                                                                           (PublicTransportationTrolleybusTourists ?? 0) +
                                                                           (PublicTransportationTramTourists ?? 0) +
                                                                           PublicTransportationMetroTourists +
                                                                           PublicTransportationTrainTourists +
                                                                           PublicTransportationShipTourists +
                                                                           PublicTransportationAirTourists +
                                                                           (PublicTransportationMonorailTourists ?? 0) +
                                                                           (PublicTransportationCableCarTourists ?? 0) +
                                                                           (PublicTransportationTaxiTourists ?? 0); } }
        public uint PublicTransportationBusTotal { get { return PublicTransportationBusResidents + PublicTransportationBusTourists; } }
        public uint PublicTransportationBusResidents;
        public uint PublicTransportationBusTourists;
        public uint? PublicTransportationTrolleybusTotal { get { return PublicTransportationTrolleybusResidents + PublicTransportationTrolleybusTourists; } }
        public uint? PublicTransportationTrolleybusResidents;
        public uint? PublicTransportationTrolleybusTourists;
        public uint? PublicTransportationTramTotal { get { return PublicTransportationTramResidents + PublicTransportationTramTourists; } }
        public uint? PublicTransportationTramResidents;
        public uint? PublicTransportationTramTourists;
        public uint PublicTransportationMetroTotal { get { return PublicTransportationMetroResidents + PublicTransportationMetroTourists; } }
        public uint PublicTransportationMetroResidents;
        public uint PublicTransportationMetroTourists;
        public uint PublicTransportationTrainTotal { get { return PublicTransportationTrainResidents + PublicTransportationTrainTourists; } }
        public uint PublicTransportationTrainResidents;
        public uint PublicTransportationTrainTourists;
        public uint PublicTransportationShipTotal { get { return PublicTransportationShipResidents + PublicTransportationShipTourists; } }
        public uint PublicTransportationShipResidents;
        public uint PublicTransportationShipTourists;
        public uint PublicTransportationAirTotal { get { return PublicTransportationAirResidents + PublicTransportationAirTourists; } }
        public uint PublicTransportationAirResidents;
        public uint PublicTransportationAirTourists;
        public uint? PublicTransportationMonorailTotal { get { return PublicTransportationMonorailResidents + PublicTransportationMonorailTourists; } }
        public uint? PublicTransportationMonorailResidents;
        public uint? PublicTransportationMonorailTourists;
        public uint? PublicTransportationCableCarTotal { get { return PublicTransportationCableCarResidents + PublicTransportationCableCarTourists; } }
        public uint? PublicTransportationCableCarResidents;
        public uint? PublicTransportationCableCarTourists;
        public uint? PublicTransportationTaxiTotal { get { return PublicTransportationTaxiResidents + PublicTransportationTaxiTourists; } }
        public uint? PublicTransportationTaxiResidents;
        public uint? PublicTransportationTaxiTourists;

        // Population
        public uint PopulationTotal { get { return PopulationChildren + PopulationTeens + PopulationYoungAdults + PopulationAdults + PopulationSeniors; } }
        public float PopulationChildrenPercent { get { return ComputePercent(PopulationChildren, PopulationTotal); } }
        public float PopulationTeensPercent { get { return ComputePercent(PopulationTeens, PopulationTotal); } }
        public float PopulationYoungAdultsPercent { get { return ComputePercent(PopulationYoungAdults, PopulationTotal); } }
        public float PopulationAdultsPercent { get { return ComputePercent(PopulationAdults, PopulationTotal); } }
        public float PopulationSeniorsPercent { get { return ComputePercent(PopulationSeniors, PopulationTotal); } }
        public uint PopulationChildren;
        public uint PopulationTeens;
        public uint PopulationYoungAdults;
        public uint PopulationAdults;
        public uint PopulationSeniors;

        // Households
        public float HouseholdsOccupiedPercent { get { return ComputePercent(HouseholdsOccupied, HouseholdsAvailable); } }
        public uint HouseholdsOccupied;
        public uint HouseholdsAvailable;

        // Employment
        public int EmploymentPeopleEmployed;
        public int EmploymentJobsAvailable;
        public int EmploymentUnfilledJobs { get { return EmploymentJobsAvailable - EmploymentPeopleEmployed; } }
        public float EmploymentUnemploymentPercent { get { return ComputePercent(EmploymentUnemployed, EmploymentEligibleWorkers); } }
        public uint EmploymentUnemployed;
        public uint EmploymentEligibleWorkers;

        // Outside Connections
        public int OutsideConnectionsImportTotal { get { return OutsideConnectionsImportGoods +
                                                                           OutsideConnectionsImportForestry +
                                                                           OutsideConnectionsImportFarming +
                                                                           OutsideConnectionsImportOre +
                                                                           OutsideConnectionsImportOil +
                                                                           (OutsideConnectionsImportMail ?? 0); } }
        public int OutsideConnectionsImportGoods;
        public int OutsideConnectionsImportForestry;
        public int OutsideConnectionsImportFarming;
        public int OutsideConnectionsImportOre;
        public int OutsideConnectionsImportOil;
        public int? OutsideConnectionsImportMail;
        public int OutsideConnectionsExportTotal { get { return OutsideConnectionsExportGoods +
                                                                           OutsideConnectionsExportForestry +
                                                                           OutsideConnectionsExportFarming +
                                                                           OutsideConnectionsExportOre +
                                                                           OutsideConnectionsExportOil +
                                                                           (OutsideConnectionsExportMail ?? 0) +
                                                                           (OutsideConnectionsExportFish ?? 0); } }
        public int OutsideConnectionsExportGoods;
        public int OutsideConnectionsExportForestry;
        public int OutsideConnectionsExportFarming;
        public int OutsideConnectionsExportOre;
        public int OutsideConnectionsExportOil;
        public int? OutsideConnectionsExportMail;
        public int? OutsideConnectionsExportFish;

        // Land Value
        public int LandValueAverage;

        // Natural Resources
        public float NaturalResourcesForestUsedPercent { get { return ComputePercent(NaturalResourcesForestUsed, NaturalResourcesForestAvailable); } }
        public uint NaturalResourcesForestUsed;
        public uint NaturalResourcesForestAvailable;
        public float NaturalResourcesFertileLandUsedPercent { get { return ComputePercent(NaturalResourcesFertileLandUsed, NaturalResourcesFertileLandAvailable); } }
        public uint NaturalResourcesFertileLandUsed;
        public uint NaturalResourcesFertileLandAvailable;
        public float NaturalResourcesOreUsedPercent { get { return ComputePercent(NaturalResourcesOreUsed, NaturalResourcesOreAvailable); } }
        public uint NaturalResourcesOreUsed;
        public uint NaturalResourcesOreAvailable;
        public float NaturalResourcesOilUsedPercent { get { return ComputePercent(NaturalResourcesOilUsed, NaturalResourcesOilAvailable); } }
        public uint NaturalResourcesOilUsed;
        public uint NaturalResourcesOilAvailable;

        // Heating
        public float? HeatingConsumptionPercent { get { return ComputePercent(HeatingConsumption, HeatingProduction); } }
        public int? HeatingConsumption;
        public int? HeatingProduction;

        // Tourism
        public int TourismCityAttractiveness;
        public float TourismLowWealthPercent { get { return ComputePercent(TourismLowWealth, TourismTotal); } }
        public float TourismMediumWealthPercent { get { return ComputePercent(TourismMediumWealth, TourismTotal); } }
        public float TourismHighWealthPercent { get { return ComputePercent(TourismHighWealth, TourismTotal); } }
        public uint TourismTotal { get { return TourismLowWealth + TourismMediumWealth + TourismHighWealth; } }
        public uint TourismLowWealth;
        public uint TourismMediumWealth;
        public uint TourismHighWealth;
        public float? TourismExchangeStudentBonus;

        // Tours
        public uint? ToursTotalTotal { get { return ToursTotalResidents + ToursTotalTourists; } }
        public uint? ToursTotalResidents { get { return ToursWalkingTourResidents + ToursSightseeingResidents + ToursBalloonResidents; } }
        public uint? ToursTotalTourists { get { return ToursWalkingTourTourists + ToursSightseeingTourists + ToursBalloonToursits; } }
        public uint? ToursWalkingTourTotal { get { return ToursWalkingTourResidents + ToursWalkingTourTourists; } }
        public uint? ToursWalkingTourResidents;
        public uint? ToursWalkingTourTourists;
        public uint? ToursSightseeingTotal { get { return ToursSightseeingResidents + ToursSightseeingTourists; } }
        public uint? ToursSightseeingResidents;
        public uint? ToursSightseeingTourists;
        public uint? ToursBalloonTotal { get { return ToursBalloonResidents + ToursBalloonToursits; } }
        public uint? ToursBalloonResidents;
        public uint? ToursBalloonToursits;

        // Tax Rate
        public int TaxRateResidentialLow;
        public int TaxRateResidentialHigh;
        public int TaxRateCommercialLow;
        public int TaxRateCommercialHigh;
        public int TaxRateIndustrial;
        public int TaxRateOffice;

        // City Economy
        public long CityEconomyTotalIncome;
        public long CityEconomyTotalExpenses;
        public long CityEconomyTotalProfit { get { return CityEconomyTotalIncome - CityEconomyTotalExpenses; } }
        public long? CityEconomyBankBalance;
        public long? CityEconomyLoanBalance;
        public long? CityEconomyCityValue;
        public float? CityEconomyCityValuePerCapita { get { return ComputePerCapita(CityEconomyCityValue); } }
        public long CityEconomyGrossDomesticProduct { get { return CityEconomyConsumption + CityEconomyGovernmentSpending + CityEconomyNetExports; } }
        public float CityEconomyGrossDomesticProductPerCapita { get { return (float)ComputePerCapita(CityEconomyGrossDomesticProduct); } }
        public long CityEconomyConsumption
        {
            get
            {
                // Consumption is the sum of consumption across all zone types.
                // Consumption is the zone type's income minus taxes (i.e. assume all nontaxed income is eventually spent and not saved forever in a bank).
                // The taxes your city collects from a zone type is the zone type's income times its tax rate, so a zone type's income is the zone type's taxes divided by the tax rate.
                // The tax rate for a zone type in this calculation is the zone type's base tax rate excluding any taxation policies.
                // For example, if a zone type's taxes are ₡100 and the tax rate is 10%, then the zone type's income is ₡100 / 10% = ₡1,000 and consumption is ₡1,000 - ₡100 = ₡900.

                // logic in CommercialBuildingAI.GetTaxRate for specialized zones uses either low or high density commercial tax rate depending on each individual building
                // because this consumption logic is not at the building level, use the average of the low and high density commercial tax rates for specialized commercial
                return
                    (TaxRateResidentialLow == 0 ? 0 : (100 * ResidentialIncomeLowDensityTotal / TaxRateResidentialLow - ResidentialIncomeLowDensityTotal)) +
                    (TaxRateResidentialHigh == 0 ? 0 : (100 * ResidentialIncomeHighDensityTotal / TaxRateResidentialHigh - ResidentialIncomeHighDensityTotal)) +
                    (TaxRateCommercialLow == 0 ? 0 : (100 * CommercialIncomeLowDensityTotal / TaxRateCommercialLow - CommercialIncomeLowDensityTotal)) +
                    (TaxRateCommercialHigh == 0 ? 0 : (100 * CommercialIncomeHighDensityTotal / TaxRateCommercialHigh - CommercialIncomeHighDensityTotal)) +
                    (TaxRateCommercialLow + TaxRateCommercialHigh == 0 ? 0 : (100 * (CommercialIncomeSpecializedTotal ?? 0) / ((TaxRateCommercialLow + TaxRateCommercialHigh) / 2) - (CommercialIncomeSpecializedTotal ?? 0))) +
                    (TaxRateIndustrial == 0 ? 0 : (100 * IndustrialIncomeTotal / TaxRateIndustrial - IndustrialIncomeTotal)) +
                    (TaxRateOffice == 0 ? 0 : (100 * OfficeIncomeTotal / TaxRateOffice - OfficeIncomeTotal));
            }
        }
        public float CityEconomyConsumptionPercent { get { return ComputePercent(CityEconomyConsumption, CityEconomyGrossDomesticProduct); } }
        public long CityEconomyGovernmentSpending { get { return CityEconomyTotalExpenses - ServiceExpensesLoans; } }
        public float CityEconomyGovernmentSpendingPercent { get { return ComputePercent(CityEconomyGovernmentSpending, CityEconomyGrossDomesticProduct); } }

        // in IndustryWorldInfoPanel.UpdateBindings, the value of a resource is computed as:  resource units * resource price / 10000
        // resource price is obtained from IndustryBuildingAI.GetResourcePrice and there are 1000 units/ton
        // for example, the resource price for forestry is ₡200, so 1000 units (i.e. 1 ton) of forestry is valued at:  1000 * 200 / 10000 = ₡20
        // but the counts on the outside connections view are not in tons because the resource units there are divided by 100, not 1000
        // this means the outside connection counts are 10 times too high (it is not known why the game does this)
        // this is handled here by setting the value per outside connection count to 1/10th of the corresponding value/ton from the IndustryWorldInfoPanel logic
        // for example, the value per outside connection count for forestry is ₡2 (from below), so 1000 units is valued at:  1000 / 100 * 2 = ₡20  (i.e. same as 1000 units above)
        // the outside connection counts have already performed the calculation of:  resource units / 100, so all that is left is to multiply OC counts by the value/OC count
        // mail is not included in the calculations because no value for mail is given in IndustryBuildingAI.GetResourcePrice (i.e. mail has no real value)
        private const int ValuePerOCCountGoods = 15;
        private const int ValuePerOCCountForestry = 2;
        private const int ValuePerOCCountFarming = 2;
        private const int ValuePerOCCountOre = 3;
        private const int ValuePerOCCountOil = 4;
        private const int ValuePerOCCountFish = 6;
        public int CityEconomyExports { get { return OutsideConnectionsExportGoods * ValuePerOCCountGoods +
                                                                           OutsideConnectionsExportForestry * ValuePerOCCountForestry +
                                                                           OutsideConnectionsExportFarming * ValuePerOCCountFarming +
                                                                           OutsideConnectionsExportOre * ValuePerOCCountOre +
                                                                           OutsideConnectionsExportOil * ValuePerOCCountOil +
                                                                           (OutsideConnectionsExportFish ?? 0) * ValuePerOCCountFish; } }
        public int CityEconomyImports { get { return OutsideConnectionsImportGoods * ValuePerOCCountGoods +
                                                                           OutsideConnectionsImportForestry * ValuePerOCCountForestry +
                                                                           OutsideConnectionsImportFarming * ValuePerOCCountFarming +
                                                                           OutsideConnectionsImportOre * ValuePerOCCountOre +
                                                                           OutsideConnectionsImportOil * ValuePerOCCountOil; } }
        public int CityEconomyNetExports { get { return CityEconomyExports - CityEconomyImports; } }
        public float CityEconomyNetExportsPercent { get { return ComputePercent(CityEconomyNetExports, CityEconomyGrossDomesticProduct); } }

        // Residential Income
        public float ResidentialIncomeTotalPercent { get { return ComputePercent(ResidentialIncomeTotal, CityEconomyTotalIncome); } }
        public long ResidentialIncomeTotal { get { return ResidentialIncomeLowDensityTotal + ResidentialIncomeHighDensityTotal; } }
        public long ResidentialIncomeLowDensityTotal { get { return ResidentialIncomeLowDensity1 +
                                                                           ResidentialIncomeLowDensity2 +
                                                                           ResidentialIncomeLowDensity3 +
                                                                           ResidentialIncomeLowDensity4 +
                                                                           ResidentialIncomeLowDensity5 +
                                                                           (ResidentialIncomeLowDensitySelfSufficient ?? 0); } }
        public long ResidentialIncomeLowDensity1;
        public long ResidentialIncomeLowDensity2;
        public long ResidentialIncomeLowDensity3;
        public long ResidentialIncomeLowDensity4;
        public long ResidentialIncomeLowDensity5;
        public long? ResidentialIncomeLowDensitySelfSufficient;
        public long ResidentialIncomeHighDensityTotal { get { return ResidentialIncomeHighDensity1 +
                                                                           ResidentialIncomeHighDensity2 +
                                                                           ResidentialIncomeHighDensity3 +
                                                                           ResidentialIncomeHighDensity4 +
                                                                           ResidentialIncomeHighDensity5 +
                                                                           (ResidentialIncomeHighDensitySelfSufficient ?? 0); } }
        public long ResidentialIncomeHighDensity1;
        public long ResidentialIncomeHighDensity2;
        public long ResidentialIncomeHighDensity3;
        public long ResidentialIncomeHighDensity4;
        public long ResidentialIncomeHighDensity5;
        public long? ResidentialIncomeHighDensitySelfSufficient;

        // Commercial Income
        public float CommercialIncomeTotalPercent { get { return ComputePercent(CommercialIncomeTotal, CityEconomyTotalIncome); } }
        public long CommercialIncomeTotal { get { return CommercialIncomeLowDensityTotal + CommercialIncomeHighDensityTotal + (CommercialIncomeSpecializedTotal ?? 0); } }
        public long CommercialIncomeLowDensityTotal { get { return CommercialIncomeLowDensity1 + CommercialIncomeLowDensity2 + CommercialIncomeLowDensity3; } }
        public long CommercialIncomeLowDensity1;
        public long CommercialIncomeLowDensity2;
        public long CommercialIncomeLowDensity3;
        public long CommercialIncomeHighDensityTotal { get { return CommercialIncomeHighDensity1 + CommercialIncomeHighDensity2 + CommercialIncomeHighDensity3; } }
        public long CommercialIncomeHighDensity1;
        public long CommercialIncomeHighDensity2;
        public long CommercialIncomeHighDensity3;
        public long? CommercialIncomeSpecializedTotal
        {
            get
            {
                if (CommercialIncomeLeisure.HasValue || CommercialIncomeTourism.HasValue || CommercialIncomeOrganic.HasValue)
                {
                    return (CommercialIncomeLeisure ?? 0) + (CommercialIncomeTourism ?? 0) + (CommercialIncomeOrganic ?? 0);
                }
                return null;
            }
        }
        public long? CommercialIncomeLeisure;
        public long? CommercialIncomeTourism;
        public long? CommercialIncomeOrganic;

        // Industrial Income
        public float IndustrialIncomeTotalPercent { get { return ComputePercent(IndustrialIncomeTotal, CityEconomyTotalIncome); } }
        public long IndustrialIncomeTotal { get { return IndustrialIncomeGenericTotal + IndustrialIncomeSpecializedTotal; } }
        public long IndustrialIncomeGenericTotal { get { return IndustrialIncomeGeneric1 + IndustrialIncomeGeneric2 + IndustrialIncomeGeneric3; } }
        public long IndustrialIncomeGeneric1;
        public long IndustrialIncomeGeneric2;
        public long IndustrialIncomeGeneric3;
        public long IndustrialIncomeSpecializedTotal { get { return IndustrialIncomeForestry + IndustrialIncomeFarming + IndustrialIncomeOre + IndustrialIncomeOil; } }
        public long IndustrialIncomeForestry;
        public long IndustrialIncomeFarming;
        public long IndustrialIncomeOre;
        public long IndustrialIncomeOil;

        //  Office Income
        public float OfficeIncomeTotalPercent { get { return ComputePercent(OfficeIncomeTotal, CityEconomyTotalIncome); } }
        public long OfficeIncomeTotal { get { return OfficeIncomeGenericTotal + (OfficeIncomeITCluster ?? 0); } }
        public long OfficeIncomeGenericTotal { get { return OfficeIncomeGeneric1 + OfficeIncomeGeneric2 + OfficeIncomeGeneric3; } }
        public long OfficeIncomeGeneric1;
        public long OfficeIncomeGeneric2;
        public long OfficeIncomeGeneric3;
        public long? OfficeIncomeITCluster;

        // Tourism Income
        public float TourismIncomeTotalPercent { get { return ComputePercent(TourismIncomeTotal, CityEconomyTotalIncome); } }
        public long TourismIncomeTotal { get { return TourismIncomeCommercialZones + TourismIncomeTransportation + (TourismIncomeParkAreas ?? 0); } }
        public long TourismIncomeCommercialZones;
        public long TourismIncomeTransportation;
        public long? TourismIncomeParkAreas;

        // Service Expenses
        public float ServiceExpensesTotalPercent { get { return ComputePercent(ServiceExpensesTotal, CityEconomyTotalExpenses); } }
        public long ServiceExpensesTotal { get { return ServiceExpensesRoads +
                                                                           ServiceExpensesElectricity +
                                                                           ServiceExpensesWaterSewageHeating +
                                                                           ServiceExpensesGarbage +
                                                                           ServiceExpensesHealthcare +
                                                                           ServiceExpensesFire +
                                                                           (ServiceExpensesEmergency ?? 0) +
                                                                           ServiceExpensesPolice +
                                                                           ServiceExpensesEducation +
                                                                           ServiceExpensesParksPlazas +
                                                                           ServiceExpensesUniqueBuildings +
                                                                           (ServiceExpensesGenericSportsArenas ?? 0) +
                                                                           ServiceExpensesLoans +
                                                                           ServiceExpensesPolicies; } }
        public long ServiceExpensesRoads;
        public long ServiceExpensesElectricity;
        public long ServiceExpensesWaterSewageHeating;
        public long ServiceExpensesGarbage;
        public long ServiceExpensesHealthcare;
        public long ServiceExpensesFire;
        public long? ServiceExpensesEmergency;
        public long ServiceExpensesPolice;
        public long ServiceExpensesEducation;
        public long ServiceExpensesParksPlazas;
        public long ServiceExpensesUniqueBuildings;
        public long? ServiceExpensesGenericSportsArenas;
        public long ServiceExpensesLoans;
        public long ServiceExpensesPolicies;

        // Park Areas
        public float? ParkAreasTotalIncomePercent { get { return ComputePercent(ParkAreasTotalIncome, CityEconomyTotalIncome); } }
        public float? ParkAreasTotalExpensesPercent { get { return ComputePercent(ParkAreasTotalExpenses, CityEconomyTotalExpenses); } }
        public long? ParkAreasTotalIncome { get { return ParkAreasCityParkIncome + ParkAreasAmusementParkIncome + ParkAreasZooIncome + ParkAreasNatureReserveIncome; } }
        public long? ParkAreasTotalExpenses { get { return ParkAreasCityParkExpenses + ParkAreasAmusementParkExpenses + ParkAreasZooExpenses + ParkAreasNatureReserveExpenses; } }
        public long? ParkAreasTotalProfit { get { return ParkAreasTotalIncome - ParkAreasTotalExpenses; } }

        public long? ParkAreasCityParkIncome;
        public long? ParkAreasCityParkExpenses;
        public long? ParkAreasCityParkProfit { get { return ParkAreasCityParkIncome - ParkAreasCityParkExpenses; } }
        public long? ParkAreasAmusementParkIncome;
        public long? ParkAreasAmusementParkExpenses;
        public long? ParkAreasAmusementParkProfit { get { return ParkAreasAmusementParkIncome - ParkAreasAmusementParkExpenses; } }
        public long? ParkAreasZooIncome;
        public long? ParkAreasZooExpenses;
        public long? ParkAreasZooProfit { get { return ParkAreasZooIncome - ParkAreasZooExpenses; } }
        public long? ParkAreasNatureReserveIncome;
        public long? ParkAreasNatureReserveExpenses;
        public long? ParkAreasNatureReserveProfit { get { return ParkAreasNatureReserveIncome - ParkAreasNatureReserveExpenses; } }

        // Industry Areas
        public float? IndustryAreasTotalIncomePercent { get { return ComputePercent(IndustryAreasTotalIncome, CityEconomyTotalIncome); } }
        public float? IndustryAreasTotalExpensesPercent { get { return ComputePercent(IndustryAreasTotalExpenses, CityEconomyTotalExpenses); } }
        public long? IndustryAreasTotalIncome { get { return IndustryAreasForestryIncome +
                                                                           IndustryAreasFarmingIncome +
                                                                           IndustryAreasOreIncome +
                                                                           IndustryAreasOilIncome +
                                                                           IndustryAreasWarehousesFactoriesIncome; } }
        public long? IndustryAreasTotalExpenses { get { return IndustryAreasForestryExpenses +
                                                                           IndustryAreasFarmingExpenses +
                                                                           IndustryAreasOreExpenses +
                                                                           IndustryAreasOilExpenses +
                                                                           IndustryAreasWarehousesFactoriesExpenses; } }
        public long? IndustryAreasTotalProfit { get { return IndustryAreasTotalIncome - IndustryAreasTotalExpenses; } }
        public long? IndustryAreasForestryIncome;
        public long? IndustryAreasForestryExpenses;
        public long? IndustryAreasForestryProfit { get { return IndustryAreasForestryIncome - IndustryAreasForestryExpenses; } }
        public long? IndustryAreasFarmingIncome;
        public long? IndustryAreasFarmingExpenses;
        public long? IndustryAreasFarmingProfit { get { return IndustryAreasFarmingIncome - IndustryAreasFarmingExpenses; } }
        public long? IndustryAreasOreIncome;
        public long? IndustryAreasOreExpenses;
        public long? IndustryAreasOreProfit { get { return IndustryAreasOreIncome - IndustryAreasOreExpenses; } }
        public long? IndustryAreasOilIncome;
        public long? IndustryAreasOilExpenses;
        public long? IndustryAreasOilProfit { get { return IndustryAreasOilIncome - IndustryAreasOilExpenses; } }
        public long? IndustryAreasWarehousesFactoriesIncome;
        public long? IndustryAreasWarehousesFactoriesExpenses;
        public long? IndustryAreasWarehousesFactoriesProfit { get { return IndustryAreasWarehousesFactoriesIncome - IndustryAreasWarehousesFactoriesExpenses; } }

        // Fishing Industry
        public long? FishingIndustryFishingIncome;
        public long? FishingIndustryFishingExpenses;
        public long? FishingIndustryFishingProfit { get { return FishingIndustryFishingIncome - FishingIndustryFishingExpenses; } }

        // Campus Areas
        public float? CampusAreasTotalIncomePercent { get { return ComputePercent(CampusAreasTotalIncome, CityEconomyTotalIncome); } }
        public float? CampusAreasTotalExpensesPercent { get { return ComputePercent(CampusAreasTotalExpenses, CityEconomyTotalExpenses); } }
        public long? CampusAreasTotalIncome { get { return CampusAreasTradeSchoolIncome + CampusAreasLiberalArtsCollegeIncome + CampusAreasUniversityIncome; } }
        public long? CampusAreasTotalExpenses { get { return CampusAreasTradeSchoolExpenses + CampusAreasLiberalArtsCollegeExpenses + CampusAreasUniversityExpenses; } }
        public long? CampusAreasTotalProfit { get { return CampusAreasTotalIncome - CampusAreasTotalExpenses; } }
        public long? CampusAreasTradeSchoolIncome;
        public long? CampusAreasTradeSchoolExpenses;
        public long? CampusAreasTradeSchoolProfit { get { return CampusAreasTradeSchoolIncome - CampusAreasTradeSchoolExpenses; } }
        public long? CampusAreasLiberalArtsCollegeIncome;
        public long? CampusAreasLiberalArtsCollegeExpenses;
        public long? CampusAreasLiberalArtsCollegeProfit { get { return CampusAreasLiberalArtsCollegeIncome - CampusAreasLiberalArtsCollegeExpenses; } }
        public long? CampusAreasUniversityIncome;
        public long? CampusAreasUniversityExpenses;
        public long? CampusAreasUniversityProfit { get { return CampusAreasUniversityIncome - CampusAreasUniversityExpenses; } }

        // Transport Economy
        public float? TransportEconomyTotalIncomePercent { get { return ComputePercent(TransportEconomyTotalIncome, CityEconomyTotalIncome); } }
        public float? TransportEconomyTotalExpensesPercent { get { return ComputePercent(TransportEconomyTotalExpenses, CityEconomyTotalExpenses); } }
        public long TransportEconomyTotalIncome { get { return TransportEconomyBusIncome +
                                                                           (TransportEconomyTrolleybusIncome ?? 0) +
                                                                           (TransportEconomyTramIncome ?? 0) +
                                                                           TransportEconomyMetroIncome +
                                                                           TransportEconomyTrainIncome +
                                                                           TransportEconomyShipIncome +
                                                                           TransportEconomyAirIncome +
                                                                           (TransportEconomyMonorailIncome ?? 0) +
                                                                           (TransportEconomyCableCarIncome ?? 0) +
                                                                           (TransportEconomyTaxiIncome ?? 0) +
                                                                           (TransportEconomyToursIncome ?? 0) +
                                                                           TransportEconomyTollBoothIncome; } }
        public long TransportEconomyTotalExpenses { get { return TransportEconomyBusExpenses +
                                                                           (TransportEconomyTrolleybusExpenses ?? 0) +
                                                                           (TransportEconomyTramExpenses ?? 0) +
                                                                           TransportEconomyMetroExpenses +
                                                                           TransportEconomyTrainExpenses +
                                                                           TransportEconomyShipExpenses +
                                                                           TransportEconomyAirExpenses +
                                                                           (TransportEconomyMonorailExpenses ?? 0) +
                                                                           (TransportEconomyCableCarExpenses ?? 0) +
                                                                           (TransportEconomyTaxiExpenses ?? 0) +
                                                                           (TransportEconomyToursExpenses ?? 0) +
                                                                           TransportEconomyTollBoothExpenses +
                                                                           (TransportEconomyMailExpenses ?? 0) +
                                                                           TransportEconomySpaceElevatorExpenses; } }
        public long TransportEconomyTotalProfit { get { return TransportEconomyTotalIncome - TransportEconomyTotalExpenses; } }
        public long TransportEconomyBusIncome;
        public long TransportEconomyBusExpenses;
        public long TransportEconomyBusProfit { get { return TransportEconomyBusIncome - TransportEconomyBusExpenses; } }
        public long? TransportEconomyTrolleybusIncome;
        public long? TransportEconomyTrolleybusExpenses;
        public long? TransportEconomyTrolleybusProfit { get { return TransportEconomyTrolleybusIncome - TransportEconomyTrolleybusExpenses; } }
        public long? TransportEconomyTramIncome;
        public long? TransportEconomyTramExpenses;
        public long? TransportEconomyTramProfit { get { return TransportEconomyTramIncome - TransportEconomyTramExpenses; } }
        public long TransportEconomyMetroIncome;
        public long TransportEconomyMetroExpenses;
        public long TransportEconomyMetroProfit { get { return TransportEconomyMetroIncome - TransportEconomyMetroExpenses; } }
        public long TransportEconomyTrainIncome;
        public long TransportEconomyTrainExpenses;
        public long TransportEconomyTrainProfit { get { return TransportEconomyTrainIncome - TransportEconomyTrainExpenses; } }
        public long TransportEconomyShipIncome;
        public long TransportEconomyShipExpenses;
        public long TransportEconomyShipProfit { get { return TransportEconomyShipIncome - TransportEconomyShipExpenses; } }
        public long TransportEconomyAirIncome;
        public long TransportEconomyAirExpenses;
        public long TransportEconomyAirProfit { get { return TransportEconomyAirIncome - TransportEconomyAirExpenses; } }
        public long? TransportEconomyMonorailIncome;
        public long? TransportEconomyMonorailExpenses;
        public long? TransportEconomyMonorailProfit { get { return TransportEconomyMonorailIncome - TransportEconomyMonorailExpenses; } }
        public long? TransportEconomyCableCarIncome;
        public long? TransportEconomyCableCarExpenses;
        public long? TransportEconomyCableCarProfit { get { return TransportEconomyCableCarIncome - TransportEconomyCableCarExpenses; } }
        public long? TransportEconomyTaxiIncome;
        public long? TransportEconomyTaxiExpenses;
        public long? TransportEconomyTaxiProfit { get { return TransportEconomyTaxiIncome - TransportEconomyTaxiExpenses; } }
        public long? TransportEconomyToursIncome;
        public long? TransportEconomyToursExpenses;
        public long? TransportEconomyToursProfit { get { return TransportEconomyToursIncome - TransportEconomyToursExpenses; } }
        public long TransportEconomyTollBoothIncome;
        public long TransportEconomyTollBoothExpenses;
        public long TransportEconomyTollBoothProfit { get { return TransportEconomyTollBoothIncome - TransportEconomyTollBoothExpenses; } }
        public long? TransportEconomyMailExpenses;
        public long? TransportEconomyMailProfit { get { return 0L - TransportEconomyMailExpenses; } }
        public long TransportEconomySpaceElevatorExpenses;
        public long TransportEconomySpaceElevatorProfit { get { return 0L - TransportEconomySpaceElevatorExpenses; } }

        // Game Limits
        public float GameLimitsBuildingsUsedPercent { get { return ComputePercent(GameLimitsBuildingsUsed, GameLimitsBuildingsCapacity); } }
        public int GameLimitsBuildingsUsed;
        public int GameLimitsBuildingsCapacity;
        public float GameLimitsCitizensUsedPercent { get { return ComputePercent(GameLimitsCitizensUsed, GameLimitsCitizensCapacity); } }
        public int GameLimitsCitizensUsed;
        public int GameLimitsCitizensCapacity;
        public float GameLimitsCitizenUnitsUsedPercent { get { return ComputePercent(GameLimitsCitizenUnitsUsed, GameLimitsCitizenUnitsCapacity); } }
        public int GameLimitsCitizenUnitsUsed;
        public int GameLimitsCitizenUnitsCapacity;
        public float GameLimitsCitizenInstancesUsedPercent { get { return ComputePercent(GameLimitsCitizenInstancesUsed, GameLimitsCitizenInstancesCapacity); } }
        public int GameLimitsCitizenInstancesUsed;
        public int GameLimitsCitizenInstancesCapacity;
        public float? GameLimitsDisastersUsedPercent { get { return ComputePercent(GameLimitsDisastersUsed, GameLimitsDisastersCapacity); } }
        public int? GameLimitsDisastersUsed;
        public int? GameLimitsDisastersCapacity;
        public float GameLimitsDistrictsUsedPercent { get { return ComputePercent(GameLimitsDistrictsUsed, GameLimitsDistrictsCapacity); } }
        public int GameLimitsDistrictsUsed;
        public int GameLimitsDistrictsCapacity;
        public float GameLimitsEventsUsedPercent { get { return ComputePercent(GameLimitsEventsUsed, GameLimitsEventsCapacity); } }
        public int GameLimitsEventsUsed;
        public int GameLimitsEventsCapacity;
        public float GameLimitsGameAreasUsedPercent { get { return ComputePercent(GameLimitsGameAreasUsed, GameLimitsGameAreasCapacity); } }
        public int GameLimitsGameAreasUsed;
        public int GameLimitsGameAreasCapacity;
        public float GameLimitsNetworkLanesUsedPercent { get { return ComputePercent(GameLimitsNetworkLanesUsed, GameLimitsNetworkLanesCapacity); } }
        public int GameLimitsNetworkLanesUsed;
        public int GameLimitsNetworkLanesCapacity;
        public float GameLimitsNetworkNodesUsedPercent { get { return ComputePercent(GameLimitsNetworkNodesUsed, GameLimitsNetworkNodesCapacity); } }
        public int GameLimitsNetworkNodesUsed;
        public int GameLimitsNetworkNodesCapacity;
        public float GameLimitsNetworkSegmentsUsedPercent { get { return ComputePercent(GameLimitsNetworkSegmentsUsed, GameLimitsNetworkSegmentsCapacity); } }
        public int GameLimitsNetworkSegmentsUsed;
        public int GameLimitsNetworkSegmentsCapacity;
        public float? GameLimitsPaintedAreasUsedPercent { get { return ComputePercent(GameLimitsPaintedAreasUsed, GameLimitsPaintedAreasCapacity); } }
        public int? GameLimitsPaintedAreasUsed;
        public int? GameLimitsPaintedAreasCapacity;
        public float GameLimitsPathUnitsUsedPercent { get { return ComputePercent(GameLimitsPathUnitsUsed, GameLimitsPathUnitsCapacity); } }
        public int GameLimitsPathUnitsUsed;
        public int GameLimitsPathUnitsCapacity;
        public float GameLimitsPropsUsedPercent { get { return ComputePercent(GameLimitsPropsUsed, GameLimitsPropsCapacity); } }
        public int GameLimitsPropsUsed;
        public int GameLimitsPropsCapacity;
        public float GameLimitsRadioChannelsUsedPercent { get { return ComputePercent(GameLimitsRadioChannelsUsed, GameLimitsRadioChannelsCapacity); } }
        public int GameLimitsRadioChannelsUsed;
        public int GameLimitsRadioChannelsCapacity;
        public float GameLimitsRadioContentsUsedPercent { get { return ComputePercent(GameLimitsRadioContentsUsed, GameLimitsRadioContentsCapacity); } }
        public int GameLimitsRadioContentsUsed;
        public int GameLimitsRadioContentsCapacity;
        public float GameLimitsTransportLinesUsedPercent { get { return ComputePercent(GameLimitsTransportLinesUsed, GameLimitsTransportLinesCapacity); } }
        public int GameLimitsTransportLinesUsed;
        public int GameLimitsTransportLinesCapacity;
        public float GameLimitsTreesUsedPercent { get { return ComputePercent(GameLimitsTreesUsed, GameLimitsTreesCapacity); } }
        public int GameLimitsTreesUsed;
        public int GameLimitsTreesCapacity;
        public float GameLimitsVehiclesUsedPercent { get { return ComputePercent(GameLimitsVehiclesUsed, GameLimitsVehiclesCapacity); } }
        public int GameLimitsVehiclesUsed;
        public int GameLimitsVehiclesCapacity;
        public float GameLimitsVehiclesParkedUsedPercent { get { return ComputePercent(GameLimitsVehiclesParkedUsed, GameLimitsVehiclesParkedCapacity); } }
        public int GameLimitsVehiclesParkedUsed;
        public int GameLimitsVehiclesParkedCapacity;
        public float GameLimitsZoneBlocksUsedPercent { get { return ComputePercent(GameLimitsZoneBlocksUsed, GameLimitsZoneBlocksCapacity); } }
        public int GameLimitsZoneBlocksUsed;
        public int GameLimitsZoneBlocksCapacity;


        #endregion

        /// <summary>
        /// constructor to initialize with snapshot date
        /// </summary>
        public Snapshot(DateTime snapshotDate)
        {
            SnapshotDate = snapshotDate;
        }

        /// <summary>
        /// compare snapshots by comparing their dates
        /// </summary>
        public int CompareTo(Snapshot snapshot)
        {
            if (snapshot.SnapshotDate.Year == SnapshotDate.Year && snapshot.SnapshotDate.Month == SnapshotDate.Month &&
                snapshot.SnapshotDate.Day == SnapshotDate.Day && snapshot.SnapshotDate.Hour == SnapshotDate.Hour &&
                snapshot.SnapshotDate.Minute == SnapshotDate.Minute)
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// get the snapshot field or property that corresponds to the statistic type
        /// this is why the snapshot field/property names must exactly match the StatisticType enum names
        /// </summary>
        public static void GetFieldProperty(Statistic.StatisticType statisticType, out FieldInfo field, out PropertyInfo property)
        {
            // try to get as a field
            field = typeof(Snapshot).GetField(statisticType.ToString(), BindingFlags.Public | BindingFlags.Instance);

            // try to get as a property
            property = typeof(Snapshot).GetProperty(statisticType.ToString(), BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// compute a percent from ints
        /// </summary>
        private float ComputePercent(int value, int total)
        {
            // check for divide by zero
            if (total == 0)
            {
                return 0f;
            }

            // normal percent calculation
            return (float)(100d * value / total);
        }

        /// <summary>
        /// compute a percent from nullable ints
        /// </summary>
        private float? ComputePercent(int? value, int? total)
        {
            // check for nulls
            if (!value.HasValue || !total.HasValue)
            {
                return null;
            }

            // check for divide by zero
            if (total == 0)
            {
                return 0f;
            }

            // normal percent calculation
            return (float?)(100d * value / total);
        }

        /// <summary>
        /// compute a percent from longs
        /// </summary>
        private float ComputePercent(long value, long total)
        {
            // check for divide by zero
            if (total == 0)
            {
                return 0f;
            }

            // normal percent calculation
            return (float)(100d * value / total);
        }

        /// <summary>
        /// compute a percent from a nullable long value
        /// </summary>
        private float? ComputePercent(long? value, long total)
        {
            // check for null
            if (!value.HasValue)
            {
                return null;
            }

            // check for divide by zero
            if (total == 0)
            {
                return 0f;
            }

            // normal percent calculation
            return (float?)(100d * value / total);
        }

        /// <summary>
        /// compute a percent from uints
        /// </summary>
        private float ComputePercent(uint value, uint total)
        {
            // check for divide by zero
            if (total == 0)
            {
                return 0f;
            }

            // normal percent calculation
            return (float)(100d * value / total);
        }

        /// <summary>
        /// compute a per capita value from a nullable long value
        /// </summary>
        private float? ComputePerCapita(long? value)
        {
            // check for null
            if (!value.HasValue)
            {
                return null;
            }

            // check for divide by zero
            if (PopulationTotal == 0)
            {
                return 0f;
            }

            // normal per capita calculation
            return (float?)((double)value / PopulationTotal);
        }

        /// <summary>
        /// compute the zone level weighted average from the individual zone level percents
        /// </summary>
        private float ComputeZoneLevelAverage(byte level1, byte level2, byte level3, byte level4 = 0, byte level5 = 0)
        {
            // accumulate totals
            float weightedTotal = 0f;
            int totalValue = 0;
            weightedTotal += 1f * level1; totalValue += level1;
            weightedTotal += 2f * level2; totalValue += level2;
            weightedTotal += 3f * level3; totalValue += level3;
            weightedTotal += 4f * level4; totalValue += level4;
            weightedTotal += 5f * level5; totalValue += level5;

            // return weighted average
            if (totalValue > 0)
            {
                return weightedTotal / totalValue;
            }
            else
            {
                return 0f;
            }
        }

        /// <summary>
        /// the demographic data for one citizen
        /// </summary>
        private class CitizenDemographic
        {
            public uint citizenID;
            public byte districtID;
            public bool deceasesd;
            public bool movingIn;
            //public int age;        // real age, not game age
            public Citizen.AgeGroup ageGroup;
            public Citizen.Education education;
            public Citizen.Gender gender;
            public Citizen.Happiness happiness;
            public Citizen.Health health;
            public Citizen.Location location;
            public ItemClass.Level residential;
            public ItemClass.Level student;    // None (i.e. -1) = not a student, Levels 1-3 (i.e. 0-2) = Elementary, High School, University
            public Citizen.Wealth wealth;
            public Citizen.Wellbeing wellbeing;
        }

        private class CitizenDemographics : List<CitizenDemographic> { }

        // the demographics buffers
        private static CitizenDemographics _citizens;

        /// <summary>
        /// get the demographic data for the specified citizen
        /// return null for citizen that should not be included
        /// </summary>
        private static CitizenDemographic GetCitizenDemographicData(uint citizenID)
        {
            // citizen must be created and must not be a tourist
            Citizen citizen = CitizenManager.instance.m_citizens.m_buffer[citizenID];
            
            if ((citizen.m_flags & Citizen.Flags.Created) != 0 && (citizen.m_flags & Citizen.Flags.Tourist) == 0)
            {
                // citizen must have a home building
                if (citizen.m_homeBuilding != 0)
                {
                    // home building must have an AI
                    Building homeBuilding = BuildingManager.instance.m_buildings.m_buffer[citizen.m_homeBuilding];
                    if (homeBuilding.Info != null && homeBuilding.Info.m_buildingAI != null)
                    {
                        // loop over type hierarchy of home building
                        Type homeBuildingAIType = homeBuilding.Info.m_buildingAI.GetType();
                        while (homeBuildingAIType != null)
                        {
                            // building AI must be or derive from ResidentialBuildingAI   OR   building AI must be or derive from NursingHomeAi (mod)
                            // PloppableRICO.GrowableResidentialAI and PloppableRICO.PloppableResidentialAI derive from ResidentialBuildingAI
                            if (homeBuildingAIType == typeof(ResidentialBuildingAI) || homeBuildingAIType.Name == "NursingHomeAi")
                            {
                                // set citizen demographic data
                                CitizenDemographic citizenDemographic = new CitizenDemographic();
                                citizenDemographic.citizenID = citizenID;
                                citizenDemographic.districtID = DistrictManager.instance.GetDistrict(homeBuilding.m_position);
                                citizenDemographic.deceasesd = ((citizen.m_flags & Citizen.Flags.Dead) != 0);
                                citizenDemographic.movingIn = ((citizen.m_flags & Citizen.Flags.MovingIn) != 0);
                                citizenDemographic.ageGroup = Citizen.GetAgeGroup(citizen.Age);
                                citizenDemographic.education = citizen.EducationLevel;
                                citizenDemographic.gender = Citizen.GetGender(citizenID);
                                citizenDemographic.happiness = Citizen.GetHappinessLevel(Citizen.GetHappiness(citizen.m_health, citizen.m_wellbeing));
                                citizenDemographic.health = Citizen.GetHealthLevel(citizen.m_health);
                                citizenDemographic.location = citizen.CurrentLocation;
                                citizenDemographic.residential = homeBuilding.Info.m_class.m_level;
                                citizenDemographic.student = citizen.GetCurrentSchoolLevel(citizenID);
                                citizenDemographic.wealth = citizen.WealthLevel;
                                citizenDemographic.wellbeing = Citizen.GetWellbeingLevel(citizen.EducationLevel, citizen.m_wellbeing);

                                // return citizen demographic data
                                return citizenDemographic;
                            }

                            // continue with base building AI type
                            homeBuildingAIType = homeBuildingAIType.BaseType;
                        }
                    }
                }
            }

            // citizen should not be included
            return null;
        }


        /// <summary>
        /// return a snapshot of current statistics
        /// </summary>
        public static Snapshot TakeSnapshot(DateTime gameDate, bool logDate = true)
        {
            // create a new snapshot to return
            // set the snapshot date to the current game date
            // the game date without time is used even if the Real Time mod is enabled and the snapshot is taken at noon
            // a snapshot is for the date, regardless of the time on that date when the snapshot is actually taken
            Snapshot snapshot = new Snapshot(gameDate);
            if (logDate)
            {
                LogUtil.LogInfo($"Taking snapshot for [{snapshot.SnapshotDate.ToString("yyyy/MM/dd HH:mm")}].");
            }

            // proceed only if all the required managers exist
            if (!AudioManager.exists ||
                !BuildingManager.exists ||
                !CitizenManager.exists ||
                !DisasterManager.exists ||
                !DistrictManager.exists ||
                !EconomyManager.exists ||
                !EventManager.exists ||
                !GameAreaManager.exists ||
                !ImmaterialResourceManager.exists ||
                !NaturalResourceManager.exists ||
                !NetManager.exists ||
                !PathManager.exists ||
                !PropManager.exists ||
                !TransportManager.exists ||
                !TreeManager.exists ||
                !VehicleManager.exists ||
                !ZoneManager.exists)
            {
                return snapshot;
            }

            // get manager instances
            AudioManager audioManagerInstance = AudioManager.instance;
            BuildingManager buildingManagerInstance = BuildingManager.instance;
            CitizenManager citizenManagerInstance = CitizenManager.instance;
            DisasterManager disasterManagerInstance = DisasterManager.instance;
            DistrictManager districtManagerInstance = DistrictManager.instance;
            EconomyManager economyManagerInstance = EconomyManager.instance;
            EventManager eventManagerInstance = EventManager.instance;
            GameAreaManager gameAreaManagerInstance = GameAreaManager.instance;
            ImmaterialResourceManager immaterialResourceManagerInstance = ImmaterialResourceManager.instance;
            NaturalResourceManager naturalResourceManagerInstance = NaturalResourceManager.instance;
            NetManager netManagerInstance = NetManager.instance;
            PathManager pathManagerInstance = PathManager.instance;
            PropManager propManagerInstance = PropManager.instance;
            TransportManager transportManagerInstance = TransportManager.instance;
            TreeManager treeManagerInstance = TreeManager.instance;
            VehicleManager vehicleManagerInstance = VehicleManager.instance;
            ZoneManager zoneManagerInstance = ZoneManager.instance;

            // get DLC flags
            // there are no dependencies on Match Day (aka Football) and Concerts (aka MusicFestival) DLCs
            bool dlcAfterDark = SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);            // 09/24/15
            bool dlcSnowfall = SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC);             // 02/18/16
            bool dlcNaturalDisasters = SteamHelper.IsDLCOwned(SteamHelper.DLC.NaturalDisastersDLC);     // 11/29/16
            bool dlcMassTransit = SteamHelper.IsDLCOwned(SteamHelper.DLC.InMotionDLC);             // 05/18/17
            bool dlcGreenCities = SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);          // 10/19/17
            bool dlcParkLife = SteamHelper.IsDLCOwned(SteamHelper.DLC.ParksDLC);                // 05/24/18
            bool dlcIndustries = SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC);             // 10/23/18
            bool dlcCampus = SteamHelper.IsDLCOwned(SteamHelper.DLC.CampusDLC);               // 05/21/19
            bool dlcSunsetHarbor = SteamHelper.IsDLCOwned(SteamHelper.DLC.UrbanDLC);                // 03/26/20
            bool dlcAirports = SteamHelper.IsDLCOwned(SteamHelper.DLC.AirportDLC);              // 01/25/22

            //Location
            // initialize citizen demographics

            _citizens = new CitizenDemographics();
            if (CitizenManager.exists && BuildingManager.exists && DistrictManager.exists)
            {
                for (uint citizenID = 0; citizenID < CitizenManager.instance.m_citizens.m_buffer.Length; citizenID++)
                {
                    CitizenDemographic citizenDemographic = GetCitizenDemographicData(citizenID);
                    if (citizenDemographic != null)
                    {
                        _citizens.Add(citizenDemographic);

                        if (citizenDemographic.location == Citizen.Location.Home)
                        {
                            snapshot.LocationHome += 1;
                        }
                        if (citizenDemographic.location == Citizen.Location.Work)
                        {
                            snapshot.LocationWork += 1;
                        }
                        if (citizenDemographic.location == Citizen.Location.Visit)
                        {
                            snapshot.LocationVisit += 1;
                        }
                        if (citizenDemographic.location == Citizen.Location.Moving)
                        {
                            snapshot.LocationMoving += 1;
                        }
                    }
                }

                //for (uint citizenID = 0; citizenID < CitizenManager.instance.m_instances.m_buffer.Length; citizenID++)
                //{
                //
                //    CitizenInstance citizenInstance = CitizenManager.instance.m_instances.m_buffer[citizenID];
                //
                //    if (citizenInstance.m_flags == CitizenInstance.Flags.OnBikeLane ||
                //        citizenInstance.m_flags == CitizenInstance.Flags.RidingBicycle)
                //    {
                //        snapshot.CitizenOnBike += 1;
                //    }
                //    if (citizenInstance.m_flags == CitizenInstance.Flags.WaitingTransport)
                //    {
                //        snapshot.CitizenWaitingTransport += 1;
                //    }
                //    if (citizenInstance.m_flags == CitizenInstance.Flags.BoredOfWaiting)
                //    {
                //        snapshot.CitizenBored += 1;
                //    }
                //}
            }

            // get the city-wide district where much of the data is obtained
            District cityDistrict = districtManagerInstance.m_districts.m_buffer[0];

            // Water - logic copied from WaterInfoViewPanel.UpdatePanel
            snapshot.WaterConsumption     = cityDistrict.GetWaterConsumption();
            snapshot.WaterPumpingCapacity = cityDistrict.GetWaterCapacity();

            // Water Tank - logic copied from WaterInfoViewPanel.UpdatePanel
            if (dlcNaturalDisasters)
            {
                snapshot.WaterTankReserved        = cityDistrict.GetWaterStorageAmount();
                snapshot.WaterTankStorageCapacity = cityDistrict.GetWaterStorageCapacity();
            }

            // Sewage - logic copied from WaterInfoViewPanel.UpdatePanel
            snapshot.SewageProduction       = cityDistrict.GetSewageAccumulation();
            snapshot.SewageDrainingCapacity = cityDistrict.GetSewageCapacity();

            // Landfill - logic copied from GarbageInfoViewPanel.UpdatePanel
            snapshot.LandfillStorage  = cityDistrict.GetGarbageAmount();
            snapshot.LandfillCapacity = cityDistrict.GetGarbageCapacity();

            // Garbage - logic copied from GarbageInfoViewPanel.UpdatePanel
            snapshot.GarbageProduction         = cityDistrict.GetGarbageAccumulation();
            snapshot.GarbageProcessingCapacity = cityDistrict.GetIncinerationCapacity();

            // Education - logic copied from EducationInfoViewPanel.UpdatePanel
            // University eligible and capacity include Campus Areas
            snapshot.EducationElementaryEligible = cityDistrict.GetEducation1Need();
            snapshot.EducationElementaryCapacity = cityDistrict.GetEducation1Capacity();
            snapshot.EducationHighSchoolEligible = cityDistrict.GetEducation2Need();
            snapshot.EducationHighSchoolCapacity = cityDistrict.GetEducation2Capacity();
            snapshot.EducationUniversityEligible = cityDistrict.GetEducation3Need();
            snapshot.EducationUniversityCapacity = cityDistrict.GetEducation3Capacity();
            snapshot.EducationLibraryUsers       = cityDistrict.GetLibraryVisitorCount();
            snapshot.EducationLibraryCapacity    = cityDistrict.GetLibraryCapacity();

            // Education Level - logic copied from EducationInfoViewPanel.UpdatePanel
            snapshot.EducationLevelUneducated     = cityDistrict.m_educated0Data.m_finalCount;
            snapshot.EducationLevelEducated       = cityDistrict.m_educated1Data.m_finalCount;
            snapshot.EducationLevelWellEducated   = cityDistrict.m_educated2Data.m_finalCount;
            snapshot.EducationLevelHighlyEducated = cityDistrict.m_educated3Data.m_finalCount;

            // Happiness - logic copied from HappinessInfoViewPanel.UpdatePanel
            snapshot.HappinessGlobal      = cityDistrict.m_finalHappiness;
            snapshot.HappinessResidential = cityDistrict.m_residentialData.m_finalHappiness;
            snapshot.HappinessCommercial  = cityDistrict.m_commercialData.m_finalHappiness;
            snapshot.HappinessIndustrial  = cityDistrict.m_industrialData.m_finalHappiness;
            snapshot.HappinessOffice      = cityDistrict.m_officeData.m_finalHappiness;

            // Healthcare - logic copied from HealthInfoViewPanel.UpdatePanel
            snapshot.HealthcareAverageHealth = cityDistrict.m_residentialData.m_finalHealth;
            snapshot.HealthcareSick          = cityDistrict.GetSickCount();
            snapshot.HealthcareHealCapacity  = cityDistrict.GetHealCapacity();

            // Deathcare - logic copied from HealthInfoViewPanel.UpdatePanel
            snapshot.DeathcareCemeteryBuried      = cityDistrict.GetDeadAmount();
            snapshot.DeathcareCemeteryCapacity    = cityDistrict.GetDeadCapacity();
            snapshot.DeathcareCrematoriumDeceased = cityDistrict.GetDeadCount();
            snapshot.DeathcareCrematoriumCapacity = cityDistrict.GetCremateCapacity();
            snapshot.DeathcareDeathRate           = cityDistrict.m_deathData.m_finalCount;

            // Childcare - logic copied from HealthInfoViewPanel.UpdatePanel
            snapshot.ChildcareAverageHealth = (byte)(cityDistrict.m_childData.m_finalCount + cityDistrict.m_teenData.m_finalCount == 0 ? 0 : cityDistrict.m_childHealthData.m_finalCount / (cityDistrict.m_childData.m_finalCount + cityDistrict.m_teenData.m_finalCount));
            snapshot.ChildcareSick          = cityDistrict.m_childSickData.m_finalCount;
            snapshot.ChildcareBirthRate     = cityDistrict.m_birthData.m_finalCount;

            // Eldercare - logic copied from HealthInfoViewPanel.UpdatePanel
            snapshot.EldercareAverageHealth   = (byte)(cityDistrict.m_seniorData.m_finalCount == 0 ? 0 : cityDistrict.m_seniorHealthData.m_finalCount / cityDistrict.m_seniorData.m_finalCount);
            snapshot.EldercareSick            = cityDistrict.m_seniorSickData.m_finalCount;
            snapshot.EldercareAverageLifeSpan = cityDistrict.GetAverageLifespan();

            // Zoning
            GetZoning(out snapshot.ZoningResidential, out snapshot.ZoningCommercial, out snapshot.ZoningIndustrial, out snapshot.ZoningOffice, out snapshot.ZoningUnzoned);

            // Zone Level - logic copied from LevelsInfoViewPanel.UpdatePanel and DistrictPrivateData.GetLevelPercentages
            snapshot.ZoneLevelResidential1 = cityDistrict.m_residentialData.m_finalLevel1;
            snapshot.ZoneLevelResidential2 = cityDistrict.m_residentialData.m_finalLevel2;
            snapshot.ZoneLevelResidential3 = cityDistrict.m_residentialData.m_finalLevel3;
            snapshot.ZoneLevelResidential4 = cityDistrict.m_residentialData.m_finalLevel4;
            snapshot.ZoneLevelResidential5 = cityDistrict.m_residentialData.m_finalLevel5;
            snapshot.ZoneLevelCommercial1  = cityDistrict.m_commercialData.m_finalLevel1;
            snapshot.ZoneLevelCommercial2  = cityDistrict.m_commercialData.m_finalLevel2;
            snapshot.ZoneLevelCommercial3  = cityDistrict.m_commercialData.m_finalLevel3;
            snapshot.ZoneLevelIndustrial1  = cityDistrict.m_industrialData.m_finalLevel1;
            snapshot.ZoneLevelIndustrial2  = cityDistrict.m_industrialData.m_finalLevel2;
            snapshot.ZoneLevelIndustrial3  = cityDistrict.m_industrialData.m_finalLevel3;
            snapshot.ZoneLevelOffice1      = cityDistrict.m_officeData.m_finalLevel1;
            snapshot.ZoneLevelOffice2      = cityDistrict.m_officeData.m_finalLevel2;
            snapshot.ZoneLevelOffice3      = cityDistrict.m_officeData.m_finalLevel3;

            // Zone Buildings - logic copied from CityInfoPanel.LateUpdate
            snapshot.ZoneBuildingsResidential = cityDistrict.m_residentialData.m_finalHomeOrWorkCount;  // households available
            snapshot.ZoneBuildingsCommercial  = cityDistrict.m_commercialData.m_finalHomeOrWorkCount;   // jobs available
            snapshot.ZoneBuildingsIndustrial  = cityDistrict.m_industrialData.m_finalHomeOrWorkCount;   // jobs available
            snapshot.ZoneBuildingsOffice      = cityDistrict.m_officeData.m_finalHomeOrWorkCount;       // jobs available

            // Zone Demand - logic copied from InfoPanel.Update
            snapshot.ZoneDemandResidential      = zoneManagerInstance.m_residentialDemand;
            snapshot.ZoneDemandCommercial       = zoneManagerInstance.m_commercialDemand;
            snapshot.ZoneDemandIndustrialOffice = zoneManagerInstance.m_workplaceDemand;

            // Traffic - logic copied from TrafficInfoViewPanel.UpdatePanel
            snapshot.TrafficAverageFlow = vehicleManagerInstance.m_lastTrafficFlow;

            // Pollution - logic copied from PollutionInfoViewPanel.UpdatePanel and NoisePollutionInfoViewPanel.UpdatePanel
            snapshot.PollutionAverageGround        = cityDistrict.GetGroundPollution();
            snapshot.PollutionAverageDrinkingWater = cityDistrict.GetWaterPollution();
            immaterialResourceManagerInstance.CheckTotalResource(ImmaterialResourceManager.Resource.NoisePollution, out int pollutionAverageNoise);
            snapshot.PollutionAverageNoise         = pollutionAverageNoise;

            // Fire Safety - logic copied from FireSafetyInfoViewPanel.UpdatePanel
            immaterialResourceManagerInstance.CheckTotalResource(ImmaterialResourceManager.Resource.FireHazard, out int fireSafetyHazard);
            snapshot.FireSafetyHazard = Mathf.Clamp(fireSafetyHazard, 0, 100);

            // Crime - logic copied from CrimeInfoViewPanel.UpdatePanel
            snapshot.CrimeRate              = cityDistrict.m_finalCrimeRate;
            snapshot.CrimeDetainedCriminals = cityDistrict.GetCriminalAmount();
            snapshot.CrimeJailsCapacity     = cityDistrict.GetCriminalCapacity();

            // Public Transportation - logic copied from TransportInfoViewPanel.UpdatePanel
            TransportPassengerData[] passengers = transportManagerInstance.m_passengers;
                                 snapshot.PublicTransportationBusResidents        = passengers[(int)TransportInfo.TransportType.Bus       ].m_residentPassengers.m_averageCount;
                                 snapshot.PublicTransportationBusTourists         = passengers[(int)TransportInfo.TransportType.Bus       ].m_touristPassengers.m_averageCount;
            if (dlcSunsetHarbor) snapshot.PublicTransportationTrolleybusResidents = passengers[(int)TransportInfo.TransportType.Trolleybus].m_residentPassengers.m_averageCount;
            if (dlcSunsetHarbor) snapshot.PublicTransportationTrolleybusTourists  = passengers[(int)TransportInfo.TransportType.Trolleybus].m_touristPassengers.m_averageCount;
            if (dlcSnowfall    ) snapshot.PublicTransportationTramResidents       = passengers[(int)TransportInfo.TransportType.Tram      ].m_residentPassengers.m_averageCount;
            if (dlcSnowfall    ) snapshot.PublicTransportationTramTourists        = passengers[(int)TransportInfo.TransportType.Tram      ].m_touristPassengers.m_averageCount;
                                 snapshot.PublicTransportationMetroResidents      = passengers[(int)TransportInfo.TransportType.Metro     ].m_residentPassengers.m_averageCount;
                                 snapshot.PublicTransportationMetroTourists       = passengers[(int)TransportInfo.TransportType.Metro     ].m_touristPassengers.m_averageCount;
                                 snapshot.PublicTransportationTrainResidents      = passengers[(int)TransportInfo.TransportType.Train     ].m_residentPassengers.m_averageCount;
                                 snapshot.PublicTransportationTrainTourists       = passengers[(int)TransportInfo.TransportType.Train     ].m_touristPassengers.m_averageCount;
                                 snapshot.PublicTransportationShipResidents       = passengers[(int)TransportInfo.TransportType.Ship      ].m_residentPassengers.m_averageCount;
                                 snapshot.PublicTransportationShipTourists        = passengers[(int)TransportInfo.TransportType.Ship      ].m_touristPassengers.m_averageCount;
                                 snapshot.PublicTransportationAirResidents        = passengers[(int)TransportInfo.TransportType.Airplane  ].m_residentPassengers.m_averageCount +
                                                                                    passengers[(int)TransportInfo.TransportType.Helicopter].m_residentPassengers.m_averageCount;
                                 snapshot.PublicTransportationAirTourists         = passengers[(int)TransportInfo.TransportType.Airplane  ].m_touristPassengers.m_averageCount +
                                                                                    passengers[(int)TransportInfo.TransportType.Helicopter].m_touristPassengers.m_averageCount;
            if (dlcMassTransit)  snapshot.PublicTransportationMonorailResidents   = passengers[(int)TransportInfo.TransportType.Monorail  ].m_residentPassengers.m_averageCount;
            if (dlcMassTransit)  snapshot.PublicTransportationMonorailTourists    = passengers[(int)TransportInfo.TransportType.Monorail  ].m_touristPassengers.m_averageCount;
            if (dlcMassTransit)  snapshot.PublicTransportationCableCarResidents   = passengers[(int)TransportInfo.TransportType.CableCar  ].m_residentPassengers.m_averageCount;
            if (dlcMassTransit)  snapshot.PublicTransportationCableCarTourists    = passengers[(int)TransportInfo.TransportType.CableCar  ].m_touristPassengers.m_averageCount;
            if (dlcAfterDark  )  snapshot.PublicTransportationTaxiResidents       = passengers[(int)TransportInfo.TransportType.Taxi      ].m_residentPassengers.m_averageCount;
            if (dlcAfterDark  )  snapshot.PublicTransportationTaxiTourists        = passengers[(int)TransportInfo.TransportType.Taxi      ].m_touristPassengers.m_averageCount;

            // Population - logic copied from CityInfoPanel.LateUpdate
            snapshot.PopulationChildren    = cityDistrict.m_childData.m_finalCount;
            snapshot.PopulationTeens       = cityDistrict.m_teenData.m_finalCount;
            snapshot.PopulationYoungAdults = cityDistrict.m_youngData.m_finalCount;
            snapshot.PopulationAdults      = cityDistrict.m_adultData.m_finalCount;
            snapshot.PopulationSeniors     = cityDistrict.m_seniorData.m_finalCount;

            // Households - logic copied from CityInfoPanel.LateUpdate
            snapshot.HouseholdsOccupied  = cityDistrict.m_residentialData.m_finalAliveCount;
            snapshot.HouseholdsAvailable = cityDistrict.m_residentialData.m_finalHomeOrWorkCount;

            // Employment - logic copied from PopulationInfoViewPanel.UpdatePanel and from District.GetUnemployment
            snapshot.EmploymentPeopleEmployed  = cityDistrict.GetWorkerCount();
            snapshot.EmploymentJobsAvailable   = cityDistrict.GetWorkplaceCount();
            snapshot.EmploymentUnemployed      = cityDistrict.m_educated0Data.m_finalUnemployed +
                                                 cityDistrict.m_educated1Data.m_finalUnemployed +
                                                 cityDistrict.m_educated2Data.m_finalUnemployed +
                                                 cityDistrict.m_educated3Data.m_finalUnemployed;
            snapshot.EmploymentEligibleWorkers = cityDistrict.m_youngData.m_finalCount + cityDistrict.m_adultData.m_finalCount;

            // Outside Connections - logic copied from OutsideConnectionsInfoViewPanel.UpdatePanel
                                 snapshot.OutsideConnectionsImportGoods    = (int)(cityDistrict.m_importData.m_averageGoods        + 99) / 100;
                                 snapshot.OutsideConnectionsImportForestry = (int)(cityDistrict.m_importData.m_averageForestry     + 99) / 100;
                                 snapshot.OutsideConnectionsImportFarming  = (int)(cityDistrict.m_importData.m_averageAgricultural + 99) / 100;
                                 snapshot.OutsideConnectionsImportOre      = (int)(cityDistrict.m_importData.m_averageOre          + 99) / 100;
                                 snapshot.OutsideConnectionsImportOil      = (int)(cityDistrict.m_importData.m_averageOil          + 99) / 100;
            if (dlcIndustries  ) snapshot.OutsideConnectionsImportMail     = (int)(cityDistrict.m_importData.m_averageMail         + 99) / 100;
                                 snapshot.OutsideConnectionsExportGoods    = (int)(cityDistrict.m_exportData.m_averageGoods        + 99) / 100;
                                 snapshot.OutsideConnectionsExportForestry = (int)(cityDistrict.m_exportData.m_averageForestry     + 99) / 100;
                                 snapshot.OutsideConnectionsExportFarming  = (int)(cityDistrict.m_exportData.m_averageAgricultural + 99) / 100;
                                 snapshot.OutsideConnectionsExportOre      = (int)(cityDistrict.m_exportData.m_averageOre          + 99) / 100;
                                 snapshot.OutsideConnectionsExportOil      = (int)(cityDistrict.m_exportData.m_averageOil          + 99) / 100;
            if (dlcIndustries  ) snapshot.OutsideConnectionsExportMail     = (int)(cityDistrict.m_exportData.m_averageMail         + 99) / 100;
            if (dlcSunsetHarbor) snapshot.OutsideConnectionsExportFish     = (int)(cityDistrict.m_exportData.m_averageFish         + 99) / 100;

            // Land Value - logic copied from LandValueInfoViewPanel.UpdatePanel
            snapshot.LandValueAverage = cityDistrict.GetLandValue();

            // Natural Resources - logic copied from NaturalResourcesInfoViewPanel.UpdatePanel
            naturalResourceManagerInstance.CalculateUsedResources(out uint usedOre, out uint usedOil, out uint usedForest, out uint usedFertility);
            naturalResourceManagerInstance.CalculateUnlockedResources(out uint availableOre, out uint availableOil, out uint availableForest, out uint availableFertility, out uint _);
            const float ForestMultiplier = 0.1116728f;
            const float FertilityMultiplier1 = 0.000446691178f;
            const float FertilityMultiplier2 = 0.00399999972f;
            availableForest = (uint)Mathf.CeilToInt(availableForest * ForestMultiplier);
            usedForest = Math.Min(availableForest, usedForest);
            availableFertility = (uint)Mathf.CeilToInt(availableFertility * FertilityMultiplier1);
            usedFertility = (uint)Mathf.CeilToInt(usedFertility * FertilityMultiplier2);
            usedFertility = Math.Min(availableFertility, usedFertility);
            snapshot.NaturalResourcesForestUsed           = usedForest;
            snapshot.NaturalResourcesForestAvailable      = availableForest;
            snapshot.NaturalResourcesFertileLandUsed      = usedFertility;
            snapshot.NaturalResourcesFertileLandAvailable = availableFertility;
            snapshot.NaturalResourcesOreUsed              = usedOre;
            snapshot.NaturalResourcesOreAvailable         = availableOre;
            snapshot.NaturalResourcesOilUsed              = usedOil;
            snapshot.NaturalResourcesOilAvailable         = availableOil;

            // Heating - logic copied from HeatingInfoViewPanel.UpdatePanel
            if (dlcSnowfall)
            {
                snapshot.HeatingConsumption = cityDistrict.GetHeatingConsumption() / 1000;
                snapshot.HeatingProduction  = cityDistrict.GetHeatingCapacity()    / 1000;
            }

            // Tourism - logic copied from TourismInfoViewPanel.UpdatePanel
                           snapshot.TourismCityAttractiveness   = immaterialResourceManagerInstance.CheckActualTourismResource();
                           snapshot.TourismLowWealth            = cityDistrict.m_tourist1Data.m_averageCount;
                           snapshot.TourismMediumWealth         = cityDistrict.m_tourist2Data.m_averageCount;
                           snapshot.TourismHighWealth           = cityDistrict.m_tourist3Data.m_averageCount;
            if (dlcCampus) snapshot.TourismExchangeStudentBonus = immaterialResourceManagerInstance.CheckExchangeStudentAttractivenessBonus() * 100f;

            // Tours - logic copied from ToursInfoViewPanel.UpdatePanel
            if (dlcParkLife)
            {
                snapshot.ToursWalkingTourResidents = passengers[(int)TransportInfo.TransportType.Pedestrian   ].m_residentPassengers.m_averageCount;
                snapshot.ToursWalkingTourTourists  = passengers[(int)TransportInfo.TransportType.Pedestrian   ].m_touristPassengers.m_averageCount;
                snapshot.ToursSightseeingResidents = passengers[(int)TransportInfo.TransportType.TouristBus   ].m_residentPassengers.m_averageCount;
                snapshot.ToursSightseeingTourists  = passengers[(int)TransportInfo.TransportType.TouristBus   ].m_touristPassengers.m_averageCount;
                snapshot.ToursBalloonResidents     = passengers[(int)TransportInfo.TransportType.HotAirBalloon].m_residentPassengers.m_averageCount;
                snapshot.ToursBalloonToursits      = passengers[(int)TransportInfo.TransportType.HotAirBalloon].m_touristPassengers.m_averageCount;
            }

            // Tax Rate - logic copied from EconomyPanel.GetTaxRate, which is called from TaxesItem.Init, which is called from EconomyPanel.PopulateTaxesTab
            snapshot.TaxRateResidentialLow  = economyManagerInstance.GetTaxRate(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,  ItemClass.Level.None);
            snapshot.TaxRateResidentialHigh = economyManagerInstance.GetTaxRate(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh, ItemClass.Level.None);
            snapshot.TaxRateCommercialLow   = economyManagerInstance.GetTaxRate(ItemClass.Service.Commercial,  ItemClass.SubService.CommercialLow,   ItemClass.Level.None);
            snapshot.TaxRateCommercialHigh  = economyManagerInstance.GetTaxRate(ItemClass.Service.Commercial,  ItemClass.SubService.CommercialHigh,  ItemClass.Level.None);
            snapshot.TaxRateIndustrial      = economyManagerInstance.GetTaxRate(ItemClass.Service.Industrial,  ItemClass.SubService.None,            ItemClass.Level.None);
            snapshot.TaxRateOffice          = economyManagerInstance.GetTaxRate(ItemClass.Service.Office,      ItemClass.SubService.None,            ItemClass.Level.None);

            // City Economy - logic for income and expenses copied from EconomyPanel.IncomeExpensesPoll.Poll;
            // total income and total expenses are read directly from the game versus summing the component incomes and expenses; the result should be the same
            // bank balance uses internal cash (not logic from global::Bindings.cash) to avoid a huge number when the Unlmited Money mod is enabled
            snapshot.CityEconomyTotalIncome   = GetEconomyIncome (ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
            snapshot.CityEconomyTotalExpenses = GetEconomyExpense(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
            snapshot.CityEconomyBankBalance   = ConvertMoney(economyManagerInstance.InternalCashAmount);  // can be negative
            snapshot.CityEconomyLoanBalance   = GetLoanBalance();
            snapshot.CityEconomyCityValue     = GetCityValue(snapshot.CityEconomyBankBalance.Value, snapshot.CityEconomyLoanBalance.Value);   // can be negative

            // Residential Income - logic copied from EconomyPanel.InitializePolls
                                snapshot.ResidentialIncomeLowDensity1               = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,     ItemClass.Level.Level1);
                                snapshot.ResidentialIncomeLowDensity2               = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,     ItemClass.Level.Level2);
                                snapshot.ResidentialIncomeLowDensity3               = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,     ItemClass.Level.Level3);
                                snapshot.ResidentialIncomeLowDensity4               = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,     ItemClass.Level.Level4);
                                snapshot.ResidentialIncomeLowDensity5               = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow,     ItemClass.Level.Level5);
            if (dlcGreenCities) snapshot.ResidentialIncomeLowDensitySelfSufficient  = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLowEco,  ItemClass.Level.None);
                                snapshot.ResidentialIncomeHighDensity1              = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh,    ItemClass.Level.Level1);
                                snapshot.ResidentialIncomeHighDensity2              = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh,    ItemClass.Level.Level2);
                                snapshot.ResidentialIncomeHighDensity3              = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh,    ItemClass.Level.Level3);
                                snapshot.ResidentialIncomeHighDensity4              = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh,    ItemClass.Level.Level4);
                                snapshot.ResidentialIncomeHighDensity5              = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh,    ItemClass.Level.Level5);
            if (dlcGreenCities) snapshot.ResidentialIncomeHighDensitySelfSufficient = GetEconomyIncome(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHighEco, ItemClass.Level.None);

            // Commercial Income - logic copied from EconomyPanel.InitializePolls
                                snapshot.CommercialIncomeLowDensity1  = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialLow,     ItemClass.Level.Level1);
                                snapshot.CommercialIncomeLowDensity2  = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialLow,     ItemClass.Level.Level2);
                                snapshot.CommercialIncomeLowDensity3  = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialLow,     ItemClass.Level.Level3);
                                snapshot.CommercialIncomeHighDensity1 = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh,    ItemClass.Level.Level1);
                                snapshot.CommercialIncomeHighDensity2 = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh,    ItemClass.Level.Level2);
                                snapshot.CommercialIncomeHighDensity3 = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh,    ItemClass.Level.Level3);
            if (dlcAfterDark  ) snapshot.CommercialIncomeLeisure      = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialLeisure, ItemClass.Level.None);
            if (dlcAfterDark  ) snapshot.CommercialIncomeTourism      = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialTourist, ItemClass.Level.None);
            if (dlcGreenCities) snapshot.CommercialIncomeOrganic      = GetEconomyIncome(ItemClass.Service.Commercial, ItemClass.SubService.CommercialEco,     ItemClass.Level.None);

            // Industrial Income - logic copied from EconomyPanel.InitializePolls
            snapshot.IndustrialIncomeGeneric1 = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialGeneric,  ItemClass.Level.Level1);
            snapshot.IndustrialIncomeGeneric2 = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialGeneric,  ItemClass.Level.Level2);
            snapshot.IndustrialIncomeGeneric3 = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialGeneric,  ItemClass.Level.Level3);
            snapshot.IndustrialIncomeForestry = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialForestry, ItemClass.Level.None);
            snapshot.IndustrialIncomeFarming  = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialFarming,  ItemClass.Level.None);
            snapshot.IndustrialIncomeOre      = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialOre,      ItemClass.Level.None);
            snapshot.IndustrialIncomeOil      = GetEconomyIncome(ItemClass.Service.Industrial, ItemClass.SubService.IndustrialOil,      ItemClass.Level.None);

            // Office Income - logic copied from EconomyPanel.InitializePolls
                                snapshot.OfficeIncomeGeneric1  = GetEconomyIncome(ItemClass.Service.Office, ItemClass.SubService.OfficeGeneric,  ItemClass.Level.Level1);
                                snapshot.OfficeIncomeGeneric2  = GetEconomyIncome(ItemClass.Service.Office, ItemClass.SubService.OfficeGeneric,  ItemClass.Level.Level2);
                                snapshot.OfficeIncomeGeneric3  = GetEconomyIncome(ItemClass.Service.Office, ItemClass.SubService.OfficeGeneric,  ItemClass.Level.Level3);
            if (dlcGreenCities) snapshot.OfficeIncomeITCluster = GetEconomyIncome(ItemClass.Service.Office, ItemClass.SubService.OfficeHightech, ItemClass.Level.None);

            // Tourism Income - logic copied from EconomyPanel.InitializePolls
                             snapshot.TourismIncomeCommercialZones = GetEconomyIncome(ItemClass.Service.Tourism, ItemClass.SubService.None, ItemClass.Level.None);
                             snapshot.TourismIncomeTransportation  = ConvertMoney(EconomyPanel.PollPublicTransportTourismIncome());
            if (dlcParkLife) snapshot.TourismIncomeParkAreas       = ConvertMoney(EconomyPanel.PollParkAreasTourismIncome());

            // Service Expenses - logic copied from EconomyPanel.InitializePolls and EconomyPanel.IncomeExpensesPoll.Poll
            List<ushort>[] arenas = GetArenasData();
                                     snapshot.ServiceExpensesRoads               = GetEconomyExpense(ItemClass.Service.Road,             ItemClass.SubService.None, ItemClass.Level.None);    // value includes Toll Booth expenses, which get subtracted later
                                     snapshot.ServiceExpensesElectricity         = GetEconomyExpense(ItemClass.Service.Electricity,      ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesWaterSewageHeating  = GetEconomyExpense(ItemClass.Service.Water,            ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesGarbage             = GetEconomyExpense(ItemClass.Service.Garbage,          ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesHealthcare          = GetEconomyExpense(ItemClass.Service.HealthCare,       ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesFire                = GetEconomyExpense(ItemClass.Service.FireDepartment,   ItemClass.SubService.None, ItemClass.Level.None);
            if (dlcNaturalDisasters) snapshot.ServiceExpensesEmergency           = GetEconomyExpense(ItemClass.Service.Disaster,         ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesPolice              = GetEconomyExpense(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, ItemClass.Level.None);
                                     snapshot.ServiceExpensesEducation           = GetEconomyExpense(ItemClass.Service.Education,        ItemClass.SubService.None, ItemClass.Level.None);    // does not include Campus Areas
                                     snapshot.ServiceExpensesParksPlazas         = GetEconomyExpense(ItemClass.Service.Beautification,   ItemClass.SubService.None, ItemClass.Level.None);    // value includes Park Area expenses, which get subtracted later
                                     snapshot.ServiceExpensesUniqueBuildings     = GetEconomyExpense(ItemClass.Service.Monument,         ItemClass.SubService.None, ItemClass.Level.None);
            if (dlcCampus          ) snapshot.ServiceExpensesGenericSportsArenas = CalculateArenasExpenses(arenas[(int)EconomyPanel.ArenaIndex.NonVarsity]);
                                     snapshot.ServiceExpensesLoans               = ConvertMoney(economyManagerInstance.GetLoanExpenses());
                                     snapshot.ServiceExpensesPolicies            = ConvertMoney(economyManagerInstance.GetPolicyExpenses());

            // Park Areas - logic adapted from ParkWorldInfoPanel.UpdateBindings
            if (dlcParkLife)
            {
                // initialize all park income and expenses to zero
                snapshot.ParkAreasCityParkIncome      = 0L; snapshot.ParkAreasCityParkExpenses      = 0L;
                snapshot.ParkAreasAmusementParkIncome = 0L; snapshot.ParkAreasAmusementParkExpenses = 0L;
                snapshot.ParkAreasZooIncome           = 0L; snapshot.ParkAreasZooExpenses           = 0L;
                snapshot.ParkAreasNatureReserveIncome = 0L; snapshot.ParkAreasNatureReserveExpenses = 0L;

                // loop over each park district
                DistrictPark[] parks = districtManagerInstance.m_parks.m_buffer;
		        for (byte parkID = 0; parkID < parks.Length; parkID++)
		        {
                    // must be a valid park
                    DistrictPark park = parks[parkID];
			        if (park.IsPark && (park.m_flags & DistrictPark.Flags.Created) != 0)
			        {
                        // get park income and expenses
                        long income = park.m_finalTicketIncome / 100u;
                        long expenses = (long)Math.Round(districtManagerInstance.GetParkExpenses(parkID) * 0.0016f, 0);

                        // accumulate income and expenses in the appropriate park type
                        // the game allows more than one of each park type
                        // each park area can have only one type of gate; a park area with no gate is Generic
                        // any park area building can be placed in any type of park (e.g. zoo buildings can be placed in an amusement park area)
                        switch (park.m_parkType)
                        {
                            case DistrictPark.ParkType.Generic:       snapshot.ParkAreasCityParkIncome      += income; snapshot.ParkAreasCityParkExpenses      += expenses; break;
                            case DistrictPark.ParkType.AmusementPark: snapshot.ParkAreasAmusementParkIncome += income; snapshot.ParkAreasAmusementParkExpenses += expenses; break;
                            case DistrictPark.ParkType.Zoo:           snapshot.ParkAreasZooIncome           += income; snapshot.ParkAreasZooExpenses           += expenses; break;
                            case DistrictPark.ParkType.NatureReserve: snapshot.ParkAreasNatureReserveIncome += income; snapshot.ParkAreasNatureReserveExpenses += expenses; break;
                            default:
                                LogUtil.LogError($"Unhandled park type [{park.m_parkType}].");
                                break;
                        }
                    }
		        }
            }

            // Fishing Industry - logic copied from EconomyPanel.InitializePolls and EconomyPanel.IncomeExpensesPoll.Poll
            if (dlcSunsetHarbor)
            {
                snapshot.FishingIndustryFishingIncome   = GetEconomyIncome (ItemClass.Service.Fishing, ItemClass.SubService.None, ItemClass.Level.None);
                snapshot.FishingIndustryFishingExpenses = GetEconomyExpense(ItemClass.Service.Fishing, ItemClass.SubService.None, ItemClass.Level.None);
            }

            // Industry Areas - logic copied from EconomyPanel.InitializePolls and EconomyPanel.IncomeExpensesPoll.Poll
            // fishing must be computed first because fishing is subtracted to compute warehouses and factories
            if (dlcIndustries)
            {
                snapshot.IndustryAreasForestryIncome              = GetEconomyIncome (ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryForestry, ItemClass.Level.None);
                snapshot.IndustryAreasForestryExpenses            = GetEconomyExpense(ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryForestry, ItemClass.Level.None);
                snapshot.IndustryAreasFarmingIncome               = GetEconomyIncome (ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryFarming,  ItemClass.Level.None);
                snapshot.IndustryAreasFarmingExpenses             = GetEconomyExpense(ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryFarming,  ItemClass.Level.None);
                snapshot.IndustryAreasOreIncome                   = GetEconomyIncome (ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOre,      ItemClass.Level.None);
                snapshot.IndustryAreasOreExpenses                 = GetEconomyExpense(ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOre,      ItemClass.Level.None);
                snapshot.IndustryAreasOilIncome                   = GetEconomyIncome (ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOil,      ItemClass.Level.None);
                snapshot.IndustryAreasOilExpenses                 = GetEconomyExpense(ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOil,      ItemClass.Level.None);
                snapshot.IndustryAreasWarehousesFactoriesIncome   = GetEconomyIncome (ItemClass.Service.PlayerIndustry, ItemClass.SubService.None,                   ItemClass.Level.None) -
                    (snapshot.IndustryAreasForestryIncome   + snapshot.IndustryAreasFarmingIncome   + snapshot.IndustryAreasOreIncome   + snapshot.IndustryAreasOilIncome   + (snapshot.FishingIndustryFishingIncome   ?? 0));
                snapshot.IndustryAreasWarehousesFactoriesExpenses = GetEconomyExpense(ItemClass.Service.PlayerIndustry, ItemClass.SubService.None,                   ItemClass.Level.None) -
                    (snapshot.IndustryAreasForestryExpenses + snapshot.IndustryAreasFarmingExpenses + snapshot.IndustryAreasOreExpenses + snapshot.IndustryAreasOilExpenses + (snapshot.FishingIndustryFishingExpenses ?? 0));
            }

            // Campus Areas - logic copied from EconomyPanel.InitializePolls and EconomyPanel.IncomeExpensesPoll.Poll
            if (dlcCampus)
            {
                snapshot.CampusAreasTradeSchoolIncome          = GetEconomyIncome (ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationTradeSchool, ItemClass.Level.None);
                snapshot.CampusAreasTradeSchoolExpenses        = GetEconomyExpense(ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationTradeSchool, ItemClass.Level.None) +
                                                                 CalculateArenasExpenses(arenas[(int)EconomyPanel.ArenaIndex.TradeSchool]);
                snapshot.CampusAreasLiberalArtsCollegeIncome   = GetEconomyIncome (ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationLiberalArts, ItemClass.Level.None);
                snapshot.CampusAreasLiberalArtsCollegeExpenses = GetEconomyExpense(ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationLiberalArts, ItemClass.Level.None) +
                                                                 CalculateArenasExpenses(arenas[(int)EconomyPanel.ArenaIndex.LiberalArts]);
                snapshot.CampusAreasUniversityIncome           = GetEconomyIncome (ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationUniversity,  ItemClass.Level.None);
                snapshot.CampusAreasUniversityExpenses         = GetEconomyExpense(ItemClass.Service.PlayerEducation, ItemClass.SubService.PlayerEducationUniversity,  ItemClass.Level.None) +
                                                                 CalculateArenasExpenses(arenas[(int)EconomyPanel.ArenaIndex.University]);
            }

            // Transport Economy - logic copied from EconomyPanel.InitializePolls and EconomyPanel.IncomeExpensesPoll.Poll
                                 snapshot.TransportEconomyBusIncome             = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus,        ItemClass.Level.None);
                                 snapshot.TransportEconomyBusExpenses           = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus,        ItemClass.Level.None);
            if (dlcSunsetHarbor) snapshot.TransportEconomyTrolleybusIncome      = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrolleybus, ItemClass.Level.None);
            if (dlcSunsetHarbor) snapshot.TransportEconomyTrolleybusExpenses    = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrolleybus, ItemClass.Level.None);
            if (dlcSnowfall    ) snapshot.TransportEconomyTramIncome            = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTram,       ItemClass.Level.None);
            if (dlcSnowfall    ) snapshot.TransportEconomyTramExpenses          = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTram,       ItemClass.Level.None);
                                 snapshot.TransportEconomyMetroIncome           = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMetro,      ItemClass.Level.None);
                                 snapshot.TransportEconomyMetroExpenses         = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMetro,      ItemClass.Level.None);
                                 snapshot.TransportEconomyTrainIncome           = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain,      ItemClass.Level.None);
                                 snapshot.TransportEconomyTrainExpenses         = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain,      ItemClass.Level.None);
                                 snapshot.TransportEconomyShipIncome            = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip,       ItemClass.Level.None);
                                 snapshot.TransportEconomyShipExpenses          = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip,       ItemClass.Level.None);
                                 snapshot.TransportEconomyAirIncome             = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane,      ItemClass.Level.None);
                                 snapshot.TransportEconomyAirExpenses           = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane,      ItemClass.Level.None);
            if (dlcMassTransit ) snapshot.TransportEconomyMonorailIncome        = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMonorail,   ItemClass.Level.None);
            if (dlcMassTransit ) snapshot.TransportEconomyMonorailExpenses      = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMonorail,   ItemClass.Level.None);
            if (dlcMassTransit ) snapshot.TransportEconomyCableCarIncome        = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar,   ItemClass.Level.None);
            if (dlcMassTransit ) snapshot.TransportEconomyCableCarExpenses      = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar,   ItemClass.Level.None);
            if (dlcAfterDark   ) snapshot.TransportEconomyTaxiIncome            = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi,       ItemClass.Level.None);
            if (dlcAfterDark   ) snapshot.TransportEconomyTaxiExpenses          = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi,       ItemClass.Level.None);
            if (dlcParkLife    ) snapshot.TransportEconomyToursIncome           = GetEconomyIncome (ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTours,      ItemClass.Level.None);
            if (dlcParkLife    ) snapshot.TransportEconomyToursExpenses         = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTours,      ItemClass.Level.None);
                                 snapshot.TransportEconomyTollBoothIncome       = GetEconomyIncome (ItemClass.Service.Vehicles,        ItemClass.SubService.None,                      ItemClass.Level.Level1) +    // small vehicle tolls
                                                                                  GetEconomyIncome (ItemClass.Service.Vehicles,        ItemClass.SubService.None,                      ItemClass.Level.Level2);     // large vehicle tolls
            if (dlcIndustries  ) snapshot.TransportEconomyMailExpenses          = GetEconomyExpense(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPost,       ItemClass.Level.None);
            GetTransportBuildingExpenses(out snapshot.TransportEconomyTollBoothExpenses, out snapshot.TransportEconomySpaceElevatorExpenses);

            // Game Limits
            // used logic is copied from Watch It! mod by Keallu
            // capacity logic for Array8, Array16, and Array32 is the buffer length, in case a mod changes the buffer size from the Manager's MAX constant
            // capacity logic for FastList is the max of the buffer length and Manager's MAX constant, because the initial FastList size is only 32, not the final size
            snapshot.GameLimitsBuildingsUsed               = buildingManagerInstance.m_buildingCount;
            snapshot.GameLimitsBuildingsCapacity           = buildingManagerInstance.m_buildings            == null ? 0 : (buildingManagerInstance.m_buildings.m_buffer     == null ? 0 : buildingManagerInstance.m_buildings.m_buffer.Length);
            snapshot.GameLimitsCitizensUsed                = citizenManagerInstance.m_citizenCount;
            snapshot.GameLimitsCitizensCapacity            = citizenManagerInstance.m_citizens              == null ? 0 : (citizenManagerInstance.m_citizens.m_buffer       == null ? 0 : citizenManagerInstance.m_citizens.m_buffer.Length);
            snapshot.GameLimitsCitizenUnitsUsed            = citizenManagerInstance.m_unitCount;
            snapshot.GameLimitsCitizenUnitsCapacity        = citizenManagerInstance.m_units                 == null ? 0 : (citizenManagerInstance.m_units.m_buffer          == null ? 0 : citizenManagerInstance.m_units.m_buffer.Length);
            snapshot.GameLimitsCitizenInstancesUsed        = citizenManagerInstance.m_instanceCount;
            snapshot.GameLimitsCitizenInstancesCapacity    = citizenManagerInstance.m_instances             == null ? 0 : (citizenManagerInstance.m_instances.m_buffer      == null ? 0 : citizenManagerInstance.m_instances.m_buffer.Length);
            if (dlcNaturalDisasters)
            {
                snapshot.GameLimitsDisastersUsed           = disasterManagerInstance.m_disasterCount;
                snapshot.GameLimitsDisastersCapacity       = Math.Max(disasterManagerInstance.m_disasters   == null ? 0 : (disasterManagerInstance.m_disasters.m_buffer     == null ? 0 : disasterManagerInstance.m_disasters.m_buffer.Length), DisasterManager.MAX_DISASTER_COUNT);
            }
            snapshot.GameLimitsDistrictsUsed               = districtManagerInstance.m_districtCount;
            snapshot.GameLimitsDistrictsCapacity           = districtManagerInstance.m_districts            == null ? 0 : (districtManagerInstance.m_districts.m_buffer     == null ? 0 : districtManagerInstance.m_districts.m_buffer.Length);
            snapshot.GameLimitsEventsUsed                  = eventManagerInstance.m_eventCount;
            snapshot.GameLimitsEventsCapacity              = Math.Max(eventManagerInstance.m_events         == null ? 0 : (eventManagerInstance.m_events.m_buffer           == null ? 0 : eventManagerInstance.m_events.m_buffer.Length), EventManager.MAX_EVENT_COUNT);
            snapshot.GameLimitsGameAreasUsed               = gameAreaManagerInstance.m_areaCount;
            snapshot.GameLimitsGameAreasCapacity           = gameAreaManagerInstance.MaxAreaCount;
            snapshot.GameLimitsNetworkLanesUsed            = netManagerInstance.m_laneCount;
            snapshot.GameLimitsNetworkLanesCapacity        = netManagerInstance.m_lanes                     == null ? 0 : (netManagerInstance.m_lanes.m_buffer              == null ? 0 : netManagerInstance.m_lanes.m_buffer.Length);
            snapshot.GameLimitsNetworkNodesUsed            = netManagerInstance.m_nodeCount;
            snapshot.GameLimitsNetworkNodesCapacity        = netManagerInstance.m_nodes                     == null ? 0 : (netManagerInstance.m_nodes.m_buffer              == null ? 0 : netManagerInstance.m_nodes.m_buffer.Length);
            snapshot.GameLimitsNetworkSegmentsUsed         = netManagerInstance.m_segmentCount;
            snapshot.GameLimitsNetworkSegmentsCapacity     = netManagerInstance.m_segments                  == null ? 0 : (netManagerInstance.m_segments.m_buffer           == null ? 0 : netManagerInstance.m_segments.m_buffer.Length);
            if (dlcParkLife || dlcIndustries || dlcCampus || dlcAirports)
            {
                snapshot.GameLimitsPaintedAreasUsed        = districtManagerInstance.m_parkCount;
                snapshot.GameLimitsPaintedAreasCapacity    = districtManagerInstance.m_parks                == null ? 0 : (districtManagerInstance.m_parks.m_buffer         == null ? 0 : districtManagerInstance.m_parks.m_buffer.Length);
            }
            snapshot.GameLimitsPathUnitsUsed               = pathManagerInstance.m_pathUnitCount;
            snapshot.GameLimitsPathUnitsCapacity           = pathManagerInstance.m_pathUnits                == null ? 0 : (pathManagerInstance.m_pathUnits.m_buffer         == null ? 0 : pathManagerInstance.m_pathUnits.m_buffer.Length);
            snapshot.GameLimitsPropsUsed                   = propManagerInstance.m_propCount;
            if (ModUtil.IsWorkshopModEnabled(ModUtil.ModIDExtendedManagersLibrary))
            {
                snapshot.GameLimitsPropsCapacity           = GetExtendedManagersLibraryPropsCapacity();
            }
            else
            {
                snapshot.GameLimitsPropsCapacity           = propManagerInstance.m_props                    == null ? 0 : (propManagerInstance.m_props.m_buffer             == null ? 0 : propManagerInstance.m_props.m_buffer.Length);
            }
            snapshot.GameLimitsRadioChannelsUsed           = audioManagerInstance.m_radioChannelCount;
            snapshot.GameLimitsRadioChannelsCapacity       = Math.Max(audioManagerInstance.m_radioChannels  == null ? 0 : (audioManagerInstance.m_radioChannels.m_buffer    == null ? 0 : audioManagerInstance.m_radioChannels.m_buffer.Length), AudioManager.MAX_RADIO_CHANNEL_COUNT);
            snapshot.GameLimitsRadioContentsUsed           = audioManagerInstance.m_radioContentCount;
            snapshot.GameLimitsRadioContentsCapacity       = Math.Max(audioManagerInstance.m_radioContents  == null ? 0 : (audioManagerInstance.m_radioContents.m_buffer    == null ? 0 : audioManagerInstance.m_radioContents.m_buffer.Length), AudioManager.MAX_RADIO_CONTENT_COUNT);
            snapshot.GameLimitsTransportLinesUsed          = transportManagerInstance.m_lineCount;
            snapshot.GameLimitsTransportLinesCapacity      = transportManagerInstance.m_lines               == null ? 0 : (transportManagerInstance.m_lines.m_buffer        == null ? 0 : transportManagerInstance.m_lines.m_buffer.Length);
            snapshot.GameLimitsTreesUsed                   = treeManagerInstance.m_treeCount;
            snapshot.GameLimitsTreesCapacity               = treeManagerInstance.m_trees                    == null ? 0 : (treeManagerInstance.m_trees.m_buffer             == null ? 0 : treeManagerInstance.m_trees.m_buffer.Length);
            snapshot.GameLimitsVehiclesUsed                = vehicleManagerInstance.m_vehicleCount;
            snapshot.GameLimitsVehiclesCapacity            = vehicleManagerInstance.m_vehicles              == null ? 0 : (vehicleManagerInstance.m_vehicles.m_buffer       == null ? 0 : vehicleManagerInstance.m_vehicles.m_buffer.Length);
            snapshot.GameLimitsVehiclesParkedUsed          = vehicleManagerInstance.m_parkedCount;
            snapshot.GameLimitsVehiclesParkedCapacity      = vehicleManagerInstance.m_parkedVehicles        == null ? 0 : (vehicleManagerInstance.m_parkedVehicles.m_buffer == null ? 0 : vehicleManagerInstance.m_parkedVehicles.m_buffer.Length);
            snapshot.GameLimitsZoneBlocksUsed              = zoneManagerInstance.m_blockCount;
            snapshot.GameLimitsZoneBlocksCapacity          = zoneManagerInstance.m_blocks                   == null ? 0 : (zoneManagerInstance.m_blocks.m_buffer            == null ? 0 : zoneManagerInstance.m_blocks.m_buffer.Length);

            // now that some other values have been obtained, adjust some Expenses Services
            snapshot.ServiceExpensesRoads       -= snapshot.TransportEconomyTollBoothExpenses;
            snapshot.ServiceExpensesParksPlazas -= (snapshot.ParkAreasTotalExpenses ?? 0);

            // return the snapshot
            return snapshot;
        }

        /// <summary>
        /// get income from EconomyManager
        /// </summary>
        private static long GetEconomyIncome(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            EconomyManager.instance.GetIncomeAndExpenses(service, subService, level, out long income, out _);
            return ConvertMoney(income);
        }

        /// <summary>
        /// get expense from EconomyManager
        /// </summary>
        private static long GetEconomyExpense(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            EconomyManager.instance.GetIncomeAndExpenses(service, subService, level, out _, out long expense);
            return ConvertMoney(expense);
        }

        /// <summary>
        /// convert a money amount with implicit cents into a rounded amount without cents
        /// </summary>
        private static long ConvertMoney(long amount)
        {
            return (amount + (amount > long.MaxValue - 50L ? 0L : 50L)) / 100L;
        }

        /// <summary>
        /// get arenas data
        /// logic mostly copied from EconomyPanel.SetupArenasData
        /// </summary>
        private static List<ushort>[] GetArenasData()
        {
            // initialize arenas; logic copied from EconomyPanel.Start
   		    List<ushort>[] arenas = new List<ushort>[(int)EconomyPanel.ArenaIndex.Count];
		    for (int i = 0; i < arenas.Length; i++)
		    {
			    arenas[i] = new List<ushort>();
		    }

            // get manager instances
            BuildingManager bmInstance = BuildingManager.instance;
            DistrictManager dmInstance = DistrictManager.instance;

            // get varsity sports building IDs
            Building[] buildings = bmInstance.m_buildings.m_buffer;
	        FastList<ushort> serviceBuildings = bmInstance.GetServiceBuildings(ItemClass.Service.VarsitySports);
	        for (int j = 0; j < serviceBuildings.m_size; j++)
	        {
                // building must have VarsitySportsArenaAI
                // not sure if this can ever be other than VarsitySportsArenaAI, but check anyway
		        ushort buildingID = serviceBuildings[j];
                Building building = buildings[buildingID];
		        if (building.Info.m_buildingAI.GetType() == typeof(VarsitySportsArenaAI))
		        {
                    // building must be active
                    if ((building.m_flags & Building.Flags.Active) != 0)
                    {
                        // check if building is in a campus district
		                byte parkID = dmInstance.GetPark(building.m_position);
		                if (parkID != 0 && dmInstance.m_parks.m_buffer[parkID].IsCampus)
		                {
                            // add the arena to the appropriate arena list based on campus type and continue to the next building
			                switch (dmInstance.m_parks.m_buffer[parkID].m_parkType)
			                {
			                    case DistrictPark.ParkType.TradeSchool: arenas[(int)EconomyPanel.ArenaIndex.TradeSchool].Add(buildingID); continue;
			                    case DistrictPark.ParkType.LiberalArts: arenas[(int)EconomyPanel.ArenaIndex.LiberalArts].Add(buildingID); continue;
			                    case DistrictPark.ParkType.University:  arenas[(int)EconomyPanel.ArenaIndex.University ].Add(buildingID); continue;
			                    case DistrictPark.ParkType.GenericCampus:
                                    // allow building to be added to non-varsity below
                                    break;
			                    default:
                                    // skip this building
                                    continue;
			                }
		                }

                        // arena is not in a campus district or is in a generic campus; add the arena to the non-varsity arena list
		                arenas[(int)EconomyPanel.ArenaIndex.NonVarsity].Add(buildingID);
                    }
		        }
	        }

            // return the arenas data
            return arenas;
        }

        /// <summary>
        /// compute arena expenses from the previously set up arenas data
        /// logic mostly copied from EconomyPanel.CalculateArenasExpenses
        /// </summary>
        private static long? CalculateArenasExpenses(List<ushort> arenaBuildingIDs)
        {
            // do each arena
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            long expenses = 0;
            foreach (ushort buildingID in arenaBuildingIDs)
            {
                // get the building AI
				PlayerBuildingAI playerBuildingAI = buildings[buildingID].Info.m_buildingAI as PlayerBuildingAI;
				if ((object)playerBuildingAI != null)
				{
                    // accumulate the maintenance cost
					int finalMaintenanceCost = playerBuildingAI.GetFinalMaintenanceCost(buildingID, ref buildings[buildingID]);
					finalMaintenanceCost = (int)(finalMaintenanceCost * 0.16d) * 100;
					expenses = ((expenses + finalMaintenanceCost) * 100 + 99) / 100;
				}
            }

            // return the accumulated expenses
            return ConvertMoney(expenses);
        }

        /// <summary>
        /// get expenses for transport buildings
        /// these values are not already computed by the game, so must compute them manually
        /// </summary>
        private static void GetTransportBuildingExpenses(out long tollBoothExpenses, out long spaceElevatorExpenses)
        {
            // initialize
            tollBoothExpenses = 0L;
            spaceElevatorExpenses = 0L;

            // loop over every building
            const Building.Flags buildingFlags = Building.Flags.Completed | Building.Flags.Active;
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            for (ushort buildingID = 1; buildingID < buildings.Length; buildingID++)
            {
                // find completed active buildings
                Building building = buildings[buildingID];
                BuildingInfo buildingInfo = building.Info;
                if (buildingInfo != null && buildingInfo.m_buildingAI != null && ((building.m_flags & buildingFlags) == buildingFlags))
                {
                    // accumulate expenses for toll booths
                    Type buildingAIType = buildingInfo.m_buildingAI.GetType();
                    if (buildingAIType == typeof(TollBoothAI))
                    {
                        TollBoothAI buildingAI = buildingInfo.m_buildingAI as TollBoothAI;
					    int finalMaintenanceCost = buildingAI.GetFinalMaintenanceCost(buildingID, ref building);
					    finalMaintenanceCost = (int)(finalMaintenanceCost * 0.16d);
					    tollBoothExpenses += finalMaintenanceCost;
                    }
                    // accumulate expenses for space elevator (there can be only one)
                    else if (buildingAIType == typeof(SpaceElevatorAI))
                    {
                        SpaceElevatorAI buildingAI = buildingInfo.m_buildingAI as SpaceElevatorAI;
					    int finalMaintenanceCost = buildingAI.GetFinalMaintenanceCost(buildingID, ref building);
					    finalMaintenanceCost = (int)(finalMaintenanceCost * 0.16d);
					    spaceElevatorExpenses += finalMaintenanceCost;
                    }
                }
            }
        }

        /// <summary>
        /// get zoned squares
        /// logic adapted from ZoneInfo mod ZoneInfoPanel.Update
        /// </summary>
        private static void GetZoning(out int residential, out int commercial, out int industrial, out int office, out int unzoned)
        {
            // initialize return values
            residential = 0;
            commercial = 0;
            industrial = 0;
            office = 0;
            unzoned = 0;

            try
            {
                // initialize data for the previous building found
                Quad2 buildingCorners = default;
                ItemClass.SubService buildingSubservice = ItemClass.SubService.None;
                bool buildingFound = false;

                // do each zone block
                ZoneBlock[] blocks = ZoneManager.instance.m_blocks.m_buffer;
                for (ushort _blockCounter = 0; _blockCounter < blocks.Length; _blockCounter++)
                {
                    // do only created blocks
                    ZoneBlock block = blocks[_blockCounter];
                    if ((block.m_flags & ZoneBlock.FLAG_CREATED) != 0)
                    {
                        // compute values for the block that will be used later to compute the position and corners of each square in the block
                        float angle = block.m_angle;
                        Vector2 vector1 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * SquareSize;
                        Vector2 vector2 = new Vector2(vector1.y, 0f - vector1.x);
                        Vector2 blockPosition = VectorUtils.XZ(block.m_position);

                        // do each row along the segment
                        int rowCount = block.RowCount;
                        for (int z = 0; z < rowCount; z++)
                        {
                            // do each column in from the segment
                            // there can never be more than 4
                            for (int x = 0; x < 4; x++)
                            {
                                // do only valid non-shared squares
                                ulong mask = (ulong)(1L << ((z << 3) | x));
                                if ((block.m_valid & mask) != 0 && (block.m_shared & mask) == 0)
                                {
                                    // use the square's zone to determine which square count to increment
                                    switch (block.GetZone(x, z))
                                    {
                                        case ItemClass.Zone.ResidentialLow:
                                        case ItemClass.Zone.ResidentialHigh:
                                            residential++;
                                            break;

                                        case ItemClass.Zone.CommercialLow:
                                        case ItemClass.Zone.CommercialHigh:
                                            commercial++;
                                            break;

                                        case ItemClass.Zone.Industrial:
                                            industrial++;
                                            break;

                                        case ItemClass.Zone.Office:
                                            office++;
                                            break;

                                        case ItemClass.Zone.Unzoned:
                                            // check if occupied
                                            bool occupied = (((block.m_occupied1 | block.m_occupied2) & mask) != 0);
                                            if (occupied)
                                            {
                                                // compute square's center position
                                                // start with the block position
                                                // +1 to convert 0-based x,z into 1-based x,z
                                                // -4 because (I don't really know why, copied logic from various ZoneManager and ZoneBlock methods, but it works)
                                                // -0.5f to put the position in the center of the square
                                                Vector2 squareCenter2 = blockPosition + ((x + 1 - 4 - 0.5f) * vector1) + ((z + 1 - 4 - 0.5f) * vector2);
                                                Vector3 squareCenter3 = new Vector3(squareCenter2.x, block.m_position.y, squareCenter2.y);

                                                // compute the corners of the square
                                                // use 0.5 because need to go half of the square size in each direction
                                                // use 0.99 to get just inside the square's corners to prevent finding buildings on adjacent squares
                                                const float multiplier = 0.5f * 0.99f;
                                                Vector2 squareVector1 = vector1 * multiplier;
                                                Vector2 squareVector2 = vector2 * multiplier;
                                                Quad2 squareCorners = default;
                                                squareCorners.a = squareCenter2 - squareVector1 - squareVector2;
                                                squareCorners.b = squareCenter2 + squareVector1 - squareVector2;
                                                squareCorners.c = squareCenter2 + squareVector1 + squareVector2;
                                                squareCorners.d = squareCenter2 - squareVector1 + squareVector2;

                                                // performance enhancement:
                                                // it is likely that the current square position being checked is in the building previously found
                                                // the Quad2.Intersect logic is much faster than GetBuildingAtPosition

                                                // check if a building was found and current square corners being checked intersect with the previous building's corners
                                                if (buildingFound && squareCorners.Intersect(buildingCorners))
                                                {
                                                    // use subservice from previous building
                                                }
                                                else
                                                {
                                                    // no building previously found or building does not intersect with the current square
                                                    // check if there is a building at the square position and get the building's corners and subservice
                                                    buildingFound = GetBuildingAtPosition(squareCenter3, squareCorners, ref buildingCorners, ref buildingSubservice);
                                                }

                                                // if a building is found, use the subservice to determine which square count to increment
                                                if (buildingFound)
                                                {
                                                    switch (buildingSubservice)
                                                    {
                                                        case ItemClass.SubService.ResidentialLow:
                                                        case ItemClass.SubService.ResidentialHigh:
                                                        case ItemClass.SubService.ResidentialLowEco:
                                                        case ItemClass.SubService.ResidentialHighEco:
                                                            residential++;
                                                            break;

                                                        case ItemClass.SubService.CommercialLow:
                                                        case ItemClass.SubService.CommercialHigh:
                                                        case ItemClass.SubService.CommercialTourist:
                                                        case ItemClass.SubService.CommercialLeisure:
                                                        case ItemClass.SubService.CommercialEco:
                                                            commercial++;
                                                            break;

                                                        case ItemClass.SubService.IndustrialGeneric:
                                                        case ItemClass.SubService.PlayerIndustryForestry:
                                                        case ItemClass.SubService.IndustrialForestry:
                                                        case ItemClass.SubService.PlayerIndustryFarming:
                                                        case ItemClass.SubService.IndustrialFarming:
                                                        case ItemClass.SubService.PlayerIndustryOre:
                                                        case ItemClass.SubService.IndustrialOre:
                                                        case ItemClass.SubService.PlayerIndustryOil:
                                                        case ItemClass.SubService.IndustrialOil:
                                                            industrial++;
                                                            break;

                                                        case ItemClass.SubService.OfficeGeneric:
                                                        case ItemClass.SubService.OfficeHightech:
                                                            office++;
                                                            break;

                                                        default:
                                                            // building is not a subservice being counted
                                                            // building could be a service building, park, or other structure that causes the square to be unzoned
                                                            // this is not an error, just count as unzoned
                                                            unzoned++;
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    // no building found even though zone block indicates occupied
                                                    // this is not an error, just count as unzoned
                                                    unzoned++;
                                                }
                                            }
                                            else
                                            {
                                                // unoccupied always gets counted as unzoned
                                                unzoned++;
                                            }
                                            break;

                                        case ItemClass.Zone.None:
                                        case ItemClass.Zone.Distant:
                                            // ignore, should never get here
                                            break;
                                    }
                                }   // do valid non-shared squares
                            }   // do each column
                        }   // do each row
                    }   // block is created
                }   // do each zone block
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }

        /// <summary>
        /// check if there is a building at the square position
        /// routine copied exactly from ZoneInfo mod except no check for stop update
        /// </summary>
        /// <remarks>logic adapted from BuildingManager.FindBuilding</remarks>
        /// <returns>whether or not a building is found</returns>
        /// <param name="buildingCorners">return the corners of the found building</param>
        /// <param name="buildingSubservice">return the subservice of the found building</param>
        private static bool GetBuildingAtPosition(Vector3 squarePosition, Quad2 squareCorners, ref Quad2 buildingCorners, ref ItemClass.SubService buildingSubservice)
        {
            // compute XZ building grid indexes of the building grid cell that contains the square position
            // Min and Max prevent the indexes from being off the map
            const int GridRes = BuildingManager.BUILDINGGRID_RESOLUTION;
            int baseGridIndexX = Math.Min(Math.Max((int)(squarePosition.x / BuildingManager.BUILDINGGRID_CELL_SIZE + GridRes / 2f), 0), GridRes - 1);
            int baseGridIndexZ = Math.Min(Math.Max((int)(squarePosition.z / BuildingManager.BUILDINGGRID_CELL_SIZE + GridRes / 2f), 0), GridRes - 1);

            // compute low and high XZ grid indexes that are -1 and +1 from the base
            // this defines a 3x3 matrix of grid cells that are horizontally, vertically, and diagonally adjacent to the base grid cell
            // a building could be centered in any of these grid cells but overlap the square being checked
            // Min and Max prevent the indexes from being off the map
            int loGridIndexX = Math.Max(baseGridIndexX - 1, 0);
            int loGridIndexZ = Math.Max(baseGridIndexZ - 1, 0);
            int hiGridIndexX = Math.Min(baseGridIndexX + 1, GridRes - 1);
            int hiGridIndexZ = Math.Min(baseGridIndexZ + 1, GridRes - 1);

            // start with the base grid cell because it is most likely to contain the building being sought
            int gridIndexX = baseGridIndexX;
            int gridIndexZ = baseGridIndexZ;

            // do each grid cell in the 3x3 matrix (i.e. 9 grid cells)
            for (int gridCellCounter = 0; gridCellCounter < 9; )
            {
                // loop over every building in the grid cell
                uint maxBuildingCount = BuildingManager.instance.m_buildings.m_size;
                int buildingCounter = 0;
                ushort buildingID = BuildingManager.instance.m_buildingGrid[gridIndexZ * GridRes + gridIndexX];
                while (buildingID != 0)
                {
                    // building AI must derive from CommonBuildingAI
                    Building building = BuildingManager.instance.m_buildings.m_buffer[buildingID];
                    if (building.Info.GetAI().GetType().IsSubclassOf(typeof(CommonBuildingAI)))
                    {
                        // compute the corner positions of the building
                        // need to go half the width and length in each direction
                        // logic adapted from BuildingManager.UpdateParkingSpaces
                        float buildingAngle = building.m_angle;
                        Vector2 buildingVector1 = new Vector2(Mathf.Cos(buildingAngle), Mathf.Sin(buildingAngle)) * SquareSize;
                        Vector2 buildingVector2 = new Vector2(buildingVector1.y, 0f - buildingVector1.x);
                        buildingVector1 *= 0.5f * building.Width;
                        buildingVector2 *= 0.5f * building.Length;
                        Vector2 buildingPosition = VectorUtils.XZ(building.m_position);
                        Quad2 tempCorners = default;
                        tempCorners.a = buildingPosition - buildingVector1 - buildingVector2;
                        tempCorners.b = buildingPosition + buildingVector1 - buildingVector2;
                        tempCorners.c = buildingPosition + buildingVector1 + buildingVector2;
                        tempCorners.d = buildingPosition - buildingVector1 + buildingVector2;

                        // square's area must intersect with the building's area
                        if (squareCorners.Intersect(tempCorners))
                        {
                            // found a building at the position, return building corners and subservice
                            buildingCorners = tempCorners;
                            buildingSubservice = building.Info.GetSubService();
                            return true;
                        }
                    }

                    // get the next building from the grid
                    buildingID = building.m_nextGridBuilding;

                    // check for error (e.g. circular reference)
                    if (++buildingCounter >= maxBuildingCount)
                    {
                        LogUtil.LogError("Invalid list detected!" + Environment.NewLine + Environment.StackTrace);
                        break;
                    }
                }

                // get next building grid cell
                // do in order:  horizontally, vertically, and diagonally adjacent to base grid cell
                gridCellCounter++;
                if (gridCellCounter == 1) { if (                                  loGridIndexX != baseGridIndexX) { gridIndexZ = baseGridIndexZ; gridIndexX = loGridIndexX;   } else gridCellCounter++; }   // left
                if (gridCellCounter == 2) { if (                                  hiGridIndexX != baseGridIndexX) { gridIndexZ = baseGridIndexZ; gridIndexX = hiGridIndexX;   } else gridCellCounter++; }   // right
                if (gridCellCounter == 3) { if (loGridIndexZ != baseGridIndexZ                                  ) { gridIndexZ = loGridIndexZ;   gridIndexX = baseGridIndexX; } else gridCellCounter++; }   // down
                if (gridCellCounter == 4) { if (hiGridIndexZ != baseGridIndexZ                                  ) { gridIndexZ = hiGridIndexZ;   gridIndexX = baseGridIndexX; } else gridCellCounter++; }   // up
                if (gridCellCounter == 5) { if (loGridIndexZ != baseGridIndexZ && loGridIndexX != baseGridIndexX) { gridIndexZ = loGridIndexZ;   gridIndexX = loGridIndexX;   } else gridCellCounter++; }   // down left
                if (gridCellCounter == 6) { if (loGridIndexZ != baseGridIndexZ && hiGridIndexX != baseGridIndexX) { gridIndexZ = loGridIndexZ;   gridIndexX = hiGridIndexX;   } else gridCellCounter++; }   // down right
                if (gridCellCounter == 7) { if (hiGridIndexZ != baseGridIndexZ && loGridIndexX != baseGridIndexX) { gridIndexZ = hiGridIndexZ;   gridIndexX = loGridIndexX;   } else gridCellCounter++; }   // up left
                if (gridCellCounter == 8) { if (hiGridIndexZ != baseGridIndexZ && hiGridIndexX != baseGridIndexX) { gridIndexZ = hiGridIndexZ;   gridIndexX = hiGridIndexX;   } else gridCellCounter++; }   // up right
            }

            // building not found
            return false;
        }

        /// <summary>
        /// get props capacity from Extended Managers Library mod
        /// to avoid an error when the mod is not subscribed, logic must be in a separate routine that is not inlined
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetExtendedManagersLibraryPropsCapacity()
        {
            return EManagersLib.API.PropAPI.PropBufferLen;
        }

        /// <summary>
        /// get loan balance
        /// </summary>
        private static long GetLoanBalance()
        {
            // initialize
            long loanBalance = 0;

            // loop over all loans
            // logic adapted from EconomyManager.SimulationStepImpl
            FieldInfo fieldInfo = typeof(EconomyManager).GetField("m_loans", BindingFlags.NonPublic | BindingFlags.Instance);
            EconomyManager.Loan[] loans = fieldInfo.GetValue(EconomyManager.instance) as EconomyManager.Loan[];
			foreach (EconomyManager.Loan loan in loans)
			{
                // loan must be active
				if (loan.m_length != 0)
				{
                    // loan must have an amount left
					if (loan.m_amountLeft > 0)
					{
                        // subtract the amount left
						loanBalance += loan.m_amountLeft;
					}
				}
			}

            // return loan balance
            return loanBalance / 100L;
        }

        /// <summary>
        /// get city value
        /// logic adapted from StatisticsPanel.OnCheckboxChanged
        /// </summary>
        private static long GetCityValue(long bankBalance, long loanBalance)
        {
            // city value is computed:
            //     construction cost of player buildings
            //     plus construction cost of player network segments
            //     plus bank balance
            //     minus loan balance

            // initialize
            long cityValue = 0;

            // loop over all buildings
            // logic adapted from PlayerBuildingAI.SimulationStep
            Building[] buildings = BuildingManager.instance.m_buildings.m_buffer;
            for (ushort buildingID = 1; buildingID < buildings.Length; buildingID++)
            {
                // building must be created
                Building building = buildings[buildingID];
                if ((building.m_flags & Building.Flags.Created) != 0)
                {
                    // AI must be PlayerBuildingAI or derived from it
                    if (building.Info != null && building.Info.m_buildingAI != null && building.Info.m_buildingAI.GetType().IsSubclassOf(typeof(PlayerBuildingAI)))
                    {
                        // add the building construction cost
                        cityValue += building.Info.m_buildingAI.GetConstructionCost();
                    }
                }
            }

            // loop over all network segments
            // logic adapted from PlayerNetAI.SimulationStep
            NetNode[] netNodes = NetManager.instance.m_nodes.m_buffer;
            NetSegment[] netSegments = NetManager.instance.m_segments.m_buffer;
            for (ushort segmentID = 1; segmentID < netSegments.Length; segmentID++)
            {
                // segment must be created
                NetSegment segment = netSegments[segmentID];
                if ((segment.m_flags & NetSegment.Flags.Created) != 0)
                {
                    // segment must be not original
                    if ((segment.m_flags & NetSegment.Flags.Original) == 0)
                    {
                        // AI must be PlayerNetAI or derived from it
                        if (segment.Info != null && segment.Info.m_netAI != null && segment.Info.m_netAI.GetType().IsSubclassOf(typeof(PlayerNetAI)))
                        {
                            // get start and end node positions
		                    Vector3 startNodePosition = netNodes[segment.m_startNode].m_position;
		                    Vector3 endNodePosition   = netNodes[segment.m_endNode].m_position;

                            // get start and end node elevations
			                float startNodeElevation = netNodes[segment.m_startNode].m_elevation;
			                float endNodeElevation   = netNodes[segment.m_endNode].m_elevation;

                            // adjust node elevations if underground
                            NetAI segmentAI = segment.Info.m_netAI;
			                if (segmentAI.IsUnderground())
			                {
				                startNodeElevation = 0f - startNodeElevation;
				                endNodeElevation = 0f - endNodeElevation;
			                }

                            // add the segment construction cost
                            cityValue += segmentAI.GetConstructionCost(startNodePosition, endNodePosition, startNodeElevation, endNodeElevation);
                        }
                    }
                }
            }

            // adjust city value
            cityValue /= 100L;

            // add bank balance
            cityValue += bankBalance;

            // subtract loan balance
            cityValue -= loanBalance;

            // return city value
            return cityValue;
        }

        /// <summary>
        /// write the snapshot to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // save snapshot date
            writer.Write(SnapshotDate);

            // write only the base values (i.e.fields), not the computed values (i.e.properties)
            // write all base values even if a required DLC is not currently enabled
            // writing each value explicitly is much faster than using reflection to find and write the values
            writer.Write(LocationHome);
            writer.Write(LocationWork);
            writer.Write(LocationVisit);
            writer.Write(LocationMoving);

            writer.Write(TrafficAverageFlow);

            writer.Write(PublicTransportationBusResidents);
            writer.Write(PublicTransportationBusTourists);
            writer.Write(PublicTransportationTrolleybusResidents);
            writer.Write(PublicTransportationTrolleybusTourists);
            writer.Write(PublicTransportationTramResidents);
            writer.Write(PublicTransportationTramTourists);
            writer.Write(PublicTransportationMetroResidents);
            writer.Write(PublicTransportationMetroTourists);
            writer.Write(PublicTransportationTrainResidents);
            writer.Write(PublicTransportationTrainTourists);
            writer.Write(PublicTransportationShipResidents);
            writer.Write(PublicTransportationShipTourists);
            writer.Write(PublicTransportationAirResidents);
            writer.Write(PublicTransportationAirTourists);
            writer.Write(PublicTransportationMonorailResidents);
            writer.Write(PublicTransportationMonorailTourists);
            writer.Write(PublicTransportationCableCarResidents);
            writer.Write(PublicTransportationCableCarTourists);
            writer.Write(PublicTransportationTaxiResidents);
            writer.Write(PublicTransportationTaxiTourists);

            writer.Write(TourismLowWealth);
            writer.Write(TourismMediumWealth);
            writer.Write(TourismHighWealth);

            writer.Write(ToursWalkingTourResidents);
            writer.Write(ToursWalkingTourTourists);
            writer.Write(ToursSightseeingResidents);
            writer.Write(ToursSightseeingTourists);
            writer.Write(ToursBalloonResidents);
            writer.Write(ToursBalloonToursits);

            writer.Write(TransportEconomyBusIncome);
            writer.Write(TransportEconomyBusExpenses);
            writer.Write(TransportEconomyTrolleybusIncome);
            writer.Write(TransportEconomyTrolleybusExpenses);
            writer.Write(TransportEconomyTramIncome);
            writer.Write(TransportEconomyTramExpenses);
            writer.Write(TransportEconomyMetroIncome);
            writer.Write(TransportEconomyMetroExpenses);
            writer.Write(TransportEconomyTrainIncome);
            writer.Write(TransportEconomyTrainExpenses);
            writer.Write(TransportEconomyShipIncome);
            writer.Write(TransportEconomyShipExpenses);
            writer.Write(TransportEconomyAirIncome);
            writer.Write(TransportEconomyAirExpenses);
            writer.Write(TransportEconomyMonorailIncome);
            writer.Write(TransportEconomyMonorailExpenses);
            writer.Write(TransportEconomyCableCarIncome);
            writer.Write(TransportEconomyCableCarExpenses);
            writer.Write(TransportEconomyTaxiIncome);
            writer.Write(TransportEconomyTaxiExpenses);
            writer.Write(TransportEconomyToursIncome);
            writer.Write(TransportEconomyToursExpenses);
            writer.Write(TransportEconomyTollBoothIncome);
            writer.Write(TransportEconomyTollBoothExpenses);
            writer.Write(TransportEconomyMailExpenses);
            writer.Write(TransportEconomySpaceElevatorExpenses);

            writer.Write(GameLimitsCitizenInstancesUsed);
            writer.Write(GameLimitsPathUnitsUsed);
            
        }

        /// <summary>
        /// read a snapshot from the game save file
        /// </summary>
        public static Snapshot Deserialize(BinaryReader reader, int version)
        {
            // create the snapshot with the snapshot date
            Snapshot snapshot = new Snapshot(reader.ReadDate());

            // values must be read in exactly the same order they were written
            // read one value at a time to ensure they are read in the correct order
            // read only the base values (i.e. fields), not the computed values (i.e. properties)
            // read all base values even if a required DLC is not currently enabled
            // reading each value explicitly is much faster than using reflection to find and read the values
            snapshot.LocationHome                     = reader.ReadInt32();
            snapshot.LocationWork                      = reader.ReadInt32();

            snapshot.LocationVisit                           = reader.ReadInt32();
            snapshot.LocationMoving                       = reader.ReadInt32();

            snapshot.TrafficAverageFlow                         = reader.ReadUInt32();

            snapshot.PublicTransportationBusResidents           = reader.ReadUInt32();
            snapshot.PublicTransportationBusTourists            = reader.ReadUInt32();
            snapshot.PublicTransportationTrolleybusResidents    = reader.ReadNullableUInt32();
            snapshot.PublicTransportationTrolleybusTourists     = reader.ReadNullableUInt32();
            snapshot.PublicTransportationTramResidents          = reader.ReadNullableUInt32();
            snapshot.PublicTransportationTramTourists           = reader.ReadNullableUInt32();
            snapshot.PublicTransportationMetroResidents         = reader.ReadUInt32();
            snapshot.PublicTransportationMetroTourists          = reader.ReadUInt32();
            snapshot.PublicTransportationTrainResidents         = reader.ReadUInt32();
            snapshot.PublicTransportationTrainTourists          = reader.ReadUInt32();
            snapshot.PublicTransportationShipResidents          = reader.ReadUInt32();
            snapshot.PublicTransportationShipTourists           = reader.ReadUInt32();
            snapshot.PublicTransportationAirResidents           = reader.ReadUInt32();
            snapshot.PublicTransportationAirTourists            = reader.ReadUInt32();
            snapshot.PublicTransportationMonorailResidents      = reader.ReadNullableUInt32();
            snapshot.PublicTransportationMonorailTourists       = reader.ReadNullableUInt32();
            snapshot.PublicTransportationCableCarResidents      = reader.ReadNullableUInt32();
            snapshot.PublicTransportationCableCarTourists       = reader.ReadNullableUInt32();
            snapshot.PublicTransportationTaxiResidents          = reader.ReadNullableUInt32();
            snapshot.PublicTransportationTaxiTourists           = reader.ReadNullableUInt32();

            snapshot.TourismLowWealth                           = reader.ReadUInt32();
            snapshot.TourismMediumWealth                        = reader.ReadUInt32();
            snapshot.TourismHighWealth                          = reader.ReadUInt32();

            snapshot.ToursWalkingTourResidents                  = reader.ReadNullableUInt32();
            snapshot.ToursWalkingTourTourists                   = reader.ReadNullableUInt32();
            snapshot.ToursSightseeingResidents                  = reader.ReadNullableUInt32();
            snapshot.ToursSightseeingTourists                   = reader.ReadNullableUInt32();
            snapshot.ToursBalloonResidents                      = reader.ReadNullableUInt32();
            snapshot.ToursBalloonToursits                       = reader.ReadNullableUInt32();

            snapshot.TransportEconomyBusIncome                  = reader.ReadInt64();
            snapshot.TransportEconomyBusExpenses                = reader.ReadInt64();
            snapshot.TransportEconomyTrolleybusIncome           = reader.ReadNullableInt64();
            snapshot.TransportEconomyTrolleybusExpenses         = reader.ReadNullableInt64();
            snapshot.TransportEconomyTramIncome                 = reader.ReadNullableInt64();
            snapshot.TransportEconomyTramExpenses               = reader.ReadNullableInt64();
            snapshot.TransportEconomyMetroIncome                = reader.ReadInt64();
            snapshot.TransportEconomyMetroExpenses              = reader.ReadInt64();
            snapshot.TransportEconomyTrainIncome                = reader.ReadInt64();
            snapshot.TransportEconomyTrainExpenses              = reader.ReadInt64();
            snapshot.TransportEconomyShipIncome                 = reader.ReadInt64();
            snapshot.TransportEconomyShipExpenses               = reader.ReadInt64();
            snapshot.TransportEconomyAirIncome                  = reader.ReadInt64();
            snapshot.TransportEconomyAirExpenses                = reader.ReadInt64();
            snapshot.TransportEconomyMonorailIncome             = reader.ReadNullableInt64();
            snapshot.TransportEconomyMonorailExpenses           = reader.ReadNullableInt64();
            snapshot.TransportEconomyCableCarIncome             = reader.ReadNullableInt64();
            snapshot.TransportEconomyCableCarExpenses           = reader.ReadNullableInt64();
            snapshot.TransportEconomyTaxiIncome                 = reader.ReadNullableInt64();
            snapshot.TransportEconomyTaxiExpenses               = reader.ReadNullableInt64();
            snapshot.TransportEconomyToursIncome                = reader.ReadNullableInt64();
            snapshot.TransportEconomyToursExpenses              = reader.ReadNullableInt64();
            snapshot.TransportEconomyTollBoothIncome            = reader.ReadInt64();
            snapshot.TransportEconomyTollBoothExpenses          = reader.ReadInt64();
            snapshot.TransportEconomyMailExpenses               = reader.ReadNullableInt64();
            snapshot.TransportEconomySpaceElevatorExpenses      = reader.ReadInt64();

            snapshot.GameLimitsCitizenInstancesUsed             = reader.ReadInt32();
            snapshot.GameLimitsPathUnitsUsed                    = reader.ReadInt32();

            // return the snapshot
            return snapshot;
        }

    }
}