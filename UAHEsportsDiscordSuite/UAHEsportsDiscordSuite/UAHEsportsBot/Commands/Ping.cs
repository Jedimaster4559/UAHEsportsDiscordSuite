using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace UAHEsportsDiscordSuite.UAHEsportsBot.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        [Alias("ping")]
        public async Task ping()
        {
            int latency = Context.Client.Latency;

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("UAH Esports Bot");
            embed.WithDescription($"Pong! {latency} ms");

            if (latency < 100)
            {
                embed.WithColor(Color.Green);
            }
            else if (latency < 200)
            {
                embed.WithColor(Color.Orange);
            }
            else
            {
                embed.WithColor(Color.Red);
            }

            await ReplyAsync("", false, embed.Build());
        }
    }
}
