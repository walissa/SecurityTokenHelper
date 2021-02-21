using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestAuthHelper
{
    [ServiceContract]
    internal interface ITestService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Token GetToken();

        [OperationContract(Name = "GetToken2")]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Token GetTokenPost();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Token GetTokenWithAuthParamsAsHeaders();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Token GetTokenWithAuthParamsAsQueryString();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Token GetTokenWithAuthParamsInBody(Stream stream);




        [OperationContract]
        string CallAPIWithToken();


        [OperationContract]
        string CallAPIWithTokenInHeader();

        [OperationContract]
        string CallAPIWithTokenInQueryString();


    }
    internal class TestService : ITestService
    {
        //[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string CallAPIWithToken()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            return request.Headers[HttpRequestHeader.Authorization];
        }

        public string CallAPIWithTokenInHeader()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            return request.Headers[HttpRequestHeader.Authorization];
        }

        public string CallAPIWithTokenInQueryString()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            return request.UriTemplateMatch.QueryParameters["Authorization"];
        }

        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Token GetToken()
        {
            Task.Delay(1000).GetAwaiter().GetResult();
            return new Token { Access_Token = Guid.NewGuid().ToString() };
        }
        public Token GetTokenPost()
        {
            return new Token { Access_Token = Guid.NewGuid().ToString() };
        }


        public Token GetTokenWithAuthParamsAsHeaders()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            var m = Regex.Match(TestHelper.AuthParams, "(?<param>[^=&]+?)=(?<value>[^=&]+)");
            bool check = true;
            while (m.Success)
            {
                check &= request.Headers[m.Groups["param"].Value] == m.Groups["value"].Value;
                m = m.NextMatch();
            }
            if (!check)
            {
                throw new InvalidOperationException("Headers are not correct");
            }
            return new Token { Access_Token = $"As Headers:{ Guid.NewGuid()}" };
        }
        public Token GetTokenWithAuthParamsAsQueryString()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            var m = Regex.Match(TestHelper.AuthParams, "(?<param>[^=&]+?)=(?<value>[^=&]+)");
            bool check = true;
            while (m.Success)
            {
                check &= request.UriTemplateMatch.QueryParameters[m.Groups["param"].Value] == m.Groups["value"].Value;
                m = m.NextMatch();
            }
            if (!check)
            {
                throw new InvalidOperationException("QueryString Params are not correct");
            }
            return new Token { Access_Token = $"As QueryString:{ Guid.NewGuid()}" };
        }
        public Token GetTokenWithAuthParamsInBody(Stream stream)
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            string body = (new StreamReader(stream)).ReadToEnd();
            if (body != TestHelper.AuthParams)
            {
                throw new InvalidOperationException("The body content does not match");
            }
            return new Token { Access_Token = $"In Body:{ Guid.NewGuid()}" };
        }
    }

    [DataContract(Namespace = "")]
    public class Token
    {
        [DataMember]
        public string Access_Token { get; set; }
    }
}
