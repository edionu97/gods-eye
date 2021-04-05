using System;
using Nito.Collections;
using System.Collections.Generic;
using GodsEye.Utility.Application.Config.BaseConfig;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;
using GodsEye.Utility.Application.Config.Configuration.Sections.RemoteWorker;
using Microsoft.Extensions.Logging;

namespace GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer.Impl
{
    public class ReverseOrderFrameBuffer : IFrameBuffer
    {
        private readonly object _lockObject = new object();

        private readonly int _maxBufferSize;

        private readonly Deque<(DateTime, NetworkImageFrameMessage)> _frameBuffer 
            = new Deque<(DateTime, NetworkImageFrameMessage)>();

        private readonly ILogger<ReverseOrderFrameBuffer> _logger;

        public ReverseOrderFrameBuffer(IConfig config, ILogger<ReverseOrderFrameBuffer> logger)
        {
            //destruct the frame buffer
            var (_, bufferSize) = config
                .Get<RemoteWorkerSectionConfig>();

            _logger = logger;

            //set the buffer size
            _maxBufferSize = bufferSize;
        }

        public void PushFrame(NetworkImageFrameMessage frame)
        {
            //critical section
            lock (_lockObject)
            {
                //if we fill the queue we remove one frame (the older one)
                if (_frameBuffer.Count == _maxBufferSize)
                {
                    _frameBuffer.RemoveFromBack();
                }

                //always keep the frames ordered by timestamp
                _frameBuffer.AddToFront((DateTime.UtcNow, frame));
            }
        }

        public Queue<(DateTime, NetworkImageFrameMessage)> TakeASnapshot()
        {
            //declare the queue
            Queue<(DateTime, NetworkImageFrameMessage)> frameQueue;

            //extract the frame buffer buffer
            lock (_lockObject)
            {
                frameQueue = new Queue<(DateTime, NetworkImageFrameMessage)>(_frameBuffer);
            }

            return frameQueue;
        }
    }
}
