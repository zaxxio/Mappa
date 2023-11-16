namespace Mappa.Streotype.Path;

[AttributeUsage(AttributeTargets.Parameter)]
public class PathParamAttribute : Attribute
{
    public string Id { get; set; }

    public PathParamAttribute(string id)
    {
        Id = id;
    }
}