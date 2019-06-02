using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace UAHEsportsDiscordSuite.Utilities
{

    public class Voice : ModuleBase<SocketCommandContext>
    {
        [Command("lock")]
        public async Task LockAsync()
        {
            var user = Context.User;
            var vc = (user as IVoiceState)?.VoiceChannel;
            if (vc != null)
            { // Make sure the users is actually in a voice channel

                if (!VoiceWhitelist.check(Context.Guild.Id, vc.Name))
                {
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} That channel is not available through that command");
                    return;
                }

                var count = (await vc.GetUsersAsync().FlattenAsync()).Count(); // Get number of users in the current voice channel
                await vc.ModifyAsync(x =>
                {
                    x.UserLimit = count; // Set the user limit of the channel equal to the current number of users
                });
                Console.WriteLine($"{user.Username}#{user.Discriminator} locked {vc.Name} with {count} users");
                await Context.Channel.SendMessageAsync($"{vc.Name} has been locked by " + user.Mention);
            }
            else
            {
                await Context.Channel.SendMessageAsync("You must be in a voice channel to perform this command");
            }
        }

        [Command("unlock")]
        public async Task UnLockAsync()
        {
            var user = Context.User;
            var vc = ((IVoiceState)user).VoiceChannel;
            if (vc != null)
            {
                await vc.ModifyAsync(x =>
                {
                    x.UserLimit = 0;
                });
                Console.WriteLine($"{user.Username}#{user.Discriminator} unlocked {vc.Name}");
                await Context.Channel.SendMessageAsync($"{vc.Name} has been unlocked by " + user.Mention);
            }
            else
            {
                await Context.Channel.SendMessageAsync("You must be in a voice channel to perform this command");
            }
        }


        // Takes a voice channel name and returns the base name, and the number of the channel
        public static VoiceChannelName ProcessName(string name)
        {
            var parts = name.Split(' '); // Separate the name at spaces and storing into an array\
            var cat = string.Join(" ", parts.Take(parts.Length - 1)); // Joins all parts except the last back into a string
            var isNum = int.TryParse(parts[parts.Length - 1], out var num); // Attempts to convert the last part to an integer
            if (isNum) // Check if the last part is an integer
            {
                return new VoiceChannelName(cat, num);
            }
            cat = cat + " " + (parts[parts.Length - 1]); // The last part is not a number so add it back on
            return new VoiceChannelName(cat.Trim(), 0);
        }

    }

    public class VoiceChannelName
    {
        public string name { get; set; }
        public int number { get; set; }

        public VoiceChannelName(string name, int number)
        {
            this.name = name;
            this.number = number;
        }
    }
}
