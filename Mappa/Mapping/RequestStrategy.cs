using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;
using Castle.DynamicProxy;
using Mappa.Domain;
using Mappa.Interceptor;
using Mappa.Strategy;
using Mappa.Streotype;
using Mappa.Streotype.Path;
using Mappa.Streotype.Query;
using Mappa.Streotype.Route;
using Newtonsoft.Json;

namespace Mappa.Mapping;

public class RequestStrategy : IHttpStrategy
{
    private readonly Dictionary<string, object> _pathParams = new();
    private readonly Dictionary<string, object> _queryParams = new();

    public async Task HandleAsync(IInvocation invocation, string baseUrl, List<PreFilter> preFilters,
        List<PostFilter> postFilters)
    {
        _pathParams.Clear();
        _queryParams.Clear();
        try
        {
            RouteAttribute? customAttribute = invocation.Method.GetCustomAttribute<RouteAttribute>();
            if (customAttribute != null && customAttribute.MethodType == MethodType.GET)
            {
                baseUrl += customAttribute.Path;
                Console.WriteLine("BEFORE EXECUTION: BASE URL " + baseUrl);
                ParameterInfo[] parameters = invocation.Method.GetParameters();

                foreach (var parameter in parameters)
                {
                    PathParamAttribute? attribute = parameter.GetCustomAttribute<PathParamAttribute>();
                    if (attribute != null)
                    {
                        _pathParams.Add(attribute.Id, invocation.Arguments[parameter.Position]);
                        Console.WriteLine("VALUE: " + invocation.Arguments[parameter.Position]);
                    }

                    QueryParamAttribute? queryAttribute = parameter.GetCustomAttribute<QueryParamAttribute>();
                    if (queryAttribute != null)
                    {
                        _queryParams.Add(queryAttribute.Id, invocation.Arguments[parameter.Position]);
                    }
                }

                foreach (var pathEntry in _pathParams)
                {
                    Console.WriteLine("Key " + pathEntry.Key + " Value " + pathEntry.Value);
                    baseUrl = baseUrl.Replace("{" + pathEntry.Key + "}", pathEntry.Value.ToString());
                }

                if (_queryParams.Count > 0)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    foreach (var queryEntry in _queryParams)
                    {
                        query[queryEntry.Key] = queryEntry.Value.ToString();
                    }

                    baseUrl += "?" + query.ToString();
                }

                // Extract Path, Query, Headers... [omitting this for brevity]

                Console.WriteLine("AFTER EXECUTION: BASE URL " + baseUrl);

                // Extract Path, Query, Headers... [omitting this for brevity]

                using (HttpClient client = new HttpClient())
                {
                    Type returnType = invocation.Method.ReturnType;

                    HttpResponseMessage response = await client.GetAsync(baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Type payloadType =
                            returnType.GetGenericArguments()[0]; // This should give you ApiResponse<List<User>>
                        Type actualPayloadType =
                            payloadType.GetGenericArguments()[0]; // This should give you List<User>

                        var deserializedPayload = JsonConvert.DeserializeObject(responseBody, actualPayloadType);

                        Type genericApiResponse = typeof(ApiResponse<>).MakeGenericType(actualPayloadType);
                        dynamic apiResponse = Activator.CreateInstance(genericApiResponse);
                        apiResponse.IsSuccess = response.IsSuccessStatusCode;
                        apiResponse.Content = deserializedPayload;

                        invocation.ReturnValue = Task.FromResult(apiResponse);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            // Consider how to handle this exception or propagate it further...
        }
    }

    public void Handle(IInvocation invocation, string baseUrl, List<PreFilter> preFilters, List<PostFilter> postFilters)
    {
        _pathParams.Clear();
        _queryParams.Clear();
        try
        {
            RouteAttribute? customAttribute = invocation.Method.GetCustomAttribute<RouteAttribute>();
            if (customAttribute != null && customAttribute.MethodType == MethodType.GET)
            {
                baseUrl += customAttribute.Path;
                Console.WriteLine("BEFORE EXECUTION: BASE URL " + baseUrl);
                ParameterInfo[] parameters = invocation.Method.GetParameters();

                foreach (var parameter in parameters)
                {
                    PathParamAttribute? attribute = parameter.GetCustomAttribute<PathParamAttribute>();
                    if (attribute != null)
                    {
                        _pathParams.Add(attribute.Id, invocation.Arguments[parameter.Position]);
                        Console.WriteLine("VALUE: " + invocation.Arguments[parameter.Position]);
                    }

                    QueryParamAttribute? queryAttribute = parameter.GetCustomAttribute<QueryParamAttribute>();
                    if (queryAttribute != null)
                    {
                        _queryParams.Add(queryAttribute.Id, invocation.Arguments[parameter.Position]);
                    }
                }

                foreach (var pathEntry in _pathParams)
                {
                    Console.WriteLine("Key " + pathEntry.Key + " Value " + pathEntry.Value);
                    baseUrl = baseUrl.Replace("{" + pathEntry.Key + "}", pathEntry.Value.ToString());
                }

                if (_queryParams.Count > 0)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    foreach (var queryEntry in _queryParams)
                    {
                        query[queryEntry.Key] = queryEntry.Value.ToString();
                    }

                    baseUrl += "?" + query.ToString();
                }

                // Extract Path, Query, Headers... [omitting this for brevity]

                Console.WriteLine("AFTER EXECUTION: BASE URL " + baseUrl);


                using (HttpClient client = new HttpClient())
                {
                    Type returnType = invocation.Method.ReturnType;

                    HttpResponseMessage response = client.GetAsync(baseUrl).Result; // Synchronous call
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result; // Synchronous call
                        Type payloadType =
                            returnType.GetGenericArguments()[0]; // This should give you ApiResponse<List<User>>
                        Type actualPayloadType =
                            payloadType.GetGenericArguments()[0]; // This should give you List<User>

                        var deserializedPayload = JsonConvert.DeserializeObject(responseBody, actualPayloadType);

                        Console.WriteLine(deserializedPayload);

                        Type genericApiResponse = typeof(ApiResponse<>).MakeGenericType(actualPayloadType);
                        dynamic apiResponse = Activator.CreateInstance(genericApiResponse);


                        apiResponse.IsSuccess = response.IsSuccessStatusCode;
                        apiResponse.Content = deserializedPayload;

                        invocation.ReturnValue = apiResponse; // No longer a Task
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            // Handle the exception or rethrow it as needed
            throw;
        }
    }


    public object CreateGenericApiResponse(Type typeOfT)
    {
        // Construct the generic type ApiResponse<T>
        Type genericType = typeof(ApiResponse<>).MakeGenericType(typeOfT);

        // Create an instance of ApiResponse<T>
        return Activator.CreateInstance(genericType);
    }
}