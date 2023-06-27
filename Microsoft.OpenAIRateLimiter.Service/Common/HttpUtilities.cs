using Newtonsoft.Json;
using System.Net;
using System.Text;


namespace Microsoft.OpenAIRateLimiter.Service.Common
{
    public static class HttpUtilities
    {

        public static HttpResponseMessage RESTResponse<T>(T model)
        {

            HttpStatusCode httpStatusCode;

            if (model is Exception)
                httpStatusCode = HttpStatusCode.InternalServerError;
            else if (model is null)
                httpStatusCode = HttpStatusCode.BadRequest;
            else
                httpStatusCode = HttpStatusCode.OK;

            return GenerateRequestMessage(model, httpStatusCode);
        }

        public static HttpResponseMessage GenerateRequestMessage<T>(T model, HttpStatusCode statusCode)
        {
            return new HttpResponseMessage(statusCode) { Content = GenerateJSONContent(model) };
        }

        private static StringContent GenerateJSONContent<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model, Formatting.Indented),
                                                 Encoding.UTF8,
                                                 "application/json");
        }

    }
}
