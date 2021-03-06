﻿using CurrencyAppShared.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyAppShared.Services
{
    public class RestService : IRestService
    {
        private HttpClient _client;
        private StringBuilder _mainUrl;
        private Uri _uri;
        HttpRequestMessage _httpRequestMessage;

        public RestService(string ApiPrefix)
        {
            _client = new HttpClient();
            _mainUrl = new StringBuilder(@"http://api.nbp.pl/api/exchangerates/").Append(ApiPrefix);
            _uri = new Uri(_mainUrl.ToString());
        }

        public async Task<string> GetDataAsync(string urlPrefix)
        {
            InitRequest();
            _httpRequestMessage.RequestUri = new Uri(_uri, urlPrefix);
            HttpResponseMessage response = null;
            try
            {
                response = await _client.SendAsync(_httpRequestMessage);
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("exception HttpRequestException/No internet Connection");
            }
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return content;
            }
            return "error";
        }

        public Task DeleteDataAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task PostDataAsync(string urlPrefix, string item, bool isNewItem = false)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDataAsync(string item)
        {
            throw new NotImplementedException();
        }
        private void InitRequest()
        {
            _httpRequestMessage = new HttpRequestMessage();
            _httpRequestMessage.Headers.Add("Accept", "application/xml");
        }
    }
}
