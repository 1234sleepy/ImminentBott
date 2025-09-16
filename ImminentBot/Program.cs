using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using ImminentBot.Embed;
using ImminentBot.Storage.Objectives.GetAllObjectives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImminentBot;

internal class Program
{
    public static DiscordClient? DiscordClient { get; private set; }

    static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        var discord = DiscordClientBuilder
            .CreateDefault(configuration["Token"], DiscordIntents.All)
            .ConfigureEventHandlers(b =>
            {
                b.HandleMessageCreated(async (s, e) =>
                {
                    if (e.Message.Channel!.Id.ToString() == configuration["botChannelId"] && !e.Author.IsBot)
                    {
                        if (Data.previousBotMessage is not null)
                            await Data.previousBotMessage.DeleteAsync();

                        await e.Channel.DeleteMessageAsync(e.Message);

                        using var scope = DiscordClient!.ServiceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                        var storage = new GetAllObjectivesStorage(db);

                        var obj = storage.GetAllObjectives();
                        var embed = EmbedFunctions.CreateEmbed(obj.Result);

                        var mess = await e.Message.RespondAsync(embed);
                        Data.previousBotMessage = mess;
                    }
                });
            })
            .UseCommands((provider, extension) =>
            {
                extension.AddCommands<ImminentBotCommands>();
                extension.AddProcessor(new SlashCommandProcessor());
            })
            .ConfigureServices(s =>
                s.AddDbContext<DataContext>(o => o.UseNpgsql(configuration["DefaultConnection"]))
            )
            .Build();


        DiscordClient = discord;

        DiscordActivity status = new("Albion Online", DiscordActivityType.Playing);



        await discord.ConnectAsync(status, DiscordUserStatus.Online);
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                using (var scope = discord.ServiceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var storage = new GetAllObjectivesStorage(db);
                    var obj = storage.GetAllObjectives();
                    var updatedObjectives = await EmbedFunctions.checkObjectives(obj.Result);
                    var embed = EmbedFunctions.CreateEmbed(updatedObjectives);

                    await Data.previousBotMessage.ModifyAsync(embed);
                }

            }
        });

        Data.objectivesEmoji = [
            DiscordEmoji.FromName(discord, ":pick:"),
            DiscordEmoji.FromName(discord, ":axe:"),
            DiscordEmoji.FromName(discord, ":rock:"),
            DiscordEmoji.FromName(discord, ":rose:"),
            DiscordEmoji.FromName(discord, ":donkey:"),
            DiscordEmoji.FromName(discord, ":cloud_tornado:"),
            DiscordEmoji.FromName(discord, ":white_circle:"),
        ];

        using (var scope = discord.ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.Migrate();
        }

        await Task.Delay(-1);
    }
}
