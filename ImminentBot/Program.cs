using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using ImminentBot.Config;
using ImminentBot.Embed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ImminentBot;

internal class Program
{
    public static DiscordClient? DiscordClient { get; private set; }

    static async Task Main(string[] args)
    {
        var jsonReader = new JSONReader();
        await jsonReader.ReadJSON();


        if (string.IsNullOrEmpty(jsonReader.token))
        {
            throw new InvalidOperationException("Discord token is missing in configuration.");
        }

        var discord = DiscordClientBuilder
            .CreateDefault(
                jsonReader.token, DiscordIntents.All)
            .ConfigureEventHandlers
            (
               b => b.HandleMessageCreated(async (s, e) =>
               {
                   if (e.Message.Channel!.Id.ToString() == jsonReader.botChannelId && !e.Author.IsBot)
                   {
                       if (Data.previousBotMessage! != null)
                       {
                           await Data.previousBotMessage.DeleteAsync();
                       }
                       await e.Channel.DeleteMessageAsync(e.Message);

                       var obj = Data.GetAllObjectives();
                       var embed = EmbedFunctions.CreateEmbed(obj);

                       var mess = await e.Message.RespondAsync(embed);
                       Data.previousBotMessage = mess;

                   }
               })
            ).UseCommands(
                (provider, extension) =>
                {
                    extension.AddCommands<ImminentBotCommands>();

                    extension.AddProcessor(new SlashCommandProcessor());
                }
            ).ConfigureServices(s =>
                s.AddDbContext<DataContext>(o => o.UseNpgsql("Host=localhost;Database=imminentbot;Username=postgres;Password=yourpassword"))
            ).Build();

        DiscordClient = discord;

        DiscordActivity status = new(" Albion Online", DiscordActivityType.Playing);

        await discord.ConnectAsync(status, DiscordUserStatus.Online);


        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (Data.previousBotMessage != null)
                {
                    var updatedObjectives = EmbedFunctions.checkObjectives(Data.GetAllObjectives()).Result;
                    var embed = EmbedFunctions.CreateEmbed(updatedObjectives);

                    await Data.previousBotMessage.ModifyAsync(embed);

                }
            }
        });

        Data.objectivesEmoji = [
            DiscordEmoji.FromName(discord,":pick:"),
            DiscordEmoji.FromName(discord,":axe:"),
            DiscordEmoji.FromName(discord,":rock:"),
            DiscordEmoji.FromName(discord,":rose:"),
            DiscordEmoji.FromName(discord,":donkey:"),
            DiscordEmoji.FromName(discord,":cloud_tornado:"),
            DiscordEmoji.FromName(discord,":white_circle:"),
        ];

        using (var scope = discord.ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.Migrate();
        }


        await Task.Delay(-1);
    }
}
