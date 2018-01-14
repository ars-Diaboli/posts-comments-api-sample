using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostsCommentsSample.Data.Filters;
using PostsCommentsSample.Data.Models;

namespace PostsCommentsSample.Data.Repositories
{
	public interface ICommentsRepository
	{
		Task<Comment> GetCommentById(int commentId);

		Task<List<Comment>> GetComments(CommentsFilter filter);

		Task CreateComment(Comment comment);

		Task UpdateComment(Comment comment);

		Task DeleteComment(int commentId);

		Task DeletePostComments(int postId);
	}

	public class CommentsRepository	: ICommentsRepository
	{
		protected static readonly List<Comment> _storage = new List<Comment>
		#region Test Data
		{
			new Comment
			{
				CommentId = 1,
				PostId = 1,
				CreationDate = DateTime.UtcNow.AddMinutes(-1345),
				Text = "I'm first commenter!",
				OwnerName = "NewUser",
			},
			new Comment
			{
				CommentId = 2,
				PostId = 1,
				CreationDate = DateTime.UtcNow.AddMinutes(-1111),
				Text = "comment",
				OwnerName = "NewUser",
			},
			new Comment
			{
				CommentId = 3,
				PostId = 1,
				CreationDate = DateTime.UtcNow.AddMinutes(-1212),
				Text = "comment",
				OwnerName = "User1",
			},
			new Comment
			{
				CommentId = 4,
				PostId = 3,
				CreationDate = DateTime.UtcNow.AddMinutes(-765),
				Text = "comment",
				OwnerName = "John Doe",
			},
			new Comment
			{
				CommentId = 5,
				PostId = 5,
				CreationDate = DateTime.UtcNow.AddMinutes(-654),
				Text = "comment",
				OwnerName = "User1",
			},
			new Comment
			{
				CommentId = 7,
				PostId = 6,
				CreationDate = DateTime.UtcNow.AddMinutes(-543),
				Text = "comment",
				OwnerName = "NewUser",
			},
		};
		#endregion Test Data

		public Task<Comment> GetCommentById(int commentId)
		{
			return Task.FromResult(_storage.FirstOrDefault(c => c.CommentId == commentId));
		}

		public Task<List<Comment>> GetComments(CommentsFilter filter)
		{
			IEnumerable<Comment> result = _storage.OrderByDescending(c => c.CreationDate);

			if (filter.PostId.HasValue)
				result = result.Where(c => c.PostId == filter.PostId.Value);

			if (filter.StartDate.HasValue)
				result = result.Where(c => c.CreationDate > filter.StartDate.Value);

			if (filter.EndDate.HasValue)
				result = result.Where(c => c.CreationDate <= filter.EndDate.Value);

			result = result.Skip(filter.PageSize * filter.PageIndex).Take(filter.PageSize);

			return Task.FromResult(result.ToList());
		}

		public Task CreateComment(Comment comment)
		{
			comment.CommentId = _storage.Any() ? (_storage.Max(c => c.CommentId) + 1) : 1;
			comment.CreationDate = DateTime.UtcNow;

			_storage.Add(comment);

			return Task.CompletedTask;
		}

		public Task UpdateComment(Comment comment)
		{
			var existingComment = _storage.FirstOrDefault(c => c.CommentId == comment.CommentId);
			if (existingComment != null)
			{
				existingComment.Text = comment.Text;
			}

			return Task.CompletedTask;
		}

		public Task DeleteComment(int commentId)
		{
			_storage.RemoveAll(c => c.CommentId == commentId);
			return Task.CompletedTask;
		}

		public Task DeletePostComments(int postId)
		{
			_storage.RemoveAll(c => c.PostId == postId);
			return Task.CompletedTask;
		}
	}
}
