using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetFlow.MemoryStore;
using TweetFlow.Providers;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TweetFlow.Stream
{
    public class SampleStream
    {
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
        private IFilteredStream filteredStream;

        public OrderedQueue Queue { get; set; }
        private TweetScoreCalculator tweetScoreCalculator;
        private ILogger<SampleStream> logger;

        public SampleStream(
            ICredentials credentials, 
            OrderedQueue orderedQueue, 
            TweetScoreCalculator tweetScoreCalculator,
            ILogger<SampleStream> logger)
        {
            TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetCredentials(new Tweetinvi.Models.TwitterCredentials(credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken, credentials.AccessTokenSecret));
            this.Queue = orderedQueue;
            this.filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.logger = logger;
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

        public SampleStream AddQueryParameter(IEnumerable<QueryParam> queryParams)
        {
            foreach (var queryParam in queryParams)
            {
                this.AddQueryParameter(queryParam.Name, queryParam.Value);
            }
            return this;
        }

        public SampleStream AddQueryParameter(string name, string value)
        {
            this.filteredStream.AddCustomQueryParameter(name, value);
            return this;
        }

        public SampleStream AddLanguages(params Language[] languages)
        {
            foreach (var language in languages)
            {
                this.AddLanguage(language);
            }
            return this;
        }

        public SampleStream AddLanguages(IEnumerable<Language> languages)
        {
            foreach (var language in languages)
            {
                this.AddLanguage(language);
            }
            return this;
        }

        public SampleStream AddLanguage(Language language)
        {
            var languageFilter = (LanguageFilter)(int)language;
            this.filteredStream.AddTweetLanguageFilter(languageFilter);
            return this;
        }

        private bool subscribedOnAllEvents = false;

        public async Task<SampleStream> StartAsync()
        {
            if (!this.subscribedOnAllEvents)
            {
                this.subscribedOnAllEvents = true;
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
            }

            if(this.filteredStream.StreamState == Tweetinvi.Models.StreamState.Stop)
            {
                await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            }

            return this;
        }

        private void FilterAndAddTweetToQueue(ITweet tweet)
        {
            if (tweet.IsRetweet)
            {
                if(tweet.RetweetedTweet == null)
                {
                    return;
                }

                if(tweet.RetweetedTweet.CreatedAt < DateTime.UtcNow.AddDays(-1))
                {
                    return;
                }
            }
            var scoredItem = new ScoredItem(this.CreateTweet(tweet));
            scoredItem.SetCustomScoreCalculator(this.tweetScoreCalculator);
            var supported = new List<string> { "bitcoin", "ethereum", "ripple", "litecoin" };
            var hashtags = tweet.Hashtags.Select(hashtag => hashtag.Text.Replace("#", string.Empty).ToLower());
            foreach (var hashtag in hashtags)
            {
                var match = supported.FirstOrDefault(p => p == hashtag);
                if(match != null)
                {
                    scoredItem.Content.Type = match;
                    scoredItem.CalculateScore();
                    this.Queue.SetQueueType(scoredItem.Content.Type).Add(scoredItem);
                }
            }
        }

        private Model.Tweet CreateTweet(ITweet iTweet)
        {
            var convertedToOriginal = false;
            var createdAt = iTweet.CreatedAt;
            if (iTweet.IsRetweet)
            {
                iTweet = iTweet.RetweetedTweet;
                convertedToOriginal = true;
            }

            var tweet = new Model.Tweet
            {
                StrId = iTweet.IdStr,
                FullText = iTweet.FullText,
                CreatedAt = createdAt,
                FavoriteCount = iTweet.FavoriteCount,
                Favorited = iTweet.Favorited,
                QuoteCount = 0,
                ReplyCount = 0,
                RetweetCount = iTweet.RetweetCount,
                IsRetweet = iTweet.IsRetweet,
                Hashtags = this.ExtractHashTags(iTweet).ToList(),
                ConvertedToOriginal = convertedToOriginal,
                UserMentionsCount = iTweet.UserMentions.Count,
                User = new Model.User
                {
                    StrId = iTweet.CreatedBy.IdStr,
                    CreatedAt = iTweet.CreatedBy.CreatedAt,
                    FavouritesCount = iTweet.CreatedBy.FavouritesCount,
                    FollowersCount = iTweet.CreatedBy.FollowersCount,
                    FriendsCount = iTweet.CreatedBy.FriendsCount,
                    ListedCount = iTweet.CreatedBy.ListedCount,
                    StatusesCount = iTweet.CreatedBy.StatusesCount,
                    Verified = iTweet.CreatedBy.Verified,
                    ProfileImageUrl400x400 = iTweet.CreatedBy.ProfileImageUrl400x400,
                    ScreenName = iTweet.CreatedBy.ScreenName,
                    Name = iTweet.CreatedBy.Name,
                    Id = iTweet.CreatedBy.Id
                }
            };
            return tweet;
        }

        private IEnumerable<string> ExtractHashTags(ITweet tweet)
        {
            if(tweet == null)
            {
                return Enumerable.Empty<string>();
            }
            return tweet.Hashtags.Select(hashtag => hashtag.Text);
        }
    }
}
