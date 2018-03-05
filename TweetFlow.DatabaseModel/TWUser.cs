using System;

namespace TweetFlow.DatabaseModel
{
    public class TWUser
    {
        public int Id { get; set; }
        public long TwitterId { get; set; }
        public int HashtagBannCount { get; set; }
        public int WordBannCount { get; set; }
        public int UserMentionBannCount { get; set; }
    }
}
