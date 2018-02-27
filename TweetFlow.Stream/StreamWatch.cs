using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace TweetFlow.Stream
{
    public class StreamWatch
    {
        public List<RestartValue> RestartValues { get; set; }
        public Timer timer;

        private EventHandler restartNow;
        public event EventHandler RestartNow;

        public bool RestartInProgress;
        public bool Subscribed { get; set; }

        public StreamWatch()
        {
            this.timer = new Timer();
            this.RestartValues = new List<RestartValue>();
        }

        public void Restart()
        {
            this.RestartInProgress = true;
            if (this.RestartValues.Count == 0)
            {
                this.RestartValues.Add(new RestartValue { Exponential = 1, OccuredAt = DateTime.UtcNow });
            }

            var exponential = this.RestartValues.MaxBy(p => p.Exponential);
            if(exponential.OccuredAt < DateTime.UtcNow.AddSeconds(-1 * exponential.Exponential))
            {
                this.RestartValues = new List<RestartValue>();
                this.RestartValues.Add(new RestartValue { Exponential = 1, OccuredAt = DateTime.UtcNow });
            }

            this.timer.Enabled = true;
            this.timer.Interval = (exponential.Exponential * 1000) * 2;
            this.timer.Elapsed -= new ElapsedEventHandler(InvokeElapsed);
            this.timer.Elapsed += new ElapsedEventHandler(InvokeElapsed);
            this.RestartValues.Add(new RestartValue { Exponential = (exponential.Exponential * 2), OccuredAt = DateTime.UtcNow });
        }

        public void InvokeElapsed(object source, ElapsedEventArgs elapsedEventArgs)
        {
            this.RestartInProgress = false;
            this.RestartNow?.Invoke(null, null);
        }

        public void Kill()
        {
            this.RestartInProgress = false;
            this.RestartValues = new List<RestartValue>();
            this.timer.Enabled = false;
            this.timer.Elapsed -= new ElapsedEventHandler(InvokeElapsed);
        }
    }

    public class RestartValue
    {
        public int Exponential { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}
