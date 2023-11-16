using Castle.DynamicProxy;
using Mappa.Interceptor;

namespace Mappa;

public class HttpManager
{
    private string BaseUrl { get; }

    private HttpManager(Builder builder)
    {
        BaseUrl = builder.RootUrl;
    }

    public class Builder
    {
        public string RootUrl = string.Empty;

        public HttpManager Build()
        {
            return new HttpManager(this);
        }

        public Builder BaseUrl(string baseUrl)
        {
            RootUrl = baseUrl;
            return this;
        }
    }

    public T CreateService<T>() where T : class
    {
        if (!typeof(T).IsInterface)
        {
            throw new IllegalServiceArgException("Type should be an interface to create a service.");
        }

        // Proxy Object From an Interface
        ProxyGenerator proxyGenerator = new ProxyGenerator();
        IInterceptor interceptor = new ExecutionInterceptor(BaseUrl);
        return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
    }
}