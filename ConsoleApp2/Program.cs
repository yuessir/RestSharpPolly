using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using RestSharpPolly;
using RestSharpPolly.PolicyProviders;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using Serilog;

namespace ConsoleApp2
{
    public class Args
    {
    }

    public class Files
    {
    }

    public class Form
    {
    }

    public class Headers
    {
        [JsonProperty("Accept")]
        public string Accept { get; set; }

        [JsonProperty("Accept-Encoding")]
        public string AcceptEncoding { get; set; }

        [JsonProperty("Accept-Language")]
        public string AcceptLanguage { get; set; }

        [JsonProperty("Connection")]
        public string Connection { get; set; }

        [JsonProperty("Cookie")]
        public string Cookie { get; set; }

        [JsonProperty("Host")]
        public string Host { get; set; }

        [JsonProperty("Upgrade-Insecure-Requests")]
        public string UpgradeInsecureRequests { get; set; }

        [JsonProperty("User-Agent")]
        public string UserAgent { get; set; }
    }

    public class QueryResult
    {
        [JsonProperty("args")]
        public Args Args { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("files")]
        public Files Files { get; set; }

        [JsonProperty("form")]
        public Form Form { get; set; }

        [JsonProperty("headers")]
        public Headers Headers { get; set; }

        [JsonProperty("json")]
        public object Json { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    internal static class Program
    {
        private static int re = 0;

        private static async Task Main(string[] args)
        {
            var asyncPolicy = BuildTimeoutAndRetryAsyncPolicy(3, 2, 10);
            var asyncPolicy2 = BuildTimeoutAndRetryAsyncPolicy2(3, 2, 10);
            var asyncPolicy3 = BuildTimeoutAndRetryAsyncPolicy3(3, 2, 10);
            var client = RestClientFactory.Create(asyncPolicy);
           
            // var client2 = RestClientFactory<IRestResponse>.Create(asyncPolicy2);
            // var client3 = RestClientFactory<IRestResponse<QueryResult>>.Create(asyncPolicy3);

            var syncPolicy2 = BuildTimeoutAndRetryPolicy(3, 2, 10);
            var client2 = RestClientFactory.Create(syncPolicy2);
            IRestRequest request2 = new RestRequest(Method.GET);
            //client2.BaseUrl = new Uri("https://httpstat.us/500");
            //if runtime have errors ,it will retry.
            var response4 = client2.Execute(request2);
            Console.ReadKey();

            IRestRequest request = new RestRequest(Method.GET);
            client.BaseUrl = new Uri("https://httpstat.us/500");
            //client2.BaseUrl = new Uri("https://httpstat.us/500");//
            //client3.BaseUrl = new Uri("https://httpbin.org/anything");

            var response = await client.ExecuteTaskAsync(request);

            //var response2 = await client2.ExecuteTaskAsync(request);
            // var response3 = await client3.ExecuteTaskAsync<QueryResult>(request);
            Console.ReadKey();

            var syncPolicy = BuildTimeoutAndRetryPolicy2(3, 2, 10);
            var client1 = RestClientFactory<IRestResponse>.Create(syncPolicy);
            IRestRequest request1 = new RestRequest(Method.GET);
            client1.BaseUrl = new Uri("https://httpstat.us/500");
            var response5 = client1.Execute(request1);
            Console.ReadKey();



        }

        public static ISyncPolicy<IRestResponse> BuildTimeoutAndRetryPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .OrResult<IRestResponse>(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(//ignore retrySleep, set to 5 seconds
                    5000), onRetry: (exception, calculatedWaitDuration) =>
                {
                    re++;
                    logger.Information(
                        $"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.Timeout<IRestResponse>(timeoutSeconds);

            var policyWrap = Policy.Wrap(timeout, retry);

            return Policy.Wrap(retry, timeout);
        }

        public static ISyncPolicy BuildTimeoutAndRetryPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(//ignore retrySleep, set to 5 seconds
                    5000), onRetry: (exception, calculatedWaitDuration) =>
                {
                    re++;
                    logger.Information(
                        $"Policy logging onRetry {re.ToString()} times , response content is {exception.Message}and error is :");
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.Timeout(timeoutSeconds);

            var policyWrap = Policy.Wrap(timeout, retry);

            return Policy.Wrap(retry, timeout);
        }

        public static IAsyncPolicy<IRestResponse> BuildTimeoutAndRetryAsyncPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>().Or<AggregateException>()
                .OrResult<IRestResponse>(r => (int)r.StatusCode == 500)
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(5000 * retryNumber * retrySleep),
                    onRetry: (exception, calculatedWaitDuration) =>
                {
                    re++;
                    logger.Information($"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.TimeoutAsync<IRestResponse>(timeoutSeconds);

            var policyWrap = Policy.WrapAsync(timeout, retry);

            return Policy.WrapAsync(retry, timeout);
        }

        public static IAsyncPolicy<IRestResponse<QueryResult>> BuildTimeoutAndRetryAsyncPolicy3(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .OrResult<IRestResponse<QueryResult>>(r => (int)r.StatusCode == 500)
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(5000 * retryNumber * retrySleep),
                    onRetry: (exception, calculatedWaitDuration) =>
                    {
                        re++;
                        logger.Information($"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                        Console.WriteLine("I am Console.WriteLine");
                    });

            var timeout = Policy.TimeoutAsync<IRestResponse<QueryResult>>(timeoutSeconds);

            var policyWrap = Policy.WrapAsync(timeout, retry);

            return Policy.WrapAsync(retry, timeout);
        }

        public static IAsyncPolicy BuildTimeoutAndRetryAsyncPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(5000),
                    onRetry: (exception, calculatedWaitDuration) =>
                    {
                        re++;
                        logger.Information($"Policy logging onRetry {re.ToString()} times , response content is {exception.Message}and error is :");
                        Console.WriteLine("I am Console.WriteLine");
                    });

            var timeout = Policy.TimeoutAsync(timeoutSeconds);

            var policyWrap = Policy.WrapAsync(timeout, retry);

            return Policy.WrapAsync(retry, timeout);
        }

    }
}