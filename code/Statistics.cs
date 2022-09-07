using System.Collections.Generic;
using System.IO;

namespace RealTimeStatistics
{
    /// <summary>
    /// a list of statistics
    /// </summary>
    public class Statistics : List<Statistic>
    {
        /// <summary>
        /// return the count of selected statistics
        /// </summary>
        public int CountSelected
        {
            get
            {
                int count = 0;
                foreach (Statistic statistic in this)
                {
                    if (statistic.Selected)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// return count of enabled statistics
        /// </summary>
        public int CountEnabled
        {
            get
            {
                int count = 0;
                foreach (Statistic statistic in this)
                {
                    if (statistic.Enabled)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// create UI
        /// </summary>
        public bool CreateUI()
        {
            // create the UI for each statistic
            foreach (Statistic statistic in this)
            {
                if (!statistic.CreateUI())
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
            // update the UI text for each statistic
            foreach (Statistic statistic in this)
            {
                statistic.UpdateUIText();
            }
        }

        /// <summary>
        /// update amounts
        /// </summary>
        public void UpdateAmounts(Snapshot snapshot)
        {
            // update the amount for each statistic
            foreach (Statistic statistic in this)
            {
                statistic.UpdateAmount(snapshot);
            }
        }

        /// <summary>
        /// write the statistics to the game save file
        /// </summary>
        public void Serialize(BinaryWriter writer)
        {
            // write each statistic
            foreach (Statistic statistic in this)
            {
                statistic.Serialize(writer);
            }
        }

        /// <summary>
        /// read the statistics from the game save file
        /// </summary>
        public void Deserialize(BinaryReader reader, int version)
        {
            // read each statistic
            foreach (Statistic statistic in this)
            {
                statistic.Deserialize(reader, version);
            }
        }

    }
}
