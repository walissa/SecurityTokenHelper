using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper.Internal
{

    [Serializable]
    [DataContract(Namespace = "")]
    public class TokenInfo
    {
        //public TokenInfo() { }
        public TokenInfo(Guid tokenId,int expiresIn)
        {
            this.TokenId = tokenId;
            this.ExpiresIn = expiresIn;
            IsNew = true;
        }
        internal bool IsNew { get; private set; }
        public Guid TokenId { get; private set; }

        public int ExpiresIn { get; private set; }

        public string Token { get; private set; }

        public  DateTime DateCreated { get; private set; }

        public DateTime ExpiresOn
        {
            get { return DateCreated.AddSeconds(ExpiresIn); }
        }
        public bool IsValid
        {
            //5 minutes margin
            get { return ExpiresOn > DateTime.Now; }
        }
        internal void SetTokenInfo(string token,DateTime dateCreated)
        {
            this.Token = token;
            this.DateCreated = dateCreated;
            IsNew = false;
        }
    }
}
