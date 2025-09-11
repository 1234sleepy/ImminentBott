using DSharpPlus;
using ImminentBot.Config;


namespace ImminentBot.methods;

public class ImminentBotService
{
    public async Task pingUser()
    {
        var jsonReader = new JSONReader();
        await jsonReader.ReadJSON();

        var guild = await Program.DiscordClient.GetGuildAsync(ulong.Parse(jsonReader.guildId!));
        var channel = await guild.GetChannelAsync(ulong.Parse(jsonReader.contentChannelId!));

        string roleMention = $"<@&{ulong.Parse(jsonReader.objectiveRoleId!)}>";

        await channel.SendMessageAsync($"{roleMention} Reminder: Objective in 15 minutes! Join VC!");

    }
}
