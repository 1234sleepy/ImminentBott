using DSharpPlus.SlashCommands;

namespace ImminentBot.Enitites;

public class Objectives
{
    public ObjectiveType Type { get; set; }
    public string? Zone { get; set; }
    public string? Tier { get; set; }
    public string? TimedUser { get; set; }
    public DateTime Date { get; set; }
    public bool isPing { get; set; }
}

public enum ObjectiveType
{
    [ChoiceName("Core")]
    core,
    [ChoiceName("Vortex")]
    vortex,
    [ChoiceName("Hide")]
    hide,
    [ChoiceName("Stone")]
    stone,
    [ChoiceName("Ore")]
    ore,
    [ChoiceName("Wood")]
    wood,
    [ChoiceName("fiber")]
    fiber,
}