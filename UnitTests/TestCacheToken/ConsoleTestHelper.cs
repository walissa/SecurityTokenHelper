using BizTalkComponents.WCFExtensions.SecurityTokenHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TestAuthHelper
{
    internal class TestHelper
    {
        internal const string basicUrl = "http://localhost:1234";
        internal const string AuthParams = "client-id=myclientId&client-secret=mysecret&grant-type=client_credentials";

        internal static ServiceHost svcHost;


        internal static void CreateSevice()
        {
            if (svcHost?.State == CommunicationState.Opened)
                return;
            WebHttpBinding binding = new WebHttpBinding();
            svcHost = new ServiceHost(typeof(TestService));
            var svcEP = svcHost.AddServiceEndpoint(typeof(ITestService), binding, basicUrl);
            svcEP.EndpointBehaviors.Add(new WebHttpBehavior() { AutomaticFormatSelectionEnabled = true });
            svcHost.Open();
            Task.Delay(2000).GetAwaiter().GetResult();
        }
        internal static SecurityTokenHelperBehavior GetBehavior(SecurityTokenHelperElement elm)
        {
            MethodInfo method = elm.GetType().GetMethod("CreateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);
            SecurityTokenHelperBehavior endpointBehavior = null;
            if (method != (MethodInfo)null)
            {
                try
                {
                    endpointBehavior = method.Invoke((object)elm, new object[0]) as SecurityTokenHelperBehavior;
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    throw;
                }
            }
            return endpointBehavior;
        }

    }
}
