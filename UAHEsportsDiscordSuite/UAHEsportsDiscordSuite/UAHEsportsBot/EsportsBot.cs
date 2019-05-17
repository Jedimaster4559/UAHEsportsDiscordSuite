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

            await SubscribeToEvents();

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

        public async Task SubscribeToEvents()
        {
            _client.MessageReceived += HandleCommandAsync;

            _client.UserVoiceStateUpdated += UpdateVoiceChannels;
        }

        /// <summary>
        /// Subscribe to received messages and register all of the commands
        /// </summary>
        /// <returns></returns>
        public async Task RegisterCommandsAsync()
        {

            // Add Commands to this by adding a new line with the Class between the <>
            await _commands.AddModuleAsync<Commands.Ping>(_services);
            await _commands.AddModuleAsync<VoiceCommands.Voice>(_services);

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

        private static async Task UpdateVoiceChannels(SocketUser su, SocketVoiceState oldState, SocketVoiceState newState)
        {
            string[] whitelist = { "General", "Meeting Room", "Tutoring" }; // List of channels not to modify
            if (newState.VoiceChannel == oldState.VoiceChannel) return; // The event can be triggered without changing channels

            // Handle joining a channel
            if (newState.VoiceChannel != null && whitelist.Contains(VoiceCommands.Voice.ProcessName(newState.VoiceChannel.Name).name)) // Check to see if a channel was joined and that channel is not in the blacklist
            {
                IGuildChannel vc = newState.VoiceChannel; // 
                var vct = VoiceCommands.Voice.ProcessName(vc.Name);
                Console.WriteLine($"{su.Username}#{su.Discriminator} has joined {vc.Name}({vc.Id})");

                var users = (await vc.GetUsersAsync().FlattenAsync()).ToList();

                if (users.Count == 1) // If there is only one person in the group, it was previously empty, and another slot needs opened up
                {
                    var pos = 0;
                    var number = 1;
                    var vcs = await vc.Guild.GetVoiceChannelsAsync(); // Get a list of voice channels
                    var vcso = vcs.OrderBy(item => item.Position); // Order the list of voice channels
                    foreach (var channel in vcso) // Loop over every channel
                    {
                        var chnl = VoiceCommands.Voice.ProcessName(channel.Name); // Get the name and number from the channel
                        if (chnl.name != vct.name) continue; // Check to see if the channel is of the same type
                        pos = Math.Max(channel.Position, pos); // If the channel is lower, save the new position
                        if (chnl.number != number)
                        { // Is the channel number correct
                            await ((SocketVoiceChannel)channel).ModifyAsync(x =>
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                x.Name = chnl.name + " " + number; // Update the channel number
                            });
                        }
                        number++; // Increment the number that the channel is supposed to be
                    }

                    var rcpe = (await vc.Guild.GetVoiceChannelsAsync()).Select(channel => new ReorderChannelProperties(channel.Id, channel.Position + (channel.Position > pos ? 1 : 0))).ToList(); // Enumerable to store the new channel order


                    var nvc = await vc.Guild.CreateVoiceChannelAsync(vct.name + " " + number); // Create the channel
                    Console.WriteLine($"Created channel {nvc.Name}({nvc.Id})");
                    await nvc.ModifyAsync(x =>
                    {
                        x.CategoryId = ((INestedChannel)vc).CategoryId;
                        x.Position = pos + 1; // Set the position of the channel
                    });
                    rcpe.Add(new ReorderChannelProperties(nvc.GuildId, pos + 1)); // Add the channel to the list of channels to reorder

                    await vc.Guild.ReorderChannelsAsync(rcpe); // Reorder the channels
                }
            }

            // Handle leaving a channel
            if (oldState.VoiceChannel != null && whitelist.Contains(VoiceCommands.Voice.ProcessName(oldState.VoiceChannel.Name).name))
            {
                IGuildChannel vc = oldState.VoiceChannel;
                var vct = VoiceCommands.Voice.ProcessName(vc.Name);
                Console.WriteLine($"{su.Username}#{su.Discriminator} has left {vc.Name}({vc.Id})");

                var users = (await vc.GetUsersAsync().FlattenAsync()).ToList();

                if (!users.Any()) // If there are no users in the channel
                {
                    Console.WriteLine($"Removing empty channel {vc.Name}({vc.Id})");
                    await vc.DeleteAsync(); // Delete it

                    var number = 1;
                    var vcs = await vc.Guild.GetVoiceChannelsAsync();
                    var vcso = vcs.OrderBy((item) => item.Position);

                    // This functions almost identically to the number adjustment in the join
                    foreach (var channel in vcso)
                    {
                        var chnl = VoiceCommands.Voice.ProcessName(channel.Name);
                        if (chnl.name != vct.name) continue;
                        if (chnl.number == vct.number) continue; // Because the deleted channel is still in the list, skip it
                        if (chnl.number != number)
                        {
                            await ((SocketVoiceChannel)channel).ModifyAsync(x =>
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                x.Name = chnl.name + " " + number;
                            });
                        }
                        number++;
                    }
                }
            }
        }

    }
}
