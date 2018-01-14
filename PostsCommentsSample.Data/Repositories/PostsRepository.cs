using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;

namespace PostsCommentsSample.Data.Repositories
{
	public interface IPostsRepository
	{
		Task<Post> GetPostById(int postId);

		Task<List<Post>> GetPosts(PostsFilter filter);

		Task CreatePost(Post post);

		Task UpdatePost(Post post);

		Task DeletePost(int postId);
	}

	public class PostsRepository : IPostsRepository
	{
		protected static readonly List<Post> _storage = new List<Post>
		#region Test Data
		{
			new Post
			{
				PostId = 1,
				Title = "Post 1",
				Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
				CreationDate = new DateTime(2017, 3, 6),
				OwnerName = "John Doe"
			},
			new Post
			{
				PostId = 2,
				Title = "Post 2",
				Content = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
				CreationDate = new DateTime(2017, 11, 26),
				LastUpdateDate = new DateTime(2017, 11, 27),
				OwnerName = "John Doe"
			},
			new Post
			{
				PostId = 3,
				Title = "Untitled Post",
				Content = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
				CreationDate = new DateTime(2018, 1, 1),
				LastUpdateDate = new DateTime(2018, 1, 3),
				OwnerName = "User1"
			},
			new Post
			{
				PostId = 5,
				Title = "New Post",
				Content = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
				CreationDate = DateTime.UtcNow.AddDays(-1),
				OwnerName = "User1"
			},
			new Post
			{
				PostId = 6,
				Title = "Another brilliant Post",
				Content = "It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like)",
				CreationDate = DateTime.UtcNow.AddDays(-3),
				LastUpdateDate = DateTime.UtcNow.AddHours(-17),
				OwnerName = "NewUser"
			},
		};
		#endregion Test Data

		public Task<Post> GetPostById(int postId)
		{
			return Task.FromResult(_storage.FirstOrDefault(p => p.PostId == postId));
		}

		public Task<List<Post>> GetPosts(PostsFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException(nameof(filter));

			IEnumerable<Post> result = _storage.OrderByDescending(p => p.CreationDate);

			if (filter.StartDate.HasValue)
				result = result.Where(p => p.CreationDate > filter.StartDate.Value);

			if (filter.EndDate.HasValue)
				result = result.Where(p => p.CreationDate <= filter.EndDate.Value);

			result = result.Skip(filter.PageSize * filter.PageIndex).Take(filter.PageSize);

			return Task.FromResult(result.ToList());
		}

		public Task CreatePost(Post post)
		{
			post.PostId = _storage.Any() ? (_storage.Max(p => p.PostId) + 1) : 1;
			post.CreationDate = DateTime.UtcNow;

			_storage.Add(post);

			return Task.CompletedTask;
		}

		public Task UpdatePost(Post post)
		{
			var existingPost = _storage.FirstOrDefault(p => p.PostId == post.PostId);
			if (existingPost != null)
			{
				existingPost.LastUpdateDate = DateTime.UtcNow;
				existingPost.Content = post.Content;
				existingPost.Title = post.Title;
			}

			return Task.CompletedTask;
		}

		public Task DeletePost(int postId)
		{
			_storage.RemoveAll(p => p.PostId == postId);
			return Task.CompletedTask;
		}
	}
}
