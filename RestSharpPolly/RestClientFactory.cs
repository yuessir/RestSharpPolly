using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace RestSharpPolly
{
    public interface IRestClient
    {
         RestClientOptions RestClientOptions { get; set; }
         RestClientFactory Build(RestClientOptions options);
    }

    /// <summary>
    /// A REST client factory.
    /// </summary>
    public class RestClientFactory : RestClientBase, IRestClient
    {
         private ISyncPolicy _pollyRetPolicy { get; set; }
        public ISyncPolicy PollyRetPolicy
        {
            get => _pollyRetPolicy;
            set => _pollyRetPolicy = value;
        }
        private IAsyncPolicy _pollyRetAsyncPolicy { get; set; }
        public IAsyncPolicy PollyRetAsyncPolicy
        {
            get => _pollyRetAsyncPolicy;
            set => _pollyRetAsyncPolicy = value;
        }
        private static RestClientFactory _instanceRestClient ;
        public static RestClientFactory InstanceRestClient
        {
            get
            {
                if (_instanceRestClient == null)
                {
                    _instanceRestClient = new RestClientFactory();

                    return _instanceRestClient;
                }


                return _instanceRestClient;
            }
            set => _instanceRestClient = value;
        }

        private RestClient _innerService;

        public IRestClient Create()
        {
            base._innerClient = _innerService;
            RestClientOptions = new RestClientOptions();
            return this;
        }

        public RestClientOptions RestClientOptions { get; set; }

        public RestClientFactory Build(RestClientOptions options)
        {
            if (options == null)
            {
                options = new RestClientOptions(); ;
            }
            _innerService = new RestClient(options);
            base._innerClient = _innerService;
          
            return this;
        }
        public IRestClient Create(ISyncPolicy syncPolicy)
        {
            RestClientOptions = new RestClientOptions();
            SetPolicy(syncPolicy);
             return this;
        }
      
        public IRestClient Create(IAsyncPolicy asyncPolicy)
        {
            RestClientOptions = new RestClientOptions();
            SetAsyncPolicy(asyncPolicy);
             return this;
        }

        private void SetPolicy(ISyncPolicy syncPolicy)
        {
            _pollyRetPolicy = syncPolicy;

        }

        private void SetAsyncPolicy(IAsyncPolicy asyncPolicy)
        {
            _pollyRetAsyncPolicy = asyncPolicy;
        }

        public RestClient UseSerializer(Func<IRestSerializer> serializerFactory)
        {
            return _innerService.UseSerializer(serializerFactory);
        }

        public RestClient UseSerializer<T>() where T : class, IRestSerializer, new()
        {
            return _innerService.UseSerializer<T>();
        }

        public RestResponse<T> Deserialize<T>(RestResponse response)
        {
            return _innerService.Deserialize<T>(response);
        }

        public RestClient UseUrlEncoder(Func<string, string> encoder)
        {
            return _innerService.UseUrlEncoder(encoder);
        }

        public RestClient UseQueryEncoder(Func<string, Encoding, string> queryEncoder)
        {
            return _innerService.UseQueryEncoder(queryEncoder);
        }
        /// <summary>
        /// According to the official documents, all the synchronous methods are gone. If you absolutely must call without using async and await, use GetAwaiter().GetResult() blocking call.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("Using Async Method, next version will deprecate the method.")]
        public RestResponse Execute(RestRequest request)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.ExecuteAsync(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }
        /// <summary>
        /// According to the official documents, all the synchronous methods are gone. If you absolutely must call without using async and await, use GetAwaiter().GetResult() blocking call.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("Using Async Method, next version will deprecate the method.")]
        public RestResponse Execute(RestRequest request, Method httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly(x => x.ExecuteAsync(request, httpMethod, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }
        /// <summary>
        /// According to the official documents, all the synchronous methods are gone. If you absolutely must call without using async and await, use GetAwaiter().GetResult() blocking call.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("Using Async Method, next version will deprecate the method.")]
        public RestResponse<T> Execute<T>(RestRequest request)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.ExecuteAsync<T>(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);


        }
        /// <summary>
        /// According to the official documents, all the synchronous methods are gone. If you absolutely must call without using async and await, use GetAwaiter().GetResult() blocking call.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Obsolete("Using Async Method, next version will deprecate the method.")]
        public RestResponse<T> Execute<T>(RestRequest request, Method httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return ExecutePolly<T>(x => x.ExecuteAsync<T>(request, httpMethod, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }

        public byte[] DownloadData(RestRequest request)
        {
            return _innerService.DownloadDataAsync(request).GetAwaiter().GetResult();
        }

        public Uri BuildUri(RestRequest request)
        {
            return _innerService.BuildUri(request);
        }

        [Obsolete("deprecated the method.",true)]
        public string BuildUriWithoutQueryParameters(RestRequest request)
        {
            throw new Exception("deprecated the method.");
        }
        [Obsolete("deprecated the method.", true)]
        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            throw new Exception("deprecated the method.");
        }
        [Obsolete("deprecated the method.", true)]
        public void AddHandler(string contentType, Func<IDeserializer> deserializerFactory)
        {
            throw new Exception("deprecated the method.");
        }
        [Obsolete("deprecated the method.", true)]
        public void RemoveHandler(string contentType)
        {
            throw new Exception("deprecated the method.");
        }
        [Obsolete("deprecated the method.", true)]
        public void ClearHandlers()
        {
            throw new Exception("deprecated the method.");
        }
        [Obsolete("Using Async Method, next version will deprecate the method.")]
        public RestResponse ExecuteAsGet(RestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            if (!string.IsNullOrEmpty(httpMethod))
            {
                return ExecutePolly(x => x.ExecuteAsync(request, (Method)Enum.Parse(typeof(Method), httpMethod), default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

            }
            return ExecutePolly(x => x.ExecuteGetAsync(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }

        public RestResponse ExecuteAsPost(RestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            if (!string.IsNullOrEmpty(httpMethod))
            {
                return ExecutePolly(x => x.ExecuteAsync(request, (Method)Enum.Parse(typeof(Method), httpMethod), default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

            }
            return ExecutePolly(x => x.ExecutePostAsync(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }

        public RestResponse<T> ExecuteAsGet<T>(RestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            if (!string.IsNullOrEmpty(httpMethod))
            {
                return ExecutePolly(x => x.ExecuteAsync<T>(request, (Method)Enum.Parse(typeof(Method), httpMethod), default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

            }
            return ExecutePolly(x => x.ExecuteGetAsync<T>(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

        }

        public RestResponse<T> ExecuteAsPost<T>(RestRequest request, string httpMethod)
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            if (!string.IsNullOrEmpty(httpMethod))
            {
                return ExecutePolly(x => x.ExecuteAsync<T>(request, (Method)Enum.Parse(typeof(Method), httpMethod), default).GetAwaiter().GetResult(), request, _pollyRetPolicy);

            }
            return ExecutePolly(x => x.ExecutePostAsync<T>(request, default).GetAwaiter().GetResult(), request, _pollyRetPolicy);


        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteAsync<T>(request, httpMethod, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request, Method httpMethod, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteAsync(request, httpMethod, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse<T>> ExecuteGetAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecuteGetAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse<T>> ExecutePostAsync<T>(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyTAsync<T>(x =>
                x.ExecutePostAsync<T>(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse> ExecuteGetAsync(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecuteGetAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }

        public async Task<RestResponse> ExecutePostAsync(RestRequest request, CancellationToken cancellationToken = new CancellationToken())
        {

            if (null == request)
                throw new AggregateException(nameof(request) + " is  null");
            return await ExecutePollyAsync(x =>
                x.ExecutePostAsync(request, cancellationToken), request, _pollyRetAsyncPolicy);

        }
        [Obsolete("Not supported")]
        public CookieContainer CookieContainer { get; set; }
        [Obsolete("Not supported")]
        public bool AutomaticDecompression { get; set; }
        [Obsolete("Not supported")]
        public int? MaxRedirects { get; set; }
        [Obsolete("Not supported")]
        public string UserAgent { get; set; }
        [Obsolete("Not supported")]
        public int Timeout { get; set; }

        [Obsolete("Not supported", true)]
        public int ReadWriteTimeout { get; set; }
        [Obsolete("Not supported", true)]
        public bool UseSynchronizationContext { get; set; }
        [Obsolete("Not supported")]
        public IAuthenticator Authenticator
        {
            get => _innerService.Authenticator;
            set => _innerService.Authenticator = value;
        }
        [Obsolete("Not supported")]
        public Uri BaseUrl { get; set; }

        [Obsolete("Not supported")]
        public Encoding Encoding { get; set; }
        [Obsolete("Not supported")]
        public bool ThrowOnDeserializationError { get; set; }
        [Obsolete("Not supported")]
        public bool FailOnDeserializationError { get; set; }
        [Obsolete("Not supported")]
        public bool ThrowOnAnyError { get; set; }
        [Obsolete("Not supported", true)]
        public string ConnectionGroupName { get; set; }
        [Obsolete("Not supported")]
        public bool PreAuthenticate { get; set; }
        [Obsolete("Not supported", true)]
        public bool UnsafeAuthenticatedConnectionSharing { get; set; }
        [Obsolete("Not supported")]
        public string BaseHost { get; set; }
        [Obsolete("Not supported")]
        public bool AllowMultipleDefaultParametersWithSameName { get; set; }
        [Obsolete("Not supported")]
        public X509CertificateCollection ClientCertificates { get; set; }
        [Obsolete("Not supported")]
        public IWebProxy Proxy { get; set; }
        [Obsolete("Not supported")]
        public RequestCachePolicy CachePolicy { get; set; }
        [Obsolete("Not supported", true)]
        public bool Pipelined { get; set; }
        [Obsolete("Not supported")]
        public bool FollowRedirects { get; set; }
        [Obsolete("Not supported")]
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }


    }
}