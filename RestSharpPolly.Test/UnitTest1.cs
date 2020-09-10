using System;
using System.Diagnostics;
using System.Net;
using Polly;
using RestSharp;
using Xunit;

namespace RestSharpPolly.Test
{
    /// <summary>   A unit test 1, 单元测试看不出效果QQ </summary>
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var policy = BuildTimeoutAndRetryPolicy(1, 1, 60);
            var client = RestClientFactory<IRestResponse>.Create(policy);

            client.BaseUrl = new Uri("https://httpstat.us/500");
            IRestRequest request = new RestRequest(Method.GET);
            //if runtime have errors ,it will retry.
            var response = client.Execute(request);
            Assert.Equal(500, (int)response.StatusCode);
        }

        private ISyncPolicy<IRestResponse> BuildTimeoutAndRetryPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            int ret = 0;
            var retry = Policy
                .HandleResult<IRestResponse>(r => r.StatusCode == HttpStatusCode.BadGateway)
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(
                    5000), onRetry: (exception, calculatedWaitDuration) =>
                {
                    ret++;
                    Debug.WriteLine(ret);
                });

            var timeout = Policy.Timeout<IRestResponse>(timeoutSeconds);

            var policyWrap = Policy.Wrap(timeout, retry);

            return Policy.Wrap(retry, timeout);
        }

        private ISyncPolicy BuildTimeoutAndRetryPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(
                    5000), onRetry: (exception, calculatedWaitDuration) =>
                {
                    Console.WriteLine("I am Console.WriteLine");
                });

            var timeout = Policy.Timeout(timeoutSeconds);

            var policyWrap = Policy.Wrap(timeout, retry);

            return Policy.Wrap(retry, timeout);
        }
    }
}