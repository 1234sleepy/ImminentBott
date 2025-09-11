using ImminentBot.Enitities;
using Microsoft.EntityFrameworkCore;

namespace ImminentBot
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Objectives> Objectives { get; set; }
    }
}
