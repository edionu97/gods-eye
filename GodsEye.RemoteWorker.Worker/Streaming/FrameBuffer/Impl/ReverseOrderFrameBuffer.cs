using System;
using Nito.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.Utility.Application.Items.Statistics.Time.ElapsedTime;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;

using Constants = GodsEye.Utility.Application.Items.Constants.Message.MessageConstants.FrameBuffer;

namespace GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer.Impl
{
    public class ReverseOrderFrameBuffer : IFrameBuffer
    {
        private int _maxBufferSize;
        private readonly object _lockObject = new object();

        private readonly ILogger<IFrameBuffer> _logger;
        private readonly FrameBufferConfig _frameBufferConfig;
        private readonly IPeriodTimeIntervalCounter _periodTimeIntervalCounter;
        private readonly Deque<(DateTime, NetworkImageFrameMessage)> _frameBuffer = new Deque<(DateTime, NetworkImageFrameMessage)>();

        private volatile int _inputRate;
        public int InputRate
        {
            get => _inputRate;
            private set
            {
                _inputRate = value;
                OnInputRateHasChangedMessage();
            }
        }

        public bool IsReady { get; private set; }
        public int BufferSize { get; private set; }

        public ReverseOrderFrameBuffer(
            IConfig config,
            IPeriodTimeIntervalCounter periodTimeIntervalCounter,
            ILogger<IFrameBuffer> logger)
        {
            //set up the logger
            _logger = logger;

            //get the frame buffer config
            _frameBufferConfig = config.Get<FrameBufferConfig>();

            //set the buffer size
            _maxBufferSize = _frameBufferConfig.BufferSize;

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

        private void OnInputRateHasChangedMessage()
        {
            //if the buffer is not configured as auto adjustable then do nothing
            if (_frameBufferConfig.BufferBehaviour != BufferBehaviourType.Dynamic)
            {
                return;
            }

            //check if the buffer size should be updated
            var newSize = _inputRate * _inputRate;
            if (_maxBufferSize >= newSize)
            {
                return;
            }

            //log the message
            _logger.LogInformation(string
                .Format(Constants
                    .FrameBufferAutoAdjustedMessage, 
                    _maxBufferSize, 
                    _inputRate * _inputRate, _inputRate));

            //update the buffer size
            _maxBufferSize = Math.Max(_maxBufferSize, _inputRate * _inputRate);
        }
    }
}
