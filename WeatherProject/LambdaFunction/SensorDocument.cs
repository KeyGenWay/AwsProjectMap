using Amazon.CloudFront;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;

namespace LambdaFunction
{
    [DynamoDBTable("sensors")]
    public class SensorDocument
    {
        [DynamoDBHashKey(AttributeName = "id")]
        public string Id { get; set; }

        [DynamoDBProperty(AttributeName ="latitude")]
        public double Latitude { get; set; }
        [DynamoDBProperty(AttributeName = "longitude")]
        public double Longtitude { get; set; }

        public double GetDistanceTo(double latitude, double longtitude)
        {
            var selfLocation = new GeoCoordinate(this.Latitude, this.Longtitude);
            var sensorLocation = new GeoCoordinate(latitude, longtitude);
            return selfLocation.GetDistanceTo(sensorLocation);
        }
    }
}
