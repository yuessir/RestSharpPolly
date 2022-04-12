using System;
using System.Diagnostics;
using System.Net;
using Polly;
using RestSharp;
using Xunit;

namespace RestSharpPolly.Test
{
    /// <summary>   A unit test 1 for under the version of 107 , See console app </summary>
    //public class UnitTest1
    //{
    //    public static int count = 0;
    //    [Fact]
    //    public void Test1()
    //    {
    //        var policy = BuildTimeoutAndRetryPolicy(3, 1, 60);
    //        var client = RestClientFactory.Create(policy);

    //        client.BaseUrl = new Uri("https://httpstat.us/500");
    //        IRestRequest request = new RestRequest(Method.GET);
    //        //if runtime have errors ,it will retry.

    //        Assert.Throws<AggregateException>(() => client.Execute(null));
    //        Assert.Equal(3, count);
    //    }

    //    public static ISyncPolicy<IRestResponse> BuildTimeoutAndRetryPolicy2(int retryNumber, int retrySleep, int timeoutSeconds)
    //    {

    //        var retry = Policy
    //            .Handle<Exception>()
    //            .OrResult<IRestResponse>(r => r.StatusCode != HttpStatusCode.OK)
    //            .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(//ignore retrySleep, set to 5 seconds
    //                5000), onRetry: (exception, calculatedWaitDuration) =>
    //            {
    //                count++;
    //                Console.WriteLine("I am Console.WriteLine");
    //            });

    //        var timeout = Policy.Timeout<IRestResponse>(timeoutSeconds);
    //        return Policy.Wrap(retry, timeout);
    //    }

    //    public static ISyncPolicy BuildTimeoutAndRetryPolicy(int retryNumber, int retrySleep, int timeoutSeconds)
    //    {
    //        count = 0;
    //        var retry = Policy
    //            .Handle<Exception>().Or<NullReferenceException>()

    //            .WaitAndRetry(retryNumber, retryAttempt => TimeSpan.FromMilliseconds(//ignore retrySleep, set to 5 seconds
    //                5000), onRetry: (exception, calculatedWaitDuration) =>
    //            {
    //                count++;
    //                Console.WriteLine("I am Console.WriteLine");
    //            });

    //        var timeout = Policy.Timeout(timeoutSeconds);
    //        return Policy.Wrap(retry, timeout);
    //    }
    //}
}