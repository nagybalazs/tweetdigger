using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Providers;
using TweetFlow.Services;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TweetFlow.Stream
{
    public class SampleStream
    {
        public bool IsStarted { get; set; }
        public StreamState CurrentState
        {
            get
            {
                var intVal = this.filteredStream.StreamState;
                return (StreamState)intVal;
            }
        }
        private IFilteredStream filteredStream;

        public IOrderedQueue<int, Model.Tweet, ScoredItem> Queue { get; set; }
        private IScoredCalculator<int, Model.Tweet> tweetScoreCalculator;
        private TWStreamInfoProvider tWStreamInfoProvider;

        public SampleStream(
            ICredentials credentials, 
            OrderedQueue orderedQueue, 
            IScoredCalculator<int, Model.Tweet> tweetScoreCalculator,
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
            this.IsStarted = true;
            this.filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                this.FilterAndAddTweetToQueue(args.Tweet);
            };

            this.filteredStream.StreamStopped += (sender, args) =>
            {
                tWStreamInfoProvider.Add(new DatabaseModel.TWStreamInfo
                {
                    EventTypeEnum = DatabaseModel.StreamInfoEventType.StreamStopped,
                    OccuredAt = DateTime.UtcNow,
                    ExceptionMessage = args.Exception?.Message,
                    Reason = args.DisconnectMessage.Reason
                });
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

            await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            return this;
        }

        private void FilterAndAddTweetToQueue(ITweet tweet)
        { 
            var scoredItem = new ScoredItem(this.CreateTweet(tweet));
            scoredItem.SetCustomScoreCalculator(this.tweetScoreCalculator);
            scoredItem.CalculateScore();

            var hashtags = tweet.Hashtags.Select(hashtag => hashtag.Text);
            foreach (var hashtag in hashtags)
            {
                if (hashtag.IsMatchToType(TweetType.Bitcoin))
                {
                    scoredItem.Content.Type = TweetType.Bitcoin;
                    this.Queue.Add(scoredItem);
                }

                if (hashtag.IsMatchToType(TweetType.Ethereum))
                {
                    scoredItem.Content.Type = TweetType.Ethereum;
                    this.Queue.Add(scoredItem);
                }

                if (hashtag.IsMatchToType(TweetType.Ripple))
                {
                    scoredItem.Content.Type = TweetType.Ripple;
                    this.Queue.Add(scoredItem);
                }

                if (hashtag.IsMatchToType(TweetType.LiteCoin))
                {
                    scoredItem.Content.Type = TweetType.LiteCoin;
                    this.Queue.Add(scoredItem);
                }
            }
        }

        private Model.Tweet CreateTweet(ITweet iTweet)
        {
            var tweet = new Model.Tweet
            {
                StrId = iTweet.IdStr,
                FullText = iTweet.FullText,
                CreatedAt = iTweet.CreatedAt,
                FavoriteCount = iTweet.FavoriteCount,
                Favorited = iTweet.Favorited,
                QuoteCount = 0,
                ReplyCount = 0,
                RetweetCount = iTweet.RetweetCount,
                IsRetweet = iTweet.IsRetweet,
                Hashtags = iTweet.Hashtags.Select(hashtag => hashtag.Text).ToList(),
                User = new Model.User
                {
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
    }
}
