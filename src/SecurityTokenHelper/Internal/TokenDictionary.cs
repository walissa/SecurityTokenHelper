using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace BizTalkComponents.WCFExtensions.SecurityTokenHelper.Internal
{

    internal class TokenDictionary : MarshalByRefObject
    {
        private ConcurrentDictionary<Guid, TokenInfo> tokenDic = new ConcurrentDictionary<Guid, TokenInfo>();
        private System.Timers.Timer localTimer = new System.Timers.Timer(60000);
        

        public TokenDictionary()
        {
            localTimer.Elapsed += LocalTimer_Elapsed;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void LocalTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var keys = tokenDic.Keys.ToList();
            foreach (Guid key in keys)
            {
                TokenInfo ti = tokenDic[key];
                if (!ti.IsValid)
                {
                    tokenDic.TryRemove(key, out ti);
                }
            }
            if (tokenDic.Count == 0)
            {
                localTimer.Enabled = false;
                WriteLogMessage("No more valid token available in the dictionary");
            }
        }

        internal TokenInfo GetOrCreateTokenInfo(Guid tokenId,int expiresIn)
        {
            var ti = tokenDic.GetOrAdd(tokenId, k => new TokenInfo(tokenId,expiresIn));
            localTimer.Enabled = true;
            return ti;
        }

        internal static void WriteLogMessage(string message, System.Diagnostics.EventLogEntryType evType = System.Diagnostics.EventLogEntryType.Information, [CallerMemberName] string procName = "")
        {
            System.Diagnostics.EventLog.WriteEntry("SecurityTokenHelper", string.Format("{0}\n{1}", procName, message), evType);
        }

    }
}
