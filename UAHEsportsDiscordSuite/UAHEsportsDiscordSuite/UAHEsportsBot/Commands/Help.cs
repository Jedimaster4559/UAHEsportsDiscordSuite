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
