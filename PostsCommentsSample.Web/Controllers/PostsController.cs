using Microsoft.AspNetCore.Mvc;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Domain.Services;
using PostsCommentsSample.Web.Framework;
using PostsCommentsSample.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsCommentsSample.Web.Controllers
{
	[Route("api/[controller]")]
	public class PostsController : Controller
	{
		private readonly IPostsService _postsService;

		public PostsController(IPostsService postsService)
		{
			_postsService = postsService;
		}

		// GET api/posts/1
		[HttpGet("{id}")]
		public async Task<PostViewModel> Get(int id)
		{
			var post = await _postsService.GetPostById(id);
			return post != null ? map(post) : null;
		}

		// GET api/posts
		[HttpGet]
		public async Task<IEnumerable<PostViewModel>> Get(PostsFilterModel filterModel)
		{
			var filter = map(filterModel);
			var posts = await _postsService.GetPosts(filter);
			return posts.Select(map);
		}

		// POST api/posts
		[HttpPost]
		[ValidateModel]
		public Task Post([FromBody]PostViewModel post)
		{
			return _postsService.CreatePost(map(post));
		}

		// PUT api/posts/5
		[HttpPut("{id}")]
		[ValidateModel]
		public Task Put(int id, [FromBody]PostViewModel post)
		{
			return _postsService.UpdatePost(id, map(post));
		}

		// DELETE api/posts/5
		[HttpDelete("{id}")]
		public Task Delete(int id)
		{
			return _postsService.DeletePost(id);
		}

		private static PostsFilter map(PostsFilterModel source)
		{
			var destination = new PostsFilter();

			destination.PageSize = source.PageSize;
			destination.PageIndex = source.PageNumber - 1;
			destination.PostIds = source.PostIds;
			destination.StartDate = source.StartDate;
			destination.EndDate = source.EndDate;
			destination.OwnerName = source.OwnerName;

			return destination;
		}

		private static Post map(PostViewModel source)
		{
			var destination = new Post();

			destination.PostId = source.PostId;
			destination.Title = source.Title;
			destination.Content = source.Content;
			destination.CreationDate = source.CreationDate;
			destination.OwnerName = source.OwnerName;

			return destination;
		}

		private static PostViewModel map(Post source)
		{
			var destination = new PostViewModel();

			destination.PostId = source.PostId;
			destination.Title = source.Title;
			destination.Content = source.Content;
			destination.CreationDate = source.CreationDate;
			destination.LastUpdateDate = source.LastUpdateDate;
			destination.OwnerName = source.OwnerName;

			return destination;
		}

	}
}
