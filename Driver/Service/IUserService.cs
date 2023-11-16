using Driver.Model;
using Mappa.Domain;
using Mappa.Streotype;
using Mappa.Streotype.Body;
using Mappa.Streotype.Path;
using Mappa.Streotype.Route;

namespace Driver.Service;

[Api]
public interface IUserService
{
    [Route(path: "/users", MethodType.GET)]
    Task<ApiResponse<List<User>>> FindAll();

    [Route(path: "/users/{id}", MethodType.GET)]
    Task<ApiResponse<User>> FindById([PathParam("id")] int id);

    [Route(path: "/users}", MethodType.GET)]
    Task<ApiResponse<User>> SignUp([RequestBody] User user);

}