using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaFunction
{
    public class RegisterNewUser
    {
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        public RegisterNewUser()
        {

            client = new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName("us-east-2"));
            context = new DynamoDBContext(client);
        }


        public string AddNewUser(string input, ILambdaContext lambdaContext)
        {
            Console.WriteLine("Received new string object: " + input);
            var json = JObject.Parse(input);
            var inputObject = json.ToObject<UserModel>();
            Console.WriteLine(string.Format("Parsed Json object: {0}", inputObject.ToString()));

            UserDocument newItem = new UserDocument() {
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = inputObject.PhoneNumber,
                Lat = inputObject.Latitude,
                Lng = inputObject.Longtitude

            };

            if(context ==null)
            {
                Console.WriteLine("Context has not been initialized!");
                Console.WriteLine("Aborting further execution...");
                return "Failed...";
            }
            else
            {
                Console.WriteLine("Adding new user to Dynamo DB..");
                context.Save(newItem);
                Console.WriteLine("User added.");
                return newItem.ToString();
            }
            
        }
    }
}
