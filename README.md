




# cosmosdb-emulator-gremlin
Azure Cosmos DB is Microsoft's globally distributed multi-model database service. You can quickly create and query document, table, key-value, and graph databases, all of which benefit from the global distribution and horizontal scale capabilities at the core of Azure Cosmos DB. 

Cosmos Emulator now supports Gremlin API (since version 2.1.4). This quick start sample provides scripts for inserting data into gremlin API running on Azure Cosmos DB with a local emulator (see instructions below), and inserting the same data into a mysql database for comparison. See below for how to get started with the Graph (Gremlin) API for Azure Cosmos DB using the open-source connector Gremlin.Net and Cosmos DB Emulator.

## Getting started
1. Clone this repository/
2. Open the GremlinNetSample.sln solution and restore the packages. 
3. Go to your Cosmos DB Emulator install location and open PowerShell window in that location. Default install path is `C:\Program Files\Azure Cosmos DB Emulator`
4. Gremlin Endpoint is not enabled by default. To enable it run Cosmos DB Emulator from CMD/PowerShell using following command: 
```powershell
.\CosmosDB.Emulator.exe /EnableGremlinEndpoint
```
 5. Gremlin Endpoint will be opened on port `8901` by default. If you want to change that port run Emulator wiht follwing command:
 
 ```powershell
.\CosmosDB.Emulator.exe /EnableGremlinEndpoint /GremlinPort=<port_number>
```
6. Endpoints in the sample are preconfigured to run with the Emulator. You don't have to change anything apart for create a db named "graph", and a graph named "people", and create partition key "/pk", within the emulator portal that starts up after you have run the above Powershell command(s). .
7. Build and Run the project.
8. This will load data into your local Cosmos DB container. 
9. You can traverse the graph data using a graph explorer application that has been added to this project (Note that node.js needs to be installed onto your system first for this to run properly - https://nodejs.org/en/download/). Open the GremlinNetSample.sln solution under the "Web" folder and restore the packages. Run the solution. 
10. You can also compare this data with the same data in mysql

## More information

- [Azure Cosmos DB](https://docs.microsoft.com/azure/cosmos-db/introduction)
- [Gremlin API](https://docs.microsoft.com/en-us/azure/cosmos-db/graph-introduction)
- [Apache Tinkerpop](https://tinkerpop.apache.org/)
