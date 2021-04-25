using System;

namespace GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime
{
    public interface IPeriodTimeIntervalCounter
    {
        public double Period { get; set; }

        /// <summary>
        /// This represents the callback that will be called when the time period expires
        /// It is called on the current thread
        /// </summary>
        public Action<int> OnPeriodExpiredCallback { get; set; }

        /// <summary>
        /// This method it is used for time advancing from one time period to another
        /// It increments the number of passing time intervals
        /// </summary>
        public void CountTimeInterval();
    }
}
