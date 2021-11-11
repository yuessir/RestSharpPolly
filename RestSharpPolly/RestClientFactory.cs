using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serialization;

namespace RestSharpPolly
{
    /// <summary>
    /// A REST client factory.
    /// </summary>
    public class RestClientFactory : RestClientBase, IRestClient
    {
        private static readonly Lazy<RestClientFactory> LazyRestFac = new Lazy<RestClientFactory>(() => new RestClientFactory());
        private static readonly Lazy<IRestClient> LazyRestClient = new Lazy<IRestClient>(() => new RestClient());
        private ISyncPolicy _pollyRetPolicy;
        private IAsyncPolicy _pollyRetAsyncPolicy;
        private static RestClientFactory InstanceRestClient => LazyRestFac.Value;
        private static IRestClient _innerService => LazyRestClient.Value;

        private RestClientFactory() : base(_innerService)
        {
        }

        public static IRestClient Create()
        {
            return _innerService;
        }

        public static RestClientFactory Create(ISyncPolicy syncPolicy)
        {
            return InstanceRestClient.SetPolicy(syncPolicy);
        }

        public static RestClientFactory Create(IAsyncPolicy asyncPolicy)
        {
            return InstanceRestClient.SetAsyncPolicy(asyncPolicy);
        }

        private RestClientFactory SetPolicy(ISyncPolicy syncPolicy)
        {
            _pollyRetPolicy = syncPolicy;
            return InstanceRestClient;
        }

        private RestClientFactory SetAsyncPolicy(IAsyncPolicy asyncPolicy)
        {
            _pollyRetAsyncPolicy = asyncPolicy;
            return InstanceRestClient;
        }

        public IRestClient UseSerializer(Func<IRestSerializer> serializerFactory)
        {
            return _innerService.UseSerializer(serializerFactory);
        }

        public IRestClient UseSerializer<T>() where T : IRestSerializer, new()
        {
            return _innerService.UseSerializer<T>();
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            return _innerService.Deserialize<T>(response);
        }

        public IRestClient UseUrlEncoder(Func<string, string> encoder)
        {
            return _innerService.UseUrlEncoder(encoder);
        }

        public IRestClient UseQueryEncoder(Func<string, Encoding, string> queryEncoder)
        {
            return _innerService.UseQueryEncoder(queryEncoder);
        }

        public IRestResponse Execute(IRestRequest request)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.Execute(request), request, _pollyRetPolicy);

        }

        public IRestResponse Execute(IRestRequest request, Method httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.Execute(request, httpMethod), request, _pollyRetPolicy);
          
        }

        public IRestResponse<T> Execute<T>(IRestRequest request)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.Execute<T>(request), request, _pollyRetPolicy);
            

        }

        public IRestResponse<T> Execute<T>(IRestRequest request, Method httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.Execute<T>(request, httpMethod), request, _pollyRetPolicy);
            
        }

        public byte[] DownloadData(IRestRequest request)
        {
            return _innerService.DownloadData(request);
        }

        public Uri BuildUri(IRestRequest request)
        {
            return _innerService.BuildUri(request);
        }

        public string BuildUriWithoutQueryParameters(IRestRequest request)
        {
            return _innerService.BuildUriWithoutQueryParameters(request);
        }

        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            _innerService.ConfigureWebRequest(configurator);
        }

        public void AddHandler(string contentType, Func<IDeserializer> deserializerFactory)
        {
            _innerService.AddHandler(contentType, deserializerFactory);
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
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.ExecuteAsGet(request, httpMethod), request, _pollyRetPolicy);
           
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.ExecuteAsPost(request, httpMethod), request, _pollyRetPolicy);
           
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.ExecuteAsGet<T>(request, httpMethod), request, _pollyRetPolicy);
            
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.ExecuteAsPost<T>(request, httpMethod), request, _pollyRetPolicy);
           
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);
          
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteAsync<T>(request, httpMethod, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse> ExecuteAsync(IRestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteAsync(request, httpMethod, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse> ExecuteAsync(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse<T>> ExecuteGetAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteGetAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse<T>> ExecutePostAsync<T>(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecutePostAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse> ExecuteGetAsync(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteGetAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);
           
        }

        public async Task<IRestResponse> ExecutePostAsync(IRestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecutePostAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);
           
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

        public bool ThrowOnDeserializationError
        {
            get => _innerService.ThrowOnDeserializationError;
            set => _innerService.ThrowOnDeserializationError = value;
        }

        public bool FailOnDeserializationError
        {
            get => _innerService.FailOnDeserializationError;
            set => _innerService.FailOnDeserializationError = value;
        }

        public bool ThrowOnAnyError
        {
            get => _innerService.ThrowOnAnyError;
            set => _innerService.ThrowOnAnyError = value;
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

        public IList<Parameter> DefaultParameters => _innerService.DefaultParameters;

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

        #region Obsolete Methods

        [Obsolete]
        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            _innerService.AddHandler(contentType, deserializer);
        }

        [Obsolete]
        public byte[] DownloadData(IRestRequest request, bool throwOnError)
        {
            return _innerService.DownloadData(request, throwOnError);
        }

        [Obsolete]
        public IRestClient UseSerializer(IRestSerializer serializer)
        {
            return _innerService.UseSerializer(serializer);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            return _innerService.ExecuteAsync(request, callback);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            return _innerService.ExecuteAsync<T>(request, callback);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _innerService.ExecuteAsync(request, callback, httpMethod);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            return _innerService.ExecuteAsync<T>(request, callback, httpMethod);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _innerService.ExecuteAsyncGet(request, callback, httpMethod);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _innerService.ExecuteAsyncPost(request, callback, httpMethod);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _innerService.ExecuteAsyncGet(request, callback, httpMethod);
        }

        [Obsolete]
        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            return _innerService.ExecuteAsyncPost(request, callback, httpMethod);
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync<T>(request));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync<T>(request, token));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, Method httpMethod)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync<T>(request, httpMethod));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteGetTaskAsync<T>(request));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteGetTaskAsync<T>(request, token));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteGetTaskAsync<T>(request));
        }

        [Obsolete]
        public async Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecutePostTaskAsync<T>(request, token));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync(request, token));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token, Method httpMethod)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync(request, token, httpMethod));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteTaskAsync(request));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteGetTaskAsync(request));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecuteGetTaskAsync(request, token));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecutePostTaskAsync(request));
        }

        [Obsolete]
        public async Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            if (null == request)
                return null;

            if (null == _pollyRetAsyncPolicy)
                return null;

            return await _pollyRetAsyncPolicy.ExecuteAsync(async () => await _innerService.ExecutePostTaskAsync(request, token));
        }

        #endregion

    }
}