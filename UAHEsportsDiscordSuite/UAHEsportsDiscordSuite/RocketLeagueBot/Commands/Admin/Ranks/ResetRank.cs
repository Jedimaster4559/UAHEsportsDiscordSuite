using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace UAHEsportsDiscordSuite.RocketLeagueBot.Commands.Admin.Ranks
{
    [Group("@reset")]
    [Alias("@Reset","@resetrank","@ResetRank")]
    [PermissionSets.BotAdmin]
    public class ResetRank : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task resetRank(IUser user = null)
        {
            if(user == null)
            {
                user = Context.User;
            }

            IGuildUser guildUser = user as IGuildUser;

            await resetUser(guildUser);

            await ReplyAsync($"{Context.User.Mention} I have reset {user.Mention}'s rank!");
        }

        public static async Task resetUser(IGuildUser user)
        {
            await Services.Ranks.RanksUser.removeRanks(user);

            await user.AddRoleAsync(Utilities.RoleHelper.getRole("Unranked/New", user.Guild));
        }
    }
}
