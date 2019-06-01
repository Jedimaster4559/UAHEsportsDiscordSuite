using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using UAHEsportsDiscordSuite.Utilities;

namespace UAHEsportsDiscordSuite.RocketLeagueBot.Services.Ranks
{
    public class RanksUser
    {
        private RocketLeagueRanks? standard = null, doubles = null, duel = null, soloStandard = null;
        private RocketLeagueRanks? hoops = null, rumble = null, dropshot = null, snowDay = null;
        private IGuildUser user = null;

        public RanksUser(Embed embed, SocketCommandContext Context)
        {
            processUser(Context);
            processEmbed(embed);

            if (user == null 
                || soloStandard == null
                || duel == null
                || doubles == null
                || standard == null)
            {

            } else
            {
                handleRanks();
            }
        }

        private async void processUser(SocketCommandContext Context)
        {
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> messagesRaw = Context.Channel.GetMessagesAsync(2);
            IReadOnlyCollection<IMessage> messages = (await messagesRaw.ToList())[1];

            try
            {
                user = messages.ElementAt(1).Author as IGuildUser;
            } catch (IndexOutOfRangeException e)
            {
                user = null;
            }
        }

        private void processEmbed(Embed embed)
        {
            foreach(EmbedField field in embed.Fields)
            {
                processField(field);
            }
        }

        private void processField(EmbedField field)
        {
            if (field.Name.Equals("Solo"))
            {
                duel = getRank(field);
            }
            else if (field.Name.Equals("Doubles"))
            {
                doubles = getRank(field);
            }
            else if (field.Name.Equals("Solo Standard"))
            {
                soloStandard = getRank(field);
            }
            else if (field.Name.Equals("Standard"))
            {
                standard = getRank(field);
            }
        }

        private RocketLeagueRanks getRank(EmbedField field)
        {
            if (field.Value.Contains("Unranked"))
            {
                return RocketLeagueRanks.Unranked;
            }
            else if (field.Value.Contains("Bronze"))
            {
                return RocketLeagueRanks.Bronze;
            }
            else if (field.Value.Contains("Silver"))
            {
                return RocketLeagueRanks.Silver;
            }
            else if (field.Value.Contains("Gold"))
            {
                return RocketLeagueRanks.Gold;
            }
            else if (field.Value.Contains("Platinum"))
            {
                return RocketLeagueRanks.Platinum;
            }
            else if (field.Value.Contains("Diamond"))
            {
                return RocketLeagueRanks.Diamond;
            }
            else if (field.Value.Contains("Grand Champion"))
            {
                return RocketLeagueRanks.GrandChamp;
            }
            else
            {
                return RocketLeagueRanks.Champ;
            }

        }

        private async void handleRanks()
        {
            RocketLeagueRanks rank = getHighestRank();

            removeRoles();

            if(rank == RocketLeagueRanks.Unranked)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Unranked/New", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Bronze)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Bronze", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Silver)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Silver", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Gold)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Gold", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Platinum)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Platinum", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Diamond)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Diamond", user.Guild));
            }
            else if (rank == RocketLeagueRanks.GrandChamp)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Grand Champion", user.Guild));
            }
            else if (rank == RocketLeagueRanks.Champ)
            {
                await user.AddRoleAsync(RoleHelper.getRole("Champion", user.Guild));
            }
        }

        private RocketLeagueRanks getHighestRank()
        {
            List<RocketLeagueRanks?> ranks = new List<RocketLeagueRanks?>();
            ranks.Add(soloStandard);
            ranks.Add(duel);
            ranks.Add(standard);
            ranks.Add(doubles);

            RocketLeagueRanks min = RocketLeagueRanks.Unranked;

            foreach (RocketLeagueRanks rank in ranks)
            {
                if (rank > min)
                {
                    min = rank;
                }
            }

            return min;
        }
    }
}
