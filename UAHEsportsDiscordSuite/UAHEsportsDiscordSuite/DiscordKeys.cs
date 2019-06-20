using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAHEsportsDiscordSuite
{
    /// <summary>
    /// DiscordKeys allows easy access to the Discord Keys for each bot this suite services. All Methods are static and no instance of this class is required.
    /// </summary>
    public class DiscordKeys
    {
        [JsonProperty]
        private string EsportsKey { get; set; }
        [JsonProperty]
        private string RocketLeagueKey { get; set; }

        private static DiscordKeys instance = null;
        private static Object padlock = new Object();
        private static string keysFile = "DiscordKeysBeta.json";

        /// <summary>
        /// Gets the discord Keys object. This includes deserialization from 
        /// </summary>
        /// <returns>The instance of the Discord Keys</returns>
        private static DiscordKeys GetInstance()
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = JsonConvert.DeserializeObject<DiscordKeys>(File.ReadAllText(keysFile));
                    }
                }
            }
            return instance;
            
        }

        /// <summary>
        /// Gets the Esports Key for the main Esports Bot
        /// </summary>
        /// <returns>The Esports Discord Key</returns>
        public static string GetEsportsKey()
        {
            return DiscordKeys.GetInstance().EsportsKey;
        }

        /// <summary>
        /// Gets the Rocket League Discord Key for the Rocket League Discord Bot
        /// </summary>
        /// <returns>The Rocket League Discord Bot Key</returns>
        public static string GetRocketLeagueKey()
        {
            return DiscordKeys.GetInstance().RocketLeagueKey;
        }
    }

}
