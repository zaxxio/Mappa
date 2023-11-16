using System.Diagnostics;
using Castle.DynamicProxy;
using Mappa.Mapping;
using Mappa.Strategy;

namespace Mappa.Interceptor;

public class ExecutionInterceptor : IInterceptor
{
    private readonly IHttpStrategy _httpStrategy;
    private readonly string BaseUrl;

    public ExecutionInterceptor(string baseUrl)
    {
        BaseUrl = baseUrl;
        _httpStrategy = new RequestStrategy();
    }

    public void Intercept(IInvocation invocation)
    {
        Debug.WriteLine("Before Execution"); 
        _httpStrategy.HandleAsync(invocation, BaseUrl, new List<PreFilter>(), new List<PostFilter>()).GetAwaiter()
            .GetResult();

        // _httpStrategy.Handle(invocation, BaseUrl, new List<PreFilter>(), new List<PostFilter>());
        Debug.WriteLine(invocation.ReturnValue);
        Debug.WriteLine("After Execution");
    }
}