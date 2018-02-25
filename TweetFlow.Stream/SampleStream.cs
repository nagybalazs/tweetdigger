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
        public EventHandler<StreamState> Rekt;
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
        private TWStreamInfoProvider tWStreamInfoProvider;

        public SampleStream(
            ICredentials credentials, 
            OrderedQueue orderedQueue, 
            TweetScoreCalculator tweetScoreCalculator,
            TWStreamInfoProvider tWStreamInfoProvider)
        {
            TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetCredentials(new Tweetinvi.Models.TwitterCredentials(credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken, credentials.AccessTokenSecret));
            this.tWStreamInfoProvider = tWStreamInfoProvider;
            this.Queue = orderedQueue;
            this.filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            this.tweetScoreCalculator = tweetScoreCalculator;
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

        public async Task<SampleStream> StartAsync()
        {
            this.filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                try
                {
                    this.FilterAndAddTweetToQueue(args.Tweet);
                }
                catch(Exception ex)
                {
                    var extText = this.ExtractExceptionFullMessge(ex);
                    tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                    {
                        EventTypeEnum = DatabaseModel.StreamInfoEventType.HandledException,
                        OccuredAt = DateTime.UtcNow,
                        ExceptionMessage = extText,
                        Reason = null
                    });
                }
            };

            this.filteredStream.StreamStopped += (sender, args) =>
            {
                var fullMessage = string.Empty;

                var ex = args.Exception;
                while(ex != null)
                {
                    fullMessage += $"EXMS: {ex.Message} EXST: {ex.StackTrace}";
                    ex = ex.InnerException;
                }

                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.StreamStopped,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = fullMessage,
                    Reason = args.DisconnectMessage?.Reason
                });

                this.Rekt.Invoke(this, StreamState.Stopped);
            };

            this.filteredStream.StreamStarted += (sender, args) =>
            {
                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.StreamStarted,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = null,
                    Reason = null
                });
            };

            this.filteredStream.DisconnectMessageReceived += (sender, args) =>
            {
                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.StreamDisconnected,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = null,
                    Reason = args.DisconnectMessage?.Reason
                });
            };

            this.filteredStream.LimitReached += (sender, args) =>
            {
                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.LimitReached,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = null,
                    Reason = $"Number of Tweets not received: {args.NumberOfTweetsNotReceived}"
                });
            };

            this.filteredStream.WarningFallingBehindDetected += (sender, args) =>
            {
                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.FallingBehind,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = null,
                    Reason = args.WarningMessage?.Message
                });
            };

            if(this.filteredStream.StreamState == Tweetinvi.Models.StreamState.Stop)
            { 
                await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            }

            return this;
        }

        private void FilterAndAddTweetToQueue(ITweet tweet)
        { 
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
            var tweet = new Model.Tweet
            {
                StrId = iTweet.IdStr,
                FullText = iTweet.IsRetweet ? iTweet.RetweetedTweet?.FullText : iTweet.FullText,
                CreatedAt = iTweet.CreatedAt,
                FavoriteCount = iTweet.FavoriteCount,
                Favorited = iTweet.Favorited,
                QuoteCount = 0,
                ReplyCount = 0,
                RetweetCount = iTweet.RetweetCount,
                IsRetweet = iTweet.IsRetweet,
                Hashtags = iTweet.IsRetweet ? this.ExtractHashTags(iTweet.RetweetedTweet).ToList() : this.ExtractHashTags(iTweet).ToList(),
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

        private string ExtractExceptionFullMessge(Exception ex)
        {
            var sb = new StringBuilder();
            while(ex != null)
            {
                sb.Append($"Exception Message: {ex.Message}").Append($"(Stack Trace: {ex.StackTrace}").Append(Environment.NewLine);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}
