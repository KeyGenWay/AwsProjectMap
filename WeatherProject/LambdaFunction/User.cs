using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.ECS.Model;
using Amazon.Runtime;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LambdaFunction
{
    [DataContract]
    public class UserModel
    {
        [DataMember(Name = PhoneNumberName)]
        public string PhoneNumber { get; set; }
        [DataMember(Name = LongtitudeName)]
        public double Longtitude { get; set; }
        [DataMember(Name = LatitudeName)]
        public double Latitude { get; set; }

        private const string PhoneNumberName = "PhoneNumber";
        private const string LongtitudeName = "Lng";
        private const string LatitudeName = "Lat";

        public UserModel() { }
        public UserModel( string number, double longtitude, double latitude)
        {
            this.PhoneNumber = number;
            this.Longtitude = longtitude;
            this.Latitude = latitude;
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", PhoneNumber, Longtitude, Latitude);
        }
    }


    [DynamoDBTable("Users")]
    public class UserDocument
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string PhoneNumber { get; set; }
        [DynamoDBProperty]
        public double Lng { get; set; }
        [DynamoDBProperty]
        public double Lat { get; set; }
    }
}
