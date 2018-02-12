﻿namespace TweetFlow.Stream
{
    public interface ICredentials
    {
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string AccessToken { get; set; }
        string AccessTokenSecret { get; set; }
    }
}
