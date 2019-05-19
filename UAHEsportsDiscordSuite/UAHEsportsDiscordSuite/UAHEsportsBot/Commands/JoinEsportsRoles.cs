using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.UAHEsportsBot.Commands
{
    public class JoinEsportsRoles : ModuleBase<SocketCommandContext>
    {
        [Command("Join")]
        [Alias("join")]
        public async Task joinRole([Remainder] string roleName)
        {
            IRole role = Utilities.RoleHelper.getRole(roleName, Context);



            string[] whiteList = { "Students", "Guests", "Alumni" };

            if (!whiteList.Contains(role.Name))
            {
                await ReplyAsync($"That role is not availible with that command. Please try a different role.");
            }

            if (role == null)
            {
                await ReplyAsync($"{Context.User.Mention} The role could not be found. You have not been added to the role ***{role.Name}***.");
            }

            IGuildUser user = (IGuildUser)Context.User;

            IRole students = Utilities.RoleHelper.getRole("Students", Context);
            if (user.RoleIds.Contains(students.Id)){
                await user.RemoveRoleAsync(students);
            }

            IRole guests = Utilities.RoleHelper.getRole("Guests", Context);
            if (user.RoleIds.Contains(guests.Id))
            {
                await user.RemoveRoleAsync(students);
            }

            IRole alumni = Utilities.RoleHelper.getRole("Alumni", Context);
            if (user.RoleIds.Contains(guests.Id))
            {
                await user.RemoveRoleAsync(students);
            }

            await ((IGuildUser)Context.User).AddRoleAsync(Utilities.RoleHelper.getRole(roleName, Context));
            await ReplyAsync($"{Context.User.Mention} You have successfully been added to the role ***{role.Name}!***");

        }
    }
}
