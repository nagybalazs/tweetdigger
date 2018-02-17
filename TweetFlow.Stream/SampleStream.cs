using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetFlow.MemoryStore;
using TweetFlow.Model;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TweetFlow.Stream
{
    public class SampleStream
    {
        public bool IsRunning { get; set; }
        private IFilteredStream filteredStream;

        public IOrderedQueue<int, Model.Tweet, ScoredItem> Queue { get; set; }

        public SampleStream(ICredentials credentials, OrderedQueue orderedQueue)
        {
            TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetCredentials(new Tweetinvi.Models.TwitterCredentials(credentials.ConsumerKey, credentials.ConsumerSecret, credentials.AccessToken, credentials.AccessTokenSecret));
            this.Queue = orderedQueue;
            this.filteredStream = Tweetinvi.Stream.CreateFilteredStream();
        }

        public SampleStream AddTracks(params string[] tracks)
        {
            foreach(var track in tracks)
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
            foreach(var queryParam in queryParams)
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
            foreach(var language in languages)
            {
                this.AddLanguage(language);
            }
            return this;
        }

        public SampleStream AddLanguages(IEnumerable<Language> languages)
        {
            foreach(var language in languages)
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
            this.IsRunning = true;
            this.filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                this.FilterAndAddTweetToQueue(args.Tweet);
            };
            await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            return this;
        }

        private void FilterAndAddTweetToQueue(ITweet tweet)
        {
            var hashtags = tweet.Hashtags.Select(hashtag => hashtag.Text);
            foreach (var hashtag in hashtags)
            {
                if (hashtag.IsMatchToType(TweetType.Bitcoin))
                {
                    this.Queue.Add(new ScoredItem(this.CreateTweet(tweet, TweetType.Bitcoin)));
                }

                if (hashtag.IsMatchToType(TweetType.Ethereum))
                {
                    this.Queue.Add(new ScoredItem(this.CreateTweet(tweet, TweetType.Ethereum)));
                }

                if (hashtag.IsMatchToType(TweetType.Ripple))
                {
                    this.Queue.Add(new ScoredItem(this.CreateTweet(tweet, TweetType.Ripple)));
                }

                if (hashtag.IsMatchToType(TweetType.LiteCoin))
                {
                    this.Queue.Add(new ScoredItem(this.CreateTweet(tweet, TweetType.LiteCoin)));
                }
            }
        }

        private Model.Tweet CreateTweet(ITweet iTweet, TweetType type)
        {
            var tweet = new Model.Tweet
            {
                Type = type,
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
                    Name = iTweet.CreatedBy.Name
                }
            };
            return tweet;
        }
    }
}
