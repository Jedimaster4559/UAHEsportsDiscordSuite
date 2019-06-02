using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.PermissionSets
{
    public class BotOwnerPermissions : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User.Id == 320286709063614467)
                return PreconditionResult.FromSuccess();
            else
                return PreconditionResult.FromError("You must be the bot owner to run this command");
        }
    }
}
