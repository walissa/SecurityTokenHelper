using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Collections.Specialized;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper
{


    public class SecurityTokenHelperBehavior : IClientMessageInspector, IEndpointBehavior
    {

        public string AuthEndpoint { get; set; }
        public HttpMethodEnum HttpMethod { get; set; }
        public HeaderCollection Headers { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }
        public string TokenPath { get; set; }

        public string TokenKey { get; set; }
        public string TokenPrefix { get; set; }
        public string TokenSuffix { get; set; }

        public bool CacheToken { get; set; }
        public Guid TokenId { get; set; }
        public int TokenExpiresIn { get; set; }

        public TokenUsageEnum TokenUsage { get; set; }

        public SecurityTokenHelperBehavior(string authEndpoint, HttpMethodEnum method, HeaderCollection headers,
            string contentType, string body, string tokenPath,
            string tokenKey, string tokenPrefix, string tokenSuffix, TokenUsageEnum tokenUsage,
            Guid tokenId, bool cacheToken, int tokenExpiresIn)
        {
            this.AuthEndpoint = authEndpoint;
            this.HttpMethod = method;
            this.Headers = headers;
            this.TokenPath = tokenPath;
            this.ContentType = contentType;
            this.Body = body;
            this.TokenKey = tokenKey;
            this.TokenPrefix = tokenPrefix;
            this.TokenSuffix = tokenSuffix;
            this.TokenUsage = tokenUsage;

            this.CacheToken = cacheToken;
            this.TokenId = tokenId;
            this.TokenExpiresIn = tokenExpiresIn;
        }
        public SecurityTokenHelperBehavior() { }

        #region IClientMessageInspector

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // do nothing
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest = null;
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            }

            if (httpRequest == null)
            {
                httpRequest = new HttpRequestMessageProperty()
                {
                    Method = "GET",
                    SuppressEntityBody = true
                };
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }
            string token = TokenHelper.GetToken(HttpMethod, AuthEndpoint, Headers, ContentType, Body, TokenPath, TokenId, TokenExpiresIn, CacheToken);
            if (!string.IsNullOrWhiteSpace(TokenPrefix)) token = TokenPrefix + token;
            if (!string.IsNullOrWhiteSpace(TokenSuffix)) token += TokenSuffix;
            if (TokenUsage == TokenUsageEnum.Header)
            {
                WebHeaderCollection headers = httpRequest.Headers;
                //Remove the header if already exists.
                headers.Remove(TokenKey);
                headers.Add(TokenKey, token);
            }
            else if (TokenUsage == TokenUsageEnum.QueyParameter)
            {
                token = $"{TokenKey}={token}";
                var builder = new UriBuilder(request.Headers.To);
                if (!string.IsNullOrEmpty(builder.Query)) token += "&";
                builder.Query = token + builder.Query;
                request.Headers.To = builder.Uri;
            }
            return null;
        }

        #endregion IClientMessageInspector

        #region IEndpointBehavior

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }


        #endregion IEndpointBehavior
    }
}

