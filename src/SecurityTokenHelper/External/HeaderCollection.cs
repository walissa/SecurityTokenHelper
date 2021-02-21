using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper
{
    public class HeaderCollection : List<HeaderKeyValue>
    {
        public HeaderCollection() : base() { }
        public HeaderCollection(int capacity) : base(capacity) { }

        public HeaderCollection(IEnumerable<HeaderKeyValue> collection) : base(collection) { }

        public void Add(string key, string value)
        {
            this.Add(new HeaderKeyValue(key, value));
        }
        public new void Add(HeaderKeyValue item)
        {
            base.Add(item);
        }
        public new void AddRange(IEnumerable<HeaderKeyValue> collection)
        {
            base.AddRange(collection);
        }

        public new bool Remove(HeaderKeyValue item)
        {
            bool retval = base.Remove(item);
            return retval;
        }
        public new void RemoveAt(int index)
        {
            var item = base[index];
            base.RemoveAt(index);
        }
        public  new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }
        public new int RemoveAll(Predicate<HeaderKeyValue> match)
        {
            return base.RemoveAll(match);
        }

    }
}
