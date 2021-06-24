using Microsoft.EntityFrameworkCore;

namespace WeatherLab
{
    public class WeatherDbContext : DbContext
    {
        public DbSet<Weather> Weather { get; set; }
    }

    public class WeatherSqliteContext : WeatherDbContext
    {
        string filename { get; }
        public WeatherSqliteContext(string filename)
        {
            this.filename = filename;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={this.filename}");
    }

}
