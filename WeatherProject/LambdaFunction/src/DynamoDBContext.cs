using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
namespace LambdaFunction.src
{
	class DynamoDBContext
	class DynamoDBContext
	{
		private AmazonDynamoDBClient amazonDbClient;

		private Table UsersTable;
		private string UsersTableName = "Users";
		public DynamoDBContext()
		{
			AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
			config.ServiceURL = "https://dynamodb.us-east-2.amazonaws.com";
			AmazonDynamoDBClient client = new AmazonDynamoDBClient(new PrivateAwsCredentials(), config);
			UsersTable = Table.LoadTable(client, UsersTableName);
		}
		public async void AddNewUser(UserDocument newUser)
		{
			await UsersTable.PutItemAsync(newUser, null);
		}
	}
	class UserDocument :Document
	{
		public long Id => GetPropertyValue<long>(IdName);
		public string PhoneNumber => GetPropertyValue<string>(PhoneNumber);

		public decimal Longtitude => GetPropertyValue<decimal>(LongtitudeName);
		public decimal Latitude => GetPropertyValue<decimal>(LatitudeName);

		private const string IdName = "Id";
		private const string PhoneNumberName = "PhoneNumber";
		private const string LongtitudeName = "Lng";
		private const string LatitudeName = "Lat";


		private TType GetPropertyValue<TType>(string propertyName)
		{
			DynamoDBEntry result;
			this.TryGetValue(propertyName, out result);
			return (TType)result.AsPrimitive().Value;
		}

		public UserDocument(long id, string number,decimal longtitude, decimal latitude)
		{
			this.Add(IdName, id);
			this.Add(PhoneNumberName, number);
			this.Add(LongtitudeName, longtitude);
			this.Add(LatitudeName, latitude);
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
