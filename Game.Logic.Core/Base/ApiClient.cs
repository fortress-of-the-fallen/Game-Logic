using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AFusion.Rpa.Sdk.Orchestrator.Helpers;

namespace AFusion.Rpa.Sdk.Orchestrator.Services.Base
{
    /// <summary>
    /// Provides HTTP client functionality for making API requests (GET, POST, PUT, DELETE) with support for base URL management,
    /// custom headers, and logging. Designed as a singleton for shared use across the application.
    /// </summary>
    public class ApiClient : Singleton<ApiClient>
    {
        private readonly HttpClient _httpClient;
        private readonly LogService _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        private ApiClient()
        {
            _httpClient = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
            _logger = LogService.Instance;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
        /// <param name="url">The request URL (relative or absolute).</param>
        /// <param name="headers">Optional custom headers to include in the request.</param>
        /// <param name="timeout">Request timeout in seconds (used if force is true).</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="force">If true, creates a new HttpClient instance for this request.</param>
        /// <returns>A tuple containing the deserialized result (or null) and the HTTP status code.</returns>
        public async Task<(TResult?, HttpStatusCode)> Get<TResult>(
            string url,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                var response = await client.GetAsync(url, effectiveToken).ConfigureAwait(false);

                TResult? result;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("Request failed (Status: {statusCode}): {responseJson}", response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("Request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given data and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
        /// <param name="url">The request URL (relative or absolute).</param>
        /// <param name="rawData">The data to send in the request body.</param>
        /// <param name="headers">Optional custom headers to include in the request.</param>
        /// <param name="timeout">Request timeout in seconds (used if force is true).</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="force">If true, creates a new HttpClient instance for this request.</param>
        /// <returns>A tuple containing the deserialized result (or null) and the HTTP status code.</returns>
        public async Task<(TResult?, HttpStatusCode)> Post<TResult>(
            string url,
            object rawData,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                var jData = rawData.ToJson();
                var content = new StringContent(jData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content, effectiveToken).ConfigureAwait(false);

                TResult? result;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("Request failed (Status: {statusCode}): {responseJson}", response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("Request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given data and deserializes the response to the specified type.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formFields"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <param name="force"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<(TResult?, HttpStatusCode)> Post<TResult>(
            string url,
            Dictionary<string, object?>? formFields = null,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                using var formData = new MultipartFormDataContent();
                if (formFields is { Count: > 0 })
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

                var response = await client.PostAsync(url, formData, effectiveToken).ConfigureAwait(false);

                TResult? result = null;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("POST form data request failed (Status: {statusCode}): {responseJson}",
                        response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("POST request timed out after {timeout} seconds", timeout);
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

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given data and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
        /// <param name="url">The request URL (relative or absolute).</param>
        /// <param name="rawData">The data to send in the request body.</param>
        /// <param name="headers">Optional custom headers to include in the request.</param>
        /// <param name="timeout">Request timeout in seconds (used if force is true).</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="force">If true, creates a new HttpClient instance for this request.</param>
        /// <returns>A tuple containing the deserialized result (or null) and the HTTP status code.</returns>
        public async Task<(TResult?, HttpStatusCode)> Put<TResult>(
            string url,
            object rawData,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                var jData = rawData.ToJson();
                var content = new StringContent(jData, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content, effectiveToken).ConfigureAwait(false);

                TResult? result;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("PUT request failed (Status: {statusCode}): {responseJson}", response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("PUT request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given data and deserializes the response to the specified type.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formFields"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <param name="force"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<(TResult?, HttpStatusCode)> Put<TResult>(
            string url,
            Dictionary<string, object?>? formFields = null,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            var disposableStreams = new List<Stream>();
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                using var formData = new MultipartFormDataContent();
                if (formFields is { Count: > 0 })
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

                TResult? result = null;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("PUT form data request failed (Status: {statusCode}): {responseJson}",
                        response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("PUT request timed out after {timeout} seconds", timeout);
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

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The request URL (relative or absolute).</param>
        /// <param name="headers">Optional custom headers to include in the request.</param>
        /// <param name="timeout">Request timeout in seconds (used if force is true).</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="force">If true, creates a new HttpClient instance for this request.</param>
        /// <returns>A tuple containing a boolean indicating success and the HTTP status code.</returns>
        public async Task<(bool, HttpStatusCode)> Delete(
            string url,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                var response = await client.DeleteAsync(url, effectiveToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (response.IsSuccessStatusCode, response.StatusCode);
                }

                _logger.Error("DELETE request failed (Status: {statusCode})", response.StatusCode);
                return (false, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("DELETE request timed out after {timeout} seconds", timeout);
                return (false, HttpStatusCode.RequestTimeout);
            }
        }

        /// <summary>
        /// Sends an HTTP PATCH request to the specified URL with the given data and deserializes the response to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to deserialize the response to.</typeparam>
        /// <param name="url">The request URL (relative or absolute).</param>
        /// <param name="rawData">The data to send in the request body.</param>
        /// <param name="headers">Optional custom headers to include in the request.</param>
        /// <param name="timeout">Request timeout in seconds (used if force is true).</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <param name="force">If true, creates a new HttpClient instance for this request.</param>
        /// <returns>A tuple containing the deserialized result (or null) and the HTTP status code.</returns>
        public async Task<(TResult?, HttpStatusCode)> Patch<TResult>(
            string url,
            object rawData,
            Dictionary<string, string>? headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false)
            where TResult : class
        {
            try
            {
                var client = !force ? _httpClient : new HttpClient();

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);
                var effectiveToken = linkedCts.Token;

                ApplyHeaders(client, headers);

                var jData = rawData.ToJson();
                var content = new StringContent(jData, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = content
                };

                var response = await client.SendAsync(requestMessage, effectiveToken).ConfigureAwait(false);

                TResult? result;
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (typeof(TResult) == typeof(string))
                    {
                        result = (TResult)(object)resultAsString;
                    }
                    else
                    {
                        result = resultAsString.ToObject<TResult>();
                    }
                }
                else
                {
                    _logger.Error("PATCH request failed (Status: {statusCode}): {responseJson}", response.StatusCode, response.ToJson());
                    return (null, response.StatusCode);
                }

                return (result, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                _logger.Error("PATCH request timed out after {timeout} seconds", timeout);
                return (null, HttpStatusCode.RequestTimeout);
            }
        }

        /// <summary>
        /// Applies custom headers to the specified <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> to apply headers to.</param>
        /// <param name="headers">A dictionary of header names and values.</param>
        private static void ApplyHeaders(
            HttpClient client,
            Dictionary<string, string>? headers = null)
        {
            if (headers == null || headers.Count == 0) return;

            client.DefaultRequestHeaders.Clear();

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}