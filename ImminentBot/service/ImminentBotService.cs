using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot;
using ImminentBot.Config;
using ImminentBot.Embed;
using ImminentBot.Enitites;


namespace ImminentBot.methods;

public class ImminentBotService
{
    private readonly DiscordClient _client;

    public ImminentBotService(DiscordClient client)
    {
        _client = client;
    }

    public async Task pingUser() {
        var jsonReader = new JSONReader();
        await jsonReader.ReadJSON();

        var guild = await _client.GetGuildAsync(ulong.Parse(jsonReader.guildId!));
        var channel = await guild.GetChannelAsync(ulong.Parse(jsonReader.contentChannelId!));

        string roleMention = $"<@&{ulong.Parse(jsonReader.objectiveRoleId!)}>";

        await channel.SendMessageAsync($"{roleMention} Reminder: Objective in 15 minutes! Join VC!");

    }
}
