namespace Mappa.Streotype.Route;

[AttributeUsage(AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public string Path { get; set; }
    public MethodType MethodType { get; set; }

    public RouteAttribute(string path, MethodType methodType)
    {
        this.Path = path;
        this.MethodType = methodType;
    }
}