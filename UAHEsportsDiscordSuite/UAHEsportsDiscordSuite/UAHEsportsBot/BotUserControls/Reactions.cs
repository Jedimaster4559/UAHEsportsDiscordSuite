using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace UAHEsportsDiscordSuite.UAHEsportsBot.BotUserControls
{
    [Group("React")]
    [Alias("react")]
    [PermissionSets.BotOwnerPermissions]
    public class Reactions : ModuleBase<SocketCommandContext>
    {
        [Command("Add")]
        [Alias("add")]
        public async void add(ulong messageID, IEmote emoji)
        {
            IUserMessage message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;
            await message.AddReactionAsync(emoji);
            await ReplyAsync($"{Context.User.Mention} Your reaction has been added!");
        }
    }
}
