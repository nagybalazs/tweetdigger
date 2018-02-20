using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetFlow.EF;
using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Model.Hubs;
using TweetFlow.Portal.Controllers;
using TweetFlow.Providers;
using TweetFlow.Services;
using TweetFlow.Stream;
using TweetFlow.StreamService;

namespace TweetFlow.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var credentials = new TwitterCredentials();
            this.Configuration.GetSection("Twitter:Credentials").Bind(credentials);

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
                .AddTransient<ICredentials>(p => credentials)
                .AddTransient<IOrderedQueue<int, Tweet, ScoredItem>, OrderedQueue>()
                .AddTransient<IScoredCalculator<int, Tweet>, TweetScoreCalculator>()
                .AddSingleton<StreamFactory>()
                .AddTransient<SampleStream>()
                .AddTransient<TWStreamInfoProvider>()
                .AddTransient<OrderedQueue>()
                .AddTransient<TWUserProvider>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.ApplicationServices
            //    .GetService<Subscriber>()
            //    .Bootstrap();

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
                routes.MapHub<BitCoinHub>("bitcoin");
                routes.MapHub<EthereumHub>("ethereum");
                routes.MapHub<LiteCoinHub>("litecoin");
                routes.MapHub<RippleHub>("ripple");
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
