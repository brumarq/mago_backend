using Newtonsoft.Json;
using System.Net;

namespace Application.Helpers
{
    public static class HttpRequestHelper
    {
        public static void CheckStatusAndParseErrorMessageFromJsonData(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content.Headers.ContentLength > 0)
                {
                    String respStr = response.Content.ReadAsStringAsync().Result;
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(respStr);
                    string message = dic["message"];
                    response.ReasonPhrase = message;
                }
                throw new HttpRequestException(response.ReasonPhrase, inner: null, response.StatusCode);
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
