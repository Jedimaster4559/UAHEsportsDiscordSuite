using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.RocketLeagueBot.Commands.Admin.Ranks
{

    [Group("@ResetAll")]
    [Alias("@resetall")]
    [PermissionSets.BotAdmin]
    public class ResetAllUsers : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task resetAll()
        {
            var progressMessage = (await Context.Channel.SendMessageAsync(getResetStatusMessage(0))) as IUserMessage;

            var users = Context.Guild.Users;
            int totalUsers = users.Count;
            int count = 0;

            foreach(SocketGuildUser user in users)
            {
                await Services.Ranks.RanksUser.removeRanks(user as IGuildUser);
                count++;
                await progressMessage.ModifyAsync(x =>
                {
                    x.Content = getResetStatusMessage((double)count / (double)totalUsers);
                });
            }

            await progressMessage.ModifyAsync(x =>
            {
                x.Content = getResetStatusMessage(1);
            });

            await ReplyAsync($"{Context.User.Mention} The rank reset has been completed!");
        }

        private string getResetStatusMessage(double percentage)
        {
            string message = "";

            if(percentage < 1)
            {
                message += "Rank Reset in Progress...\n";
            }
            else
            {
                message += "Rank Reset has been completed";
            }

            message += "Status: " + Math.Round(percentage * 100, 2).ToString() + "%\n";
            message += "[";

            for (int i = 0; i < 10; i++)
            {
                if (Math.Floor(percentage * 10) >= i + 1)
                {
                    message += "X";
                }
                else
                {
                    message += "-";
                }
            }

            message += "]";

            return message;
        }

    }
}
