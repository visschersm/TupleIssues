using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System;
using MPTech.Mongo.Extensions;

namespace MPTech.TestUtilities.Mongo
{
    public class DatabaseManager : IDisposable
    {
        private readonly IDictionary<string, IMongoDatabase> databases = new Dictionary<string, IMongoDatabase>();
        public bool Disposed { get; private set; }

        ~DatabaseManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IMongoCollection<TDocument> GetDocument<TDocument>(string databaseName, string collectionName)
        {
            return databases[databaseName].GetCollection<TDocument>(collectionName);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            databases.ToList()
                .ForEach(database => database.Value.DropAllCollections());

            Disposed = true;
        }
    }
}