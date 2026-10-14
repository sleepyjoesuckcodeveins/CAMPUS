using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DAL.Tests.Schema
{
    public class SeedDataTests
    {
        [Fact]
        public void SeedData_Loaded_HasSevenCampuses()
        {
            // Arrange
            using var db = new SqliteDatabase();

            // Act
            var count = db.Context.Campuses.Count();

            // Assert
            Assert.Equal(7, count);
        }
    }
}
