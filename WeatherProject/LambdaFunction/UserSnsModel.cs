using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.ECS.Model;
using Amazon.Runtime;
using Newtonsoft.Json.Schema;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LambdaFunction
{
    [DataContract]
    public class UserSnsModel
    {
        [DataMember(Name = PhoneNumberName)]
        public string PhoneNumber { get; set; }
        [DataMember(Name = EmailAddressName)]
        public string EmailAddress { get; set; }
        [DataMember(Name = StationIdName)]
        public string StationId { get; set; }
        [DataMember(Name = UserIdName)]
        public string UserId{ get; set; }

        private const string PhoneNumberName = "phoneNumber";
        private const string EmailAddressName = "email";
        private const string UserIdName = "userId";
        private const string StationIdName= "stationId";

        public static readonly JsonSchema Schema = JsonSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'PhoneNumber': {'type':'string'},
                'Email': {'type':'string'},
                'Lng':{'type':'number'},
                'Lat':{'type':'number'},
              },
              'additionalProperties': false,
            }");

        public UserSnsModel() { }
        //public UserModel( string emailAddress, string number, double longtitude, double latitude)
        //{
        //    this.PhoneNumber = number;
        //    this.EmailAddress = emailAddress;
        //    this.Longtitude = longtitude;
        //    this.Latitude = latitude;
        //}
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", PhoneNumber,EmailAddress, UserId, StationId);
        }
    }
}
