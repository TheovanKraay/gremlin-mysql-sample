using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GremlinNetSample
{
    /// <summary>
    /// Sample program that shows how to get started with the Graph (Gremlin) API for Azure Cosmos DB using the open-source connector Gremlin.Net.
    /// </summary>
    internal class Program
    {
        // Azure Cosmos DB Configuration variables

        #region Config

        /// <summary>
        /// The Cosmos DB hostname
        /// </summary>
        private const string Hostname = "localhost";

        /// <summary>
        /// The port
        /// </summary>
        private const int Port = 8901;

        /// <summary>
        /// The authentication key
        /// </summary>
        private const string AuthKey =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        /// <summary>
        /// If set to <c>true</c> then SSL will be enabled, <c>false</c> otherwise
        /// </summary>
        private const bool EnableSsl = false;

        // Replace the values in these variables to your own.
        /// <summary>
        /// The database
        /// </summary>
        private const string Database = "graph";

        /// <summary>
        /// The collection
        /// </summary>
        private const string Graph = "people";

        /// <summary>
        /// The partition key property name
        /// </summary>
        private const string PartitionKeyPropertyName = "pk";

        #endregion Config

        /// <summary>
        /// Gremlin queries that will be executed.
        /// </summary>
        private static readonly Dictionary<string, string> GremlinQueries = new Dictionary<string, string>
        {
            {"Cleanup", "g.V().drop()"},
            //Add groups (vertices)
            {"AddVertex 1", $"g.addV('group').property('pk', 'pk').property('id', 'Microsoft').property('name', 'Microsoft')"},
            {"AddVertex 2", $"g.addV('group').property('pk', 'pk').property('id', 'Azure').property('name', 'Azure')"},
            {"AddVertex 3", $"g.addV('group').property('pk', 'pk').property('id', 'Sales').property('name', 'Sales')"},
            {"AddVertex 4", $"g.addV('group').property('pk', 'pk').property('id', 'Engineering').property('name', 'Engineering')"},

            //Add People (vertices)
            {"AddVertex 5", $"g.addV('person').property('pk', 'pk').property('id', 'Rimma N.').property('name', 'Rimma N.')"},
            {"AddVertex 6", $"g.addV('person').property('pk', 'pk').property('id', 'Andrew L.').property('name', 'Andrew L.')"},
            {"AddVertex 7", $"g.addV('person').property('pk', 'pk').property('id', 'Luis B.').property('name', 'Luis B.')"},
            {"AddVertex 8", $"g.addV('person').property('pk', 'pk').property('id', 'New Person.').property('name', 'New Person.')"},

            //add group memberships (edges)
            {"AddEdge 1", "g.V('Microsoft').addE('subgroup').to(g.V('Azure'))"},
            {"AddEdge 2", "g.V('Azure').addE('subgroup').to(g.V('Sales'))"},
            {"AddEdge 3", "g.V('Azure').addE('subgroup').to(g.V('Engineering'))"},
            {"AddEdge 4", "g.V('Engineering').addE('member').to(g.V('Rimma N.'))"},
            {"AddEdge 5", "g.V('Engineering').addE('member').to(g.V('Andrew L.'))"},
            {"AddEdge 6", "g.V('Engineering').addE('member').to(g.V('New Person.'))"},
            {"AddEdge 7", "g.V('Sales').addE('member').to(g.V('Luis B.'))"},
            {"AddEdge 8", "g.V('Sales').addE('member').to(g.V('Andrew L.'))"},

            //add reporting hierarchies (edges)
            {"AddEdge 9", "g.V('Rimma N.').addE('directReports').to(g.V('Andrew L.'))"},
            {"AddEdge 10", "g.V('Rimma N.').addE('directReports').to(g.V('Luis B.'))"},
            {"AddEdge 11", "g.V('Rimma N.').addE('directReports').to(g.V('New Person.'))"},
   
        };

        /// <summary>
        /// Defines the entry point of the application. Starts a console application that executes every Gremlin query in the gremlinQueries dictionary.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            var gremlinServer = new GremlinServer(Hostname, Port, enableSsl: EnableSsl,
                username: "/dbs/" + Database + "/colls/" + Graph,
                password: AuthKey);

            using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType))
            {
                foreach (var query in GremlinQueries)
                {
                    Console.WriteLine($"Running this query: {query.Key}: {query.Value}");

                    // Create async task to execute the Gremlin query.
                    var resultSet = SubmitRequest(gremlinClient, query).Result;
                    if (resultSet.Count > 0)
                    {
                        Console.WriteLine("\tResult:");
                        foreach (var result in resultSet)
                        {
                            // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                            string output = JsonConvert.SerializeObject(result);
                            Console.WriteLine($"\t{output}");
                        }

                        Console.WriteLine();
                    }

                    // Print the status attributes for the result set.
                    // This includes the following:
                    //  x-ms-status-code            : This is the sub-status code which is specific to Cosmos DB.
                    //  x-ms-total-request-charge   : The total request units charged for processing a request.
                    PrintStatusAttributes(resultSet.StatusAttributes);
                    Console.WriteLine();
                }
            }

            // Exit program
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Submits the request to Cosmos DB.
        /// </summary>
        /// <param name="gremlinClient">The gremlin client.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private static Task<ResultSet<dynamic>> SubmitRequest(IGremlinClient gremlinClient,
            KeyValuePair<string, string> query)
        {
            try
            {
                return gremlinClient.SubmitAsync<dynamic>(query.Value);
            }
            catch (ResponseException e)
            {
                Console.WriteLine("\tRequest Error!");

                // Print the Gremlin status code.
                Console.WriteLine($"\tStatusCode: {e.StatusCode}");

                // On error, ResponseException.StatusAttributes will include the common StatusAttributes for successful requests, as well as
                // additional attributes for retry handling and diagnostics.
                // These include:
                //  x-ms-retry-after-ms         : The number of milliseconds to wait to retry the operation after an initial operation was throttled. This will be populated when
                //                              : attribute 'x-ms-status-code' returns 429.
                //  x-ms-activity-id            : Represents a unique identifier for the operation. Commonly used for troubleshooting purposes.
                PrintStatusAttributes(e.StatusAttributes);
                Console.WriteLine(
                    $"\t[\"x-ms-retry-after-ms\"] : {GetValueAsString(e.StatusAttributes, "x-ms-retry-after-ms")}");
                Console.WriteLine(
                    $"\t[\"x-ms-activity-id\"] : {GetValueAsString(e.StatusAttributes, "x-ms-activity-id")}");

                throw;
            }
        }

        /// <summary>
        /// Prints the status attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        private static void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes)
        {
            Console.WriteLine($"\tStatusAttributes:");
            Console.WriteLine($"\t[\"x-ms-status-code\"] : {GetValueAsString(attributes, "x-ms-status-code")}");
            Console.WriteLine(
                $"\t[\"x-ms-total-request-charge\"] : {GetValueAsString(attributes, "x-ms-total-request-charge")}");
        }

        /// <summary>
        /// Gets the value as a string.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key) =>
            JsonConvert.SerializeObject(GetValueOrDefault(dictionary, key));

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key) =>
            dictionary.ContainsKey(key) ? dictionary[key] : null;
    }
}
