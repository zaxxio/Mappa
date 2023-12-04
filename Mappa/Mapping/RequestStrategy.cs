using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;
using Castle.DynamicProxy;
using Mappa.Domain;
using Mappa.Interceptor;
using Mappa.Strategy;
using Mappa.Streotype;
using Mappa.Streotype.Body;
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
            if (customAttribute != null)
            {
                baseUrl += customAttribute.Path;

                using (HttpClient client = new HttpClient())
                {
                    // Handling GET requests
                    if (customAttribute.MethodType == MethodType.GET)
                    {
                        ExtractParameters(invocation, _pathParams, _queryParams);
                        AppendPathAndQueryParameters(ref baseUrl, _pathParams, _queryParams);
                        HttpResponseMessage response = await client.GetAsync(baseUrl);
                        // Process GET response
                        await ProcessResponseAsync(invocation, response);
                    }
                    // Handling POST requests
                    else if (customAttribute.MethodType == MethodType.POST)
                    {
                        ExtractParameters(invocation, _pathParams, _queryParams);
                        AppendPathAndQueryParameters(ref baseUrl, _pathParams, _queryParams);
                        Object requestBody = ExtractRequestBody(invocation);
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8,
                            "application/json");

                        HttpResponseMessage response = await client.PostAsync(baseUrl, jsonContent);
                        // Process POST response
                        await ProcessResponseAsync(invocation, response);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public void Handle(IInvocation invocation, string baseUrl, List<PreFilter> preFilters, List<PostFilter> postFilters)
    {
    }

    private void ExtractParameters(IInvocation invocation, Dictionary<string, object> pathParams,
        Dictionary<string, object> queryParams)
    {
        ParameterInfo[] parameters = invocation.Method.GetParameters();
        foreach (var parameter in parameters)
        {
            PathParamAttribute? pathAttribute = parameter.GetCustomAttribute<PathParamAttribute>();
            if (pathAttribute != null)
            {
                pathParams.Add(pathAttribute.Id, invocation.Arguments[parameter.Position]);
            }

            QueryParamAttribute? queryAttribute = parameter.GetCustomAttribute<QueryParamAttribute>();
            if (queryAttribute != null)
            {
                queryParams.Add(queryAttribute.Id, invocation.Arguments[parameter.Position]);
            }
        }
    }

    private void AppendPathAndQueryParameters(ref string baseUrl, Dictionary<string, object> pathParams,
        Dictionary<string, object> queryParams)
    {
        foreach (var pathEntry in pathParams)
        {
            baseUrl = baseUrl.Replace("{" + pathEntry.Key + "}", pathEntry.Value.ToString());
        }

        if (queryParams.Count > 0)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryEntry in queryParams)
            {
                query[queryEntry.Key] = queryEntry.Value.ToString();
            }

            baseUrl += "?" + query.ToString();
        }
    }

    private Object ExtractRequestBody(IInvocation invocation)
    {
        ParameterInfo[] parameters = invocation.Method.GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].GetCustomAttribute<RequestBodyAttribute>() != null)
            {
                return invocation.Arguments[i];
            }
        }

        return null; // Or throw an exception if a request body is mandatory
    }


    private async Task ProcessResponseAsync(IInvocation invocation, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Type returnType = invocation.Method.ReturnType;
            Type payloadType = returnType.GetGenericArguments()[0]; // Assuming Task<ApiResponse<T>>
            Type actualPayloadType =
                payloadType.GetGenericArguments()[0]; // This should give you List<User>


            var deserializedPayload = JsonConvert.DeserializeObject(responseBody, actualPayloadType);

            Type genericApiResponse = typeof(ApiResponse<>).MakeGenericType(actualPayloadType);
            dynamic apiResponse = Activator.CreateInstance(genericApiResponse);
            apiResponse.IsSuccess = response.IsSuccessStatusCode;
            apiResponse.Content = deserializedPayload;

            invocation.ReturnValue = Task.FromResult(apiResponse);
        }
        else
        {
            // Handle non-successful response
        }
    }
}