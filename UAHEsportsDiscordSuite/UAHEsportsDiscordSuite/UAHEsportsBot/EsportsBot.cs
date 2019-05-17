using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace UAHEsportsDiscordSuite.UAHEsportsBot
{
    /// <summary>
    /// A Discord Bot for the UAH Esports Server
    /// </summary>
    public class EsportsBot
    {
        /// <summary>
        /// Starts a new Rocket League Bot and runs the bot.
        /// </summary>
        public static void runEsportsBot() => new EsportsBot().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        /// <summary>
        /// Runs the UAH Esports Bot
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, DiscordKeys.GetEsportsKey());

            await _client.StartAsync();

            await _client.SetGameAsync("!help");

            await Task.Delay(-1);
        }

        /// <summary>
        /// Log a message to the console
        /// </summary>
        /// <param name="arg">The message to log</param>
        /// <returns></returns>
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Subscribe to received messages and register all of the commands
        /// </summary>
        /// <returns></returns>
        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModuleAsync<Commands.Ping>(_services);
            //await _commands.AddModulesAsync(Assembly.Load(this.GetType().GetTypeInfo().Assembly.GetName()), _services);
        }

        /// <summary>
        /// Process the comands properly.
        /// </summary>
        /// <param name="arg">The message the contains the command</param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            var context = new SocketCommandContext(_client, message);

            if (message is null || message.Author.IsBot || message.Author.IsWebhook) return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }

    }
}
