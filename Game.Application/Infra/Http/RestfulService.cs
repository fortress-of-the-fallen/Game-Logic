using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Interface;
using Newtonsoft.Json;

namespace Game.Application.Infra.Http
{
    public class RestfulService : IRestfulService
    {
        // private readonly ILogger<RestfulService> _logger;
        private readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        // public RestfulService(ILogger<RestfulService> logger)
        // {
        //     _logger = logger;
        // }

        public async Task<(bool, HttpStatusCode)> Delete(string url, Dictionary<string, string> headers = null, int timeout = 30, CancellationToken token = default, bool force = false)
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    var response = await client.DeleteAsync(url, effectiveToken).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return (response.IsSuccessStatusCode, response.StatusCode);
                    }

                    return (false, response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError("DELETE request timed out after {timeout} seconds", timeout);
                return (false, HttpStatusCode.RequestTimeout);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Get<TResult>(
            string url,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    var response = await client.GetAsync(url, effectiveToken);

                    TResult result = null;

                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync();

                        if (typeof(TResult) == typeof(string))
                        {
                            result = (TResult)(object)resultAsString;
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>(resultAsString);
                        }
                    }
                    else
                    {
                        // _logger.LogError($"Request failed (Status: {response.StatusCode}): {await response.Content.ReadAsStringAsync()}");
                        return (null, response.StatusCode);
                    }

                    return (result, response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError($"Request timed out after {timeout} seconds");
                return (null, HttpStatusCode.RequestTimeout);
            }
        }


        public Task<(TResult, HttpStatusCode)> Patch<TResult>(string url, object rawData, Dictionary<string, string> headers = null, int timeout = 30, CancellationToken token = default, bool force = false) where TResult : class
        {
            throw new System.NotImplementedException();
        }

        public async Task<(TResult, HttpStatusCode)> Post<TResult>(string url, object rawData, Dictionary<string, string> headers = null, int timeout = 30, CancellationToken token = default, bool force = false) where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    var jData = JsonConvert.SerializeObject(rawData);
                    var content = new StringContent(jData, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content, effectiveToken).ConfigureAwait(false);

                    TResult result;
                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (typeof(TResult) == typeof(string))
                        {
                            result = (TResult)(object)resultAsString;
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>(resultAsString);
                        }
                    }
                    else
                    {
                        // _logger.LogError($"Request failed (Status: {response.StatusCode}): {JsonConvert.SerializeObject(rawData)}");
                        return (null, response.StatusCode);
                    }

                    return (result, response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError($"Request timed out after {timeout} seconds");
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Post<TResult>(
            string url,
            Dictionary<string, object> formFields = null,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    using (var formData = new MultipartFormDataContent())
                    {
                        if (formFields != null && formFields.Count > 0)
                        {
                            foreach (var field in formFields)
                            {
                                if (field.Value is FileStream fs)
                                {
                                    var fileContent = new StreamContent(fs);
                                    var fileName = Path.GetFileName(fs.Name);
                                    formData.Add(fileContent, field.Key, fileName);
                                    disposableStreams.Add(fs);
                                }
                                else
                                {
                                    formData.Add(new StringContent(field.Value != null ? field.Value.ToString() : string.Empty), field.Key);
                                }
                            }
                        }

                        var response = await client.PostAsync(url, formData, effectiveToken);

                        TResult result = null;

                        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var resultAsString = await response.Content.ReadAsStringAsync();

                            if (typeof(TResult) == typeof(string))
                            {
                                result = (TResult)(object)resultAsString;
                            }
                            else
                            {
                                result = JsonConvert.DeserializeObject<TResult>(resultAsString);
                            }
                        }
                        else
                        {
                            var responseJson = await response.Content.ReadAsStringAsync();
                            // _logger.LogError($"POST form data request failed (Status: {response.StatusCode}): {responseJson}");
                            return (null, response.StatusCode);
                        }

                        return (result, response.StatusCode);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError("POST request timed out after {0} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
            finally
            {
                foreach (var s in disposableStreams)
                {
                    s.Dispose();
                }
            }
        }


        public async Task<(TResult, HttpStatusCode)> Put<TResult>(string url, object rawData, Dictionary<string, string> headers = null, int timeout = 30, CancellationToken token = default, bool force = false) where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    var jData = JsonConvert.SerializeObject(rawData);
                    var content = new StringContent(jData, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(url, content, effectiveToken).ConfigureAwait(false);

                    TResult result;
                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (typeof(TResult) == typeof(string))
                        {
                            result = (TResult)(object)resultAsString;
                        }
                        else
                        {
                            result = JsonConvert.DeserializeObject<TResult>(resultAsString);
                        }
                    }
                    else
                    {
                        // _logger.LogError("PUT request failed (Status: {statusCode}): {responseJson}", response.StatusCode, JsonConvert.SerializeObject(rawData));
                        return (null, response.StatusCode);
                    }

                    return (result, response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError("PUT request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        public async Task<(TResult, HttpStatusCode)> Put<TResult>(string url, Dictionary<string, object> formFields = null, Dictionary<string, string> headers = null, int timeout = 30, CancellationToken token = default, bool force = false) where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    var effectiveToken = linkedCts.Token;

                    ApplyHeaders(client, headers);

                    using (var formData = new MultipartFormDataContent())
                    {
                        if (formFields != null && formFields.Count != 0)
                        {
                            foreach (var field in formFields)
                            {
                                if (field.Value is FileStream fs)
                                {
                                    var fileContent = new StreamContent(fs);
                                    var fileName = Path.GetFileName(fs.Name);
                                    formData.Add(fileContent, field.Key, fileName);
                                    disposableStreams.Add(fs);
                                }
                                else
                                {
                                    formData.Add(new StringContent(field.Value?.ToString() ?? string.Empty), field.Key);
                                }
                            }
                        }

                        var response = await client.PutAsync(url, formData, effectiveToken).ConfigureAwait(false);

                        TResult result = null;
                        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                            if (typeof(TResult) == typeof(string))
                            {
                                result = (TResult)(object)resultAsString;
                            }
                            else
                            {
                                result = JsonConvert.DeserializeObject<TResult>(resultAsString);
                            }
                        }
                        else
                        {
                            // _logger.LogError("PUT form data request failed (Status: {statusCode}): {responseJson}",
                                // response.StatusCode, JsonConvert.SerializeObject(formFields));
                            return (null, response.StatusCode);
                        }

                        return (result, response.StatusCode);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // _logger.LogError("PUT request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
            finally
            {
                foreach (var s in disposableStreams)
                {
                    s.Dispose();
                }
            }
        }

        private void ApplyHeaders(HttpClient client, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }
    }
}