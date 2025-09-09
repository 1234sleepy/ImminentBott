using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ImminentBot.Config;
using ImminentBot.Embed;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ImminentBot;

internal class Program
{
 
    static async Task Main(string[] args)
    {
        var jsonReader = new JSONReader();
        await jsonReader.ReadJSON();


        if (string.IsNullOrEmpty(jsonReader.token))
        {
            throw new InvalidOperationException("Discord token is missing in configuration.");
        }

        var discordBuilder = DiscordClientBuilder.CreateDefault(
            jsonReader.token, DiscordIntents.All);


        discordBuilder.ConfigureEventHandlers
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
           );


        var commands = discordBuilder.UseCommands(
            (provider, extension) =>
            {
                extension.AddCommands<ImminentBotCommands>();

                extension.AddProcessor(new SlashCommandProcessor());
            }
        );


        var client = discordBuilder.Build();

        DiscordActivity status = new(" Albion Online", DiscordActivityType.Playing);

        await client.ConnectAsync(status, DiscordUserStatus.Online);


        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (Data.previousBotMessage != null)
                {
                    var updatedObjectives = EmbedFunctions.checkObjectives(Data.GetAllObjectives(), client).Result;
                    var embed = EmbedFunctions.CreateEmbed(updatedObjectives);
                    
                    await Data.previousBotMessage.ModifyAsync(embed);

                }
            }
        });



        await Task.Delay(-1);
    }
}
