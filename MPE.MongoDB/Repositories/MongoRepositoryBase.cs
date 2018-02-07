using System;
using System.Configuration;
using System.Linq;
using System.Text;
using Functional.Maybe;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MPE.Models;
using MPE.Models.Interfaces;

namespace MPE.MongoDB.Repositories
{
    public class MongoRepositoryBase<T> : IRepository<T> 
        where T : EntityAbstract
    {
        private const string ConnectionStringName = "MongoDb";

        protected string Server;
        protected int Port;
        protected string Database;
        protected string Username;
        protected string Password;

        public MongoRepositoryBase(
            string server,
            int port,
            string database,
            string username = null,
            string password = null)
        {
            Server = server;
            Port = port;
            Database = database;
            Username = username;
            Password = password;

            Initiate();
        }

        public MongoRepositoryBase()
        {
            var mongoUrl = MongoUrl.Create(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);
            Server = mongoUrl.Server.Host;
            Port = mongoUrl.Server.Port;
            Database = mongoUrl.DatabaseName;

            Initiate();
        }

        public void Delete(T obj)
        {
            var collection = GetCollection();
            collection.DeleteOne(z => z.Id == obj.Id);
        }

        public Maybe<T> Get(Guid id)
        {
            var collection = GetCollection();
            var existing = collection.AsQueryable().FirstOrDefault(z => z.Id == id);

            return existing.ToMaybe();
        }

        public IQueryable<T> GetAll()
        {
            var collection = GetCollection();
            return collection.AsQueryable();
        }

        public void Save(T obj)
        {
            var collection = GetCollection();
            var existing = collection.AsQueryable().FirstOrDefault(z => z.Id == obj.Id);
            if (existing == null)
            {
                if (obj.Id == default(Guid))
                {
                    obj.Id = Guid.NewGuid();
                }

                collection.InsertOne(obj);
            }
            else
            {
                existing = obj;
                collection.ReplaceOne(z => z.Id == obj.Id, existing);
            }
        }

        protected IMongoClient GetClient()
        {
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(Server, Port),
                ReadEncoding = new UTF8Encoding(false, false),
                GuidRepresentation = GuidRepresentation.Standard
            };

            if (!string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(Password))
            {
                settings.Credential = MongoCredential.CreateCredential(Database, Username, Password);
            }

            return new MongoClient();
        }

        protected IMongoDatabase GetDatabase()
        {
            var client = GetClient();
            return client.GetDatabase(Database);
        }

        protected IMongoCollection<T> GetCollection()
        {
            var db = GetDatabase();
            return db.GetCollection<T>(typeof(T).Name);
        }

        private void Initiate()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }
    }
}
