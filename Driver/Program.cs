using System.Diagnostics;
using Driver.Model;
using Driver.Service;
using Mappa;
using Mappa.Domain;

HttpManager httpManager = new HttpManager.Builder()
    .BaseUrl("https://jsonplaceholder.typicode.com")
    .Build();

var postService =
    httpManager.CreateService<IPostService>(); // this postService could be used later so do not lose instance 


Post p = new Post();
p.Id = 1;
p.Title = "Game";
p.Body = "sbjhsdf";
p.UserId = 1;

var postListResponse = postService.RegisterUser(p);
Console.WriteLine(postListResponse.Result.Content);

// Post Service
// var postService = httpManager.CreateService<IPostService>();
// Task<ApiResponse<List<Comment>>> commentService = postService.FindById(1, "comments");
// Task<ApiResponse<List<Post>>> postListResponse = postService.FindByCommentsByPostId(1);
// ApiResponse<List<Post>> response = postListResponse.Result;
//
// foreach (Post post in response.Payload)
// {
//     Console.WriteLine("==================================================================");
//     Console.WriteLine(post);
// }

// ApiResponse<List<Comment>> serviceResult = commentService.Result;
//
// List<Comment> postList = serviceResult.Payload;
// foreach (Comment post in postList)
// {
//     Console.WriteLine(post);
// }
//
// ApiResponse<List<Post>> postList = postListResponse.Result;
// foreach (Post post in postList.Payload)
// {
//     Debug.WriteLine(post);
// }