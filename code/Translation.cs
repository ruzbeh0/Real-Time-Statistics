using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RealTimeStatistics
{
    /// <summary>
    /// get translated text
    /// </summary>
    public class Translation
    {
        // translations are in the Translation.csv file
        // the translation file is an embedded resource in the mod DLL so that a separate file does not need to be downloaded with the mod
        // the translation file was created and maintained using LibreOffice Calc

        // translation file format:
        //      line 1: blank,language code 1,language code 2,...,language code n
        //      line 2: translation key 1,translated text 1,translated text 2,...,translated text n
        //      line 3: translation key 2,translated text 1,translated text 2,...,translated text n
        //      ...
        //      line m: translation key m-1,translated text 1,translated text 2,...,translated text n

        // translation file notes:
        //      the file must contain a translation for the default language code
        //      the file should contain a translation for every language code supported by the base game
        //      the file may contain translations for other language codes
        //      language codes must be two characters
        //      language codes in the file can be in any order
        //      a blank line is skipped
        //      a line with a blank translation key is skipped (except the first line)
        //      a line with a translation key that starts with the character (#) is considered a comment and is skipped
        //      a translation key cannot be duplicated
        //      the file must not contain blank columns
        //      each language code, translation key, and translated text may or may not be enclosed in double quotes
        //      spaces around the comma separators will be included in the translated text
        //      to include a comma in the translated text, the translated text must be enclosed in double quotes
        //      to include a double quote in the translated text, use two double quotes inside the double quoted translated text
        //      translated text cannot be blank for the default language
        //      blank translated text in a non-default language will use the translated text for the default language

        // use singleton pattern:  there can be only one Translation in the game
        private static readonly Translation _instance = new Translation();
        public static Translation instance { get { return _instance; } }
        private Translation() { Initialize(); }

        // translation keys
        // the numeric values of the translation key enums are not important
        // only the string values of the translation key enums are used to look up translations
        // therefore, enum member names must exactly match the translation keys in the translation file
        public enum Key
        {
            // mod name and description
            Title,
            Description,

            // general options UI
            General,
            ChooseYourLanguage,
            GameLanguage,
            LanguageName,
            CurrentValueUpdateInterval,

            // in-game options UI
            InGame,
            ExportAllStatistics,
            ExportSelectedStatistics,
            ExportFile,
            SnapshotDate,
            DeleteSnapshots,
            SaveSettingsAndSnapshots,

            // main panel UI
            ExpandAll,
            CollapseAll,
            Selected,
            DeselectAll,
            YearsToShow,
            DatesToShow,
            All,
            FromTo,
            SnapshotCount,

            // graph month labels
            Month1,
            Month2,
            Month3,
            Month4,
            Month5,
            Month6,
            Month7,
            Month8,
            Month9,
            Month10,
            Month11,
            Month12,

            // category descriptions
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
            Home,
            Work,
            Visit,
            Moving,
            CitizenOnBike,
            CitizenWaiting,
            CitizenBored,
            // statistic descriptions
            // some preceding keys are also used as statistic descriptions
            None,               // has special meaning
            Consumption,
            Production,
            PumpingCapacity,
            Reserved,
            StorageCapacity,
            DrainingCapacity,
            Storage,
            Capacity,
            ProcessingCapacity,
            Elementary,
            HighSchool,
            University,
            PublicLibrary,
            Eligible,
            Users,
            Uneducated,
            Educated,
            WellEducated,
            HighlyEducated,
            Global,
            Residential,
            Commercial,
            Industrial,
            Office,
            Unzoned,
            AverageHealth,
            Sick,
            HealCapacity,
            Cemetery,
            Buried,
            Crematorium,
            Deceased,
            DeathRate,
            SickChildrenTeens,
            ChildrenTeens,
            BirthRate,
            SickSeniors,
            AverageLifeSpan,
            Average,
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            IndustrialOffice,
            Flow,
            Ground,
            DrinkingWater,
            Noise,
            Hazard,
            Rate,
            DetainedCriminals,
            JailsCapacity,
            Total,
            Bus,
            Trolleybus,
            Tram,
            Metro,
            Train,
            Ship,
            Air,
            Monorail,
            CableCar,
            Taxi,
            Children,
            Teens,
            YoungAdults,
            Adults,
            Seniors,
            Occupied,
            Available,
            PeopleEmployed,
            JobsAvailable,
            UnfilledJobs,
            Unemployment,
            Unemployed,
            EligibleWorkers,
            Import,
            Export,
            Goods,
            Forestry,
            Farming,
            Ore,
            Oil,
            Mail,
            Fish,
            Forest,
            FertileLand,
            Used,
            CityAttractiveness,
            LowWealth,
            MediumWealth,
            HighWealth,
            ExchangeStudentBonus,
            WalkingTour,
            SightseeingBus,
            Balloon,
            Income,
            Expenses,
            Profit,
            BankBalance,
            LoanBalance,
            CityValue,
            GrossDomesticProduct,
            GovernmentSpending,
            Exports,
            Imports,
            NetExports,
            LowDensity,
            HighDensity,
            SelfSufficient,
            SpecializedTotal,
            Leisure,
            OrganicAndLocalProduce,
            Generic,
            ITCluster,
            CommercialZones,
            Roads,
            WaterSewage,
            WaterSewageHeating,
            Fire,
            Emergency,
            Police,
            ParksPlazasLandscaping,
            UniqueBuildings,
            GenericSportsArenas,
            Loans,
            Policies,
            CityPark,
            AmusementPark,
            Zoo,
            NatureReserve,
            WarehousesAndFactories,
            Fishing,
            TradeSchool,
            LiberalArtsCollege,
            TollBooth,
            SpaceElevator,
            Buildings,
            Citizens,
            CitizenUnits,
            CitizenInstances,
            Disasters,
            Districts,
            Events,
            GameAreas,
            NetworkSegments,
            NetworkNodes,
            NetworkLanes,
            PaintedAreas,
            PathUnits,
            Props,
            RadioChannels,
            RadioContents,
            TransportLines,
            Trees,
            Vehicles,
            VehiclesParked,
            ZoneBlocks,

            // statistic units
            // some preceding keys are also used as statistic units
            PctOfProduction,
            MegaWatts,
            PctOfPumpingCapacity,
            CubicMetersPerWeek,
            PctOfStorageCapacity,
            CubicMeters,
            PctOfDrainingCapacity,
            PctOfCapacity,
            Units,
            PctOfProcessingCapacity,
            UnitsPerWeek,
            Students,
            Visitors,
            PctOfPopulation,
            Percent,
            PctOfHealCapacity,
            CitizensPerWeek,
            PctOfChildrenTeens,
            PctOfSeniors,
            Years,
            Level1To5,
            Level1To3,
            PctOfResidential,
            PctOfCommercial,
            PctOfIndustrial,
            PctOfOffice,
            PctOfJailsCapacity,
            TotalPerWeek,
            ResidentsPerWeek,
            TouristsPerWeek,
            PctOfAvailable,
            Squares,
            Jobs,
            HouseholdsPlusJobs,
            PctOfEligible,
            PctOfForestAvailable,
            PctOfFertileLandAvailable,
            PctPerWeekOfOreAvailable,
            PctPerWeekOfOilAvailable,
            PctOfTotal,
            Hectare,
            PctOfCityIncome,
            PctOfCityExpenses,
            MoneyPerSquareMeter,
            MoneyPerWeek,
            MoneyPerCapita,
            MoneyPerWeekPerCapita,
            PctOfGrossDomesticProduct,
            Money,
            Amount
        }

        // default language code
        // used when working with translation keys and as the language code when a translation is not present
        public const string DefaultLanguageCode = "en";    // English

        // translations for a single language
        // the dictionary key is the translation key
        // the dictionary value is the translated text for the key
        private class TranslationLanguage : Dictionary<string, string> { }

        // translations for all languages in the file
        // the dictionary key is the language code
        // the dictionary value contains the translations for the language
        private Dictionary<string, TranslationLanguage> _languages = new Dictionary<string, TranslationLanguage>();

        /// <summary>
        /// initialize the translation from the translation file
        /// </summary>
        private void Initialize()
        {
            // make sure the translation CSV file exists
            // assumes the namespace of this class is the same as the namespace of the project
            string translationFile = $"{typeof(Translation).Namespace}.Translation.csv";
            if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains(translationFile))
            {
                LogUtil.LogError($"Translation file [{translationFile}] does not exist in the assembly.");
                return;
            }

            // read the lines from the translations CSV file
            string[] lines;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(translationFile))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    lines = reader.ReadToEnd().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
                }
            }

            // translation file must contain at least one line for the language codes
            if (lines.Length < 1)
            {
                LogUtil.LogError($"Translation file must contain at least one line for language codes.");
                return;
            }

            // first line cannot be blank or a comment
            if (lines[0].Length == 0 || lines[0].StartsWith("#"))
            {
                LogUtil.LogError($"Translation file first line must contain language codes.");
                return;
            }

            // read the language codes from the first line
            List<string> languageCodes = new List<string>();
            using (StringReader reader = new StringReader(lines[0]))
            {
                // read and ignore the first value, which should be blank
                ReadCSVValue(reader);

                // read language codes
                string languageCode = ReadCSVValue(reader);
                while (languageCode.Length != 0)
                {
                    // language code must be two characters
                    if (languageCode.Length != 2)
                    {
                        LogUtil.LogError($"Translation file language code [{languageCode}] must be two characters.");
                        return;
                    }

                    // add the language code to the list
                    languageCodes.Add(languageCode);

                    // initialize empty language
                    _languages[languageCode] = new TranslationLanguage();

                    // get next language code
                    languageCode = ReadCSVValue(reader);
                }
            }

            // translations must contain default language code
            if (!_languages.ContainsKey(DefaultLanguageCode))
            {
                LogUtil.LogError($"Translation file must contain translations for default language code [{DefaultLanguageCode}].");
                return;
            }

            // process each subsequent line
            for (int i = 1; i < lines.Length; i++)
            {
                // do only non-blank lines
                string line = lines[i];
                if (line.Length > 0)
                {
                    // create a string reader on the line
                    using (StringReader reader = new StringReader(line))
                    {
                        // the first value in the line is the translation key
                        // do only non-blank, non-comment translation keys
                        string translationKey = ReadCSVValue(reader);
                        if (translationKey.Length != 0 && !translationKey.StartsWith("#"))
                        {
                            // check for duplicates
                            if (_languages[DefaultLanguageCode].ContainsKey(translationKey))
                            {
                                LogUtil.LogError($"Translation key [{translationKey}] is duplicated in translation file.");
                                return;
                            }
                            else
                            {
                                // read the translated text for each language code
                                foreach (string languageCode in languageCodes)
                                {
                                    _languages[languageCode][translationKey] = ReadCSVValue(reader);
                                }
                            }

                            // translated text for default language code cannot be blank
                            if (string.IsNullOrEmpty(_languages[DefaultLanguageCode][translationKey]))
                            {
                                LogUtil.LogError($"Translation key [{translationKey}] must not be blank for detault language [{DefaultLanguageCode}].");
                                return;
                            }
                        }
                    }
                }
            }

            // verify every key enum value has a key in the translation file
            Key[] keyEnumValues = (Key[])Enum.GetValues(typeof(Key));
            foreach (Key key in keyEnumValues)
            {
                _ = Get(key);
            }

            // verify every key in the translation file has a key enum value
            foreach (string translationKey in _languages[DefaultLanguageCode].Keys.ToArray())
            {
                bool found = false;
                foreach (Key key in keyEnumValues)
                {
                    if (translationKey == key.ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    LogUtil.LogError($"Translation key [{translationKey}] in the translation file does not have a translation key enum value.");
                }
            }
        }

        /// <summary>
        /// read a CSV value
        /// </summary>
        private string ReadCSVValue(StringReader reader)
        {
            // the value to return
            StringBuilder value = new StringBuilder();

            // read until non-quoted comma or end-of-string is reached
            bool inQuotes = false;
            int currentChar = reader.Read();
            while (currentChar != -1)
            {
                // check for double quote char
                if (currentChar == '\"')
                {
                    // check whether or not already in double quotes
                    if (!inQuotes)
                    {
                        // not already in double quotes
                        // this double quote is the start of a quoted string, don't append the double quote
                        inQuotes = true;
                    }
                    else
                    {
                        // already in double quotes, check next char
                        if (reader.Peek() == '\"')
                        {
                            // next char is double quote
                            // consume the second double quote and replace the two consecutive double quotes with one double qoute
                            reader.Read();
                            value.Append((char)currentChar);
                        }
                        else
                        {
                            // next char is not double quote
                            // this double quote is the end of a quoted string, don't append the double quote
                            inQuotes = false;
                        }
                    }
                }
                else
                {
                    // a comma not in double quotes ends the value, don't append the comma
                    if (currentChar == ',' && !inQuotes)
                    {
                        break;
                    }

                    // all other cases, append the char
                    value.Append((char)currentChar);
                }

                // get next char
                currentChar = reader.Read();
            }

            // return the value
            return value.ToString();
        }

        /// <summary>
        /// supported language codes, sorted by language code
        /// </summary>
        public string[] SupportedLanguageCodes
        {
            get
            {
                string[] languageCodes = _languages.Keys.ToArray();
                Array.Sort(languageCodes);
                return languageCodes;
            }
        }

        /// <summary>
        /// get the translation of the key using the language selected in Options
        /// </summary>
        public string Get(Key key)
        {
            return Get(key, Options.instance.GetLanguageCode());
        }

        /// <summary>
        /// get the translation of the key using the specified language
        /// </summary>
        public string Get(Key key, string languageCode)
        {
            // if language code is not supported, then use default language code
            // this can happen when a mod is used to add a language to the game and that language is not supported by this mod
            if (!_languages.Keys.Contains(languageCode))
            {
                languageCode = DefaultLanguageCode;
            }

            // get key as a string (used often)
            string keyString = key.ToString();

            // get translated text for the language and translation key
            string translatedText =  _languages[languageCode][keyString];
            if (string.IsNullOrEmpty(translatedText))
            {
                // get translation from default language
                translatedText = _languages[DefaultLanguageCode][keyString];

                // if still blank, then use key
                if (string.IsNullOrEmpty(translatedText))
                {
                    LogUtil.LogError($"Translation is blank for default language [{DefaultLanguageCode}] and key [{keyString}] in the translation file.");
                    return keyString;
                }
            }

            // return the translated text
            return translatedText;
        }
    }
}
