using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Security;

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Permissions;

namespace WorkingWithMongoDB
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.ReadLine();
        }

        static async Task MainAsync()
        {
            var connectionString = "mongodb://localmongo1:27017?ssl=true&authMechanism=MONGODB-X509&ssl_ca_certs=./test-ca.pem";

            var settings = new MongoClientSettings 
            {
                Credential =  MongoCredential.CreateMongoX509Credential("CN=ChrisChoClient,OU=TestClientCertificateOrgUnit,O=TestClientCertificateOrg,L=TestClientCertificateLocality,ST=TestClientCertificateState,C=US"),
                SslSettings = new SslSettings
                {
                    ClientCertificates = new List<X509Certificate>()
                    {
                        new X509Certificate2("./client-certificate.pfx", "mypass")
                    },
                },
                UseSsl = true
            };

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("test");
            var collection = database.GetCollection<Entity>("stuff");

            var entity = new Entity { Name = "Tom" };
            collection.InsertOne(entity);
            var id = entity.Id;

            //var query = Query<Entity>.EQ(e => e.Id, id);
            //entity = collection.FindOne(query);

            //entity.Name = "Mot";
            //collection.Save(entity);
        }
    }
}