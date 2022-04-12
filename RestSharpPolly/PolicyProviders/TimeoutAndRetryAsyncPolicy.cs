using System;
using System.Net;
using Polly;
using RestSharp;

namespace RestSharpPolly.PolicyProviders
{
    /// <summary>
    /// A timeout and retry policy.
    /// </summary>
    public class TimeoutAndRetryAsyncPolicy
    {
        public static IAsyncPolicy<RestResponse> Build(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var retry = Policy
                .Handle<Exception>()
                .OrResult<RestResponse>(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetryAsync(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(
                    5000 * retryNumber * retrySleep));

            var timeout = Policy.TimeoutAsync<RestResponse>(timeoutSeconds);
            return Policy.WrapAsync(timeout, retry);
        }
    }
}