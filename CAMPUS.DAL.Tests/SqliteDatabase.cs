using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DAL.Tests
{
    public sealed class SqliteDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;

        public SqliteDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            Context = CreateContext();
            Context.Database.EnsureCreated();
        }

        public CampusDbContext Context { get; }

        public CampusDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<CampusDbContext>()
                .UseSqlite(_connection)
                .Options;

            return new CampusDbContext(options);
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}
