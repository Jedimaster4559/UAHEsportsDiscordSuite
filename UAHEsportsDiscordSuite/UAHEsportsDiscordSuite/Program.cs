using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UAHEsportsDiscordSuite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeBots();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        /// <summary>
        /// Initializes all of the Discord bots as their own Tasks
        /// </summary>
        private static void InitializeBots()
        {
            // TODO: Update and Improve Cancellation handling
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken cancellationToken = source.Token;

            Task esportsBot = new Task(UAHEsportsBot.EsportsBot.runEsportsBot, cancellationToken);
            esportsBot.Start();

            Task rocketLeagueBot = new Task(RocketLeagueBot.RocketLeagueBot.runRocketLeagueBot, cancellationToken);
            rocketLeagueBot.Start();
        }

        
    }
}
