
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Transform;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http;
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
            var request = PrepareRequest(inputString);
            var testObject = new RegisterNewUser();
            
            var response =  testObject.AddNewUser(request, lambdaMock.Object);

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(RegisterNewUser.SuccesfullyResponseMessage, response.Body);
        }

        [Theory]
        [InlineData("{ \"PhoneNumber\": \"500 341 924\",  \"Lng\": 22.22, \"Lat 44.44 }")]
        [InlineData("{ \"PhoneNumber\": \"500 341 924\", \"Lat 44.44 }")]
        [InlineData("{ \"Pho\"Lat 44.ne4Number\": \"500 341 92\", 44 }")]
        public void ShouldThrowErrorForIncorrectJson(string inputString)
        {
            var request = PrepareRequest(inputString);
            var testObject = new RegisterNewUser();

            var response = testObject.AddNewUser(request, lambdaMock.Object);

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(RegisterNewUser.JsonParseErrorResponseMessage, response.Body);
        }

        [Theory]
        [InlineData("{ \"PhoneNumber\": \"500 341 924\",  \"Longng\": 22.22, \"Latitude\": 44.44 }")]
        [InlineData("{ \"PhNNumber\": \"500 341 924\",  \"Longtitude\": 22.22, \"Lattt\": 44.44 }")]
        public void ShouldThrowErrorForIncorrectJsonModelSchema(string inputString)
        {
            var request = PrepareRequest(inputString);
            var testObject = new RegisterNewUser();

            var response = testObject.AddNewUser(request, lambdaMock.Object);

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(RegisterNewUser.JsonParseErrorResponseMessage, response.Body);
        }
        [Fact]
        public void SHouldReturnHealthCheckWithGet()
        {
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();
            request.HttpMethod = HttpMethod.Get.ToString();
            var testObject = new RegisterNewUser();

            var response = testObject.AddNewUser(request, lambdaMock.Object);

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(RegisterNewUser.HealthCheckResponse, response.Body);
        }

        private APIGatewayProxyRequest PrepareRequest(string body)
        {
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();
            request.Body = body;
            request.HttpMethod = HttpMethod.Post.ToString();
            request.IsBase64Encoded = false;

            return request;
        }
    }

}
