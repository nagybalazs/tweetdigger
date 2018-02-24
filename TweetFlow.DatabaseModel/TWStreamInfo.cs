using System;

namespace TweetFlow.DatabaseModel
{
    public class TWStreamInfo
    {
        public int EventType { get; private set; }
        public int Id { get; set; }
        public StreamInfoEventType EventTypeEnum
        {
            get
            {
                if(Enum.IsDefined(typeof(StreamInfoEventType), this.EventType))
                {
                    return (StreamInfoEventType)this.EventType;
                }
                throw new ArgumentOutOfRangeException($"Value {this.EventType} does not exist.");
            }
            set
            {
                this.EventType = (int)value;
            }
        }
        public string Reason { get; set; }
        public string ExceptionMessage { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}
