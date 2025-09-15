using ImminentBot.Enitities;
using Microsoft.EntityFrameworkCore;

namespace ImminentBot;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Objectives> Objectives { get; set; }
}

