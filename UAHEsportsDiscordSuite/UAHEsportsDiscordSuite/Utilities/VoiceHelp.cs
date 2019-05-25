using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace UAHEsportsDiscordSuite.Utilities
{
    [Group("Help")]
    [Alias("help")]
    public class VoiceHelp : ModuleBase<SocketCommandContext>
    {
        [Command("Lock")]
        [Alias("lock")]
        public async Task lockHelp()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Voice Lock Help");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command allows users to lock their current voice channel to the number of members in the channel");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue("!lock");

            await ReplyAsync("", false, embed.Build());
        }

        [Command("Unlock")]
        [Alias("unlock")]
        public async Task unlockHelp()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Voice Unlock Help");

            EmbedFieldBuilder function = new EmbedFieldBuilder();
            function.WithName("Function:");
            function.WithValue("This command allows users to unlock their current voice channel.");

            EmbedFieldBuilder usage = new EmbedFieldBuilder();
            usage.WithName("Usage:");
            usage.WithValue("!unlock");

            await ReplyAsync("", false, embed.Build());
        }
    }
}
