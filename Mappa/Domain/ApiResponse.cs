namespace Mappa.Domain;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }

    public T Payload
    {
        get { return (T)Content; }
    }

    public dynamic Content { get; set; }
}