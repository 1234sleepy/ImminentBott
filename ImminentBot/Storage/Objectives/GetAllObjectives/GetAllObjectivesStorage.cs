namespace ImminentBot.Storage.Objectives.GetAllObjectives;
using ImminentBot.Enitities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class GetAllObjectivesStorage(DataContext dataContext)
{
    private readonly DataContext _dataContext = dataContext;

    public Task<List<Objectives>> GetAllObjectives()
    {
        return _dataContext.Objectives.ToListAsync();
    }

}
