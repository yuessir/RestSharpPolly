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
        public static ISyncPolicy<IRestResponse> Build(int retryNumber, int retrySleep, int timeoutSeconds)
        {
            var retry = Policy
                .Handle<Exception>()
                .OrResult<IRestResponse>(r => r.StatusCode != HttpStatusCode.OK)
                .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(
                    5000 * retryNumber * retrySleep));

            var timeout = Policy.Timeout<IRestResponse>(timeoutSeconds);
            var policyWrap = Policy.Wrap(timeout, retry);
            return Policy.Wrap(retry, timeout);
        }
    }
}