# RestSharpPolly
RestSharp with Polly
Wrapping the RestClient with Polly framework.
Usage:

```
 var client = RestClientFactory<IRestResponse>.Create(TimeoutAndRetryPolicy.Build(3, 10, 60));
            client.BaseUrl = new Uri("");
            var request = new RestRequest(Method.GET);
            request.AddJsonBody(model);
            var response = client.Execute(request);
```
The class RestClientFactory is generic.
```
RestClientFactory<TResult> Create(ISyncPolicy<TResult> syncPolicy)
```



# Install
Import RestSharpPolly into an existing project

    Go to the project folder of the application and install the Nuget package reference

    $ dotnet add package RestSharpPolly


ID your project using the RestSharp v107, to checkout the branch **`V107`**

in the v107 case,

```  var asyncPolicy2 = BuildTimeoutAndRetryAsyncPolicy2(3, 2, 10);
            var client1 = new RestClientFactory<RestResponse>().Create(asyncPolicy2);
            var request1 = new RestRequest();
            
            client1.RestClientOptions.BaseUrl = new Uri("https://httpstat.us/500");
            var host = client1.Build(client1.RestClientOptions);
            var response5 = await host.ExecuteAsync(request1);
            Console.ReadKey();
```

About v107 branch
            
**NOT fully tested, Not recommended for  production use unless you know what you're doing. :)**
btw official RestSharp 107.3.0 is not compatible with .net45 /.net46 /.net47, so [RestSharpPolly](https://github.com/yuessir/RestSharpPolly) does not support for the frameworks.

