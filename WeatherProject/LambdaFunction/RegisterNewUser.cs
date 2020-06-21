using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaFunction
{
    public class RegisterNewUser
    {

        public const string SuccesfullyResponseMessage = "User succesfully saved.";
        public const string ContextErrorResponseMessage = "Failed to create DB Context.";
        public const string JsonParseErrorResponseMessage = "Failed to parse payload. Incorrect json schema. Please provide { \"PhoneNumber\": \"{string}\",  \"Lng\": {decimal} \"Lat\": {decimal} }";
        public const string UnknownException = "Unkown exception. Please check logs.";

        public const string HealthCheckResponse = "Service is Healthy and listeing...";
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        public RegisterNewUser()
        {
            client = new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName("us-east-2"));
            context = new DynamoDBContext(client);
        }


        public APIGatewayProxyResponse AddNewUser(APIGatewayProxyRequest input, ILambdaContext lambdaContext)
        {
            var response = new APIGatewayProxyResponse();
            response.IsBase64Encoded = false;

            if (input.HttpMethod == HttpMethod.Get.ToString())
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = HealthCheckResponse;
                return response;
            }
            

            Console.WriteLine("Received new request: " + input);
            JObject jsonBody;
            UserModel user;

            try
            {
                IList<string> jsonErrorMessages;
                jsonBody = JObject.Parse(input.Body);
                var isValid = jsonBody.IsValid(UserModel.Schema, out jsonErrorMessages);
                if(isValid)
                {
                    user = jsonBody.ToObject<UserModel>();
                    Console.WriteLine(string.Format("Parsed Json object: {0}", user.ToString()));
                } else
                {

                    throw new JsonReaderException(string.Join("\n", jsonErrorMessages));
                }
                
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.ToString());
                response.Body = JsonParseErrorResponseMessage;
                response.Body += ex.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }
            catch (Exception commonEx)
            {
                Console.WriteLine(commonEx.ToString());
                response.Body = UnknownException;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return response;
            }

            UserDocument newItem = new UserDocument()
            {
                Id = Guid.NewGuid().ToString(),
                PhoneNumber = user.PhoneNumber,
                Lat = user.Latitude,
                Lng = user.Longtitude

            };


            if (context == null)
            {
                Console.WriteLine("Context has not been initialized!");
                Console.WriteLine("Aborting further execution...");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Body = ContextErrorResponseMessage;
                return response;
            }
            else
            {
                Console.WriteLine("Adding new user to Dynamo DB..");
                context.Save(newItem);
                Console.WriteLine("User added.");
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = SuccesfullyResponseMessage;
                return response;
            }
        }

        
    }

 
}

