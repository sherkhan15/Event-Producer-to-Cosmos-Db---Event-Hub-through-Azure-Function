﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

using System.Linq.Expressions;
using Microsoft.Azure.Documents.Linq;

namespace IoTEmulator
{
    class Program
    {
        static string _iotid;
        static double _temp;
        static long _timestamp;
        static long MAX_DOCUMENTS = 1000;

        public static DocumentClient client;
        public static Uri collectionUri;
        public static string _database;
        public static string _collection;

        static void Main(string[] args)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            connectionPolicy.UserAgentSuffix = " samples-net/3";
            connectionPolicy.ConnectionMode = ConnectionMode.Direct;
            connectionPolicy.ConnectionProtocol = Protocol.Tcp;

            // Set the read region selection preference order
            connectionPolicy.PreferredLocations.Add(LocationNames.WestUS); // first preference
            connectionPolicy.PreferredLocations.Add(LocationNames.NorthEurope); // second preference
            connectionPolicy.PreferredLocations.Add(LocationNames.SoutheastAsia); // third preference


            Initialize(ConfigurationManager.AppSettings["database"],
                                                          ConfigurationManager.AppSettings["collection"],
                                                          ConfigurationManager.AppSettings["endpoint"],
                                                          ConfigurationManager.AppSettings["authKey"], connectionPolicy);
            PumpData();
            
            Console.ReadKey();
            return;
        }

        private static async void PumpData()
        {
            Random rnd = new Random();
            int min = 0;
            int max = 100;
            long docCtr = 0;

            do
            {
                docCtr++;
                if (docCtr % 1000 == 1)
                    Console.WriteLine(docCtr);
             
                _iotid = GetRandomlyRepeatedString(3);
                _temp = rnd.Next(min, max);
                _timestamp = DateTime.UtcNow.Ticks;
                Random r = new Random(DateTime.Now.Millisecond);
                IoTData data = new IoTData
                {
                    iotid = _iotid,
                    temp = _temp,
                    lat = r.Next(100),
                    longitude = r.Next(100),
                    carid = GetRandomlyRepeatedString(3),
                    timestamp = _timestamp
                };

                await client.CreateDocumentAsync(collectionUri, data);
                Console.Write("*");

            } while (docCtr < MAX_DOCUMENTS);
        }


        public static string GetRandomlyRepeatedString(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)random.Next('A', 'Z'));
            }
            return sb.ToString();
        }

   
   

        public static void Initialize(string database, string collection, string endpoint, string authkey, ConnectionPolicy connectionPolicy)
        {
            _database = database;
            _collection = collection;
            client = new DocumentClient(new Uri(endpoint), authkey, connectionPolicy);

            collectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);
            CreateDatabaseIfNotExistsAsync(database).Wait();
            CreateCollectionIfNotExistsAsync(database, collection).Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync(string dataBase)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(dataBase));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = dataBase });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync(string dataBase, string collection)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(dataBase, collection));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    DocumentCollection myCollection = new DocumentCollection();
                    myCollection.Id = collection;
                    myCollection.PartitionKey.Paths.Add("/iotid");

                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(dataBase),
                        myCollection,
                        new RequestOptions { OfferThroughput = 2500 });
                }
                else
                {
                    throw;
                }
            }
        }

    }


}


