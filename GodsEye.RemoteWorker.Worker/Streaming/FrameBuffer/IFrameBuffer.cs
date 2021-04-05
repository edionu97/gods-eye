using System;
using System.Collections.Generic;
using GodsEye.Utility.Application.Items.Messages.CameraToWorker;

namespace GodsEye.RemoteWorker.Worker.Streaming.FrameBuffer
{
    public interface IFrameBuffer
    {
        /// <summary>
        /// Inserts a frame into the buffer
        /// </summary>
        /// <param name="frame">the frame that will be inserted</param>
        public void PushFrame(NetworkImageFrameMessage frame);

        /// <summary>
        /// This method will create a snapshot of the current buffer
        /// </summary>
        /// <returns>a queue of tuples (1, 2)
        ///     1. the timestamp in which the frame entered into the buffer
        ///     2. the frame itself
        /// </returns>
        public Queue<(DateTime, NetworkImageFrameMessage)> TakeASnapshot();
    }
}
