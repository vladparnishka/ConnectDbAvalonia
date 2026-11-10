using Microsoft.EntityFrameworkCore;

namespace ConnectDbAvalonia.Data
{
    public static class Db
    {
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;
                    Database=rest_base;
                    Trusted_Connection=True;
                    TrustServerCertificate=True";

        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}