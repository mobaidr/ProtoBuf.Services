using System;
using System.Net;

namespace ProtoBuf.Services.WebAPI.Client
{
    internal sealed class ExtendedWebClient : WebClient
    {
        private TimeSpan _timeout;

        public ExtendedWebClient(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var rq = base.GetWebRequest(address) as HttpWebRequest;

            if (rq != null)
            {
                rq.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                rq.Timeout = Convert.ToInt32(_timeout.TotalMilliseconds);
            }

            return rq;
        }
    }
}