# RestSharpPolly
RestSharp with Polly
Wrapping the RestClient with Polly framework.
Usage:

````csharp
 var client = RestClientFactory<IRestResponse>.Create(TimeoutAndRetryPolicy.Build(3, 10, 60));
            client.BaseUrl = new Uri("");
            var request = new RestRequest(Method.GET);
            request.AddJsonBody(model);
            var response = client.Execute(request);
````

The class RestClientFactory is generic.
````csharp
RestClientFactory<TResult> Create(ISyncPolicy<TResult> syncPolicy)
````

# Install
Import RestSharpPolly into an existing project

    Go to the project folder of the application and install the Nuget package reference

````command
     $ dotnet add package RestSharpPolly
````
    


