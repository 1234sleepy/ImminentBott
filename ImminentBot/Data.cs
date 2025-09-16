using DSharpPlus.Entities;
using ImminentBot.Enitities;

namespace ImminentBot;

public static class Data
{

    public static List<Objectives> SavedObjectives = new List<Objectives>();
    public static DiscordMessage? previousBotMessage { get; set; }

    public static DiscordEmoji[]? objectivesEmoji { get; set; }
    
}
