using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Stream;
using TweetFlow.StreamService;

namespace TweetFlow.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddEnvironmentVariables()
            //    .Build();

            //var credentials = new TwitterCredentials();
            //configuration.GetSection("Twitter:Credentials").Bind(credentials);


            //var provider = new ServiceCollection()
            //    .AddOptions()
            //    .AddTransient<ICredentials>(p => credentials)
            //    .AddTransient<IOrderedQueue<int, Tweet, ScoredItem>, OrderedQueue>()
            //    .AddTransient<SampleStream>()
            //    .AddTransient<StreamFactory>()
            //    .BuildServiceProvider();

            //var streamFactory = provider.GetService<StreamFactory>();

            //var stream = streamFactory.Coin(credentials);

            //stream.Queue.ContentAdded += (a, b) =>
            //{
            //    Console.WriteLine(b);
            //};
            //stream.Start();
        }
    }
        
}