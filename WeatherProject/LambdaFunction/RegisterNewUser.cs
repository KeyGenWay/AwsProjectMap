using Amazon;
using Amazon.CloudFront.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.RDS.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //public const string FrontendUrl = "http://weatherfrontend.s3-website.us-east-2.amazonaws.com";
        public const string FrontendUrl = "http://localhost:4200";

        private const string SnSTopic = "arn:aws:sns:us-east-2:464446151961:email-notification-topic";
        private const string SnSLambdaTopic = "arn:aws:sns:us-east-2:464446151961:join-lambdas";

        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        private readonly AmazonSimpleNotificationServiceClient snsClient;
        public RegisterNewUser()
        {
            client = new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName("us-east-2"));
            snsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint.GetBySystemName("us-east-2"));
            context = new DynamoDBContext(client);
        }


        public APIGatewayProxyResponse AddNewUser(APIGatewayProxyRequest input, ILambdaContext lambdaContext)
        {
            var response = new APIGatewayProxyResponse();
            response.IsBase64Encoded = false;
            response.Headers = new Dictionary<string, string>();
            response.Headers.Add("Content-Type", "text");
            response.Headers.Add("Access-Control-Allow-Origin", FrontendUrl);

            string origin;
            input.Headers.TryGetValue("Origin", out origin);
            if (origin != FrontendUrl )
            {
                response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return response;
            }
            
            Console.WriteLine("Received new request with method " + input.HttpMethod);
            if (input.HttpMethod == HttpMethod.Get.ToString())
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = HealthCheckResponse;
                return response;
            }
            

            Console.WriteLine("Received new request: " + input.Body);
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
                Email = user.EmailAddress,
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

                UserSnsModel snsModel = new UserSnsModel
                {
                    UserId = newItem.Id,
                    PhoneNumber = newItem.PhoneNumber,
                    StationId = newItem.StationId,
                    EmailAddress = newItem.Email
                };
                SendSnsNotification(snsModel);
                //CreateSubscriptionInSnS(newItem.Email, newItem.StationId);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Body = SuccesfullyResponseMessage;
                return response;
            }
        }
        private void GetNearestGeoStation(UserDocument user){

        }

        private void CreateSubscriptionInSnS(string email, int stationId)
        {
            Console.WriteLine("Starting creation of SnS Subscription...");
            Console.WriteLine(string.Format("Input parameters: email: {0}, stationId: {1}", email, stationId));
            SubscribeRequest request = new SubscribeRequest(SnSTopic, "email", email);
            var response = snsClient.Subscribe(request);
            string filterPolicyString = string.Format("{\"stationId\":[\"{0}\"]}", stationId);
            SetSubscriptionAttributesRequest attributeRequest = new SetSubscriptionAttributesRequest(response.SubscriptionArn, "FilterPolicy", filterPolicyString);
            snsClient.SetSubscriptionAttributes(attributeRequest);
        }

        private void SendSnsNotification(UserSnsModel model)
        {
            Console.WriteLine("Starting creation of SnS Lambda Topic...");
            Console.WriteLine(string.Format("Input parameters: {0}", model.ToString()));

            string payload = JsonConvert.SerializeObject(model);

            Console.WriteLine(string.Format("Serialized payload: {0}", payload));

            PublishRequest publishRequest = new PublishRequest(SnSLambdaTopic, payload);
            snsClient.Publish(publishRequest);

            Console.WriteLine("Succesfully published message.");

        }
        
    }

 
}

