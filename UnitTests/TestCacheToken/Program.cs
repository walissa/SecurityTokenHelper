using BizTalkComponents.WCFExtensions.SecurityTokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TestAuthHelper;

namespace TestCacheToken
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating Test service...");
            TestHelper.CreateSevice();
            Console.WriteLine("Test service has been created successfully.");
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Testing Retrieving token for the second call from cache...");
            TestCallAPIWithCachedToken();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Testing multi-threading with token cached for serveral calls...");
            TestCallAPIWithTokenMultiCall();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Testing Retrieving token for the second call from cache when the cached token expired...");
            TestCallAPIWithCachedTokenExpired();
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("All tests finished with success!");
            Console.ReadLine();
        }
        static void TestCallAPIWithCachedToken()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint = TestHelper.basicUrl + "/GetTokenWithAuthParamsAsHeaders",
                ContentType = "application/json",
                Headers = "Accept: application/json\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.QueyParameter,
                TokenKey = "Authorization",
                TokenPrefix = "In QueryString",
                TokenPath = "$..Access_Token",
                CacheToken = true,
                TokenExpiresIn = 1000,
                TokenId = Guid.NewGuid(),
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string token1 = channel.CallAPIWithTokenInQueryString();
            Console.WriteLine($"Retrieved token for the first call: token1={token1}");
            Console.WriteLine("Wait for 1s..");
            Task.Delay(1000).GetAwaiter().GetResult();
            string token2 = channel.CallAPIWithTokenInQueryString();
            Console.WriteLine($"Retrieved token for the second call: token2={token2}");
            if (token1 != token2)
                throw new Exception("Not Equal");
            Console.WriteLine($"token1=token2");
        }

        static void TestCallAPIWithCachedTokenExpired()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint = TestHelper.basicUrl + "/GetTokenWithAuthParamsAsHeaders",
                ContentType = "application/json",
                Headers = "Accept: application/json\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.QueyParameter,
                TokenKey = "Authorization",
                TokenPrefix = "In QueryString",
                TokenPath = "$..Access_Token",
                CacheToken = true,
                TokenExpiresIn = 2,
                TokenId = Guid.NewGuid(),
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string token1 = channel.CallAPIWithTokenInQueryString();
            Console.WriteLine($"Retrieved token for the first call: token1={token1}");
            Console.WriteLine("Wait for 3s until the token gets expired...");
            Task.Delay(3000).GetAwaiter().GetResult();
            string token2 = channel.CallAPIWithTokenInQueryString();
            Console.WriteLine($"Retrieved token for the second call: token2={token2}");
            if (token1 == token2)
                throw new Exception("Must not be equal");
            Console.WriteLine("token1!=token2");
        }

        static void TestCallAPIWithTokenMultiCall()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint = TestHelper.basicUrl + "/GetToken",
                ContentType = "application/json",
                Headers = "Accept: application/json\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.QueyParameter,
                TokenKey = "Authorization",
                TokenPrefix = "In QueryString",
                TokenPath = "$..Access_Token",
                CacheToken = true,
                TokenExpiresIn = 10,
                TokenId = Guid.NewGuid(),
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string token1 = "a", token2 = "b", token3 = "c", token4 = "d", token5 = "e";
            var task1 = Task.Run(() => token1 = channel.CallAPIWithTokenInQueryString());
            var task2 = Task.Run(() => token2 = channel.CallAPIWithTokenInQueryString());
            var task3 = Task.Run(() => token3 = channel.CallAPIWithTokenInQueryString());
            var task4 = Task.Run(() => token4 = channel.CallAPIWithTokenInQueryString());
            var task5 = Task.Run(() => token5 = channel.CallAPIWithTokenInQueryString());
            Task.WaitAll(task1, task2, task3, task4, task5);
            Console.WriteLine($"Retrieved token for the first call: token1={token1}");
            Console.WriteLine($"Retrieved token for the second call: token2={token2}");
            Console.WriteLine($"Retrieved token for the third call: token3={token3}");
            Console.WriteLine($"Retrieved token for the fourth call: token4={token4}");
            Console.WriteLine($"Retrieved token for the fifth call: token5={token5}");
            if (token1 != token2 | token2!=token3 | token3!=token4 | token4!=token5)
                throw new Exception("Not equal");
            Console.WriteLine($"token1=token2=token3=token4=token5");
        }
    }
}
