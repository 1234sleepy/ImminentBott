using Microsoft.Extensions.Configuration;

namespace ImminentBot.methods;

public class ImminentBotService
{
    public async Task pingUser()
    {

        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();



        var guild = await Program.DiscordClient.GetGuildAsync(ulong.Parse(configuration["GuildId"]!));
        var channel = await guild.GetChannelAsync(ulong.Parse(configuration["ContentChannelId"]!));

        string roleMention = $"<@&{ulong.Parse(configuration["ObjectiveRoleId"]!)}>";

        await channel.SendMessageAsync($"{roleMention} Reminder: Objective in 15 minutes! Join VC!");

    }
}
