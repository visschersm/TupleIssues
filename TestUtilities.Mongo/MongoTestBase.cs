using System;

namespace MPTech.TestUtilities.Mongo
{
    public class MongoTestBase
    {
        private readonly DatabaseManager databaseManager;

        public MongoTestBase()
        {
            this.databaseManager = new DatabaseManager();
        }
    }
}
