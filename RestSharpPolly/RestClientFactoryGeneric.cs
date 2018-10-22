using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;

namespace RestSharpPolly
{
    /// <summary>   A REST client factory. </summary>
    public class RestClientFactory<TResult> : IRestClient where TResult : IRestResponse
    {
        private static readonly Lazy<RestClientFactory<TResult>> LazyRestFac = new Lazy<RestClientFactory<TResult>>(() => new RestClientFactory<TResult>());
        private static readonly Lazy<IRestClient> LazyRestClient = new Lazy<IRestClient>(() => new RestClient());
        private ISyncPolicy<TResult> _pollyRetPolicyGeneric;
        private IAsyncPolicy<TResult> _pollyRetAsyncPolicyGeneric;
        private static RestClientFactory<TResult> InstanceRestClient => LazyRestFac.Value;
        private static IRestClient _innerService => LazyRestClient.Value;

        private RestClientFactory()
        {
        }

        public static IRestClient Create()
        {
            return _innerService;
        }

        public static RestClientFactory<TResult> Create(ISyncPolicy<TResult> syncPolicy)
        {
            return InstanceRestClient.SetPolicy(syncPolicy);
        }

        public static RestClientFactory<TResult> Create(IAsyncPolicy<TResult> asyncPolicy)
        {
            return InstanceRestClient.SetAsyncPolicy(asyncPolicy);
        }

        public RestClientFactory<TResult> SetAsyncPolicy(IAsyncPolicy<TResult> asyncPolicyGeneric)
        {
            _pollyRetAsyncPolicyGeneric = asyncPolicyGeneric;
            return InstanceRestClient;
        }

        public RestClientFactory<TResult> SetPolicy(ISyncPolicy<TResult> syncPolicyGeneric)
        {
            _pollyRetPolicyGeneric = syncPolicyGeneric;
            return InstanceRestClient;
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            return _innerService.ExecuteAsync(request, callback);
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            return _innerService.ExecuteAsync<T>(request, callback);
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _innerService.ExecuteAsync(request, callback, httpMethod);
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _innerService.ExecuteAsync<T>(request, callback, httpMethod);
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            return _innerService.Deserialize<T>(response);
        }

        public IRestResponse Execute(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return _pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.Execute(request));
        }

        public IRestResponse Execute(IRestRequest request, Method httpMethod)
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return _pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.Execute(request, httpMethod));
        }

        public IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return (IRestResponse<T>)_pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.Execute<T>(request));
        }

        public IRestResponse<T> Execute<T>(IRestRequest request, Method httpMethod) where T : new()
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return (IRestResponse<T>)_pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.Execute<T>(request, httpMethod));
        }

        public byte[] DownloadData(IRestRequest request)
        {
            return _innerService.DownloadData(request);
        }

        public byte[] DownloadData(IRestRequest request, bool throwOnError)
        {
            return _innerService.DownloadData(request, throwOnError);
        }

        public Uri BuildUri(IRestRequest request)
        {
            return _innerService.BuildUri(request);
        }

        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            _innerService.ConfigureWebRequest(configurator);
        }

        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            _innerService.AddHandler(contentType, deserializer);
        }

        public void RemoveHandler(string contentType)
        {
            _innerService.RemoveHandler(contentType);
        }

        public void ClearHandlers()
        {
            _innerService.ClearHandlers();
        }

        public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return _pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.ExecuteAsGet(request, httpMethod));
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return _pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.ExecuteAsPost(request, httpMethod));
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod) where T : new()
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return (IRestResponse<T>)_pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.ExecuteAsGet<T>(request, httpMethod));
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod) where T : new()
        {
            if (null == request)
                return null;
            if (null == _pollyRetPolicyGeneric)
                return null;
            return (IRestResponse<T>)_pollyRetPolicyGeneric.Execute(() => (TResult)_innerService.ExecuteAsPost<T>(request, httpMethod));
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return (IRestResponse<T>)await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync<T>(request, token));
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, Method httpMethod)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return (IRestResponse<T>)await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync<T>(request, httpMethod));
        }

        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return (IRestResponse<T>)await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync<T>(request));
        }

        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return (IRestResponse<T>)await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteGetTaskAsync<T>(request));
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync(request, token));
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token, Method httpMethod)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync(request, token, httpMethod));
        }

        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteTaskAsync(request));
        }

        public async Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteGetTaskAsync(request));
        }

        public async Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecuteGetTaskAsync(request, token));
        }

        public async Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;
            if (null == _pollyRetAsyncPolicyGeneric)
                return null;
            return await _pollyRetAsyncPolicyGeneric.ExecuteAsync(async () => (TResult)await _innerService.ExecutePostTaskAsync(request));
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public CookieContainer CookieContainer
        {
            get => _innerService.CookieContainer;

            set => _innerService.CookieContainer = value;
        }

        public bool AutomaticDecompression
        {
            get => _innerService.AutomaticDecompression;

            set => _innerService.AutomaticDecompression = value;
        }

        public int? MaxRedirects
        {
            get => _innerService.MaxRedirects;

            set => _innerService.MaxRedirects = value;
        }

        public string UserAgent
        {
            get => _innerService.UserAgent;

            set => _innerService.UserAgent = value;
        }

        public int Timeout
        {
            get => _innerService.Timeout;

            set => _innerService.Timeout = value;
        }

        public int ReadWriteTimeout
        {
            get => _innerService.ReadWriteTimeout;

            set => _innerService.ReadWriteTimeout = value;
        }

        public bool UseSynchronizationContext
        {
            get => _innerService.UseSynchronizationContext;

            set => _innerService.UseSynchronizationContext = value;
        }

        public IAuthenticator Authenticator
        {
            get => _innerService.Authenticator;

            set => _innerService.Authenticator = value;
        }

        public Uri BaseUrl
        {
            get => _innerService.BaseUrl;

            set => _innerService.BaseUrl = value;
        }

        public Encoding Encoding
        {
            get => _innerService.Encoding;

            set => _innerService.Encoding = value;
        }

        public string ConnectionGroupName
        {
            get => _innerService.ConnectionGroupName;

            set => _innerService.ConnectionGroupName = value;
        }

        public bool PreAuthenticate
        {
            get => _innerService.PreAuthenticate;

            set => _innerService.PreAuthenticate = value;
        }

        public bool UnsafeAuthenticatedConnectionSharing
        {
            get => _innerService.UnsafeAuthenticatedConnectionSharing;

            set => _innerService.UnsafeAuthenticatedConnectionSharing = value;
        }

        public IList<Parameter> DefaultParameters
        {
            get => _innerService.DefaultParameters;
        }

        public string BaseHost
        {
            get => _innerService.BaseHost;

            set => _innerService.BaseHost = value;
        }

        public bool AllowMultipleDefaultParametersWithSameName
        {
            get => _innerService.AllowMultipleDefaultParametersWithSameName;

            set => _innerService.AllowMultipleDefaultParametersWithSameName = value;
        }

        public X509CertificateCollection ClientCertificates
        {
            get => _innerService.ClientCertificates;

            set => _innerService.ClientCertificates = value;
        }

        public IWebProxy Proxy
        {
            get => _innerService.Proxy;

            set => _innerService.Proxy = value;
        }

        public RequestCachePolicy CachePolicy
        {
            get => _innerService.CachePolicy;

            set => _innerService.CachePolicy = value;
        }

        public bool Pipelined
        {
            get => _innerService.Pipelined;

            set => _innerService.Pipelined = value;
        }

        public bool FollowRedirects
        {
            get => _innerService.FollowRedirects;

            set => _innerService.FollowRedirects = value;
        }

        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
        {
            get => _innerService.RemoteCertificateValidationCallback;

            set => _innerService.RemoteCertificateValidationCallback = value;
        }
    }
}