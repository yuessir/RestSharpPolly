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
        protected IRestClient _innerService { get; }
        protected RestClientBase(IRestClient innerService)
        {
            _innerService = innerService;
        }
        protected delegate IRestResponse RestClientCarrier(IRestClient client);
        protected delegate IRestResponse<T> RestClientCarrier<T>(IRestClient client);
        protected delegate Task<IRestResponse<T>> RestClientTaskCarrier<T>(IRestClient client);
        protected delegate Task<IRestResponse> RestClientTaskCarrier(IRestClient client);

        protected delegate Task<IRestResponse> AsyncInterceptor(IRestRequest req, Func<Task<IRestResponse>> exec);
        protected delegate Task<IRestResponse<T>> AsyncInterceptorT<T>(IRestRequest req, Func<Task<IRestResponse<T>>> exec);
        protected delegate IRestResponse<T> SyncInterceptorT<T>(IRestRequest req, Func<IRestResponse<T>> exec);
        protected delegate IRestResponse SyncInterceptor(IRestRequest req, Func<IRestResponse> exec);

      
        protected IRestResponse ExecutePolly(Expression<RestClientCarrier> methodLambda, IRestRequest request, ISyncPolicy policy)

        {
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(policy?.ExecuteAndCapture(f)));
        }

        protected IRestResponse<T> ExecutePolly<T>(Expression<RestClientCarrier<T>> methodLambda, IRestRequest request, ISyncPolicy policy)
        {
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(policy?.ExecuteAndCapture(f)));
        }
        private IRestResponse<T> ExecuteImpl<T>(Expression<RestClientCarrier<T>> methodLambda, IRestRequest request, SyncInterceptor interceptor)
        {
            var response = interceptor == null
                ? methodLambda.Compile().Invoke(_innerService)
                : interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerService)) as IRestResponse<T>;

            return response;
        }
        protected IRestResponse ExecutePolly<TResult>(Expression<RestClientCarrier> methodLambda, IRestRequest request, ISyncPolicy<TResult> policy)
            where TResult : IRestResponse
        {
            var innerpolicy = policy as ISyncPolicy<IRestResponse>;
            return ExecuteImpl(methodLambda, request, (r, f) => request.GetPolicyResult(innerpolicy?.ExecuteAndCapture(f)));
        }

        private IRestResponse ExecuteImpl(Expression<RestClientCarrier> methodLambda, IRestRequest request, SyncInterceptor interceptor)
        {
            var response = interceptor == null
                ? methodLambda.Compile().Invoke(_innerService)
                : interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerService));

            return response;
        }

        protected IRestResponse<T> ExecutePollyT<T, T2>(Expression<RestClientCarrier> methodLambda, IRestRequest request, ISyncPolicy<T2> policy)
            where T2 : IRestResponse
        {
            var innerpolicy = policy as ISyncPolicy<IRestResponse<T>>;
            return ExecuteImplT<T>(methodLambda, request,
                (r, f) => request.GetPolicyResultT<T>(innerpolicy?.ExecuteAndCapture(f)));
        }

        private IRestResponse<T> ExecuteImplT<T>(Expression<RestClientCarrier> methodLambda, IRestRequest request, SyncInterceptorT<T> interceptor)
        {
            var response = interceptor == null
                ? (IRestResponse<T>)methodLambda.Compile().Invoke(_innerService)
                : interceptor.Invoke(request, () => (IRestResponse<T>)methodLambda.Compile().Invoke(_innerService));

            return response;
        }


        //async
        protected async Task<IRestResponse> ExecutePollyAsync(Expression<RestClientTaskCarrier> methodLambda, IRestRequest request, IAsyncPolicy policy)
        {
            return await ExecuteImplAsync(methodLambda, request, (r, f) => request.GetPolicyTaskResult(policy.ExecuteAndCaptureAsync(f)));
        }
        //async <Result>
        protected async Task<IRestResponse> ExecutePollyAsync<T>(Expression<RestClientTaskCarrier> methodLambda, IRestRequest request, IAsyncPolicy<T> policy)
        {
            var innerpolicy = policy as IAsyncPolicy<IRestResponse>;
            return await ExecuteImplAsync(methodLambda, request,
                (r, f) => request.GetPolicyTaskResult(innerpolicy?.ExecuteAndCaptureAsync(f)));
        }

        protected async Task<IRestResponse<T>> ExecutePollyTAsync<T>(Expression<RestClientTaskCarrier<T>> methodLambda, IRestRequest request, IAsyncPolicy policy)
        {
            return await ExecuteImplTAsync<T>(methodLambda, request,
                (r, f) => request.GetPolicyTaskResultT<T>(policy?.ExecuteAndCaptureAsync((f))));
        }
        private async Task<IRestResponse> ExecuteImplAsync(Expression<RestClientTaskCarrier> methodLambda, IRestRequest request, AsyncInterceptor interceptor)
        {
           
            var response = interceptor == null
                ? await methodLambda.Compile().Invoke(_innerService)
                : await interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerService));
 
            return response;
        }

        //async<T, T2>
        protected async Task<IRestResponse<T>> ExecutePollyTAsync<T, T2>(Expression<RestClientTaskCarrier<T>> methodLambda, IRestRequest request, IAsyncPolicy<T2> policy)
            where T2 : IRestResponse
        {
            var innerpolicy = policy as IAsyncPolicy<IRestResponse<T>>;
            return await ExecuteImplTAsync<T>(methodLambda, request,
                (r, f) => request.GetPolicyTaskResultT<T>(innerpolicy?.ExecuteAndCaptureAsync((f))));
        }
        private async Task<IRestResponse<T>> ExecuteImplTAsync<T>(Expression<RestClientTaskCarrier<T>> methodLambda, IRestRequest request, AsyncInterceptorT<T> interceptor)
        {
            var response = interceptor == null
                ? await methodLambda.Compile().Invoke(_innerService)
                : await interceptor.Invoke(request, () => methodLambda.Compile().Invoke(_innerService));

            return response;
        }
    }
}
