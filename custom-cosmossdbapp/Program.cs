/*PACKAGES:
dotnet add package System.Net.Http
dotnet add package System.Configuration
dotnet add package System.Configuration.ConfigurationManager
dotnet add package Microsoft.Azure.DocumentDB.Core
dotnet add package Newtonsoft.Json
dotnet add package System.Threading.Tasks
dotnet add package System.Linq
dotnet restore
 */

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
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.BasicOperations().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
        private async Task BasicOperations()
        {
            var cosmossDBConnect = await CosmossDBConnect.New();
            Console.WriteLine("Database and collection validation complete");
            User yanhe = new User
            {
                Id = "1",
                UserId = "yanhe",
                LastName = "He",
                FirstName = "Yan",
                Email = "yanhe@contoso.com",
                OrderHistory = new OrderHistory[] { new OrderHistory { OrderId = "1000", DateShipped = "08/17/2018", Total = "52.49" } },
                ShippingPreference = new ShippingPreference[]
                {
                    new ShippingPreference {
                            Priority = 1,
                            AddressLine1 = "90 W 8th St",
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                    }
                },
            };

            await cosmossDBConnect.CreateUserDocumentIfNotExists("Users", "WebCustomers", yanhe);

            User nelapin = new User
            {
                Id = "2",
                UserId = "nelapin",
                LastName = "Pindakova",
                FirstName = "Nela",
                Email = "nelapin@contoso.com",
                Dividend = "8.50",
                Coupons = new CouponsUsed[] { new CouponsUsed{ CouponCode = "Fall2018" } },
                OrderHistory = new OrderHistory[] { new OrderHistory { OrderId = "1001", DateShipped = "08/17/2018", Total = "105.89" } },
                ShippingPreference = new ShippingPreference[]
                {
                    new ShippingPreference {
                            Priority = 1,
                            AddressLine1 = "505 NW 5th St",
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                    },
                    new ShippingPreference {
                            Priority = 2,
                            AddressLine1 = "505 NW 5th St",
                            City = "New York",
                            State = "NY",
                            ZipCode = "10001",
                            Country = "USA"
                    }
                }
            };

            await cosmossDBConnect.CreateUserDocumentIfNotExists("Users", "WebCustomers", yanhe);
            await cosmossDBConnect.CreateUserDocumentIfNotExists("Users", "WebCustomers", nelapin);
            yanhe.LastName = "Suh";
            await cosmossDBConnect.ReplaceUserDocument("Users", "WebCustomers", yanhe);
            await cosmossDBConnect.ReadUserDocument("Users", "WebCustomers", yanhe);
            await cosmossDBConnect.RunStoredProcedure("Users", "WebCustomers", yanhe);
            await cosmossDBConnect.DeleteUserDocument("Users", "WebCustomers", yanhe);
            cosmossDBConnect.ExecuteSimpleQuery("Users", "WebCustomers");
        }
    }
}
