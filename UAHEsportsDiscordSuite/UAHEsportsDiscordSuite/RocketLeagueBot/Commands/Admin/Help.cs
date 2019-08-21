using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace UAHEsportsDiscordSuite.RocketLeagueBot.Commands.Admin
{
    [Group("@help")]
    [Alias("@Help")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {
            EmbedBuilder embed = getHelpEmbed();

            string message = "";
            message += "!@reset <user>\n";
            message += "!@resetall\n";

            EmbedFieldBuilder field = new EmbedFieldBuilder();
            field.WithName("Available Commands:");
            field.WithValue(message);

            EmbedFieldBuilder additionalHelp = new EmbedFieldBuilder();
            additionalHelp.WithName("For more help, type type the help command with a specific Command");
            additionalHelp.WithValue("Ex) !help reset");

            embed.AddField(field);
            embed.AddField(additionalHelp);

            await send(embed);
        }

        [Command("reset")]
        [Alias("Reset", "resetrank", "ResetRank")]
        public async Task helpReset()
        {
            EmbedBuilder embed = getHelpEmbed("!reset <user>");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command will reset a user to the default rank of Unranked/New");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue($"!reset {Context.Client.CurrentUser.Mention}");

            embed.AddField(function);
            embed.AddField(usage);

            await send(embed);
        }

        [Command("ResetAll")]
        [Alias("resetall")]
        public async Task resetAllHelp()
        {
            EmbedBuilder embed = getHelpEmbed("!resetall");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command will reset all users in the server to the default rank of Unranked/New. This can take some time.");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue("!resetall");

            embed.AddField(function);
            embed.AddField(usage);

            await send(embed);
        }

        private EmbedBuilder getHelpEmbed(string command = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"Rocket League Bot Help ***{command} ***");
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
