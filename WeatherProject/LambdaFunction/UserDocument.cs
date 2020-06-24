using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json.Schema;

namespace LambdaFunction
{
    [DynamoDBTable("Users")]
    public class UserDocument
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string PhoneNumber { get; set; }
        [DynamoDBProperty]
        public string Email { get; set; }
        [DynamoDBProperty]
        public double Lng { get; set; }
        [DynamoDBProperty]
        public double Lat { get; set; }
       
        [DynamoDBProperty]
        public string StationId { get; set; }
    }
}
