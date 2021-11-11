using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using RestSharp;

namespace RestSharpPolly
{
    public static class RestReqExt
    {
        public static IRestResponse GetPolicyResult(this IRestRequest request, PolicyResult<IRestResponse> pr)
        {
            if (pr.Outcome == OutcomeType.Successful)
                return pr.Result;

            if (pr.FinalException != null)
                throw pr.FinalException;

            if (pr.FinalHandledResult != null)
            {

                return pr.FinalHandledResult;
            }

            throw new Exception("unhandled policy fault, no result");
        }
        public static IRestResponse<T> GetPolicyResultT<T>(this IRestRequest request, PolicyResult<IRestResponse<T>> pr)
        {
            if (pr.Outcome == OutcomeType.Successful)
                return pr.Result;

            if (pr.FinalException != null)
                throw pr.FinalException;

            if (pr.FinalHandledResult != null)
            {

                return pr.FinalHandledResult;
            }

            throw new Exception("unhandled policy fault, no result");
        }
        public static Task<IRestResponse> GetPolicyTaskResult(this IRestRequest request, Task<PolicyResult<IRestResponse>> pr)
        {
            if (pr.Result.Outcome == OutcomeType.Successful)
                return Task.FromResult(pr.Result.Result);

            if (pr.Result.FinalException != null)
                throw pr.Result.FinalException;

            if (pr.Result.FinalHandledResult != null)
            {

                return Task.FromResult(pr.Result.FinalHandledResult);
            }

            throw new Exception("unhandled policy fault, no result");
        }
        public static Task<IRestResponse<T>> GetPolicyTaskResultT<T>(this IRestRequest request, Task<PolicyResult<IRestResponse<T>>> pr)
        {
            if (pr.Result.Outcome == OutcomeType.Successful)
                return  Task.FromResult(pr.Result.Result);

            if (pr.Result.FinalException != null)
                throw pr.Result.FinalException;

            if (pr.Result.FinalHandledResult != null)
            {

                return Task.FromResult(pr.Result.FinalHandledResult);
            }

            throw new Exception("unhandled policy fault, no result");
        }

    }
}
