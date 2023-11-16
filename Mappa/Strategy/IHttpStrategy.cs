using Castle.DynamicProxy;
using Mappa.Interceptor;

namespace Mappa.Strategy;

public interface IHttpStrategy
{ 
    Task HandleAsync(IInvocation invocation, string baseUrl, List<PreFilter> preFilters, List<PostFilter> postFilters);
    void Handle(IInvocation invocation, string baseUrl, List<PreFilter> preFilters, List<PostFilter> postFilters);
}