using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot;
using ImminentBot.Embed;
using ImminentBot.Enitities;
using Microsoft.Extensions.Configuration;

public class ImminentBotCommands
{

    [Command("addObjective")]
    public async Task AddObjective(SlashCommandContext ctx, [Option("type", "type")] ObjectiveType type, [Option("tier", "tiere")] string tier, string zone, [Option("hour", "")] int h, [Option("minutes", "")] int m, [Option("seconds", "")] int s)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        if (ctx.Channel.Id.ToString() != configuration["BotChannelId"])
        {
            var res = await ctx.Channel.SendMessageAsync("Wrong Channel. You can not use it here");
            return;
        }
        await ctx.DeferResponseAsync();

        if (Data.previousBotMessage! != null)
        {
            await Data.previousBotMessage.DeleteAsync();
        }

        var obj = Data.AddObjectives(type, tier, zone, ctx.User.GlobalName, h, m, s);
        var embed = EmbedFunctions.CreateEmbed(obj);

        Data.previousBotMessage = await ctx.Channel.SendMessageAsync(embed);

        var response = await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Success"));

        await response.DeleteAsync();

    }
}