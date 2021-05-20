﻿using System;

namespace GodsEye.RemoteWorker.Workers.Messages.Components
{
    public class JobSummary
    {
        public string JobHashId { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string SearchedImage { get; set; }
    }
}
