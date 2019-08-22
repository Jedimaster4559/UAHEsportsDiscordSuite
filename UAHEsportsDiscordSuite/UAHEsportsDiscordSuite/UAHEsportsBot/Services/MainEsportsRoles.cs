using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.UAHEsportsBot.Services
{
    public static class MainEsportsRoles
    {
        private static ulong messageID = 613933958899564563;

        public async static Task HandleRoleSelection(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            
            IUserMessage message = await arg1.DownloadAsync();

            if(message.Id != messageID)
            {
                return;
            }

            IGuildUser user = (IGuildUser)arg3.User.Value;

            await HandleLeaveAll(user);

            IRole students = Utilities.RoleHelper.getRole("Students", user.Guild);
            if (arg3.Emote.Name.Equals(":discordimage:"))
            {
                await user.AddRoleAsync(students);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been to the role ***{students.Name}***."));
            }

            IRole alumni = Utilities.RoleHelper.getRole("Alumni", user.Guild);
            if (arg3.Emote.Name.Equals(":older_man:"))
            {
                await user.AddRoleAsync(Utilities.RoleHelper.getRole("Alumni", user.Guild));
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{alumni.Name}***."));
            }

            IRole guests = Utilities.RoleHelper.getRole("Guests", user.Guild);
            if (arg3.Emote.Name.Equals(":sunglasses:")) //placeholder text
            {
                await user.AddRoleAsync(guests);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{guests.Name}***."));
            }
        }

        private async static Task HandleLeaveAll(IGuildUser user)
        {

            IRole students = Utilities.RoleHelper.getRole("Students", user.Guild);
            if (user.RoleIds.Contains(students.Id))
            {
                await user.RemoveRoleAsync(students);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{students.Name}***."));
            }

            IRole guests = Utilities.RoleHelper.getRole("Guests", user.Guild);
            if (user.RoleIds.Contains(guests.Id))
            {
                await user.RemoveRoleAsync(guests);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{guests.Name}***."));
            }

            IRole alumni = Utilities.RoleHelper.getRole("Alumni", user.Guild);
            if (user.RoleIds.Contains(alumni.Id))
            {
                await user.RemoveRoleAsync(alumni);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{alumni.Name}***."));
            }
        }

        public async static Task RemoveRoleSelection(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            IUserMessage message = await arg1.DownloadAsync();

            if (message.Id != messageID)
            {
                return;
            }


            IGuildUser user = (IGuildUser)arg3.User.Value;

            IRole students = Utilities.RoleHelper.getRole("Students", user.Guild);
            if (arg3.Emote.Name.Equals(":discordimage:"))
            {
                await user.RemoveRoleAsync(students);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{students.Name}***."));
            }

            IRole guests = Utilities.RoleHelper.getRole("Guests", user.Guild);
            if (arg3.Emote.Name.Equals(":older_man:"))
            {
                await user.RemoveRoleAsync(guests);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{guests.Name}***."));
            }

            IRole alumni = Utilities.RoleHelper.getRole("Alumni", user.Guild);
            if (arg3.Emote.Name.Equals(":sunglasses:")) //Placeholder
            {
                await user.RemoveRoleAsync(alumni);
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(($"{user.Mention} You have been removed from the role ***{alumni.Name}***."));
            }
        }
    }
}
