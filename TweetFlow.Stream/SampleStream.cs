using System;
using Tweetinvi;
using System.Linq;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using TweetFlow.MemoryStore;
using System.Threading.Tasks;
using TweetFlow.Stream.Factory;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TweetFlow.Stream
{
    public class SampleStream
    {
        private IEnumerable<string> channelNames;
        private bool subscribedOnAllEvents = false;
        private IFilteredStream filteredStream;
        private ILogger<SampleStream> logger;
        private ChannelFactory channelFactory;
        private TweetScoreCalculator tweetScoreCalculator;
        private StreamCredentials streamCredentials;

        public OrderedQueue Queue { get; set; }
        public EventHandler<Guid> Stopped;

        public bool IsStarted
        {
            get
            {
                return this.CurrentState == StreamState.Running;
            }
        }

        public StreamState CurrentState
        {
            get
            {
                var intVal = this.filteredStream.StreamState;
                return (StreamState)intVal;
            }
        }


        public SampleStream(
            ChannelFactory channelFactory,
            StreamCredentials streamCredentials, 
            OrderedQueue orderedQueue, 
            TweetScoreCalculator tweetScoreCalculator,
            ILogger<SampleStream> logger)
        {
            this.channelNames = channelFactory.ChannelNames;
            this.Queue = orderedQueue;
            this.streamCredentials = streamCredentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.logger = logger;
            this
                .Configure()
                .SubscribeOnAllStreamEvents();
        }

        private SampleStream Configure()
        {
            TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetCredentials(new TwitterCredentials(this.streamCredentials.ConsumerKey, this.streamCredentials.ConsumerSecret, this.streamCredentials.AccessToken, this.streamCredentials.AccessTokenSecret));
            this.filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            return this;
        }

        private SampleStream SubscribeOnAllStreamEvents()
        {
            this.filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                try
                {
                    this.FilterAndAddTweetToQueue(args.Tweet);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"Error in filtering: {this.filteredStream.StreamState.AsStr()}.");
                }
            };

            this.filteredStream.StreamStopped += (sender, args) =>
            {
                var message = args.DisconnectMessage == null ? "Internal error." : args.DisconnectMessage.Reason;
                var code = args.DisconnectMessage == null ? "0" : args.DisconnectMessage.Code.ToString();

                this.logger.LogError(args.Exception,
                    $"Stream stopped: {this.filteredStream.StreamState.AsStr()}. " +
                    $"Error message: {message} (Code: {code}).");

                this.Stopped.Invoke(this, Guid.NewGuid());
            };

            this.filteredStream.StreamStarted += (sender, args) =>
            {
                this.logger.LogInformation($"Stream started: {this.filteredStream.StreamState.AsStr()}");
            };

            this.filteredStream.DisconnectMessageReceived += (sender, args) =>
            {
                this.logger.LogInformation(
                    $"Disconnect message received: {this.filteredStream.StreamState.AsStr()}. " +
                    $"Disconnect message: {args.DisconnectMessage.Reason} (Code: {args.DisconnectMessage.Code}).");
            };

            this.filteredStream.LimitReached += (sender, args) =>
            {
                this.logger.LogInformation(
                    $"Limit reached message received: {this.filteredStream.StreamState.AsStr()}. " +
                    $"Number of tweet not received: {args.NumberOfTweetsNotReceived}");
            };

            this.filteredStream.WarningFallingBehindDetected += (sender, args) =>
            {
                this.logger.LogInformation(
                    $"Warning failling behind detected message received: {this.filteredStream.StreamState.AsStr()}. " +
                    $"Warning message: {args.WarningMessage.Message} (Code: {args.WarningMessage.Code}).");
            };

            return this;
        }

        public SampleStream AddTracks(params string[] tracks)
        {
            foreach (var track in tracks)
            {
                this.AddTrack(track);
            }
            return this;
        }

        public SampleStream AddTrack(string track)
        {
            this.filteredStream.AddTrack(track);
            return this;
        }

        public SampleStream AddQueryParameter(string name, string value)
        {
            this.filteredStream.AddCustomQueryParameter(name, value);
            return this;
        }

        public SampleStream AddLanguage(Language language)
        {
            var languageFilter = (LanguageFilter)(int)language;
            this.filteredStream.AddTweetLanguageFilter(languageFilter);
            return this;
        }

        public async Task<SampleStream> StartAsync()
        {
            if(this.filteredStream.StreamState == Tweetinvi.Models.StreamState.Stop)
            {
                await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            }

            return this;
        }

        private void FilterAndAddTweetToQueue(ITweet tweet)
        {
            if (tweet.IsInvalidRetweet())
            {
                return;
            }

            var scoredItem = tweet
                .ToScoredItem()
                .SetCustomScoreCalculator(this.tweetScoreCalculator);

            var hashtags = tweet.ExtractHashTagsAsQueueType();
            foreach (var hashtag in hashtags)
            {
                var match = this.channelNames.FirstOrDefault(p => p == hashtag);
                if(match != null)
                {
                    scoredItem.Content.Type = match;
                    scoredItem.CalculateScore();
                    this.Queue.SetQueueType(scoredItem.Content.Type).Add(scoredItem);
                }
            }
        }
    }
}
