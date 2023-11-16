namespace Driver.Model;

public class Post
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }

    public override string ToString()
    {
        return $"UserId: {UserId}, Id: {Id}, Title: {Title}, Body: {Body}";
    }
}
