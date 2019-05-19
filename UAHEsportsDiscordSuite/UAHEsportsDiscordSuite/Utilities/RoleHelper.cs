using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;

namespace UAHEsportsDiscordSuite.Utilities
{
    public class RoleHelper
    {
        public static ulong getRoleID(string roleName, SocketCommandContext Context)
        {
            IRole role = getRole(roleName, Context);
            return role.Id;
        }

        public static IRole getRole(string roleName, SocketCommandContext Context)
        {
            var roles = Context.Guild.Roles;
            roleName = roleName.ToLower();

            IRole returnRole = null;

            foreach(IRole role in roles)
            {
                if(role.Name.ToLower() == roleName)
                {
                    returnRole = role;
                    break;
                }
            }

            return returnRole;
        }
    }
}
