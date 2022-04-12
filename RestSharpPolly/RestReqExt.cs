using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RestSharp;

namespace RestSharpPolly
{
    public static class RestReqExt
    {
        public static RestResponse GetPolicyResult(this RestRequest request, PolicyResult<RestResponse> pr)
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
        public static RestResponse<T> GetPolicyResultT<T>(this RestRequest request, PolicyResult<RestResponse<T>> pr)
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
     
        public static Task<RestResponse> GetPolicyTaskResult(this RestRequest request, Task<PolicyResult<RestResponse>> pr)
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
        public static Task<RestResponse<T>> GetPolicyTaskResultT<T>(this RestRequest request, Task<PolicyResult<RestResponse<T>>> pr)
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
