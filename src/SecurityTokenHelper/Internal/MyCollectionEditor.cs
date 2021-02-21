using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper.Internal
{
    public class MyCollectionEditor : CollectionEditor
    {
        public MyCollectionEditor(Type type)
            : base(type)
        { }
        protected override IList GetObjectsFromInstance(object instance)
        {
            return base.GetObjectsFromInstance(instance);
        }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return base.EditValue(context, provider, value);

        }
        protected override object SetItems(object editValue, object[] value)
        {

            // var ss=(PropertyDescriptorGridEntry)this.Context.PropertyDescriptor;

            this.Context.PropertyDescriptor.AddValueChanged(this.Context, new EventHandler(abc));
            return base.SetItems(editValue, value);
        }
        private void abc(object sender, EventArgs e)
        {

        }
    }
}
