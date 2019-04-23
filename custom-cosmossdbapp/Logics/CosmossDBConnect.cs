using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace cosmossdbapp
{
    public class CosmossDBConnect
    {

        private DocumentClient _client;
        private CosmossDBConnect(DocumentClient client)
        {
            _client = client;
        }

        public static async Task<CosmossDBConnect> New()
        {
            var client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["accountEndpoint"]), ConfigurationManager.AppSettings["accountKey"]);
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Users" });
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("Users"), new DocumentCollection { Id = "WebCustomers" });
            return new CosmossDBConnect(client);
        }


        public void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        public async Task ReadUserDocument(string databaseName, string collectionName, User user)
        {
            try
            {
                await this._client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.Id), new RequestOptions { PartitionKey = new PartitionKey(user.UserId) });
                this.WriteToConsoleAndPromptToContinue("Read user {0}", user.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound) { this.WriteToConsoleAndPromptToContinue("User {0} not read", user.Id); }
                else { throw; }
            }
        }
        public async Task ReplaceUserDocument(string databaseName, string collectionName, User updatedUser)
        {
            try
            {
                await this._client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, updatedUser.Id), updatedUser, new RequestOptions { PartitionKey = new PartitionKey(updatedUser.UserId) });
                this.WriteToConsoleAndPromptToContinue("Replaced last name for {0}", updatedUser.LastName);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound) { this.WriteToConsoleAndPromptToContinue("User {0} not found for replacement", updatedUser.Id); }
                else { throw; }
            }
        }
        public async Task DeleteUserDocument(string databaseName, string collectionName, User deletedUser)
        {
            try
            {
                await this._client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, deletedUser.Id), new RequestOptions { PartitionKey = new PartitionKey(deletedUser.UserId) });
                Console.WriteLine("Deleted user {0}", deletedUser.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound) { this.WriteToConsoleAndPromptToContinue("User {0} not found for deletion", deletedUser.Id); }
                else { throw; }
            }
        }

        public async Task CreateUserDocumentIfNotExists(string databaseName, string collectionName, User user)
        {
            try
            {
                await this._client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.Id), new RequestOptions { PartitionKey = new PartitionKey(user.UserId) });
                this.WriteToConsoleAndPromptToContinue("User {0} already exists in the database", user.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this._client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    this.WriteToConsoleAndPromptToContinue("Created User {0}", user.Id);
                }
                else { throw; }
            }
        }
        public void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
            var docCollUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

            // Execute the query via Linq
            IQueryable<User> userQuery = this._client.CreateDocumentQuery<User>(docCollUri, queryOptions).Where(u => u.LastName == "Pindakova");
            Console.WriteLine("Running LINQ query...");
            foreach (User user in userQuery) { Console.WriteLine("\tRead {0}", user); }

            // Execute the query via direct SQL
            IQueryable<User> userQueryInSql = this._client.CreateDocumentQuery<User>(docCollUri, "SELECT * FROM User WHERE User.lastName = 'Pindakova'", queryOptions);
            Console.WriteLine("Running direct SQL query...");
            foreach (User user in userQueryInSql) { Console.WriteLine("\tRead {0}", user); }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        public async Task RunStoredProcedure(string databaseName, string collectionName, User user)
        {
            await _client.ExecuteStoredProcedureAsync<string>(UriFactory.CreateStoredProcedureUri(databaseName, collectionName, "UpdateOrderTotal"), new RequestOptions { PartitionKey = new PartitionKey(user.UserId) });
            Console.WriteLine("Stored procedure complete");
        }
    }
}