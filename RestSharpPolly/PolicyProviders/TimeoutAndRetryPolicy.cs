using System;
using System.Net;
using Polly;
using RestSharp;

namespace RestSharpPolly.PolicyProviders
{
    /// <summary>
    /// A timeout and retry policy.
    /// </summary>
    public class TimeoutAndRetryPolicy
    {
        public static ISyncPolicy<RestResponse> Build(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var retry = Policy
                .Handle<Exception>()
                .OrResult<RestResponse>(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(
                    5000 * retryNumber * retrySleep));

            var timeout = Policy.Timeout<RestResponse>(timeoutSeconds);
            return Policy.Wrap(retry, timeout);
        }
    }
}