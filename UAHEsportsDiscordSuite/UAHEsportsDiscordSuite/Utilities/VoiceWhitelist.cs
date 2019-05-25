using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAHEsportsDiscordSuite.Utilities
{
    public class VoiceWhitelist
    {
        public List<GuildWhitelist> whitelist { get; set; }

        private static VoiceWhitelist instance = null;
        private static Object padlock = new object();
        
        private static VoiceWhitelist getInstance()
        {
            if(instance == null)
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = JsonConvert.DeserializeObject<VoiceWhitelist>(File.ReadAllText("VoiceWhitelist.json"));
                    }
                }
            }

            return instance;
        }

        public static bool check(ulong guildId, string vc)
        {
            getInstance();
            foreach(GuildWhitelist guild in instance.whitelist)
            {
                if(guild.guildId == guildId)
                {
                    foreach(string channel in guild.whitelistChannels)
                    {
                        if (channel.Equals(Voice.ProcessName(vc)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
