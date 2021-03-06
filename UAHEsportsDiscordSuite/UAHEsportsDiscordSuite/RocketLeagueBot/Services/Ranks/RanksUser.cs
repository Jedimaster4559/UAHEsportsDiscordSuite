﻿using System;
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
        private bool parseState = false;
        private Embed embed;
        private SocketCommandContext Context;

        public RanksUser(Embed embed, SocketCommandContext Context)
        {
            this.embed = embed;
            this.Context = Context;
        }

        public async Task parse()
        {
            await processUser(Context);
            processEmbed(embed);

            if (user == null
                || soloStandard == null
                || duel == null
                || doubles == null
                || standard == null)
            {

            }
            else
            {
                parseState = true;
            }
        }

        public bool getState()
        {
            return parseState;
        }

        private async Task processUser(SocketCommandContext Context)
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

        public async Task handleRanks()
        {
            RocketLeagueRanks rank = getHighestRank();

            await removeRanks(user);

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

        public static async Task removeRanks(IGuildUser inputUser)
        {

            IRole unranked = Utilities.RoleHelper.getRole("Unranked/New", inputUser.Guild);
            if (inputUser.RoleIds.Contains(unranked.Id))
            {
                await inputUser.RemoveRoleAsync(unranked);
            }

            IRole bronze = Utilities.RoleHelper.getRole("Bronze", inputUser.Guild);
            if (inputUser.RoleIds.Contains(bronze.Id))
            {
                await inputUser.RemoveRoleAsync(bronze);
            }

            IRole silver = Utilities.RoleHelper.getRole("Silver", inputUser.Guild);
            if (inputUser.RoleIds.Contains(silver.Id))
            {
                await inputUser.RemoveRoleAsync(silver);
            }

            IRole gold = Utilities.RoleHelper.getRole("Gold", inputUser.Guild);
            if (inputUser.RoleIds.Contains(gold.Id))
            {
                await inputUser.RemoveRoleAsync(gold);
            }

            IRole platinum = Utilities.RoleHelper.getRole("Platinum", inputUser.Guild);
            if (inputUser.RoleIds.Contains(platinum.Id))
            {
                await inputUser.RemoveRoleAsync(platinum);
            }

            IRole diamond = Utilities.RoleHelper.getRole("Diamond", inputUser.Guild);
            if (inputUser.RoleIds.Contains(diamond.Id))
            {
                await inputUser.RemoveRoleAsync(diamond);
            }

            IRole champ = Utilities.RoleHelper.getRole("Champion", inputUser.Guild);
            if (inputUser.RoleIds.Contains(champ.Id))
            {
                await inputUser.RemoveRoleAsync(champ);
            }

            IRole grandChamp = Utilities.RoleHelper.getRole("Grand Champion", inputUser.Guild);
            if (inputUser.RoleIds.Contains(grandChamp.Id))
            {
                await inputUser.RemoveRoleAsync(grandChamp);
            }
        }
    }
}
