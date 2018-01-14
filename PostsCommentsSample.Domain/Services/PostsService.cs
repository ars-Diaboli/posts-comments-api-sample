using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;
using PostsCommentsSample.Data.Repositories;

namespace PostsCommentsSample.Domain.Services
{
	public interface IPostsService
	{
		Task<Post> GetPostById(int postId);

		Task<List<Post>> GetPosts(PostsFilter filter);

		Task CreatePost(Post post);

		Task UpdatePost(int postId, Post post);

		Task DeletePost(int postId);
	}

	public class PostsService : IPostsService
	{
		private readonly IPostsRepository _postsRepository;
		private readonly ICommentsRepository _commentsRepository;

		public PostsService(IPostsRepository postsRepository, ICommentsRepository commentsRepository)
		{
			_postsRepository = postsRepository;
			_commentsRepository = commentsRepository;
		}

		public Task<Post> GetPostById(int postId)
		{
			return _postsRepository.GetPostById(postId);
		}

		public Task<List<Post>> GetPosts(PostsFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException(nameof(filter));

			return _postsRepository.GetPosts(filter);
		}

		public Task CreatePost(Post post)
		{
			return _postsRepository.CreatePost(post);
		}

		public Task UpdatePost(int postId, Post post)
		{
			post.PostId = postId;
			return _postsRepository.UpdatePost(post);
		}

		public async Task DeletePost(int postId)
		{
			await _commentsRepository.DeletePostComments(postId);
			await _postsRepository.DeletePost(postId);
		}
	}
}
