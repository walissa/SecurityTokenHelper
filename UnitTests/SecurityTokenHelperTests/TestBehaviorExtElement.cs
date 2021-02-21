using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizTalkComponents.WCFExtensions;
using System.ServiceModel;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using BizTalkComponents.WCFExtensions.SecurityTokenHelper;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TestAuthHelper
{
    [TestClass]
    public class TestBehaviorExtElement
    {
        public TestBehaviorExtElement()
        {
            TestHelper.CreateSevice();
        }

        [TestMethod]
        public void TestGetTokenWithAuthParamsAsHeaders()
        {
            HeaderCollection headers = new HeaderCollection();
            var m = Regex.Match(TestHelper.AuthParams, "(?<param>[^=&]+?)=(?<value>[^=&]+)");
            while (m.Success)
            {
                headers.Add(m.Groups["param"].Value, m.Groups["value"].Value);
                m = m.NextMatch();
            }
            string token = TokenHelper.GetToken(HttpMethodEnum.GET, TestHelper.basicUrl + "/GetTokenWithAuthParamsAsHeaders",
                headers, null, null, "$..Access_Token");
            Assert.IsTrue(token.StartsWith("As Headers"));
        }

        [TestMethod]
        public void TestGetTokenWithAuthParamsAsQueryString()
        {
            UriBuilder ub = new UriBuilder(TestHelper.basicUrl + "/GetTokenWithAuthParamsAsQueryString");
            ub.Query = TestHelper.AuthParams;
            string token = TokenHelper.GetToken(HttpMethodEnum.GET, ub.ToString(),
                null, null, null, "$..Access_Token");
            Assert.IsTrue(token.StartsWith("As QueryString"));
        }
        [TestMethod]
        public void TestGetTokenWithAuthParamsInBody()
        {
            string token = TokenHelper.GetToken(HttpMethodEnum.POST, TestHelper.basicUrl + "/GetTokenWithAuthParamsInBody",
                null, "text/plain", TestHelper.AuthParams, "$..Access_Token");
            Assert.IsTrue(token.StartsWith("In Body"));
        }


        [TestMethod]
        public void TestCallAPIWithTokenInHeader()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint =TestHelper.basicUrl+ "/GetTokenWithAuthParamsAsHeaders",
                ContentType = "application/json",
                Headers = "Accept: application/json\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.Header,
                TokenKey = "Authorization",
                TokenPrefix = "In Headers ",
                TokenPath = "$..Access_Token",
            };
            var behavior = TestHelper.GetBehavior(elm);
            var binding = new WebHttpBinding();
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string ret = channel.CallAPIWithTokenInHeader();
            Assert.IsTrue(ret.StartsWith("In Headers"));
        }

        [TestMethod]
        public void TestCallAPIWithTokenInQueryString()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint = TestHelper.basicUrl+ "/GetTokenWithAuthParamsAsHeaders",
                ContentType = "application/json",
                Headers = "Accept: application/json\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.QueyParameter,
                TokenKey = "Authorization",
                TokenPrefix = "In QueryString",
                TokenPath = "$..Access_Token",
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string ret = channel.CallAPIWithTokenInQueryString();
            Assert.IsTrue(ret.StartsWith("In QueryString"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCallAPIWithTokenPathNotFound()
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
                TokenPath = "$..Access_token",
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string ret = channel.CallAPIWithTokenInQueryString();
            Assert.IsTrue(ret.StartsWith("In QueryString"));
        }

        [TestMethod]
        public void TestCallAPIWithTokenResponseXml()
        {
            var elm = new SecurityTokenHelperElement
            {
                AuthEndpoint = TestHelper.basicUrl + "/GetTokenWithAuthParamsAsHeaders",
                ContentType = "application/json",
                Headers = "Accept: application/xml\n" + TestHelper.AuthParams.Replace("&", "\n").Replace("=", ":"),
                HttpMethod = HttpMethodEnum.GET,
                TokenUsage = TokenUsageEnum.QueyParameter,
                TokenKey = "Authorization",
                TokenPrefix = "In QueryString",
                TokenPath = "/Token/Access_Token",
            };
            var binding = new WebHttpBinding();
            var behavior = TestHelper.GetBehavior(elm);
            var factory = new ChannelFactory<ITestService>(binding, new EndpointAddress(TestHelper.basicUrl));
            factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            var channel = factory.CreateChannel();
            string ret = channel.CallAPIWithTokenInQueryString();
            Assert.IsTrue(ret.StartsWith("In QueryString"));
        }
    }
}
