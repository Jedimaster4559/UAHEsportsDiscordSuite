using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace UAHEsportsDiscordSuite.RocketLeagueBot
{
    /// <summary>
    /// A Discord Bot for the UAH Esports Rocket League Server
    /// </summary>
    public class RocketLeagueBot
    {
        /// <summary>
        /// Starts a new Rocket League Bot and runs the bot.
        /// </summary>
        public static void runRocketLeagueBot() => new RocketLeagueBot().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        /// <summary>
        /// Runs the Rocket League  Bot
        /// </summary>
        /// <returns></returns>
        private async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            _client.Log += Log;

            SubscribeToEvents();

            await RegisterCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, DiscordKeys.GetRocketLeagueKey());

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

        public void SubscribeToEvents()
        {
            _client.MessageReceived += HandleCommandAsync;

            _client.MessageReceived += HandleRanksAsync;

            _client.UserVoiceStateUpdated += UpdateVoiceChannels;
        }

        private async Task HandleRanksAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            var context = new SocketCommandContext(_client, message);

            if (message is null || !message.Author.Id.Equals(424398041043435520)) return;

            Embed embed = null;

            if (message.Embeds.Count > 0) embed = message.Embeds.ElementAt(0);

            Services.Ranks.RanksUser user = new Services.Ranks.RanksUser(embed, context);

            await user.parse();

            if (!user.getState()) return;

            await user.handleRanks();
        }

        /// <summary>
        /// Subscribe to received messages and register all of the commands
        /// </summary>
        /// <returns></returns>
        public async Task RegisterCommandsAsync()
        {
            // Add Commands to this by adding a new line with the Class between the <>
            await _commands.AddModuleAsync<Commands.Ping>(_services);
            await _commands.AddModuleAsync<Utilities.Voice>(_services);
            await _commands.AddModuleAsync<Utilities.VoiceHelp>(_services);
            await _commands.AddModuleAsync<Commands.Admin.Ranks.ResetAllUsers>(_services);
            await _commands.AddModuleAsync<Commands.Admin.Ranks.ResetRank>(_services);
            await _commands.AddModuleAsync<Commands.Admin.Help>(_services);

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
            if (newState.VoiceChannel == oldState.VoiceChannel) return; // The event can be triggered without changing channels

            // Handle joining a channel
            if (newState.VoiceChannel != null && Utilities.VoiceWhitelist.check(newState.VoiceChannel.Guild.Id, newState.VoiceChannel.Name)) // Check to see if a channel was joined and that channel is not in the blacklist
            {
                IGuildChannel vc = newState.VoiceChannel; // 
                var vct = Utilities.Voice.ProcessName(vc.Name);
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
                        var chnl = Utilities.Voice.ProcessName(channel.Name); // Get the name and number from the channel
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
            if (oldState.VoiceChannel != null && Utilities.VoiceWhitelist.check(oldState.VoiceChannel.Guild.Id, oldState.VoiceChannel.Name))
            {
                IGuildChannel vc = oldState.VoiceChannel;
                var vct = Utilities.Voice.ProcessName(vc.Name);
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
                        var chnl = Utilities.Voice.ProcessName(channel.Name);
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
