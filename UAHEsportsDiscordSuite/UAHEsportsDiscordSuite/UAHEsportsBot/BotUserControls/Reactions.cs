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
        public async Task add(ulong messageID, [Remainder]String emoji)
        {
            IUserMessage message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;
            try
            {
                var emote = Emote.Parse(emoji);
                await message.AddReactionAsync(emote);
            } catch (System.ArgumentException e)
            {
                var actualEmoji = new Emoji(emoji);
                await message.AddReactionAsync(actualEmoji);
            }
            
            await ReplyAsync($"{Context.User.Mention} Your reaction has been added!");
        }

        [Command("Clear")]
        [Alias("clear")]
        public async Task clear(ulong messageID)
        {
            IUserMessage message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;

            await message.RemoveAllReactionsAsync();
            await ReplyAsync($"{Context.User.Mention} All reactions have been removed!");
        }
    }
}
