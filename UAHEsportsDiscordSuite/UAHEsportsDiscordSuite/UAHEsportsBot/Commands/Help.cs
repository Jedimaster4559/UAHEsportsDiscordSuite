using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;

namespace UAHEsportsDiscordSuite.UAHEsportsBot.Commands
{
    [Group("Help")]
    [Alias("help")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {
            EmbedBuilder embed = getHelpEmbed();

            string message = "";
            message += "!join <role>\n";
            message += "!leave\n";
            message += "!help <command>\n";

            EmbedFieldBuilder field = new EmbedFieldBuilder();
            field.WithName("Available Commands:");
            field.WithValue(message);

            EmbedFieldBuilder additionalHelp = new EmbedFieldBuilder();
            additionalHelp.WithName("For more help, type type the help command with a specific Command");
            additionalHelp.WithValue("Ex) !help join");

            embed.AddField(field);
            embed.AddField(additionalHelp);

            await send(embed);
        }

        [Command("Leave")]
        [Alias("leave")]
        public async Task leaveHelp()
        {
            EmbedBuilder embed = getHelpEmbed("!leave");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command removes all command based roles from the user.");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue("!leave");

            embed.AddField(function);
            embed.AddField(usage);

            await send(embed);
        }

        [Command("Join")]
        [Alias("join")]
        public async Task joinHelp()
        {
            EmbedBuilder embed = getHelpEmbed("!join <role>");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command allows users to join roles in the server");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue("!join <role>\nEx) `!join guests`");

            EmbedFieldBuilder roles = new EmbedFieldBuilder();
            roles.WithName("Available Roles:");
            roles.WithValue("Students\nAlumni\nGuests");

            embed.AddField(function);
            embed.AddField(usage);
            embed.AddField(roles);

            await send(embed);
        }

        private EmbedBuilder getHelpEmbed(string command = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"Esports Bot Help ***{command}***");
            embed.WithColor(Color.Blue);
            embed.WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl());

            return embed;
        }

        private async Task send(EmbedBuilder embed)
        {
            await ReplyAsync("", false, embed.Build());
        }
    }
}
