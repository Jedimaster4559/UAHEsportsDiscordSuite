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
        private static Object padlock = new Object();
        
        private static VoiceWhitelist GetInstance()
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
            foreach(GuildWhitelist guild in VoiceWhitelist.GetInstance().whitelist)
            {
                if(guild.guildId == guildId)
                {
                    foreach(string channel in guild.whitelistChannels)
                    {
                        if (channel.Equals(Voice.ProcessName(vc).name))
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
