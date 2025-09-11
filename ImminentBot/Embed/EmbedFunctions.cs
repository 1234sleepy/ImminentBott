using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot.Enitities;
using ImminentBot.methods;

namespace ImminentBot.Embed;

public static class EmbedFunctions
{
    public static async Task<List<Objectives>> checkObjectives(List<Objectives> objectives)
    {

        var newList = objectives.Where(obj => ((DateTimeOffset)obj.Date).ToUnixTimeSeconds() > DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToList();

        var service = new ImminentBotService();

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

            string emoji = "";

            switch (obj.Type.GetName()!.ToLower())
            {
                case "ore":
                    emoji = Data.objectivesEmoji![0].Name;
                    break;
                case "wood":
                    emoji = Data.objectivesEmoji![1].Name;
                    break;
                case "stone":
                    emoji = Data.objectivesEmoji![2].Name;
                    break;
                case "fiber":
                    emoji = Data.objectivesEmoji![3].Name;
                    break;
                case "hide":
                    emoji = Data.objectivesEmoji![4].Name;
                    break;
                case "vortex":
                    emoji = Data.objectivesEmoji![5].Name;
                    break;
                case "core":
                    emoji = Data.objectivesEmoji![6].Name;
                    break;
            }

            embed.AddField($" {obj.Tier} Tier {obj.Type.GetName()!} {emoji}", $"<t:{time.ToString()}:R> (UTC){obj.Date:HH:mm:ss} posted by {obj.TimedUser}");
        }


        return embed.Build();
    }
}
