using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace TweetFlow.Stream
{
    public class StreamWatch
    {
        public List<RestartValue> RestartValues { get; set; }
        public Timer timer;
        private ILogger<StreamWatch> logger;
        public event EventHandler RestartNow;
        public bool RestartInProgress;
        public bool Subscribed { get; set; }

        public StreamWatch(ILogger<StreamWatch> logger)
        {
            this.logger = logger;
            this.timer = new Timer();
            this.RestartValues = new List<RestartValue>();
            this.timer.Elapsed += new ElapsedEventHandler(InvokeElapsed);
        }

        public void Start()
        {
            this.timer.Enabled = true;
            this.timer.Interval = (30 * 1000);
        }

        public void Stop()
        {
            this.timer.Enabled = false;
        }

        public void InvokeElapsed(object source, ElapsedEventArgs elapsedEventArgs)
        {
            this.RestartInProgress = false;
            this.RestartNow?.Invoke(null, null);
        }
    }

    public class RestartValue
    {
        public int Exponential { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}
