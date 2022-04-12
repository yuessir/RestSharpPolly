using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using RestSharpPolly;
using RestSharpPolly.PolicyProviders;
using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using RestSharp;
using Serilog;

namespace ConsoleApp2
{

    /// <summary>
    /// see response content from https://httpstat.us/500 
    /// </summary>
    public class QueryResultModel
    {

        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }

    internal static class Program
    {
        private static int re = 0;

        private static async Task Main(string[] args)
        {
            // To uncomment to try
            //async demo for v107

            var asyncPolicy2 = BuildTimeoutAndRetryAsyncPolicy2(3, 2, 10);
            var client1 = new RestClientFactory<RestResponse>().Create(asyncPolicy2);
            var request1 = new RestRequest();
            
            client1.RestClientOptions.BaseUrl = new Uri("https://httpstat.us/500");
            var host = client1.Build(client1.RestClientOptions);
            var response5 = await host.ExecuteAsync(request1);
            Console.ReadKey();


            // demo for  under version of 107 
            // To uncomment to try
            ////async
            //var asyncPolicy2 = BuildTimeoutAndRetryAsyncPolicy2(3, 2, 10);
            //var client1 = RestClientFactory<IRestResponse>.Create(asyncPolicy2);
            //var request1 = new RestRequest(Method.GET);
            //client1.BaseUrl = new Uri("https://httpstat.us/500");
            //var response5 = await client1.ExecuteAsync(request1);
            //Console.ReadKey();

            ////////async (no result)
            //////////https://github.com/App-vNext/Polly/wiki/Non-generic-and-generic-policies
            //var noResultAyncPolicy = BuildTimeoutAndRetryAsyncPolicy(3, 2, 10); 
            //var client2Async = RestClientFactory.Create(noResultAyncPolicy);
            //client2Async.BaseUrl = new Uri("https://httpstat.us/200?sleep=15000");
            //var request2 = new RestRequest(Method.GET);
            ////if runtime have errors ,it will retry.
            //var response2 = await client2Async.ExecuteAsync(request2);
            //Console.ReadKey();

            ////// To uncomment to try
            //////async
            //var noResultaAyncPolicyT = BuildTimeoutAndRetryAsyncPolicy3(3, 2, 10);
            //var client2Async = RestClientFactory<RestResponse<QueryResultModel>>.Create(noResultaAyncPolicyT);
            //client2Async.BaseUrl = new Uri("https://httpstat.us/500");
            //var request3 = new RestRequest(Method.GET);
            ////if runtime have errors ,it will retry.
            //var response6 = await client2Async.ExecuteAsync<QueryResultModel>(request3);
            //Console.ReadKey();

            ////// To uncomment to try
            //////sync (no result) 
            ////https://github.com/App-vNext/Polly/wiki/Non-generic-and-generic-policies
            //var syncPolicy2 = BuildTimeoutAndRetryPolicy(3, 2, 5);
            //var syncClient2 = RestClientFactory.Create(syncPolicy2);
            //var request2 = new RestRequest(Method.GET);
            //syncClient2.BaseUrl = new Uri("https://httpstat.us/200?sleep=15000");
            ////if runtime have errors ,it will retry.
            //var response4 = syncClient2.Execute(request2);
            //Console.ReadKey();

            //////sync
            //////To uncomment to try
            //var syncPolicy3 = BuildTimeoutAndRetryPolicy2(3, 2, 10);
            //var syncclient3 = RestClientFactory<RestResponse>.Create(syncPolicy3);
            //var request4 = new RestRequest(Method.GET);
            //syncclient3.BaseUrl = new Uri("https://httpstat.us/500");
            ////if runtime have errors ,it will retry.
            //var res = syncclient3.Execute(request4);
            //Console.ReadKey();

        }

        public static ISyncPolicy<RestResponse> BuildTimeoutAndRetryPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .OrResult<RestResponse>(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(//ignore retrySleep, set to 5 seconds
                    5000), onRetry: (exception, calculatedWaitDuration) =>
                {
                    re++;
                    logger.Information(
                        $"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.Timeout<RestResponse>(timeoutSeconds);
            return Policy.Wrap(retry, timeout);
        }

        public static ISyncPolicy BuildTimeoutAndRetryPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var timeoutPolicy = Policy
                .Timeout(
                    TimeSpan.FromSeconds(timeoutSeconds),
                    TimeoutStrategy.Optimistic,
                    (context, timeout, _, exception) =>
                    {
                        Console.WriteLine($"{"Timeout",-10}{timeout,-10:ss\\.fff}: {exception.GetType().Name}");

                    });

            var onRetryInner = new Action<Exception, int>((e, i) =>
            {

                Console.WriteLine($"Retry #{i} due to exception '{(e.InnerException ?? e).Message}'");

            });

            var retryPolicy = Policy
                .Handle<Exception>()
                .Retry(retryNumber, onRetryInner);
            return Policy.Wrap(timeoutPolicy, retryPolicy);

        }

        public static IAsyncPolicy<RestResponse> BuildTimeoutAndRetryAsyncPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>().Or<AggregateException>()
                .OrResult<RestResponse>(r => (int)r.StatusCode == 500)
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(5000 * retryNumber * retrySleep),
                    onRetry: (exception, calculatedWaitDuration) =>
                {
                    re++;
                    logger.Information($"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.TimeoutAsync<RestResponse>(timeoutSeconds);
            return Policy.WrapAsync(retry, timeout);
        }

        public static IAsyncPolicy<RestResponse<QueryResultModel>> BuildTimeoutAndRetryAsyncPolicy3(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();
            var retry = Policy
                .Handle<Exception>()
                .OrResult<RestResponse<QueryResultModel>>(r => (int)r.StatusCode == 500)
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(5000 * retryNumber * retrySleep),
                    onRetry: (exception, calculatedWaitDuration) =>
                    {
                        re++;
                        logger.Information($"Policy logging onRetry {re.ToString()} times , response content is {exception.Result.Content}and error is :");
                        Console.WriteLine("I am Console.WriteLine");
                    });

            var timeout = Policy.TimeoutAsync<RestResponse<QueryResultModel>>(timeoutSeconds);

            return Policy.WrapAsync(retry, timeout);
        }

        public static IAsyncPolicy BuildTimeoutAndRetryAsyncPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext()
                .WriteTo.ColoredConsole().CreateLogger();

            var timeoutPolicy = Policy
                .TimeoutAsync(
                    TimeSpan.FromSeconds(timeoutSeconds),
                    TimeoutStrategy.Pessimistic,
                    (context, timeout, _, exception) =>
                    {
                        Console.WriteLine($"{"Timeout",-10}{timeout,-10:ss\\.fff}: {exception.GetType().Name}");
                        return Task.CompletedTask;
                    });

            var onRetryInner = new Func<Exception, int, Task>((e, i) =>
            {
                Console.WriteLine($"Retry #{i} due to exception '{(e.InnerException ?? e).Message}'");
                return Task.CompletedTask;
            });

            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(retryNumber, onRetryInner);
            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }


    }


}