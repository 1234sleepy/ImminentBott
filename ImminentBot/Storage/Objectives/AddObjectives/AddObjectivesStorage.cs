namespace ImminentBot.Storage.Objectives.AddObjectives;

using ImminentBot.Enitities;
using Microsoft.EntityFrameworkCore;

public class AddObjectivesStorage(DataContext dataContext)
{
    private readonly DataContext _dataContext = dataContext;

    public async Task<List<Objectives>> AddObjectives(ObjectiveType type, string tier, string zone, string timedUser, int h, int m, int s)
    {
        var obj = new Objectives
        {
            Zone = zone.ToLower(),
            TimedUser = timedUser.ToLower(),
            Type = type,
            Tier = tier,
            Date = DateTime.UtcNow.AddHours(h).AddMinutes(m).AddSeconds(s),
            isPing = false,
        };

        await _dataContext.Objectives.AddAsync(obj);

        await _dataContext.SaveChangesAsync();

        return await _dataContext.Objectives.ToListAsync();
    }
}
