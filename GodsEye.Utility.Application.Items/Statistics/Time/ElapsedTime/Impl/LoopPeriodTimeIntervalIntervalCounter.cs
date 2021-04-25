using System;
using System.Diagnostics;

namespace GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime.Impl
{
    public class LoopPeriodTimeIntervalIntervalCounter : IPeriodTimeIntervalCounter, IDisposable
    {
        public double Period { get; set; }

        public Action<int> OnPeriodExpiredCallback { get; set; }

        private TimeSpan? _startPeriodTime;

        private readonly Stopwatch _internalCounter;

        private int _timePeriodCount = 0;

        public LoopPeriodTimeIntervalIntervalCounter()
        {
            _internalCounter = Stopwatch.StartNew();
        }

        public void CountTimeInterval()
        {
            //initialize the start time
            _startPeriodTime ??= _internalCounter.Elapsed;

            //compute the elapsed time and check if the time period has expired
            var elapsedTime = _internalCounter.Elapsed - _startPeriodTime;
            if (elapsedTime.GetValueOrDefault().TotalMilliseconds <= Period)
            {
                ++_timePeriodCount;
                return;
            }

            //call the method 
            OnPeriodExpiredCallback?.Invoke(_timePeriodCount);

            //reset the start period to the elapsed time
            _timePeriodCount = 0;
            _startPeriodTime = _internalCounter.Elapsed;
        }

        public void Dispose()
        {
            _internalCounter.Stop();
        }
    }
}
