using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Design;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper
{
    public class SecurityTokenHelperElement : BehaviorExtensionElement
    {
        public SecurityTokenHelperElement()
        {
        }
        public override Type BehaviorType
        {
            get { return typeof(SecurityTokenHelperBehavior); }
        }

        protected override object CreateBehavior()
        {
            HeaderCollection headers = new HeaderCollection();
            var m = Regex.Match(Headers, @"(?m)^(?<Header>[a-zA-Z\-]+)\s*:\s*(?<Value>.+?)\s*\r?$");
            while (m.Success)
            {
                headers.Add(m.Groups["Header"].Value, m.Groups["Value"].Value);
                m = m.NextMatch();
            }
            return new SecurityTokenHelperBehavior(AuthEndpoint, HttpMethod, headers, ContentType, Body, TokenPath, TokenKey, TokenPrefix, TokenSuffix, TokenUsage, TokenId, CacheToken, TokenExpiresIn);
        }

        [Category("Auth Url Info")]
        [ConfigurationProperty("AuthEndpoint", IsRequired = true)]
        public string AuthEndpoint
        {
            get { return (string)this["AuthEndpoint"]; }
            set { this["AuthEndpoint"] = value; }
        }

        [Category("Auth Url Info")]
        [ConfigurationProperty("HttpMethod", IsRequired = true, DefaultValue = null)]
        public HttpMethodEnum HttpMethod
        {
            get { return (HttpMethodEnum)this["HttpMethod"]; }
            set { this["HttpMethod"] = value; }
        }

        [Category("Auth Url Info")]
        [ConfigurationProperty("Headers", IsRequired = false, DefaultValue = null)]
        [Description("Headers for the Auth call")]
        [RegexStringValidator(@"(?m)(^$)|^([a-zA-Z\-]+)\s*?:\s*?(.+)$")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Headers
        {
            get
            { return (string)this["Headers"]; }
            set
            {
                var m = Regex.Match(value, @"(?m)^(?<Header>[a-zA-Z\-]+)\s*:\s*(?<Value>.+?)\s*\r?$");
                StringBuilder sb = new StringBuilder();
                while (m.Success)
                {
                    sb.AppendLine($"{m.Groups["Header"].Value}: {m.Groups["Value"].Value}");
                    m = m.NextMatch();
                }
                value = sb.ToString();
                this["Headers"] = value;
            }
        }

        [Category("Auth Url Info")]
        [Description("Content Type for the Auth call")]
        [ConfigurationProperty("ContentType", IsRequired = false, DefaultValue = null)]
        public string ContentType
        {
            get { return (string)this["ContentType"]; }
            set { this["ContentType"] = value; }
        }

        [Category("Auth Url Info")]
        [Description("The body for the Auth call")]
        [ConfigurationProperty("Body", IsRequired = false, DefaultValue = null)]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Body
        {
            get { return (string)this["Body"]; }
            set { this["Body"] = value; }
        }

        [Category("Auth Url Info")]
        [Description("The token's path in the response, e.g.(for Json, user JPath like $..access_token, for xml, use XPath like /Response/Access_Token")]
        [ConfigurationProperty("TokenPath", IsRequired = true)]
        public string TokenPath
        {
            get { return (string)this["TokenPath"]; }
            set { this["TokenPath"] = value; }
        }

        [Category("Token Usage")]
        [Description("The token key, either in the header or in the query string, like 'Authorization' in the header, or 'token' in the query string")]
        [ConfigurationProperty("TokenKey", IsRequired = true)]
        public string TokenKey
        {
            get { return (string)this["TokenKey"]; }
            set { this["TokenKey"] = value; }
        }

        [Category("Token Usage")]
        [ConfigurationProperty("TokenPrefix", IsRequired = false)]
        [Description("A prefix to be added before the token when it is assigned to the token key in the main call, like 'Bearer '")]
        public string TokenPrefix
        {
            get { return (string)this["TokenPrefix"]; }
            set { this["TokenPrefix"] = value; }
        }

        [Category("Token Usage")]
        [Description("A suffix to be added to the token when it is assigned to the token key in the main call")]
        [ConfigurationProperty("TokenSuffix", IsRequired = false)]
        public string TokenSuffix
        {
            get { return (string)this["TokenSuffix"]; }
            set { this["TokenSuffix"] = value; }
        }

        [Category("Token Usage")]
        [Description("Defines how to use the returned token for the main call.")]
        [ConfigurationProperty("TokenUsage", IsRequired = true)]
        public TokenUsageEnum TokenUsage
        {
            get { return (TokenUsageEnum)this["TokenUsage"]; }
            set { this["TokenUsage"] = value; }
        }

        [Category("Token Usage")]
        [Description("Cache the token so the same token can be reused while it is still valid.")]
        [ConfigurationProperty("CacheToken", IsRequired = false)]
        public bool CacheToken
        {
            get
            { return (bool)this["CacheToken"]; }
            set { this["CacheToken"] = value; }
        }

        [Category("Token Usage")]
        [Description("TokenId is a unique identifier for the retrieved token, that allows to cache the token.")]
        [ConfigurationProperty("TokenId", IsRequired = false)]
        public Guid TokenId
        {
            get
            {
                if (this["TokenId"] == null || (Guid)this["TokenId"] == Guid.Empty) this["TokenId"] = Guid.NewGuid();
                return (Guid)this["TokenId"];
            }
            set { this["TokenId"] = value; }
        }
        [Category("Token Usage")]
        [Description("The expiry period in seconds")]
        [IntegerValidator(MinValue = 1)]
        [ConfigurationProperty("TokenExpiresIn", IsRequired = false, DefaultValue = 3300)]
        public int TokenExpiresIn
        {
            get { return (int)this["TokenExpiresIn"]; }
            set { this["TokenExpiresIn"] = value; }
        }
    }
}

