using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Timers;

namespace TweetFlow.Stream.Watch
{
    public class StreamWatch
    {
        public Timer timer;
        public bool RestartInProgress;
        private ILogger<StreamWatch> logger;
        public event EventHandler RestartNow;
        public bool Subscribed { get; set; }
        private int tryRestartIntervalInSeconds = 30;

        public StreamWatch(ILogger<StreamWatch> logger)
        {
            this.logger = logger;
            this.timer = new Timer();
            this.timer.Elapsed += new ElapsedEventHandler(InvokeElapsed);
        }

        public void Start()
        {
            this.timer.Enabled = true;
            this.timer.Interval = this.tryRestartIntervalInSeconds;
        }

        public StreamWatch SetTryRestartInterval(int retrySeconds)
        {
            this.tryRestartIntervalInSeconds = retrySeconds;
            return this;
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
}
