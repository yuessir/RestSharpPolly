using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RestSharp;

namespace RestSharpPolly
{
    public class RestClientBase
    {
        protected RestClient _innerClient { get; set; }
        protected RestClientBase(RestClient innerClient)
        {
            _innerClient = innerClient;
        }
        protected RestClientBase( )
        {
        }
        protected delegate RestResponse RestClientCarrier(RestClient client);
        protected delegate RestResponse<T> RestClientCarrier<T>(RestClient client);
        protected delegate Task<RestResponse<T>> RestClientTaskCarrier<T>(RestClient client);
        protected delegate Task<RestResponse> RestClientTaskCarrier(RestClient client);

        protected delegate Task<RestResponse> AsyncInterceptor(RestRequest req, Func<Task<RestResponse>> exec);
        protected delegate Task<RestResponse<T>> AsyncInterceptorT<T>(RestRequest req, Func<Task<RestResponse<T>>> exec);
        protected delegate RestResponse<T> SyncInterceptorT<T>(RestRequest req, Func<RestResponse<T>> exec);
        protected delegate RestResponse SyncInterceptor(RestRequest req, Func<RestResponse> exec);

      
        protected RestResponse ExecutePolly(Expression<RestClientCarrier> methodLambda, RestRequest request, ISyncPolicy policy)

        {
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(policy?.ExecuteAndCapture(f)));
        }

        protected RestResponse<T> ExecutePolly<T>(Expression<RestClientCarrier<T>> methodLambda, RestRequest request, ISyncPolicy policy)
        {
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(policy?.ExecuteAndCapture(f)));
        }
        private RestResponse<T> ExecuteImpl<T>(Expression<RestClientCarrier<T>> methodLambda, RestRequest request, SyncInterceptor interceptor)
        {
            var response = interceptor == null
                ? methodLambda.Compile().Invoke(_innerClient)
                : interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerClient)) as RestResponse<T>;

            return response;
        }
        protected RestResponse ExecutePolly<TResult>(Expression<RestClientCarrier> methodLambda, RestRequest request, ISyncPolicy<TResult> policy)
            where TResult : RestResponse
        {
            var innerpolicy = policy as ISyncPolicy<RestResponse>;
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(innerpolicy?.ExecuteAndCapture(f)));
        }

        private RestResponse ExecuteImpl(Expression<RestClientCarrier> methodLambda, RestRequest request, SyncInterceptor interceptor)
        {
            var response = interceptor == null
                ? methodLambda.Compile().Invoke(_innerClient)
                : interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerClient));

            return response;
        }

        protected RestResponse<T> ExecutePollyT<T, T2>(Expression<RestClientCarrier> methodLambda, RestRequest request, ISyncPolicy<T2> policy)
            where T2 : RestResponse
        {
            var innerpolicy = policy as ISyncPolicy<RestResponse<T>>;
            return ExecuteImplT<T>(methodLambda, request,
                (r, f) => request.GetPolicyResultT<T>(innerpolicy?.ExecuteAndCapture(f)));
        }

        private RestResponse<T> ExecuteImplT<T>(Expression<RestClientCarrier> methodLambda, RestRequest request, SyncInterceptorT<T> interceptor)
        {
            var response = interceptor == null
                ? (RestResponse<T>)methodLambda.Compile().Invoke(_innerClient)
                : interceptor.Invoke(request, () => (RestResponse<T>)methodLambda.Compile().Invoke(_innerClient));

            return response;
        }


        //async
        protected async Task<RestResponse> ExecutePollyAsync(Expression<RestClientTaskCarrier> methodLambda, RestRequest request, IAsyncPolicy policy)
        {
            return await ExecuteImplAsync(methodLambda, request, (r, f) => request.GetPolicyTaskResult(policy.ExecuteAndCaptureAsync(f)));
        }
        //async <Result>
        protected async Task<RestResponse> ExecutePollyAsync<T>(Expression<RestClientTaskCarrier> methodLambda, RestRequest request, IAsyncPolicy<T> policy)
        {
            var innerpolicy = policy as IAsyncPolicy<RestResponse>;
            return await ExecuteImplAsync(methodLambda, request,
                (r, f) => request.GetPolicyTaskResult(innerpolicy?.ExecuteAndCaptureAsync(f)));
        }

        protected async Task<RestResponse<T>> ExecutePollyTAsync<T>(Expression<RestClientTaskCarrier<T>> methodLambda, RestRequest request, IAsyncPolicy policy)
        {
            return await ExecuteImplTAsync<T>(methodLambda, request,
                (r, f) => request.GetPolicyTaskResultT<T>(policy?.ExecuteAndCaptureAsync((f))));
        }
        private async Task<RestResponse> ExecuteImplAsync(Expression<RestClientTaskCarrier> methodLambda, RestRequest request, AsyncInterceptor interceptor)
        {
           
            var response = interceptor == null
                ? await methodLambda.Compile().Invoke(_innerClient)
                : await interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerClient));
 
            return response;
        }

        //async<T, T2>
        protected async Task<RestResponse<T>> ExecutePollyTAsync<T, T2>(Expression<RestClientTaskCarrier<T>> methodLambda, RestRequest request, IAsyncPolicy<T2> policy)
            where T2 : RestResponse
        {
            var innerpolicy = policy as IAsyncPolicy<RestResponse<T>>;
            return await ExecuteImplTAsync<T>(methodLambda, request,
                (r, f) => request.GetPolicyTaskResultT<T>(innerpolicy?.ExecuteAndCaptureAsync((f))));
        }
        private async Task<RestResponse<T>> ExecuteImplTAsync<T>(Expression<RestClientTaskCarrier<T>> methodLambda, RestRequest request, AsyncInterceptorT<T> interceptor)
        {
            var response = interceptor == null
                ? await methodLambda.Compile().Invoke(_innerClient)
                : await interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerClient));

            return response;
        }
    }
}
