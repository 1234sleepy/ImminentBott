using DSharpPlus.Entities;
using ImminentBot.Enitites;

namespace ImminentBot;

public static class Data
{
    public static List<Objectives> SavedObjectives = new List<Objectives>();
    public static DiscordMessage? previousBotMessage { get; set; }

    public static List<Objectives> GetAllObjectives()
    {

        return SavedObjectives!;
    }

    public static List<Objectives> AddObjectives(ObjectiveType type, string tier, string zone, string timedUser, int h, int m, int s)
    {
        SavedObjectives.Add(new Objectives
        {
            Zone = zone.ToLower(),
            TimedUser = timedUser.ToLower(),
            Type = type,
            Tier = tier,
            Date = DateTime.UtcNow.AddHours(h).AddMinutes(m).AddSeconds(s),
            isPing = false,
        });

        return SavedObjectives;
    }

}
