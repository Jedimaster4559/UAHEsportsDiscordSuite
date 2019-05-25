using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAHEsportsDiscordSuite.Utilities
{
    public class GuildWhitelist
    {
        public string name { get; set; }
        public ulong guildId { get; set; }
        public List<string> whitelistChannels { get; set; }


    }
}
