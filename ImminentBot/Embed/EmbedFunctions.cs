using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot.Enitites;
using ImminentBot.methods;
using System.Threading.Tasks;

namespace ImminentBot.Embed;

public static class EmbedFunctions
{
    public static async Task<List<Objectives>> checkObjectives(List<Objectives> objectives, DiscordClient discord)
    {

        var newList = objectives.Where(obj => ((DateTimeOffset)obj.Date).ToUnixTimeSeconds() > DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToList();

        var service = new ImminentBotService(discord);

        foreach (Objectives obj in newList)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan diff = now - obj.Date;

            if (diff.TotalMinutes <= 15 && !obj.isPing)
            {
                await service.pingUser();
                obj.isPing = true;
            }

        }

        return newList;
    }


    public static DiscordEmbed CreateEmbed(List<Objectives> objectives)
    {

        var embed = new DiscordEmbedBuilder
        {
            Title = $"Objectives : {objectives.Count}",
            Color = DiscordColor.Red
        };

        var obsj = objectives.OrderBy(o => o.Type).ThenBy(s => s.Tier);



        foreach (var obj in obsj)
        {
            var time = ((DateTimeOffset)obj.Date).ToUnixTimeSeconds();
            var color = DiscordColor.Gray;

            switch (obj.Tier!.ToLower())
            {
                case string tier when tier.Contains(".4") || tier.Contains("gold"):
                    color = DiscordColor.Gold;
                    break;
                case string tier when tier.Contains(".3") || tier.Contains("purple"):
                    color = DiscordColor.Purple;
                    break;
                case string tier when tier.Contains(".2") || tier.Contains("blue"):
                    color = DiscordColor.Blue;
                    break;
                case string tier when tier.Contains(".1") || tier.Contains("green"):
                    color = DiscordColor.Green;
                    break;
            }

            embed.AddField($"# {obj.Tier} {obj.Type.GetName()!}", $"<t:{time.ToString()}:R> (UTC){obj.Date:HH:mm:ss} {obj.TimedUser} - timed");
        }


        return embed.Build();
    }
}
