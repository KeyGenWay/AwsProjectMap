
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace LambdaFunction.Tests
{
    public class LambdaFunctionTests
    {
        private readonly Mock<ILambdaContext> lambdaMock = new Mock<ILambdaContext>();
        [Theory]
        [InlineData("{ \"PhoneNumber\": \"500 341 924\",  \"Lng\": 22.22, \"Lat\": 44.44 }")]
        [InlineData("{ \"PhoneNumber\": \"123 551 222\",  \"Lng\": 23.22, \"Lat\": 55.44 }")]
        public  void ShouldRegisterNewUserWithJson(string inputString)
        {
            /*ar parsedJsonObject = new UserModel(, "500 341 924", 22.22, 44.44);*/
            var testObject = new RegisterNewUser();
            
             testObject.AddNewUser(inputString, lambdaMock.Object);

            //testObject.dynamoDbMock.Verify(x => x.AddNewUser(parsedJsonObject));

        }
    }

}
