using System;
using System.Diagnostics;
using System.Net;
using Polly;
using RestSharp;
using Xunit;

namespace RestSharpPolly.Test
{
    public class UnitTest1
    {
    
        [Fact]
        public void Test_RestClientOptions_Not_NULL()
        {
            var client1 = new RestClientFactory<RestResponse>().Create();
            Assert.NotNull(client1.RestClientOptions);
    

        }
        [Fact]
        public void Test_RestClient_Not_NULL()
        {
            var client1 = RestClientFactory<RestResponse>.InstanceRestClient;
            Assert.NotNull(client1);

        }
    }

}