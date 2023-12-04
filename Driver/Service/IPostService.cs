using Driver.Model;
using Mappa.Domain;
using Mappa.Streotype;
using Mappa.Streotype.Body;
using Mappa.Streotype.Path;
using Mappa.Streotype.Query;
using Mappa.Streotype.Route;

namespace Driver.Service;

[Api]
public interface IPostService
{
    [Route(path: "/posts/{id}/{path}", MethodType.GET)]
    Task<ApiResponse<List<Comment>>> FindById([PathParam("id")] int id, [PathParam("path")] string path);

    [Route(path: "/posts", MethodType.POST)]
    Task<ApiResponse<List<Post>>> FindAll();

    [Route(path: "/posts", MethodType.POST)]
    Task<ApiResponse<Post>> RegisterUser([RequestBody] Post post);

    [Route(path: "/comments", MethodType.GET)]
    Task<ApiResponse<List<Post>>> FindByCommentsByPostId([QueryParam("postId")] int postId);
}