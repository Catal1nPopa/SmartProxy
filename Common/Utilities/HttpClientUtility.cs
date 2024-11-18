using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public class HttpClientUtility
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<HttpResponseMessage> SendJsonAsync(string json, string url, string method)
        {
            var httpMethod = new HttpMethod(method.ToUpper());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(httpMethod, url)
            {
                Content = content
            };

            try
            {
                var response = await _client.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                // Loghează eroarea sau gestioneaz-o conform cerințelor
                throw new HttpRequestException($"Eroare în trimiterea cererii către {url}: {ex.Message}", ex);
            }
        }
    }
}
