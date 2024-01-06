using Newtonsoft.Json;
using System.Net;

namespace Application.Helpers
{
    public static class HttpRequestHelper
    {
        /// <summary>
        /// This method is made to read out Python exceptions in C# as they are differently handled.
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="HttpRequestException"></exception>
        public static void CheckStatusAndParseErrorMessageFromJsonData(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Headers.ContentLength > 0)
                {
                    string respStr = response.Content.ReadAsStringAsync().Result;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(respStr);
                    string message = dic["message"];
                    response.ReasonPhrase = message;
                }
                throw new HttpRequestException(response.ReasonPhrase, inner: null, response.StatusCode);
            }
            response.EnsureSuccessStatusCode(); // just in case this fails, use normal way of handling messages...
        }
    }
}
