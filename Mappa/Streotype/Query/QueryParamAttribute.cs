namespace Mappa.Streotype.Query;

[AttributeUsage(AttributeTargets.Parameter)]
public class QueryParamAttribute : Attribute
{
    public string Id { get; set; }

    public QueryParamAttribute(string id)
    {
        Id = id;
    }
}