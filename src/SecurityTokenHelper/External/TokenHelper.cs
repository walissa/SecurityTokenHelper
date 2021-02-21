using BizTalkComponents.WCFExtensions.SecurityTokenHelper.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper
{
    [Serializable]
    public static class TokenHelper
    {

        public static string GetBasicAuthEncoded(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");
            string credentials = string.Format("{0}:{1}", username, password);
            byte[] data = Encoding.UTF8.GetBytes(credentials);
            string encoded = Convert.ToBase64String(data);
            return encoded;
        }

        public static string GetToken(HttpMethodEnum method, string url, HeaderCollection headers, string contentType, string body, string tokenPath)
        {
            HttpClient client = new HttpClient();
            StringContent scontent = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(tokenPath))
            {
                throw new ArgumentNullException("TokenPath cannot be null", "tokenPath");
            }
            if (!string.IsNullOrEmpty(body))
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    throw new ArgumentNullException("ContentType cannot be null or empty when Body is specified", "contentType");
                }
                scontent = new StringContent(body);
                scontent.Headers.ContentType.MediaType = contentType;
            }
            string strMethod = Enum.GetName(typeof(HttpMethodEnum), method);
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(strMethod), uri) { Content = scontent };
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    if (kv.Key.ToLower() != "content-type")
                        request.Headers.Add(kv.Key, kv.Value);
                }
            }
            string retval = null, respContentType = null, respContent = "";
            try
            {
                var resp = client.SendAsync(request).GetAwaiter().GetResult();
                resp.EnsureSuccessStatusCode();
                respContent = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                respContentType = resp.Content.Headers.ContentType.MediaType.ToLower();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (respContentType.Contains("json"))
            {
                var jObj = JObject.Parse(respContent);
                JToken jt = jObj.SelectToken(tokenPath);
                retval = jt?.Value<string>();
            }
            else if (respContentType.Contains("xml"))
            {
                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(respContent);
                var node = xmldoc.SelectSingleNode(tokenPath);
                retval = node?.InnerText;
            }
            else
            {
                throw new InvalidCastException($"the authentication call returned unexpected content-type: {respContentType}");
            }
            if (retval == null)
            {
                throw new ArgumentException("The selected path for return value returned null.", "tokenPath");
            }
            return retval;
        }

        public static string GetToken(HttpMethodEnum method, string url, HeaderCollection headers, string contentType, string body, string tokenPath, Guid tokenId, int tokenExpiresIn, bool cachedToken)
        {
            if (!string.IsNullOrEmpty(body) & string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentNullException("ContentType cannot be null or empty when Body is specified", "contentType");
            }
            if (string.IsNullOrEmpty(tokenPath))
            {
                throw new ArgumentNullException("TokenPath cannot be null", "tokenPath");
            }
            string token = null;
            if (cachedToken)
            {
                if (tokenExpiresIn <= 0)
                {
                    throw new ArgumentException("tokenExpiresIn must have a value greater than zero.", "tokenExpiresIn");
                }
                if (tokenId == null || tokenId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid tokenId", "tokenId");
                }
                TokenInfo ti = AppDomainHelper.TokenDictionary.GetOrCreateTokenInfo(tokenId, tokenExpiresIn);
                lock (ti)
                {
                    if (ti.IsNew || !ti.IsValid | !cachedToken)
                    {
                        token = GetToken(method, url, headers, contentType, body, tokenPath);
                        ti.SetTokenInfo(token, DateTime.Now);
                        TokenDictionary.WriteLogMessage($"Get new token for TokenId '{tokenId}'.");
                    }
                    else
                    {
                        token = ti.Token;
                        TokenDictionary.WriteLogMessage($"Get token for TokenId '{tokenId}' from dictionary.");
                    }
                }
            }
            else
            {
                token = GetToken(method, url, headers, contentType, body, tokenPath);
                TokenDictionary.WriteLogMessage($"Get new token");
            }
            return token;
        }
    }
}
