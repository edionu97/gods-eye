using System;
using Nito.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

namespace GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer.Impl
{
    public class ReverseOrderFrameBuffer : IFrameBuffer
    {
        private readonly int _maxBufferSize;
        private readonly object _lockObject = new object();

        private readonly IPeriodTimeIntervalCounter _periodTimeIntervalCounter;
        private readonly Deque<(DateTime, NetworkImageFrameMessage)> _frameBuffer = new Deque<(DateTime, NetworkImageFrameMessage)>();

        private volatile int _inputRate;
        public int InputRate
        {
            get => _inputRate;
            private set => _inputRate = value;
        }

        public bool IsReady { get; private set; }
        public int BufferSize { get; private set; }

        public ReverseOrderFrameBuffer(IConfig config, IPeriodTimeIntervalCounter periodTimeIntervalCounter)
        {
            //destruct the frame buffer
            var (_, bufferSize) = config
                .Get<RemoteWorkerSectionConfig>();

            //set the buffer size
            _maxBufferSize = bufferSize;

            //configure the time interval counter
            _periodTimeIntervalCounter = periodTimeIntervalCounter;
            _periodTimeIntervalCounter.Period = 1000;
            _periodTimeIntervalCounter.OnPeriodExpiredCallback = value => InputRate = value;
        }

        public void PushFrame(NetworkImageFrameMessage frame)
        {
            //critical section
            lock (_lockObject)
            {
                //check if the time has expired
                _periodTimeIntervalCounter.CountTimeInterval();

                //if we fill the queue we remove one frame (the older one)
                if (_frameBuffer.Count == _maxBufferSize)
                {
                    IsReady = true;
                    _frameBuffer.RemoveFromBack();
                }

                //set the buffer size
                BufferSize = _frameBuffer.Count;

                //always keep the frames ordered by timestamp
                _frameBuffer.AddToFront((DateTime.UtcNow, frame));
            }
        }

        public Queue<(DateTime, NetworkImageFrameMessage)> TakeASnapshot()
        {
            //declare the queue
            Queue<(DateTime, NetworkImageFrameMessage)> frameList;

            //extract the frame buffer buffer
            lock (_lockObject)
            {
                frameList = new Queue<(DateTime, NetworkImageFrameMessage)>(_frameBuffer);
            }

            return frameList;
        }
    }
}
