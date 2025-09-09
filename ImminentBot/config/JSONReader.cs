using Newtonsoft.Json;

namespace ImminentBot.Config;

public class JSONReader
{
    public string? token { get; set; }
    public string? botChannelId { get; set; }
    public string? contentChannelId { get; set; }
    public string? objectiveRoleId { get; set; }
    public string? guildId { get; set; }
    public async Task ReadJSON()
    {
        using (StreamReader sr = new StreamReader("config.json"))
        {
            string json = await sr.ReadToEndAsync();
            JSONStructe? data = JsonConvert.DeserializeObject<JSONStructe>(json);
            if (data != null)
            {
                token = data.token;
                botChannelId = data.botChannelId;
                objectiveRoleId = data.objectiveRoleId;
                guildId = data.guildId;
                contentChannelId = data.contentChannelId;
            }
        }
    }
}

internal sealed class JSONStructe
{
    public string? token { get; set; }
    public string? botChannelId { get; set; }
    public string? contentChannelId { get; set; }
    public string? objectiveRoleId { get; set; }
    public string? guildId { get; set; }
}
