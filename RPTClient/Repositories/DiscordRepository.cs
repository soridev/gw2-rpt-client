using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;

namespace RPTClient.Repositories
{
    class DiscordRepository
    {
        private string _webhookUrl = string.Empty;

        public DiscordRepository(string webhookUrl) 
        {
            _webhookUrl = webhookUrl;
        }

        public async Task MainAsync()
        {
            using(DiscordWebhookClient client = new DiscordWebhookClient(_webhookUrl))
            {
                var embed = new EmbedBuilder
                {
                    Title = "UwU",
                    Description = ":KEKW:"
                };

                await client.SendMessageAsync(text: ":monkaStop: :monkaStop: :monkaStop:", embeds: new[] { embed.Build() });
            }
        }        
    }
}
