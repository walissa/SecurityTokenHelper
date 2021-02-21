namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper
{
    public class HeaderKeyValue
    {
        public HeaderKeyValue() { }
        public HeaderKeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Key) | string.IsNullOrEmpty(Value)
                ? "New Key"
                : $"{Key}: {Value}";
        }
    }
}
