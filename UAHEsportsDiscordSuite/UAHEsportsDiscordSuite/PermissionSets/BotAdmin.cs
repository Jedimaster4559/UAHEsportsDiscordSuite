using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.PermissionSets
{
    public class BotAdmin : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User.Id == 320286709063614467
                || isAdmin(context.User as IGuildUser)
                || await context.Guild.GetOwnerAsync() == context.User as IGuildUser)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                await context.Channel.SendMessageAsync($"{context.User.Mention} This command can only be run by server admins.");
                return PreconditionResult.FromError("You must be a server admin to run this command.");
            }
        }

        private bool isAdmin(IGuildUser user)
        {
            IGuild guild = user.Guild;
            var roles = guild.Roles;
            var userRoles = user.RoleIds;
            
            foreach(IRole role in roles)
            {
                if(role.Permissions.Administrator && userRoles.Contains(role.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
