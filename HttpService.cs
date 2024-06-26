﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LiveStatsPoller
{
    public class HttpService 
    {
        public async Task<TResponse> GetAsync<TResponse>(string requestUri, Dictionary<string, string> headerKeyValues)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(httpClientHandler);

            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var (key, value) in headerKeyValues.Where(headerKeyValue =>
                !client.DefaultRequestHeaders.Contains(headerKeyValue.Key)))
            {
                client.DefaultRequestHeaders.Add(key, value);
            }

            var response = await client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<TResponse>(body) : default;
        }
    }
}
