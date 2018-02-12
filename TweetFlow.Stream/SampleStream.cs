using System.Collections.Generic;
using System.Threading.Tasks;
using TweetFlow.MemoryStore;
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
                if (args.Tweet.IsRetweet)
                {
                    return;
                }

                var tweet = new Model.Tweet
                {
                    StrId = args.Tweet.IdStr,
                    FullText = args.Tweet.FullText,
                    CreatedAt = args.Tweet.CreatedAt,
                    FavoriteCount = args.Tweet.FavoriteCount,
                    Favorited = args.Tweet.Favorited,
                    QuoteCount = 0,
                    ReplyCount = 0,
                    RetweetCount = args.Tweet.RetweetCount,
                    IsRetweet = args.Tweet.IsRetweet,
                    User = new Model.User
                    {
                        CreatedAt = args.Tweet.CreatedBy.CreatedAt,
                        FavouritesCount = args.Tweet.CreatedBy.FavouritesCount,
                        FollowersCount = args.Tweet.CreatedBy.FollowersCount,
                        FriendsCount = args.Tweet.CreatedBy.FriendsCount,
                        ListedCount = args.Tweet.CreatedBy.ListedCount,
                        StatusesCount = args.Tweet.CreatedBy.StatusesCount,
                        Verified = args.Tweet.CreatedBy.Verified,
                        ProfileImageUrl400x400 = args.Tweet.CreatedBy.ProfileImageUrl400x400,
                        ScreenName = args.Tweet.CreatedBy.ScreenName,
                        Name = args.Tweet.CreatedBy.Name
                    }
                };

                this.Queue.Add(new ScoredItem(tweet));
            };
            await this.filteredStream.StartStreamMatchingAllConditionsAsync();
            return this;
        }
    }
}
