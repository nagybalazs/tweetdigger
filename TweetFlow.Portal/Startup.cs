using Serilog;
using System.IO;
using TweetFlow.EF;
using TweetFlow.Stream;
using TweetFlow.Services;
using TweetFlow.Providers;
using TweetFlow.MemoryStore;
using TweetFlow.Stream.Watch;
using TweetFlow.Stream.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using TweetFlow.Stream.Hubs;

namespace TweetFlow.Portal
{
    public class Startup
    {
        private ChannelFactory channelFactory;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var credentials = new StreamCredentials();
            this.Configuration.GetSection("Twitter:Credentials").Bind(credentials);

            var optionsBuilder =
                new DbContextOptionsBuilder<TweetFlowContext>()
                    .UseSqlServer(this.Configuration.GetConnectionString("TweetFlowConnection"));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services
                .AddOptions()
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(opts =>
                {
                    Configuration.GetSection("OpenIdConnect").Bind(opts);
                });

            services
                .AddSignalR();

            services
                .AddTransient(builder => optionsBuilder.Options)
                .AddTransient(p => credentials)
                .AddTransient<OrderedQueue>()
                .AddTransient<TweetScoreCalculator>()
                .AddTransient<SampleStream>()
                .AddTransient<OrderedQueue>()
                .AddTransient<TWUserProvider>()
                .AddTransient<TWAccountProvider>()
                .AddTransient<AccountService>()
                .AddTransient<TweetService>()
                .AddTransient<TWTweetProvider>()
                .AddSingleton<StreamFactory>()
                .AddSingleton<StreamWatch>()
                .AddSingleton<ChannelFactory>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.channelFactory = app.ApplicationServices.GetService<ChannelFactory>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<BaseHub>("/tweets");
            })
            .UseAuthentication()
            .UseStaticFiles()
            .UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
