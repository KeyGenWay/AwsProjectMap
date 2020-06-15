using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherProject
{
    class Program
    {
        private static AmazonDynamoDBClient amazonDbClient;

        private static Table UsersTable;
        private static string UsersTableName = "Users";

        private static string Name = "Maciej";
        private static string SurName = "Kowalczyk";
        private static string Project = "WeatherProject";
        private static string Number = "123 456 789";

        static async Task Main(string[] args)
        {
            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();

            config.ServiceURL = "https://dynamodb.us-east-2.amazonaws.com";
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(new PrivateAwsCredentials(),config);

            UsersTable = Table.LoadTable(client, UsersTableName);

            Document newItem = new Document();
            newItem.Add("Id", 1);
            newItem.Add("Name", Name);
            newItem.Add("SurName", SurName);
            newItem.Add("Project", Project);
            newItem.Add("Number", Number);
            Task<Document> addNewUser = UsersTable.PutItemAsync(newItem, null);
            Console.WriteLine("Put item ready to be submitted");
            Console.ReadKey();

            var result = await addNewUser;
            Console.WriteLine("Put item has been sent");
        
        }

    }

    class PrivateAwsCredentials : AWSCredentials
    {
        private string AwsAccessKey = "AKIAJ5X7VGCUZ4SGPW5Q";
        private string AwsSecretAccessKey = "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";
            public override ImmutableCredentials GetCredentials()
        {
            return new ImmutableCredentials(AwsAccessKey, AwsSecretAccessKey, null);
        }
    }

}
