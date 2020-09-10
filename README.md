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
Import Ant Design Blazor into an existing project

    Go to the project folder of the application and install the Nuget package reference

    $ dotnet add package RestSharpPolly


