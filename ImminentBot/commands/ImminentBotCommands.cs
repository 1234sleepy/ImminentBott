using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot;
using ImminentBot.Embed;
using ImminentBot.Enitities;
using ImminentBot.Storage.Objectives.AddObjectives;
using ImminentBot.Storage.Objectives.GetAllObjectives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To fix CS0120, you need an instance of DiscordClient to access ServiceProvider.
// Add a constructor to ImminentBotCommands to accept a DiscordClient instance.
// Store it in a private field and use that field in your method.

public class ImminentBotCommands
{
    private readonly DiscordClient _discordClient;

    public ImminentBotCommands(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

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

        using (var scope = _discordClient.ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var storage = new AddObjectivesStorage(db);
            var obj = await storage.AddObjectives(type, tier, zone, ctx.User.GlobalName, h, m, s);
            var updatedObjectives = await EmbedFunctions.checkObjectives(obj);
            var embed = EmbedFunctions.CreateEmbed(updatedObjectives);

            if (Data.previousBotMessage is not null)
            {
                try
                {
                    await Data.previousBotMessage.ModifyAsync(embed);
                }
                catch (DSharpPlus.Exceptions.NotFoundException)
                {
                    // The message was deleted, ignore or send a new one
                    Data.previousBotMessage = await ctx.Channel.SendMessageAsync(embed);
                }
            }
            else
            {
                // If it doesn't exist, send a new message
                Data.previousBotMessage = await ctx.Channel.SendMessageAsync(embed);
            }

        }



        var response = await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Success"));

        await response.DeleteAsync();
    }
}