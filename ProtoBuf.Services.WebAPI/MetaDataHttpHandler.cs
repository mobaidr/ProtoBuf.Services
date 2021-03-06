using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using ProtoBuf.Services.Infrastructure;
using ProtoBuf.Services.Infrastructure.Encryption;

namespace ProtoBuf.Services.WebAPI
{
    public class MetaDataHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var metaDataProvider = new JsonMetaDataProvider();

            if (!request.Headers.Contains(RestfulServiceConstants.RqModelTypeHeaderKey))
                return GetMetaNotFoundResponse("required head key was missing");

            var rqType = request.Headers.GetValues(RestfulServiceConstants.RqModelTypeHeaderKey).ToList();

            if (rqType.Count == 0)
                return GetMetaNotFoundResponse("required header key was empty");

            var modelType = Type.GetType(EncryptionManager.Decrypt(rqType[0]), false);

            if (modelType == null)
                return GetMetaNotFoundResponse("requested model type was not found");

            return Task.Factory.StartNew(() => 
                new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(metaDataProvider.GetMetaData(modelType))
                    });
        }

        private static Task<HttpResponseMessage> GetMetaNotFoundResponse(string message)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.PreconditionFailed){Content = new StringContent(message)});
        }
    }
}